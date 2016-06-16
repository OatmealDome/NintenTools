using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Syroot.NintenTools.ObjInfoDumper
{
    /// <summary>
    /// The main class of the program containing the application entry point.
    /// </summary>
    internal class Program
    {
        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private static void Main(string[] args)
        {
            // Read the ObjFlow XML.
            XDocument file = XDocument.Load(
                @"D:\Archive\Hacking\Wii U\_Games\Mario Kart 8 USA 1.0 (v0) (merged with DLC)\content\data\objflow.xml");
            IEnumerable<ObjFlowValue> values = from value in file.Root.Elements("value")
                select new ObjFlowValue
                {
                    AiReact = int.Parse(value.Attribute("AiReact").Value),
                    MgrId = int.Parse(value.Attribute("MgrId").Value),
                    ObjId = int.Parse(value.Attribute("ObjId").Value),
                    PathType = int.Parse(value.Attribute("PathType").Value),
                    VR = bool.Parse(value.Attribute("VR").Value),
                    Label = (string)value.Element("Label"),
                    ResNames = 
                    (
                        from resName in value.Element("ResName").Elements("value")
                        select resName.Value
                    ).ToArray()
                };

            // Write the output CSV.
            using (FileStream stream = new FileStream(@"D:\Pictures\objflowdump.txt", FileMode.Create, FileAccess.Write,
                FileShare.Read))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                foreach (ObjFlowValue value in values)
                {
                    writer.WriteLine(String.Join(";", value.MgrId, value.ObjId, value.Label,
                        String.Join("|", value.ResNames), value.AiReact, value.PathType, value.VR));
                }
            }
        }
    }
}
