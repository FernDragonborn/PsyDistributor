using System;
using System.Collections.Generic;
using System.IO;
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
        static string SpreadsheetId = "1nnBP0fQ5Li2OpwjRMVtLPfvPVaHBc3hp3-Jnd1z6aMk";
        static string sheet;//= "Основний лист";
        //static readonly string sheet = "Основний лист";
        static SheetsService service;

        /*static void MainCRUD(string[] args){
        }*/
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

            WriteLine("CRUD initialization successfull");
        }
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

        //entry = запись
        public static void CreateEntry(int bookId, int sheetId, string firstColumn, List<object> enteriesList)
        {
            BookAndSheetSelection(bookId, sheetId);
            //for the simplicity this methods helps you to only enter 1st column name
            int index = Array.IndexOf(ColumnNames, firstColumn) + enteriesList.Count;
            string secondColumn = ColumnNames[index];
            var range = $"{sheet}!{firstColumn}:{secondColumn}";
            

            var valueRange = new ValueRange();
            valueRange.Values = new List<IList<object>> { enteriesList };

            var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
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
        public static void ReadEntry_Console(int bookId, int sheetId, string cells)
        {
            var values = ReadEntry(bookId, sheetId, cells);
            
            if(values != null && values.Count > 0){
                foreach(var row in values)
                    WriteLine($"- {row[0]} | {row[1]} | {row[2]} | {row[3]} | {row [4]} | {row[5]}");
            } 
            else WriteLine("No data found");
        }
        public static void UpdateEntry(int bookId, int sheetId, string cell, List<object> enteriesList)
        {
            BookAndSheetSelection(bookId, sheetId);
            string range = $"{sheet}!{cell}";
            var valueRange = new ValueRange();

            //var objectList = new List<object>() {"updated"};
            valueRange.Values = new List<IList<object>> { enteriesList };

            var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest
                .ValueInputOptionEnum.USERENTERED;
            var updateResponse = updateRequest.Execute();
        }
        public static void DeleteEntry(int bookId, int sheetId, string cells)
        {
            string range = $"{sheet}!{cells}";
            var requestBody = new ClearValuesRequest();

            var deleteRequest = service.Spreadsheets.Values.Clear(requestBody, SpreadsheetId, range);
            var deleteResponse = deleteRequest.Execute();
        }
    }
}
