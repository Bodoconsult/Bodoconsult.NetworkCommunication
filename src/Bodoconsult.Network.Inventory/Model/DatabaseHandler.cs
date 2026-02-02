//using System;
//using System.Data.OleDb;
//using Bodoconsult.Database;
//using Bodoconsult.Inventory.Model;

//namespace Bodoconsult.Inventory
//{
//    public class DatabaseHandler
//    {
//        private AdapterConnManager _db;

//        public string DatabasePath { get; set; }  

//        public void Open(string databasePath)
//        {
//            DatabasePath = databasePath;

//            var conn = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};User Id=admin;Password=;", databasePath);

//            _db = ConnManagerFactory.GetConnManager(conn, "System.Data.OleDb");            
//        }


//        public void Open()
//        {
//            var conn = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", DatabasePath);
//            _db = ConnManagerFactory.GetConnManager(conn, "System.Data.OleDb");
//        }


//        /// <summary>
//        /// Vorbereitungen in Datenbank
//        /// </summary>
//        public void Start()
//        {
//            var sql = "UPDATE NetworkItems SET [Exists]=false WHERE [Manual]=false";
//            _db.Exec(sql, false);

//            sql = "UPDATE Drives SET [Exists]=false";
//            _db.Exec(sql, false);

//            sql = "UPDATE LogicalDrives SET [Exists]=false";
//            _db.Exec(sql, false);

//            sql = "UPDATE NetworkAdapters SET [Exists]=false";
//            _db.Exec(sql, false);

//            sql = "UPDATE Software SET [Exists]=false";
//            _db.Exec(sql, false);
//        }



//        public void RegisterIp(string ipAddress)
//        {
//            var sql = String.Format(@"SELECT ID from NetworkItems WHERE Ip=""{0}""", ipAddress);

//            var erg = _db.ExecWithResult(sql);

//            if (String.IsNullOrEmpty(erg))
//            {

//                sql = String.Format(@"INSERT INTO NetworkItems(Ip, [Exists], [Manual]) SELECT ""{0}"", true, false", ipAddress);
//                _db.Exec(sql, false);

//            }
//            else
//            {
//                sql = String.Format(@"UPDATE NetworkItems SET [Exists]=true WHERE Ip=""{0}""", ipAddress);
//                _db.Exec(sql, false);
//            }

//        }


//        public void RegisterDrive(int id, string name)
//        {
//            var sql = String.Format(@"SELECT ID from Drives WHERE NetworkItemId={0} AND DriveName=""{1}""", id, name);

//            var erg = _db.ExecWithResult(sql);

//            if (String.IsNullOrEmpty(erg))
//            {
//                sql = String.Format(@"INSERT INTO Drives(NetworkItemId, DriveName, [Exists]) SELECT {0}, ""{1}"", true", id, name);
//                _db.Exec(sql, false);
//            }
//            else
//            {
//                sql = String.Format(@"UPDATE Drives SET [Exists]=true WHERE NetworkItemId={0} AND DriveName=""{1}""" , id, name);
//                _db.Exec(sql, false);
//            }
//        }


//        /// <summary>
//        /// Daten bereinigen
//        /// </summary>
//        public void ClearData()
//        {

//            var sql = "DELETE * FROM NetworkItems WHERE [Manual]=false AND [Exists]=false";
//            _db.Exec(sql, false);

//            sql = "DELETE * FROM Drives WHERE [Exists]=false";
//            _db.Exec(sql, false);

//            sql = "DELETE * FROM LogicalDrives WHERE [Exists]=false";
//            _db.Exec(sql, false);

//            sql = "DELETE * FROM NetworkAdapters WHERE [Exists]=false";
//            _db.Exec(sql, false);

//            sql = "DELETE * FROM Software WHERE [Exists]=false";
//            _db.Exec(sql, false);

//        }


//        public void UpdateNetworkItem(NetworkItem item)
//        {
//            var sql = String.Format(@"SELECT ID from NetworkItems WHERE Ip=""{0}""", item.Ip);
//            var id = Convert.ToInt32(_db.ExecWithResult(sql));

//            sql = "UPDATE NetworkItems SET OperatingSystem=?, Ram=?, HostName=?, DomainRole=?, NumberOfProcessors=?, VirtualMachine=?, FreeRam=? WHERE ID=?";

//            var cmd = new OleDbCommand {CommandText = sql};

