public class Cannon
{
    public PointF position;
    float angle;
    GraphicsView view;

    public float Angle => angle; // Додаємо властивість для доступу до angle

    public Cannon(GraphicsView view)
    {
        this.view = view;
        position = new PointF(50, (float)view.Height / 2);
        System.Diagnostics.Debug.WriteLine($"Cannon position: X={position.X}, Y={position.Y}, View Height={view.Height}, Width={view.Width}");
    }

    public void RotateTo(float x, float y)
    {
        angle = (float)Math.Atan2(y - position.Y, x - position.X);
    }

    public CannonBall Fire()
    {
        System.Diagnostics.Debug.WriteLine($"Firing Cannon at Angle={angle}, Position={position}, View Width={view.Width}");
        SoundService.Play("cannon_fire");
        return new CannonBall(position, angle, view.Width, view);
    }

    public void Draw(ICanvas canvas)
    {
        canvas.StrokeColor = Colors.White;
        canvas.DrawLine(position.X, position.Y,
            position.X + 50 * (float)Math.Cos(angle),
            position.Y + 50 * (float)Math.Sin(angle));
        canvas.FillColor = Colors.Gray;
        canvas.FillCircle(position, 20);
    }
}