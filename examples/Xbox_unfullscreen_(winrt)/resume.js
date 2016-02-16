
exports.invoke = function() {
    // Needed for AppContainer apps
    function ResumeAllThreads() {
        var ResumeThread = new NativeFunction(Module.findExportByName("kernel32.dll", "ResumeThread"), 'int', ['int']);
        var OpenThread = new NativeFunction(Module.findExportByName("kernel32.dll", "OpenThread"), 'int', ['int', 'int', 'int']);

        Process.enumerateThreads({
            onMatch: function (thread) {
                if (thread.state == "stopped") {
                    ResumeThread(OpenThread(0x10FFFF, 0, thread.id));
                }
            },
            onComplete: function(){} // required
        });
    }
    ResumeAllThreads();
}