// Worker-based persistence for GitHub Pages (no COOP/COEP/OPFS required)
// - Uses sqlite3-worker1-promiser to drive sqlite3 inside a Worker
// - Restores DB from IndexedDB on init
// - Exposes globalThis.saveDatabase() that exports DB bytes from the worker and saves to IndexedDB

// Requires these files to be available alongside this script:
// - sqlite3-worker1.js
// - sqlite3-worker1-promiser.js

import './sqlite3-worker1-promiser.js';

// After sqlite-storage.js has loaded
// initialization is performed via loadSQLite() called from main.js

// Keep a safe no-op immediately so .NET callers won't crash while initialization runs:
globalThis.saveDatabase = async () => {
    console.warn('saveDatabase called before sqlite persistence was initialized; no-op.');
};

// IndexedDB helpers
export const IDB_DB = 'avalonia-sqlite3';
export const IDB_STORE = 'dbfiles';
export const IDB_KEY = 'todo.db';

export function idbOpen() {
    return new Promise((resolve, reject) => {
        const req = indexedDB.open(IDB_DB, 1);
        req.onupgradeneeded = () => req.result.createObjectStore(IDB_STORE);
        req.onsuccess = () => resolve(req.result);
        req.onerror = () => reject(req.error);
    });
}

export async function idbPut(key, uint8) {
    const db = await idbOpen();
    return new Promise((resolve, reject) => {
        const tx = db.transaction(IDB_STORE, 'readwrite');
        const store = tx.objectStore(IDB_STORE);
        // store a plain ArrayBuffer or Uint8Array; IndexedDB will persist either
        const value = (uint8 instanceof Uint8Array) ? uint8.slice().buffer : uint8;
        const req = store.put(value, key);
        req.onsuccess = () => resolve();
        req.onerror = () => reject(req.error);
    });
}

export async function idbGet(key) {
    const db = await idbOpen();
    return new Promise((resolve, reject) => {
        const tx = db.transaction(IDB_STORE, 'readonly');
        const store = tx.objectStore(IDB_STORE);
        const req = store.get(key);
        req.onsuccess = () => {
            const v = req.result;
            if (!v) return resolve(null);
            // IndexedDB may return an ArrayBuffer; normalize to Uint8Array
            resolve(v instanceof ArrayBuffer ? new Uint8Array(v) : (v instanceof Uint8Array ? v : new Uint8Array(v)));
        };
        req.onerror = () => reject(req.error);
    });
}

