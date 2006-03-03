using System;
namespace MathML.Rendering
{
  
  public interface IGraphicDevice
  {
    System.Drawing.Color Color { get; set; }
    void DrawFilledRectangle(float top, float left, float right, float bottom);
    void DrawGlyph(ushort index, float x, float y);
    void DrawLine(System.Drawing.PointF from, System.Drawing.PointF to);
    void DrawLines(System.Drawing.PointF[] points);
    void DrawString(float x, float y, string s);
    MathML.LineStyle LineStyle { get; set; }
    void RestoreFont(IFontHandle font);
    IFontHandle SetFont(IFontHandle font);
  }
  
}
