using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using Bodoconsult.Snmp.mib.RFC1157;

namespace Bodoconsult.Snmp.Helpers
{
    /// <summary>
    /// This class allow you to create a MIB
    /// 
    /// Based on https://www.codeproject.com/Articles/12993/SNMP-library by Zitun 
    /// </summary>
    public class MibHelper
    {
        #region init variable + decl variable + constructor
        private readonly Mgmt _myMib;
        private readonly Hashtable _htOidName; //key=oid value = fullName; key=name value = oid; key = fullName value = oid

        public MibHelper()
        {
            _myMib = new Mgmt();
            _htOidName = new Hashtable();
        }
        #endregion

        #region public method


        #region get Or exist oid - name - fullName
        /// <summary>
        /// Try to see if an oid exists into the loaded mib
        /// </summary>
        /// <param name="oid">oid string</param>
        /// <returns>true if exist else false</returns>
        public bool ExistOid(string oid)
        {
            return GetFullName(oid) != "";
        }


        /// <summary>
        /// Try to see if a fullName exists into the loaded mib
        /// </summary>
        /// <param name="fullName">fullName of an oid</param>
        /// <returns>true if exist else false</returns>
        public bool ExistFullName(string fullName)
        {
            if (GetOid(fullName) != "") return true;
            else return false;
        }



        /// <summary>
        /// Try to find the simple name of the oid (last part of the name)
        /// </summary>
        /// <param name="oid">oid</param>
        /// <returns>the last part of the full name or emptyString if the oid does not exist</returns>
        public string GetSimpleName(string oid)
        {
            if (_htOidName.Contains(oid))
            {
                var s = ((string)_htOidName[oid]).Split('.');
                return s[s.Length - 1];
            }
            else
            {
                var s = GetFullName(oid).Split('.');
                return s[s.Length - 1];
            }
        }



        /// <summary>
        /// Look after oid from a simple name. 
        /// </summary>
        /// <param name="name">simple name (not the fullName path</param>
        /// <returns>Return the oid or return empty string if the name does not exist</returns>
        public string GetOidFromSimpleName(string name)
        {
            if (!_htOidName.Contains(name)) return (string)_htOidName[name];

            //Parcourir tout et si trouvé, balancer information
            Debug.Print("Walk through the MIB to get the information");
            Walk();

            if (!_htOidName.Contains(name)) return (string)_htOidName[name];
            else return "";
        }



        /// <summary>
        /// Return the oid of the fullname given path
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public string GetOid(string fullName)
        {
            try
            {
                if (!_htOidName.Contains(fullName)) return (string)_htOidName[fullName];
                else _htOidName.Add(fullName, _myMib.giveOID(fullName));
                return (string)_htOidName[fullName];
            }
            catch //Does not exist oid with the full name given
            {
                return "";
            }
        }


        /// <summary>
        /// Look after the full name from the oid
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public string GetFullName(string oid)
        {
            try
            {
                if (_htOidName.Contains(oid)) return (string)_htOidName[oid];
                else _htOidName.Add(oid, _myMib.getFullNameFromOID(oid));
                return (string)_htOidName[oid];
            }
            catch //Does not exist oid with the full name given
            {
                return "";
            }
        }
        #endregion


        ///// <summary>
        ///// Look if a SNMPObject has a description, if not return emptyString
        ///// </summary>
        ///// <param name="mySNMPObject"></param>
        ///// <returns></returns>
        //public string getDescription(SNMPObject mySNMPObject)
        //{
        //    return mySNMPObject.getDescription();
        //}


        /// <summary>
        /// Method used on mib.dll
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public string GetDescription(string oid)
        {
            return _myMib.getDescription(oid);
        }

        #region load mib, add oid
        /// <summary>
        /// try to add an oid with his name to the MIB. ! the oid parent must exsit
        /// </summary>
        /// <param name="oid">oid to add</param>
        /// <param name="name">name of the oid</param>
        /// <exception cref="Exception">return an exception if the oid parent does not already exist into the MIB or if the oid already exist with an different name</exception>
        public void AddOidName(string oid, string name)
        {
            var s = oid.Split('.');
            var oidParent = "";
            for (var j = 0; j < s.Length - 1; j++)
            {
                oidParent += s[j];
            }

            //If the oid parent does not exist throw an error
            if (!ExistOid(oidParent)) throw new Exception("Error the oid parent : " + oidParent + " does not exist into the MIB");
            //If the added already exist with a different name throw an error
            if (ExistOid(oid))
            {
                if (GetSimpleName(oid) != name) throw new Exception("Error : the oid already exist with an different name");
                else return;
            }

            //Sinon ajouter
            _htOidName.Add(oid, GetFullName(oidParent) + "." + name);
        }



