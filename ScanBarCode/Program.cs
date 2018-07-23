using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScanBarCode
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool blnIsRunning;
            Mutex mutexApp = new Mutex(false, Assembly.GetExecutingAssembly().FullName, out blnIsRunning);
            if (!blnIsRunning)
            {
                MessageBox.Show("程序已经运行！", "提示",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
