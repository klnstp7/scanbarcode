using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScanBarCode
{
    public abstract class ParaHelper
    {
        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="parameter"></param>
        public static Parameter initParameter(string filePath)
        {
            Parameter parameter = new Parameter()
            { 
                portName = "",
                boudRate = 115200,
                dataBit = 8,
                stopBit = 1,
                soundType = "",
                standardValue = ""
            };
            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);
            try
            {
                string data = JsonConvert.SerializeObject(parameter);
                sw.Write(data);
                sw.Flush();
            }
            catch (Exception ex)
            {
               
            }
            finally
            {
                sw.Close();
                fs.Close();
            }
            return parameter;
        }

        /// <summary>
        /// 读取参数
        /// </summary>
        public static Parameter readParameter(string filePath)
        {
            Parameter parameter = null; 
            using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    parameter = JsonConvert.DeserializeObject<Parameter>(line.ToString());
                }
            };
            return parameter;
        }


        /// <summary>
        /// 保存参数
        /// </summary>
        /// <param name="parameter"></param>
        public static void saveParameter(string filePath,Parameter parameter)
        {
            FileStream fs = new FileStream(filePath, FileMode.Truncate, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);
            try
            {
                string data = JsonConvert.SerializeObject(parameter);
                sw.Write(data);
                sw.Flush();
            }
            catch (Exception ex)
            { 
                return;
            }
            finally
            {
                sw.Close();
                fs.Close();
            }
        }
    }
}
