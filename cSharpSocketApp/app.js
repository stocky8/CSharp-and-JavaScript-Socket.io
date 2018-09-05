var app = require('express')();
var http = require('http').Server(app);
var io = require('socket.io')(http);

var users = {};

app.get('/', function(req, res){
  res.sendFile(__dirname + '/public/index.html');
});

app.get('/style.css', function(req, res){
    res.sendFile(__dirname + '/public/style.css');
});

app.get('/context-menu.js', function(req, res){
  res.sendFile(__dirname + '/public/context-menu.js');
});

app.get('/test', function(req, res){
  res.sendFile(__dirname + '/public/test.html');
});

io.on('connection', function(socket){
    socket.on('new user', function(data, callback){
      if(data in users){
        callback(false);
      }else{
        callback(true);
        socket.nickname = data;
        users[socket.nickname] = socket;
        updateNicknames();
        io.emit('new message', {usr: "Server", msg: socket.nickname + " has connected."});
      }
    });

    function updateNicknames(){
      io.sockets.emit('usernames', Object.keys(users));
    }

    socket.on('new message', function(data){
      var msg = data.msg.trim();
      if(msg.substr(0,3) === '/w '){
        msg = msg.substr(3);
        var ind = msg.indexOf(' ');

        if(ind !== -1){
          var name = msg.substring(0, ind);
          var msg = msg.substring(ind + 1);

          if(name in users){
            users[name].emit('new message', {usr: data.usr, msg: msg});
            console.log('Whispering!');
          }else{
            console.log('enter valid user')
          }
        }else{
          console.log('invalid format for whisper, require message')
        }
        
      }else{
        io.emit('new message', {usr: data.usr, msg: data.msg});
      }
    });
    socket.on('screenshot', function(data){
      io.emit('screenshot', data);
    });
    socket.on('command', function(data){
      io.emit('command', data);
    });

    socket.on('disconnect', function(){
      if(!socket.nickname) return;
      delete users[socket.nickname];
      updateNicknames();
      io.emit('new message', {usr: "Server", msg: socket.nickname + " has disconnected."})
    });
});

/*
io.sockets.on('connection', function(socket){
    socket.on('send message', function(data){
        io.sockets.emit('new message', data); // Send to everyone
        //socket.broatcast.emit('new message', data); // Send to everyone except sender
    });
});
*/

http.listen(3000, function(){
  console.log('listening on *:3000');
});