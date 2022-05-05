using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Auth.OAuth2;
using static System.Console;
using static PsyDistributor.Crud;
using static PsyDistributor.Program;
using System.Globalization;

namespace PsyDistributor
{
    public static class Program
    {
        public static void countRequestTimeStats()
        {
            var request = ReadEntry(3, 1, "A2:A1764");
            var values = new List<DateTime>();
            string a = null;
            values.Add(DateTime.Parse(Convert.ToString(request[0][0])));
            foreach (var row in request)
                foreach (var value in row)
                {
                    a = value.ToString().Replace(",", " ");
                    values.Add(Convert.ToDateTime(a));
                }

            var hours = new int[24];
            for (int i = 0; i < values.Count; i++)
                hours[values[i].Hour]++;

            int num = 0;
            foreach (var value in hours)
                num += value;
            WriteLine($"\n\tвсего: \t\t{num} запросов\n");

            var shifts = new int[8];
            for (int i = 0; i < shifts.Length; i++)
                shifts[i] += hours[i * 3] + hours[i * 3 + 1] + hours[i * 3 + 2];

            int b = 1;
            foreach (var item in shifts)
            {
                double percent = Convert.ToDouble(item) / values.Count * 100;
                percent = Math.Round(percent, 1);
                WriteLine($"\t{(b - 1) * 3}-{b * 3} смена: \t{item} \t {percent}%");
                b++;
            }

            WriteLine("\n");
            b = 0;
            foreach (var item in hours)
            {
                double percent = Convert.ToDouble(item) / values.Count * 100;
                percent = Math.Round(percent, 1);
                WriteLine($"\t{b} час:  \t{item} \t {percent}%");
                b++;
            }
        }
    }
    class App
    {
        static void Main()
        {
            DbInit();
            /*var oblist = new List<object>() { "Hello", "moto!" };
            * CreateEntry(1, 1, "A", oblist);
            * var uplist = new List<object>() { "updated" };
            * UpdateEntry(1, 1, "C384", uplist);
            */

            /*WriteLine("Try to search a psy for 177");
            IList<IList<object>> entry = ReadEntry(2, 1, "A177:I");
            if (entry != null && entry.Count > 0)
            {
                foreach (var row in entry)
                    WriteLine($"- {row[0]} | {row[1]} | {row[2]} | {row[3]} | {row[4]} | {row[5]} " +
                        $"| {row[6]} | {row[7]} | {row[8]}");
            }
            else WriteLine("No data found");

            IList<IList<object>> psyList = ReadEntry(1, 1, "A177:I");
            if (entry != null && psyList.Count > 0)
            {
                foreach (var row in psyList)
                    WriteLine($"- {row[0]} | {row[1]} | {row[2]} | {row[3]} | {row[4]} | {row[5]} " +
                        $"| {row[6]} | {row[7]} | {row[8]}");
            }
            else WriteLine("No data found");*/
            //var values = new List<string>();



            //if (values != null && values.Count > 0)
            //{
            //    foreach (var item in values)
            //        WriteLine($"-  {item}");
            //}
            //else WriteLine("No data found");

            countRequestTimeStats();
        }
    }
}