using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace Battleships.GUIClient.Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window, IWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        public LoginWindow(BattleshipsClient battleshipsClient)
            : this()
        {
            this.BattleshipsClient = battleshipsClient;
        }

        public BattleshipsClient BattleshipsClient { get; private set; }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var task = this.DoLogin();
        }

        private async Task DoLogin()
        {
            if (EmailField.Text.Length == 0)
            {
                MessageBox.Show("Email cannot be empty.");
            }

            else if (PasswordField.Password.Length == 0)
            {
                MessageBox.Show("Password cannot be empty.");
            }
            else
            {
                await BattleshipsClient.Login(EmailField.Text.Trim(), PasswordField.Password.Trim());

                if (this.BattleshipsClient.CurrentResponse != null
                    && this.BattleshipsClient.CurrentResponse.IsSuccessStatusCode)
                {
                    MessageBox.Show("Successfully Logged!");

                    GameLobbyWindow gameLobbyWindow = new GameLobbyWindow(this.BattleshipsClient);

                    gameLobbyWindow.Show();

                    this.Close();
                }
                else
                {
                    if (this.BattleshipsClient.CurrentResponse != null)
                    {
                        string data = await this.BattleshipsClient.CurrentResponse.Content.ReadAsStringAsync();

                        MessageBox.Show(JsonConvert.DeserializeObject<dynamic>(data)["error_description"].ToString());
                    }
                }
            }
        } 
    }
}