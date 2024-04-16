using NPOI.POIFS.NIO;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using static VMMS.BasePrintClass;

/// <summary>
/// </summary>
/// Summary description for Class1

namespace VMMS
{
    internal class ObjBillPrint
    {
        public ObjBill obj;
        // 读取配置项
        string StoreName = StoreSetting.Instance.settings["StoreName"]; 
        string StoreThank = StoreSetting.Instance.settings["StoreThank"]+ "\r\n" + StoreSetting.Instance.settings["StoreThank2"];
        string StoreTelephone = StoreSetting.Instance.settings["StoreTelephone"];
        string StoreAddress = StoreSetting.Instance.settings["StoreAddress"];
        string PrinterName = StoreSetting.Instance.settings["PrinterName"];
        string PrinterNum = StoreSetting.Instance.settings["PrinterNum"];
        //维修单
        public void PrintStartRepair()
        {

            float ContentHeight = 22;  //正文表格高度
            float ContentFontSize = 9; //正文内容字号
            //xxxxx
            // 创建 BasePrintClass 的实例
            BasePrintClass printer = new BasePrintClass();
            printer.PrinterName = PrinterName;
            // 设置页边距：左、上、右、下边距（单位：百分之一英寸）
            printer.SetMargins(45, 45, 25, 45); // 例如，设置左右边距为1英寸（100），上下边距为0.5英寸（50）

            // 定义表格数据
            List<List<BasePrintClass.Cell>> tableData = new List<List<BasePrintClass.Cell>>
            {
                // 添加表头
                new List<BasePrintClass.Cell>
                {
                    new BasePrintClass.Cell(StoreName, 750, 45,20, CellContentAlignment.BottomCenter,false)
                },
                // 添加列
                new List<BasePrintClass.Cell>
                {
                    new BasePrintClass.Cell("维修单", 750, 35,15, CellContentAlignment.BottomCenter,false)
                },
                new List<BasePrintClass.Cell>
                {
                    new BasePrintClass.Cell("维修单编号：", 93, ContentHeight,ContentFontSize, CellContentAlignment.BottomRight,false),
                    new BasePrintClass.Cell(obj.BillCode, 177, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft,false),
                    new BasePrintClass.Cell("进厂时间：", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomRight,false),
                    new BasePrintClass.Cell(obj.BillDate.ToString("yyyy年MM月dd日"), 200, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft,false),
                    new BasePrintClass.Cell("完工时间：", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomRight, false),
                    new BasePrintClass.Cell(obj.CompleteTime.ToString("yyyy年MM月dd日"), 120, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft, false)
                },
                new List<BasePrintClass.Cell>
                {
                    new BasePrintClass.Cell("客户名称：", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomRight),
                    new BasePrintClass.Cell(obj.SendName, 320, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft),
                    new BasePrintClass.Cell("客户电话：", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomRight),
                    new BasePrintClass.Cell(obj.MobilePhone, 270, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft)
                },
                    new List<BasePrintClass.Cell>
                {
                    new BasePrintClass.Cell("车牌号：", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomRight),
                    new BasePrintClass.Cell(obj.LicensePlate, 320, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft),
                    new BasePrintClass.Cell("行驶里程：", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomRight),
                    new BasePrintClass.Cell(string.Format(" {0}KM",obj.CarMileage), 270, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft)
                },
                    new List<BasePrintClass.Cell>
                {
                    new BasePrintClass.Cell("车型：", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomRight),
                    new BasePrintClass.Cell(obj.ModelName, 320, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft),
                    new BasePrintClass.Cell("车架号：", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomRight),
                    new BasePrintClass.Cell(obj.VIN, 270, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft)
                },
                    new List<BasePrintClass.Cell>
                {
                    new BasePrintClass.Cell(" ", 750, ContentFontSize,1, CellContentAlignment.BottomCenter,false)
                },
            };
            //工时项目
            var newRow = new List<BasePrintClass.Cell>
                {
                    new BasePrintClass.Cell("序号", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                    new BasePrintClass.Cell("工时项目", 320, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                    new BasePrintClass.Cell("单价", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                    new BasePrintClass.Cell("数量", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                    new BasePrintClass.Cell("状态", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                    new BasePrintClass.Cell("金额", 110, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter)
                };
            tableData.Add(newRow);
            // 遍历数据源，为每行数据创建一个新的List<BasePrintClass.Cell>并添加到tableData中
            var v = 0;
            decimal GS_SalesAmount = 0;
            foreach (var row in obj.ListDetail)
            {
                if (row.PropertyID == 5)
                {
                    v++;
                    newRow = new List<BasePrintClass.Cell>
                    {
                        new BasePrintClass.Cell(string.Format("{0}",v), 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                        new BasePrintClass.Cell(string.Format("{0}",row.ProductName), 320, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                        new BasePrintClass.Cell(string.Format("{0}",row.SalesPrice), 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                        new BasePrintClass.Cell(string.Format("{0}",row.SalesNumber), 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                        new BasePrintClass.Cell("正常", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                        new BasePrintClass.Cell(string.Format("{0}",row.SalesAmount), 110, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter)
                    };
                    GS_SalesAmount = GS_SalesAmount + row.SalesAmount;
                    tableData.Add(newRow);
                }
            };
            newRow = new List<BasePrintClass.Cell>
                {
                    new BasePrintClass.Cell(" ", 750, ContentFontSize,1, CellContentAlignment.BottomCenter,false)
                };
            tableData.Add(newRow);
            //配件项目
            newRow = new List<BasePrintClass.Cell>
                {
                    new BasePrintClass.Cell("序号", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                    new BasePrintClass.Cell("配件项目", 320, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                    new BasePrintClass.Cell("单价", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                    new BasePrintClass.Cell("数量", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                    new BasePrintClass.Cell("状态", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                    new BasePrintClass.Cell("金额", 110, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter)
                };
            tableData.Add(newRow);
            // 遍历数据源，为每行数据创建一个新的List<BasePrintClass.Cell>并添加到tableData中
            v = 0;
            decimal PJ_SalesAmount = 0;
            foreach (var row in obj.ListDetail)
            {
                if (row.PropertyID == 1)
                {
                    v++;
                    newRow = new List<BasePrintClass.Cell>
                    {
                    new BasePrintClass.Cell(string.Format("{0}",v), 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                    new BasePrintClass.Cell(string.Format("{0}",row.ProductName), 320, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                    new BasePrintClass.Cell(string.Format("{0}",row.SalesPrice), 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                    new BasePrintClass.Cell(string.Format("{0}",row.SalesNumber), 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                    new BasePrintClass.Cell("正常", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                    new BasePrintClass.Cell(string.Format("{0}",row.SalesAmount), 110, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter)
                    };
                    PJ_SalesAmount = PJ_SalesAmount + row.SalesAmount;
                    tableData.Add(newRow);
                }
            }
            newRow = new List<BasePrintClass.Cell>
                {
                    new BasePrintClass.Cell(" ", 750, ContentFontSize,1, CellContentAlignment.BottomCenter,false)
                };
            tableData.Add(newRow);
            newRow = new List<BasePrintClass.Cell>
                {
                    new BasePrintClass.Cell("原价合计：", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomRight),
                    new BasePrintClass.Cell(obj.TotalSalesAmount.ToString("C"), 320, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft),
                    new BasePrintClass.Cell("工时：", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomRight),
                    new BasePrintClass.Cell(string.Format("{0}",GS_SalesAmount.ToString("C")), 95, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft),
                    new BasePrintClass.Cell("配件：", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomRight),
                    new BasePrintClass.Cell(string.Format("{0}",PJ_SalesAmount.ToString("C")), 95, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft),
                };
            tableData.Add(newRow);
            newRow = new List<BasePrintClass.Cell>
                {
                    new BasePrintClass.Cell("合计：", 220, ContentHeight,ContentFontSize, CellContentAlignment.BottomRight),
                    new BasePrintClass.Cell(obj.TotalSalesAmount.ToString("C"), 100, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft),
                    new BasePrintClass.Cell("大写：", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomRight),
                    new BasePrintClass.Cell( BaseStringClass.ToRMB(obj.TotalSalesAmount), 350, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft),
                };
            tableData.Add(newRow);
            newRow = new List<BasePrintClass.Cell>
                {
                    new BasePrintClass.Cell("结算信息", 750, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft,false)
                };
            tableData.Add(newRow);
            string TotalDiffAmount = null;
            if (obj.TotalDiffAmount > 0 && obj.IsCharge) //有打折优惠价格，并且已结账。
            {
                TotalDiffAmount = obj.TotalDiffAmount.ToString("C");
            }
            newRow = new List<BasePrintClass.Cell>
                {
                    new BasePrintClass.Cell("现金：", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomRight,false),
                    new BasePrintClass.Cell(" ", 107, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft,false),
                    new BasePrintClass.Cell("微信：", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomRight,false),
                    new BasePrintClass.Cell(" ", 107, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft,false),
                    new BasePrintClass.Cell("支付宝：", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomRight,false),
                    new BasePrintClass.Cell(" ", 107, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft,false),
                    new BasePrintClass.Cell("优惠：", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomRight,false),
                    new BasePrintClass.Cell(TotalDiffAmount, 109, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft,false),
                };
            tableData.Add(newRow);
            string TotalChargeAmount = null;
            if (obj.IsCharge)
            {
                TotalChargeAmount = obj.TotalChargeAmount.ToString("C");
            }
            newRow = new List<BasePrintClass.Cell>
                {
                    new BasePrintClass.Cell(" ", 561, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft),
                    new BasePrintClass.Cell("实际支付：", 80, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft),
                    new BasePrintClass.Cell(TotalChargeAmount, 109, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft)
                };
            tableData.Add(newRow);
            newRow = new List<BasePrintClass.Cell>
                {
                    new Cell(" 出车报告: \r\n   ", 750, ContentHeight * 2,8, CellContentAlignment.MiddleLeft)
                };
            tableData.Add(newRow);
            newRow = new List<BasePrintClass.Cell>
                {
                    new BasePrintClass.Cell("服务满意度调查：", 150, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                    new BasePrintClass.Cell("口 非常满意", 150, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                    new BasePrintClass.Cell("口 基本满意", 150, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                    new BasePrintClass.Cell("口 一般", 150, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                    new BasePrintClass.Cell("口 不满意", 150, ContentHeight,ContentFontSize, CellContentAlignment.BottomCenter),
                };
            tableData.Add(newRow);
            newRow = new List<BasePrintClass.Cell>
                {
                    new BasePrintClass.Cell(StoreThank, 750, ContentHeight*2,ContentFontSize, CellContentAlignment.BottomLeft)
                };
            tableData.Add(newRow);
            newRow = new List<BasePrintClass.Cell>
                {
                    new BasePrintClass.Cell(string.Format("门店电话：{0}",StoreTelephone), 750, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft,false)
                };
            tableData.Add(newRow);
            newRow = new List<BasePrintClass.Cell>
                {
                    new BasePrintClass.Cell(string.Format("门店地址：{0}",StoreAddress), 750, ContentHeight,ContentFontSize, CellContentAlignment.BottomLeft,false)
                };
            tableData.Add(newRow);



            // 设置打印类的表格数据
            printer.TableData = tableData;
            if (PrinterNum == "0")
            {
                // 打印预览（在实际应用中，可以根据用户的选择来调用 PrintStart 或 PrintView）
                printer.PrintView();
            }
            else if (PrinterNum == "1")
            {
                //用户选择打印机、指定打印份数和其他打印选项
                printer.PrintSet();
            }
            else
            {
                // 直接开始打印（根据实际需求调用）
                printer.PrintStart();
            }

        }
    }
}