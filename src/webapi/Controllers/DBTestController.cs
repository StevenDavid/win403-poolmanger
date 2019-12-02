using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using webapi.Models;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    public class DBTestController : Controller
    {
        static int calls = 1; 
        static string ConnStr = Helpers.GetMSSQLConnectionString();
        static SqlConnection con = null;

        // GET api/dbtest
        [HttpGet]
        public IEnumerable<string> Get()
        {
            Console.WriteLine("DBTest was called.");
            return new string[] { "{'service-status': 'up'}" };
        }

        [HttpGet("scalar")]
        public string TestMSSQLRunScalar() {
            return MSSQLRunQuery("SELECT CONNECTIONPROPERTY('client_net_address') AS CNA ");
        }

        [HttpGet("scalar/{encodedSQL}")]
        public string TestMSSQLRunScalar(string encodedSQL) {
            byte[] decodedBytes = Convert.FromBase64String(encodedSQL);
            string query = System.Text.Encoding.UTF8.GetString(decodedBytes);
            return MSSQLRunQuery(query);
        }

        public string MSSQLRunQuery(string query)
        {
            DataConnectionViewModel ConResult = new DataConnectionViewModel(); 
            ConResult.Calls = calls;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //string ConnStr = Helpers.GetMSSQLConnectionString();

            Console.WriteLine("ConnectionString=" + ConnStr);

            //using (SqlConnection con = new SqlConnection(ConnStr)) {
            //    con.Open();
                if (con == null){
                    con = new SqlConnection(ConnStr);
                    con.Open();
                }

                Console.WriteLine("connection open");

                try {
                    var cmd = new SqlCommand(query, con);
                    var result = cmd.ExecuteScalar();
                    ConResult.IPAddress = result.ToString(); 
                    stopwatch.Stop();
                    ConResult.ElapsedTime = stopwatch.Elapsed.ToString();
                    ConResult.ElapsedTimeMilli = stopwatch.Elapsed.Milliseconds.ToString();

                    Console.WriteLine("Connection Succeeded! Time=" + ConResult.ElapsedTime);
                    
                    string output = JsonConvert.SerializeObject(ConResult);
                    Console.WriteLine("ConResult=" + output);
                    calls++; 
                    return output;
                    
                }
                catch(System.Data.Common.DbException ex) {
                    Console.WriteLine("Connection failed, message=" + ex.Message);
                    return "Something went wrong: " + ex.Message;
                }
            //}

        }

    }
}
