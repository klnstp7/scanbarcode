using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScanBarCode
{
    public class Parameter
    {

        public string portName { get; set; }

        public int boudRate { get; set; }

        public int dataBit { get; set; }

        public int stopBit { get; set; }

        public string soundType { get; set; }

        public string standardValue { get; set; }
    }
}
