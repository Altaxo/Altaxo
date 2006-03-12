using System;
using Scaled = System.Single;

namespace MathML.Rendering
{
  
  public interface IFormattingContext
  {
    /// <summary>
    /// Clones the instance of the formatting context.
    /// </summary>
    /// <returns>New instance of the formatting context.</returns>
    IFormattingContext Clone();
    
    /// <summary>
    /// evaluate a length using the current font size (in pixels)
    /// as the default size
    /// </summary>
    float Evaluate(Length length);
   
    /// <summary>
    /// get a font based on the font type given in the variant, 
    /// and the size given in height. If the size can not be 
    /// evaluated to a valid size, the default font size is used.
    /// the fontName override anything in the context if it is given.
    /// </summary>
    /// <param name="fontName"></param>
    /// <param name="altFontName"></param>
    /// <returns></returns>
    IFontHandle GetFont(string fontName, string altFontName);

    
    /// <summary>
    ///create a font based only on the MathVariant and MathSize 
    /// fields of the FormattingContext
    /// Currently, this only a shortcut to GetFont(context, "", ""), but
    /// internally may be optimized in the future.
    /// </summary>
    /// <returns>Font.</returns>
    IFontHandle GetFont();

    /// <summary>
    /// number of nested scripts
    /// </summary>
    int ScriptLevel { get; set; }

    /// <summary>
    /// The current font size, this is the current emHeight of the font to used
    /// for creating areas when a node is formatted.
    /// </summary>
    float Size { get; set; }
    
    /// <summary>
    /// the extent at which the node is being asked to stretch to
    /// </summary>
    BoundingBox Stretch { get; set; }

    /// <summary>
    /// The width property of the Stretch property.
    /// </summary>
    float StretchWidth { get; set; }

    /// <summary>
    /// should the current area be cached?
    /// </summary>
    bool cacheArea { get; set; }

    /// <summary>
    /// currently used for content areas. If true, an apply element should use parens, as
    /// it is inside a function or some other item that does not need parens.
    /// </summary>
    bool Parens { get; set; }

    /// <summary>
    /// true if formulas must be formated in display mode
    /// </summary>
    MathML.Display DisplayStyle { get; set; }

    /// <summary>
    /// the MathVariant from a presentation token node
    /// </summary>
    MathVariant MathVariant { get; set; }


    IFontHandle CreateFont(float emHeight, bool italic, int weight, String fontName);

    /// <summary>
    /// the centerline of the current font, this is not 
		/// the baseline, but where the 2 lines cross in an 'x'
    /// </summary>
    Scaled Axis { get; }

		/// <summary>
    /// the height of the character "x" in the current 
		/// font size
    /// </summary>
    float Ex { get; }

   Scaled DefaultLineThickness { get; }

    bool MeasureGlyph(IFontHandle font, ushort index, out BoundingBox box, out Scaled left, out Scaled right);


    float OnePixel { get; }


  }
   
}
