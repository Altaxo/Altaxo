#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Windows.Data;
using System.Windows.Documents;
using Markdig;
using Markdig.Wpf;

namespace Altaxo.Gui.Common.Converters
{
  /// <summary>
  /// Converts a Markdown text to a <see cref="FlowDocument"/>.
  /// Formulas are supported. Images are only
  /// supported if coming from the local resources.
  /// </summary>
  [ValueConversion(typeof(string), typeof(FlowDocument))]
  public class MarkdownToFlowDocumentConverter : IValueConverter
  {
    private static MarkdownPipeline DefaultPipeline => new MarkdownPipelineBuilder().UseSupportedExtensions().Build();
    private MarkdownPipeline? Pipeline { get; set; }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var result = value is string text ?
        RenderDocument(text, System.Globalization.CultureInfo.InvariantCulture) :
        Binding.DoNothing;
      return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }


    /// <summary>
    /// Renders the document.
    /// </summary>
    /// only those parts that were changed in the source text are rendered anew.
    /// Note that setting this parameter to <c>true</c> does not force a new rendering of the images; for that, call <see cref="IWpfImageProvider.ClearCache"/> of the <see cref="ImageProvider"/> member before rendering.</param>
    private FlowDocument RenderDocument(string sourceText, System.Globalization.CultureInfo documentCulture)
    {
      var pipeline = Pipeline ?? DefaultPipeline;

      var markdownDocument = Markdig.Markdown.Parse(sourceText, pipeline);

      // We override the renderer with our own writer
      var flowDocument = new FlowDocument
      {
        IsHyphenationEnabled = true,
        Language = System.Windows.Markup.XmlLanguage.GetLanguage(documentCulture.IetfLanguageTag)
      };

      var renderer = new Markdig.Renderers.WpfRenderer(flowDocument, DynamicStyles.Instance)
      {
        ImageProvider = new Altaxo.Gui.Analysis.NonLinearFitting.ResourceOnlyImageProvider()
      };

      pipeline.Setup(renderer);
      renderer.Render(markdownDocument);
      return flowDocument;
    }
  }
}
