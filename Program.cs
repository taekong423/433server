using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Ircc;
using static Ircc.IrccHelper;

namespace Chat_Server
{
    class Client
    {
        int cliNum = 0;
        int roomId = 0;
        byte[] data = new byte[1024];
        string sendMessage = string.Empty;
        ReceiveHandler rh = new ReceiveHandler();
        Socket clientSocket;
        Header header;
        Packet sendPacket;
        Packet recvPacket;
        static List<Client> clients = new List<Client>();

        public void StartClient(Socket client)
        {
            clientSocket = client;
            clients.Add(this);

            Thread thread = new Thread(GetMessage);
            thread.Start();
        }

        public void GetMessage()
        {
            string getMessage = string.Empty;
            int received;

            try
            {
                while (clientSocket.Connected == true)
                {
                    received = clientSocket.Receive(data);
                    recvPacket = BytesToPacket(data);
                    sendPacket = rh.PacketHandler(recvPacket);
                    //header = recvPacket.header;
                    //data = recvPacket.data;
                    getMessage = Encoding.UTF8.GetString(data, 0, data.Length);
                    SendMessage(getMessage);
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine(se);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine("Client Disconnected");
                clientSocket.Close();
            }
        }

        public void SendMessage(string getMessage)
        {
            //sendMessage = "Client " + ": " + getMessage + " ";
            //data = Encoding.UTF8.GetBytes(sendMessage);
            data = PacketToBytes(sendPacket);
            foreach (Client c in clients)
            {
                c.clientSocket.Send(data);
            }
            
            Console.WriteLine("Log: {0} [{1}]", getMessage, DateTime.Now);
        }
    }

    class ReceiveHandler
    {
        public Packet PacketHandler(Packet recvPacket)
        {
            byte[] returnData = null;
            Header returnHeader = new Header();

            Console.WriteLine("Comm: {0}, Code: {1}, Size: {2}\nData: {3}", 
                recvPacket.header.comm, recvPacket.header.code, recvPacket.header.size, Encoding.UTF8.GetString(recvPacket.data, 0, recvPacket.data.Length));

            //Client to Server side
            if (Comm.CS == recvPacket.header.comm)
            {
                switch (recvPacket.header.code)
                {
                    //------------CREATE------------
                    case Code.CREATE:
                        //CL -> FE side
                        break;
                    case Code.CREATE_DUPLICATE_ERR:
                        //FE -> CL side
                        break;
                    case Code.CREATE_FULL_ERR:
                        //FE -> CL side
                        break;


                    //------------DESTROY------------
                    case Code.DESTROY:
                        //CL -> FE side
                        break;
                    case Code.DESTROY_ERR:
                        //FE -> CL side
                        break;


                    //------------FAIL------------
                    case Code.FAIL:
                        //
                        break;


                    //------------HEARTBEAT------------
                    case Code.HEARTBEAT:
                        //FE -> CL side
                        break;
                    case Code.HEARTBEAT_RES:
                        //CL -> FE side
                        break;


                    //------------JOIN------------
                    case Code.JOIN:
                        //CL -> FE side
                        break;
                    case Code.JOIN_FULL_ERR:
                        //FE -> CL side
                        break;
                    case Code.JOIN_NULL_ERR:
                        //FE -> CL side
                        break;


                    //------------LEAVE------------
                    case Code.LEAVE:
                        //CL -> FE side
                        break;
                    case Code.LEAVE_ERR:
                        //FE -> CL side
                        break;


                    //------------LIST------------
                    case Code.LIST:
                        //CL -> FE side
                        break;
                    case Code.LIST_ERR:
                        //FE -> CL side
                        break;
                    case Code.LIST_RES:
                        //FE -> CL side
                        break;


                    //------------MSG------------
                    case Code.MSG:
                        //CL <--> FE side
                        break;
                    case Code.MSG_ERR:
                        //CL <--> FE side
                        break;


                    //------------SIGNIN------------
                    case Code.SIGNIN:
                        //CL -> FE -> BE side
                        break;
                    case Code.SIGNIN_ERR:
                        //BE -> FE -> CL side
                        break;
                    case Code.SIGNIN_RES:
                        //BE -> FE -> CL side
                        break;


                    //------------SIGNUP------------
                    case Code.SIGNUP:
                        //CL -> FE -> BE side
                        //Do SignUp Process                        
                        break;
                    case Code.SIGNUP_ERR:
                        //BE -> FE -> CL side
                        //error handling
                        break;
                    case Code.SIGNUP_RES:
                        //BE -> FE -> CL side
                        //success
                        break;


                    //------------SUCCESS------------
                    case Code.SUCCESS:
                        //
                        break;
                }
            }
            //Server to Server Side
            else if (Comm.SS == recvPacket.header.comm)
            {
                switch (recvPacket.header.code)
                {
                    //------------SDESTROY------------
                    case Code.SDESTROY:
                        //FE side
                        break;
                    case Code.SDESTROY_ERR:
                        //FE side
                        break;


                    //------------SJOIN------------
                    case Code.SJOIN:
                        //FE side
                        break;
                    case Code.SJOIN_ERR:
                        //FE side
                        break;


                    //------------SLIST------------
                    case Code.SLIST:
                        //FE side
                        break;
                    case Code.SLIST_ERR:
                        //FE side
                        break;
                    case Code.SLIST_RES:
                        //FE side
                        break;


                    //------------SMSG------------                
                    case Code.SMSG:
                        //FE side
                        break;
                    case Code.SMSG_ERR:
                        //FE side
                        break;
                }
            }
            //Dummy to Server Side
            else if (Comm.DUMMY == recvPacket.header.comm)
            {

            }

            returnHeader = recvPacket.header;
            returnData = recvPacket.data;
            Packet returnPacket = new Packet(returnHeader, returnData);

            return returnPacket;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            
            int port = 30000;
            Socket listener = null;
            Socket client = null;
            IPAddress address = null;  

            try
            {
                if (IPAddress.TryParse("10.100.58.7", out address))
                {
                    listener = new Socket
                        (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    listener.Bind(new IPEndPoint(address, port));
                }
                else Console.WriteLine("Invalid IP Address");
                
                Console.WriteLine("Listening...");
                listener.Listen(5);

                while (true)
                {
                    client = listener.Accept();
                    Console.WriteLine("Client Connected");
                    Client c = new Client();
                    c.StartClient(client);
                }
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("IOException");
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.Message);
            }
            finally
            {

                listener.Close();
            }
        }
    }
}