// Initialize worker + restore DB, then wire saveDatabase().
export async function loadSQLite() {
    console.log('Initializing SQLite Worker + persistence (GitHub Pages fallback)...');

    // Wait for the promiser factory (v2 returns a Promise that resolves with the promiser)
    if (typeof globalThis.sqlite3Worker1Promiser?.v2 !== 'function') {
        console.warn('sqlite3Worker1Promiser.v2() not found. Falling back to in-thread initialization is required.');
        return;
    }

    let promiser;
    try {
        console.log('Requesting sqlite3Worker1Promiser.v2...');
        
        // Timeout to prevent infinite hang
        const timeoutPromise = new Promise((_, reject) => 
            setTimeout(() => reject(new Error('Timeout waiting for sqlite3 worker to initialize')), 15000)
        );

        // This will create a Worker that loads sqlite3-worker1.js (which in turn loads sqlite3.js)
        const promiserPromise = globalThis.sqlite3Worker1Promiser.v2({
            // Ensure we use the correct path to the worker
            worker: () => {
                const url = new URL('sqlite3-worker1.js', import.meta.url).href;
                console.log('Creating worker with URL:', url);
                const w = new Worker(url);
                w.onerror = (e) => console.error('Worker error event:', e);
                return w;
            }
        });

        promiser = await Promise.race([promiserPromise, timeoutPromise]);
        console.log('sqlite3Worker1Promiser.v2 resolved.');
    } catch (err) {
        console.error('Failed to start sqlite3 worker promiser:', err);
        return;
    }

    // Helper to post a promiser call and return its response object
    const callWorker = async (msg) => {
        try {
            const r = await promiser(msg);
            return r; // r is the raw object the worker posted back
        } catch (e) {
            console.error('Worker call failed:', msg, e);
            throw e;
        }
    };

    // Restore saved DB (if any) into worker FS
    try {
        const saved = await idbGet(IDB_KEY);
        const DB_FILENAME = 'todo.db';
        
        if (saved && saved.byteLength > 0) {
            console.log('Found saved DB in IndexedDB (' + saved.byteLength + ' bytes) — restoring into worker...');
            // We use our custom 'upload' message to populate the worker's MEMFS
            const uploadResp = await callWorker({ 
                type: 'upload', 
                args: { filename: DB_FILENAME, deserialize: saved } 
            });
            console.log('DB bytes uploaded to worker VFS:', uploadResp);
        } else {
            console.log('No prior DB found in IndexedDB — worker will start with an empty DB.');
        }

        // send 'open' message to worker
        console.log('Sending open message for:', DB_FILENAME);
        const resp = await callWorker({ type: 'open', args: { filename: DB_FILENAME } });
        if (resp && resp.type === 'open') {
            console.log('DB opened in worker VFS:', resp);
        } else {
            console.warn('Worker open response (unexpected type):', resp);
        }
    } catch (e) {
        console.error('Error opening/restoring DB in worker. Full error object:', JSON.stringify(e, null, 2));
    }

    // Replace global saveDatabase with one that asks the worker to export the DB and then persists to IndexedDB
    globalThis.saveDatabase = async () => {
        try {
            const DB_FILENAME = 'todo.db';
            
            // Priority: Try to save from the main thread's .NET VFS first
            const runtime = globalThis.window?.dotnetRuntime;
            const dotnetFS = runtime && ((typeof runtime.getModule === 'function' ? runtime.getModule().FS : runtime.Module?.FS) || runtime.FS);
            if (dotnetFS) {
                try {
                    console.log('Saving DB: reading from .NET main thread VFS...');
                    const data = dotnetFS.readFile(DB_FILENAME);
                    if (data && data.byteLength > 0) {
                        await idbPut(IDB_KEY, data);
                        console.log('DB persisted from .NET VFS to IndexedDB (' + data.byteLength + ' bytes).');
                        return;
                    }
                } catch (vfsErr) {
                    console.warn('Failed to read DB from .NET VFS (might not be created yet):', vfsErr);
                }
            }

            console.log('Saving DB: fallback to requesting export from worker...');
            // Ask the worker to export the DB bytes back to us
            const resp = await callWorker({ type: 'export', args: { filename: DB_FILENAME } });
            // Expect the worker to respond with an object containing result.byteArray: Uint8Array
            const data = resp?.result?.byteArray;
            if (!data) {
                throw new Error('Worker did not return exported DB bytes (response: ' + JSON.stringify(resp) + ')');
            }
            const uint8 = (data instanceof Uint8Array) ? data : new Uint8Array(data);
            console.log('Received exported DB bytes from worker (' + uint8.byteLength + ' bytes). Persisting to IndexedDB...');
            await idbPut(IDB_KEY, uint8);
            console.log('DB persisted to IndexedDB.');
        } catch (err) {
            console.error('saveDatabase failed:', err);
            throw err;
        }
    };

    // Expose a small helper so other code can send arbitrary worker requests if needed
    window.__sqlite3WorkerPromiser = {
        call: (msg) => callWorker(msg)
    };

    console.log('SQLite worker initialized; manual persistence enabled (IndexedDB fallback).');
}

// Save Settings helper
export function setItem(key, value) {
    localStorage.setItem(key, value);
}

export function getItem(key) {
    return localStorage.getItem(key);
}