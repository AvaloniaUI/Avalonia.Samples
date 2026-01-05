// Worker-based persistence for GitHub Pages (no COOP/COEP/OPFS required)
// - Uses sqlite3-worker1-promiser to drive sqlite3 inside a Worker
// - Restores DB from IndexedDB on init
// - Exposes globalThis.saveDatabase() that exports DB bytes from the worker and saves to IndexedDB

// Requires these files to be available alongside this script:
// - sqlite3-worker1.js
// - sqlite3-worker1-promiser.js

// Keep a safe no-op immediately so .NET callers won't crash while initialization runs:
globalThis.saveDatabase = async () => {
    console.warn('saveDatabase called before sqlite persistence was initialized; no-op.');
};

// IndexedDB helpers
const IDB_DB = 'avalonia-sqlite3';
const IDB_STORE = 'dbfiles';
const IDB_KEY = 'todo.db';

function idbOpen() {
    return new Promise((resolve, reject) => {
        const req = indexedDB.open(IDB_DB, 1);
        req.onupgradeneeded = () => req.result.createObjectStore(IDB_STORE);
        req.onsuccess = () => resolve(req.result);
        req.onerror = () => reject(req.error);
    });
}

async function idbPut(key, uint8) {
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

async function idbGet(key) {
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
        // This will create a Worker that loads sqlite3-worker1.js (which in turn loads sqlite3.js)
        promiser = await sqlite3Worker1Promiser.v2();
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
        if (saved && saved.byteLength > 0) {
            console.log('Found saved DB in IndexedDB (' + saved.byteLength + ' bytes) — restoring into worker...');
            // send 'restore-db' message to worker; worker should write the bytes into its VFS
            // message shape: { type: 'restore-db', args: { path: '/data/todo.db', data: Uint8Array } }
            // note: promiser posts as structured clone (no transfer list); it's fine.
            const resp = await callWorker({ type: 'restore-db', args: { path: '/data/todo.db', data: saved } });
            if (resp && (resp.result === 'ok' || resp.ok)) {
                console.log('DB restored into worker VFS.');
            } else {
                console.warn('Worker restore-db response:', resp);
            }
        } else {
            console.log('No prior DB found in IndexedDB — worker will start with an empty DB.');
        }
    } catch (e) {
        console.error('Error restoring DB into worker:', e);
    }

    // Replace global saveDatabase with one that asks the worker to export the DB and then persists to IndexedDB
    globalThis.saveDatabase = async () => {
        try {
            console.log('Saving DB: requesting export from worker...');
            // Ask the worker to export the DB bytes back to us
            // message: { type: 'export-db', args: { path: '/data/todo.db' } }
            const resp = await callWorker({ type: 'export-db', args: { path: '/data/todo.db' } });
            // Expect the worker to respond with an object containing data: Uint8Array (or ArrayBuffer)
            const data = resp?.data ?? resp?.args?.data ?? resp?.resultData;
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