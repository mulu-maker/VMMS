using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Windows;

namespace VMMS
{
    /// <summary>
    /// WindowItem.xaml 的交互逻辑
    /// </summary>
    public partial class WindowStore : Window
    {
        public WindowStore()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var settings = StoreSetting.Instance;

            List<string> installedFonts = GetInstalledFonts();
            FontsName.ItemsSource = installedFonts;
            // 从Settings.settings读取FontsName值并设置默认选定项
            string defaultFontName = settings.settings["FontsName"];
            if (!string.IsNullOrEmpty(defaultFontName) && FontsName.Items.Contains(defaultFontName))
            {
                FontsName.SelectedItem = defaultFontName;
            }
            else if (FontsName.Items.Count > 0)
            {
                FontsName.SelectedIndex = 0; // 如果默认值不存在，选择第一项作为默认值
            }
            // 加载打印机名称列表
            var printerNames = LoadPrinterNames();

            // 假设您有一个名为printersComboBox的ComboBox
            foreach (var printerName in printerNames)
            {
                PrinterName.Items.Add(printerName);
            }
            string defaultPrinterName = settings.settings["PrinterName"];
            if (!string.IsNullOrEmpty(defaultPrinterName) && PrinterName.Items.Contains(defaultPrinterName))
            {
                PrinterName.SelectedItem = defaultPrinterName;
                // 可选：设置默认选定的打印机
            }
            else if (PrinterName.Items.Count > 0)
            {
                PrinterName.SelectedIndex = 0; // 默认选择第一个打印机
            }
            txtStoreName.Text = settings.settings["StoreName"];
            txtStoreTelephone.Text = settings.settings["StoreTelephone"];
            txtStoreAddress.Text = settings.settings["StoreAddress"];
            txtStoreThank.Text = settings.settings["StoreThank"];
            txtStoreThank2.Text = settings.settings["StoreThank2"];
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            // OK按钮
            var settings = StoreSetting.Instance;

            // 当用户更改选定项时，更新并保存设置
            if (FontsName.SelectedItem != null)
            {
                settings.settings["FontsName"] = FontsName.SelectedItem.ToString();
            }
            // 当用户更改选定项时，更新并保存设置
            if (FontsName.SelectedItem != null)
            {
                settings.settings["PrinterName"] = PrinterName.SelectedItem.ToString();
            }
            settings.settings["StoreName"] = txtStoreName.Text;
            settings.settings["StoreTelephone"] = txtStoreTelephone.Text;
            settings.settings["StoreAddress"] = txtStoreAddress.Text;
            settings.settings["StoreThank"] = txtStoreThank.Text;
            settings.settings["StoreThank2"] = txtStoreThank2.Text;

            // 这里调用保存到配置文件的逻辑
            settings.SaveSettings(); // 假设StoreSetting有一个方法来保存设置
            this.Close();

        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public List<string> GetInstalledFonts()
        {
            InstalledFontCollection fontsCollection = new InstalledFontCollection();
            List<string> fontsList = new List<string>();

            foreach (FontFamily font in fontsCollection.Families)
            {
                fontsList.Add(font.Name);
            }

            return fontsList;
        }
        private List<string> LoadPrinterNames()
        {
            List<string> printerNames = new List<string>();

            foreach (string printerName in PrinterSettings.InstalledPrinters)
            {
                printerNames.Add(printerName);
            }

            return printerNames;
        }


    }
}
