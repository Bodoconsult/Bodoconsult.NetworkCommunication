using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Bodoconsult.Inventory.Handler;
using Bodoconsult.Inventory.Model;
using Bodoconsult.Web.Mail;

namespace BodoInventory;

internal class Program
{

    private const string MailBody =
        @"<h1 style=""font-family:Arial;font-size:12pt;"">Hardware warnings</h1><p style=""font-family:Arial;font-size:10pt;"">{0}</p>";


    [STAThread]
    public static void Main(string[] args)
    {

        //try
        //{

        //Console.WriteLine("Hallo");

        if (args.Any())
        {
            var value = args[0];

            if (value != "1") return;

            var s = Bodoconsult.Console.PasswordHandler.ReadPassword();

            Clipboard.SetText(s);

            Environment.Exit(0);

        }

        var settings = new GeneralSettings
        {
            DiskLimitAbsolut = Convert.ToDouble(Properties.Settings.Default.DiskLimitAbsolute),
            DiskLimitRelative = Convert.ToDouble(Properties.Settings.Default.DiskLimitRelative),
            HtmlTargetDir = Properties.Settings.Default.HtmlTargetDir,
            Username = Properties.Settings.Default.DomainAdminUsername,
            Password = Properties.Settings.Default.DomainAdminPassword,
            StatusMessage = Status,
        };

        var ih = new HtmlDocumentationHandler(settings);
        ih.GetNetworkData();
        ih.CreateWebsite();

        Environment.Exit(0);


        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine(ex.Message);
        //}

    }

    public static void Status(string modul, string message)
    {
        Debug.Print("{0}: {1}", modul, message);
        Console.WriteLine("{0}: {1}", modul, message);
    }

        




    internal static void SendMail(string msg)
    {

        try
        {
            var server = Properties.Settings.Default.SmtpServer;

            var account = Properties.Settings.Default.SmtpUser;

            var from = Properties.Settings.Default.SmtpFrom;

            var mailTo = Properties.Settings.Default.SmtpTo;

            var pwd = Bodoconsult.Console.PasswordHandler.Decrypt(Properties.Settings.Default.SmtpPassword);

            var body = string.Format(MailBody, msg);

            var smtp = new SmtpMailer { From = from, SmtpAccount = account, SmtpPassword = pwd, SmtpServer = server };
            smtp.Init();
            smtp.SendMail(mailTo, "BodoInventory: hardware warnings", body);
        }
        catch (Exception ex)
        {
            //var newEx = new Exception("SendMail", ex);
            Console.WriteLine(ex.Message);

        }

    }

}