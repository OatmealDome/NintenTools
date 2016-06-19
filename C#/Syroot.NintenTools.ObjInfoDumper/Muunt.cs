using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Syroot.NintenTools.ObjInfoDumper
{
    /// <summary>
    /// Represents either a course_muunt.byaml, battle_muunt.byaml or course_muunt_200.byaml file.
    /// </summary>
    internal class Muunt
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="Muunt"/> class from the given XML file.
        /// </summary>
        /// <param name="fileName">Name of the XML file representing the *_muunt.byaml.</param>
        internal Muunt(string fileName)
        {
            XDocument file = XDocument.Load(fileName);
            Values = (
                from value in file.Root.Element("Obj").Elements("value")
                select new MuuntObj
                {
                    ObjId = int.Parse(value.Attribute("ObjId").Value)
                }).ToList();
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the list of <see cref="MuuntObj"/> instances in the Muunt file.
        /// </summary>
        internal List<MuuntObj> Values
        {
            get;
            private set;
        }
    }
}
