using System;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace Battleships.GUIClient.Windows
{
    /// <summary>
    /// Interaction logic for JoinedGameWindow.xaml
    /// </summary>
    public partial class JoinedGameWindow : Window, IWindow
    {
        private string gameId;

        public JoinedGameWindow()
        {
            InitializeComponent();
        }

        public JoinedGameWindow(BattleshipsClient battleshipsClient)
            : this()
        {
            this.BattleshipsClient = battleshipsClient;

            var task = this.GetInfo();
        }

        public BattleshipsClient BattleshipsClient { get; private set; }

        private async Task MakeAMove()
        {
            if (XPositionField.Text == String.Empty || YPositionField.Text == string.Empty)
            {
                MessageBox.Show("Fields cannot be empty.");
            }
            else
            {
                await this.BattleshipsClient.PlayTurn(this.gameId, XPositionField.Text, YPositionField.Text);

                if (this.BattleshipsClient.CurrentResponse != null)
                {
                    if (this.BattleshipsClient.CurrentResponse.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Successfull move.");

                        XPositionField.Text = string.Empty;
                        YPositionField.Text = String.Empty;
                    }
                    else
                    {
                        string data = await this.BattleshipsClient.CurrentResponse.Content.ReadAsStringAsync();

                        MessageBox.Show(JsonConvert.DeserializeObject<dynamic>(data)["Message"].ToString());
                    }
                }
            }
        }

        private async Task GetInfo()
        {
            if (this.BattleshipsClient.CurrentResponse != null)
            {
                if (this.BattleshipsClient.CurrentResponse.IsSuccessStatusCode)
                {
                    string data = await this.BattleshipsClient.CurrentResponse.Content.ReadAsStringAsync();

                    this.gameId = JsonConvert.DeserializeObject<string>(data);

                    this.GameTitleLabel.Content = "Game Id: " + this.gameId;
                }
                else
                {
                    this.gameId = this.BattleshipsClient.CurrentGameId;

                    this.GameTitleLabel.Content = "Created by you. Game Id: " + this.gameId;
                }
            }
        }

        private void GameLobbyButton_Click(object sender, RoutedEventArgs e)
        {
            GameLobbyWindow gameLobbyWindow = new GameLobbyWindow(this.BattleshipsClient);

            gameLobbyWindow.Show();

            this.Close();
        }

        private void TurnButton_Click(object sender, RoutedEventArgs e)
        {
            var task = this.MakeAMove();
        }
    }
}