using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace ScanBarCode
{
    public partial class frmPara : Form
    {
        private string _filePath;
        private Parameter _parameter;

        public frmPara(string filePath, Parameter parameter)
        {    
            InitializeComponent();
            _filePath = filePath;
            _parameter = parameter;
        }

        private void frmPara_Load(object sender, EventArgs e)
        {
           
            foreach (string portName in SerialPort.GetPortNames())
            {
                this.comName.Items.Add(portName);
            }

            foreach (string soundType in new string[] { "主板蜂鸣器", "系统告警声", "告警音频" })
            {
                this.comWarn.Items.Add(soundType);
            }


            this.comName.SelectedItem = _parameter.portName;
            this.comWarn.SelectedItem = _parameter.soundType;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            _parameter.portName = String.Format("{0}",this.comName.SelectedItem);
            _parameter.soundType = String.Format("{0}", this.comWarn.SelectedItem);
            if (String.IsNullOrWhiteSpace(_parameter.portName))
            {
                MessageBox.Show("请选择Com串口！", "系统提示");
                return;
            }
            if (String.IsNullOrWhiteSpace(_parameter.soundType))
            {
                MessageBox.Show("请选择告警方式！", "系统提示");
                return;
            }
            try
            {
                ParaHelper.saveParameter(_filePath,_parameter);
                if (MessageBox.Show("操作成功", "系统提示") == DialogResult.OK)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示");

            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
