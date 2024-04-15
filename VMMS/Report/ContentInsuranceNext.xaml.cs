using System;
using System.Windows;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>
    /// ContentInsuranceNext.xaml 的交互逻辑
    /// </summary>
    public partial class ContentInsuranceNext : Window
    {
        public ObjCar s;

        public ContentInsuranceNext()
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
            s.DateStart = d.Date;
            s.DateEnd = d.AddDays(30).Date;
            dataGrid1.ItemsSource = null;
            this.DataContext = s;
        }

        private void LoadDataGrid()
        {
            dataGrid1.ItemsSource = DalCar.GetInsuranceList(s);//查询符合条件的数据并刷新datagrid
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

        private void dpStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if(dpStart.SelectedDate>BaseDateTimeClass.BaseDate)
            {
                dpEnd.SelectedDate = dpStart.SelectedDate.Value.AddDays(30);
            }
        }
    }
}
