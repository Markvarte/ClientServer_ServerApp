using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.IO.MemoryMappedFiles;
using System.Messaging;
public enum ServerProgramType { FIFO, queue, mappedMemory }

namespace ClientServer_ServerApp
{
    class ServerProgram
    {
        // References computer journal queues.
        public void MonitorComputerJournal()
        {
            MessageQueue computerJournal = new
                MessageQueue(".\\Journal$");
            while (true)
            {
                Message journalMessage = computerJournal.Receive();
                // Process the journal message.

                Console.WriteLine("[Server]: quene journal", journalMessage);
            }
        }

        public void ThreadStartServer()
        {
            // Tiek izveidots FIFO
            using (NamedPipeServerStream pipeStream = new NamedPipeServerStream("pipe"))
            {
                // Lai pārliecinātos par savienojuma izveidi, jāatgriež rindiņa par savienojumu, {0} vietā tiek atgriezts keša kods, kas identificē savienojumu

                Console.WriteLine("[Server] izveidots {0}", pipeStream.GetHashCode());

                // Savienojuma gaidīšana ar klientu

                pipeStream.WaitForConnection();

                //Parādās servera pusē tad, kad tiek palaista klienta puse, ziņo par veiksmīgu savienojumu
                Console.WriteLine("[Server] Fifo savienojums izveidots");

                //Lasa klienta nodoto informāciju pa FIFO kanālu, informācija tiek atgriezta kopā ar datumu un laiku.

                using (StreamReader sr = new StreamReader(pipeStream))
                {
                    string temp;

                    while ((temp = sr.ReadLine()) != null)
                    {
                        Console.WriteLine("{0}: {1}", DateTime.Now, temp);
                    }
                }
            }
            //Būs zudis savienojums, ja klients izies no programmas

            Console.WriteLine("Savienojums zudis");
        }


        static void Main(string[] args)
        {
            ServerProgramType c = ServerProgramType.queue; //  for now
            switch (c) {
            case ServerProgramType.FIFO: // 0
                    //Klients sūta ziņojumus, bet serveris saņem, tiek veidoti jauni pavedieni, kuri atgriež klienta informāciju

                    ServerProgram Server = new ServerProgram();


                    Thread ServerThread = new Thread(Server.ThreadStartServer);


                    ServerThread.Start();
            break;

            case ServerProgramType.queue: // 1
                    // Create a new instance of the class.
                    ServerProgram myNewQueue = new ServerProgram();

                    // Send a message to a queue.
                    myNewQueue.SendMessage();

                    // Receive a message from a queue.
                    myNewQueue.ReceiveMessage();

                    break;

            case ServerProgramType.mappedMemory: // 2
                    //Tiek palaists serveris

                    Console.WriteLine("Memory mapped failu serveris");

                    //Tiek izveidots dalītās atmiņas mehānisms un ierakstīta informācija failā

                    using (var file = MemoryMappedFile.CreateNew("myFile", int.MaxValue))
                    {
                        var bytes = new byte[24];
                        for (var i = 0; i < bytes.Length; i++)
                            bytes[i] = (byte)(65 + i);

                        using (var writer = file.CreateViewAccessor(0, bytes.Length))
                        {
                            writer.WriteArray<byte>(0, bytes, 0, bytes.Length);
                        }
                        Console.WriteLine("Palaidiet memory mapped failu lasītāju pirms izejiet no programmas");

                        Console.ReadLine();
                    }
            break;

                default:
                    Console.WriteLine("Wrong server type.");
                    break;
            }
         

        }
        //**************************************************
        // Sends an Order to a queue.
        //**************************************************

        public void SendMessage()
        {

            // Create a new order and set values.
            MyMSMQ sentOrder = new MyMSMQ();
            sentOrder.orderId = 3;
            sentOrder.orderTime = DateTime.Now;

            // Connect to a queue on the local computer.
            MessageQueue myQueue = new MessageQueue(".\\private$\\new");
            // Send the Order to the queue.
            // myQueue.Formatter = new BinaryMessageFormatter();
            Console.WriteLine(myQueue.Path);
            //Console.WriteLine(myQueue.GetType());
            Message mes = new Message(sentOrder);
            Console.WriteLine(mes);
            //Console.WriteLine(sentOrder);
            Console.WriteLine("sending end");
            // myQueue.Send("sending text plz...");
            myQueue.Send(mes);
            myQueue.DefaultPropertiesToSend.Label = "sending text plz...";

            return;
        }

        //**************************************************
        // Receives a message containing an Order.
        //**************************************************

        public void ReceiveMessage()
        {
            // Connect to the a queue on the local computer.
            MessageQueue myQueue = new MessageQueue(".\\private$\\new");
            //Console.WriteLine(myQueue.Path);
            //Console.WriteLine(myQueue.GetType());
            //Console.WriteLine(myQueue.GetAllMessages().Length);
            //Console.WriteLine(myQueue.CanWrite);
            // Set the formatter to indicate body contains an Order.
            myQueue.Formatter = new XmlMessageFormatter(new Type[]
                {typeof(MyMSMQ)});

            try
            {
            // Receive and format the message.
            Message myMessage = myQueue.Receive();
            Console.WriteLine("SOmething happens after receive ??");
               // Console.WriteLine(myMessage.Body);
                MyMSMQ myOrder = (MyMSMQ)myMessage.Body;

               // Display message information.
               Console.WriteLine("Order ID: " +
                   myOrder.orderId.ToString());
                Console.WriteLine("Sent: " +
                    myOrder.orderTime.ToString());
            }

            catch (MessageQueueException e)
            {
                Console.WriteLine("error 1");
                // Handle Message Queuing exceptions.
            }

            // Handle invalid serialization format.
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
            }

            // Catch other exceptions as necessary.

            return;
        }
    }
}
