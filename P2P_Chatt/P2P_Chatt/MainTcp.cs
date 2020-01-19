using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Media;
using Newtonsoft.Json;
using System.Windows.Media.Imaging;

namespace P2P_Chatt
{
    class MainTcp
    {
        TcpListener server = null;
        TcpClient client = null;
        NetworkStream stream = null;
        private string name;
        Byte[] bytes = new byte[550000];
        Byte[] _data = new byte[550000];
        String data = null;
        Boolean close = false;
        Messages msgs = null;
        Boolean setup = false;


        //NetworkStream c_stream = null;
        public MainTcp(Messages msgs)
        {
            this.msgs = msgs;
      
        }

        //Server part
        public void CreateListener(int port, string name, string ip = "127.0.0.1")
        {
            this.name = name;
            IPAddress addr = IPAddress.Parse(ip);
            server = new TcpListener(addr, port);
            server.Start();
            Console.WriteLine("Listener Created on ip: " + ip + " port: " + port.ToString());
        } 
        //Fixa thread
        public void Listen()
        {
            if(server == null)
            {
                Console.WriteLine("There is no Listener Object");
                return;
            }
            try
            {
                // Buffer for reading data


                // Enter the listening loop.
                while (true)
                {
                    Console.WriteLine("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    client = server.AcceptTcpClient();
                    data = null;

                    // Get a stream object for reading and writing
                    stream = client.GetStream();

                    MessageBoxResult result = MessageBox.Show("Someone is trying to connect to your client\nWould you like to accept?", "Chat Request", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.No)
                    {
                        CloseTcp();
                        close = false;
                        //client.Close();
                        continue;
                    }

                    ListenStream();
                   
                    // Shutdown and end connection
                    //client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }

        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public void ListenStream()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                msgs.reset();
            });
            setup = false;

            int i;
            try
            {
                // Loop to receive all the data sent by the client.
                while (stream != null)
                {
                    if(close == true)
                    {               
                        close = false;
                        break;
                    }
                    if (setup == false)
                    {
                        SendMessage("We can now chat!");
                        setup = true;
                    }
                    if (stream.DataAvailable)
                    {

                        byte[] readBuffer = new byte[550000];
                        i = stream.Read(readBuffer, 0, readBuffer.Length);

                        //byte[] buffer = ReadFully(stream);

                        data = System.Text.Encoding.ASCII.GetString(readBuffer, 0, i);
                        Console.WriteLine(data);
                        Console.WriteLine("1");
                        
                        Message m = JsonConvert.DeserializeObject<Message>(data, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                        //Console.WriteLine(m.imageinbytes);
                        Console.WriteLine("=========================");
                        if (m.text == "/buzz")
                        {
                            string audio = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\Ring.wav";
                            Console.WriteLine(audio);
                            SoundPlayer snd = new SoundPlayer(audio);
                            snd.Play();
                        }
                        

                        
                        if(m.text == "/bye")
                        {
                            Console.WriteLine("Recieved bye, closing down");
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                msgs.AddMessage(new Tmessage("Server", "Your So called ´Friend` just left, LOL"),"A");
                            });
                            Console.WriteLine("Closed Stream");
                            stream.Close();
                            close = false;
                          
                            break;
                        }

                        //string[] e = data.Split(':'); 
                        
                        Application.Current.Dispatcher.Invoke(() =>
                       {
                           msgs.AddMessage(m,"R");
                       });
                      
                        Console.WriteLine("Received: {0}", data);
                    }
                }
                client.Close();
                stream = null;
                client = null;
                Console.WriteLine("Closed Client");
                Console.WriteLine(name);
            }
            catch(System.IO.IOException)
            {
                Console.WriteLine("Shit broke");
            }

}
        public void CloseTcp()
        {
            try
            {
                if (client == null)
                {
                    string m = "Not Connected to Anyone";
                    Console.WriteLine(m);
                    throw new OwnException(m);
                }
                if (stream == null)
                {
                    string m = "Stream is null";
                    Console.WriteLine(m);
                    throw new OwnException(m);
                }

                Console.WriteLine("Closing Connection");
                SendMessage("/bye");
                Console.WriteLine("Sent bye");
                close = true;
            }
            catch(OwnException)
            {

            }
        }
        //Client part
        public void CreateClient(string _name, int port, string ip = "127.0.0.1")
        {
            try
            {
                name = _name;
                client = new TcpClient(ip, port);
                stream = client.GetStream();
                Console.WriteLine("Created Client");
                ListenStream();
            }
            catch (SocketException e)
            {
                string messageBoxText = e.Message;
                string caption = "Error";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBox.Show(messageBoxText, caption, button, icon);
            }

        }
        public void SendMessage(string message)
        {
            try
            {
                if (stream != null)
                {
                    Tmessage m = new Tmessage(name, message);
                    string json = JsonConvert.SerializeObject(m, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                    Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
                    // Send the message to the connected TcpServer. 
                    stream.Write(data, 0, data.Length);

                    Console.WriteLine("Sent: {0}", message);
                }
                else
                {
                    throw new OwnException("Stream not available");
                }
            }
            catch(OwnException)
            {
                //Console.WriteLine(e.message);
            }
        }

        public void SendPic(byte[] pic)
        {
            //Skicka bild
            Imessage i = new Imessage(name, "picture", pic);
            string json = JsonConvert.SerializeObject(i, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

            _data = System.Text.Encoding.ASCII.GetBytes(json);
            // Send the message to the connected TcpServer. 
            //Console.WriteLine(data.Length);
            stream.Write(_data, 0, _data.Length);

            Console.WriteLine("Sent: {0}", "picture");
        }
        public bool isconnected()
        {
            return (client != null && stream != null);
        }

        public bool isListening()
        {
            return server != null;
        }

    }
}
