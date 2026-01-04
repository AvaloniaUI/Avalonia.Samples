import './jswasm/sqlite3.js';

export async function loadSQLite() {
    console.log('Initializing SQLite WASM...');

    const sqlite3 = await window.sqlite3InitModule({
        print: console.log,
        printErr: console.error
    });

    /* --------------------------------------------------
     * 1️⃣ Grab the *module‑specific* FS (not the Blazor one)
     * -------------------------------------------------- */
    const FS = sqlite3.Module?.FS ?? sqlite3.FS;
    if (!FS) {
        console.warn('Emscripten FS not available – persistence will be disabled.');
        return;
    }

    /* --------------------------------------------------
     * 2️⃣ Create the persistence directory & mount IDBFS
     * -------------------------------------------------- */
    const DATA_DIR = '/data';
    try { FS.mkdir(DATA_DIR); } catch { /* ignore if already exists */ }

    if (!FS.filesystems?.IDBFS) {
        console.warn('IDBFS not exposed by the Emscripten FS – persistence disabled.');
        return;
    }

    FS.mount(FS.filesystems.IDBFS, {}, DATA_DIR);

    /* --------------------------------------------------
     * 3️⃣ Load any existing data from IndexedDB
     * -------------------------------------------------- */
    await new Promise((resolve, reject) => {
        FS.syncfs(true, (err) => err ? reject(err) : resolve());
    });

    /* --------------------------------------------------
     * 4️⃣ Expose a helper for .NET to call
     * -------------------------------------------------- */
    globalThis.saveDatabase = async () => new Promise((resolve, reject) => {
        FS.syncfs(false, (err) => err ? reject(err) : resolve());
    });

    /* --------------------------------------------------
     * 5️⃣ Expose raw exports to Microsoft.Data.Sqlite
     * -------------------------------------------------- */
    window.SQLITE_MODULE = sqlite3.capi?.wasmExports ?? sqlite3.Module;

    console.log('SQLite WASM module loaded and persistence enabled.');
}