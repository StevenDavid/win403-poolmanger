using System;

namespace webapi.Models
{
    public class DataConnectionViewModel
    {
        public int Calls { get; set; }
        public string ElapsedTime { get; set; }
        public string ElapsedTimeMilli { get; set; }
        public string IPAddress { get; set; }

    }

    public class DB_CS {
        public string username { get; set; }
        public string password { get; set; }
        public string host { get; set; }        
        public string engine { get; set; }
        public string port {get; set;}
        public string dbInstanceIdentifier {get; set;}
    }
}