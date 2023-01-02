namespace Markdig.Renderers.Wpf.Extensions
{
    public class FigureRenderer : WpfObjectRenderer<Markdig.Extensions.Figures.Figure>
    {
        protected override void Write(WpfRenderer renderer, Markdig.Extensions.Figures.Figure obj)
        {
            renderer.WriteChildren(obj);
        }
    }
}
