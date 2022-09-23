using System.Windows;

namespace IDCardScannerWithFelica
{
    /// <summary>
    /// Version.xaml の相互作用ロジック
    /// </summary>
    public partial class Version : Window
    {
        public Version()
        {
            InitializeComponent();
        }
        private void License_open_Click(object sender, RoutedEventArgs e)
        {
            License licenceW = new License();
            licenceW.ShowDialog();
        }
        private void Version_OK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
