using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;

namespace Readme
{
    public partial class Form1 : Form
    {
        float x = 0.55555555f;
        double y = 4.99999999;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int ti = 0;
            float tf = 0;
            double td = 0;

            ti = (int)x;
            ti = (int)y;

            tf = x;
            tf = (float)y;

            td = x;
            td = y;

            webBrowser1.Navigate("http://www.baidu.com");
            webBrowser1.ScriptErrorsSuppressed = false;

            // test weekday
            f_GetWeekDay(2015,11,18);
        }
        /*=========================================================
        *  Name:  void f_GetWeekDay(int i_Year, int i_Month, int i_Day)
        *  Description: 计算公元某年某月某日为星期几
        *  输入变量:  年,月,日
        *  输出变量:  星期几

        * 修改日期			修改人		修改说明
        * 2015-11-18		CDS 		完成
        =========================================================*/
        private void f_GetWeekDay(int i_Year, int i_Month, int i_Day)
        {
            int nDaySum;
            int iIndex;
            nDaySum = (i_Year - 1) * 365 + (i_Month - 1) * 30 + i_Day;
                                                                       
            for (iIndex = 1; iIndex < i_Year; iIndex++)
            {
                if (iIndex % 100 == 0)
                {
                    if (iIndex % 400 == 0)
                    {
                        nDaySum++;
                    }
                }
                else
                {
                    if (iIndex % 4 == 0)
                    {
                        nDaySum++;
                    }
                }
            }
            for (iIndex = 1; iIndex < i_Month; iIndex++)
            {
                if (iIndex == 1 || iIndex == 3 || iIndex == 5 || iIndex == 7 || iIndex == 8 || iIndex == 10)
                    nDaySum++;
                else if (iIndex == 2)
                {
                    if (i_Year % 100 == 0)
                    {
                        if (i_Year % 400 == 0)
                        {
                            nDaySum--;
                        }
                    }
                    else
                    {
                        if (i_Year % 4 == 0)
                        {
                            nDaySum--;
                        }
                        else
                        {
                            nDaySum -= 2;
                        }
                    }
                }
            }
            //nDaySum += 6;
            nDaySum %= 7;
            if (nDaySum == 0)
            {
                nDaySum = 7;
            }
            toolStripStatusLabel1.Text = i_Year.ToString() + "." + i_Month.ToString() + "." + i_Day.ToString() + "." + nDaySum.ToString();
            //	return (nDaySum == 0)? 7 : nDaySum;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text.Replace(" ", "").Substring(0, 7) != "http://")
                {
                    textBox1.Text = textBox1.Text.Insert(0, "http://");
                }

                webBrowser1.Url = new Uri(textBox1.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }

        private void webBrowser1_NewWindow(object sender, CancelEventArgs e)
        {
            //防止弹窗；
            e.Cancel = true;
            string url = this.webBrowser1.StatusText;
            this.webBrowser1.Url = new Uri(url);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            webBrowser1.Dispose();
            this.Dispose();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //if (webBrowser1.ReadyState != WebBrowserReadyState.Complete) return;
            //Size szb = new Size(webBrowser1.Document.Body.OffsetRectangle.Width,
            //    webBrowser1.Document.Body.OffsetRectangle.Height);
            //Size sz = webBrowser1.Size;

            //int xbili = (int)((float)sz.Width / (float)szb.Width * 100);//水平方向缩小比例
            //int ybili = (int)((float)sz.Height / (float)szb.Height * 100);//垂直方向缩小比例
            //webBrowser1.Document.Body.Style = "zoom:" + xbili.ToString() + "%";
            //webBrowser1.Invalidate();

            webBrowser1.Document.Body.Style = "zoom:70%";
            textBox1.Text = webBrowser1.Url.ToString();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }
    }
}