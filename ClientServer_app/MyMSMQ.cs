using System;

namespace ClientServer_ServerApp
{
    [Serializable]
    public class MyMSMQ
    {
        // This class represents an object the following example
        // sends to a queue and receives from a queue.

            public int orderId;
            public DateTime orderTime;
    }

}
