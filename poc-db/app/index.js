
const Filequeue = require('filequeue')
const fq = new Filequeue(200);
const res = 'resources/';
const tables = require('./tables')
const fs = require('fs')
const out = 'out/files/';
const log = 'out/log/log.txt'

function list() {

    tables().forEach(function (tableName, idx, arr) {
        fq.readdir(res, function (err, files) {
            if (err) {
                throw err;
            }

            files.forEach(function (file) {
                fq.readFile(res + file, { encoding: 'utf8' }, function (err, data) {
                    var s = data.toString();
                    if (s.indexOf(tableName) != -1) {
                        console.log('Table: ' + tableName + ' file: ' + file);
                        gravaArquivo(file, data);
                    }
                })
            });
        });
    })

}

function gravaArquivo(file, data) {
    fs.writeFile(out + file, data, { encoding: 'utf8' }, function (err) {
        if (err) {
            console.error(err);
            throw err;
        }
    })
    fs.appendFile(log, 'Arquivo copiado: ' + file, { encoding: 'utf8' }, function (err) {
        if (err) {
            throw err;
        }
    })
    console.log('Arquivo copiado: ' + file);
}
list();
