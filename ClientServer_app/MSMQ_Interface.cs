using System.ServiceModel;

namespace ClientServer_ServerApp
{
    [ServiceContract]
    interface MSMQ_Interface
    {
            [OperationContract(IsOneWay = true)]
            void ShowReceivedMessageInDebug(string message);

    }
}
