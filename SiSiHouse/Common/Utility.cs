namespace SiSiHouse.Common
{
    using AdvanceSoftware.ExcelCreator;
    using AdvanceSoftware.ExcelCreator.Xlsx;
    using SiSiHouse.Models.Entities;
    using SiSiHouse.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;

    public class Utility
    {
        public static void ExportDataToExcel(Controller controller, ExportExcelModel viewModel, object data, string templateFileName = "ThongKeTemp.xlsx", string OutputFileName = "ThongKeDoanhThu.xlsx")
        {
            string strPath = HttpContext.Current.Server.MapPath(controller.Request.ApplicationPath);
            string strInFileName = strPath + @"\App_Data\" + templateFileName;

            OutputFileName = "ThongKeDoanhThu__" + viewModel.TARGET_YEAR.ToString() + "_" + viewModel.TARGET_MONTH.ToString("D2") + ".xlsx";

            XlsxCreator xlsxCreator = new XlsxCreator();
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();

            try
            {
                xlsxCreator.OpenBook(memoryStream, strInFileName);
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

                var cost = product.QUANTITY * product.REAL_PRICE;
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
    }
}