using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Syroot.NintenTools.ObjInfoDumper
{
    /// <summary>
    /// The main class of the program containing the application entry point.
    /// </summary>
    internal class Program
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        #region Track Names
        private static readonly string[] _baseTrackNames = new string[] {
            "Gu_Menu",
            "Gu_FirstCircuit",
            "Gu_WaterPark",
            "Gu_Cake",
            "Gu_DossunIseki",
            "Gu_MarioCircuit",
            "Gu_City",
            "Gu_HorrorHouse",
            "Gu_Expert",
            "Gu_Airport",
            "Gu_Ocean",
            "Gu_Techno",
            "Gu_SnowMountain",
            "Gu_Cloud",
            "Gu_Desert",
            "Gu_BowserCastle",
            "Gu_RainbowRoad",
            "Gwii_MooMooMeadows",
            "Gagb_MarioCircuit",
            "Gds_PukupukuBeach",
            "G64_KinopioHighway",
            "Ggc_DryDryDesert",
            "Gsfc_DonutsPlain3",
            "G64_PeachCircuit",
            "G3ds_DKJungle",
            "Gds_WarioStadium",
            "Ggc_SherbetLand",
            "G3ds_MusicPark",
            "G64_YoshiValley",
            "Gds_TickTockClock",
            "G3ds_PackunSlider",
            "Gwii_GrumbleVolcano",
            "G64_RainbowRoad"
        };
        private static readonly Tuple<string, string>[] _dlcTrackNames = new Tuple<string, string>[] {
            new Tuple<string, string>("0013", "Dgc_YoshiCircuit"),
            new Tuple<string, string>("0013", "Du_ExciteBike"),
            new Tuple<string, string>("0013", "Du_DragonRoad"),
            new Tuple<string, string>("0013", "Du_MuteCity"),
            new Tuple<string, string>("0015", "Dwii_WariosMine"),
            new Tuple<string, string>("0015", "Dsfc_RainbowRoad"),
            new Tuple<string, string>("0015", "Du_IcePark"),
            new Tuple<string, string>("0015", "Du_Hyrule"),
            new Tuple<string, string>("0017", "Dgc_BabyPark"),
            new Tuple<string, string>("0017", "Dagb_CheeseLand"),
            new Tuple<string, string>("0017", "Du_Woods"),
            new Tuple<string, string>("0017", "Du_Animal_Spring"),
            new Tuple<string, string>("0017", "Du_Animal_Summer"),
            new Tuple<string, string>("0017", "Du_Animal_Autumn"),
            new Tuple<string, string>("0017", "Du_Animal_Winter"),
            new Tuple<string, string>("0019", "D3ds_NeoBowserCity"),
            new Tuple<string, string>("0019", "Dagb_RibbonRoad"),
            new Tuple<string, string>("0019", "Du_Metro"),
            new Tuple<string, string>("0019", "Du_BigBlue")
        };
        #endregion

        // ---- MEMBERS ------------------------------------------------------------------------------------------------

        private static ObjFlow _objFlow;
        private static Muunt[] _muuntNormal;
        private static Muunt[] _muunt200;
        private static Muunt[] _muuntBattle;

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private static int Main(string[] args)
        {
            // Read the parameters.
            if (args.Length != 2)
            {
                return -1;
            }
            string objFlowPath = args[0];
            string csvPath = args[1];

            // Load the files.
            _objFlow = new ObjFlow(objFlowPath);
            LoadAllMuuntFiles(objFlowPath);

            // Write the dump.
            WriteOutputCsv(csvPath);

            return 0;
        }

        private static void LoadAllMuuntFiles(string objFlowPath)
        {
            // From the given objflow.xml path, try to find all possible track muunt files.
            int trackCount = _baseTrackNames.Length + _dlcTrackNames.Length;
            _muuntNormal = new Muunt[trackCount];
            _muunt200    = new Muunt[trackCount];
            _muuntBattle = new Muunt[trackCount];

            // Load the base track instances.
            string courseDirectory = Path.Combine(Path.GetDirectoryName(objFlowPath), "..", "course");
            int baseTrackIndex;
            for (baseTrackIndex = 0; baseTrackIndex < _baseTrackNames.Length; baseTrackIndex++)
            {
                string baseTrackName = _baseTrackNames[baseTrackIndex];
                LoadBaseTrack(baseTrackIndex, courseDirectory, baseTrackName);
            }

            // Load the DLC track instances.
            string dlcPath = GetDlcPath(objFlowPath);
            for (int dlcTrackIndex = 0; dlcTrackIndex < _dlcTrackNames.Length; dlcTrackIndex++)
            {
                Tuple<string, string> dlcTrackName = _dlcTrackNames[dlcTrackIndex];
                string dlcCourseDirectory = Path.Combine(dlcPath, dlcTrackName.Item1, "course");
                LoadDlcTrack(baseTrackIndex + dlcTrackIndex, dlcCourseDirectory, courseDirectory, dlcTrackName.Item2);
            }
        }

        private static string GetDlcPath(string objFlowPath)
        {
            string volPath = Path.Combine(Path.GetDirectoryName(objFlowPath), "..", "..");

            string path = Path.Combine(volPath, "aoc0005000c1010eb00");
            if (Directory.Exists(path))
            {
                // The JAP DLC folder exists.
                return path;
            }
            path = Path.Combine(volPath, "aoc0005000c1010ec00");
            if (Directory.Exists(path))
            {
                // The USA DLC folder exists.
                return path;
            }
            path = Path.Combine(volPath, "aoc0005000c1010ed00");
            if (Directory.Exists(path))
            {
                // The EUR DLC folder exists.
                return path;
            }

            // No DLC folder found.
            return null;
        }

        private static void LoadBaseTrack(int index, string courseDirectory, string name)
        {
            string trackDirectory = Path.Combine(courseDirectory, name);
            Console.WriteLine("Loading {0}...", trackDirectory);

            // Load the normal Muunt file.
            string courseMuuntPath = Path.Combine(trackDirectory, "course_muunt.xml");
            if (File.Exists(courseMuuntPath))
            {
                _muuntNormal[index] = new Muunt(courseMuuntPath);
            }

            // Load the 200cc Muunt file.
            string cc200MuuntPath = Path.Combine(trackDirectory, "course_muunt_200.xml");
            if (File.Exists(cc200MuuntPath))
            {
                _muunt200[index] = new Muunt(cc200MuuntPath);
            }

            // Load the battle Muunt file.
            string battleMuuntPath = Path.Combine(trackDirectory, "battle_muunt.xml");
            if (File.Exists(battleMuuntPath))
            {
                _muuntBattle[index] = new Muunt(battleMuuntPath);
            }
        }

        private static void LoadDlcTrack(int index, string dlcCourseDirectory, string courseDirectory, string name)
        {
            // Load the normal Muunt file.
            string dlcTrackDirectory = Path.Combine(dlcCourseDirectory, name);
            Console.WriteLine("Loading {0}...", dlcTrackDirectory);
            string courseMuuntPath = Path.Combine(dlcTrackDirectory, "course_muunt.xml");
            if (File.Exists(courseMuuntPath))
            {
                _muuntNormal[index] = new Muunt(courseMuuntPath);
            }

            // Load the 200cc Muunt file.
            string trackDirectory = Path.Combine(courseDirectory, name);
            string cc200MuuntPath = Path.Combine(trackDirectory, "course_muunt_200.xml");
            if (File.Exists(cc200MuuntPath))
            {
                _muunt200[index] = new Muunt(cc200MuuntPath);
            }
        }

        private static void WriteOutputCsv(string csvPath)
        {
            // Write the output CSV.
            using (FileStream stream = new FileStream(csvPath, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                // Go through each track object.
                foreach (ObjFlowValue value in _objFlow.Values)
                {
                    // Get generic object information.
                    writer.Write(String.Join(";", value.MgrId, value.ObjId, value.Label, 
                        String.Join("|", value.ResNames), value.AiReact, value.PathType, value.VR));
                    writer.Write(";");
                    // Get track usage counters (querying only normal or 200cc files at the moment).
                    List<int> trackUsageCounts = new List<int>(_muuntNormal.Length);
                    for (int i = 0; i < _muuntNormal.Length; i++)
                    {
                        Muunt muunt = _muunt200[i];
                        if (muunt == null)
                        {
                            muunt = _muuntNormal[i];
                        }
                        trackUsageCounts.Add(GetMuuntObjectCount(muunt, value.ObjId));
                    }
                    // Get the names of the tracks using this object the most.
                    List<string> maxUseTrackNames = new List<string>();
                    int maxUseCount = trackUsageCounts.Max();
                    if (maxUseCount > 0)
                    {
                        for (int i = 0; i < trackUsageCounts.Count; i++)
                        {
                            if (trackUsageCounts[i] == maxUseCount)
                            {
                                maxUseTrackNames.Add(GetTrackName(i));
                            }
                        }
                    }
                    writer.Write(String.Join("|", maxUseTrackNames));
                    writer.Write(";");
                    writer.WriteLine(String.Join(";", trackUsageCounts));
                }
            }
        }

        private static int GetMuuntObjectCount(Muunt muunt, int objId)
        {
            if (muunt == null)
            {
                return -1;
            }
            return muunt.Values.Count(obj => obj.ObjId == objId);
        }

        private static string GetTrackName(int index)
        {
            if (index < _baseTrackNames.Length)
            {
                return _baseTrackNames[index];
            }
            else
            {
                return _dlcTrackNames[index - _baseTrackNames.Length].Item2;
            }
        }
    }
}
