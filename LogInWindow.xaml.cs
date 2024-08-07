using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.IO;

namespace scheduler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        public bool loginFail;
        public string loginError = "The username and password did not match. Please try again.";
        public LogInWindow()
        {
            InitializeComponent();

            Console.WriteLine(CultureInfo.CurrentUICulture.LCID);
            if(CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "es")
            {
                infoPromptLabel.Content = "Introduzca su nombre de usuario y contraseña";
                usernameLabel.Content = "nombre de usuario";
                passwordLabel.Content = "contraseña";
                loginButton.Content = "Iniciar sesión";
                loginError = "El nombre de usuario y la contraseña no coinciden.";
            }
        }

        public static bool UserLogin(string username, string password)
        {
            string uName = username;
            mySQLDB mySQLDB = new mySQLDB();
            int loginSuccess = mySQLDB.LoginDBUser(username, password);
            if (loginSuccess > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordBox.Password;
            DateTime now = DateTime.UtcNow;
            bool attempt = UserLogin(username, password);
            if (attempt)
            {
                Console.WriteLine("Login Success");
                UserLoginAttempts(username, now, attempt);

                new HomePage().Show();
                this.Close();

            }
            else
            {
                Console.WriteLine("Login Fail");
                UserLoginAttempts(username, now, attempt);
                loginFail = true;
                usernameTextBox.Text = String.Empty;
                passwordBox.Password = String.Empty;
                System.Windows.Forms.MessageBox.Show(loginError, "scheduler - Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        public static void UserLoginAttempts(string username, DateTime timeStamp, bool success)
        {
            string uName = username;
            DateTime tStamp = timeStamp;
            bool bSuccess = success;
            StreamWriter accessLog;
            string logEntry = "Username: " + uName + " -- Timestamp: " + tStamp.ToString() + " UTC" + " -- Login Success?: " + bSuccess;

            if (!File.Exists("AccessControlLog.txt"))
            {
                accessLog = new StreamWriter("AccessControlLog.txt");
            }
            else
            {
                accessLog = File.AppendText("AccessControlLog.txt");
            }

            accessLog.WriteLine(logEntry);
            accessLog.WriteLine();
            accessLog.Close();
        }

        private void passwordBox_EnterKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(sender, e);
            }
        }
    }
}
