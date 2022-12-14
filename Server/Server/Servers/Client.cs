using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using GameServer.Tool;
using SocketGameProtocol;
using System.Data.SqlClient;

namespace GameServer.Servers
{
    class Client
    {
        private DbManager _DB;

        private Socket _clientSocket;
        private Server _server;
        private Message _msg;
        private SqlConnection _sqlConnt;

        public Client(Socket socket,Server server)
        {
            _clientSocket = socket;
            _server = server;
        }

        /// <summary>
        /// 异步接收消息
        /// </summary>
        private void StartReceive()
        {
            _clientSocket.BeginReceive(_msg.Buffer, _msg.StartIndex, _msg.Remsize, SocketFlags.None, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult iar)
        {
            try
            {
                if (_clientSocket == null || _clientSocket.Connected == false) return;
                int len = _clientSocket.EndReceive(iar);
                Console.WriteLine("接收");
                if (len == 0)
                {
                    Console.WriteLine("接收数据为0");
                    Close();
                    return;
                }

                _msg.ReadBuffer(len, HandleRequest);
                StartReceive();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Close();
            }
        }

        public void Send(MainPack pack)
        {
            if (_clientSocket == null || _clientSocket.Connected == false) return;
            try
            {
                _clientSocket.Send(Message.PackData(pack));
            }
            catch
            {

            }
        }

        private void Close()
        {
            Console.WriteLine("断开");

            _clientSocket.Close();
            _server.RemoveClient(this);
        }

        void HandleRequest(MainPack pack)
        {
            _server.HandleRequest(pack, this);
        }
    }

}
