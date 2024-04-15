using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace VMMS
{
    /// <summary>
    /// WindowAbout.xaml 的交互逻辑
    /// </summary>
    public partial class WindowAbout : Window
    {
        public WindowAbout()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFile("license.txt", rtbLicense);
            lbVersion.Content = "版本号：v" + DalDataConfig.SoftVerion;
        }

        private static void LoadFile(string filename, RichTextBox richTextBox)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                MessageBox.Show(new ArgumentNullException().ToString());
            }
            if (!File.Exists(filename))
            {
                MessageBox.Show(new FileNotFoundException().ToString());
            }
            using (FileStream stream = File.OpenRead(filename))
            {
                TextRange documentTextRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                string dataFormat = DataFormats.Text;
                string ext = System.IO.Path.GetExtension(filename);
                if (String.Compare(ext, ".xaml", true) == 0)
                {
                    dataFormat = DataFormats.Xaml;
                }
                else if (String.Compare(ext, ".rtf", true) == 0)
                {
                    dataFormat = DataFormats.Rtf;
                }
                documentTextRange.Load(stream, dataFormat);
            }
        }
        private void link1_Click(object sender, RoutedEventArgs e)
        {
            BaseFileClass.OpenFile("Apache2.txt");
        }

        private void link2_Click(object sender, RoutedEventArgs e)
        {
            BaseFileClass.OpenFile("NumericInput.txt");
        }
        private void link3_Click(object sender, RoutedEventArgs e)
        {
            BaseFileClass.OpenFile("license.txt");
        }

    }
}
