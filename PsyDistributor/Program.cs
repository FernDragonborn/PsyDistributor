using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Auth.OAuth2;
using PsyDistributor.APIs;
using static System.Console;
using static PsyDistributor.APIs.Crud;
using static PsyDistributor.Program;
using System.Threading.Tasks;

namespace PsyDistributor;

public static class Program
{
    public static void CountRequestTimeStats()
    {
        var request = ReadEntry(3, 1, "A2:A1764");
        var values = new List<DateTime>();
        string a = null;
        values.Add(DateTime.Parse(Convert.ToString(request[0][0])));
        foreach (var row in request)
        {
            foreach (var value in row)
            {
                a = value.ToString().Replace(",", " ");
                values.Add(Convert.ToDateTime(a));
            }
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
        Console.OutputEncoding = System.Text.Encoding.Unicode;
#pragma warning disable CS4014
        //TgBot.TgInit();
#pragma warning restore CS4014 
        //DbInit();
        //OneBox.GetToken("https://testingapi.1b.app");
        OneBox.OneBoxInit();
        //ReadEntryConsole(1, 1, "A2:G5");
        //ReadEntryByValueFilter(1, 1);
        Read();
    }
}