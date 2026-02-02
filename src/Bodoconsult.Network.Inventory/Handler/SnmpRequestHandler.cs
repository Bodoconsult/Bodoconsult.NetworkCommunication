//using System;
//using System.Diagnostics;
//using System.Linq;
//using System.Net.Cache;
//using Bodoconsult.Inventory.Helper;
//using Bodoconsult.Inventory.Model;
//using Bodoconsult.Snmp;

//namespace Bodoconsult.Inventory.Handler;

///// <summary>
///// Handles snmp requests
///// </summary>
//public class SnmpRequestHandler
//{
//    private readonly SnmpHandler _snmp;

//    public SnmpRequestHandler()
//    {
//        _snmp = new SnmpHandler(FileHelper.MibPath);
//        _snmp.Status += GotStatus;

//        if (SnmpProfileHandler.Profiles.Count == 0) SnmpProfileHandler.LoadProfiles(FileHelper.ProfileDir);
//    }


//    public event SnmpHandler.StatusMessage Status;

//    /// <summary>
//    /// Run the SNMP request
//    /// </summary>
//    /// <param name="request"></param>
//    public void RunRequest(SnmpRequest request)
//    {
//        // Add company base OID
//        if (!string.IsNullOrEmpty(request.CompanyBaseOid))
//        {
//            request.Oids.Add(request.CompanyBaseOid);
//        }

//        // Add other OID
//        foreach (var item in request.SnmpRequestItems.Where(x=> !string.IsNullOrEmpty(x.Oid) && string.IsNullOrEmpty(x.Value)))
//        {
//            request.Oids.Add(item.Oid);
//        }

//        // Request data from device
//        _snmp.GetBulk(request);

//        // Fill the request items with the collected data
//        foreach (var item in request.SnmpRequestItems.Where(x => !string.IsNullOrEmpty(x.Oid) && string.IsNullOrEmpty(x.Value)))
//        {
//            var data = request.Results.FirstOrDefault(x => x.Oid == item.Oid);
//            if (data == null) continue;
//            item.Value = data.Value;
//            item.Description = data.Description;


//        }

//        // Check data with a SNMP profile
//        if (!string.IsNullOrEmpty(request.ProfileName))
//        {
//            foreach (var result in request.Results)
//            {
//                var w = SnmpProfileHandler.CheckValue(request.ProfileName, result.Oid,
//                    Convert.ToInt32(result.Value));
//                if (w == null) continue;

//                w.Message = "Network item IP " + request.IpAddresses[0] + ": " + w.Message;
//                request.Warnings.Add(w);
//            }
//        }

//    }

//    protected void GotStatus(string msg)
//    {
//        Debug.Print(msg);
//        //_IsStart = false;
//        var x = Status;
//        if (x != null) x(msg);
//    }
//}