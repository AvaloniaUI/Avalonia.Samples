import { dotnet } from './_framework/dotnet.js'
import {loadSQLite, idbGet, IDB_KEY} from "./sqlite-storage.js";

const is_browser = typeof window != "undefined";
if (!is_browser) throw new Error(`Expected to be running in a browser`);

await loadSQLite();

const dotnetRuntime = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

// Expose runtime for storage synchronization
window.dotnetRuntime = dotnetRuntime;

// Restore database into .NET VFS if it exists in IndexedDB
try {
    const savedBytes = await idbGet(IDB_KEY);
    if (savedBytes && savedBytes.byteLength > 0) {
        console.log(`Restoring database to .NET VFS (${savedBytes.byteLength} bytes)...`);
        // Try different ways to access FS as it might depend on the runtime version
        const FS = (typeof dotnetRuntime.getModule === 'function' ? dotnetRuntime.getModule().FS : dotnetRuntime.Module?.FS) || dotnetRuntime.FS;
        if (!FS) throw new Error("Could not find Emscripten FS in dotnetRuntime");
        FS.writeFile('todo.db', savedBytes);
        console.log('Database restored to .NET VFS.');
    } else {
        console.log('No saved database found in IndexedDB to restore to .NET VFS.');
    }
} catch (err) {
    console.error('Failed to restore database to .NET VFS:', err);
}

const config = dotnetRuntime.getConfig();

await dotnetRuntime.runMain(config.mainAssemblyName, [globalThis.location.href]);
