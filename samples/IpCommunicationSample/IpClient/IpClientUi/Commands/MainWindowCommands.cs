//// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

//using System.Windows.Documents;
//using System.Windows.Input;
//using IpClient.Helpers;

//namespace IpClient.Commands
//{
//    public class MainWindowCommands
//    {

//        /// <summary>
//        /// Print Command
//        /// </summary>
//        private static ICommand _showFlowDocumentCommand;

//        public static ICommand ShowFlowDocumentCommand
//        {
//            get
//            {
//                return _showFlowDocumentCommand ??
//                       (_showFlowDocumentCommand = new DelegateCommand(ShowFlowDocument, () => true));
//            }
//        }


//        private static void ShowFlowDocument()
//        {
//            // View injection

//            //var regionManager = ApplicationHelper.GetRegionManager();

//            //var region = regionManager.Regions[];

//            //var container = ApplicationHelper.GetContainer();

//            //var ordersView = container.Resolve<>();
//            //region.Add(ordersView, "FlowDocument");
//            //region.Activate(ordersView);


//            NavigateGoBack<FlowDocumentReaderControl, FlowDocumentViewerControlLoadEvent, FlowDocument> test = ApplicationHelper.Navigate<FlowDocumentReaderControl,  FlowDocument>;

//            var fds = new FlowDocumentService();

//            fds.AddSection();
//            fds.AddHeader1(FlowDocHelper.Header1);
//            fds.AddRuler();
//            fds.AddHeader2(FlowDocHelper.Header2);
//            fds.AddParagraph(FlowDocHelper.MassText);
//            fds.AddParagraph(FlowDocHelper.MassText);
//            fds.AddImage(@"Resources/testimage.png");
//            fds.AddHeader2(FlowDocHelper.Header2);
//            fds.AddParagraph(FlowDocHelper.MassText);
//            fds.AddParagraph(FlowDocHelper.MassText);
//            fds.AddParagraph(FlowDocHelper.MassText);

//            fds.AddHeader2(FlowDocHelper.Header2);
//            fds.AddParagraph(FlowDocHelper.MassText);
//            fds.AddParagraph(FlowDocHelper.MassText);

//            fds.AddHeader1(FlowDocHelper.Header1);
//            fds.AddRuler();
//            fds.AddParagraph(FlowDocHelper.MassText);
//            fds.AddFigure(@"Resources/testimage.png", "Quantum ipsum lorem delete");
//            fds.AddParagraph(FlowDocHelper.MassText);
//            fds.AddParagraph(FlowDocHelper.MassText);

//            test(fds.Document);

//            AvaloniaDocumentUtility.SaveDocumentAsXps(fds.Document, @"c:\temp\SaveDocumentAsXps.xps");
//        }


//        /// <summary>
//        /// Print Command
//        /// </summary>
//        private static ICommand _changePasswordCommand;

//        public static ICommand ChangePasswordCommand
//        {
//            get
//            {
//                return _changePasswordCommand ??
//                       (_changePasswordCommand = new DelegateCommand(ChangePassword, () => true));
//            }
//        }

//        private static void ChangePassword()
//        {
//            var data = new ChangePasswordData();
//            //{
//            //    //CancelButtonLabel = "Abbrechen",
//            //    ChangePasswordButtonLabel = "Paßwort ändern",
//            //    PasswordLabel = "Altes Paßwort:",
//            //    NewPasswordRepeatLabel = "Neues Paßwort wiederholen:",
//            //    NewPasswordRepeatTooltip = "Bitte neues Paßwort eingeben",
//            //    PasswordTooltip = "Bitte Paßwort eingeben.",
//            //    TitleLabel = "Paßwort ändern",
//            //    NewPasswordLabel = "Neues Paßwort:",
//            //    NewPasswordTooltip = "Bitte neues Paßwort eingeben"
//            //};

//            data.CheckChangePasswordData += ChangePassword;

//            var viewModel = new ChangePasswordViewModel { ChangePasswordData = data };

//            var form = new ChangePasswordWindow(viewModel);
//            form.ShowDialog();

//            var erg = form.DialogResult;

//            if (erg == false)
//            {
//                // Any reaction if password change was not succesful
//                form.Close();
//            }
//        }


//        /// <summary>
//        /// Print Command
//        /// </summary>
//        private static ICommand _loginCommand;

//        public static ICommand LoginCommand
//        {
//            get
//            {
//                return _loginCommand ??
//                       (_loginCommand = new DelegateCommand(Login, () => true));
//            }
//        }

        


