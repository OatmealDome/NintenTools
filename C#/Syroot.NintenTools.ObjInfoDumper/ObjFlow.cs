using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Syroot.NintenTools.ObjInfoDumper
{
    /// <summary>
    /// Represents the contents of the /vol/content/data/objflow.byaml file.
    /// </summary>
    internal class ObjFlow
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjFlow"/> class from the given XML file.
        /// </summary>
        /// <param name="fileName">Name of the XML file representing the objflow.byaml.</param>
        internal ObjFlow(string fileName)
        {
            XDocument file = XDocument.Load(fileName);
            Values = (
                from value in file.Root.Elements("value")
                select new ObjFlowValue
                {
                    AiReact = int.Parse(value.Attribute("AiReact").Value),
                    MgrId = int.Parse(value.Attribute("MgrId").Value),
                    ObjId = int.Parse(value.Attribute("ObjId").Value),
                    PathType = int.Parse(value.Attribute("PathType").Value),
                    VR = bool.Parse(value.Attribute("VR").Value),
                    Label = (string)value.Element("Label"),
                    ResNames = (
                        from resName in value.Element("ResName").Elements("value")
                        select resName.Value
                    ).ToList()
                }).ToList();
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the list of <see cref="ObjFlowValue"/> instances in the ObjFlow file.
        /// </summary>
        internal List<ObjFlowValue> Values
        {
            get;
            private set;
        }
    }
}
