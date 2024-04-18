using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media;
using PrintDialog = System.Windows.Forms.PrintDialog;

namespace VMMS
{
    internal class BasePrintClass
    {
        private PrintDocument printDocument;
        bool cellFont_errr = false;

        public string PrinterName;
        // 用于存储表格数据的公共属性
        public List<List<Cell>> TableData { get; set; } = new List<List<Cell>>();

        public BasePrintClass()
        {
            printDocument = new PrintDocument();
            
            printDocument.PrintPage += new PrintPageEventHandler(this.printDocument_PrintPage);
        }

        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            // 设定一个起始打印位置
            float startX = e.MarginBounds.Left;
            float startY = e.MarginBounds.Top;
            // 如果存在表格数据，绘制表格
            if (TableData.Any())
            {
                DrawTable(g, TableData, startX, startY);
            }
        }

        // 绘制表格的方法
        private void DrawTable(Graphics g, List<List<Cell>> tableData, float startX, float startY)
        {
            // 获取当前 Graphics 对象的 DPI 设置
            //float dpiX = g.DpiX;
            //float dpiY = g.DpiY;
            float dpiX = 96;
            float dpiY = 96;

            float x = startX;
            float y = startY;
            string FontsName = StoreSetting.Instance.settings["FontsName"];

            foreach (var row in tableData)
            {
                float maxHeightInPixels = 0; // 用于计算行的最大高度（像素）

                foreach (var cell in row)
                {
                    Font cellFont;
                    // 将单元格的尺寸从英寸转换为像素
                    float cellWidthInPixels = (cell.Width / 100) * dpiX;
                    float cellHeightInPixels = (cell.Height / 100) * dpiY;

                    //cellFont = new Font("等线", cell.FontSize);   //Arial
                    cellFont = TryCreateFont(FontsName, cell.FontSize);
                    if (cellFont.Name != FontsName) {
                        cellFont = new Font("等线", cell.FontSize);
                        if (!cellFont_errr)
                        {
                            MessageBox.Show($"创建字体异常，使用默认字体: {cellFont.Name}");
                            cellFont_errr = true;
                        }
                        
                    }
                    StringFormat stringFormat = new StringFormat();

                    // 根据对齐方式设置 StringFormat
                    switch (cell.ContentAlignment)
                    {
                        case CellContentAlignment.TopLeft:
                            stringFormat.Alignment = StringAlignment.Near;
                            stringFormat.LineAlignment = StringAlignment.Near;
                            break;

                        case CellContentAlignment.TopCenter:
                            stringFormat.Alignment = StringAlignment.Center;
                            stringFormat.LineAlignment = StringAlignment.Near;
                            break;

                        case CellContentAlignment.TopRight:
                            stringFormat.Alignment = StringAlignment.Far;
                            stringFormat.LineAlignment = StringAlignment.Near;
                            break;

                        case CellContentAlignment.MiddleLeft:
                            stringFormat.Alignment = StringAlignment.Near;
                            stringFormat.LineAlignment = StringAlignment.Center;
                            break;

                        case CellContentAlignment.MiddleCenter:
                            stringFormat.Alignment = StringAlignment.Center;
                            stringFormat.LineAlignment = StringAlignment.Center;
                            break;

                        case CellContentAlignment.MiddleRight:
                            stringFormat.Alignment = StringAlignment.Far;
                            stringFormat.LineAlignment = StringAlignment.Center;
                            break;

                        case CellContentAlignment.BottomLeft:
                            stringFormat.Alignment = StringAlignment.Near;
                            stringFormat.LineAlignment = StringAlignment.Far;
                            break;

                        case CellContentAlignment.BottomCenter:
                            stringFormat.Alignment = StringAlignment.Center;
                            stringFormat.LineAlignment = StringAlignment.Far;
                            break;

                        case CellContentAlignment.BottomRight:
                            stringFormat.Alignment = StringAlignment.Far;
                            stringFormat.LineAlignment = StringAlignment.Far;
                            break;
                    }
                    // 绘制单元格边框（如果 Cell.Border 为 true）
                    if (cell.Border)
                    {
                        g.DrawRectangle(Pens.Black, x, y, cellWidthInPixels, cellHeightInPixels);
                    }
                    //g.DrawRectangle(Pens.Black, x, y, cell.Width, cell.Height); // 绘制单元格边框
                    // 使用指定的文字大小和对齐方式绘制单元格内容
                    g.DrawString(cell.Content, cellFont, System.Drawing.Brushes.Black, new RectangleF(x, y, cellWidthInPixels, cellHeightInPixels), stringFormat);
                    x += cellWidthInPixels;
                    maxHeightInPixels = Math.Max(maxHeightInPixels, cellHeightInPixels); // 更新当前行的最大高度（像素）
                }
                y += maxHeightInPixels; // 移动到下一行的起始位置
                x = startX; // 重置 x 到初始值
            }
        }
        //允许用户选择打印机、指定打印份数和其他打印选项
        public void PrintSet()
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument;
            printDialog.ShowDialog();
        }
        //允许用户配置页面相关的设置，比如纸张大小、方向（横向或纵向）、页边距等
        protected void PageSet()
        {
            PageSetupDialog pageSetupDialog = new PageSetupDialog();
            pageSetupDialog.Document = printDocument;
            pageSetupDialog.ShowDialog();
        }
        //提供打印内容的预览。这允许用户在实际打印之前查看最终的打印输出将是什么样子。
        public void PrintView()
        {
            PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
            printPreviewDialog.StartPosition = FormStartPosition.CenterScreen;
            printPreviewDialog.Document = printDocument;
            try
            {
                printPreviewDialog.ShowDialog();
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message, "打印出错", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //尝试直接启动打印任务，使用PrintDocument对象当前的设置。如果在打印过程中遇到错误，比如打印机无响应或配置错误，将通过消息框显示异常信息。
        public void PrintStart()
        {
            // 尝试设置打印机名称
            printDocument.PrinterSettings.PrinterName = PrinterName;

            // 检查指定的打印机名称是否有效
            if (!printDocument.PrinterSettings.IsValid)
            {
                // 如果指定的打印机不存在，使用默认打印机
                MessageBox.Show($"指定的打印机“{PrinterName}”不存在，将使用默认打印机。");
                // 注意：不需要显式设置为默认打印机，PrintDocument 默认使用系统默认打印机
                printDocument.PrinterSettings.PrinterName = null;
            }
            try
            {
                printDocument.Print();
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message, "打印出错", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //再次显示PrintDialog，与PrintSet方法类似。不同之处在于，如果用户确认打印（点击“打印”按钮），它将立即开始打印任务，而不是仅设置打印机选项。
        public void PrintDialogStart()
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument;
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    printDocument.Print();
                }
                catch (Exception excep)
                {
                    MessageBox.Show(excep.Message, "打印出错", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private Font TryCreateFont(string fontName, float fontSize)
        {
            try
            {
                return new Font(fontName, fontSize);
            }
            catch (ArgumentException ex)
            {
                if (!cellFont_errr)
                {
                    MessageBox.Show($"无法创建字体 '{fontName}'. 错误: {ex.Message}");
                    cellFont_errr = true;
                }
                // 返回一个默认字体
                return new Font("等线", fontSize);
            }
        }

        public void SetMargins(int left, int top, int right, int bottom)
        {
            // 设置页边距
            printDocument.DefaultPageSettings.Margins = new Margins(left, right, top, bottom);
        }

        public enum CellContentAlignment
        {
            TopLeft,      // 文本顶部对齐并靠左
            TopCenter,    // 文本顶部对齐并居中
            TopRight,     // 文本顶部对齐并靠右
            MiddleLeft,   // 文本垂直居中对齐并靠左
            MiddleCenter, // 文本垂直居中对齐并水平居中
            MiddleRight,  // 文本垂直居中对齐并靠右
            BottomLeft,   // 文本底部对齐并靠左
            BottomCenter, // 文本底部对齐并居中
            BottomRight   // 文本底部对齐并靠右
        }

        public class Cell
        {
            public string Content { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }
            public float FontSize { get; set; }
            public CellContentAlignment ContentAlignment { get; set; } // 对齐方式属性
            public bool Border { get; set; } // 新增属性，表示是否绘制边框

            public Cell(string content, float width, float height, float fontSize, CellContentAlignment contentAlignment, bool border = true)
            {
                Content = content;
                Width = width;
                Height = height;
                FontSize = fontSize;
                ContentAlignment = contentAlignment;
                Border = border; // 默认情况下绘制边框
            }
        }
    }
}