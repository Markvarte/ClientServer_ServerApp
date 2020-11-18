﻿using System;
using System.Threading;
using System.IO.MemoryMappedFiles;

public enum ServerProgramType { FIFOtype, queue, mappedMemory }

namespace ClientServer_ServerApp
{
    class ServerProgram
    {
        static void Main(string[] args)
        {
            int clientChouse;
            // Create a new instance of the class.
            MyMSMQ myNewQueue_0 = new MyMSMQ();

            // Receive a message from a queue.
            clientChouse = Convert.ToInt32(myNewQueue_0.ReceiveMessage());

            ServerProgramType c = (ServerProgramType)(int)clientChouse; 
            switch (c) {
            case ServerProgramType.FIFOtype: // 0
                    //Klients sūta ziņojumus, bet serveris saņem, tiek veidoti jauni pavedieni, kuri atgriež klienta informāciju

                    MyFIFO myFIFO = new MyFIFO();


                    Thread ServerThread = new Thread(myFIFO.ThreadStartServer);


                    ServerThread.Start();
            break;

            case ServerProgramType.queue: // 1

                    // Create a new instance of the class.
                    MyMSMQ myNewQueue = new MyMSMQ();

                    Console.WriteLine("MSMQ Queuen serveris");
                    string receivedMgs;
                    while (true)
                    {
                        // Receive a message from a queue.
                        receivedMgs = myNewQueue.ReceiveMessage();
                        if (receivedMgs == "quit")
                        {
                            break;
                        }
                    }


            break;

            case ServerProgramType.mappedMemory: // 2
                    //Tiek palaists serveris

                    Console.WriteLine("Memory mapped failu serveris");

                    //Tiek izveidots dalītās atmiņas mehānisms un ierakstīta informācija failā

                    // Failu izveide
                    using (var file = MemoryMappedFile.CreateNew("myFile", int.MaxValue))
                    {
                        // satura veidosana
                        var bytes = new byte[24];
                        for (var i = 0; i < bytes.Length; i++)
                            // 24 burti alfabetiska seciba sakot no A
                            bytes[i] = (byte)(65 + i);

                        // satura ierakstisana failaa
                        using (var writer = file.CreateViewAccessor(0, bytes.Length))
                        {
                            writer.WriteArray(0, bytes, 0, bytes.Length);
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
