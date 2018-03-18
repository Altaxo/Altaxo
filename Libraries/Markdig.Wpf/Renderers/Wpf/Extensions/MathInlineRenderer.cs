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
    public class MathInlineRenderer : WpfObjectRenderer<MathInline>
    {
        private static TexFormulaParser formulaParser = new TexFormulaParser();
        private static Pen pen = new Pen(Brushes.Black, 1);

        protected override void Write([NotNull] WpfRenderer renderer, [NotNull] MathInline obj)
        {
            var text = obj.Content.Text.Substring(obj.Content.Start, obj.Content.Length);

            TexFormula formula = null;
            try
            {
                formula = formulaParser.Parse(text);
            }
            catch (Exception)
            {
                renderer.WriteInline(new Run("[!!FORMULA PARSE ERROR!!]") { Tag = obj });
                return;
            }

            var fontSize = renderer.CurrentFontSize();
            var formulaRenderer = formula.GetRenderer(TexStyle.Display, fontSize, "Arial");
            var geo = formulaRenderer.RenderToGeometry(0, 0);
            var geoD = new System.Windows.Media.GeometryDrawing(Brushes.Black, null, geo);
            var di = new DrawingImage(geoD);
            var uiImage = new System.Windows.Controls.Image() { Source = di };
            uiImage.Height = formulaRenderer.RenderSize.Height; // size image to match rendersize -> get a zoom of 100%
            uiImage.Margin = new System.Windows.Thickness(0, 0, 0, -formulaRenderer.RenderSize.Height * formulaRenderer.RelativeDepth); // Move image so that baseline matches that of text
            var uiInline = new System.Windows.Documents.InlineUIContainer()
            {
                Child = uiImage,
                Background = Brushes.Yellow,
                BaselineAlignment = System.Windows.BaselineAlignment.Baseline,
                Tag = obj,
            };
            renderer.WriteInline(uiInline);
        }
    }
}
