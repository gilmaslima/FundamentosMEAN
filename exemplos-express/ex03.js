const express = require('express');
const server = express();
const lodash = require('lodash');

server.use(function (req, res, next) {
    console.log('Inicio...');
    console.log('\n');
    var obj  = req.query;
    var r = lodash.get(obj, 'r');

    console.log('\n');
    console.log(`r = ${r}`);
    next();
    console.log('Fim...');
    
})

server.use(function (req, res, next) {
    console.log('Resposta...');
    res.send('<h1>API!</h1>');
    next();
})



server.listen(3000, () => console.log('Executando...'))
