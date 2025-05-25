public class Target
{
    public RectF Bounds;
    public float Speed;
    public Color Color;
    GraphicsView view;

    public Target(GraphicsView view, int index)
    {
        this.view = view;
        float width = 20;
        float height = 40;
        float startX = 150 + index * (width + 5); // Зміщено до 150 для видимості
        float startY = (float)(view.Height / 2 - height / 2);
        Speed = (float)((index % 2 == 0 ? 1 : -1) * (50 + index * 10));
        Color = index % 2 == 0 ? Colors.Yellow : Colors.Blue;
        Bounds = new RectF(startX, startY, width, height);
        System.Diagnostics.Debug.WriteLine($"Target {index} position: X={startX}, Y={startY}, View Height={view.Height}, Width={view.Width}");
    }

    public void Update()
    {
        Bounds = Bounds with { Y = Bounds.Y + Speed * 0.016f };
        if (Bounds.Top < 0 || Bounds.Bottom > view.Height)
            Speed = -Speed;
    }

    public void Draw(ICanvas canvas)
    {
        canvas.FillColor = Color;
        canvas.FillRectangle(Bounds);
    }

    public bool Collides(CannonBall ball) => Bounds.IntersectsWith(ball.Bounds);
}