using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using app.core.utility;

namespace console.client
{
    internal class StartClient
    {
        private static void Main(string[] args)
        {
            ProcessSocketTest();
            Console.ReadLine();
        }     

        private static void ProcessSocketTest()
        {
            const string values = "111,222, \"33,44,55\",666, \"77,88\" , \"99\"";
            var row = CsvDao.OtherSplit(values);
            var row2 = CsvDao.SplitColumns(values);

            var path = CsvDao.CombineUri("http://Drive1/Plate/", @"\Place/Coco","Sft:Piper",@"\Nasty/Bmm","gogo.jpg");
            var fname = Path.GetFileName(path);
            var nfilename = Path.ChangeExtension(path, "mp4");

            Console.WriteLine("..starting socket test");
            Thread.Sleep(3000);

            for (var i = 0; i < 10000; i++)
            {
                Task.Factory.StartNew(RunSocketTest);
            }
        }
        private static readonly List<string> Resp = new List<string>();

        /// <summary>
        /// Run Socket Test
        /// </summary>
        private static void RunSocketTest()
        {

            try
            {
                var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(IPAddress.Parse("127.0.0.1"), 9801);
                client.ReceiveTimeout = 10000;

                const string msg = @"<request><claim-type>90000</claim-type><claim>Success</claim></request>\r\n";
                var buffer = Encoding.ASCII.GetBytes(msg);
                client.Send(buffer);

                int totalBytes;
                var readbuffer = new byte[1];
                var message = "";

                while ((totalBytes = client.Receive(readbuffer, 0, readbuffer.Length, SocketFlags.None)) > 0)
                {
                    var data = Encoding.ASCII.GetString(readbuffer, 0, totalBytes);
                    message = message + data;

                    if (message.EndsWith(Environment.NewLine) || message.EndsWith("\\r\\n"))
                        break;

                    if (!client.Connected)
                    {

                    }
                }

                Console.WriteLine("Reveived secure response @ {0}", DateTime.Now);
                Resp.Add(message);
            }
            catch (AggregateException exception)
            {
                Console.WriteLine("[AggregateException]: {0}", exception.Message);
            }
            catch (Exception exc)
            {
                Console.WriteLine("[Exception]: {0}", exc.Message);
            }
        }
    }
}


