using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace RFIDWCFService
{
    [ServiceContract(Namespace = "http://sybcom.rfid.api")]
    public interface IRFID_service
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string ConnectRFID(string ipPort, string name = "Default");

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string StopScan(string name);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string StartScan(string name);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string ReadOnce(string name, int readPoint);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string WriteOnce(string name, int readPoint, string newValue, int mem = 1, int adr = 4);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        RfidAddress ReadCardMemory(string name, int readPoint);
    }

    [DataContract]
    public class RfidAddress
    {
        string usr = "";
        string tid = "";
        string epc = "";

        [DataMember]
        public string USR
        {
            get { return usr; }
            set { usr = value; }
        }
        [DataMember]
        public string TID
        {
            get { return tid; }
            set { tid = value; }
        }
        [DataMember]
        public string EPC
        {
            get { return epc; }
            set { epc = value; }
        }
    }
}
