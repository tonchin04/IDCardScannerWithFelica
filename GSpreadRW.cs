using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace IDCardScannerWithFelica
{
    class GSpreadRW : IDisposable

    {
        public string JsonFilePath
        {
            get { return Properties.Settings.Default.JsonFilePath; }
            set { Properties.Settings.Default.JsonFilePath = value; }
        }
        public string SpreadSheetID
        {
            get { return Properties.Settings.Default.SpreadSheetID; }
            set { Properties.Settings.Default.SpreadSheetID = value; }
        }

        public string StudentListName
        {
            get { return Properties.Settings.Default.StudentListName; }
            set { Properties.Settings.Default.StudentListName = value; }
        }

        public string ScanListName
        {
            get { return Properties.Settings.Default.ScanListName; }
            set { Properties.Settings.Default.ScanListName = value; }
        }

        public string StudentNameRow
        {
            get { return Properties.Settings.Default.StudentNameRow; }
            set { Properties.Settings.Default.StudentNameRow = value; }
        }

        public string StudentIDRow
        {
            get { return Properties.Settings.Default.StudentIDRow; }
            set { Properties.Settings.Default.StudentIDRow = value; }
        }
        public string ServiceAccountEmail
        {
            get { return Properties.Settings.Default.ServiceAccountEmail; }
            set { Properties.Settings.Default.ServiceAccountEmail = value; }
        }

        public string EntryNumberRow
        {
            get { return Properties.Settings.Default.EntryNumberRow; }
            set { Properties.Settings.Default.EntryNumberRow = value; }
        }
        public string NodeName
        {
            get { return Properties.Settings.Default.NodeName; }
            set { Properties.Settings.Default.NodeName = value; }
        }
        // Google Sheet Apiのスコープとアプリ名
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string ApplicationName = "cardscan";

        private int ConvertInt(string alphabet)
        {
            char[] rev = alphabet.Reverse().ToArray();

            int index = 0;
            for (int i = 0, len = rev.Length; i < len; ++i)
            {
                index += (rev[i] - 'A' + 1) * (int)Math.Pow(26, i);
            }
            return index;
        }

        public string ReadName(string studentid)
        {
            try
            {
                ServiceAccountCredential credential;

                // credentialを取得
                using (var stream = new FileStream(@JsonFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    credential = (ServiceAccountCredential)GoogleCredential.FromStream(stream).UnderlyingCredential;

                    var initializer = new ServiceAccountCredential.Initializer(credential.Id)
                    {
                        User = ServiceAccountEmail,
                        Key = credential.Key,
                        Scopes = Scopes
                    };
                    credential = new ServiceAccountCredential(initializer);
                }

                // Google Sheets API サービスを作る
                var service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                // スプレッドシート設定
                string spreadsheetId = SpreadSheetID;
                string studentListName = StudentListName;
                string sheetName = ScanListName;

                var range = studentListName + "!A:J";
                var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
                var response = request.Execute();
                var values = response.Values;

                string searchStudentNumber = studentid;
                int currentRow = 0;

                int StudentIDRowNum = ConvertInt(StudentIDRow) - 1;
                int StudentNameRowNum = ConvertInt(StudentNameRow) - 1;
                string? currentNumber = null;

                if (values != null && values.Count > 0)
                {
                    foreach (var row in values)
                    {
                        currentRow++;
                        currentNumber = (string)values[currentRow][StudentIDRowNum];
                        if (currentNumber.ToUpper() == searchStudentNumber.ToUpper())
                        {
                            string? ReadStudentName = Convert.ToString(values[currentRow][StudentNameRowNum]);
                            if (ReadStudentName == null)
                            {
                                return "学生番号に一致する名前は登録されていません";
                            }
                            else
                            {
                                return ReadStudentName;
                            }
                        }
                    }
                    return "Error";
                }
                else
                {
                    return "参加者リストが取得できませんでした。";
                }
            }
            catch (Exception eGoogleApi)
            {
                string errorMes;
                errorMes = "" + eGoogleApi;
                if (errorMes.Substring(0, 34) == "System.ArgumentOutOfRangeException")
                {
                    return "未エントリーの学生番号です";
                }
                return "Google APIへの接続に失敗しました";
            }

        }

        public int ReadEntryNum(string studentid)
        {
            try
            {
                ServiceAccountCredential credential;

                // credentialを取得
                using (var stream = new FileStream(@JsonFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    credential = (ServiceAccountCredential)GoogleCredential.FromStream(stream).UnderlyingCredential;

                    var initializer = new ServiceAccountCredential.Initializer(credential.Id)
                    {
                        User = ServiceAccountEmail,
                        Key = credential.Key,
                        Scopes = Scopes
                    };
                    credential = new ServiceAccountCredential(initializer);
                }

                // Google Sheets API サービスを作る
                var service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                // スプレッドシート設定
                string spreadsheetId = SpreadSheetID;
                string studentListName = StudentListName;
                string sheetName = ScanListName;

                var range = studentListName + "!A:J";
                var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
                var response = request.Execute();
                var values = response.Values;

                string searchStudentNumber = studentid;
                int ReadEntryNumber = 0;
                int currentRow = 0;

                int StudentIDRowNum = ConvertInt(StudentIDRow) - 1;
                int EntryRowNum = ConvertInt(EntryNumberRow) - 1;
                string? currentNumber = null;

                if (values != null && values.Count > 0)
                {
                    foreach (var row in values)
                    {
                        currentRow++;
                        currentNumber = (string)values[currentRow][StudentIDRowNum];
                        if (currentNumber.ToUpper() == searchStudentNumber.ToUpper())
                        {
                            ReadEntryNumber = Convert.ToInt32(values[currentRow][EntryRowNum]);
                            return ReadEntryNumber;
                        }
                    }
                    return -1;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception eGoogleApi)
            {
                string errorMes;
                errorMes = "" + eGoogleApi;
                MessageBox.Show(errorMes);
                if (errorMes.Substring(0, 34) == "System.ArgumentOutOfRangeException")
                {
                    //StatusUpdate("未エントリーの学生番号です。");
                    return -1;
                }
                //StatusUpdate("Google APIへの接続に失敗しました。");
                return -2;
            }

        }

        public int SheetWrite(DateTime dt, string studentid, string idm)
        {
            try
            {
                ServiceAccountCredential credential;

                // credentialを取得
                using (var stream = new FileStream(@JsonFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    credential = (ServiceAccountCredential)GoogleCredential.FromStream(stream).UnderlyingCredential;

                    var initializer = new ServiceAccountCredential.Initializer(credential.Id)
                    {
                        User = ServiceAccountEmail,
                        Key = credential.Key,
                        Scopes = Scopes
                    };
                    credential = new ServiceAccountCredential(initializer);
                }

                // Google Sheets API サービスを作る
                var service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                // スプレッドシート設定
                string spreadsheetId = SpreadSheetID;
                string studentListName = StudentListName;
                string sheetName = ScanListName;
                string nodename = NodeName;
                var wv = new List<IList<object>>()
                    {
                        new List<object>{ dt.ToString("yyyy/MM/dd HH:mm:ss"), studentid, "=IFERROR(index('" + studentListName + "'!" + StudentNameRow + ":" + StudentNameRow + ",match(TO_PURE_NUMBER(INDIRECT(ADDRESS(ROW(),2,4))),'" + studentListName + "'!" + StudentIDRow + ":" + StudentIDRow + ",0)),\"一致なし\")", idm, nodename, "=IFERROR(index('" + studentListName + "'!" + EntryNumberRow + ":" + EntryNumberRow + ",match(TO_PURE_NUMBER(INDIRECT(ADDRESS(ROW(), 2, 4))), '" + studentListName + "'!" + StudentIDRow + ":" + StudentIDRow + ", 0)),\"一致なし\")" }
                    };
                var body = new ValueRange() { Values = wv };
                var req = service.Spreadsheets.Values.Append(body, spreadsheetId, sheetName + "!A1");
                req.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var result = req.Execute();

                return 0;
            }
            catch (Exception eGoogleApi)
            {
                string errorMes;
                errorMes = "" + eGoogleApi;
                if (errorMes.Substring(0, 39) == "error:System.ArgumentOutOfRangeException")
                {
                    return 1;
                }

                return -1;
            }

        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
