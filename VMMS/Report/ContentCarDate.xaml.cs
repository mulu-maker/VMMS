using System;
using System.Windows;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>
    /// ContentCarDate.xaml 的交互逻辑
    /// </summary>
    public partial class ContentCarDate : Window
    {
        public ObjCar s;

        public ContentCarDate()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DalModel.BindingDataGridComboBoxColumn(dataGrid1, 3);
            Clear(); 
            LoadDataGrid();
        }

        private void Clear()
        {
            s = new ObjCar();
            DateTime d = DateTime.Now;
            s.DateStart = d.AddMonths(-6).Date;
            s.DateEnd = d.Date;
            dataGrid1.ItemsSource = null;
            this.DataContext = s;
        }

        private void LoadDataGrid()
        {
            dataGrid1.ItemsSource = DalCar.GetDateList(s);//查询符合条件的数据并刷新datagrid
            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(BaseWindowClass.DataGrid_LoadingRow);//显示行号
        }

        private void DocumentContent_Closed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void dataGrid1_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                ObjCar car = dataGrid1.SelectedItem as ObjCar;
                ContentCarLog child = new ContentCarLog();
                child.IsDetail = true;
                child.s.CarGUID = car.CarGUID;
                child.ShowDialog();
            }
        }
    }
}
