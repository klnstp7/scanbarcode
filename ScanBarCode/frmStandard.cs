using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ScanBarCode
{
    public partial class frmStandard : Form
    {
        private string _filePath;
        private Parameter _parameter;

        public frmStandard(string filePath,Parameter parameter)
        {
            InitializeComponent();
            _filePath = filePath;
            _parameter = parameter;
        }

        private void frmStandard_Load(object sender, EventArgs e)
        {
            this.tbxStandard.Text = _parameter.standardValue;
        }

 

        public void btnSubmit_Click(object sender, EventArgs e)
        {
            _parameter.standardValue = this.tbxStandard.Text;
            if (String.IsNullOrWhiteSpace(_parameter.standardValue))
            {
                MessageBox.Show("请输入校验值！", "系统提示");
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
            catch(Exception ex)
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
