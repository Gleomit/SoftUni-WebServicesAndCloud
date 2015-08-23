using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace Battleships.GUIClient.Windows
{
    /// <summary>
    /// Interaction logic for GameLobbyWindow.xaml
    /// </summary>
    public partial class GameLobbyWindow : Window, IWindow
    {
        private class GameData
        {
            public string Id { get; set; }
            
            public string PlayerOne { get; set; }
        }

        private List<GameData> availableGames; 

        public GameLobbyWindow()
        {
            InitializeComponent();

            this.availableGames = new List<GameData>();
        }

        public GameLobbyWindow(BattleshipsClient battleshipsClient)
            : this()
        {
            this.BattleshipsClient = battleshipsClient;

            this.WelcomeLabel.Content = "Welcome " + this.BattleshipsClient.Username;

            var task = LoadAvailableGames();
        }

        public BattleshipsClient BattleshipsClient { get; private set; }

        private async Task LoadAvailableGames()
        {
            await this.BattleshipsClient.GetAvailableGames();

            if (this.BattleshipsClient.CurrentResponse != null)
            {
                if (this.BattleshipsClient.CurrentResponse.IsSuccessStatusCode)
                {
                    string data = await this.BattleshipsClient.CurrentResponse.Content.ReadAsStringAsync();

                    this.availableGames = JsonConvert.DeserializeObject<List<GameData>>(data);

                    this.AvailableGamesControl.Items.Clear();

                    foreach (var game in this.availableGames)
                    {
                        this.AvailableGamesControl.Items.Add(game);
                    }                  
                }
                else
                {
                    string data = await this.BattleshipsClient.CurrentResponse.Content.ReadAsStringAsync();

                    MessageBox.Show(JsonConvert.DeserializeObject<dynamic>(data)["Message"].ToString());
                }
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();

            mainWindow.Show();

            this.Close();
        }

        private void CreateGameButton_Click(object sender, RoutedEventArgs e)
        {
            var task = this.DoCreateGame();
        }

        private async Task DoCreateGame()
        {
            await this.BattleshipsClient.CreateGame();

            if (this.BattleshipsClient.CurrentResponse != null)
            {
                if (this.BattleshipsClient.CurrentResponse.IsSuccessStatusCode)
                {
                    MessageBox.Show("Successfully created game.");

                    string data = await this.BattleshipsClient.CurrentResponse.Content.ReadAsStringAsync();

                    this.BattleshipsClient.CurrentGameId = JsonConvert.DeserializeObject<string>(data).ToString();

                    JoinedGameWindow joinedGameWindow = new JoinedGameWindow(this.BattleshipsClient);

                    joinedGameWindow.Show();

                    this.Close();
                }
                else
                {
                    string data = await this.BattleshipsClient.CurrentResponse.Content.ReadAsStringAsync();

                    MessageBox.Show(JsonConvert.DeserializeObject<dynamic>(data)["Message"].ToString());
                }
            }
        }

        private async Task DoJoinGame()
        {
            if (this.JoinGameField.Text == string.Empty)
            {
                MessageBox.Show("Field is empty.");
            }
            else
            {
                await this.BattleshipsClient.JoinGame(this.JoinGameField.Text);

                if (this.BattleshipsClient.CurrentResponse != null)
                {
                    this.BattleshipsClient.CurrentGameId = this.JoinGameField.Text;

                    if (this.BattleshipsClient.CurrentResponse.IsSuccessStatusCode)
                    {
                        JoinedGameWindow joinedGameWindow = new JoinedGameWindow(this.BattleshipsClient);

                        joinedGameWindow.Show();

                        this.Close();
                    }
                    else
                    {
                        string data = await this.BattleshipsClient.CurrentResponse.Content.ReadAsStringAsync();

                        string message = JsonConvert.DeserializeObject<dynamic>(data)["Message"].ToString();

                        if (message == "You can not join in your game!")
                        {
                            MessageBox.Show("Attention! You are not joining the game, you are entering in it, because it's game created by you.");

                            JoinedGameWindow joinedGameWindow = new JoinedGameWindow(this.BattleshipsClient);

                            joinedGameWindow.Show();

                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show(message);
                        } 
                    }
                }
            }
        }

        private void JoinGameButton_Click(object sender, RoutedEventArgs e)
        {
            var task = DoJoinGame();
        }

        private void RefreshGamesButton_Click(object sender, RoutedEventArgs e)
        {
            var task = LoadAvailableGames();
        }

        private bool CheckGameId(string gameId)
        {
            foreach (var game in this.availableGames)
            {
                if (game.Id == gameId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}