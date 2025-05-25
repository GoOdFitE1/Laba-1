using Microsoft.Maui.Graphics;

public class CannonBall
{
    public PointF Position;
    public float Radius = 10;
    public PointF Velocity;
    private readonly GraphicsView view; // Додаємо поле

    public CannonBall(PointF startPos, float angle, double screenWidth, GraphicsView view)
    {
        this.view = view;
        Position = startPos;
        float speed = (float)(screenWidth * 5.1); // Зменшена швидкість
        Velocity = new PointF(speed * (float)Math.Cos(angle), speed * (float)Math.Sin(angle));
        System.Diagnostics.Debug.WriteLine($"CannonBall created at X={Position.X}, Y={Position.Y}, Speed={speed}, Angle={angle}");
    }

    public void Update()
    {
        Position = new PointF(Position.X + Velocity.X * 0.016f, Position.Y + Velocity.Y * 0.016f);
    }

    public void Reverse()
    {
        Velocity = new PointF(-Velocity.X, Velocity.Y);
    }

    public bool IsOutOfBounds()
    {
        System.Diagnostics.Debug.WriteLine($"IsOutOfBounds: X={Position.X}, Y={Position.Y}, View Width={view.Width}, View Height={view.Height}");
        return Position.X > view.Width || Position.Y < 0 || Position.Y > view.Height;
    }

    public void Draw(ICanvas canvas)
    {
        canvas.FillColor = Colors.Red;
        canvas.FillCircle(Position, Radius);
    }

    public RectF Bounds => new(Position.X - Radius, Position.Y - Radius, Radius * 2, Radius * 2);
}