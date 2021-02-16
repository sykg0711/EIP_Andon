using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TubeMarking_Data
{
    class TubeLib : EIPForm.EIPLib
    {
        public Form1 GetForm { get; set; }

        public override EIP_Status ReadInstance(byte instanceid, DataType dataType, bool isString, bool isBCD, ushort tcpport = 44818, int destination = 0)
        {
            return base.ReadInstance(instanceid, dataType, isString, isBCD, tcpport, destination);
        }

        public override void WriteInstance(byte instanceid, DataType dataType, byte[] sendData, bool isString, ushort tcpport = 44818)
        {
            base.WriteInstance(instanceid, dataType, sendData, isString, tcpport);
        }

    }
}
