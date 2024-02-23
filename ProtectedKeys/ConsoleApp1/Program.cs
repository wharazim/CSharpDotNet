using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml;
using System.Security.Cryptography;
using System.Data;
using CrowKoko.ProtectedKeys;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ps = ConfigurationManager.GetSection("protectedKeys") as ProtectedKeySection;
            if (ps.Exception != null)
            {
                return;
            }

            var nm1 = ps["Name1"].Id;
            var nm2 = ps["test1"].Id;

            var nm3 = ps["test1"]["id"];
            var cs = ps["test3"]["client_secret"];
        }
    }
}
