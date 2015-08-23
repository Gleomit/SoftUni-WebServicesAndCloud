using System.Windows;
using Battleships.GUIClient.Windows;

namespace Battleships.GUIClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IWindow
    {

        public MainWindow()
        {
            InitializeComponent();
            this.BattleshipsClient = new BattleshipsClient();
        }

        public BattleshipsClient BattleshipsClient { get; private set; }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow(this.BattleshipsClient);

            loginWindow.Show();

            this.Close();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow(this.BattleshipsClient);

            registerWindow.Show();

            this.Close();
        }
    }
}
