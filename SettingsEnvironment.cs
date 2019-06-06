using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DrawingTools
{
    public static class SettingsEnvironment
    {
        /// <summary>
        /// x, y, line_offset, line_width
        /// </summary>
        public static double[] MARK_SECTION;


        private static void Init()
        {

        }

        public static void Init(string env_name)
        {
            Init();
            if (env_name.ToLower() == "russia")
            {
                MARK_SECTION = new double[4]
                {
                    1.2,
                    0.75,
                    10,
                    12
                };
            }
        }
        
        public static string GetEnvironmentModel
        {
            get
            {
                try
                {
                    Tekla.Structures.Model.Model model = new Tekla.Structures.Model.Model();
                    Tekla.Structures.Model.ModelInfo mif = model.GetInfo();

                    string directory = mif.ModelPath;
                    string file = System.IO.Path.Combine(directory, "TeklaStructuresModel.xml");
                    XElement xml = XElement.Load(file);
                    return xml.Element("Model").Element("Environment").Value;
                }
                catch
                {
                    return "";
                }
            }
        }



    }
}
