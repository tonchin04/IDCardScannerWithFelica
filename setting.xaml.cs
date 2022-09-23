using Microsoft.Win32;
using System.Windows;

namespace IDCardScannerWithFelica
{
    /// <summary>
    /// setting.xaml の相互作用ロジック
    /// </summary>
    public partial class Setting : Window
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
        public Setting()
        {
            InitializeComponent();

            var mainWindow = (MainWindow)App.Current.MainWindow;
            nodetextBox.Text = NodeName;
            jsonTextBox.Text = JsonFilePath;
            spreadSheetIDBox.Text = SpreadSheetID;
            entryListbox.Text = StudentListName;
            scanListBox.Text = ScanListName;
            StudentNameBox.Text = StudentNameRow;
            StudentIDBox.Text = StudentIDRow;
            ServiceAccountIDBox.Text = ServiceAccountEmail;
            EntryNumberRowBox.Text = EntryNumberRow;


        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveSettingsbutton_Click(object sender, RoutedEventArgs e)
        {
            NodeName = nodetextBox.Text;
            JsonFilePath = jsonTextBox.Text;
            SpreadSheetID = spreadSheetIDBox.Text;
            StudentListName = entryListbox.Text;
            ScanListName = scanListBox.Text;
            StudentNameRow = StudentNameBox.Text;
            StudentIDRow = StudentIDBox.Text;
            ServiceAccountEmail = ServiceAccountIDBox.Text;
            EntryNumberRow = EntryNumberRowBox.Text;

            Properties.Settings.Default.Save();
            this.Close();
        }

        private void PickJsonButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            // ファイルの種類を設定
            dialog.Filter = "jsonファイル (*.json)|*.json|全てのファイル (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                jsonTextBox.Text = AddQuotesIfRequired(dialog.FileName);
            }
        }

        public string AddQuotesIfRequired(string path)
        {
            return !string.IsNullOrWhiteSpace(path) ?
                path.Contains(" ") && (!path.StartsWith("\"") && !path.EndsWith("\"")) ?
                    "\"" + path + "\"" : path :
                    string.Empty;
        }
    }
}
