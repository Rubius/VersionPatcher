var path = require('path');
var tl = require('vso-task-lib');

var pash = new tl.ToolRunner(tl.which('pash', true));
var scriptPath = path.resolve(__dirname, 'patcher.ps1');
pash.arg('"& ' + scriptPath + " " + __dirname + '"');

pash.exec({ failOnStdErr: false })
    .then(function(code) {
        tl.exit(code);
    })
    .fail(function (err) {
        console.error(err.message);
        tl.debug('taskRunner fail');
        tl.exit(1);
    });