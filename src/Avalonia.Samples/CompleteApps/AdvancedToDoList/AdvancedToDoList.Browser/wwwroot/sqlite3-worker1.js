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

   // sqlite3-worker1.js
   importScripts('./sqlite3.js');
   if (globalThis.sqlite3InitModule) {
       globalThis.sqlite3InitModule().then(sqlite3 => {
           if (sqlite3.initWorker1API) {
               sqlite3.initWorker1API();

               // Wrap the default onmessage to handle custom 'upload' type
               const oldOnMessage = globalThis.onmessage;
               globalThis.onmessage = async function(ev) {
                   const data = ev.data;
                   if (data && data.type === 'upload') {
                       const args = data.args;
                       try {
                           console.log('Worker: Handling upload for', args.filename, 'data size:', args.deserialize?.byteLength);
                           // Use the internal capi to create the file in MEMFS
                           sqlite3.capi.sqlite3_js_posix_create_file(args.filename, args.deserialize);
                           console.log('Worker: Upload successful for', args.filename);
                           globalThis.postMessage({
                               type: 'upload',
                               messageId: data.messageId,
                               result: { filename: args.filename }
                           });
                       } catch (e) {
                           console.error('Worker: Upload failed for', args.filename, 'Error:', e.message);
                           globalThis.postMessage({
                               type: 'error',
                               messageId: data.messageId,
                               error: e.message,
                               stack: e.stack
                           });
                       }
                   } else {
                       // console.log('Worker: Delegating message to original handler:', data.type);
                       return oldOnMessage(ev);
                   }
               };
           } else {
               console.error("sqlite3.initWorker1API not found");
           }
       }).catch(e => {
           console.error("Failed to initialize sqlite3 module in worker:", e);
       });
   } else {
       console.error("globalThis.sqlite3InitModule not found after importScripts");
   }