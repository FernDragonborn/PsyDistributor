using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Auth.OAuth2;
using static PsyDistributor.Crud;
using static System.Console;

namespace PsyDistributor 
{
    class Program
    {
        static void Main(string[] args)
        {
            DbInit();
            var oblist = new List<object>() { "Hello", "moto!" };
            CreateEntry(1, 1, "A", oblist);

            ReadLine();
            var uplist = new List<object>() { "updated" };
            UpdateEntry(1, 1, "C384", uplist);

            Read();
            DeleteEntry(1, 1, "B384:C386");
        }
    }
}