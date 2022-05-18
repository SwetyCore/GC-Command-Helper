using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GC_Command_Helper
{
    public static class GlobalProps
    {

        public delegate void setCMD(string cmd);

        public static setCMD SetCMD;


        public class SimpleItem
        {
            public string Name { get; set; }
            public string Id { get; set; }
        }

    }
}
