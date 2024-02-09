using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1.DB
{
    public static class Connection
    {
        private static readonly string _connectionString = "server=127.0.0.1;uid=root;pwd=root;database=mydb;charset=utf8;";

        public static string ConnectionString { get { return _connectionString; } }
    }
}
