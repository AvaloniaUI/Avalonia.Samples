/*
  2022-05-23

  The author disclaims copyright to this source code.  In place of a
  legal notice, here is a blessing:

  *   May you do good and not evil.
  *   May you find forgiveness for yourself and forgive others.
  *   May you share freely, never taking more than you give.

  ***********************************************************************

  This is a JS Worker file for the main sqlite3 api. It loads
  sqlite3.js, initializes the module, and postMessage()'s a message
  after the module is initialized:

  {type: 'sqlite3-api', result: 'worker1-ready'}

  This seemingly superfluous level of indirection is necessary when
  loading sqlite3.js via a Worker. Instantiating a worker with new
  Worker("sqlite.js") will not (cannot) call sqlite3InitModule() to
  initialize the module due to a timing/order-of-operations conflict
  (and that symbol is not exported in a way that a Worker loading it
  that way can see it).  Thus JS code wanting to load the sqlite3
  Worker-specific API needs to pass _this_ file (or equivalent) to the
  Worker constructor and then listen for an event in the form shown
  above in order to know when the module has completed initialization.

  This file accepts a URL arguments to adjust how it loads sqlite3.js:

  - `sqlite3.dir`, if set, treats the given directory name as the
    directory from which `sqlite3.js` will be loaded.
*/

"use strict";
{
  const urlParams = globalThis.location
        ? new URL(globalThis.location.href).searchParams
        : new URLSearchParams();
  let theJs = 'sqlite3.js';
  if(urlParams.has('sqlite3.dir')){
    theJs = urlParams.get('sqlite3.dir') + '/' + theJs;
  }
  
  importScripts(theJs);
}
sqlite3InitModule().then(sqlite3 => sqlite3.initWorker1API());

// Patch to add restore/export handlers to sqlite3-worker1.js
// Place this in the Worker (after sqlite3.initWorker1API() completes).
// It expects `sqlite3` variable to be available in the worker scope.
  // make sqlite3 visible to message handler
  self.sqlite3 = sqlite3;

  // convenience aliases to C API helpers (if present)
  const capi = sqlite3.capi || {};

  self.onmessage = async (ev) => {
    const msg = ev.data;
    try {
      switch (msg.type) {
        case 'restore-db': {
          // args: { path, data: Uint8Array | ArrayBuffer }
          const { path = '/data/todo.db', data } = msg.args || {};
          if (!data) {
            postMessage({ messageId: msg.messageId, type: 'restore-db', result: 'error', error: 'no-data' });
            break;
          }
          // If capi.sqlite3_js_posix_create_file exists, use it (safer)
          if (capi.sqlite3_js_posix_create_file) {
            try {
              // Ensure path string and data as Uint8Array
              const u8 = (data instanceof Uint8Array) ? data : new Uint8Array(data);
              capi.sqlite3_js_posix_create_file(path, u8, u8.byteLength);
              postMessage({ messageId: msg.messageId, type: 'restore-db', result: 'ok' });
            } catch (e) {
              postMessage({ messageId: msg.messageId, type: 'restore-db', result: 'error', error: String(e) });
            }
          } else if (sqlite3.Module?.FS || sqlite3.FS) {
            try {
              const FS = sqlite3.Module?.FS ?? sqlite3.FS;
              try { FS.mkdir('/data'); } catch (e) { /* ignore */ }
              FS.writeFile(path, (data instanceof Uint8Array) ? data : new Uint8Array(data), { encoding: 'binary' });
              postMessage({ messageId: msg.messageId, type: 'restore-db', result: 'ok' });
            } catch (e) {
              postMessage({ messageId: msg.messageId, type: 'restore-db', result: 'error', error: String(e) });
            }
          } else {
            postMessage({ messageId: msg.messageId, type: 'restore-db', result: 'error', error: 'no-fs-or-capi' });
          }
          break;
        }
        case 'export-db': {
          // args: { path }
          const path = (msg.args && msg.args.path) || '/data/todo.db';
          // Try capi.sqlite3_js_db_export(0) first (some builds accept 0 as main db)
          if (capi.sqlite3_js_db_export) {
            try {
              const exported = capi.sqlite3_js_db_export(0); // expected Uint8Array
              postMessage({ messageId: msg.messageId, type: 'export-db', result: 'ok', data: exported });
            } catch (e) {
              // fallback to reading the file from FS
              try {
                const FS = sqlite3.Module?.FS ?? sqlite3.FS;
                const data = FS.readFile(path, { encoding: 'binary' });
                const u8 = data instanceof Uint8Array ? data : new Uint8Array(data);
                postMessage({ messageId: msg.messageId, type: 'export-db', result: 'ok', data: u8 });
              } catch (e2) {
                postMessage({ messageId: msg.messageId, type: 'export-db', result: 'error', error: String(e2) });
              }
            }
          } else if (sqlite3.Module?.FS || sqlite3.FS) {
            try {
              const FS = sqlite3.Module?.FS ?? sqlite3.FS;
              const data = FS.readFile(path, { encoding: 'binary' });
              const u8 = data instanceof Uint8Array ? data : new Uint8Array(data);
              postMessage({ messageId: msg.messageId, type: 'export-db', result: 'ok', data: u8 });
            } catch (e) {
              postMessage({ messageId: msg.messageId, type: 'export-db', result: 'error', error: String(e) });
            }
          } else {
            postMessage({ messageId: msg.messageId, type: 'export-db', result: 'error', error: 'no-fs-or-capi' });
          }
          break;
        }
        default:
          // Let the existing worker (sqlite3.initWorker1API()) handle other messages
          // (do nothing here so other handlers can process)
          break;
      }
    } catch (ex) {
      postMessage({ messageId: msg.messageId, type: msg.type, result: 'error', error: String(ex) });
    }
  };
});