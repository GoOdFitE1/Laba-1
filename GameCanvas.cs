using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using System.Diagnostics;
using System.Timers;
using System.Linq;
using System.Collections.Generic;

public class GameCanvas : IDrawable
{
    GraphicsView view;
    Cannon cannon;
    List<Target> targets;
    Blocker blocker;
    CannonBall? cannonBall;
    int shots = 0;
    double timeLeft = 10;
    bool gameOver = false;
    public bool isInitialized = false; // Залишаємо public для доступу

    Stopwatch stopwatch = new();
    System.Timers.Timer gameTimer;

    public GameCanvas(GraphicsView view)
    {
        this.view = view;

        // Додаємо обробник зміни розміру
        view.SizeChanged += OnViewSizeChanged;

        gameTimer = new System.Timers.Timer(200); // Залишаємо 50 мс для зменшення навантаження
        gameTimer.Elapsed += (s, e) =>
        {
            UpdateGame();
            view.Invalidate();
        };
    }

    private void OnViewSizeChanged(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"View Size Changed: Width={view.Width}, Height={view.Height}");
        if (view.Width > 0 && view.Height > 0 && !isInitialized)
        {
            StartGame();
            isInitialized = true;
        }
    }

    public void StartGame()
    {
        System.Diagnostics.Debug.WriteLine($"StartGame: View Width={view.Width}, Height={view.Height}");
        if (view.Width <= 0 || view.Height <= 0)
        {
            System.Diagnostics.Debug.WriteLine("Warning: View size is invalid, delaying initialization.");
            return;
        }

        System.Diagnostics.Debug.WriteLine("Game Started");

        timeLeft = 10;
        shots = 0;
        gameOver = false;

        // Залишаємо 3 цілі для зменшення навантаження
        cannon = new Cannon(view);
        blocker = new Blocker(view);
        targets = Enumerable.Range(0, 3)
            .Select(i => new Target(view, i))
            .ToList();

        System.Diagnostics.Debug.WriteLine($"Cannon position after init: X={cannon.position.X}, Y={cannon.position.Y}");

        stopwatch.Restart();
        gameTimer.Start();
    }

    public void ResetGame()
    {
        cannonBall = null;
        gameOver = false;
        timeLeft = 10;
        shots = 0;
        if (targets != null) targets.Clear();
        if (gameTimer != null) gameTimer.Stop();
        isInitialized = false; // Дозволяємо повторну ініціалізацію
    }

    public void OnTouch(float x, float y)
    {
        System.Diagnostics.Debug.WriteLine($"OnTouch: Cannon={cannon != null}, GameOver={gameOver}, CannonBall={cannonBall != null}, Touch at X={x}, Y={y}, View Size: Width={view.Width}, Height={view.Height}, Condition Check: {(cannonBall == null && !gameOver && view.Width > 0 && view.Height > 0)}");
        if (cannonBall == null && !gameOver && view.Width > 0 && view.Height > 0)
        {
            cannon.RotateTo(x, y);
            cannonBall = cannon.Fire();
            shots++;
            System.Diagnostics.Debug.WriteLine($"Shot fired. Shots={shots}, CannonBall Position={cannonBall?.Position}, Angle={cannon?.Angle}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"Cannot fire: CannonBall={cannonBall != null}, GameOver={gameOver}, View Size Valid={view.Width > 0 && view.Height > 0}");
        }
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (cannon == null || blocker == null || targets == null)
        {
            canvas.FontSize = 20;
            canvas.FontColor = Colors.White;
            canvas.DrawString("Натисніть 'Почати гру'", 10, 30, HorizontalAlignment.Left);
            return;
        }

        // Видаляємо малювання фону
        // canvas.FillColor = Colors.Black;
        // canvas.FillRectangle(0, 0, (float)view.Width, (float)view.Height);

        cannon.Draw(canvas);
        blocker.Draw(canvas);
        foreach (var t in targets) t.Draw(canvas);
        cannonBall?.Draw(canvas);

        canvas.FontSize = 20;
        canvas.FontColor = Colors.White;
        canvas.DrawString($"Time Left: {timeLeft:0.0}s", 10, 10, HorizontalAlignment.Left);
    }

    private int frameCounter = 0;

    void UpdateGame()
    {
        if (gameOver) return;

        double elapsed = stopwatch.Elapsed.TotalSeconds;
        stopwatch.Restart();
        timeLeft -= elapsed;

        System.Diagnostics.Debug.WriteLine($"UpdateGame: Time Left={timeLeft}, Shots={shots}, Targets={targets.Count}");

        frameCounter++;
        if (frameCounter % 2 == 0) // Оновлюємо об’єкти кожні 2 кадри
        {
            blocker.Update();
            foreach (var t in targets) t.Update();

            if (cannonBall != null)
            {
                cannonBall.Update();
                System.Diagnostics.Debug.WriteLine($"CannonBall Update: X={cannonBall.Position.X}, Y={cannonBall.Position.Y}");

                if (blocker.Collides(cannonBall))
                {
                    cannonBall.Reverse();
                    timeLeft -= 2;
                    SoundService.Play("blocker_hit");
                }

                var hitTarget = targets.FirstOrDefault(t => t.Collides(cannonBall));
                if (hitTarget != null)
                {
                    targets.Remove(hitTarget);
                    cannonBall = null;
                    timeLeft += 3;
                    SoundService.Play("target_hit");
                }

                if (cannonBall != null && cannonBall.IsOutOfBounds())
                {
                    System.Diagnostics.Debug.WriteLine($"CannonBall out of bounds: X={cannonBall.Position.X}, Y={cannonBall.Position.Y}");
                    cannonBall = null;
                }
            }
        }

        if (targets.Count == 0 || timeLeft <= 0)
        {
            gameTimer.Stop();
            gameOver = true;
            ShowResult();
        }
    }

    void ShowResult()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            string message = targets.Count == 0 ? "Ви виграли!" : "Ви програли!";
            await Application.Current.MainPage.DisplayAlert(
                "Гра завершена",
                $"{message}\nЗроблено пострілів: {shots}\nЧас: {10 - timeLeft:0.0}s",
                "ОК");
        });
    }
}