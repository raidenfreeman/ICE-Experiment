using LumiSoft.Net.STUN.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ICE_ICE_BABY
{
    public partial class Form1 : Form
    {
        const int portNumber = 43788;
        byte[] byteBuffer = new byte[1024];
        Regex ValidIpAddressRegex;
        public Form1()
        {
            InitializeComponent();
            ValidIpAddressRegex = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5]):[0-9]+$");
        }

        private string connectionID;

        private void button1_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            connectToServer.Visible = false;
            startServer.Visible = false;
            chatInput.Visible = false;
            chatOutput.Visible = false;
            try
            {

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.Bind(new IPEndPoint(IPAddress.Any, portNumber));

                STUN_Result result = STUN_Client.Query("stun1.l.google.com", 19302, socket);
                PrintOutput("NAT Type: " + result.NetType.ToString());
                PrintOutput("Local Endpoint: " + socket.LocalEndPoint.ToString());
                if (result.NetType != STUN_NetType.UdpBlocked)
                {
                    PrintOutput("Public Endpoint: " + result.PublicEndPoint.ToString());
                    var id = Base64Encode(result.PublicEndPoint.ToString());
                    //Clipboard.SetText(id);
                    idText.Text = id;
                }
                PrintOutput("=================");
                socket.Close();
            }
            catch (Exception x)
            {
                MessageBox.Show(this, "Error: " + x.ToString(), "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
        delegate void PrintOutputCallback(string text);
        void PrintOutput(string s)
        {
            if (this.outputText.InvokeRequired)
            {
                PrintOutputCallback d = new PrintOutputCallback(PrintOutput);
                this.Invoke(d, new object[] { s });
            }
            else
            {
                outputText.AppendText(s + Environment.NewLine);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(idText.Text);
        }

        Socket sk;
        IPEndPoint targetEndpoint;

        delegate void ChatAppendCallback(string text, bool isIncoming);
        private void ChatAppend(string text, bool isIncoming)
        {
            if (this.chatOutput.InvokeRequired)
            {
                ChatAppendCallback d = new ChatAppendCallback(ChatAppend);
                this.Invoke(d, new object[] { text, isIncoming });
            }
            else
            {
                text += Environment.NewLine;
                if (isIncoming)
                {
                    text += Environment.NewLine;
                }

                chatOutput.SelectionStart = chatOutput.TextLength;
                chatOutput.SelectionLength = 0;

                chatOutput.SelectionColor = isIncoming ? Color.Salmon : Color.SpringGreen;
                chatOutput.SelectionAlignment = HorizontalAlignment.Right;
                chatOutput.AppendText(text);
                chatOutput.SelectionColor = chatOutput.ForeColor;
            }
        }

        private void OnSend(IAsyncResult ar)
        {
            try
            {
                sk.EndSend(ar);
                PrintOutput($"Sent: {chatInput.Text}");
                ChatAppend(chatInput.Text, false);
            }
            catch (ObjectDisposedException)
            { }
            catch (Exception ex)
            {
                PrintOutput(ex.Message);
            }
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => chatInput.Clear()));
                return;
            }
        }

        private void startServer_Click(object sender, EventArgs e)
        {
            var targetIP = IPAddress.Parse(Base64Decode(idInput.Text).Split(':')[0]);
            var targetPort = Convert.ToInt32(Base64Decode(idInput.Text).Split(':')[1]);
            targetEndpoint = new IPEndPoint(targetIP, targetPort);

            if (sk != null)
            {
                sk.Close();
            }
            sk = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                sk.Bind(new IPEndPoint(IPAddress.Any, portNumber));
                PrintOutput($"🖥 Started server at port:{portNumber}");

                // target: 85.72.40.234:43788  base64=> ODUuNzIuNDAuMjM0OjQzNzg4
                PrintOutput($"👊 Hole punching...");
                // Punchthrough
                byte[] bytes = Encoding.ASCII.GetBytes("ggwp");
                for (int i = 0; i < 10; i++)
                {
                    // Send a few packets to the target, to punch a hole in our NAT
                    sk.SendTo(bytes, targetEndpoint);
                    //s.Send(bytes, bytes.Length, targetEndpoint);
                }
                PrintOutput($"Done");

                //TODO: CHANGE THIS TO TARGET ENDPOINT 😱😱😱😱😱😱😱😱😱😱😱😱😱😱😱😱😱😱😱😱😱😱😱
                IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
                //The epSender identifies the incoming clients
                EndPoint epSender = (EndPoint)ipeSender;
                //Start receiving data
                sk.BeginReceiveFrom(byteBuffer, 0, byteBuffer.Length,
                    SocketFlags.None, ref epSender, new AsyncCallback(OnReceive), epSender);

                PrintOutput($"👂 Listenning for data...");
                chatInput.Visible = true;
                chatOutput.Visible = true;
            }
            catch (Exception ex)
            {
                PrintOutput("Something bad happened 😱");
                PrintOutput(ex.Message);
                Console.WriteLine(ex.ToString());
                sk.Close();
                PrintOutput("🔌 Connection closed");
                connectToServer.Visible = false;
                startServer.Visible = false;
                chatInput.Visible = false;
                chatOutput.Visible = false;
            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            EndPoint epSender = (EndPoint)targetEndpoint;
            try
            {

                sk.EndReceiveFrom(ar, ref epSender);

                var msgReceived = Encoding.ASCII.GetString(byteBuffer);

                ChatAppend(msgReceived, true);
                PrintOutput("Got: " + msgReceived);
                byteBuffer = new byte[1024];

                //Start listening to receive more data from the user
                sk.BeginReceiveFrom(byteBuffer, 0, byteBuffer.Length, SocketFlags.None, ref epSender,
                                           new AsyncCallback(OnReceive), null);
            }
            catch (Exception ex)
            {
                PrintOutput(ex.Message);
                sk.Close();
                PrintOutput("🔌 Connection closed");
            }
        }


        private void idInput_TextChanged(object sender, EventArgs e)
        {
            var target = Base64Decode(idInput.Text);

            var isMatch = ValidIpAddressRegex.IsMatch(target);
            connectToServer.Visible = isMatch;
            startServer.Visible = isMatch;
            chatInput.Visible = false;
            chatOutput.Visible = false;

        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sk != null)
            {
                sk.Close();
            }
        }
        bool t = false;

        private void chatInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                var textToSend = chatInput.Text;
                if (textToSend != String.Empty)
                {
                    if (sk != null)
                    {
                        try
                        {
                            byte[] byteData = Encoding.ASCII.GetBytes(chatInput.Text);

                            //Send it to the target
                            sk.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, targetEndpoint, new AsyncCallback(OnSend), null);
                        }
                        catch (Exception)
                        {
                            PrintOutput("Unable to send message to the target.");
                        }
                    }
                    //else
                    //{
                    //    ChatAppend(textToSend, t);
                    //    t = !t;
                    //}
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            idInput.Text = Clipboard.GetText();
        }
    }
}
