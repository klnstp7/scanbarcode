using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Newtonsoft.Json;
using System.Media;
using System.IO.Ports;

namespace ScanBarCode
{
    public partial class frmMain : Form
    {
        [DllImport("kernel32.dll", EntryPoint = "Beep")]
        public static extern int Beep(int dwFreq,int dwDuration);

        [DllImport("user32.dll")]
        public static extern int MessageBeep(uint uType);

        private string filePath;
        private Parameter parameter;
        private PortControlHelper port;

        private FormWindowState currentState;

        frmStandard frmStandard = null;

        public frmMain()
        {
            InitializeComponent();
            port = new PortControlHelper();
            port.OnComReceiveDataHandler += new PortControlHelper.ComReceiveDataHandler(ComReceiveData);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.initFilePath();
            this.init();
            this.ActiveControl = this.tbxBarCode;
            this.tbxBarCode.Focus();
        }

        /// <summary>
        /// 初始化文件
        /// </summary>
        private void initFilePath()
        {
            string appPath = Application.StartupPath;
            string fileName = "Setup.txt";
            filePath = String.Format("{0}\\{1}", appPath, fileName);
            if (!File.Exists(filePath))
            {
                parameter=  ParaHelper.initParameter(filePath);
            }
        }

        private void init()
        {
            parameter = ParaHelper.readParameter(filePath);       
            //显示参数
            this.tbxStandard.Text = parameter.standardValue;
           
            if (!this.timer1.Enabled) this.timer1.Enabled = true;
           
        }

        private void btnPara_Click(object sender, EventArgs e)
        {
            frmPara frmPara = new frmPara(filePath, parameter);
            if (frmPara.ShowDialog() == DialogResult.OK)
            {
                this.init();
                if (port.PortState) port.ClosePort();
            }
        }

        private void btnStandard_Click(object sender, EventArgs e)
        {
            frmStandard = new frmStandard(filePath, parameter);
            if (frmStandard.ShowDialog() == DialogResult.OK)
            {
                this.init();
            }
            frmStandard = null;
        }


        /// <summary>
        /// 接收数据，判断结果
        /// </summary>
        /// <param name="data"></param>
        private void ComReceiveData(string data)
        {
            this.Invoke(new EventHandler(delegate
            {    
                if (!String.IsNullOrEmpty(data))
                {
                    data = data.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", ""); ;
                }
                if (frmStandard != null)
                {
                    frmStandard.tbxStandard.Text = data;
                    frmStandard.btnSubmit.PerformClick();
                }
                else
                {
                    this.tbxBarCode.Text = data;
                    if (String.Equals(parameter.standardValue, data, StringComparison.CurrentCultureIgnoreCase))
                    {
                        this.picResult.Image = global::ScanBarCode.Properties.Resources.green;
                    }
                    else
                    {
                        this.picResult.Image = global::ScanBarCode.Properties.Resources.red;

                        switch (parameter.soundType)
                        {
                            case "主板蜂鸣器":
                                Task.Factory.StartNew(() =>
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        Beep(1000, 500);
                                    }
                                });
                                break;
                            case "系统告警声":
                                Task.Factory.StartNew(() =>
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        MessageBeep(0x00000010);
                                        Thread.Sleep(1000);
                                    }
                                });
                                break;
                            case "告警音频":
                                Task.Factory.StartNew(() =>
                                {
                                    SoundPlayer simpleSound = new SoundPlayer(global::ScanBarCode.Properties.Resources._2);
                                    simpleSound.Play();
                                });
                                break;
                            default:
                                break;
                        }

                    }
                }
               
            }));
        }

        #region 任务栏图标


        private void 显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Show();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否确认退出程序？", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                // 关闭所有的线程
                this.notifyIcon1.Visible = false;
                this.Dispose();
                this.Close();
            }
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                currentState = this.WindowState;
            }
        }

        private void frmMain_Deactivate(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.Visible = false;
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.Visible)
                this.Visible = false;
            else
                this.Visible = true;
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = currentState;
            else
                this.WindowState = FormWindowState.Minimized;
        }


        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("是否确认退出程序？", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                // 关闭所有的线程
                this.Dispose();
                this.Close();
            }
            else
            {
                e.Cancel = true;
            }
        }

        #endregion

        #region 检查状态
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Enabled = false;
            if (!String.IsNullOrWhiteSpace(parameter.portName) && !String.IsNullOrWhiteSpace(parameter.soundType))
            {
                string[] portNames = SerialPort.GetPortNames();
                if (!portNames.Contains(parameter.portName))
                {
                    this.lblLinkStatus.Text = "连接失败";       
                    port.ClosePort();
                }
                else
                {
                    if (!port.PortState)
                    {
                      
                        try
                        {
                            port.OpenPort(parameter.portName, parameter.boudRate, parameter.dataBit, parameter.stopBit);
                            this.lblLinkStatus.Text = "已连接";
                        }
                        catch (Exception ex)
                        {
                     
                            port.ClosePort();
                            this.lblLinkStatus.Text = "连接失败";
                        }
                    }
                } 
            }
            else
            {
                this.lblLinkStatus.Text = "未设置参数";
            }
            this.timer1.Enabled = true;
        }
        #endregion



    }
}
