using FelicaLib;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Input;

namespace IDCardScannerWithFelica
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // GoogleSpreadSheetに送信するデータを格納
        public class StudentList
        {
            public int No { get; set; }
            public string? ScanTime { get; set; }
            public string? StudentID { get; set; }
            public string? StudentName { get; set; }
            public string? StudentIDm { get; set; }
            public string? StudentEntryNumber { get; set; }
            public string? Status { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            var dataList = new ObservableCollection<StudentList>();
            dataGrid.ItemsSource = dataList;
            dataGrid.CanUserResizeColumns = true;
            dataGrid.CanUserResizeRows = false;
            dataGrid.CanUserReorderColumns = false;
            dataGrid.CanUserDeleteRows = false;
            dataGrid.CanUserSortColumns = false;
            dataGrid.CanUserAddRows = false;
        }

        private void IDmTestButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using Scan scan = new();
                string idm = scan.GetIDm((int)SystemCode.Any);
                UpdateStatus("読み込んだカードのIDm : " + idm);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void StudentIDTestButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using Scan scan = new();
                string studentid = scan.GetStudentID((int)SystemCode.Any, 0x300B, 0);
                UpdateStatus("読み込んだカードの学生番号 : " + studentid);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void ReadCard()
        {
            using Scan scan = new();
            string studentid = await Task.Run(() => scan.GetStudentID((int)SystemCode.Any, 0x300B, 0));
            string idm = await Task.Run(() => scan.GetIDm((int)SystemCode.Any));
            UpdateStatus("学生証を読み取りました : " + studentid);
            int seq = dataGrid.Items.Count;
            DateTime dt = DateTime.Now;
            seq = ++seq;
            AddTable(seq, dt, idm, studentid);

            using GSpreadRW gspread = new();
            string StudentName = await Task.Run(() => gspread.ReadName(studentid));
            int StudentEntryNumber = await Task.Run(() => gspread.ReadEntryNum(studentid));

            if (StudentEntryNumber == -1)
            {
                UpdateTable(seq, dt, idm, studentid, StudentName, StudentEntryNumber, "該当するエントリーなし");
                UpdateStatus("学生番号：" + studentid + "に該当するエントリーが見つかりませんでした。");
            }
            else if (StudentEntryNumber == -2)
            {
                UpdateTable(seq, dt, idm, studentid, StudentName, StudentEntryNumber, "スプレッドシートにアクセスできません");
                UpdateStatus("ネットワークに接続できないかスプレッドシートにアクセスできません");
                UpdateStatus("スプレッドシートの設定を確認してください。");
            }

            UpdateTable(seq, dt, idm, studentid, StudentName, StudentEntryNumber, "スプレッドシートにアップロード中");

            int WriteResult = await Task.Run(() => gspread.SheetWrite(dt, studentid, idm));
            if (WriteResult == 0)
            {
                UpdateTable(seq, dt, idm, studentid, StudentName, StudentEntryNumber, "完了");
                return;
            }
            else if (WriteResult == -1)
            {
                UpdateTable(seq, dt, idm, studentid, StudentName, StudentEntryNumber, "スプレッドシートにアクセスできません");
                UpdateStatus("スプレッドシートの設定を確認してください。");
                return;
            }
        }

        private async void ManualInput(string studentid)
        {
            string idm = "手動入力";
            UpdateStatus("学生番号を手動で入力しました : " + studentid);
            int seq = dataGrid.Items.Count;
            DateTime dt = DateTime.Now;
            seq = ++seq;
            AddTable(seq, dt, idm, studentid);

            using GSpreadRW gspread = new();
            string StudentName = await Task.Run(() => gspread.ReadName(studentid));
            int StudentEntryNumber = await Task.Run(() => gspread.ReadEntryNum(studentid));

            if (StudentEntryNumber == -1)
            {
                UpdateTable(seq, dt, idm, studentid, StudentName, StudentEntryNumber, "該当するエントリーなし");
                UpdateStatus("学生番号：" + studentid + "に該当するエントリーが見つかりませんでした。");
            }
            else if (StudentEntryNumber == -2)
            {
                UpdateTable(seq, dt, idm, studentid, StudentName, StudentEntryNumber, "スプレッドシートにアクセスできません");
                UpdateStatus("ネットワークに接続できないかスプレッドシートにアクセスできません");
                UpdateStatus("スプレッドシートの設定を確認してください。");
            }

            UpdateTable(seq, dt, idm, studentid, StudentName, StudentEntryNumber, "スプレッドシートにアップロード中");

            int WriteResult = await Task.Run(() => gspread.SheetWrite(dt, studentid, idm));
            if (WriteResult == 0)
            {
                UpdateTable(seq, dt, idm, studentid, StudentName, StudentEntryNumber, "完了");
                return;
            }
            else if (WriteResult == -1)
            {
                UpdateTable(seq, dt, idm, studentid, StudentName, StudentEntryNumber, "スプレッドシートにアクセスできません");
                UpdateStatus("スプレッドシートの設定を確認してください。");
                return;
            }
        }

        private void AddTable(int seq, DateTime dt, string idm, string studentid)
        {
            var data = new StudentList { No = seq, ScanTime = dt.ToString("yyyy/MM/dd HH:mm:ss"), StudentID = studentid, StudentName = "未取得", StudentIDm = idm, StudentEntryNumber = "未取得", Status = "エントリー番号検索中" };

            if (dataGrid.ItemsSource is ObservableCollection<StudentList> dataList)
            {
                dataList.Add(data);
                dataGrid.ScrollIntoView(dataGrid.Items.GetItemAt(dataGrid.Items.Count - 1));
            }

            return;

        }

        private void UpdateTable(int seq, DateTime dt, string idm, string studentid, string studentname, int entrynumber, string statustext)
        {
            if (dataGrid.Items[seq - 1] is StudentList student && dataGrid.ItemsSource is ObservableCollection<StudentList> dataList)
            {
                dataList.Remove(student);
                string entrynumberstring = entrynumber.ToString();
                var data = new StudentList { No = seq, ScanTime = dt.ToString("yyyy/MM/dd HH:mm:ss"), StudentID = studentid, StudentName = studentname, StudentIDm = idm, StudentEntryNumber = entrynumberstring, Status = statustext };
                dataList.Add(data);
                dataGrid.ScrollIntoView(dataGrid.Items.GetItemAt(dataGrid.Items.Count - 1));
            }

            return;
        }
        private void UpdateStatus(string text)
        {
            SnackBar.MessageQueue?.Enqueue(text, null, null, null, false, true, TimeSpan.FromSeconds(3));
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ReadStart.IsEnabled = false;
            ReadCard();
            ReadStart.IsEnabled = true;
        }
        private void ManualInput_Click(object sender, RoutedEventArgs e)
        {
            if (InputBoxStudentNumber.Text == string.Empty) return;
            if (InputBoxStudentNumber.Text.Length != 7)
            {
                MessageBox.Show("学生番号の書式が正しくありません", "学生番号書式エラー", MessageBoxButton.OK, MessageBoxImage.Hand);
                UpdateStatus("学生番号の書式が正しくありません。");
                return;
            }
            ReadStart.IsEnabled = false;
            ManualInput(InputBoxStudentNumber.Text.ToUpper());
            ReadStart.IsEnabled = true;
            InputBoxStudentNumber.Text = String.Empty;
        }
        private void BoxStudentID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                AutomationPeer peer = UIElementAutomationPeer.CreatePeerForElement(ManualDialogOK);
                if (peer.GetPattern(PatternInterface.Invoke) is IInvokeProvider invokeProv)
                {
                    invokeProv.Invoke();
                }
            }
        }

        private void LaunchSetting(object sender, RoutedEventArgs e)
        {
            Setting settingW = new();
            settingW.ShowDialog();
        }

        private void VersionLunch(object sender, RoutedEventArgs e)
        {
            Version versionW = new();
            versionW.ShowDialog();
        }
        private void ClearList_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.ItemsSource is ObservableCollection<StudentList> dataList)
            {
                dataList.Clear();
            }
        }
    }
}
