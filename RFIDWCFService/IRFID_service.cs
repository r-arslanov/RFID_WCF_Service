using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace RFIDWCFService
{
    [ServiceContract(Namespace ="http://sybcom.rfid.api")]
    public interface IRFID_service
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        [OperationContract]
        [WebGet]
        string ConnectRFID(string ipPort, string name = "Default");

        [OperationContract]
        [WebGet]
        string StopScan(string name);

        [OperationContract]
        [WebGet]
        string StartScan(string name);

        [OperationContract]
        [WebGet]
        string ReadOnce(string name, int readPoint);

        [OperationContract]
        [WebGet]
        string WriteOnce(string name, int readPoint, string newValue);
    }

    // Используйте контракт данных, как показано в примере ниже, чтобы добавить составные типы к операциям служб.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
