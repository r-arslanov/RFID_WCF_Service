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

        public ResultCommand ConnectRFID(string ipPort, string name = "Default")
        {
            RF600 rfid = new RF600(ipPort, name);
            ResultCommand result = new ResultCommand();
            rfid.connectOPC();

            result.Data = "{name:"+name+",ipPort:"+ipPort+"}";
            if (rfid.connected)
            {
                dic_rfid.Add(name, rfid);
                rfid.startScan();
                result.Status = 0;
                result.Comment = "Successfull connection";
            }
            else
            {
                result.Status = -1;
                result.Comment = "Error connection";
            }
            return result;
        }

        public ResultCommand StopScan(string name, int readPoint = 0)
        {
            ResultCommand result = new ResultCommand();

            result.Status = -1;
            result.Comment = "RFID dictionary not contained " + name;
            result.Data = "{name:" + name + ",readPoint:" + readPoint + "}";

            if (dic_rfid.ContainsKey(name))
            {
                dic_rfid[name].stopScan(readPoint);
                result.Status = 0;
                result.Comment = "Succesfull stopScan " + name;
            }
            return result;
        }

        public ResultCommand StartScan(string name, int readPoint = 0)
        {
            ResultCommand result = new ResultCommand();

            result.Status = -1;
            result.Comment = "RFID dictionary not contained " + name;
            result.Data = "{name:" + name + ",readPoint:" + readPoint + "}";

            if (dic_rfid.ContainsKey(name))
            {
                dic_rfid[name].startScan(readPoint);
                result.Status = 0;
                result.Comment = "Succesfull startScan " + name;
            }
            return result;
        }

        public ResultCommand WriteCardMemory(string name, int readPoint, string newValue, int mem = 1, int adr = 4)
        {
            ResultCommand result = new ResultCommand();

            result.Status = -1;
            result.Comment = "Error write data";

            if (dic_rfid.ContainsKey(name))
            {
                result.Comment = dic_rfid[name].writeTag(readPoint, newValue, mem, adr);
                result.Data = "{name:" + name + ", memory:" + mem + ", adress:" + adr + ",newValue:" + newValue + "}";
                result.Status = 0;
            }
            return result;
        }

        public RfidAddress ReadCardMemory(string name, int readPoint)
        {
            RfidAddress rfa = new RfidAddress();

            rfa.EPC = dic_rfid[name].readTag(readPoint, 1, 4, 30);
            rfa.TID = dic_rfid[name].readTag(readPoint, 2, 0, 8);
            rfa.USR = dic_rfid[name].readTag(readPoint, 3, 0, 32);

            return rfa;
        }
    }
}
