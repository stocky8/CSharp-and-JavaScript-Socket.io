using Newtonsoft.Json.Linq;
using System.Collections.Immutable;
using Quobject.EngineIoClientDotNet.Client;
using Quobject.EngineIoClientDotNet.Client.Transports;
using Quobject.EngineIoClientDotNet.ComponentEmitter;
using Quobject.EngineIoClientDotNet.Modules;
using Quobject.EngineIoClientDotNet.Parser;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sock = Quobject.SocketIoClientDotNet.Client.Socket;

namespace Socket.io_Client
{
    public class Connection
    {
        private ManualResetEvent ManualResetEvent = null;
        private Sock socket;
        public string Message;
        private int Number;
        private bool Flag;

        public void Connect()
        {
            socket = IO.Socket("http://localhost:3000");
            socket.On(Sock.EVENT_CONNECT, () =>
            {
                
            });

            socket.On(Sock.EVENT_DISCONNECT, (data) =>
            {
                Message = (string)data;
            });

        }
    }
}