//            var para = new OleDbParameter {ParameterName = "OperatingSystem", Value = item.OperatingSystem};
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "Ram", Value = item.Ram };
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "HostName", Value = item.HostName };
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "DomainRole", Value = item.DomainRole };
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "NumberOfProcessors", Value = item.NumberOfProcessors };
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "VirtualMachine", Value = item.VirtualMachine };
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "FreeRam", Value = item.FreeRam };
//            cmd.Parameters.Add(para);
            
//            para = new OleDbParameter { ParameterName = "Id", Value = id };
//            cmd.Parameters.Add(para);

//            _db.Exec(cmd);



//            item.Remark = _db.ExecWithResult(String.Format("SELECT [Remark] FROM NetworkItems WHERE Id={0}", id));

//            item.RoleDescription = _db.ExecWithResult(String.Format("SELECT [RoleDescription] FROM NetworkItems WHERE Id={0}", id));

//            item.HandlingInstructions = _db.ExecWithResult(String.Format("SELECT [HandlingInstructions] FROM NetworkItems WHERE Id={0}", id));


            
//            UpdateDrives(id, item);

//            UpdateLogicalDrives(id, item);

//            UpdateNetworkAdapters(id, item);

//            UpdateSoftware(id, item);

//        }

//        private void UpdateLogicalDrives(int id, NetworkItem item)
//        {
//            foreach (var drive in item.LogicalDrives)
//            {
//                RegisterLogicalDrive(id, drive.Name);

//                UpdateLogicalDrive(id, drive);
//            }
//        }

//        private void UpdateLogicalDrive(int id, LogicalDriveItem item)
//        {
//            var sql = String.Format(@"SELECT ID from LogicalDrives WHERE NetworkItemId={0} and DriveName=""{1}""", id, item.Name);
//            var did = Convert.ToInt32(_db.ExecWithResult(sql));

//            sql = "UPDATE LogicalDrives SET [Size]=?, [FreeSpace]=?, [Type]=?, [FileSystem]=? WHERE ID=?";
//            var cmd = new OleDbCommand { CommandText = sql };

//            var para = new OleDbParameter { ParameterName = "Size", Value = item.Size };
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "FreeSpace", Value = item.FreeSpace };
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "Type", Value = item.Type };
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "FileSystem", Value = item.FileSystem };
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "Id", Value = did };
//            cmd.Parameters.Add(para);

//            _db.Exec(cmd);

//            item.Remark = _db.ExecWithResult(String.Format("SELECT [Remark] FROM LogicalDrives WHERE Id={0}", did));
//        }

//        private void RegisterLogicalDrive(int id, string name)
//        {
//            var sql = String.Format(@"SELECT ID from LogicalDrives WHERE NetworkItemId={0} AND DriveName=""{1}""", id, name);

//            var erg = _db.ExecWithResult(sql);

//            if (String.IsNullOrEmpty(erg))
//            {

//                sql = String.Format(@"INSERT INTO LogicalDrives(NetworkItemId, DriveName, [Exists]) SELECT {0}, ""{1}"", true", id, name);
//                _db.Exec(sql, false);

//            }
//            else
//            {
//                sql = String.Format(@"UPDATE LogicalDrives SET [Exists]=true WHERE NetworkItemId={0} AND DriveName=""{1}""", id, name);
//                _db.Exec(sql, false);
//            } 
//        }


//        private void UpdateDrives(int id, NetworkItem item)
//        {        
//            foreach (var drive in item.Drives)
//            {
//                RegisterDrive(id, drive.Name);

//                UpdateDrive(id, drive);
//            }
//        }


//        private void UpdateDrive(int id, DriveItem item)
//        {

//            var sql = String.Format(@"SELECT ID from Drives WHERE NetworkItemId={0} and DriveName=""{1}""", id, item.Name);
//            var did = Convert.ToInt32(_db.ExecWithResult(sql));

//            sql = "UPDATE Drives SET [DriveId]=?, [Size]=?, [SizeUsed]=? WHERE ID=?";
//            var cmd = new OleDbCommand { CommandText = sql };

//            var para = new OleDbParameter { ParameterName = "DriveId", Value = item.DriveId };
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "Size", Value = item.Size };
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "SizeUsed", Value = item.SizeUsed };
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "Id", Value = did };
//            cmd.Parameters.Add(para);

//            _db.Exec(cmd);

//            item.Remark = _db.ExecWithResult(String.Format("SELECT [Remark] FROM Drives WHERE Id={0}", did));
//        }


//        private void UpdateNetworkAdapters(int id, NetworkItem item)
//        {
//            foreach (var drive in item.NetworkAdapters)
//            {
//                RegisterUpdateNetworkAdapter(id, drive.Name);

