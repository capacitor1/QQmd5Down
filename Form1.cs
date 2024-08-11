using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace QQmd5Down
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.MD5.KeyDown += new KeyEventHandler(textBox1_KeyDown);
            _saveDir = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory);

        }
        private string _saveDir;
        private void textBox1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //if条件检测按下的是不是Enter键
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }
        public static bool IsHexString(string hexString)
        {
            if (Regex.IsMatch(hexString, "^[0-9A-Fa-f]+$"))
            {
                //校验成功
                return true;
            }
            //校验失败
            return false;
        }
        public static FileExtension CheckFile(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
            string fileType = string.Empty;
            try
            {
                byte data = br.ReadByte();
                fileType += data.ToString();
                data = br.ReadByte();
                fileType += data.ToString();
                FileExtension extension;
                try
                {
                    extension = (FileExtension)Enum.Parse(typeof(FileExtension), fileType);
                }
                catch
                {

                    extension = FileExtension.VALIDFILE;
                }
                return extension;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    br.Close();
                }
            }
        }

        private bool ex;
        public enum FileExtension
        {
            JPG = 255216,
            GIF = 7173,
            PNG = 13780,
            SWF = 6787,
            RAR = 8297,
            ZIP = 8075,
            _7Z = 55122,
            VALIDFILE = 9999999
        }
        

        /// <summary>        
        /// c#,.net 下载文件        
        /// </summary>        
        /// <param name="URL">下载文件地址</param>       
        /// 
        /// <param name="Filename">下载后的存放地址</param>        
        /// <param name="Prog">用于显示的进度条</param>        
        /// 
        public void DownloadFile(string URL, string filename, System.Windows.Forms.ProgressBar prog, System.Windows.Forms.Label label1)
        {
            float percent = 0;
            try
            {
                System.Net.HttpWebRequest Myrq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URL);
                System.Net.HttpWebResponse myrp = (System.Net.HttpWebResponse)Myrq.GetResponse();
                long totalBytes = myrp.ContentLength;
                if (prog != null)
                {
                    prog.Maximum = (int)totalBytes;
                }
                System.IO.Stream st = myrp.GetResponseStream();
                System.IO.Stream so = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                long totalDownloadedByte = 0;
                byte[] by = new byte[1024];
                int osize = st.Read(by, 0, (int)by.Length);
                while (osize > 0)
                {
                    totalDownloadedByte = osize + totalDownloadedByte;
                    System.Windows.Forms.Application.DoEvents();
                    so.Write(by, 0, osize);
                    if (prog != null)
                    {
                        prog.Value = (int)totalDownloadedByte;
                    }
                    osize = st.Read(by, 0, (int)by.Length);

                    percent = (float)totalDownloadedByte / (float)totalBytes * 100;
                    label1.Text = percent.ToString() + "%";
                    System.Windows.Forms.Application.DoEvents(); //必须加注这句代码，否则label1将因为循环执行太快而来不及显示信息
                }
                so.Close();
                st.Close();
                ex = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                ex = true;
            }



        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.MD5.ReadOnly = true;
            string imageurl = "https://gchat.qpic.cn/gchatpic_new/0/0-0-" + MD5.Text + "/0";
            if (MD5.Text == string.Empty)
            {
                MessageBox.Show("下载链接为空！");
            }
            else if (MD5.Text.Length < 32 || !IsHexString(MD5.Text))
            {
                MessageBox.Show("下载链接不正确（非MD5）！");
            }
            else
            {
                string tfp = Path.Combine(_saveDir, MD5.Text);
                if (!System.IO.File.Exists(tfp + ".jpg") & !System.IO.File.Exists(tfp + ".png") & !System.IO.File.Exists(tfp + ".gif") & !System.IO.File.Exists(tfp + ".bin"))
                {
                    string filePath = Path.Combine(_saveDir, MD5.Text + ".dwnl");

                    DownloadFile(imageurl, filePath, DLpgs, DLpct);
                    if (ex == false)
                    {
                        FileInfo f = new FileInfo(filePath);
                        string fp = CheckFile(filePath).ToString();
                        if (fp == "JPG")
                        {
                            filePath = Path.ChangeExtension(filePath, "jpg");
                            f.MoveTo(filePath);
                        }

                        else if (fp == "PNG")
                        {
                            filePath = Path.ChangeExtension(filePath, "png");
                            f.MoveTo(filePath);
                        }

                        else if (fp == "GIF")
                        {
                            filePath = Path.ChangeExtension(filePath, "gif");
                            f.MoveTo(filePath);
                        }

                        else
                        {
                            filePath = Path.ChangeExtension(filePath, "bin");
                            f.MoveTo(filePath);
                        }
                        MessageBox.Show(MD5.Text + "." + CheckFile(filePath).ToString() + "下载完成！");
                    }
                }
                else
                {
                    MessageBox.Show(MD5.Text + " 已经存在！");
                }
                

            }
            this.MD5.ReadOnly = false;
            if(chk.Checked == true)
            {
                MD5.Clear();
            }
            MD5.Focus();

        }
        
}
}
