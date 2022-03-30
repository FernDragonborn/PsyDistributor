﻿using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Auth.OAuth2;

//сейчас прога работает с таблицей на гугл диске Папороти
//потом подрубить к основной
namespace PsyDistributor
{
    public class CRUD
    {
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string ApplicationName = "PsyDistributorApp";
        static readonly string SpreadsheetIdPsy = "1nnBP0fQ5Li2OpwjRMVtLPfvPVaHBc3hp3-Jnd1z6aMk";
        static readonly string SpreadsheetIdClient = "1EMfaPrLPFQkfLvo0-RLRcfGWU8v21j6mCAG4Q_HW6kg";
        //static readonly string sheet = "Основний лист";
        static SheetsService service;

        static void MainCRUD(string[] args)
        {
            GoogleCredential credential;
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read)){
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }

            service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer(){
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            
        }

        //entry = запись
        public void CreateEntry(){
            string range = $"{sheet}!A:G";
            var valueRange = new ValueRange();

            //добавляет в таблицу значения по столбикам
            var objectList = new List<object>() {"1","1","1","1","1","1"};
            valueRange.Values = new List<IList<object>> {objectList};

            service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range).ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest
                .ValueInputOptionEnum.USERENTERED;
            var appendResponse = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range).Execute();
        }
        public static void ReadEntry(int bookId, int sheetId, string range)
        {
            string SpreadsheetId = SpreadsheetIdPsy;
            string sheet = "Основний лист";
            if (bookId == 1) {
                SpreadsheetId = SpreadsheetIdPsy;
                if (sheetId == 1) sheet = "Основний лист";
                else if (sheetId == 2) sheet = "2га анкета";
                else Console.WriteLine("Error: wrong sheet id");
            }
            else if (bookId == 2) {
                SpreadsheetId = SpreadsheetIdClient;
                if (sheetId == 1) sheet = "РОБОЧА ПЛОЩИНА НОВЕ";
                else if (sheetId == 2) sheet = "НІЧНА ЗМІНА";
                else Console.WriteLine("Error: wrong sheet id");
            }
            else Console.WriteLine("Error: wrong book id");

            string fullRange = $"{sheet}!{range}";
            var request = service.Spreadsheets.Values.Get(SpreadsheetId, fullRange);

            var response = request.Execute();
            var values = response.Values;
            if(values != null && values.Count > 0){
                foreach(var row in values){
                    Console.WriteLine("{0} {1} | {2} | {3}", row[5], row[4], row[3], row[2], row[1]);
                }
            } 
            else{
                Console.WriteLine("No data found");
            }
        }
        public void UpdateEntry(){
            string range = $"{sheet}!D514";
            var valueRange = new ValueRange();

            var objectList = new List<object>() {"updated"};
            valueRange.Values = new List<IList<object>> {objectList};

            var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest
                .ValueInputOptionEnum.USERENTERED;
            var updateResponse = updateRequest.Execute();
        }
        public void DeleteEntry(){
            string range = $"{sheet}!A543:F";
            var requestBody = new ClearValuesRequest();

            var deleteRequest = service.Spreadsheets.Values.Clear(requestBody, SpreadsheetId, range);
            var deleteResponse = deleteRequest.Execute();
        }
    }
}
