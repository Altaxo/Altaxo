using Markdig.Extensions.Mathematics;
using System;
using System.Windows.Documents;
using System.Windows.Media;
using WpfMath.Parsers;
using WpfMath.Rendering;
using XamlMath;

namespace Markdig.Renderers.Wpf.Extensions
{
    public class MathBlockRenderer : WpfObjectRenderer<MathBlock>
    {
        private static TexFormulaParser formulaParser = WpfTeXFormulaParser.Instance;

        static internal TexEnvironment _texEnvironment = WpfTeXEnvironment.Create(TexStyle.Display);

        protected override void Write(WpfRenderer renderer, MathBlock obj)
        {
            string text = string.Empty; // obj.Content.Text.Substring(obj.Content.Start, obj.Content.Length);

            for (int i = 0; i < obj.Lines.Count; ++i)
            {
                var l = obj.Lines.Lines[i];
                text += l.Slice.Text.Substring(l.Slice.Start, l.Slice.Length);
            }

            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            TexFormula formula = null;
            try
            {
                formula = formulaParser.Parse(text);
            }
            catch (Exception)
            {
                var paragraph = new Paragraph() { Tag = obj };
                renderer.Push(paragraph);
                renderer.WriteInline(new Run("[!!FORMULA PARSE ERROR!!]") { Tag = obj });
                renderer.Pop();
                return;
            }

            var fontSize = renderer.CurrentFontSize();
            if (fontSize <= 0)
            {
                throw new InvalidProgramException();
            }

            var geo = WpfTeXFormulaExtensions.RenderToGeometry(formula, _texEnvironment, fontSize);

            //var formulaRenderer = formula.GetRenderer(TexStyle.Display, fontSize, "Arial");
            //var geo = formulaRenderer.RenderToGeometry(0, 0);
            var geoD = new System.Windows.Media.GeometryDrawing(Brushes.Black, null, geo);
            var di = new DrawingImage(geoD);
            var uiImage = new System.Windows.Controls.Image() { Source = di };
            uiImage.Height = geoD.Bounds.Height; // size image to match rendersize -> get a zoom of 100%
            // uiImage.Margin = new System.Windows.Thickness(0, 0, 0, -formulaRenderer.RenderSize.Height * formulaRenderer.RelativeDepth); // Move image so that baseline matches that of text
            var uiBlock = new System.Windows.Documents.BlockUIContainer()
            {
                Child = uiImage,
                Tag = obj,
            };
            renderer.WriteBlock(uiBlock);
        }
    }
}
