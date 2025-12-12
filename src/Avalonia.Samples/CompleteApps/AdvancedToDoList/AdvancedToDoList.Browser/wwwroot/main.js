import { dotnet } from './_framework/dotnet.js'

const is_browser = typeof window != "undefined";
if (!is_browser) throw new Error(`Expected to be running in a browser`);

const dotnetRuntime = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

// --- Persistence Setup ---
const DATA_DIR = '/data';

// Prefer global FS if exposed; fall back to Module.FS (older packs)
const FS = globalThis.FS ?? dotnetRuntime.Module?.FS;
const IDBFS = FS?.filesystems?.IDBFS;

if (FS && IDBFS) {
    try { FS.mkdir(DATA_DIR); } catch { /* ignore if exists */ }

    try {
        FS.mount(IDBFS, {}, DATA_DIR);
        // Sync from IndexedDB into the in-memory FS
        await new Promise((resolve, reject) => {
            FS.syncfs(true, (err) => {
                if (err) {
                    console.error('Error restoring data from IndexedDB:', err);
                    reject(err);
                } else {
                    console.log('Data restored from IndexedDB');
                    resolve();
                }
            });
        });
        // Expose save function for C# to call
        globalThis.saveDatabase = async () => new Promise((resolve, reject) => {
            FS.syncfs(false, (err) => {
                if (err) {
                    console.error('Error saving to IndexedDB:', err);
                    reject(err);
                } else {
                    console.log('Data saved to IndexedDB');
                    resolve();
                }
            });
        });
    } catch (e) {
        console.warn('Failed to mount IDBFS; falling back to no-op persistence:', e);
        globalThis.saveDatabase = async () => {};
    }
} else {
    console.warn('FS/IDBFS not available in this runtime; persistence will be disabled.');
    globalThis.saveDatabase = async () => {};
}
// -------------------------

const config = dotnetRuntime.getConfig();

await dotnetRuntime.runMain(config.mainAssemblyName, [globalThis.location.href]);
