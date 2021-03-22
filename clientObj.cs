using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;

namespace broadcastTelnetServer {
    class clientObj {
        private byte[] buffer;
        private string myInput;
        private Socket mySocket;
        private int amountToReadFromBuffer;
        private List<clientObj> clientsList;

        public clientObj(Socket socket, List<clientObj> clientsList) {
            this.myInput = "";
            this.mySocket = socket;
            this.buffer = new byte[1500];
            this.clientsList = clientsList;
            this.amountToReadFromBuffer = 0;
            this.mySocket.Send(Encoding.UTF8.GetBytes(" > Telnet 127.0.0.1 9999\n\r > "));
        } // O(1)

        private bool getMsgToBroadcast() {
            this.myInput = "";
            while (!this.myInput.Contains('\n')) {
                try {
                    this.amountToReadFromBuffer = this.mySocket.Receive(this.buffer);
                    this.myInput += Encoding.UTF8.GetString(this.buffer, 0, this.amountToReadFromBuffer);
                }
                catch {
                    this.clientsList.Remove(this);
                    return true;
                }
            }
            return false;
        } // O(N)

        private void sendMsg(byte[] msg) {
            this.mySocket.Send(msg);
        } // O(1)

        private void sendMsgToOtherClient(clientObj client) {
            try {
                client.sendMsg(Encoding.UTF8.GetBytes("\n\r < " + this.myInput + "\r > "));
            } catch {
                clientsList.Remove(client);
                return;
            }
        }  // O(N)

        private void whomToSendMsg(clientObj client) {
            if (client == this)
                this.mySocket.Send(Encoding.UTF8.GetBytes("\r > "));
            else
                this.sendMsgToOtherClient(client);
        } // O(1)

        public void startBroadcasting() {
            while (true) {
                if (this.getMsgToBroadcast())
                    return;
                foreach (clientObj client in this.clientsList) {
                    try {
                        this.whomToSendMsg(client);
                    } catch {
                        continue;
                    }
                }
            }
        } // O(∞)

        public override string ToString() {
            return this.mySocket.ToString();
        } // O(1)

        ~clientObj() {
            this.mySocket.Close();
            this.mySocket = null;
        } // O(1)
    }
}