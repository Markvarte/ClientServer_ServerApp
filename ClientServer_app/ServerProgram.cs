using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;


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

            //Klients sūta ziņojumus, bet serveris saņem, tiek veidoti jauni pavedieni, kuri atgriež klienta informāciju

            ServerProgram Server = new ServerProgram();


            Thread ServerThread = new Thread(Server.ThreadStartServer);


            ServerThread.Start();

        }

    }
}
