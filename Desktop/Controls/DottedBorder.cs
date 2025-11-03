using Microsoft.Maui.Graphics;

namespace Desktop.Controls;

public class DottedBorderDrawable : IDrawable
{
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.StrokeColor = Colors.Gray;
        canvas.StrokeSize = 2;
        canvas.StrokeDashPattern = new float[] { 5, 5 };
        canvas.DrawRoundedRectangle(dirtyRect.Inflate(-2, -2), 8);
    }
}

public class DottedBorder : GraphicsView
{
    public DottedBorder()
    {
        Drawable = new DottedBorderDrawable();
    }
}