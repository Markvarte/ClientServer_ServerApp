using System;
using System.Messaging;

namespace ClientServer_ServerApp
{
    [Serializable]
    public class MyMSMQ
    {
        //**************************************************
        // Receives a message containing an Order.
        //**************************************************

        public void ReceiveMessage()
        {
            // Connect to the a queue on the local computer.
            MessageQueue myQueue = new MessageQueue(".\\private$\\new");

            // Set the formatter to indicate body contains an mgs.
            myQueue.Formatter = new XmlMessageFormatter(new Type[]
                {typeof(string)});

            try
            {
                // Receive and format the message.
                Message myMessage = myQueue.Receive();

                Console.WriteLine("Receive message... ");

                string clientMgs = (string)myMessage.Body;

                // Display message information.
                Console.WriteLine("Client message is: " + clientMgs);
            }

            catch (MessageQueueException e)
            {
                Console.WriteLine(e.Message);
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
