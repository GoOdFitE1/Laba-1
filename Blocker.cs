public class Blocker
{
    public RectF Bounds;
    public float Speed;
    GraphicsView view;

    public Blocker(GraphicsView view)
    {
        this.view = view;
        float width = 20;
        float height = 80;
        float startX = 100; // Встановлюємо фіксоване значення для тестування
        float startY = (float)(view.Height / 2 - height / 2);
        Speed = 100;
        Bounds = new RectF(startX, startY, width, height);
        System.Diagnostics.Debug.WriteLine($"Blocker position: X={startX}, Y={startY}, View Height={view.Height}, Width={view.Width}");
    }

    public void Update()
    {
        Bounds = Bounds with { Y = Bounds.Y + Speed * 0.016f };
        if (Bounds.Top < 0 || Bounds.Bottom > view.Height)
            Speed = -Speed;
    }

    public void Draw(ICanvas canvas)
    {
        canvas.FillColor = Colors.Black;
        canvas.FillRectangle(Bounds);
    }

    public bool Collides(CannonBall ball) => Bounds.IntersectsWith(ball.Bounds);
}