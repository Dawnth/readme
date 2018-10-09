using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;
using System.Collections;
using System.Diagnostics;

namespace Readme
{
    public partial class FormMain : Form
    {
        // Text Name
        string sTextName = "";
        // Text had been uploaded flag
        bool bTextUploadFlag = false;
        // Directory
        string sInitialDirectory = "";
        // Line Number
        int iTextSelectionStart = 0;
        // Zoom factor
        float iTextZoomFactor = 1;

        // Find parameter
        int iFindIndex = 0;
        // Find result
        bool bFindFlag = false;

        // Timer counter for show the file name of txt
        int iTimerShowCounter = 0;
        int iTimerMaxCounter = 16;

        public FormMain()
        {
            InitializeComponent();

            // Register the mouse wheel event
            this.richTextBox1.MouseWheel += new MouseEventHandler(richTextBox1_MouseWheel);

            // Disable max button
            this.MaximizeBox = false;

            this.textBoxName.BringToFront();

            AdjustComboBoxDropDownListWidth(comboBox1);

            richTextBox1.Text = "Version " + Application.ProductVersion + "\n\n" +
                                "alt + o : open folder \n" +
                                "alt + r : reload last file\n" +
                                "ctl + mouse wheel: to change font size\n" +
                                "z: page up\n" +
                                "x: page down\n" +
                                "alt + c : boss key\n\n" +
                                "Author: Anthony\n" +
                                "Email: cvcds @sina.com";
        }

        // Load
        private void Form1_Load(object sender, EventArgs e)
        {
            // Backup Size:
            // 290, 240
            try
            {
                // Get screen width and height
                Size ssize = SystemInformation.WorkingArea.Size;

                // Set default as gray
                comboBox1.SelectedIndex = 0;
                // Clear TextBox
                //richTextBox1.Clear();
                // Add Config
                if (File.Exists(Application.StartupPath + @"\ReadmeConfig.cds"))
                {
                    // Read
                    XmlDocument cdsxml = new XmlDocument();
                    cdsxml.Load(Application.StartupPath + @"\ReadmeConfig.cds");

                    // Get txt file name
                    sTextName = cdsxml.SelectSingleNode("Config/TextName").InnerText;

                    // Get txt file directory
                    sInitialDirectory = cdsxml.SelectSingleNode("Config/InitialDirectory").InnerText;
                    openFileDialog1.InitialDirectory = sInitialDirectory;

                    // Get selected position
                    iTextSelectionStart = Convert.ToInt32(cdsxml.SelectSingleNode("Config/TextSelectionStart").InnerText);

                    // Get zoom factor
                    iTextZoomFactor = float.Parse(cdsxml.SelectSingleNode("Config/TextZoomFactor").InnerText);
                    if (iTextZoomFactor < 0.1f)
                    {
                        iTextZoomFactor = 0.1f;
                    }
                    else if (iTextZoomFactor > 5)
                    {
                        iTextZoomFactor = 5;
                    }

                    // Form Width
                    this.Width = Convert.ToInt32(cdsxml.SelectSingleNode("Config/FormWidth").InnerText);
                    if (this.Width <= 0)
                    {
                        this.Width = 290;
                    }

                    // Form Height
                    this.Height = Convert.ToInt32(cdsxml.SelectSingleNode("Config/FormHeight").InnerText);
                    if (this.Height <= 0)
                    {
                        this.Height = 240;
                    }

                    // Position left and right
                    this.Left = Convert.ToInt32(cdsxml.SelectSingleNode("Config/FormPositionX").InnerText);
                    if (this.Left <= -(this.Width - 36))
                    {
                        // Limit of left position
                        this.Left = -(this.Width - 36);
                    }
                    else if (this.Left >= (ssize.Width - 36))
                    {
                        // Limit of right position
                        this.Left = ssize.Width - 36;
                    }

                    // Position top and bottom
                    this.Top = Convert.ToInt32(cdsxml.SelectSingleNode("Config/FormPositionY").InnerText);
                    if (this.Top <= 0)
                    {
                        // Limit of top position
                        this.Top = 0;
                    }
                    else if (this.Top >= (ssize.Height - 36))
                    {
                        // Limit of bottom position
                        this.Top = ssize.Height - 36;
                    }

                    // Opacity upload
                    if (Convert.ToInt32(cdsxml.SelectSingleNode("Config/OpacityValue").InnerText) != 0)
                    {
                        trackBar1.Value = Convert.ToInt32(cdsxml.SelectSingleNode("Config/OpacityValue").InnerText);
                        setFormOpacity();
                    }

                    // richTextBox font color
                    if ((Convert.ToInt32(cdsxml.SelectSingleNode("Config/ColorIndex").InnerText) < 0) ||
                        (Convert.ToInt32(cdsxml.SelectSingleNode("Config/ColorIndex").InnerText) > (comboBox1.Items.Count - 1))
                       )
                    {
                        comboBox1.SelectedIndex = 0;
                    }
                    else
                    {
                        comboBox1.SelectedIndex = Convert.ToInt32(cdsxml.SelectSingleNode("Config/ColorIndex").InnerText);
                    }


                    richTextBox1.Focus();

                }
                else if (!File.Exists(Application.StartupPath + @"\ReadmeConfig.cds"))
                {
                    fBuildConfigFile();
                }
            }
            catch
            {
                fBuildConfigFile();
            }
        }

