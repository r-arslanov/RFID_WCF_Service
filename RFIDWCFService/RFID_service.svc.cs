using System;
using System.Collections.Generic;
using RFIDWCFService.Additional;

namespace RFIDWCFService
{
    public class RFID_service : IRFID_service
    {
        #region fields
        public static Dictionary<string, RF600> dic_rfid = new Dictionary<string, RF600>();
        #endregion

        #region Default
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
        #endregion

        public string ConnectRFID(string ipPort, string name = "Default")
        {
            RF600 rfid = new RF600(ipPort, name);
            rfid.connectOPC();
            if (rfid.connected)
            {
                dic_rfid.Add(name, rfid);
                rfid.startScan();
            }
            return rfid.connected ? "OK Connect: " + name : "Error connect: " + name;
        }

        public string StopScan(string name)
        {
            string result = "Not contains in dictionary";
            if (dic_rfid.ContainsKey(name))
            {
                dic_rfid[name].stopScan();
                result = "Ok exec command";
            }
            return result;
        }

        public string StartScan(string name)
        {
            string result = "Not contains in dictionary";
            if (dic_rfid.ContainsKey(name))
            {
                dic_rfid[name].startScan();
                result = "Ok exec command";
            }
            return result;
        }

        public string ReadOnce(string name, int readPoint)
        {
            return dic_rfid[name].readTag(readPoint);
        }

        public string WriteOnce(string name, int readPoint, string newValue)
        {
            return dic_rfid[name].writeTag(readPoint, newValue); 
        }
    }
}
