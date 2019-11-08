using Microsoft.Win32;
using Opc.Ua;
using Opc.Ua.Client;
using RfidOpcUaForm;
using Siemens.UAClientHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RFIDWCFService.Additional
{
    
    public class RF600
    {
        #region Fields
        private UAClientHelperAPI myHelperApi;                        // Экземплят API считывателя (от Siemens)
        private UARfidMethodIdentifiers myRfidMethodIdentifiers;      // Экземпляр идентификаторов методов OPC UA (от Siemens)
        
        private string url;         // ip:port считывателя
        private string name;        // имя считывателя

        ushort rfidNamespaceIdx;    // Пространство имен считывателя (От Siemens, без него не работает)

        public  bool connected = false;

        private Subscription mySubscribe;
        #endregion
     
        #region Constructor
        public RF600(string url, string name = "One")
        {
            // Экземпляр API от Siemens
            myHelperApi = new UAClientHelperAPI();
            // Присваивание полям значений
            this.url = url;
            this.name = name;
        }
        #endregion

        #region Controls
        // Подключение к считывателю по OPC UA
        public void connectOPC()
        {
            // Точка подключения
            EndpointDescription eds = new EndpointDescription("opc.tcp://" + url + "/");
            myHelperApi.CertificateValidationNotification += new CertificateValidationEventHandler(Notification_ServerCertificate);
            myHelperApi.KeepAliveNotification += new KeepAliveEventHandler(Notification_KeepAlive);
            try
            {
                myHelperApi.Connect(eds, true, false, "", "").Wait();
                Debug.WriteLine("___Success connection___");
            }
            catch
            {
                Debug.WriteLine("___Error connection___");
            }
            // Проверка результатов подключения
            if (myHelperApi.Session != null && myHelperApi.Session.Connected)
            {
                mySubscribe = myHelperApi.Subscribe(1000);
                myHelperApi.CertificateValidationNotification += new CertificateValidationEventHandler(Notification_ServerCertificate);
                myHelperApi.KeepAliveNotification += new KeepAliveEventHandler(Notification_KeepAlive);

                // Запрос методов и пространства имен
                myRfidMethodIdentifiers = new UARfidMethodIdentifiers(myHelperApi);
                rfidNamespaceIdx = GetRfidNamespaceIndex();
            }

            this.connected =  (myHelperApi.Session != null && myHelperApi.Session.Connected);

        }
        // Запуск сканирования 
        public void startScan( )
        {
            NodeId met = null;
            NodeId obj = null;
            // Запрос методов и объектов
            met = new NodeId(myRfidMethodIdentifiers.MethodIdList[0].Find(x => x.method == MethodToCall.ScanStart).methodNodeId, rfidNamespaceIdx);
            obj = new NodeId(myRfidMethodIdentifiers.MethodIdList[0].Find(x => x.method == MethodToCall.ScanStart).objectNodeId, rfidNamespaceIdx);
            // Выходные и входные параметры
            IList<object> cs = new List<object>();
            ScanSettings scs = new ScanSettings
            {
                DataAvailable = false,  // Хз что это (Изменение значения видимых результатов не дает)
                Duration = 0D,          // Время задержки между циклами опроса
                Cycles = 0              // Количество циклов
            };

            try
            {
                // Запрос на выполнение методаа
                cs = myHelperApi.Session.Call(obj, met, scs);
                Debug.WriteLine(url + " | Success called start read");
            }
            catch
            {
                Debug.WriteLine(url + " | Error called start read");
            }
        }
        // Остановка сканирования (Содержимое идентично команде StartScan()
        public void stopScan()
        {
            NodeId met = new NodeId(myRfidMethodIdentifiers.MethodIdList[0].Find(x => x.method == MethodToCall.ScanStop).methodNodeId, rfidNamespaceIdx);
            NodeId obj = new NodeId(myRfidMethodIdentifiers.MethodIdList[0].Find(x => x.method == MethodToCall.ScanStop).objectNodeId, rfidNamespaceIdx);

            IList<object> cs = new List<object>();
            // Нет конфигурационных элементов (вместо них отправляется null)
            try
            {
                cs = myHelperApi.Session.Call(obj, met, null);
                Debug.WriteLine(url + " | Success called stop read");
            }
            catch
            {
                Debug.WriteLine(url + " | Error called stop read");
            }
        }
        // Чтения параметров с карты
        public string readTag(int readPoint, int mem, int adr, int size)
        {
            string result = "No transponder";   // Результат запроса
            NodeId met = null;
            NodeId obj = null;
            // Объекты и методы подключения
            met = new NodeId(myRfidMethodIdentifiers.MethodIdList[readPoint].Find(x => x.method == MethodToCall.ReadTag).methodNodeId, rfidNamespaceIdx);
            obj = new NodeId(myRfidMethodIdentifiers.MethodIdList[readPoint].Find(x => x.method == MethodToCall.ReadTag).objectNodeId, rfidNamespaceIdx);

            IList<object> cs = new List<object>();
            // Инициализация параметров для выполнения метода
            object[] inp = new object[6];
            ScanData sd = new ScanData();

            inp[0] = sd;                                // Не используется
            inp[1] = "RAW:BYTES";                       // Тип входных параметров (Не используется)
            inp[2] = (ushort)mem;                       // Индекс памяти из которой необходимо считывать информацию (0-RESERVED, 1-EPC, 2-TID, 3-UD)
            inp[3] = (uint)adr;                         // Смещение в памяти (Адрес начала)
            inp[4] = (uint)size;                        // Длинна масива
            inp[5] = ConvertStringToByteArray("");      // Пароль от считывателя (если используется)

            try
            {
                // Вызов функции считывания
                cs = myHelperApi.Session.Call(obj, met, inp);
                // Статус результата
                if ((AutoIdOperationStatusEnumeration)cs[1] == 0)
                {
                    result = ConvertByteArrayToString((byte[])cs[0]).ToUpper();
                }
            }
            catch
            {
                result = this.url + " | Error read tag";
            }
            // Запись результата в реестр
            return result;
        }
        // Запись команд на карту
        public string writeTag(int readPoint, string newValue = "", int mem = 1, int adr = 4)
        {
            // Инициализация переменных
            string result = "No transponder";

            byte[] newByteValue = ConvertStringToByteArray(newValue);         // Нет проверки на количество бит информации

            NodeId met = null;
            NodeId obj = null;

            met = new NodeId(myRfidMethodIdentifiers.MethodIdList[readPoint].Find(x => x.method == MethodToCall.WriteTag).methodNodeId, rfidNamespaceIdx);
            obj = new NodeId(myRfidMethodIdentifiers.MethodIdList[readPoint].Find(x => x.method == MethodToCall.WriteTag).objectNodeId, rfidNamespaceIdx);

            IList<object> cs = new List<object>();
            //  Инициализация входных параметров
            object[] inp = new object[6];
            ScanData sd = new ScanData();

            inp[0] = sd;                                // Не используется
            inp[1] = "RAW:BYTES";                       // Тип входных параметров
            inp[2] = (ushort)mem;                       // Индекс памяти (0-RESERVED, 1-EPC, 2-TID, 3-UD)
            inp[3] = (uint)adr;                         // Смещение адресации
            inp[4] = newByteValue;                      // Новое значение (Запишется на карту)
            inp[5] = ConvertStringToByteArray("");      // Пароль (Если установлен)

            try
            {
                cs = myHelperApi.Session.Call(obj, met, inp);
                result = "New EPC | " + newValue.ToUpper();
            }
            catch
            {
                result = this.url + " | Error write tag";
            }

            return result;
        }
        #endregion

        // Заимствованы из проекта Siemens
        #region Helper 
        public byte[] ConvertStringToByteArray(string hex)
        {
            // If string is not codable caused by the length is not even add a string 0 at the front.
            if (hex.Length % 2 != 0)
            {
                return null;
            }

            int numberChars = hex.Length;

            var bytes = new byte[numberChars / 2];//create a new byteArray with the half of the length of the previous hex

            try
            {
                for (var i = 0; i < numberChars; i += 2)
                {
                    bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);//
                }
                return bytes;
            }
            catch
            {
                return null;
            }
        }

        public string ConvertByteArrayToString(byte[] arr)
        {
            StringBuilder hex = new StringBuilder(arr.Length * 2);
            foreach (byte b in arr)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        private ushort GetRfidNamespaceIndex()
        {
            ushort nameSpaceIndex = 0;

            ReferenceDescriptionCollection refDescCol = new ReferenceDescriptionCollection();
            refDescCol = myHelperApi.BrowseRoot();

            //Browse to variable "AutoIdModelVersion" (mandatory in AutoID) in RfidReaderDeviceType object to find out namespace
            foreach (ReferenceDescription refDescA in refDescCol)
            {
                if (refDescA.BrowseName.Name == "Objects")
                {
                    refDescCol = myHelperApi.BrowseNode(refDescA);
                    foreach (ReferenceDescription refDescB in refDescCol)
                    {
                        if (refDescB.BrowseName.Name == "DeviceSet")
                        {
                            refDescCol = myHelperApi.BrowseNode(refDescB);
                            foreach (ReferenceDescription refDescC in refDescCol)
                            {
                                if (refDescC.TypeDefinition == new ExpandedNodeId(RfidOpcUaForm.AutoID.ObjectTypes.RfidReaderDeviceType, (ushort)myHelperApi.GetNamespaceIndex(RfidOpcUaForm.AutoID.Namespaces.AutoID)))
                                {
                                    refDescCol = myHelperApi.BrowseNode(refDescC);
                                    foreach (ReferenceDescription refDescD in refDescCol)
                                    {
                                        if (refDescD.BrowseName.Name == "AutoIdModelVersion")
                                        {
                                            nameSpaceIndex = refDescD.NodeId.NamespaceIndex;
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }

            return nameSpaceIndex;
        }

        private void Notification_KeepAlive(Session session, KeepAliveEventArgs e)
        {
            // check for events from discarded sessions.
            if (!Object.ReferenceEquals(session, myHelperApi.Session))
            {
                return;
            }
        }

        private void Notification_ServerCertificate(CertificateValidator sender, CertificateValidationEventArgs e)
        {
            try
            {
                //Always accept
                e.Accept = true;
            }
            catch
            {
                ;
            }
        }

        #endregion

    }
}
