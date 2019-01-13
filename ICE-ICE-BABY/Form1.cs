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
                if (result.NetType == STUN_NetType.UdpBlocked)
                {
                    PrintOutput("Did not manage to contact STUN server, trying alternatives");
                    foreach (var item in STUN_PROVIDERS)
                    {
                        var res = STUN_Client.Query(item.Item1, item.Item2, socket);
                        if (res.NetType != STUN_NetType.UdpBlocked)
                        {
                            result = res;
                            break;
                        }
                    }
                }
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
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            idInput.Text = Clipboard.GetText();
        }
        readonly (string, int)[] STUN_PROVIDERS = new[] {("iphone-stun.strato-iphone.de",3478),("numb.viagenie.ca",3478),("sip1.lakedestiny.cordiaip.com",3478),("stun.12connect.com",3478),("stun.12voip.com",3478),("stun.1cbit.ru",3478),("stun.1und1.de",3478),("stun.2talk.co.nz",3478),("stun.2talk.com",3478),("stun.3clogic.com",3478),("stun.3cx.com",3478),("stun.726.com",3478),("stun.a-mm.tv",3478),("stun.aa.net.uk",3478),("stun.aceweb.com",3478),("stun.acrobits.cz",3478),("stun.acronis.com",3478),("stun.actionvoip.com",3478),("stun.advfn.com",3478),("stun.aeta-audio.com",3478),("stun.aeta.com",3478),("stun.allflac.com",3478),("stun.anlx.net",3478),("stun.antisip.com",3478),("stun.avigora.com",3478),("stun.avigora.fr",3478),("stun.b2b2c.ca",3478),("stun.bahnhof.net",3478),("stun.barracuda.com",3478),("stun.beam.pro",3478),("stun.bitburger.de",3478),("stun.bluesip.net",3478),("stun.bomgar.com",3478),("stun.botonakis.com",3478),("stun.budgetphone.nl",3478),("stun.budgetsip.com",3478),("stun.cablenet-as.net",3478),("stun.callromania.ro",3478),("stun.callwithus.com",3478),("stun.cheapvoip.com",3478),("stun.cloopen.com",3478),("stun.cognitoys.com",3478),("stun.comfi.com",3478),("stun.commpeak.com",3478),("stun.communigate.com",3478),("stun.comrex.com",3478),("stun.comtube.com",3478),("stun.comtube.ru",3478),("stun.connecteddata.com",3478),("stun.cope.es",3478),("stun.counterpath.com",3478),("stun.counterpath.net",3478),("stun.crimeastar.net",3478),("stun.dcalling.de",3478),("stun.demos.ru",3478),("stun.demos.su",3478),("stun.dls.net",3478),("stun.dokom.net",3478),("stun.dowlatow.ru",3478),("stun.duocom.es",3478),("stun.dus.net",3478),("stun.e-fon.ch",3478),("stun.easemob.com",3478),("stun.easycall.pl",3478),("stun.easyvoip.com",3478),("stun.eibach.de",3478),("stun.ekiga.net",3478),("stun.ekir.de",3478),("stun.elitetele.com",3478),("stun.emu.ee",3478),("stun.engineeredarts.co.uk",3478),("stun.eoni.com",3478),("stun.epygi.com",3478),("stun.faktortel.com.au",3478),("stun.fbsbx.com",3478),("stun.fh-stralsund.de",3478),("stun.fmbaros.ru",3478),("stun.fmo.de",3478),("stun.freecall.com",3478),("stun.freeswitch.org",3478),("stun.freevoipdeal.com",3478),("stun.genymotion.com",3478),("stun.gmx.de",3478),("stun.gmx.net",3478),("stun.gnunet.org",3478),("stun.gradwell.com",3478),("stun.halonet.pl",3478),("stun.highfidelity.io",3478),("stun.hoiio.com",3478),("stun.hosteurope.de",3478),("stun.i-stroy.ru",3478),("stun.ideasip.com",3478),("stun.imweb.io",3478),("stun.infra.net",3478),("stun.innovaphone.com",3478),("stun.instantteleseminar.com",3478),("stun.internetcalls.com",3478),("stun.intervoip.com",3478),("stun.ipcomms.net",3478),("stun.ipfire.org",3478),("stun.ippi.com",3478),("stun.ippi.fr",3478),("stun.it1.hr",3478),("stun.ivao.aero",3478),("stun.jabbim.cz",3478),("stun.jumblo.com",3478),("stun.justvoip.com",3478),("stun.kaospilot.dk",3478),("stun.kaseya.com",3478),("stun.kaznpu.kz",3478),("stun.kiwilink.co.nz",3478),("stun.kuaibo.com",3478),("stun.l.google.com",19302),("stun.lamobo.org",3478),("stun.levigo.de",3478),("stun.lindab.com",3478),("stun.linphone.org",3478),("stun.linx.net",3478),("stun.liveo.fr",3478),("stun.lowratevoip.com",3478),("stun.lundimatin.fr",3478),("stun.maestroconference.com",3478),("stun.mangotele.com",3478),("stun.mgn.ru",3478),("stun.mit.de",3478),("stun.miwifi.com",3478),("stun.mixer.com",3478),("stun.modulus.gr",3478),("stun.mrmondialisation.org",3478),("stun.myfreecams.com",3478),("stun.myvoiptraffic.com",3478),("stun.mywatson.it",3478),("stun.nacsworld.com",3478),("stun.nas.net",3478),("stun.nautile.nc",3478),("stun.netappel.com",3478),("stun.nextcloud.com",3478),("stun.nfon.net",3478),("stun.ngine.de",3478),("stun.node4.co.uk",3478),("stun.nonoh.net",3478),("stun.nottingham.ac.uk",3478),("stun.nova.is",3478),("stun.onesuite.com",3478),("stun.onthenet.com.au",3478),("stun.ooma.com",3478),("stun.oovoo.com",3478),("stun.ozekiphone.com",3478),("stun.personal-voip.de",3478),("stun.petcube.com",3478),("stun.pexip.com",3478),("stun.phone.com",3478),("stun.pidgin.im",3478),("stun.pjsip.org",3478),("stun.planete.net",3478),("stun.poivy.com",3478),("stun.powervoip.com",3478),("stun.ppdi.com",3478),("stun.rackco.com",3478),("stun.redworks.nl",3478),("stun.ringostat.com",3478),("stun.rmf.pl",3478),("stun.rockenstein.de",3478),("stun.rolmail.net",3478),("stun.rudtp.ru",3478),("stun.russian-club.net",3478),("stun.rynga.com",3478),("stun.sainf.ru",3478),("stun.schlund.de",3478),("stun.sigmavoip.com",3478),("stun.sip.us",3478),("stun.sipdiscount.com",3478),("stun.sipgate.net",10000),("stun.sipgate.net",3478),("stun.siplogin.de",3478),("stun.sipnet.net",3478),("stun.sipnet.ru",3478),("stun.siportal.it",3478),("stun.sippeer.dk",3478),("stun.siptraffic.com",3478),("stun.sma.de",3478),("stun.smartvoip.com",3478),("stun.smsdiscount.com",3478),("stun.snafu.de",3478),("stun.solcon.nl",3478),("stun.solnet.ch",3478),("stun.sonetel.com",3478),("stun.sonetel.net",3478),("stun.sovtest.ru",3478),("stun.speedy.com.ar",3478),("stun.spoiltheprincess.com",3478),("stun.srce.hr",3478),("stun.ssl7.net",3478),("stun.stunprotocol.org",3478),("stun.swissquote.com",3478),("stun.t-online.de",3478),("stun.talks.by",3478),("stun.tel.lu",3478),("stun.telbo.com",3478),("stun.telefacil.com",3478),("stun.threema.ch",3478),("stun.tng.de",3478),("stun.trueconf.ru",3478),("stun.twt.it",3478),("stun.ucallweconn.net",3478),("stun.ucsb.edu",3478),("stun.ucw.cz",3478),("stun.uiscom.ru",3478),("stun.uls.co.za",3478),("stun.unseen.is",3478),("stun.up.edu.ph",3478),("stun.usfamily.net",3478),("stun.uucall.com",3478),("stun.veoh.com",3478),("stun.vipgroup.net",3478),("stun.viva.gr",3478),("stun.vivox.com",3478),("stun.vline.com",3478),("stun.vmi.se",3478),("stun.vo.lu",3478),("stun.vodafone.ro",3478),("stun.voicetrading.com",3478),("stun.voip.aebc.com",3478),("stun.voip.blackberry.com",3478),("stun.voip.eutelia.it",3478),("stun.voiparound.com",3478),("stun.voipblast.com",3478),("stun.voipbuster.com",3478),("stun.voipbusterpro.com",3478),("stun.voipcheap.co.uk",3478),("stun.voipcheap.com",3478),("stun.voipdiscount.com",3478),("stun.voipfibre.com",3478),("stun.voipgain.com",3478),("stun.voipgate.com",3478),("stun.voipinfocenter.com",3478),("stun.voipplanet.nl",3478),("stun.voippro.com",3478),("stun.voipraider.com",3478),("stun.voipstunt.com",3478),("stun.voipwise.com",3478),("stun.voipzoom.com",3478),("stun.voxgratia.org",3478),("stun.voxox.com",3478),("stun.voztele.com",3478),("stun.wcoil.com",3478),("stun.webcalldirect.com",3478),("stun.whc.net",3478),("stun.whoi.edu",3478),("stun.wifirst.net",3478),("stun.wwdl.net",3478),("stun.xn----8sbcoa5btidn9i.xn--p1ai",3478),("stun.xten.com",3478),("stun.xtratelecom.es",3478),("stun.yy.com",3478),("stun.zadarma.com",3478),("stun.zepter.ru",3478),("stun.zoiper.com",3478),("stun1.faktortel.com.au",3478),("stun1.l.google.com",19302),("stun2.l.google.com",19302),("stun3.l.google.com",19302),("stun4.l.google.com",19302),("stun.zoiper.com",3478)};
    }
}
