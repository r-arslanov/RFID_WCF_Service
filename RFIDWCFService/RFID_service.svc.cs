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

        public RfidAddress ReadCardMemory(string name, int readPoint)
        {
            RfidAddress rfa = new RfidAddress();

            rfa.EPC = dic_rfid[name].readTag(readPoint, 1, 4, 30);
            rfa.TID = dic_rfid[name].readTag(readPoint, 2, 0, 8);
            rfa.USR = dic_rfid[name].readTag(readPoint, 3, 0, 32);

            return rfa;
        }
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
            return dic_rfid[name].readTag(readPoint, 1, 4, 12);
        }

        public string WriteOnce(string name, int readPoint, string newValue, int mem = 1, int adr = 4)
        {
            return dic_rfid[name].writeTag(readPoint, newValue, mem, adr); 
        }
    }
}
