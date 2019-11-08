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
        ResultCommand ConnectRFID(string ipPort, string name = "Default"); 

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        ResultCommand StopScan(string name, int readPoint = 0);     

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        ResultCommand StartScan(string name, int readPoint = 0);    
        
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        ResultCommand WriteCardMemory(string name, int readPoint, string newValue, int mem = 1, int adr = 4); 

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

    [DataContract]
    public class ResultCommand
    {
        int status = 0;
        string comment;
        string data;

        [DataMember]
        public int Status
        {
            get { return status; }
            set { status = value; }
        }
        [DataMember]
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }
        [DataMember]
        public string Data
        {
            get { return data; }
            set { data = value; }
        }
    }
}
