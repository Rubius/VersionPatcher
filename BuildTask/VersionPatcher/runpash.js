var path = require('path');
var tl = require('vso-task-lib');

var powershell = new tl.ToolRunner(tl.which('powershell', true));
var scriptPath = path.resolve(__dirname, 'patcher.ps1');
powershell.arg('-File ' + scriptPath);

powershell.exec({ failOnStdErr: false, cwd: __dirname})
    .then(function(code) {
        tl.exit(code);
    })
    .fail(function (err) {
        console.error(err.message);
        tl.debug('taskRunner fail');
        tl.exit(1);
    });