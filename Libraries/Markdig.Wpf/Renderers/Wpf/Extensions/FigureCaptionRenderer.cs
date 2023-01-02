using System.Windows.Documents;

namespace Markdig.Renderers.Wpf.Extensions
{
    public class FigureCaptionRenderer : WpfObjectRenderer<Markdig.Extensions.Figures.FigureCaption>
    {
        protected override void Write(WpfRenderer renderer, Markdig.Extensions.Figures.FigureCaption obj)
        {
            Paragraph paragraph = new Paragraph() { Tag = obj };
            renderer.Push(paragraph);
            renderer.WriteLeafInline(obj);
            renderer.Pop();
        }
    }
}
