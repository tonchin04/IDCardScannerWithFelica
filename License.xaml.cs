using System.Windows;

namespace IDCardScannerWithFelica
{
    /// <summary>
    /// License.xaml の相互作用ロジック
    /// </summary>
    public partial class License : Window
    {
        public License()
        {
            InitializeComponent();
        }

        private void License_OK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
