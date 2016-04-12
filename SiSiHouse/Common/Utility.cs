namespace SiSiHouse.Common
{
    using AdvanceSoftware.ExcelCreator;
    using AdvanceSoftware.ExcelCreator.Xlsx;
    using SiSiHouse.Models.Entities;
    using SiSiHouse.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;

    public class Utility
    {
        public static void ExportDataToExcel(Controller controller, ExportExcelModel viewModel, object data, string templateFileName = "ThongKeTemp.xlsx", string OutputFileName = "ThongKeDoanhThu.xlsx")
        {
            string strFilePath = HttpContext.Current.Server.MapPath(Path.Combine(ConfigurationManager.AppSettings[ConfigurationKeys.EXPORT_FILE], templateFileName));

            OutputFileName = "ThongKeDoanhThu__" + viewModel.TARGET_YEAR.ToString() + "_" + viewModel.TARGET_MONTH.ToString("D2") + ".xlsx";

            XlsxCreator xlsxCreator = new XlsxCreator();
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();

            try
            {
                xlsxCreator.OpenBook(memoryStream, strFilePath);
                xlsxCreator.Cell("**ThongKe").Value = string.Format(Constant.ExportRevenue.TITLE, viewModel.TARGET_MONTH.ToString(), viewModel.TARGET_YEAR.ToString());

                ExportRevenue(ref xlsxCreator, data);
            }
            catch
            {
                controller.Response.Clear();
                controller.Response.Write("Đã có lỗi xảy ra trong quá trình xuất dữ liệu ra file excel. Vui lòng thử lại sau.");
                controller.Response.End();
                return;
            }

            Regex rgx = new Regex("[\\/:*?\"<>|]");
            OutputFileName = rgx.Replace(OutputFileName, "-");

            if (controller.Request.Browser.Browser.Equals("InternetExplorer"))
            {
                OutputFileName = HttpUtility.UrlPathEncode(OutputFileName);
            }

            // file close
            xlsxCreator.CloseBook(true);

            // ouput
            controller.Response.Clear();
            controller.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            controller.Response.AddHeader("content-disposition", "attachment; filename=\"" + OutputFileName + "\"");
            controller.Response.BinaryWrite(memoryStream.ToArray());

            memoryStream.Close();
            xlsxCreator.Dispose();
            controller.Response.End();
        }

        private static void ExportRevenue(ref XlsxCreator xlsxCreator, object data)
        {
            var dataList = (List<Product>)data;
            xlsxCreator.InitFormulaAnswer = true;
            int index = 0;
            int row = Constant.ExportRevenue.START_ROW;
            int sumQuantity = 0;
            decimal sumSales = 0;
            decimal sumCost = 0;
            decimal sumProfit = 0;

            foreach (var product in dataList)
            {
                index++;
                row++;

                var cost = product.QUANTITY * InitialDecimal(product.REAL_PRICE);
                var profit = product.SALES - cost;
                var profitRate = product.SALES == 0 ? "0 %" : ((profit / product.SALES) * 100).ToString("#,##0.00") + " %";

                sumQuantity += product.QUANTITY;
                sumSales += product.SALES;
                sumCost += cost;
                sumProfit += profit;

                xlsxCreator.Cell("A" + row.ToString()).Value = index;
                xlsxCreator.Cell("B" + row.ToString()).Value = "Mã: " + product.PRODUCT_CODE + "\nTên: " + product.PRODUCT_NAME;
                xlsxCreator.Cell("C" + row.ToString()).Value = "Hãng: " + product.BRAND_NAME + "\nLoại: " + product.CATEGORY_NAME;
                xlsxCreator.Cell("D" + row.ToString()).Value = "Màu: " + product.COLOR_NAME + "\nSize: " + product.SIZE;
                xlsxCreator.Cell("E" + row.ToString()).Value = product.QUANTITY;
                xlsxCreator.Cell("F" + row.ToString()).Value = product.SALES;
                xlsxCreator.Cell("G" + row.ToString()).Value = cost;
                xlsxCreator.Cell("H" + row.ToString()).Value = profit;
                xlsxCreator.Cell("I" + row.ToString()).Value = profitRate;
                xlsxCreator.Cell("J" + row.ToString()).Value = product.MODIFIED_DATE.Value.ToString("yyyy/MM/dd") + "\n" + product.MODIFIED_DATE.Value.ToString("HH:mm:ss");
                xlsxCreator.Cell("J" + row.ToString()).Attr.HorizontalAlignment = HorizontalAlignment.Center;
            }

            xlsxCreator.Pos(0, Constant.ExportRevenue.START_ROW, Constant.ExportRevenue.END_COLUMN, row - 1).Attr.LineTop(BorderStyle.Thin, xlColor.Gray50);
            xlsxCreator.Pos(0, Constant.ExportRevenue.START_ROW, Constant.ExportRevenue.END_COLUMN, row - 1).Attr.LineLeft(BorderStyle.Thin, xlColor.Gray50);
            xlsxCreator.Pos(0, Constant.ExportRevenue.START_ROW, Constant.ExportRevenue.END_COLUMN, row - 1).Attr.LineRight(BorderStyle.Thin, xlColor.Gray50);
            xlsxCreator.Pos(0, Constant.ExportRevenue.START_ROW, Constant.ExportRevenue.END_COLUMN, row - 1).Attr.LineBottom(BorderStyle.Thin, xlColor.Gray50);

            row++;
            xlsxCreator.Cell("E" + row.ToString()).Value = sumQuantity;
            xlsxCreator.Cell("F" + row.ToString()).Value = sumSales;
            xlsxCreator.Cell("G" + row.ToString()).Value = sumCost;
            xlsxCreator.Cell("H" + row.ToString()).Value = sumProfit;
            xlsxCreator.Cell("I" + row.ToString()).Value = sumSales == 0 ? "0 %" : ((sumProfit / sumSales) * 100).ToString("#,##0.00") + " %";
            xlsxCreator.Pos(Constant.ExportRevenue.START_TOTAL_COLUMN, row - 1, Constant.ExportRevenue.END_TOTAL_COLUMN, row - 1).Attr.BackColor2 = xlColor.Yellow;
            xlsxCreator.Pos(Constant.ExportRevenue.START_TOTAL_COLUMN, row - 1, Constant.ExportRevenue.END_TOTAL_COLUMN, row - 1).Attr.LineTop(BorderStyle.Thin, xlColor.Gray50);
            xlsxCreator.Pos(Constant.ExportRevenue.START_TOTAL_COLUMN, row - 1, Constant.ExportRevenue.END_TOTAL_COLUMN, row - 1).Attr.LineLeft(BorderStyle.Thin, xlColor.Gray50);
            xlsxCreator.Pos(Constant.ExportRevenue.START_TOTAL_COLUMN, row - 1, Constant.ExportRevenue.END_TOTAL_COLUMN, row - 1).Attr.LineRight(BorderStyle.Thin, xlColor.Gray50);
            xlsxCreator.Pos(Constant.ExportRevenue.START_TOTAL_COLUMN, row - 1, Constant.ExportRevenue.END_TOTAL_COLUMN, row - 1).Attr.LineBottom(BorderStyle.Thin, xlColor.Gray50);

            xlsxCreator.Pos(0, Constant.ExportRevenue.START_ROW, Constant.ExportRevenue.END_COLUMN, row - 1).RowHeight = 32;

            xlsxCreator.Pos(0, Constant.ExportRevenue.START_ROW, Constant.ExportRevenue.END_COLUMN, row - 1).Attr.FontName = "Calibri";
            xlsxCreator.Pos(0, Constant.ExportRevenue.START_ROW, Constant.ExportRevenue.END_COLUMN, row - 1).Attr.FontPoint = 12;
        }

        public static decimal InitialDecimal(decimal? data)
        {
            return data.HasValue ? data.Value : 0;
        }

        public static int InitialInteger(int? data)
        {
            return data.HasValue ? data.Value : 0;
        }

        public static string GetStatusName(int? statusID)
        {
            string statusName = "";

            if (statusID.HasValue)
            {
                switch (statusID.Value.ToString())
                {
                    case Constant.Status.WAITING:
                        statusName = Constant.Status.Items[0].ToString();
                        break;
                    case Constant.Status.SELLING:
                        statusName = Constant.Status.Items[1].ToString();
                        break;
                    case Constant.Status.SALE_OFF:
                        statusName = Constant.Status.Items[2].ToString();
                        break;
                    case Constant.Status.OUT_OF_STOCK:
                        statusName = Constant.Status.Items[3].ToString();
                        break;
                    default:
                        break;
                }
            }

            return statusName;
        }

        public static string GetCategoryTypeName(int type)
        {
            string typeName = "";

            switch (type)
            {
                case Constant.CategoryType.CLOTHES:
                    typeName = Constant.CategoryType.Items[0].ToString();
                    break;
                case Constant.CategoryType.FOOTWEARS:
                    typeName = Constant.CategoryType.Items[1].ToString();
                    break;
                case Constant.CategoryType.ACCESSORIES:
                    typeName = Constant.CategoryType.Items[2].ToString();
                    break;
                default:
                    break;
            }

            return typeName;
        }

        public static string GetPicturePath(long productID, string fileName)
        {
            string filePath = string.IsNullOrEmpty(fileName) ? "" : Path.Combine(ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_PICTURE], productID.ToString(), fileName);

            return filePath;
        }
    }
}