        // Build Configuration
        private void fBuildConfigFile()
        {
            File.Delete(Application.StartupPath + @"\ReadmeConfig.cds");
            XmlDocument cdsxml = new XmlDocument();
            cdsxml.LoadXml(
                "<Config>" +
                "<TextName></TextName>" +
                "<InitialDirectory></InitialDirectory>" +
                "<TextSelectionStart>0</TextSelectionStart>" +
                "<TextZoomFactor>1</TextZoomFactor>" +
                "<FormWidth>290</FormWidth>" +
                "<FormHeight>240</FormHeight>" +
                "<FormPositionX>0</FormPositionX>" +
                "<FormPositionY>0</FormPositionY>" +
                "<OpacityValue>10</OpacityValue>" +
                "<ColorIndex>0</ColorIndex>" +
                "</Config>");
            cdsxml.Save(Application.StartupPath + @"\ReadmeConfig.cds");
            // Set Hidden
            File.SetAttributes(Application.StartupPath + @"\ReadmeConfig.cds", FileAttributes.System | FileAttributes.Hidden);
            //File.SetAttributes(Application.StartupPath + @"\ReadmeConfig.cds", FileAttributes.Hidden);
        }

        // When form closing
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            fWriteConfig();
        }

        // Top most
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost = false;
            }
            // richTextBox Get Focus
            richTextBox1.Focus();
        }

        // Open File
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog1.InitialDirectory == "")
                {
                    openFileDialog1.InitialDirectory = Application.StartupPath;
                    openFileDialog1.ShowDialog();
                }
                else
                {
                    // If not empty, upload the newest InitialDirectory
                    openFileDialog1.InitialDirectory = sInitialDirectory;
                    openFileDialog1.ShowDialog();

                }
                // richTextBox Get Focus
                richTextBox1.Focus();
            }
            catch
            {
                openFileDialog1.InitialDirectory = Application.StartupPath;
                openFileDialog1.ShowDialog();
            }

        }

        // opendFileDiaglog click ok
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                // Directory
                sInitialDirectory = openFileDialog1.FileName.Substring(0, openFileDialog1.FileName.LastIndexOf("\\") + 1);
                // Name
                sTextName = openFileDialog1.FileName.Substring(openFileDialog1.FileName.LastIndexOf("\\") + 1);

                // Clear the last read position
                iTextSelectionStart = 0;

                fWriteConfig();

                // Read whole file
                fReadWholeFile(openFileDialog1.FileName, iTextSelectionStart, iTextZoomFactor);

                // Then show the current reading percent
                this.Text = "FormMain" + " - " + (Convert.ToSingle(iTextSelectionStart * 10000 / richTextBox1.TextLength) / 100).ToString("f2") + "%";

                timer1.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //  Get and show txt File
        private void fReadWholeFile(string sFN, int index, float zoom)
        {
            try
            {
                // richTextBox get focus
                richTextBox1.Focus();

                richTextBox1.Clear();
                // Add All Text
                richTextBox1.AppendText(File.ReadAllText(sFN, UnicodeEncoding.GetEncoding("GB2312")));

                // Get the zoom value
                richTextBox1.ZoomFactor = zoom;

                // Get the position
                richTextBox1.SelectionStart = index;

                // Scroll to selected position
                richTextBox1.ScrollToCaret();

                // Set text had been uploaded flag to true
                bTextUploadFlag = true;

                // Reset bFindFlag
                bFindFlag = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Write Config
        private void fWriteConfig()
        {
            try
            {
                File.SetAttributes(Application.StartupPath + @"\ReadmeConfig.cds", FileAttributes.Normal);

                XmlDocument cdsxml = new XmlDocument();
                cdsxml.Load(Application.StartupPath + @"\ReadmeConfig.cds");

                cdsxml.SelectSingleNode("Config/TextName").InnerText = sTextName;
                cdsxml.SelectSingleNode("Config/InitialDirectory").InnerText = sInitialDirectory;
                cdsxml.SelectSingleNode("Config/TextSelectionStart").InnerText = iTextSelectionStart.ToString();
                cdsxml.SelectSingleNode("Config/TextZoomFactor").InnerText = richTextBox1.ZoomFactor.ToString();
                cdsxml.SelectSingleNode("Config/FormWidth").InnerText = this.Width.ToString();
                cdsxml.SelectSingleNode("Config/FormHeight").InnerText = this.Height.ToString();
                cdsxml.SelectSingleNode("Config/FormPositionX").InnerText = this.Left.ToString();
                cdsxml.SelectSingleNode("Config/FormPositionY").InnerText = this.Top.ToString();
                cdsxml.SelectSingleNode("Config/OpacityValue").InnerText = trackBar1.Value.ToString();
                cdsxml.SelectSingleNode("Config/ColorIndex").InnerText = comboBox1.SelectedIndex.ToString();

                cdsxml.Save(Application.StartupPath + @"\ReadmeConfig.cds");

                File.SetAttributes(Application.StartupPath + @"\ReadmeConfig.cds", FileAttributes.System | FileAttributes.Hidden);
            }
            catch
            {
                fBuildConfigFile();
            }
        }

        // Reload the last txt
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (sInitialDirectory != "" && sTextName != "")
                {
                    // Read whole file
                    fReadWholeFile(sInitialDirectory + sTextName, iTextSelectionStart, iTextZoomFactor);

                    // Then show the current reading percent
                    this.Text = "FormMain" + " - " + (Convert.ToSingle(iTextSelectionStart * 10000 / richTextBox1.TextLength) / 100).ToString("f2") + "%";

                    timer1.Enabled = true;
                }
            }
            catch
            {

                fBuildConfigFile();
            }
        }

        // Get RichTextBox SelectionStart Value
        public void fGetSelectionStart()
        {
            if (richTextBox1.Text != "" && bTextUploadFlag == true)
            {
                // Get Position
                iTextSelectionStart = richTextBox1.SelectionStart;

                // Then show the current reading percent
                this.Text = "FormMain" + " - " + (Convert.ToSingle(iTextSelectionStart * 10000 / richTextBox1.TextLength) / 100).ToString("f2") + "%";
            }
        }

        // Page Up
        private void button1_Click(object sender, EventArgs e)
        {
            pageUpEvent();
        }

        // Page Down
        private void button2_Click(object sender, EventArgs e)
        {
            pageDownEvent();
        }

        private void pageUpEvent()
        {
            // richTextBox Get Focus
            richTextBox1.Focus();
            SendKeys.SendWait("{PGUP}");
            richTextBox1.Focus();

            // Get Position
            fGetSelectionStart();

            // Get Focus
            button1.Focus();
        }

        private void pageDownEvent()
        {
            // richTextBox Get Focus
            richTextBox1.Focus();
            SendKeys.SendWait("{PGDN}");
            richTextBox1.Focus();

            // Get Position
            fGetSelectionStart();

            // Get Focus
            button2.Focus();
        }

        // Mouse Click
        private void richTextBox1_MouseClick(object sender, MouseEventArgs e)
        {
            // Get Position
            fGetSelectionStart();

            // Hide and disable
            textBoxFind.Visible = false;
            button6.Visible = false;
            timer1.Enabled = false;
            textBoxName.Visible = false;
            bFindFlag = false;
        }

        // Font size control
        private void richTextBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            //if (e.Delta > 0 && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            //{
                //richTextBox1.Font.Size = richTextBox1.Font.Size + 1;
            //}
            //else if (e.Delta < 0 && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            //{
                //richTextBox1.Font.Size = richTextBox1.Font.Size - 1;
            //}
        }

        // Press PageUp And PageDown Get The Selection Start
        private void richTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            textBox1.Text = e.KeyCode.ToString() + "  " + e.KeyValue.ToString();
            // Save position when press PGUP/33 PGDN/34 END/35 HOME/36 LEFT/37 UP/38 RIGHT/39 DOWN/40 
            if (e.KeyValue >= 33 && e.KeyValue <= 40)
            {
                // Get Position
                fGetSelectionStart();
            }
        }

        // scorll the trackbar to set the form opacity
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            setFormOpacity();
        }

        // set the Form Opacity
        private void setFormOpacity()
        {
            try
            {
                this.Opacity = (float)trackBar1.Value / 10;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // web browser
        private void button5_Click(object sender, EventArgs e)
        {
            Form1 fm = new Form1();
            fm.Show();
        }

        // richTextBox's font Color
        Color[] mycomboBoxColor = new Color[] 
        {
            Color.Gray,
            Color.Black,
            Color.Blue,
            Color.Red,
            Color.Gold,
            Color.Lime,
            Color.Cyan,
            Color.Brown,
            Color.Yellow,
            Color.Pink,
            Color.White,
            Color.MidnightBlue,
            Color.Teal,
            Color.DarkSlateGray,
        };
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            richTextBox1.ForeColor = mycomboBoxColor[comboBox1.SelectedIndex];
        }

        // Item's width change
        private void AdjustComboBoxDropDownListWidth(object comboBox)
        {
            Graphics g = null;
            Font font = null;
            try
            {
                ComboBox senderComboBox = null;
                if (comboBox is ComboBox)
                    senderComboBox = (ComboBox)comboBox;
                else if (comboBox is ToolStripComboBox)
                    senderComboBox = ((ToolStripComboBox)comboBox).ComboBox;
                else
                    return;

                int width = senderComboBox.Width;
                g = senderComboBox.CreateGraphics();
                font = senderComboBox.Font;

                //checks if a scrollbar will be displayed.
                //If yes, then get its width to adjust the size of the drop down list.
                int vertScrollBarWidth =
                    (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems)
                    ? SystemInformation.VerticalScrollBarWidth : 0;

                int newWidth;
                foreach (object s in senderComboBox.Items)  //Loop through list items and check size of each items.
                {
                    if (s != null)
                    {
                        newWidth = (int)g.MeasureString(s.ToString().Trim(), font).Width
                            + vertScrollBarWidth;
                        if (width < newWidth)
                            width = newWidth;   //set the width of the drop down list to the width of the largest item.
                    }
                }
                senderComboBox.DropDownWidth = width;
            }
            catch
            { }
            finally
            {
                if (g != null)
                    g.Dispose();
            }
        }

        // Find string
        private void findStringInTXT(string sFindString)
        {
            try
            {
                if (sFindString.Trim() != "")
                {
                    // If no result or first find, find the word from Selection
                    if (bFindFlag == false)
                    {
                        iFindIndex = richTextBox1.Find(sFindString, richTextBox1.SelectionStart, RichTextBoxFinds.None);
                    }
                    // If already find a result, find the word from Selection + sFindString.Length
                    else
                    {
                        iFindIndex = richTextBox1.Find(sFindString, richTextBox1.SelectionStart + sFindString.Length, RichTextBoxFinds.None);
                    }

                    // If find a result from iFindIndex to end
                    if (iFindIndex != -1)
                    {
                        // Set bFindFlag to true
                        bFindFlag = true;

                        richTextBox1.SelectionStart = iFindIndex;
                        richTextBox1.SelectionLength = sFindString.Length;
                        // Blue Bold 12# Times New Roman
                        //richTextBox1.SelectionColor = Color.Blue;
                        //richTextBox1.SelectionFont = new Font("Times New Roman", (float)12, FontStyle.Bold);
                        richTextBox1.Focus();

                        // Hide search control
                        textBoxFind.Visible = false;
                        button6.Visible = false;
                    }
                    else
                    {
                        // If not find a result from iFindIndex to end
                        // Try to find from 0 to Selection
                        iFindIndex = richTextBox1.Find(sFindString, 0, richTextBox1.SelectionStart + sFindString.Length, RichTextBoxFinds.None);
                        // Find a result
                        if (iFindIndex != -1)
                        {
                            // Set bFindFlag to true
                            bFindFlag = true;

                            richTextBox1.SelectionStart = iFindIndex;
                            richTextBox1.SelectionLength = sFindString.Length;
                            // Blue Bold 12# Times New Roman
                            //richTextBox1.SelectionColor = Color.Blue;
                            //richTextBox1.SelectionFont = new Font("Times New Roman", (float)12, FontStyle.Bold);
                            richTextBox1.Focus();

                            // Hide search control
                            textBoxFind.Visible = false;
                            button6.Visible = false;
                        }
                        else
                        {
                            //textBoxFind.BackColor = Color.Red;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Search
        private void button6_Click(object sender, EventArgs e)
        {
            findStringInTXT(textBoxFind.Text);
        }

        // Search when press enter
        private void textBoxFind_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                findStringInTXT(textBoxFind.Text);
            }
        }

        // Event when text changed
        private void textBoxFind_TextChanged(object sender, EventArgs e)
        {
        }

        // Timer used for show the file name of txt
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (iTimerShowCounter >= iTimerMaxCounter)
            {
                textBoxName.Text = "";
                textBoxName.Visible = false;

                timer1.Enabled = false;
                iTimerShowCounter = 0;
            }
            else
            {
                textBoxName.Text = "Loaded " + sTextName;
                textBoxName.Visible = true;
            }
            iTimerShowCounter++;
        }

        #region [------HotKey------]
        // Register HotKey
        // Form Activated
        private void Form1_Activated(object sender, EventArgs e)
        {
            // Register HotKey
            // HotKey.KeyModifiers
            HotKey.RegisterHotKey(Handle, 100, HotKey.KeyModifiers.Alt, Keys.C);
            HotKey.RegisterHotKey(Handle, 101, HotKey.KeyModifiers.None, Keys.Z);
            HotKey.RegisterHotKey(Handle, 102, HotKey.KeyModifiers.None, Keys.X);
            HotKey.RegisterHotKey(Handle, 103, HotKey.KeyModifiers.Ctrl, Keys.F);
        }

        // Unregister HotKey
        private void FormMain_Deactivate(object sender, EventArgs e)
        {
            // Unregister HotKey
            //HotKey.UnregisterHotKey(Handle, 100);
            HotKey.UnregisterHotKey(Handle, 101);
            HotKey.UnregisterHotKey(Handle, 102);
            HotKey.UnregisterHotKey(Handle, 103);
        }

        // Unregister HotKey
        private void Form1_Leave(object sender, EventArgs e)
        {
            // Unregister HotKey
            //HotKey.UnregisterHotKey(Handle, 100);
            HotKey.UnregisterHotKey(Handle, 101);
            HotKey.UnregisterHotKey(Handle, 102);
            HotKey.UnregisterHotKey(Handle, 103);
        }

        // monitoring Windows Message
        // override WndProc to Make Sure HotKey Useful
        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            // Press HotKey
            switch (m.Msg)
            {
                case WM_HOTKEY:
                    switch (m.WParam.ToInt32())
                    {
                        // Push Alt + C
                        case 100:
                            // Hiden Form
                            if (this.Visible == true)
                            {
                                this.Visible = false;
                            }
                            // Recovery Form
                            else
                            {
                                this.Visible = true;
                            }
                            break;
                        // Push Z
                        case 101:
                            pageUpEvent();
                            break;
                        // Push X
                        case 102:
                            pageDownEvent();
                            break;
                        // Push Ctrl + F
                        case 103:
                            textBoxFind.Visible = true;
                            button6.Visible = true;
                            textBoxFind.Focus();
                            break;
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
            //base.WndProc(ref m);
        }
        #endregion

    }
}