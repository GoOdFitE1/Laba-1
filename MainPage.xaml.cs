using Microsoft.Maui.Controls;
using Lab2;

namespace Lab2
{
    public partial class MainPage : ContentPage
    {
        public GameCanvas GameCanvas { get; private set; }

        public MainPage()
        {
            InitializeComponent();
            GameCanvas = new GameCanvas(GameView); // GameView — x:Name у XAML
            BindingContext = this;
            Loaded += async (_, _) => await SoundService.LoadSounds(); // Завантажуємо звуки при старті
        }

        private void OnStartGameClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"OnStartGameClicked: GameView Size: Width={GameView.Width}, Height={GameView.Height}");
            if (GameCanvas != null && GameView.Width > 0 && GameView.Height > 0)
            {
                GameCanvas.ResetGame();
                GameCanvas.StartGame();
            }
        }

        private void OnTap(object sender, TappedEventArgs e)
        {
            var point = e.GetPosition(GameView);
            if (point.HasValue)
            {
                System.Diagnostics.Debug.WriteLine($"Tap detected at X={point.Value.X}, Y={point.Value.Y}, GameView Size: Width={GameView.Width}, Height={GameView.Height}");
                if (GameCanvas != null)
                {
                    GameCanvas.OnTouch((float)point.Value.X, (float)point.Value.Y);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Tap event failed to get position or GameCanvas is null.");
            }
        }

        private void OnGameViewSizeChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"GameView Size Changed: Width={GameView.Width}, Height={GameView.Height}");
            if (GameCanvas != null && GameView.Width > 0 && GameView.Height > 0 && !GameCanvas.isInitialized)
            {
                GameCanvas.StartGame();
            }
        }
    }
}