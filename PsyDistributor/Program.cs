using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Auth.OAuth2;

namespace PsyDistributor 
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hii");
            //Crud.ReadEntry(1,1,"A:G");
            Crud.ReadEntry();
        }
    }
}