//        private static void Login()
//        {
//            var data = new LoginData();
//            data.CheckLoginData += CheckLoginData;
//            //data.UserName = "robert";
//            //data.CancelButtonLabel = "Abbrechen";
//            //data.LoginButtonLabel = "Anmelden";
//            //data.PasswordLabel = "Paßwort:";
//            //data.UserNameLabel = "Benutzername: ";
//            //data.UserNameTooltip = "Bitte Benutzernamen eingeben.";
//            //data.PasswordTooltip = "Bitte Paßwort eingeben.";
//            //data.TitleLabel = "Anmelden an XY-App";

//            var viewModel = new LoginWindowViewModel { LoginData = data };

//            var form = new LoginWindow(viewModel) { Owner = null };
//            form.ShowDialog();

//            if (form.DialogResult == false)
//            {
//                form.Close();
//            }
//            form.Close();
//        }


//        private static ICommand _testControlsWithDefaultSkin;

//        public static ICommand TestControlsWithDefaultSkin
//        {
//            get
//            {
//                return _testControlsWithDefaultSkin ??
//                       (_testControlsWithDefaultSkin = new DelegateCommand(DefaultSkinTest, () => true));
//            }
//        }

//        private static void DefaultSkinTest()
//        {
//            ApplicationHelper.Navigate<DefaultSkinTest>();
//        }


//        /// <summary>
//        /// Method checking the provided login data
//        /// </summary>
//        /// <param name="userName">User name</param>
//        /// <param name="password">Password</param>
//        /// <returns>Returns true if login was successfully, otherwise false</returns>
//        private static bool CheckLoginData(string userName, string password)
//        {
//            return userName == "robert" && password == "test";
//        }

//        /// <summary>
//        /// Method to change password
//        /// </summary>
//        /// <param name="passwordOld">old password</param>
//        /// <param name="passwordNew">new password</param>
//        /// <returns>returns true if password was changed successfully, otherwise false</returns>
//        private static bool ChangePassword(string passwordOld, string passwordNew)
//        {
//            return passwordOld != passwordNew;
//        }

        

//        private static ICommand _changeLanguageCommand;

//        /// <summary>
//        /// Command for ChangeLanguage
//        /// </summary>	
//        public static ICommand ChangeLanguageCommand
//        {
//            get
//            {
//                return _changeLanguageCommand ??
//                       (_changeLanguageCommand = new DelegateCommand(ChangeLanguage, () => true));
//            }
//        }

//        /// <summary>
//        /// Method for ChangeLanguage
//        /// </summary>
//        public static void ChangeLanguage()
//        {
//            var language = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

//            language = language.StartsWith("de") ? "en-US" : "de-DE";

//            ApplicationHelper.ChangeLanguage(language);
//        }


//        private static ICommand _testEditorControl;

//        public static ICommand TestEditorControlCommand
//        {
//            get
//            {
//                return _testEditorControl ??
//                       (_testEditorControl = new DelegateCommand(TestingEditorControl, () => true));
//            }
//        }

//        private static void TestingEditorControl()
//        {
//            ApplicationHelper.Navigate<TestEditorControl>();
//        }




//        private static ICommand _testControlsCommand;

//        /// <summary>
//        /// Command for TestControls
//        /// </summary>	
//        public static ICommand TestControlsCommand
//        {
//            get
//            {
//                return _testControlsCommand ??
//                       (_testControlsCommand = new DelegateCommand(TestControls, () => true));
//            }
//        }

//        /// <summary>
//        /// Method for TestControls
//        /// </summary>
//        public static void TestControls()
//        {
//            ApplicationHelper.Navigate<TestControls>();
//        }


//        private static ICommand _testInputBoxCommand;

//        /// <summary>
//        /// Command for TestControls
//        /// </summary>	
//        public static ICommand TestInputBoxCommand
//        {
//            get
//            {
//                return _testInputBoxCommand ??
//                       (_testInputBoxCommand = new DelegateCommand(TestInputBox, () => true));
//            }
//        }

//        /// <summary>
//        /// Method for TestControls
//        /// </summary>
//        public static void TestInputBox()
//        {
            
//            var data = new InputBoxData();

//            new InputBox(data).ShowDialog();

//            if (string.IsNullOrEmpty(data.UserInput))
//            {
//                AvaloniaStandardDialogUtility.ShowInfo("InputBox", "Cancelled");
//            }
//            else
//            {
//                AvaloniaStandardDialogUtility.ShowInfo("InputBox", "User input"+data.UserInput);
//            }

//        }

//    }
//}
