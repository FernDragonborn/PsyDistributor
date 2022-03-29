using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Auth.OAuth2;

//сейчас прога работает с таблицей на гугл диске Папороти
//потом подрубить к основной
namespace PsyDistributor
{
    class Program
    {
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string ApplicationName = "PsyDistributorApp";
        static readonly string SpreadsheetId = "1nnBP0fQ5Li2OpwjRMVtLPfvPVaHBc3hp3-Jnd1z6aMk";
        static readonly string sheet = "Основний лист";
        static SheetsService service;

        static void Main(string[] args)
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
        static void CreateEntry(){
            var range = $"{sheet}!A:G";
            var valueRange = new ValueRange();

            //добавляет в таблицу значения по столбикам
            var objectList = new List<object>() {"1","1","1","1","1","1"};
            valueRange.Values = new List<IList<object>> {objectList};

            var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest
                .ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.Execute();
        }
        static void ReadEntry()
        {
            var range = $"{sheet}!A1:G10";
            var request = service.Spreadsheets.Values.Get(SpreadsheetId, range);

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
        static void UpdateEntry(){
            var range = $"{sheet}!D514";
            var valueRange = new ValueRange();

            var objectList = new List<object>() {"updated"};
            valueRange.Values = new List<IList<object>> {objectList};

            var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest
                .ValueInputOptionEnum.USERENTERED;
            var updateResponse = updateRequest.Execute();
        }
        static void DeleteEntry(){
            var range = $"{sheet}!A543:F";
            var requestBody = new ClearValuesRequest();

            var deleteRequest = service.Spreadsheets.Values.Clear(requestBody, SpreadsheetId, range);
            var deleteResponse = deleteRequest.Execute();
        }
    }
}
