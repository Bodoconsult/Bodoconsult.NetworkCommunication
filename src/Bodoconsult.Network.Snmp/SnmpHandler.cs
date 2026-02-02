using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using Bodoconsult.Inventory.Model;
using Bodoconsult.Snmp.Helpers;
using Bodoconsult.Snmp.Models;
using SnmpSharpNet;

namespace Bodoconsult.Snmp
{
    /// <summary>
    /// Loads all MIB files from MIB directory and 
    /// </summary>
    public class SnmpHandler
    {

        public delegate void StatusMessage(string message);

        public SnmpHandler(string mibDirectory)
        {

            if (!string.IsNullOrEmpty(mibDirectory))
            {
                MibHandler.MibDirectory = mibDirectory;
                MibHandler.LoadAllMibFiles();
            }


        }

        /// <summary>
        /// Show a status message
        /// </summary>
        public event StatusMessage Status;


        /// <summary>
        /// Get the data from the SNMP device via bulk request
        /// </summary>
        /// <param name="request">SNMP request data</param>
        public void GetBulk(SnmpBaseRequest request)
        {
            try
            {
                // Send a ping to IP to wake up the device
                var ping = NetworkHelper.Ping(request.IpAddresses[0]);
                if (ping.Status != IPStatus.Success)
                {
                    Thread.Sleep(500);

                    ping = NetworkHelper.Ping(request.IpAddresses[0]);

                    if (ping.Status != IPStatus.Success)
                    {
                        Thread.Sleep(500);

                        NetworkHelper.Ping(request.IpAddresses[0]);
                    }
                }

                var ipa = new IpAddress(request.IpAddresses[0]);

                var target = new UdpTarget((IPAddress)ipa) { Retry = 5, Timeout = 5000 };


                var param = new SecureAgentParameters();
                try
                {

                    if (!target.Discovery(param))
                    {
                        target.Close();
                        return;
                    }
                }
                catch (Exception e)
                {
                    GotStatus(string.Format("SnmpError: {0}", e.Message));
                    return;
                }


                // Construct a Protocol Data Unit (PDU)
                // Set the request type to GetBulk
                var pdu = new Pdu { Type = PduType.GetBulk };

                foreach (var item in request.Oids)
                {
                    pdu.VbList.Add(item);
                }

                if (pdu.VbList.Count == 0) return;

                // Add variables you wish to query
                //pdu.VbList.Add("1.3.6.1.2.1.1.1.0");
                //Oid oidVal1 = new Oid(new int[] {1, 3, 6, 1, 2, 1, 1, 2, 0});
                //pdu.VbList.Add(oidVal1);
                //Oid oidVal2 = new Oid("1.3.6.1.2.1.1.3.0");
                //pdu.VbList.Add(oidVal2);

                //Oid oidVal=new Oid("1.3.6.1.4.1.6574.1.1.0");
                //pdu.VbList.Add(oidVal);

                //oidVal = new Oid("1.3.6.1.4.1.6574.1.2.0");
                //pdu.VbList.Add(oidVal);

                //oidVal = new Oid("1.3.6.1.4.1.6574.1.3.0");
                //pdu.VbList.Add(oidVal);

                //oidVal = new Oid("1.3.6.1.4.1.6574.1.4.1.0");
                //pdu.VbList.Add(oidVal);

                //oidVal = new Oid("1.3.6.1.4.1.6574.1.4.2.0");
                //pdu.VbList.Add(oidVal);

                //oidVal = new Oid("1.3.6.1.4.1.6574.1.5.1.0");
                //pdu.VbList.Add(oidVal);

                //oidVal = new Oid("1.3.6.1.4.1.6574.1.5.2.0");
                //pdu.VbList.Add(oidVal);

                //oidVal = new Oid("1.3.6.1.4.1.6574.1.5.3.0");
                //pdu.VbList.Add(oidVal);

                // 

                // optional: make sure no authentication or privacy is configured in the 
                // SecureAgentParameters class (see discovery section above)
                param.authPriv(
                    request.Username,
                    request.AuthenticationType, PasswordHelper.DecryptPassword(request.Password),
                    request.PrivacyProtocols, PasswordHelper.DecryptPassword(request.Password));
                // Make a request. Request can throw a number of errors so wrap it in try/catch
                SnmpV3Packet result;
                try
                {
                    result = (SnmpV3Packet)target.Request(pdu, param);
                }
                catch (Exception ex)
                {
                    result = null;
                    GotStatus(string.Format("SnmpError: {0}", ex.Message));
                }
                if (result != null)
                {
                    if (result.ScopedPdu.Type == PduType.Report)
                    {
                        foreach (var v in result.ScopedPdu.VbList)
                        {
                            var h = v;


                            var result1 = new SnmpResult { Oid = v.Oid.ToString(), Value = v.Value.ToString() };
                            if (MibHandler.MibHelpers.Count>0)
                            {

                                result1.Description = MibHandler.GetDescription(result1.Oid);
                                result1.FullName = MibHandler.GetFullName(result1.Oid);

                            }
                            request.Results.Add(result1);


                            //Debug.Print("{0} -> ({1}) {2}",
                            //    v.Oid,
                            //    SnmpConstants.GetTypeName(v.Value.Type), v.Value);
                        }
                    }
                    else
                    {
                        if (result.ScopedPdu.ErrorStatus == 0)
                        {
                            foreach (var v in result.ScopedPdu.VbList)
                            {

                                //var h = v;
                                //foreach (var item in request.SnmpRequestItems.Where(x => x.Oid == h.Oid.ToString()))
                                //{
                                //    var ri = item;
                                //    ri.Value = v.Value.ToString();
                                //}

                                var result1 = new SnmpResult { Oid = v.Oid.ToString(), Value = v.Value.ToString() };
                                if (MibHandler.MibHelpers.Count>0)
                                {

                                    result1.Description = MibHandler.GetDescription(result1.Oid);
                                    result1.FullName = MibHandler.GetFullName(result1.Oid);

                                }
                                request.Results.Add(result1);


                                //Debug.Print("{0} -> ({1}) {2}",
                                //    v.Oid,
                                //    SnmpConstants.GetTypeName(v.Value.Type), v.Value);
                            }
                        }
                        else
                        {
                            var msg = String.Format("SNMPError: {0} ({1}): {2}",
                                SnmpError.ErrorMessage(result.ScopedPdu.ErrorStatus),
                                result.ScopedPdu.ErrorStatus, result.ScopedPdu.ErrorIndex);
                            GotStatus(msg);
                        }
                    }
                }
                target.Close();
            }
            catch (Exception e)
            {
                GotStatus(string.Format("SnmpError: {0}", e.Message));
            }
        }

        protected void GotStatus(string msg)
        {
            //_IsStart = false;
            var x = Status;
            if (x != null) x(msg);
        }
    }
}
