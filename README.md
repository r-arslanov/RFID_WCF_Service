# RFID_WCF_Service

## Первоначальная настройка
  1. В скомпилированом приложении в папке с .exe файлом находится файл "RFIDWCFService.exe.config"
  2. Участок кода ниже определяет по какому адресу:порту будет доступен сервер 
     ```xml
     <baseAddresses>
       <add baseAddress="http://localhost:8000/RFIDService"/>
     </baseAddresses>
     ```
     Необходимо изменить если порт 8000 недоступен, вместо localhost можно использовать локальное имя сервера
  2. Зарегистрировать RFIDWCFService.exe как службу windows
  3. Для выполнения метода необходимо отправить запрос по адресу [http://localhost:8000/RFIDService/<Метод>?<Параметры>](http://localhost:8000/RFIDService)
## Пример выполнения методов
### Перечень методов с параметрами
  ```C#
    // Подключение к считывателю
    ResultCommand ConnectRFID(string ipPort, string name = "Default"); 
    // Остановить постоянное сканирование
    ResultCommand StopScan(string name, int readPoint = 0);     
    // Запустить постоянное сканирование
    ResultCommand StartScan(string name, int readPoint = 0);    
    // Записать данные на карту
    ResultCommand WriteCardMemory(string name, int readPoint, string newValue, int mem = 1, int adr = 4); 
    // Считать данные с карты
    RfidAddress ReadCardMemory(string name, int readPoint);
  ```
### Описание методов
1. Подключение к считывателю
  ```C#
  ResultCommand ConnectRFID(string ipPort, string name = "Default"); 
  ```
  * ipPort - адрес считывателя в формате <ip-адрес>:<порт>
  * name - внутреннее имя считывателя, по умолчанию совпадает с точкой данных WinCC OA 
2. Остановить постоянное сканирование
  ```C#
  ResultCommand StopScan(string name, int readPoint = 0); 
  ```
  * name - адрес считывателя зарегестрированый в системе функцией ConnectRFID
  * readPoint - номер антенны (0-3)
2. Запустить постоянное сканирование
  ```C#
  ResultCommand StartScan(string name, int readPoint = 0); 
  ```
  * name - адрес считывателя зарегестрированый в системе функцией ConnectRFID
  * readPoint - номер антенны (0-3)
2. Записть данные на карту
  ```C#
    ResultCommand WriteCardMemory(string name, int readPoint, string newValue, int mem = 1, int adr = 4); 
  ```
  * name - адрес считывателя зарегестрированый в системе функцией ConnectRFID
  * readPoint - номер антенны (0-3)
  * newValue - Данные которые запишутся на карт (Строка в формате ByteArray) Допустимые символы (0-9, A-F)
  * mem - область памяти для записи (0-зарезервировано, 1-EPC, 2-TID, 3-USER_DATA)
  * adr - смещение в области памяти от начального регистра
2. Считать данные с карты
  ```C#
    RfidAddress ReadCardMemory(string name, int readPoint);
  ```
  * name - адрес считывателя зарегестрированый в системе функцией ConnectRFID 
  * readPoint - номер антенны (0-3)
### json-значения которые возврашают методы
1. RfidAddress
  * string USR - Область памяти пользователя
  * string TID - Область памяти TID
  * string EPC - Область памяти EPC
2. ResultCommand
  * int Status - Статус выполнения метода (0 - ОК, -1 - Ошибка)
  * string Comment - Комментарий к статусу
  * string Data - Данные которые вернул метод (Обычно имя и адрес считывателя)
### Принцип образования ссылок
Для формирования ссылки необходимо обратится к файлу конфигурации описаному в первой части. Это начала адреса.

Для вызова определенного метода к изначальному адресу необходимо добавить имя метода и параметры, например:
  
  для подключения считывателя с ip 192.168.2.10, портом 4840, именем RFID необходимо отправить GET запрос по адресу: [http://localhost:8000/RFIDService/ConnectRFID?ipPort=192.168.2.10:4840&name=RFID](http://localhost:8000/RFIDService/ConnectRFID?ipPort=192.168.2.10:4840&name=RFID)
  
  
