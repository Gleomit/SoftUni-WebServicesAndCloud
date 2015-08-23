using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace Battleships.GUIClient.Windows
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window, IWindow
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        public RegisterWindow(BattleshipsClient battleshipsClient)
            : this()
        {
            this.BattleshipsClient = battleshipsClient;
        }

        public BattleshipsClient BattleshipsClient { get; private set; }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var task = this.DoRegister();
        }

        private async Task DoRegister()
        {
            if (EmailField.Text.Length == 0)
            {
                MessageBox.Show("Email cannot be empty.");
            }

            else if (PasswordField.Password.Length == 0)
            {
                MessageBox.Show("Password cannot be empty.");
            }

            else if (ConfirmPasswordField.Password != PasswordField.Password)
            {
                MessageBox.Show("Passwords does not match.");
            }

            else
            {
                await BattleshipsClient.Register(EmailField.Text, PasswordField.Password, ConfirmPasswordField.Password);

                if (this.BattleshipsClient.CurrentResponse != null
                    && this.BattleshipsClient.CurrentResponse.IsSuccessStatusCode)
                {
                    MessageBox.Show("Successfully Registered, redirecting you to the main window!");

                    MainWindow mainWindow = new MainWindow();

                    mainWindow.Show();

                    this.Close();
                }
                else
                {
                    if (this.BattleshipsClient.CurrentResponse != null)
                    {
                        string data = await this.BattleshipsClient.CurrentResponse.Content.ReadAsStringAsync();

                        MessageBox.Show(JsonConvert.DeserializeObject<dynamic>(data)["Message"].ToString());
                    }
                }
            } 
        } 
    }
}