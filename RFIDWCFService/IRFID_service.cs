using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace RFIDWCFService
{
    [ServiceContract(Namespace = "http://sybcom.rfid.api")]
    public interface IRFID_service
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ConnectRFID/{ipPort}/{name}")]
        string ConnectRFID(string ipPort, string name = "Default");

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/StopScan/{name}")]
        string StopScan(string name);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/StartScan/{name}")]
        string StartScan(string name);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ReadOnce/{name}/{readPoint}")]
        string ReadOnce(string name, int readPoint);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/WriteOnce/{name}/{readPoint}/{newValue}")]
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