//                UpdateNetworkAdapter(id, drive);
//            }
//        }

//        private void UpdateNetworkAdapter(int id, NetworkAdapterItem item)
//        {
//            var sql = String.Format(@"SELECT ID from NetworkAdapters WHERE NetworkItemId={0} and AdapterName=""{1}""", id, item.Name);
//            var did = Convert.ToInt32(_db.ExecWithResult(sql));

//            sql = "UPDATE NetworkAdapters SET [AdapterName]=?, [AdapterId]=?, [Speed]=?, [MacAddress]=?, [IpAddress]=?,[DnsServer]=?,[DhcpEnabled]=? ,[DefaultIpGateway]=?  WHERE ID=?";
//            var cmd = new OleDbCommand { CommandText = sql };

//            var para = new OleDbParameter { ParameterName = "Name", Value = item.Name};
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "AdapterId", Value = item.Id };
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "Speed", Value = item.Speed };
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "MacAddress", Value = item.MacAddress };
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "IpAddress", Value = item.IpAddress ?? "" };
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "DnsServer", Value = item.DnsServer ?? "" };
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "DhcpEnabled", Value = item.DhcpEnabled};
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "DefaultIpGateway", Value = item.DefaultIpGateway ?? "" };
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "Id", Value = did };
//            cmd.Parameters.Add(para);


//            _db.Exec(cmd);

//            item.Remark = _db.ExecWithResult(String.Format("SELECT [Remark] FROM NetworkAdapters WHERE Id={0}", did));
//        }



//        private void RegisterUpdateNetworkAdapter(int id, string name)
//        {
//            var sql = String.Format(@"SELECT ID from NetworkAdapters WHERE NetworkItemId={0} AND AdapterName=""{1}""", id, name);

//            var erg = _db.ExecWithResult(sql);

//            if (String.IsNullOrEmpty(erg))
//            {

//                sql = String.Format(@"INSERT INTO NetworkAdapters(NetworkItemId, AdapterName, [Exists]) SELECT {0}, ""{1}"", true", id, name);
//                _db.Exec(sql, false);

//            }
//            else
//            {
//                sql = String.Format(@"UPDATE NetworkAdapters SET [Exists]=true WHERE NetworkItemId={0} AND AdapterName=""{1}""", id, name);
//                _db.Exec(sql, false);
//            } 
//        }

//        private void UpdateSoftware(int id, NetworkItem item)
//        {
//            foreach (var drive in item.Software)
//            {
//                RegisterUpdateSoftware(id, drive.Name);

//                 UpdateSoftware(id, drive);
//            }
//        }

//        private void UpdateSoftware(int id, SoftwareItem item)
//        {
//            var sql = String.Format(@"SELECT ID from Software WHERE NetworkItemId={0} and SoftwareName=""{1}""", id, item.Name);
//            var did = Convert.ToInt32(_db.ExecWithResult(sql));

//            sql = "UPDATE Software SET [IdentifyingNumber]=?, [Vendor]=?, [Version]=? WHERE ID=?";
//            var cmd = new OleDbCommand { CommandText = sql };

//            var para = new OleDbParameter { ParameterName = "IdentifyingNumber", Value = item.IdentifyingNumber };
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "Vendor", Value = item.Vendor };
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "Version", Value = item.Version };
//            cmd.Parameters.Add(para);

//            para = new OleDbParameter { ParameterName = "Id", Value = did };
//            cmd.Parameters.Add(para);

//            _db.Exec(cmd);


//            item.Remark = _db.ExecWithResult(String.Format("SELECT [Remark] FROM Software WHERE Id={0}", did));

//        }

//        private void RegisterUpdateSoftware(int id, string name)
//        {
//            var sql = String.Format(@"SELECT ID from Software WHERE NetworkItemId={0} AND SoftwareName=""{1}""", id, name);

//            var erg = _db.ExecWithResult(sql);

//            if (String.IsNullOrEmpty(erg))
//            {
//                sql = String.Format(@"INSERT INTO Software(NetworkItemId, SoftwareName, [Exists]) SELECT {0}, ""{1}"", true", id, name);
//                _db.Exec(sql, false);
//            }
//            else
//            {
//                sql = String.Format(@"UPDATE Software SET [Exists]=true WHERE NetworkItemId={0} AND SoftwareName=""{1}""", id, name);
//                _db.Exec(sql, false);
//            } 
//        }
//    }
//}
