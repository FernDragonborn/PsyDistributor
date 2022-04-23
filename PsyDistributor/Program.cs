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
            WriteLine("Started");
            Init();
            WriteLine("Processing request");
            ReadEntry(1, 1, "A:G");
        }
    }
}