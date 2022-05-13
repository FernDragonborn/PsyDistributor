using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Auth.OAuth2;
using static System.Console;


//сейчас прога работает с таблицей на гугл диске Папороти
//потом подрубить к основной
namespace PsyDistributor
{
    public static class Crud
    {
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string ApplicationName = "PsyDistributorApp";
        static readonly string SpreadsheetIdPsy = "1nnBP0fQ5Li2OpwjRMVtLPfvPVaHBc3hp3-Jnd1z6aMk";
        static readonly string SpreadsheetIdClient = "1EMfaPrLPFQkfLvo0-RLRcfGWU8v21j6mCAG4Q_HW6kg";
        static readonly string SpreadsheetIdRequest = "1IfkLxmyl09BKXKaozkRtXXODkUjvenZOxGmn_6mo3VY";
        static string SpreadsheetId = "1nnBP0fQ5Li2OpwjRMVtLPfvPVaHBc3hp3-Jnd1z6aMk";
        static string sheet;
        static SheetsService service;

        public static void DbInit()
        {
            //for ukr lang support
            Console.OutputEncoding = System.Text.Encoding.Default;
            //google authentication shit. Don't touch
            GoogleCredential credential;
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }
            service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            WriteLine("CRUD-module initialization successfull");
        }

        //public static void CreatePsySearcEngineFile() {
        //    WriteLine("Trying to create a psy engine file");
        //    IList<IList<object>> psyTable = ReadEntry(1, 1, "A2:G378");
        //    string path = @"D:\data\code\PsyDistributor\PsySearchEngineDB.txt";

        //    // Delete the file if it exists.
        //    if (File.Exists(path)) {
        //        File.Delete(path);
        //    }

        //    var lines = new List<string>();
        //    foreach (var line in psyTable) {
        //        foreach (var item in line) { 
        //            lines.Add(item.ToString());
        //        }
        //    }

        //    //Create the file.
        //    using (FileStream fs = File.Create(path))
        //    foreach (var line in lines) { }
        //    File.WriteAllLines(path, lines);

        //    lines = File.ReadAllLines(path).ToList();
        //    foreach (string line in lines) {
        //        WriteLine(line);
        //    }
        //}

        private static string BookAndSheetSelection(int bookId, int sheetId)
        {
            //bookId = 1 - Psychologist DB
            //bookId = 2 - Clients DB
            if (bookId == 1)
            {
                SpreadsheetId = SpreadsheetIdPsy;
                if (sheetId == 1) sheet = "Основний лист";
                else if (sheetId == 2) sheet = "2га анкета";
                else WriteLine("Error: wrong sheet id");
            }
            else if (bookId == 2)
            {
                SpreadsheetId = SpreadsheetIdClient;
                if (sheetId == 1) sheet = "РОБОЧА ПЛОЩИНА НОВЕ";
                else if (sheetId == 2) sheet = "НІЧНА ЗМІНА";
                else WriteLine("Error: wrong sheet id");
            }
            else if (bookId == 3)
            {
                SpreadsheetId = SpreadsheetIdRequest;
                if (sheetId == 1) sheet = "Відповіді форми (1)";
                else WriteLine("Error: wrong sheet id");
            }
            else WriteLine("Error: wrong book id");

            WriteLine($"BookID: {bookId}\nSheet name: {sheet}");
            return sheet;
        }

        readonly static string[] ColumnNames = {
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O",
            "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
            "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM",
            "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ"
        };

        private static void CheckNullInput(dynamic data)
        {
            var ErrNullIput = new Exception("Null input");
            if (data == null) throw ErrNullIput;
        }

        //entry = запись
        public static void CreateEntry(int bookId, int sheetId, string firstColumn, List<object> enteriesList)
        {
            CheckNullInput(enteriesList);
            BookAndSheetSelection(bookId, sheetId);
            //for the simplicity this methods helps you to only enter 1st column name
            int index = Array.IndexOf(ColumnNames, firstColumn) + enteriesList.Count;
            string secondColumn = ColumnNames[index];
            var range = $"{sheet}!{firstColumn}:{secondColumn}";

            var valueRange = new ValueRange();
            valueRange.Values = new List<IList<object>> { enteriesList };

            var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.
                ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.Execute();

        }
        public static IList<IList<object>> ReadEntry(int bookId, int sheetId, string cells)
        {
            BookAndSheetSelection(bookId, sheetId);
            string range = $"{sheet}!{cells}";

            SpreadsheetsResource.ValuesResource.GetRequest readRequest =
                service.Spreadsheets.Values.Get(SpreadsheetId, range);
            var response = readRequest.Execute();
            var values = response.Values;
            if (values != null && values.Count > 0)
                return values;
            else { WriteLine("No data found"); return values; }
        }
        public static void UpdateEntry(int bookId, int sheetId, string cell, List<object> enteriesList)
        {
            CheckNullInput(enteriesList);
            BookAndSheetSelection(bookId, sheetId);
            string range = $"{sheet}!{cell}";
            var valueRange = new ValueRange();

            //{"updated"} only 1 entry is supported
            valueRange.Values = new List<IList<object>> { enteriesList };

            var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest
                .ValueInputOptionEnum.USERENTERED;
            var updateResponse = updateRequest.Execute();
        }
        public static void DeleteEntry(int bookId, int sheetId, string cells)
        {
            BookAndSheetSelection(bookId, sheetId);
            string range = $"{sheet}!{cells}";
            var requestBody = new ClearValuesRequest();

            var deleteRequest = service.Spreadsheets.Values.Clear(requestBody, SpreadsheetId, range);
            var deleteResponse = deleteRequest.Execute();
        }
        public static void ReadEntry_Console(int bookId, int sheetId, string cells)
        {
            var values = ReadEntry(bookId, sheetId, cells);

            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                    WriteLine($"- {row[0]} | {row[1]} | {row[2]} | {row[3]} | {row[4]} | {row[5]}");
            }
            else WriteLine("No data found");
        }

        public static void ReadEntry_Filter(int bookId, int sheetId)
        {
            BookAndSheetSelection(bookId, sheetId);

            var dataFilters = new List<DataFilter>();
            var dataFilter = new DataFilter();
            dataFilter.A1Range = "Київ";
            dataFilters.Add(dataFilter);
            var requestBody = new BatchGetValuesByDataFilterRequest();
            requestBody.DataFilters = dataFilters;

            SpreadsheetsResource.ValuesResource.BatchGetByDataFilterRequest batchReadRequest =
                service.Spreadsheets.Values.BatchGetByDataFilter(requestBody, SpreadsheetId);

            var response = batchReadRequest.Execute();
            var values = JsonConvert.SerializeObject(response);

            WriteLine(JsonConvert.SerializeObject(values));
            WriteLine(JsonConvert.SerializeObject(response));
        }
    }
}