        /// <summary>
        /// Load a mib file
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadMib(string fileName)
        {
            FileName = fileName;
            _myMib.loadFile(fileName);
        }

        /// <summary>
        /// Current MIB file
        /// </summary>
        public string FileName { get; private set; }

        ///// <summary>
        ///// Load all MIB file in a directory
        ///// </summary>
        ///// <param name="directoryName"></param>
        //public void LoadDirectory(string directoryName)
        //{
        //    var dir = new DirectoryInfo(directoryName);
        //    var list = dir.GetFiles("*.mib");
        //    foreach (var f in list)
        //    {
        //        LoadMib(f.FullName);
        //        Debug.Print("Load " + f.FullName);
        //    }
        //}

        #endregion

        /// <summary>
        /// Display to the Console the result of the mib
        /// </summary>
        public void Walk()
        {
            //Private part
            Debug.Print("Walk thru the private part of the MIB");
            GiveAllKids("private");

            //Mgmt part
            Debug.Print("Walk through the Mgmt part of the MIB");
            GiveAllKids("mgmt");
        }



        /// <summary>
        /// Write the result into a file
        /// </summary>
        /// <param name="fileName"></param>
        public void Walk(string fileName)
        {
            var sw = new StreamWriter(fileName);
            //Private part
            Debug.Print("Private part");
            GiveAllKids("private", sw);

            //Mgmt part
            Debug.Print("Mgmt part");
            GiveAllKids("mgmt", sw);

        }




        #endregion

        #region private part
        #region private method for the walk method

        /// <summary>
        /// Procédure récursive
        /// </summary>
        /// <param name="nameParent"></param>
        /// <param name="sw"></param>
        private void GiveAllKids(string nameParent, StreamWriter sw)
        {
            var result = GiveKids(nameParent);

            for (var j = 0; j < result.Length; j++)
            {
                var tempMo = _myMib.getDef(nameParent + "." + result[j]);
                var myOid = _myMib.giveOID(nameParent + "." + result[j]);
                if (tempMo != null && tempMo.help != null)
                {

                    Debug.Print(myOid + ";" + result[j] + ";" + tempMo.help);
                    sw.WriteLine(myOid + ";" + result[j] + ";" + tempMo.help);

                }
                else
                {
                    Debug.Print(myOid + ";" + result[j]);
                    sw.WriteLine(myOid + ";" + result[j]);
                }
                if (!_htOidName.Contains(result[j])) _htOidName.Add(result[j], myOid);
                GiveAllKids(nameParent + "." + result[j], sw);
            }



        }

        /// <summary>
        /// RECURSIVE PROCEDURE
        /// </summary>
        /// <param name="nameParent"></param>
        private void GiveAllKids(string nameParent)
        {
            var result = GiveKids(nameParent);

            for (var j = 0; j < result.Length; j++)
            {
                var tempMo = _myMib.getDef(nameParent + "." + result[j]);
                var myOid = _myMib.giveOID(nameParent + "." + result[j]);
                if (tempMo != null && tempMo.help != null)
                {
                    Debug.Print(myOid + " : " + result[j]);
                    Debug.Print("Description : " + tempMo.help);
                }
                else
                {
                    Debug.Print(myOid + " : " + result[j]);

                }
                if (!_htOidName.Contains(result[j])) _htOidName.Add(result[j], myOid);
                GiveAllKids(nameParent + "." + result[j]);
            }
        }

        private string[] GiveKids(string nameParent)
        {
            var oid = nameParent.Split('.');
            int j;
            var mo = _myMib.def;
            for (j = 0; j < oid.Length; j++)
            {
                var mn = mo[oid[j]];
                if (mn == null)
                    break;
                mo = mn;
                //if (j != 0)
                //    ;
            }
            var kids = mo.Kids();
            return kids;
        }
        #endregion
        #endregion

    }
}
