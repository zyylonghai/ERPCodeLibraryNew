using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;

namespace Library.Core
{
    public class ExcelsOperateHelp
    {
        ExcelPackage package;
        ExcelWorksheet worksheet;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="path"></param>
        public ExcelsOperateHelp(string sheetName = "", string path = "")
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                if (!string.IsNullOrEmpty(path))
                {
                    package = new ExcelPackage(new FileInfo(path));
                }
                else
                {
                    package = new ExcelPackage();
                }

                if (package.Workbook.Worksheets.Count > 0)
                {
                    worksheet = package.Workbook.Worksheets[0];
                }
                else
                {
                    CreateSheet(DateTime.Now.ToString("yyyyMMdd"));
                }

                if (!string.IsNullOrWhiteSpace(sheetName))
                {
                    worksheet.Name = sheetName;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 创建工作薄
        /// </summary>
        /// <param name="sheetName"></param>
        public void CreateSheet(string sheetName)
        {
            try
            {
                worksheet = package.Workbook.Worksheets.Add(sheetName);//创建worksheet

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 切换工作薄
        /// </summary>
        /// <param name="index"></param>
        public void ChangeSheet(int index)
        {
            try
            {
                worksheet = package.Workbook.Worksheets[index];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 切换工作簿
        /// </summary>
        /// <param name="sheetName"></param>
        public void ChangeSheet(string sheetName)
        {
            try
            {
                worksheet = package.Workbook.Worksheets[sheetName];
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        ///// <summary>
        ///// 保存excel
        ///// </summary>
        ///// <param name="password"></param>
        //public byte[] ExportExcel(HttpResponseBase response, string excelName)
        //{
        //    try
        //    {
        //        if (package != null)
        //        {
        //            if (!string.IsNullOrEmpty(excelName))
        //            {
        //                //package.Save();
        //                //package.SaveAs();
        //                response.BinaryWrite(package.GetAsByteArray());
        //                response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //                response.AddHeader("content-disposition", "attachment;  filename=" + excelName + ".xlsx");
        //            }
        //            else
        //            {
        //                response.BinaryWrite(package.GetAsByteArray());
        //                response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //                response.AddHeader("content-disposition", "attachment;  filename=" + (DateTime.Now.ToString("yyyyMMddHHmmss")) + ".xlsx");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// 通过索引赋值，索引从1开始
        /// </summary>
        /// <param name="x">行</param>
        /// <param name="y">列</param>
        /// <param name="value"></param>
        public void SetValue(int row, int col, string value)
        {
            worksheet.Cells[row, col].Value = value;//直接指定行列数进行赋值
        }

        /// <summary>
        /// 单元格赋值
        /// </summary>
        /// <param name="cell">单元格，如：A1</param>
        /// <param name="value"></param>
        public void SetValue(string cell, string value)
        {
            worksheet.Cells[cell].Value = value;//直接指定单元格进行赋值
        }

        /// <summary>
        /// 设置样式
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="isWrapText">是否换行</param>
        /// <param name="horizontal">水平格式</param>
        /// <param name="vertical">垂直格式</param>
        /// <param name="isBold">是否粗体</param>
        /// <param name="size">文字大小</param>
        /// <param name="height">行高</param>
        /// <param name="isShowGridLines">是否显示网格线</param>
        public void SetStyle(int x, int y, bool isWrapText = true, ExcelHorizontalAlignment horizontal = ExcelHorizontalAlignment.Center, ExcelVerticalAlignment vertical = ExcelVerticalAlignment.Center, bool isBold = false, int size = 12, int height = 15, bool isShowGridLines = false)
        {
            //worksheet.Cells[x, y].Style.Numberformat.Format = "#,##0.00";//这是保留两位小数

            worksheet.Cells[x, y].Style.HorizontalAlignment = horizontal;//水平居中
            worksheet.Cells[x, y].Style.VerticalAlignment = vertical;//垂直居中
            //worksheet.Cells[1, 4, 1, 5].Merge = true;//合并单元格
            worksheet.Cells.Style.WrapText = isWrapText;//自动换行

            worksheet.Cells[x, y].Style.Font.Bold = isBold;//字体为粗体
            //worksheet.Cells[1, 1].Style.Font.Color.SetColor(Color.White);//字体颜色
            //worksheet.Cells[1, 1].Style.Font.Name = "微软雅黑";//字体
            worksheet.Cells[x, y].Style.Font.Size = size;//字体大小

            //worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            //worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(128, 128, 128));//设置单元格背景色

            worksheet.Cells[x, y].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.FromArgb(191, 191, 191));//设置单元格所有边框
            worksheet.Cells[x, y].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;//单独设置单元格底部边框样式和颜色（上下左右均可分开设置）
            worksheet.Cells[x, y].Style.Border.Bottom.Color.SetColor(Color.FromArgb(191, 191, 191));

            //worksheet.Cells.Style.ShrinkToFit = true;//单元格自动适应大小
            worksheet.Row(x).Height = height;//设置行高
                                             //worksheet.Row(1).CustomHeight = true;//自动调整行高
                                             //worksheet.Column(1).Width = 15;//设置列宽

            worksheet.View.ShowGridLines = isShowGridLines;//去掉sheet的网格线
            //worksheet.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //worksheet.Cells.Style.Fill.BackgroundColor.SetColor(Color.LightGray);//设置背景色
            //worksheet.BackgroundImage.Image = Image.FromFile(@"firstbg.jpg");//设置背景图片
        }

        public void SetMergeCell(int x1, int y1, int x2, int y2)
        {
            worksheet.Cells[x1, y1, x2, y2].Merge = true;//合并单元格
        }

        public byte[] TableToExcel(DataTable dt, string title)
        {
            DataColumnCollection columns = dt.Columns;
            int totalcols = dt.Columns.Count;
            int addIndex = 1;
            worksheet.View.ShowGridLines = false;//去掉sheet的网格线
            using (var heard = worksheet.Cells[addIndex, 1, addIndex, totalcols])
            {
                heard.Style.Font.Bold = true;//设置字体为粗体
                heard.Style.Fill.PatternType = ExcelFillStyle.Solid;
                heard.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);//这两行共同设置背景颜色
                heard.Style.Font.Color.SetColor(Color.Black);//设置字体颜色
                heard.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.FromArgb(191, 191, 191));//设置单元格所有边框
            }
            using (var datarange = worksheet.Cells[addIndex + 1, 1, dt.Rows.Count + 1, dt.Columns.Count])
            {
                datarange.Style.Font.Bold = false;//设置字体为粗体
                datarange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                datarange.Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);//这两行共同设置背景颜色
                datarange.Style.Font.Color.SetColor(Color.Black);//设置字体颜色
                //datarange.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.FromArgb(191, 191, 191));//设置单元格所有边框
            }

            //表头
            if (columns.Count > 0)
            {
                int columnIndex = 1;
                ExcelRange colrange = null;
                foreach (DataColumn dc in columns)
                {
                    colrange = worksheet.Cells[addIndex, columnIndex, dt.Rows.Count, columnIndex];
                    colrange.Style.Border.Right.Style = ExcelBorderStyle.Thin;//单独设置单元格底部边框样式和颜色（上下左右均可分开设置）
                    colrange.Style.Border.Right.Color.SetColor(Color.White);
                    SetValue(addIndex, columnIndex, dc.Caption);
                    columnIndex += 1;
                }
            }

            //数据
            if (dt.Rows.Count > 0)
            {
                int rowIndex = 1 + addIndex;
                ExcelRange rowrange = null;
                foreach (DataRow dr in dt.Rows)
                {
                    rowrange = worksheet.Cells[rowIndex, 1, rowIndex, totalcols];
                    rowrange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;//单独设置单元格底部边框样式和颜色（上下左右均可分开设置）
                    rowrange.Style.Border.Bottom.Color.SetColor(Color.White);
                    if (rowIndex % 2 == 0)
                    {
                        rowrange.Style.Fill.BackgroundColor.SetColor(Color.Aqua);
                    }
                    for (int i = 0; i < columns.Count; i++)
                    {
                        SetValue(rowIndex, i + 1, dr[i].ToString());
                    }

                    rowIndex += 1;
                }
            }
            return package.GetAsByteArray();
        }
    }
}
