using System.Collections.Generic;
using System.IO;
using Bodoconsult.Snmp.Helpers;

namespace Bodoconsult.Snmp
{
    /// <summary>
    /// Loads all MIB files from MIB directory and 
    /// </summary>
    public static class MibHandler
    {

        private static string _mibPath=null;

        public static readonly List<MibHelper> MibHelpers = new List<MibHelper>();

        /// <summary>
        /// Directory path to the MIB files
        /// </summary>
        public static string MibDirectory { get; set; }

        /// <summary>
        /// Load all MIB files in the MIB directory
        /// </summary>
        public static void LoadAllMibFiles()
        {

            if (!Directory.Exists(MibDirectory))
            {
                Directory.CreateDirectory(MibDirectory);
                return;
            }

            if (_mibPath != MibDirectory)
            {
                MibHelpers.Clear();
                _mibPath = MibDirectory;
            }


            var dir = new DirectoryInfo(MibDirectory);

            foreach (var file in dir.GetFiles("*.mib"))
            {
                try
                {
                    var myMib = new MibHelper();
                    myMib.LoadMib(file.FullName);
                    MibHelpers.Add(myMib);
                }
                catch
                {
                    // ignored
                }
            }

        }

        /// <summary>
        /// Get the description of a OID from MIB file
        /// </summary>
        /// <param name="oid">OID to search</param>
        /// <returns>description of the OID</returns>
        public static string GetDescription(string oid)
        {
            foreach (var h in MibHelpers)
            {
                var result = h.GetDescription(oid);
                if (string.IsNullOrEmpty(result)) continue;
                return result.Trim();
            }

            return null;
        }


        /// <summary>
        /// Get the full name of a OID from MIB file
        /// </summary>
        /// <param name="oid">OID to search</param>
        /// <returns>full name of the OID</returns>
        public static string GetFullName(string oid)
        {
            foreach (var h in MibHelpers)
            {
                //Debug.Print(h.FileName);

                var result = h.GetFullName(oid);
                if (string.IsNullOrEmpty(result)) continue;
                return result;
            }

            return null;
        }

        /// <summary>
        /// Get the simple name of a OID from MIB file
        /// </summary>
        /// <param name="oid">OID to search</param>
        /// <returns>Simple name of the OID</returns>
        public static string GetSimpleName(string oid)
        {
            foreach (var h in MibHelpers)
            {
                var result = h.GetSimpleName(oid);
                if (string.IsNullOrEmpty(result)) continue;
                return result;
            }

            return null;
        }

    }
}
