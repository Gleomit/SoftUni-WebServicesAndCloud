using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Battleships.GUIClient
{
    public class BattleshipsClient
    {
        private class LoginData
        {
            public string Access_Token { get; set; }

            public string UserName { get; set; }
        }

        private string loginEndPoint = "http://localhost:62858/Token";
        private string registerEndPoint = "http://localhost:62858/api/account/register";
        private string createGameEndPoint = "http://localhost:62858/api/games/create";
        private string joinGameEndPoint = "http://localhost:62858/api/games/join";
        private string playEndPoint = "http://localhost:62858/api/games/play";
        private string availableGamesEndPoint = "http://localhost:62858/api/games/available";

        private string authorizationToken = "";

        private LoginData loginData;

        public BattleshipsClient()
        {
            this.IsLogged = false;
        }

        public string Username { get { return this.loginData.UserName; } }

        public bool IsLogged { get; private set; }

        public string CurrentGameId { get; set; }

        public HttpResponseMessage CurrentResponse { get; private set; } 

        public async Task Login(string username, string password)
        {
            using (var httpClient = new HttpClient())
            {
                this.CurrentResponse = null;

                var bodyData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("Username", username),
                    new KeyValuePair<string, string>("Password", password),
                    new KeyValuePair<string, string>("grant_type", "password")
                });

                var response = await httpClient.PostAsync(loginEndPoint, bodyData);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();

                    this.loginData = JsonConvert.DeserializeObject<LoginData>(data);
                    this.IsLogged = true;
                }

                this.CurrentResponse = response;
            }
        }

        public async Task Register(string email, string password, string confirmPassword)
        {
            using (var httpClient = new HttpClient())
            {
                this.CurrentResponse = null;

                var bodyData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("Email", email),
                    new KeyValuePair<string, string>("Password", password),
                    new KeyValuePair<string, string>("ConfirmPassword", confirmPassword)
                });

                var response = await httpClient.PostAsync(registerEndPoint, bodyData);

                this.CurrentResponse = response;
            }
        }

        public async Task JoinGame(string gameId)
        {
            using (var httpClient = new HttpClient())
            {
                this.CurrentResponse = null;

                httpClient.DefaultRequestHeaders.Add(
                   "Authorization", "Bearer " + this.loginData.Access_Token);

                var bodyData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("GameId", gameId)
                });

                var response = await httpClient.PostAsync(joinGameEndPoint, bodyData);

                this.CurrentResponse = response;
            }
        }

        public async Task CreateGame()
        {
            using (var httpClient = new HttpClient())
            {
                this.CurrentResponse = null;

                httpClient.DefaultRequestHeaders.Add(
                   "Authorization", "Bearer " + this.loginData.Access_Token);

                var response = await httpClient.PostAsync(createGameEndPoint, null);

                this.CurrentResponse = response;
            }
        }

        public async Task PlayTurn(string gameId, string positionX, string positionY)
        {
            using (var httpClient = new HttpClient())
            {
                this.CurrentResponse = null;

                httpClient.DefaultRequestHeaders.Add(
                   "Authorization", "Bearer " + this.loginData.Access_Token);

                var bodyData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("GameId", gameId),
                    new KeyValuePair<string, string>("PositionX", positionX),
                    new KeyValuePair<string, string>("PositionY", positionY)
                });

                var response = await httpClient.PostAsync(playEndPoint, bodyData);

                this.CurrentResponse = response;
            }
        }

        public async Task GetAvailableGames()
        {
            using (var httpClient = new HttpClient())
            {
                this.CurrentResponse = null;

                httpClient.DefaultRequestHeaders.Add(
                   "Authorization", "Bearer " + this.loginData.Access_Token);

                var response = await httpClient.GetAsync(availableGamesEndPoint);

                this.CurrentResponse = response;
            }
        }
    }
}