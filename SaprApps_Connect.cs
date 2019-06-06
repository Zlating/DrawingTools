using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingTools
{
    public class SaprApps_Connect :IPlugin_arnkey
    {
        public string Name
        {
            get
            {
                return "Drawing Tools";
            }
        }

        public string Description
        {
            get
            {
                return "Drawing tools";
            }
        }

        public void Run()
        {
            DrawingTools.UI mw = new UI();
            mw.ShowDialog();
        }
    }
}
