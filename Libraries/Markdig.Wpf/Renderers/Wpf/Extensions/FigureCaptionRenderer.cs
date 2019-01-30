using Markdig.Annotations;
using Markdig.Extensions.Mathematics;
using Markdig.Renderers;
using Markdig.Renderers.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using WpfMath;

namespace Markdig.Renderers.Wpf.Extensions
{
    public class FigureCaptionRenderer : WpfObjectRenderer<Markdig.Extensions.Figures.FigureCaption>
    {
        protected override void Write([NotNull] WpfRenderer renderer, [NotNull] Markdig.Extensions.Figures.FigureCaption obj)
        {
            Paragraph paragraph = new Paragraph() { Tag = obj };
            renderer.Push(paragraph);
            renderer.WriteLeafInline(obj);
            renderer.Pop();
        }
    }
}
