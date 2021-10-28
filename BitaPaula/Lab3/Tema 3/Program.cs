using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tema_3
{
    class Program
    {
        private static DriveService _service;
        private static string _token;
        static void Main(string[] args)
        {
            Initialize();
            UploadFile();
        }

        static void Initialize()
        {
            string[] scopes = new string[]
            {
                DriveService.Scope.Drive,
                DriveService.Scope.DriveFile
            };

            var clientId = "954487004602-h5u6hv1b5mbfatikgdieqkth6akbuvco.apps.googleusercontent.com";
            var clientSecret = "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";

            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync
            (
                new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                },
                scopes,
                Environment.UserName,
                CancellationToken.None,

                new FileDataStore("Daimto.GoogleDrive.Auth.Store1")
            ).Result;

            _service = new DriveService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });

            _token = credential.Token.AccessToken;

            Console.WriteLine("Token: " + credential.Token.AccessToken);

            GetMyFiles();
        }

        static void GetMyFiles()
        {
            var request = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/drive/v3/files?q='root'%20in%20parents");
            request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + _token);

            using (var response = request.GetResponse())
            {
                using (Stream data = response.GetResponseStream())
                using (var reader = new StreamReader(data))
                {
                    string text = reader.ReadToEnd();
                    var myData = JObject.Parse(text);
                    foreach (var file in myData["files"])
                    {
                        if (file["mimeType"].ToString() != "application/vnd.google-apps.folder")
                        {
                            Console.WriteLine("File name: " + file["name"]);
                        }
                    }
                }
            }
        }

        static void UploadFile()
        {
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            writer.WriteLine("Acest text va aparea in fisier.");
            writer.Flush();

            var file = new Google.Apis.Drive.v3.Data.File();
            file.Name = "Fisier.txt";
            file.MimeType = MediaTypeNames.Text.Plain;
            file.Parents = new string[] { "1ZjSNfPYRgCvu7DO4IzaiRwB9aJApugaG" };
            var fileCreateRequest = _service.Files.Create(file, stream, MediaTypeNames.Text.Plain);
            fileCreateRequest.Upload();
        }

    }
}
