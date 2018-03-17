const http = require('http');


const server = http.createServer(function (req, res) {

    res.writeHead(200, { "Content-Type": "text/html" });
    res.end('<h1>Hello World!!!');
});


const porta = 9000
server.listen(porta, function(){
    console.log(`Escutando a porta: ${porta}`);
});