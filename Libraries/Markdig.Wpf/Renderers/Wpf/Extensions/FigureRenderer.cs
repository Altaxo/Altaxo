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
    public class FigureRenderer : WpfObjectRenderer<Markdig.Extensions.Figures.Figure>
    {
        protected override void Write([NotNull] WpfRenderer renderer, [NotNull] Markdig.Extensions.Figures.Figure obj)
        {
            renderer.WriteChildren(obj);
        }
    }
}
