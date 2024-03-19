using Markdig.Extensions.Mathematics;
using System;
using System.Windows.Documents;
using System.Windows.Media;
using WpfMath.Parsers;
using WpfMath.Rendering;
using XamlMath;

namespace Markdig.Renderers.Wpf.Extensions
{
    public class MathInlineRenderer : WpfObjectRenderer<MathInline>
    {
        private static TexFormulaParser formulaParser = WpfTeXFormulaParser.Instance;

        static internal TexEnvironment _texEnvironment = WpfTeXEnvironment.Create(TexStyle.Display);

        protected override void Write(WpfRenderer renderer, MathInline obj)
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


            var geo = WpfTeXFormulaExtensions.RenderToGeometry(formula, _texEnvironment, out var box, fontSize);
            var geoD = new System.Windows.Media.GeometryDrawing(Brushes.Black, null, geo);
            var di = new DrawingImage(geoD);
            var uiImage = new System.Windows.Controls.Image() { Source = di };
            uiImage.Height = geoD.Bounds.Height; // size image to match rendersize -> get a zoom of 100%
            uiImage.Margin = new System.Windows.Thickness(0, 0, 0, -box.Depth * fontSize  /* formulaRenderer.RelativeDepth */); // Move image so that baseline matches that of text
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
