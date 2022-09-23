using FelicaLib;
using System;
using System.Text;

namespace IDCardScannerWithFelica
{
    public class Scan : IDisposable
    {
        public void Dispose()
        {

        }

        public string BytesToHexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public string GetIDm(int SystemCode)
        {
            try
            {
                using (Felica felica = new())
                {
                    felica.Polling((int)SystemCode);
                    byte[] data = felica.IDm();
                    string idm = "";
                    idm = BytesToHexString(data);
                    return idm;
                }
            }
            catch (Exception ex)
            {
                return "Error : " + ex.Message;
            }
        }

        public string GetStudentID(int SystemCode, int IDServiceCode, int ReadAddr)
        {
            try
            {
                using (Felica felica = new())
                {
                    felica.Polling(SystemCode);
                    byte[]? data = felica.ReadWithoutEncryption(IDServiceCode, ReadAddr);
                    string StudentID = "";
                    if (data == null)
                    {
                        return "Error";
                    }
                    else
                    {
                        StudentID = Encoding.UTF8.GetString(data).Substring(0, 7);
                        return StudentID;
                    }

                }
            }
            catch (Exception ex)
            {
                return "Error" + ex.Message;
            }
        }
    }
}
