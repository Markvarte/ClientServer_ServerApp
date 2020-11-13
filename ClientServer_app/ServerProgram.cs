using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.IO.MemoryMappedFiles;
public enum ServerProgramType { FIFO, queue, mappedMemory }

namespace ClientServer_ServerApp
{
    class ServerProgram
    {
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
            ServerProgramType c = ServerProgramType.mappedMemory; //  for now
            switch (c) {
            case ServerProgramType.FIFO: // 0
                    //Klients sūta ziņojumus, bet serveris saņem, tiek veidoti jauni pavedieni, kuri atgriež klienta informāciju

                    ServerProgram Server = new ServerProgram();


                    Thread ServerThread = new Thread(Server.ThreadStartServer);


                    ServerThread.Start();
            break;

            case ServerProgramType.queue: // 1
                    

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

    }
}
