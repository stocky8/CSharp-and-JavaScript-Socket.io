using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Collections.Immutable;
using Quobject.EngineIoClientDotNet.Client;
using Quobject.EngineIoClientDotNet.Client.Transports;
using Quobject.EngineIoClientDotNet.ComponentEmitter;
using Quobject.EngineIoClientDotNet.Modules;
using Quobject.EngineIoClientDotNet.Parser;
using Quobject.SocketIoClientDotNet.Client;
using Sock = Quobject.SocketIoClientDotNet.Client.Socket;
using Newtonsoft.Json;
using System.Drawing.Imaging;
using System.IO;

namespace Socket.io_Client
{

    public partial class Form1 : Form
    {

        private Sock socket;
        private string ip = "http://localhost:3000";

        public Form1()
        {
            InitializeComponent();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            sendMessage(textBoxSend.Text);
            textBoxSend.Text = "";
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            labelStatus.Text = "Connecting...";
            socketIOConnect();
            
        }

        private void socketIOConnect()
        {
            if (socket == null)
            {
                socket = IO.Socket(ip);
                socketIOManager();
            }
            else
            {
                updateStatus("Connected");
            }
        }

        private void socketIOManager()
        {
            if (socket != null)
            {
                socket.On(Sock.EVENT_CONNECT, () =>
                {
                    updateStatus("Connected");
                    //socket.Emit("chat message", Environment.UserDomainName + "/" + Environment.UserName + " has connected.");
                });

                socket.On("new message", (data) =>
                {

                    JObject o = (JObject)data;

                    var username = (string)o.GetValue("usr");
                    var message = (string)o.GetValue("msg");

                    updateChatBox(username + ": " + message);

                    //string json = JsonConvert.SerializeObject(o);

                    //updateChatBox(json);

                });

                socket.On("command", (data) =>
                {

                    string command = (string)data;

                    switch (command)
                    {
                        case "screengrab":
                            this.grabScreen();
                            break;
                        default:
                            updateChatBox("Received invalid command: " + command);
                            break;
                    }

                });

            }
        }

        private void sendMessage(String msg)
        {
            if (socket != null)
            {
                MessageConvert messageObj = new MessageConvert();

                messageObj.usr = Environment.UserDomainName + "/" + Environment.UserName;
                messageObj.msg = msg;

                JObject json = JObject.FromObject(messageObj);

                socket.Emit("new message", json);
            }
        }

        private void updateStatus(string status)
        {
            if (socket != null)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke((Action)(() => labelStatus.Text = status));
                }
                else
                {
                    this.labelStatus.Text = status;
                }
            }
        }

        private void updateChatBox(string message)
        {
            if (socket != null)
            {
                if (this.chatBox.InvokeRequired)
                {
                    chatBox.Invoke(new Action(() => chatBox.Text = chatBox.Text + message + Environment.NewLine));
                }
                else
                {
                    this.chatBox.Text = chatBox.Text + message + Environment.NewLine;
                }
            }
            
        }

        private void buttonScreenshot_Click(object sender, EventArgs e)
        {
            grabScreen();
        }

        private void grabScreen()
        {
            Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics g = Graphics.FromImage(bitmap);
            g.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

            socket.Emit("screenshot", ImageToBase64(bitmap, ImageFormat.Jpeg));
        }

        private string ImageToBase64(Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return "data:image/jpeg;base64," + base64String;
            }
        }

    }
}
