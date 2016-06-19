using System.Collections.Generic;

namespace Syroot.NintenTools.ObjInfoDumper
{
    internal class ObjFlowValue
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        internal int AiReact { get; set; }
        internal int MgrId { get; set; }
        internal int ObjId { get; set; }
        internal int PathType { get; set; }
        internal bool VR { get; set; }
        internal string Label { get; set; }
        internal List<string> ResNames { get; set; }
    }
}