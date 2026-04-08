/***************************************************************************
    File                 : OriginObj.h
    --------------------------------------------------------------------
    Copyright            : (C) 2005-2007, 2017 Stefan Gerlach
                           (C) 2007-2008 Alex Kargovsky, Ion Vasilief
    Copyright            : (C) 2024 Dirk Lellinger (translation to C#, bug fixing)
    Email (use @ for *)  : kargovsky*yumr.phys.msu.su, ion_vasilief*yahoo.fr
    Description          : Origin internal object classes

 ***************************************************************************/

 /***************************************************************************
 *                                                                         *
 *  This program is free software; you can redistribute it and/or modify   *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation; either version 2 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  This program is distributed in the hope that it will be useful,        *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *   You should have received a copy of the GNU General Public License     *
 *   along with this program; if not, write to the Free Software           *
 *   Foundation, Inc., 51 Franklin Street, Fifth Floor,                    *
 *   Boston, MA  02110-1301  USA                                           *
 *                                                                         *
 ***************************************************************************/
 

global using ColorMapVector = System.Collections.Generic.List<(double, Altaxo.Serialization.Origin.ColorMapLevel)>;
using System;
using System.Collections.Generic;
using Altaxo.Collections;

namespace Altaxo.Serialization.Origin
{
  /// <summary>
  /// Designates the data type that was used to deserialize the data.
  /// This does not tell you how to interpret the data. For instance, double can
  /// also interpreted as DateTime, Time, etc.
  /// </summary>
  public enum DeserializedDataType
  {
    /// <summary>
    /// Value not set yet, or type is unknown.
    /// </summary>
    Unknown,

    /// <summary>Text</summary>
    Text,

    /// <summary>Text and Double</summary>
    TextAndNumber,

    /// <summary>Complex (2 x double)</summary>
    Complex,

    /// <summary>Double (8 byte)</summary>
    Double,

    /// <summary>Double (10 byte)</summary>
    Double10,

    /// <summary>Single (4 byte)</summary>
    Single,

    /// <summary>Signed int (4 byte)</summary>
    Int32,

    /// <summary>Unsigned int (4 byte)</summary>
    UInt32,

    /// <summary>Signed int (2 byte)</summary>
    Int16,

    /// <summary>Unsigned int (2 byte)</summary>
    UInt16,

    /// <summary>Signed int (1 byte)</summary>
    SByte,

    /// <summary>Unsigned int (1 byte)</summary>
    Byte,
  };



  /// <summary>Type of value contained in a column or axis.</summary>
  public enum ValueType
  {
    /// <summary>Numeric values.</summary>
    Numeric = 0,

    /// <summary>Text values.</summary>
    Text = 1,

    /// <summary>Time values.</summary>
    Time = 2,

    /// <summary>Date values.</summary>
    Date = 3,

    /// <summary>Month values.</summary>
    Month = 4,

    /// <summary>Day values.</summary>
    Day = 5,

    /// <summary>Column heading values.</summary>
    ColumnHeading = 6,

    /// <summary>Tick labels indexed by a data set.</summary>
    TickIndexedDataset = 7,

    /// <summary>Values containing both text and numeric content.</summary>
    TextNumeric = 9,

    /// <summary>Categorical values.</summary>
    Categorical = 10
  };


  /// <summary>
  /// Specifies formatting details for values (numeric, text, date, time, etc.).
  /// </summary>
  public enum ValueTypeSpecification
  {
    // Numeric Format:

    /// <summary>Decimal, e.g. 1000</summary>
    Numeric_Decimal = 0x0000,
    /// <summary>Scientific, e.g. 1E3</summary>
    Numeric_Scientific = 0x0001,
    /// <summary>Engineering, e.g. 1k</summary>
    Numeric_Engineering = 0x0002,
    /// <summary>With separators, e.g. 1,000</summary>
    Numeric_DecimalWithMarks = 0x0003,
    /// <summary>User defined</summary>
    Numeric_UserDefined = 0x0005,


    // Text format

    /// <summary>Default text formatting.</summary>
    Text_Default = 0x0100,

    // Time Format:
    // hh:mm | hh | hh:mm:ss | hh:mm:ss.zz | hh ap | hh:mm ap | mm:ss
    // mm:ss.zz | hhmm | hhmmss | hh:mm:ss.zzz

    /// <summary>Time format <c>hh:mm</c>.</summary>
    Time_HH_MM = 0x0200,

    /// <summary>Time format <c>hh</c>.</summary>
    Time_HH = 0x0201,

    /// <summary>Time format <c>hh:mm:ss</c>.</summary>
    Time_HH_MM_SS = 0x0202,

    /// <summary>Time format <c>hh:mm:ss.zz</c>.</summary>
    Time_HH_MM_SS_ZZ = 0x0203,

    /// <summary>Time format <c>hh ap</c>.</summary>
    Time_HH_AP = 0x0204,

    /// <summary>Time format <c>hh:mm ap</c>.</summary>
    Time_HH_MM_AP = 0x0205,

    /// <summary>Time format <c>mm:ss</c>.</summary>
    Time_MM_SS = 0x0206,

    /// <summary>Time format <c>mm:ss.zz</c>.</summary>
    Time_MM_SS_ZZ = 0x0207,

    /// <summary>Time format <c>hhmm</c>.</summary>
    Time_HHMM = 0x0208,

    /// <summary>Time format <c>hhmmss</c>.</summary>
    Time_HHMMSS = 0x0209,

    /// <summary>Time format <c>hh:mm:ss.zzz</c>.</summary>
    Time_HH_MM_SS_ZZZ = 0x020A,


    // Date Format:
    // dd/MM/yyyy | dd/MM/yyyy HH:mm | dd/MM/yyyy HH:mm:ss | dd.MM.yyyy | y. (year abbreviation) | MMM d
    // M/d | d | ddd | First letter of day | yyyy | yy | dd.MM.yyyy hh:mm | dd.MM.yyyy hh:mm:ss
    // yyMMdd | yyMMdd hh:mm | yyMMdd hh:mm:ss | yyMMdd hhmm | yyMMdd hhmmss | MMM
    // First letter of month | Quartal | M-d-yyyy (Custom1) | hh:mm:ss.zzzz (Custom2)

    /// <summary>Date format <c>dd/MM/yyyy</c>.</summary>
    Date_DD_MM_YYYY = 0x0300,

    /// <summary>Date format <c>MM/dd/yyyy HH:mm</c>.</summary>
    Date_MM_DD_YYYY_HH_MM = 0x0309,

    /// <summary>Date format <c>dd/MM/yyyy HH:mm:ss</c>.</summary>
    Date_DD_MM_YYYY_HH_MM_SS = 0x030A,

    /// <summary>Date format <c>dd.MM.yyyy</c>.</summary>
    Date_DDMMYYYY = 0x0380,

    /// <summary>Year abbreviation format.</summary>
    Date_Y = 0x0381,

    /// <summary>Date format <c>MMM d</c>.</summary>
    Date_MMM_D = 0x0382,

    /// <summary>Date format <c>M/d</c>.</summary>
    Date_M_D = 0x0383,

    /// <summary>Day-of-month format.</summary>
    Date_D = 0x0384,

    /// <summary>Abbreviated day-of-week format.</summary>
    Date_DDD = 0x0385,

    /// <summary>Single-letter day-of-week format.</summary>
    Date_DAY_LETTER = 0x0386,

    /// <summary>Four-digit year format.</summary>
    Date_YYYY = 0x0387,

    /// <summary>Two-digit year format.</summary>
    Date_YY = 0x0388,

    /// <summary>Date format <c>dd.MM.yyyy hh:mm</c>.</summary>
    Date_DDMMYYYY_HH_MM = 0x0389,

    /// <summary>Date format <c>dd.MM.yyyy hh:mm:ss</c>.</summary>
    Date_DDMMYYYY_HH_MM_SS = 0x038A,

    /// <summary>Date format <c>yyMMdd</c>.</summary>
    Date_YYMMDD = 0x038B,

    /// <summary>Date format <c>yyMMdd hh:mm</c>.</summary>
    Date_YYMMDD_HH_MM = 0x038C,

    /// <summary>Date format <c>yyMMdd hh:mm:ss</c>.</summary>
    Date_YYMMDD_HH_MM_SS = 0x038D,

    /// <summary>Date format <c>yyMMdd hhmm</c>.</summary>
    Date_YYMMDD_HHMM = 0x038E,

    /// <summary>Date format <c>yyMMdd hhmmss</c>.</summary>
    Date_YYMMDD_HHMMSS = 0x038F,

    /// <summary>Abbreviated month format.</summary>
    Date_MMM = 0x0390,

    /// <summary>Single-letter month format.</summary>
    Date_MONTH_LETTER = 0x0391,

    /// <summary>Quarter format.</summary>
    Date_Q = 0x0392,

    /// <summary>Date format <c>M-d-yyyy</c>.</summary>
    Date_M_D_YYYY = 0x0393,

    /// <summary>Time format <c>hh:mm:ss.zzzz</c>.</summary>
    Date_HH_MM_SS_ZZZZ = 0x0394,

    // Month Format:
    //  MMM | MMMM | First letter of month

    /// <summary>Month format <c>MMM</c>.</summary>
    Month_MMM = 0x0400,

    /// <summary>Month format <c>MMMM</c>.</summary>
    Month_MMMM = 0x0401,

    /// <summary>Single-letter month format.</summary>
    Month_LETTER = 0x0402,

    // ddd | dddd | First letter of day

    /// <summary>Day format <c>ddd</c>.</summary>
    Day_DDD = 0x0500,

    /// <summary>Day format <c>dddd</c>.</summary>
    Day_DDDD = 0x0501,

    /// <summary>Single-letter day format.</summary>
    Day_LETTER = 0x0502,
  }

  /// <summary>How to display numeric values.</summary>
  public enum NumericDisplayType
  {
    /// <summary>Use the default number of decimal digits.</summary>
    DefaultDecimalDigits = 0,

    /// <summary>Display a fixed number of decimal places.</summary>
    DecimalPlaces = 1,

    /// <summary>Display a fixed number of significant digits.</summary>
    SignificantDigits = 2
  };

  /// <summary>Attachment target for objects (frame, page, scale).</summary>
  public enum Attach
  {
    /// <summary>Attach to the frame.</summary>
    Frame = 0,

    /// <summary>Attach to the page.</summary>
    Page = 1,

    /// <summary>Attach to the scale.</summary>
    Scale = 2,

    /// <summary>Sentinel end value.</summary>
    End_
  };

  /// <summary>Type of border to draw around an object.</summary>
  public enum BorderType
  {
    /// <summary>A simple black line border.</summary>
    BlackLine = 0,

    /// <summary>A shadow border.</summary>
    Shadow = 1,

    /// <summary>A dark marble border.</summary>
    DarkMarble = 2,

    /// <summary>A white-out border.</summary>
    WhiteOut = 3,

    /// <summary>A black-out border.</summary>
    BlackOut = 4,

    /// <summary>No border.</summary>
    None = -1
  };

  /// <summary>Fill pattern for shape fill areas.</summary>
  public enum FillPattern : byte
  {
    /// <summary>No fill pattern.</summary>
    NoFill = 0,

    /// <summary>Dense backward diagonal pattern.</summary>
    BDiagDense = 1,

    /// <summary>Medium backward diagonal pattern.</summary>
    BDiagMedium = 2,

    /// <summary>Sparse backward diagonal pattern.</summary>
    BDiagSparse = 3,

    /// <summary>Dense forward diagonal pattern.</summary>
    FDiagDense = 4,

    /// <summary>Medium forward diagonal pattern.</summary>
    FDiagMedium = 5,

    /// <summary>Sparse forward diagonal pattern.</summary>
    FDiagSparse = 6,

    /// <summary>Dense diagonal cross pattern.</summary>
    DiagCrossDense = 7,

    /// <summary>Medium diagonal cross pattern.</summary>
    DiagCrossMedium = 8,

    /// <summary>Sparse diagonal cross pattern.</summary>
    DiagCrossSparse = 9,

    /// <summary>Dense horizontal pattern.</summary>
    HorizontalDense = 10,

    /// <summary>Medium horizontal pattern.</summary>
    HorizontalMedium = 11,

    /// <summary>Sparse horizontal pattern.</summary>
    HorizontalSparse = 12,

    /// <summary>Dense vertical pattern.</summary>
    VerticalDense = 13,

    /// <summary>Medium vertical pattern.</summary>
    VerticalMedium = 14,

    /// <summary>Sparse vertical pattern.</summary>
    VerticalSparse = 15,

    /// <summary>Dense cross pattern.</summary>
    CrossDense = 16,

    /// <summary>Medium cross pattern.</summary>
    CrossMedium = 17,

    /// <summary>Sparse cross pattern.</summary>
    CrossSparse = 18
  };
  /// <summary>Direction of a color gradient.</summary>
  public enum ColorGradientDirection
  {
    /// <summary>No gradient.</summary>
    NoGradient = 0,

    /// <summary>Gradient toward the top-left corner.</summary>
    TopLeft = 1,

    /// <summary>Gradient toward the left.</summary>
    Left = 2,

    /// <summary>Gradient toward the bottom-left corner.</summary>
    BottomLeft = 3,

    /// <summary>Gradient toward the top.</summary>
    Top = 4,

    /// <summary>Gradient toward the center.</summary>
    Center = 5,

    /// <summary>Gradient toward the bottom.</summary>
    Bottom = 6,

    /// <summary>Gradient toward the top-right corner.</summary>
    TopRight = 7,

    /// <summary>Gradient toward the right.</summary>
    Right = 8,

    /// <summary>Gradient toward the bottom-right corner.</summary>
    BottomRight = 9
  };

  /// <summary>Type of color specification.</summary>
  public enum ColorType : byte
  {
    /// <summary>No color specified.</summary>
    None = 0,

    /// <summary>Automatically determined color.</summary>
    Automatic = 1,

    /// <summary>Regular predefined color.</summary>
    Regular = 2,

    /// <summary>Custom color.</summary>
    Custom = 3,

    /// <summary>Incremented color.</summary>
    Increment = 4,

    /// <summary>Indexed color.</summary>
    Indexing = 5,

    /// <summary>RGB color.</summary>
    RGB = 6,

    /// <summary>Mapped color.</summary>
    Mapping = 7
  };

  /// <summary>Predefined regular colors used in Origin objects.</summary>
  public enum RegularColor : byte
  {
    /// <summary>Black.</summary>
    Black = 0,
    /// <summary>Red.</summary>
    Red = 1,
    /// <summary>Green.</summary>
    Green = 2,
    /// <summary>Blue.</summary>
    Blue = 3,
    /// <summary>Cyan.</summary>
    Cyan = 4,
    /// <summary>Magenta.</summary>
    Magenta = 5,
    /// <summary>Yellow.</summary>
    Yellow = 6,
    /// <summary>Dark yellow.</summary>
    DarkYellow = 7,
    /// <summary>Navy.</summary>
    Navy = 8,
    /// <summary>Purple.</summary>
    Purple = 9,
    /// <summary>Wine.</summary>
    Wine = 10,
    /// <summary>Olive.</summary>
    Olive = 11,
    /// <summary>Dark cyan.</summary>
    DarkCyan = 12,
    /// <summary>Royal blue.</summary>
    Royal = 13,
    /// <summary>Orange.</summary>
    Orange = 14,
    /// <summary>Violet.</summary>
    Violet = 15,
    /// <summary>Pink.</summary>
    Pink = 16,
    /// <summary>White.</summary>
    White = 17,
    /// <summary>Light gray.</summary>
    LightGray = 18,
    /// <summary>Gray.</summary>
    Gray = 19,
    /// <summary>Light yellow.</summary>
    LTYellow = 20,
    /// <summary>Light cyan.</summary>
    LTCyan = 21,
    /// <summary>Light magenta.</summary>
    LTMagenta = 22,
    /// <summary>Dark gray.</summary>
    DarkGray = 23,
    /// <summary>Special Origin 7 axis color.</summary>
    SpecialV7Axis = 0xF7
    /*, Custom = 255*/
  };

  /// <summary>Represents a color used by Origin objects. The ordering of fields is significant.</summary>
  public struct Color
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Color"/> struct.
    /// </summary>
    public Color()
    {
    }

    /// <summary>Initialize a color with a color type and a regular color index.</summary>
    /// <param name="ctype">The color representation type.</param>
    /// <param name="cregular">The regular color value.</param>
    public Color(ColorType ctype, RegularColor cregular)
    {
      ColorType = ctype;
      Regular = (byte)cregular;
    }

    /// <summary>Specifies how the color is defined.</summary>
    public ColorType ColorType;
    // do not change the order
    /// <summary>Primary color byte. Also used as custom[0].</summary>
    public byte Regular; // also: custom[0]
    /// <summary>Second color byte. Also used as custom[1].</summary>
    public byte Starting; // also: custom[1]
    /// <summary>Third color byte. Also used as custom[2].</summary>
    public byte Column; // also: custom[2]

    /// <summary>Alias for <see cref="Regular"/> when interpreting as a custom color byte 0.</summary>
    public byte Custom0 { get => Regular; set => Regular = value; }
    /// <summary>Alias for <see cref="Starting"/> when interpreting as a custom color byte 1.</summary>
    public byte Custom1 { get => Starting; set => Starting = value; }
    /// <summary>Alias for <see cref="Column"/> when interpreting as a custom color byte 2.</summary>
    public byte Custom2 { get => Column; set => Column = value; }
  };

  /// <summary>Simple rectangle defined by left, top, right and bottom coordinates.</summary>
  public struct Rect
  {
    /// <summary>The left coordinate of the rectangle.</summary>
    public short Left;

    /// <summary>The top coordinate of the rectangle.</summary>
    public short Top;

    /// <summary>The right coordinate of the rectangle.</summary>
    public short Right;

    /// <summary>The bottom coordinate of the rectangle.</summary>
    public short Bottom;

    /// <summary>
    /// Initializes a new instance of the <see cref="Rect"/> struct with the specified width and height.
    /// </summary>
    /// <param name="width">The initial right coordinate.</param>
    /// <param name="height">The initial bottom coordinate.</param>
    public Rect(short width = 0, short height = 0)
    {
      Right = width;
      Bottom = height;
    }

    /// <summary>Returns the height of the rectangle (Bottom - Top).</summary>
    /// <returns>The rectangle height.</returns>
    public int Height()
    {
      return Bottom - Top;
    }

    /// <summary>Returns the width of the rectangle (Right - Left).</summary>
    /// <returns>The rectangle width.</returns>
    public int Width()
    {
      return Right - Left;
    }

    /// <summary>Returns true if the rectangle has positive width and height.</summary>
    /// <returns><see langword="true"/> if the rectangle is valid; otherwise, <see langword="false"/>.</returns>
    public bool IsValid()
    {
      return Height() > 0 && Width() > 0;
    }
  }

  /// <summary>Properties that define a color map level.</summary>
  public class ColorMapLevel
  {
    /// <summary>Color used to fill the area of this level.</summary>
    public Color FillColor;

    /// <summary>Fill pattern index for this level.</summary>
    public byte FillPattern;

    /// <summary>Color used for the fill pattern lines.</summary>
    public Color FillPatternColor;

    /// <summary>Line width for the fill pattern.</summary>
    public double FillPatternLineWidth;

    /// <summary>Whether the outline line for the level is visible.</summary>
    public bool IsLineVisible;

    /// <summary>Color used for the outline line.</summary>
    public Color LineColor;

    /// <summary>Line style index for the outline.</summary>
    public byte LineStyle;

    /// <summary>Width of the outline line.</summary>
    public double LineWidth;

    /// <summary>Whether labels for this level are visible.</summary>
    public bool IsLabelVisible;
  };

  /// <summary>Collection of color map settings and levels.</summary>
  public class ColorMap
  {
    /// <summary>True if fill is enabled for this color map.</summary>
    public bool IsFillEnabled;
    /// <summary>List of levels for the color map.</summary>
    public ColorMapVector Levels = [];
  };

  /// <summary>State of a window.</summary>
  public enum WindowState
  {
    /// <summary>The window is in its normal state.</summary>
    Normal,

    /// <summary>The window is minimized.</summary>
    Minimized,

    /// <summary>The window is maximized.</summary>
    Maximized
  };
  /// <summary>Which title is shown for a window.</summary>
  public enum WindowTitle
  {
    /// <summary>Show the name only.</summary>
    Name,

    /// <summary>Show the label only.</summary>
    Label,

    /// <summary>Show both name and label.</summary>
    Both
  };

  /// <summary>Represents a top-level window with common metadata and appearance settings.</summary>
  public class Window
  {
    /// <summary>Name of the window.</summary>
    public string Name;

    /// <summary>Label associated with the window.</summary>
    public string Label;

    /// <summary>Identifier for the object in the original file/project.</summary>
    public int ObjectID;

    /// <summary>Whether the window is hidden.</summary>
    public bool IsHidden;

    /// <summary>Current state of the window (normal, minimized, maximized).</summary>
    public WindowState State;

    /// <summary>Which title is displayed for the window.</summary>
    public WindowTitle Title;

    /// <summary>Rectangle that defines the window frame.</summary>
    public Rect FrameRect;

    /// <summary>Creation timestamp of the window.</summary>
    public DateTimeOffset CreationDate;

    /// <summary>Last modification timestamp of the window.</summary>
    public DateTimeOffset ModificationDate;

    /// <summary>Direction of the background color gradient for the window.</summary>
    public ColorGradientDirection WindowBackgroundColorGradient;

    /// <summary>Base color for the window background.</summary>
    public Color WindowBackgroundColorBase;

    /// <summary>End color for the window background gradient.</summary>
    public Color WindowBackgroundColorEnd;

    /// <summary>Initializes a new instance of <see cref="Window"/>.</summary>
    /// <param name="name">Optional window name.</param>
    /// <param name="label">Optional window label.</param>
    /// <param name="hidden">If <see langword="true"/>, the window is hidden.</param>
    public Window(string name = "", string label = "", bool hidden = false)
    {
      Name = name;

      Label = label;
      ObjectID = -1;
      IsHidden = hidden;
      State = WindowState.Normal;
      Title = WindowTitle.Both;
      CreationDate = DateTimeOffset.MinValue;
      ModificationDate = DateTimeOffset.MinValue;
      WindowBackgroundColorGradient = ColorGradientDirection.NoGradient;
      WindowBackgroundColorBase = new Color(ColorType.Regular, RegularColor.White);
      WindowBackgroundColorEnd = new Color(ColorType.Regular, RegularColor.White);
    }
  }

  /// <summary>
  /// Variant that can hold either a <see cref="Double"/> value or a <see cref="String"/>
  /// </summary>
  public struct Variant
  {
    private VType _type = VType.V_DOUBLE;
    private double _double;
    private string? _string;

    /// <summary>
    /// Identifies the value type currently stored in a <see cref="Variant"/>.
    /// </summary>
    public enum VType
    {
      /// <summary>No value is stored.</summary>
      V_Empty,

      /// <summary>A <see cref="double"/> value is stored.</summary>
      V_DOUBLE,

      /// <summary>A <see cref="string"/> value is stored.</summary>
      V_STRING
    };

    /// <summary>Returns the currently stored variant type.</summary>
    public VType ValueType()
    {
      return _type;
    }

    /// <summary>
    /// Initializes a new empty <see cref="Variant"/> value.
    /// </summary>
    public Variant()
    {
    }
    /// <summary>Creates a variant containing a double value.</summary>
    /// <param name="d">The double value to store in the variant.</param>
    public Variant(double d)
    {
      if (d == -1.23456789E-300)
        d = double.NaN;

      _double = d;
      _type = VType.V_DOUBLE;
    }
    /// <summary>Creates a variant containing a string value.</summary>
    /// <param name="s">The string value to store in the variant.</param>
    public Variant(string s)
    {
      _string = s;
      _double = double.NaN;
      _type = VType.V_STRING;
    }

    /// <summary>Creates a variant from an object (string or double).</summary>
    /// <exception cref="Exception">Thrown when the object type is unsupported.</exception>
    public Variant(object o)
    {
      if (o is string s)
      {
        _type = VType.V_STRING;
        _string = s;
        _double = double.NaN;
      }
      else if (o is double d)
      {
        if (d == -1.23456789E-300)
          d = double.NaN;

        _type = VType.V_DOUBLE;
        _double = d;
      }
      else
      {
        throw new Exception($"Can not create a variant with object {o}");
      }
    }

    /// <summary>True when the variant is empty.</summary>
    public readonly bool IsEmpty => _type == VType.V_Empty;

    /// <summary>True when the variant contains a double value.</summary>
    public readonly bool IsDouble => _type == VType.V_DOUBLE;
    /// <summary>True when the variant contains a double or has no string value.</summary>
    public readonly bool IsDoubleOrNaN => _type == VType.V_DOUBLE || string.IsNullOrEmpty(_string);

    /// <summary>Return the contained value as double or NaN when empty.</summary>
    /// <returns>The double value or NaN.</returns>
    public readonly double AsDouble()
    {
      if (_type == VType.V_DOUBLE)
      {
        return _double;
      }
      else if (_type == VType.V_Empty || string.IsNullOrEmpty(_string))
      {
        return double.NaN;
      }
      else
      {
        throw new ApplicationException($"Variant contains {_type}, but expecting type Double");
      }
    }

    /// <summary>Implicit conversion to double for variants.</summary>
    public static implicit operator double(Variant f)
    {
      if (f._type == VType.V_DOUBLE)
      {
        return f._double;
      }
      else if (f._type == VType.V_Empty)
      {
        return double.NaN;
      }
      else
      {
        throw new ApplicationException($"Variant contains {f._type}, but expecting type Double");
      }
    }

    /// <summary>Return the contained value as a DateTime relative to 1970-01-01 UTC or null.</summary>
    public readonly DateTime? AsDateTime()
    {
      var refDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
      if (_type == VType.V_DOUBLE)
      {
        return refDate.AddDays(_double - 2440587);
      }
      else if (_type == VType.V_Empty || string.IsNullOrEmpty(_string))
      {
        return null;
      }
      else
      {
        throw new ApplicationException($"Variant contains {_type}, but expecting type Double");
      }
    }

    /// <summary>Return the contained value as a TimeSpan or null.</summary>
    public readonly TimeSpan? AsTimeSpan()
    {
      if (_type == VType.V_DOUBLE)
      {
        return TimeSpan.FromDays(_double);
      }
      else if (_type == VType.V_Empty || string.IsNullOrEmpty(_string))
      {
        return null;
      }
      else
      {
        throw new ApplicationException($"Variant contains {_type}, but expecting type Double");
      }
    }


    /// <summary>True when the variant contains a string.</summary>
    public readonly bool IsString => _type == VType.V_STRING;

    /// <summary>Return the contained value as string or empty string when empty.</summary>
    public readonly string AsString()
    {
      if (_type == VType.V_STRING)
      {
        return _string ?? string.Empty;
      }
      else if (_type == VType.V_Empty)
      {
        return string.Empty;
      }
      else
      {
        throw new ApplicationException($"Variant contains {_type}, but expecting type String");
      }
    }

    /// <summary>Implicit conversion to string for variants.</summary>
    public static implicit operator string(Variant f)
    {
      if (f._type == VType.V_STRING)
      {
        return f._string ?? string.Empty;
      }
      else if (f._type == VType.V_Empty)
      {
        return string.Empty;
      }
      else
      {
        throw new ApplicationException($"Variant contains {f._type}, but expecting type String");
      }
    }
  }

  /// <summary>Types of columns used in spreadsheets.</summary>
  public enum SpreadColumnType
  {
    /// <summary>X data column.</summary>
    X,

    /// <summary>Y data column.</summary>
    Y,

    /// <summary>Z data column.</summary>
    Z,

    /// <summary>X error column.</summary>
    XErr,

    /// <summary>Y error column.</summary>
    YErr,

    /// <summary>Label column.</summary>
    Label,

    /// <summary>Ignored column.</summary>
    Ignore,

    /// <summary>Grouping column.</summary>
    Group,

    /// <summary>Subject column.</summary>
    Subject,
  };

  /// <summary>Represents a single column in a spreadsheet with formatting and data.</summary>
  public class SpreadColumn
  {
    /// <summary>Name of the column.</summary>
    public string Name;

    /// <summary>Name of the dataset this column belongs to (if any).</summary>
    public string DatasetName;

    /// <summary>Type of the column in the spreadsheet (X, Y, Z, etc.).</summary>
    public SpreadColumnType ColumnType;

    /// <summary>Logical value type for the column (numeric, text, date, etc.).</summary>
    public ValueType ValueType;

    /// <summary>Formatting specification for the column values.</summary>
    public ValueTypeSpecification ValueTypeSpecification;

    /// <summary>Number of significant digits used for display.</summary>
    public int SignificantDigits;

    /// <summary>Number of decimal places used for display.</summary>
    public int DecimalPlaces;

    /// <summary>How numeric values are displayed.</summary>
    public NumericDisplayType NumericDisplayType;

    /// <summary>Command or formula associated with this column (if any).</summary>
    public string Command;

    /// <summary>
    /// Designates the data type that was used to deserialize the data.
    /// This does not tell you how to interpret the data. For instance, double can
    /// also interpreted as DateTime, Time, etc.
    /// </summary>
    public DeserializedDataType DeserializedDataType { get; set; }

    /// <summary>Gets or sets the long name of the column.</summary>
    public string LongName { get; set; } = string.Empty;

    /// <summary>Gets or sets the units of the column.</summary>
    public string Units { get; set; } = string.Empty;

    /// <summary>Gets or sets the comments of the column.</summary>
    public string Comments { get; set; } = string.Empty;

    /// <summary>Display width for the column.</summary>
    public int Width;

    /// <summary>Index of the column within its sheet or dataset.</summary>
    public int Index;

    /// <summary>Column index used by the originating application.</summary>
    public int ColIndex;

    /// <summary>Sheet number this column belongs to.</summary>
    public int Sheet;

    /// <summary>Number of rows of data in this column.</summary>
    public int NumRows;

    /// <summary>Starting row index for valid data.</summary>
    public int BeginRow;

    /// <summary>Ending row index for valid data.</summary>
    public int EndRow;

    /// <summary>Data values stored in this column. Each element is a <see cref="Variant"/>.</summary>
    public List<Variant> Data;

    /// <summary>
    /// The imaginary data. Is only not null if
    /// the data are complex.
    /// </summary>
    public List<double>? ImaginaryData;

    /// <summary>Initializes a new instance of <see cref="SpreadColumn"/>.</summary>
    /// <param name="name">Optional column name.</param>
    /// <param name="index">Optional column index.</param>
    public SpreadColumn(string name = "", int index = 0)
    {
      Name = name;
      ColumnType = SpreadColumnType.Y;
      ValueType = ValueType.Numeric;
      ValueTypeSpecification = 0;
      SignificantDigits = 6;
      DecimalPlaces = 6;
      NumericDisplayType = NumericDisplayType.DefaultDecimalDigits;
      Width = 8;
      Index = index;
      ColIndex = 0;
      Sheet = 0;
      NumRows = 0;
      BeginRow = 0;
      EndRow = 0;
      Data = new();
    }

  }

  /// <summary>Represents a spreadsheet window containing multiple columns.</summary>
  public class SpreadSheet : Window
  {
    /// <summary>Maximum number of rows supported by the sheet.</summary>
    public int MaxRows;

    /// <summary>Whether the sheet uses loose storage semantics.</summary>
    public bool Loose;

    /// <summary>Number of sheets (pages) represented inside this spreadsheet window.</summary>
    public int Sheets;

    /// <summary>Columns contained in the spreadsheet.</summary>
    public List<SpreadColumn> Columns;

    /// <summary>Initialize a new <see cref="SpreadSheet"/> with an optional name.</summary>
    /// <param name="name">The spreadsheet name.</param>
    public SpreadSheet(string name = "")
        : base(name)
    {
      MaxRows = 30;
      Loose = true;
      Sheets = 1;
      Columns = [];
    }
  }

  /// <summary>Represents an Excel-like workbook.</summary>
  public class Excel : Window
  {
    /// <summary>Maximum number of rows supported by sheets in this workbook.</summary>
    public int MaxRows;

    /// <summary>Whether the workbook uses loose storage semantics.</summary>
    public bool Loose;

    /// <summary>Sheets contained in the workbook.</summary>
    public List<SpreadSheet> Sheets = [];

    /// <summary>Initialize a new <see cref="Excel"/> instance.</summary>
    /// <param name="name">The workbook name.</param>
    /// <param name="label">The workbook label.</param>
    /// <param name="maxRows">The maximum number of rows.</param>
    /// <param name="hidden">If <see langword="true"/>, the workbook window is hidden.</param>
    /// <param name="loose">If <see langword="true"/>, loose storage semantics are used.</param>
    public Excel(string name = "", string label = "", int maxRows = 0, bool hidden = false, bool loose = true)
        : base(name, label, hidden)
    {
      MaxRows = maxRows;
      Loose = loose;
    }
  }

  /// <summary>Represents a single sheet inside a matrix object.</summary>
  public class MatrixSheet
  {
    /// <summary>
    /// Defines the supported display modes of a matrix sheet.
    /// </summary>
    public enum ViewType
    {
      /// <summary>Display matrix values as data.</summary>
      DataView,

      /// <summary>Display matrix values as an image.</summary>
      ImageView
    };

    /// <summary>Name of the matrix sheet.</summary>
    public string Name;

    /// <summary>Number of rows in the sheet.</summary>
    public short RowCount;

    /// <summary>Number of columns in the sheet.</summary>
    public short ColumnCount;

    /// <summary>Formatting specification for values in the sheet.</summary>
    public int ValueTypeSpecification;

    /// <summary>Number of significant digits used for display in the sheet.</summary>
    public int SignificantDigits;

    /// <summary>Number of decimal places used for display in the sheet.</summary>
    public int DecimalPlaces;

    /// <summary>Numeric display type for values in this sheet.</summary>
    public NumericDisplayType NumericDisplayType;

    /// <summary>Command or formula associated with the matrix sheet.</summary>
    public string Command;

    /// <summary>Width (display) for the matrix sheet.</summary>
    public short Width;

    /// <summary>Index of the sheet within the parent matrix.</summary>
    public int Index;

    /// <summary>View type (data view or image view) used to render the sheet.</summary>
    public ViewType View;

    /// <summary>Color map applied to the sheet.</summary>
    public ColorMap ColorMap;

    /// <summary>Raw data values for the matrix sheet stored in row-major order.</summary>
    public List<double> Data = [];

    /// <summary>Imaginary parts corresponding to <see cref="Data"/>, used for complex numbers.</summary>
    public List<double> ImaginaryData = [];

    /// <summary>The x-value that corresponds to the first column of the matrix.</summary>
    public double X1 { get; set; }

    /// <summary>The x-value that corresponds to the last column of the matrix.</summary>
    public double X2 { get; set; }

    /// <summary>The y-value that corresponds to the first row of the matrix.</summary>
    public double Y1 { get; set; }

    /// <summary>The y-value that corresponds to the last row of the matrix.</summary>
    public double Y2 { get; set; }

    /// <summary>Initializes a new instance of <see cref="MatrixSheet"/>.</summary>
    /// <param name="name">The sheet name.</param>
    /// <param name="index">The sheet index.</param>
    public MatrixSheet(string name = "", int index = 0)
    {
      Name = name;
      RowCount = 8;
      ColumnCount = 8;
      ValueTypeSpecification = 0;
      SignificantDigits = 6;
      DecimalPlaces = 6;
      NumericDisplayType = NumericDisplayType.DefaultDecimalDigits;
      Width = 8;
      Index = index;
      View = ViewType.DataView;
      ColorMap = new ColorMap();
      X1 = Y1 = 1;
      X2 = Y2 = 10;
    }

    /// <summary>Get the matrix value at row r and column c, or NaN if out of range.</summary>
    /// <param name="r">Row index.</param>
    /// <param name="c">Column index.</param>
    /// <returns>The value at the requested position or <c>double.NaN</c> if out of range.</returns>
    public double this[int r, int c]
    {
      get
      {
        int i = r * ColumnCount + c;
        return i < Data.Count ? Data[i] : double.NaN;
      }
    }

    /// <summary>Return the imaginary part at the specified position or NaN if out of range.</summary>
    /// <param name="r">Row index.</param>
    /// <param name="c">Column index.</param>
    /// <returns>The imaginary part at the requested position or <c>double.NaN</c> if out of range.</returns>
    public double ImaginaryPart(int r, int c)
    {
      int i = r * ColumnCount + c;
      return i < ImaginaryData.Count ? ImaginaryData[i] : double.NaN;
    }
  }

  /// <summary>Represents a matrix window containing one or more sheets.</summary>
  public class Matrix : Window
  {
    /// <summary>
    /// Defines the supported header display modes for a matrix.
    /// </summary>
    public enum HeaderViewType
    {
      /// <summary>Show column and row headers.</summary>
      ColumnRow,

      /// <summary>Show X and Y headers.</summary>
      XY
    };

    /// <summary>Index of the active sheet in the matrix.</summary>
    public int ActiveSheet;

    /// <summary>Header view type used by the matrix.</summary>
    public HeaderViewType Header;

    /// <summary>Sheets that belong to this matrix.</summary>
    public List<MatrixSheet> Sheets = [];

    /// <summary>Initializes a new instance of <see cref="Matrix"/>.</summary>
    /// <param name="name">The matrix name.</param>
    public Matrix(string name = "")
        : base(name)
    {
      ActiveSheet = 0;
      Header = HeaderViewType.ColumnRow;
    }
  }

  /// <summary>Represents a function (formula-based dataset).</summary>
  public class Function
  {
    /// <summary>
    /// Defines the supported function representations.
    /// </summary>
    public enum FunctionType
    {
      /// <summary>A standard Cartesian function.</summary>
      Normal,

      /// <summary>A polar function.</summary>
      Polar
    };

    /// <summary>Name of the function/dataset.</summary>
    public string Name;

    /// <summary>Type of the function (normal or polar).</summary>
    public FunctionType Type;

    /// <summary>Formula or expression that defines the function.</summary>
    public string Formula;

    /// <summary>Beginning value of the domain.</summary>
    public double Begin;

    /// <summary>End value of the domain.</summary>
    public double End;

    /// <summary>Total number of points to generate for the function.</summary>
    public int TotalPoints;

    /// <summary>Index of the function within its container.</summary>
    public int Index;

    /// <summary>Initializes a new instance of <see cref="Function"/>.</summary>
    /// <param name="name">The function name.</param>
    /// <param name="index">The function index.</param>
    public Function(string name = "", int index = 0)
    {
      Name = name;
      Type = FunctionType.Normal;
      Begin = 0.0;
      End = 0.0;
      TotalPoints = 0;
      Index = index;
    }
  }

  /// <summary>Text box object placed on a window.</summary>
  public class TextBox
  {
    /// <summary>Text contained in the text box.</summary>
    public string Text;

    /// <summary>Client rectangle defining the box position and size.</summary>
    public Rect ClientRect;

    /// <summary>Color used to draw the text.</summary>
    public Color Color;

    /// <summary>Font size used for the text (in some internal units).</summary>
    public short FontSize;

    /// <summary>Rotation angle for the text.</summary>
    public int Rotation;

    /// <summary>Tab width used for the text box.</summary>
    public int Tab;

    /// <summary>Border type drawn around the text box.</summary>
    public BorderType BorderType;

    /// <summary>Attachment target for the text box (frame, page, etc.).</summary>
    public Attach Attach;

    /// <summary>Whether the text box is shown.</summary>
    public bool IsShown;

    /// <summary>Creates a new text box with default appearance.</summary>
    /// <param name="text">The text content.</param>
    public TextBox(string text = "")
    {
      Text = text;
      Color = new Color(ColorType.Regular, RegularColor.Black);
      FontSize = 20;
      Rotation = 0;
      Tab = 8;
      BorderType = BorderType.BlackLine;
      Attach = Attach.Frame;
      ClientRect = new Rect(); // Default value
    }

    /// <summary>Creates a new text box with explicit properties.</summary>
    /// <param name="text">The text content.</param>
    /// <param name="clientRect">The client rectangle.</param>
    /// <param name="color">The text color.</param>
    /// <param name="fontSize">The font size.</param>
    /// <param name="rotation">The text rotation angle.</param>
    /// <param name="tab">The tab width.</param>
    /// <param name="borderType">The border type.</param>
    /// <param name="attach">The attachment target.</param>
    /// <param name="isShown">If <see langword="true"/>, the text box is shown.</param>
    public TextBox(string text, Rect clientRect, Color color, short fontSize, int rotation, int tab, BorderType borderType, Attach attach, bool isShown = true)
    {
      Text = text;
      ClientRect = clientRect;
      Color = color;
      FontSize = fontSize;
      Rotation = rotation;
      Tab = tab;
      BorderType = borderType;
      Attach = attach;
      IsShown = isShown;
    }
  }

  /// <summary>Properties used by pie chart plots.</summary>
  public class PieProperties
  {
    /// <summary>View angle of the pie chart.</summary>
    public byte ViewAngle;

    /// <summary>Thickness of the pie (for 3D appearance).</summary>
    public byte Thickness;

    /// <summary>Whether the pie sections rotate clockwise.</summary>
    public bool ClockwiseRotation;

    /// <summary>Rotation of the pie in degrees.</summary>
    public short Rotation;

    /// <summary>Radius of the pie.</summary>
    public short Radius;

    /// <summary>Horizontal offset applied to the pie center.</summary>
    public short HorizontalOffset;

    /// <summary>Number of displaced sections (max 32).</summary>
    public int DisplacedSectionCount; // maximum - 32 sections

    /// <summary>Displacement distance for displaced sections.</summary>
    public short Displacement;

    //labels
    /// <summary>Whether label formatting is automatic.</summary>
    public bool FormatAutomatic;

    /// <summary>Whether to format numeric values in labels.</summary>
    public bool FormatValues;

    /// <summary>Whether to format percentages in labels.</summary>
    public bool FormatPercentages;

    /// <summary>Whether to format categories in labels.</summary>
    public bool FormatCategories;

    /// <summary>Whether labels are positioned associated to sections.</summary>
    public bool PositionAssociate;

    /// <summary>Distance of labels from the pie.</summary>
    public short Distance;

    /// <summary>Initializes pie properties with sensible defaults.</summary>
    public PieProperties()
    {
      ViewAngle = 33;
      Thickness = 33;
      ClockwiseRotation = false;
      Rotation = 33;
      Radius = 70;
      HorizontalOffset = 0;
      DisplacedSectionCount = 0;
      Displacement = 25;
      FormatAutomatic = false;
      FormatValues = false;
      FormatPercentages = false;
      FormatCategories = false;
      PositionAssociate = false;
      Distance = 25;
    }
  }

  /// <summary>Properties used by vector plots.</summary>
  public class VectorProperties
  {
    /// <summary>
    /// Defines where the vector is positioned relative to its anchor point.
    /// </summary>
    public enum VectorPosition
    {
      /// <summary>Position the vector by its tail.</summary>
      Tail,

      /// <summary>Position the vector by its midpoint.</summary>
      Midpoint,

      /// <summary>Position the vector by its head.</summary>
      Head
    };

    /// <summary>Color used for the vector lines/arrows.</summary>
    public Color Color;

    /// <summary>Line width for the vector.</summary>
    public double Width;

    /// <summary>Length of the arrowhead.</summary>
    public short ArrowLength;

    /// <summary>Opening angle of the arrowhead.</summary>
    public byte ArrowAngle;

    /// <summary>Whether the arrowhead is closed.</summary>
    public bool ArrowClosed;

    /// <summary>Name of the column that contains X coordinates for vector ends.</summary>
    public string EndXColumnName;

    /// <summary>Name of the column that contains Y coordinates for vector ends.</summary>
    public string EndYColumnName;

    /// <summary>Position of the vector relative to its data point (tail, midpoint, head).</summary>
    public VectorPosition Position;

    /// <summary>Name of the column that contains the angle values.</summary>
    public string AngleColumnName;

    /// <summary>Name of the column that contains magnitude values.</summary>
    public string MagnitudeColumnName;

    /// <summary>Multiplier applied to magnitude values.</summary>
    public float Multiplier;

    /// <summary>Constant angle used when angle column is not provided.</summary>
    public int ConstAngle;

    /// <summary>Constant magnitude used when magnitude column is not provided.</summary>
    public int ConstMagnitude;

    /// <summary>Initializes vector properties with default values.</summary>
    public VectorProperties()
    {
      Color = new Color(ColorType.Regular, RegularColor.Black);
      Width = 2.0;
      ArrowLength = 45;
      ArrowAngle = 30;
      ArrowClosed = false;
      Position = VectorPosition.Tail;
      Multiplier = 1.0f;
      ConstAngle = 0;
      ConstMagnitude = 0;
    }
  }

  /// <summary>Properties that control text appearance.</summary>
  public class TextProperties
  {
    /// <summary>
    /// Defines the horizontal justification of text.
    /// </summary>
    public enum TextJustify
    {
      /// <summary>Left-justified text.</summary>
      Left,

      /// <summary>Centered text.</summary>
      Center,

      /// <summary>Right-justified text.</summary>
      Right
    };

    /// <summary>Color used for the text.</summary>
    public Color Color;

    /// <summary>Whether the font is bold.</summary>
    public bool FontBold;

    /// <summary>Whether the font is italic.</summary>
    public bool FontItalic;

    /// <summary>Whether the font is underlined.</summary>
    public bool FontUnderline;

    /// <summary>Whether to white-out the background behind the text.</summary>
    public bool WhiteOut;

    /// <summary>Justification for the text.</summary>
    public TextJustify Justify;

    /// <summary>Rotation angle for the text.</summary>
    public short Rotation;

    /// <summary>Horizontal offset for the text.</summary>
    public short XOffset;

    /// <summary>Vertical offset for the text.</summary>
    public short YOffset;

    /// <summary>Font size used for the text.</summary>
    public short FontSize;
  }

  /// <summary>Types of surface rendering.</summary>
  public enum SurfaceType
  {
    /// <summary>3D color map surface.</summary>
    ColorMap3D,

    /// <summary>Color-filled surface.</summary>
    ColorFill,

    /// <summary>Wireframe surface.</summary>
    WireFrame,

    /// <summary>Bar-style surface.</summary>
    Bars
  };

  /// <summary>Grid types for surface plots.</summary>
  public enum SurfaceGrids
  {
    /// <summary>No surface grid lines.</summary>
    None,

    /// <summary>Grid lines along the X direction.</summary>
    X,

    /// <summary>Grid lines along the Y direction.</summary>
    Y,

    /// <summary>Grid lines along both X and Y directions.</summary>
    XY
  };

  /// <summary>Properties that control surface plot appearance and coloration.</summary>
  public class SurfaceProperties
  {
    /// <summary>Helper type that groups coloration options for parts of a surface.</summary>
    public class SurfaceColoration
    {
      /// <summary>Whether to fill the region.</summary>
      public bool Fill;
      /// <summary>Whether to draw contour lines.</summary>
      public bool Contour;
      /// <summary>Color of contour or lines.</summary>
      public Color LineColor;
      /// <summary>Line width for contours/lines.</summary>
      public double LineWidth;
    }



    /// <summary>Surface rendering type identifier.</summary>
    public byte Type;

    /// <summary>Grid type displayed on the surface.</summary>
    public SurfaceGrids Grids;

    /// <summary>Width of grid lines.</summary>
    public double GridLineWidth;

    /// <summary>Color used for the grid.</summary>
    public Color GridColor;

    /// <summary>Whether back color is enabled for the surface.</summary>
    public bool BackColorEnabled;

    /// <summary>Front color for the surface.</summary>
    public Color FrontColor;

    /// <summary>Back color for the surface.</summary>
    public Color BackColor;

    /// <summary>Whether side walls are enabled.</summary>
    public bool SideWallEnabled;

    /// <summary>Color used for the X side wall.</summary>
    public Color XSideWallColor;

    /// <summary>Color used for the Y side wall.</summary>
    public Color YSideWallColor;

    /// <summary>Gets or sets surface coloration details.</summary>
    public SurfaceColoration Surface
    {
      get { return field ??= new(); }
      set { field = value; }
    }

    /// <summary>Gets or sets the top contour coloration details.</summary>
    public SurfaceColoration TopContour
    {
      get { return field ??= new(); }
      set { field = value; }
    }
    /// <summary>Gets or sets the bottom contour coloration details.</summary>
    public SurfaceColoration BottomContour
    {
      get { return field ??= new(); }
      set { field = value; }
    }

    /// <summary>Gets or sets the color map used for the surface.</summary>
    public ColorMap ColorMap
    {
      get { return field ??= new(); }
      set { field = value; }
    }
  }

  /// <summary>Properties controlling percentile/box-plot rendering.</summary>
  public class PercentileProperties
  {
    /// <summary>Symbol type used for the maximum value marker.</summary>
    public byte MaxSymbolType;

    /// <summary>Symbol type used for the 99th percentile marker.</summary>
    public byte P99SymbolType;

    /// <summary>Symbol type used for the mean marker.</summary>
    public byte MeanSymbolType;

    /// <summary>Symbol type used for the 1st percentile marker.</summary>
    public byte P1SymbolType;

    /// <summary>Symbol type used for the minimum value marker.</summary>
    public byte MinSymbolType;

    /// <summary>Color used for the symbols.</summary>
    public Color SymbolColor;

    /// <summary>Fill color used for the symbols.</summary>
    public Color SymbolFillColor;

    /// <summary>Size of the symbols.</summary>
    public short SymbolSize;

    /// <summary>Box width range setting.</summary>
    public byte BoxRange;

    /// <summary>Whiskers range setting.</summary>
    public byte WhiskersRange;

    /// <summary>Coefficient used to compute box size.</summary>
    public double BoxCoeff;

    /// <summary>Coefficient used to compute whiskers size.</summary>
    public double WhiskersCoeff;

    /// <summary>Whether the box shape is diamond-like.</summary>
    public bool DiamondBox;

    /// <summary>Labeling option flags.</summary>
    public byte Labels;

    /// <summary>Initializes percentile properties with default values.</summary>
    public PercentileProperties()
    {
      MaxSymbolType = 1;
      P99SymbolType = 2;
      MeanSymbolType = 3;
      P1SymbolType = 4;
      MinSymbolType = 5;
      SymbolColor = new Color(ColorType.Regular, RegularColor.Black);
      SymbolFillColor = new Color(ColorType.Regular, RegularColor.White);
      SymbolSize = 5;
      BoxRange = 25;
      WhiskersRange = 5;
      BoxCoeff = 1.0;
      WhiskersCoeff = 1.5;
      DiamondBox = true;
      Labels = 0;
    }
  }

  /// <summary>Line styles for plotting.</summary>
  public enum LineStyle
  {
    /// <summary>Solid line.</summary>
    Solid = 0,

    /// <summary>Dashed line.</summary>
    Dash = 1,

    /// <summary>Dotted line.</summary>
    Dot = 2,

    /// <summary>Dash-dot line.</summary>
    DashDot = 3,

    /// <summary>Dash-dot-dot line.</summary>
    DashDotDot = 4,

    /// <summary>Short dashed line.</summary>
    ShortDash = 5,

    /// <summary>Short dotted line.</summary>
    ShortDot = 6,

    /// <summary>Short dash-dot line.</summary>
    ShortDashDot = 7
  }

  /// <summary>How line segments are connected between data points.</summary>
  public enum LineConnect
  {
    /// <summary>Do not connect points with lines.</summary>
    NoLine = 0,

    /// <summary>Connect points with straight segments.</summary>
    Straight = 1,

    /// <summary>Use two-point segments.</summary>
    TwoPointSegment = 2,

    /// <summary>Use three-point segments.</summary>
    ThreePointSegment = 3,

    /// <summary>Use a B-spline connection.</summary>
    BSpline = 8,

    /// <summary>Use a spline connection.</summary>
    Spline = 9,

    /// <summary>Use horizontal step segments.</summary>
    StepHorizontal = 11,

    /// <summary>Use vertical step segments.</summary>
    StepVertical = 12,

    /// <summary>Use centered horizontal step segments.</summary>
    StepHCenter = 13,

    /// <summary>Use centered vertical step segments.</summary>
    StepVCenter = 14,

    /// <summary>Use a Bézier connection.</summary>
    Bezier = 15
  }

  /// <summary>Represents a plot curve with visual properties and data references.</summary>
  public class GraphCurve
  {
    /// <summary>Plot types supported by a graph curve.</summary>
    public enum Plot
    {
      /// <summary>3D scatter plot.</summary>
      Scatter3D = 101,

      /// <summary>3D surface plot.</summary>
      Surface3D = 103,

      /// <summary>3D vector plot.</summary>
      Vector3D = 183,

      /// <summary>3D scatter plot with error bars.</summary>
      ScatterAndErrorBar3D = 184,

      /// <summary>Ternary contour plot.</summary>
      TernaryContour = 185,

      /// <summary>Polar plot with X as radius and Y as angle.</summary>
      PolarXrYTheta = 186,

      /// <summary>Smith chart plot.</summary>
      SmithChart = 191,

      /// <summary>Polar plot.</summary>
      Polar = 192,

      /// <summary>Bubble plot with indexed sizes or colors.</summary>
      BubbleIndexed = 193,

      /// <summary>Bubble plot with color mapping.</summary>
      BubbleColorMapped = 194,

      /// <summary>Line plot.</summary>
      Line = 200,

      /// <summary>Scatter plot.</summary>
      Scatter = 201,

      /// <summary>Line and symbol plot.</summary>
      LineSymbol = 202,

      /// <summary>Column plot.</summary>
      Column = 203,

      /// <summary>Area plot.</summary>
      Area = 204,

      /// <summary>High-low-close plot.</summary>
      HiLoClose = 205,

      /// <summary>Box plot.</summary>
      Box = 206,

      /// <summary>Floating column plot.</summary>
      ColumnFloat = 207,

      /// <summary>Vector plot.</summary>
      Vector = 208,

      /// <summary>Dot plot.</summary>
      PlotDot = 209,

      /// <summary>3D wall plot.</summary>
      Wall3D = 210,

      /// <summary>3D ribbon plot.</summary>
      Ribbon3D = 211,

      /// <summary>3D bar plot.</summary>
      Bar3D = 212,

      /// <summary>Stacked column plot.</summary>
      ColumnStack = 213,

      /// <summary>Stacked area plot.</summary>
      AreaStack = 214,

      /// <summary>Bar plot.</summary>
      Bar = 215,

      /// <summary>Stacked bar plot.</summary>
      BarStack = 216,

      /// <summary>Flow vector plot.</summary>
      FlowVector = 218,

      /// <summary>Histogram plot.</summary>
      Histogram = 219,

      /// <summary>Matrix image plot.</summary>
      MatrixImage = 220,

      /// <summary>Pie chart.</summary>
      Pie = 225,

      /// <summary>Contour plot.</summary>
      Contour = 226,

      /// <summary>Unknown plot type.</summary>
      Unknown = 230,

      /// <summary>Error bar plot.</summary>
      ErrorBar = 231,

      /// <summary>Text plot.</summary>
      TextPlot = 232,

      /// <summary>X error bar plot.</summary>
      XErrorBar = 233,

      /// <summary>Surface plot with color mapping.</summary>
      SurfaceColorMap = 236,

      /// <summary>Surface plot with color fill.</summary>
      SurfaceColorFill = 237,

      /// <summary>Wireframe surface plot.</summary>
      SurfaceWireframe = 238,

      /// <summary>Surface bar plot.</summary>
      SurfaceBars = 239,

      /// <summary>3D line plot.</summary>
      Line3D = 240,

      /// <summary>3D text plot.</summary>
      Text3D = 241,

      /// <summary>3D mesh plot.</summary>
      Mesh3D = 242,

      /// <summary>XYZ contour plot.</summary>
      XYZContour = 243,

      /// <summary>XYZ triangular surface plot.</summary>
      XYZTriangular = 245,

      /// <summary>Line series plot.</summary>
      LineSeries = 246,

      /// <summary>Y error bar plot.</summary>
      YErrorBar = 254,

      /// <summary>XY error bar plot.</summary>
      XYErrorBar = 255
    }



    /// <summary>Whether the curve is hidden.</summary>
    public bool IsHidden;

    /// <summary>Type of plot for this curve.</summary>
    public GraphCurve.Plot PlotType;

    /// <summary>Name of the dataset that contains the curve data.</summary>
    public string DataName;

    /// <summary>Name of the dataset providing X values (if different).</summary>
    public string XDataName;

    /// <summary>Name of the X column used by the curve.</summary>
    public string XColumnName;

    /// <summary>Name of the Y column used by the curve.</summary>
    public string YColumnName;

    /// <summary>Name of the Z column used by the curve (for 3D plots).</summary>
    public string ZColumnName;

    /// <summary>Color used for the curve line.</summary>
    public Color LineColor;

    /// <summary>Transparency level for the line (0-255).</summary>
    public byte LineTransparency;

    /// <summary>Line style index.</summary>
    public byte LineStyle;

    /// <summary>How line segments are connected between points.</summary>
    public byte LineConnect;

    /// <summary>Width for box-type plots.</summary>
    public byte BoxWidth;

    /// <summary>Width of the line used when drawing the curve.</summary>
    public double LineWidth;

    /// <summary>Whether the area under/around the curve is filled.</summary>
    public bool FillArea;

    /// <summary>Type identifier for fill areas.</summary>
    public byte FillAreaType;

    /// <summary>Fill pattern index for the fill area.</summary>
    public byte FillAreaPattern;

    /// <summary>Color used to fill the area.</summary>
    public Color FillAreaColor;

    /// <summary>Transparency for the fill area.</summary>
    public byte FillAreaTransparency;

    /// <summary>Whether the fill area respects line transparency.</summary>
    public bool FillAreaWithLineTransparency;

    /// <summary>Color used for the fill area pattern.</summary>
    public Color FillAreaPatternColor;

    /// <summary>Line width used for the fill area pattern.</summary>
    public double FillAreaPatternWidth;

    /// <summary>Border style for the fill area pattern.</summary>
    public byte FillAreaPatternBorderStyle;

    /// <summary>Color for the fill area pattern border.</summary>
    public Color FillAreaPatternBorderColor;

    /// <summary>Width for the fill area pattern border.</summary>
    public double FillAreaPatternBorderWidth;

    /// <summary>Interior style for symbols.</summary>
    public byte SymbolInterior;

    /// <summary>Shape identifier for symbols.</summary>
    public byte SymbolShape;

    /// <summary>Color used for symbol outlines.</summary>
    public Color SymbolColor;

    /// <summary>Fill color used for symbols.</summary>
    public Color SymbolFillColor;

    /// <summary>Transparency for symbol fill.</summary>
    public byte SymbolFillTransparency;

    /// <summary>Size of symbols.</summary>
    public double SymbolSize;

    /// <summary>Thickness of symbol outlines.</summary>
    public byte SymbolThickness;

    /// <summary>Offset applied to points when rendering.</summary>
    public byte PointOffset;

    /// <summary>Whether symbols are connected by lines.</summary>
    public bool ConnectSymbols;

    //pie
    /// <summary>Pie chart specific properties. Initialized lazily.</summary>
    public PieProperties? Pie
    {
      get { return field ??= new(); }
      set { field = value; }
    }

    //vector
    /// <summary>Vector plot specific properties. Initialized lazily.</summary>
    public VectorProperties? Vector
    {
      get { return field ??= new(); }
      set { field = value; }
    }

    //text
    /// <summary>Text plot specific properties. Initialized lazily.</summary>
    public TextProperties? Text
    {
      get { return field ??= new(); }
      set { field = value; }
    }

    //surface
    /// <summary>Surface plot specific properties. Initialized lazily.</summary>
    public SurfaceProperties? Surface
    {
      get { return field ??= new(); }
      set { field = value; }
    }

    //contour
    /// <summary>Color map used for contour plots. Initialized lazily.</summary>
    public ColorMap? ColorMap
    {
      get { return field ??= new(); }
      set { field = value; }
    }
  }

  /// <summary>Defines a break in an axis scale with visual parameters.</summary>
  public class GraphAxisBreak
  {
    /// <summary>Whether the break is shown.</summary>
    public bool Show;

    /// <summary>Whether the break is displayed on a logarithmic scale (base 10).</summary>
    public bool Log10;

    /// <summary>Start value of the break.</summary>
    public double From;

    /// <summary>End value of the break.</summary>
    public double To;

    /// <summary>Position of the break as a percentage of the axis.</summary>
    public double Position;

    /// <summary>Scale increment before the break.</summary>
    public double ScaleIncrementBefore;

    /// <summary>Scale increment after the break.</summary>
    public double ScaleIncrementAfter;

    /// <summary>Number of minor ticks before the break.</summary>
    public byte MinorTicksBefore;

    /// <summary>Number of minor ticks after the break.</summary>
    public byte MinorTicksAfter;

    /// <summary>Initializes a new instance of <see cref="GraphAxisBreak"/> with default values.</summary>
    public GraphAxisBreak()
    {
      Show = false;
      Log10 = false;
      From = 4.0;
      To = 6.0;
      Position = 50.0;
      ScaleIncrementBefore = 5.0;
      ScaleIncrementAfter = 5.0;
      MinorTicksBefore = 1;
      MinorTicksAfter = 1;
    }
  }
  /// <summary>Grid line settings for an axis.</summary>
  public class GraphGrid
  {
    /// <summary>Whether the grid line is hidden.</summary>
    public bool Hidden;

    /// <summary>Color index used for the grid line.</summary>
    public byte Color;

    /// <summary>Style index used for the grid line.</summary>
    public byte Style;

    /// <summary>Width of the grid line.</summary>
    public double Width;
  }

  /// <summary>Formatting and appearance settings for an axis.</summary>
  public class GraphAxisFormat
  {
    /// <summary>Whether the axis format is hidden.</summary>
    public bool IsHidden;

    /// <summary>Color index used to draw the axis.</summary>
    public byte Color;

    /// <summary>Thickness of the axis line.</summary>
    public double Thickness;

    /// <summary>Length of major tick marks.</summary>
    public double MajorTickLength;

    /// <summary>Major ticks type identifier.</summary>
    public int MajorTicksType;

    /// <summary>Minor ticks type identifier.</summary>
    public int MinorTicksType;

    /// <summary>Axis position type identifier.</summary>
    public int AxisPosition;

    /// <summary>Numeric value used for axis position when needed.</summary>
    public double AxisPositionValue;

    /// <summary>Label displayed for the axis.</summary>
    public TextBox Label;

    /// <summary>Prefix applied to tick labels.</summary>
    public string Prefix;

    /// <summary>Suffix applied to tick labels.</summary>
    public string Suffix;

    /// <summary>Scaling factor applied to tick labels.</summary>
    public string Factor;
  }

  /// <summary>Settings for axis ticks and labels.</summary>
  public class GraphAxisTick
  {
    /// <summary>Whether major labels are shown for ticks.</summary>
    public bool ShowMajorLabels;

    /// <summary>Color index used for tick labels.</summary>
    public byte Color;

    /// <summary>Logical value type used by tick labels (numeric, date, etc.).</summary>
    public ValueType ValueType;

    /// <summary>Formatting specification for tick label values.</summary>
    public int ValueTypeSpecification;

    /// <summary>Decimal places used when formatting tick values.</summary>
    public int DecimalPlaces;

    /// <summary>Font size used for tick labels.</summary>
    public short FontSize;

    /// <summary>Whether tick label font is bold.</summary>
    public bool FontBold;

    /// <summary>Name of the data set used for ticks (if any).</summary>
    public string DataName;

    /// <summary>Name of the column used for tick values (if any).</summary>
    public string ColumnName;

    /// <summary>Rotation applied to tick labels.</summary>
    public int Rotation;
  }

  /// <summary>Represents an axis with scaling, grid and tick configuration.</summary>
  public class GraphAxis
  {
    /// <summary>
    /// Defines the position of an axis within a graph.
    /// </summary>
    public enum AxisPosition
    {
      /// <summary>Left axis.</summary>
      Left = 0,

      /// <summary>Bottom axis.</summary>
      Bottom,

      /// <summary>Right axis.</summary>
      Right,

      /// <summary>Top axis.</summary>
      Top,

      /// <summary>Front axis.</summary>
      Front,

      /// <summary>Back axis.</summary>
      Back
    }

    /// <summary>
    /// Defines the supported axis scaling modes.
    /// </summary>
    public enum AxisScale
    {
      /// <summary>Linear scale.</summary>
      Linear = 0,

      /// <summary>Base-10 logarithmic scale.</summary>
      Log10 = 1,

      /// <summary>Probability scale.</summary>
      Probability = 2,

      /// <summary>Probit scale.</summary>
      Probit = 3,

      /// <summary>Reciprocal scale.</summary>
      Reciprocal = 4,

      /// <summary>Offset reciprocal scale.</summary>
      OffsetReciprocal = 5,

      /// <summary>Natural logarithmic scale.</summary>
      Logit = 6,

      /// <summary>Natural logarithm scale.</summary>
      Ln = 7,

      /// <summary>Base-2 logarithmic scale.</summary>
      Log2 = 8
    }

    /// <summary>Position of the axis (left, right, top, bottom, etc.).</summary>
    public AxisPosition Position;

    /// <summary>Whether a zero baseline is drawn.</summary>
    public bool ZeroLine;

    /// <summary>Whether the opposite axis line is drawn.</summary>
    public bool OppositeLine;

    /// <summary>Minimum value of the axis scale.</summary>
    public double Min;

    /// <summary>Maximum value of the axis scale.</summary>
    public double Max;

    /// <summary>Step increment for the axis.</summary>
    public double Step;

    /// <summary>Anchor value for anchored axes.</summary>
    public double Anchor;

    /// <summary>Number of major ticks.</summary>
    public byte MajorTicks;

    /// <summary>Number of minor ticks.</summary>
    public byte MinorTicks;

    /// <summary>Scale type identifier.</summary>
    public byte Scale;

    /// <summary>Major grid settings for the axis.</summary>
    public GraphGrid MajorGrid;

    /// <summary>Minor grid settings for the axis.</summary>
    public GraphGrid MinorGrid;

    /// <summary>Formatting settings for the axis. Two elements expected.</summary>
    public GraphAxisFormat[] FormatAxis; // Assuming size of 2

    /// <summary>Tick settings for the axis. Two elements expected.</summary>
    public GraphAxisTick[] TickAxis; // Assuming size of 2

    /// <summary>Initializes a new instance of <see cref="GraphAxis"/> with default sub-objects.</summary>
    public GraphAxis()
    {
      Position = new AxisPosition();
      MajorGrid = new GraphGrid();
      MinorGrid = new GraphGrid();
      FormatAxis = new GraphAxisFormat[2] { new GraphAxisFormat(), new GraphAxisFormat() };
      TickAxis = new GraphAxisTick[2] { new GraphAxisTick(), new GraphAxisTick() };
    }
  }

  /// <summary>Shapes that may be used as annotation figures.</summary>
  public enum FigureType
  {
    /// <summary>Rectangle figure.</summary>
    Rectangle,

    /// <summary>Circle figure.</summary>
    Circle
  }

  /// <summary>Represents a simple figure or annotation placed on a window.</summary>
  public class Figure
  {
    /// <summary>Type of figure.</summary>
    public FigureType FigureType;

    /// <summary>Client rectangle defining the figure bounds.</summary>
    public Rect ClientRect;

    /// <summary>Attachment target for the figure.</summary>
    public Attach Attach;

    /// <summary>Color used to draw the figure outline.</summary>
    public Color Color;

    /// <summary>Style index used for the outline.</summary>
    public byte Style;

    /// <summary>Line width for the outline.</summary>
    public double Width;

    /// <summary>Fill color used for the figure area.</summary>
    public Color FillAreaColor;

    /// <summary>Pattern used to fill the figure area.</summary>
    public FillPattern FillAreaPattern;

    /// <summary>Color used for the fill area pattern.</summary>
    public Color FillAreaPatternColor;

    /// <summary>Line width used for the fill pattern.</summary>
    public double FillAreaPatternWidth;

    /// <summary>Whether to use the border color for the fill area.</summary>
    public bool UseBorderColor;

    /// <summary>Initializes a new <see cref="Figure"/> with default appearance.</summary>
    public Figure(FigureType type = FigureType.Rectangle)
    {
      FigureType = type;
      Attach = Attach.Frame;
      Color = new Color(ColorType.Regular, RegularColor.Black);
      Style = 0;
      Width = 1.0;
      FillAreaColor = new Color(ColorType.Regular, RegularColor.LightGray);
      FillAreaPattern = FillPattern.NoFill;
      FillAreaPatternColor = new Color(ColorType.Regular, RegularColor.Black);
      FillAreaPatternWidth = 1;
      UseBorderColor = false;
    }
  }

  /// <summary>Vertex representing a shape marker for a line.</summary>
  public class LineVertex
  {
    /// <summary>Shape type identifier for the vertex.</summary>
    public byte ShapeType;

    /// <summary>Width of the shape marker.</summary>
    public double ShapeWidth;

    /// <summary>Length of the shape marker.</summary>
    public double ShapeLength;

    /// <summary>X coordinate of the vertex.</summary>
    public double X;

    /// <summary>Y coordinate of the vertex.</summary>
    public double Y;

    /// <summary>Initializes a new <see cref="LineVertex"/> with default values.</summary>
    public LineVertex()
    {
      ShapeType = 0;
      ShapeWidth = 0;
      ShapeLength = 0;
      X = 0;
      Y = 0;
    }
  }

  /// <summary>Represents a drawable line element with optional end vertices.</summary>
  public class Line
  {
    /// <summary>Client rectangle that bounds the line.</summary>
    public Rect ClientRect;

    /// <summary>Color used to draw the line.</summary>
    public Color Color;

    /// <summary>Attachment target for the line.</summary>
    public Attach Attach;

    /// <summary>Width of the line.</summary>
    public double Width;

    /// <summary>Style index for the line.</summary>
    public byte Style;

    /// <summary>Vertex at the beginning of the line.</summary>
    public LineVertex Begin;

    /// <summary>Vertex at the end of the line.</summary>
    public LineVertex End;
  }

  /// <summary>Bitmap image embedded in a window.</summary>
  public class Bitmap
  {
    /// <summary>Client rectangle where the bitmap is displayed.</summary>
    public Rect ClientRect;

    /// <summary>Attachment target for the bitmap.</summary>
    public Attach Attach;

    /// <summary>Size in bytes of the bitmap data.</summary>
    public long Size;

    /// <summary>Name of the window the bitmap belongs to (if any).</summary>
    public string WindowName;

    /// <summary>Border type drawn around the bitmap.</summary>
    public BorderType BorderType;

    /// <summary>Raw bitmap data bytes.</summary>
    public byte[] Data;

    /// <summary>Creates an empty bitmap with an optional window name.</summary>
    /// <param name="name">The associated window name.</param>
    public Bitmap(string name = "")
    {
      Attach = Attach.Frame;
      Size = 0;
      WindowName = name;
      BorderType = BorderType.BlackLine;
      Data = null;
    }

    /// <summary>Creates a deep copy of the provided bitmap.</summary>
    /// <param name="bitmap">Source bitmap to copy.</param>
    public Bitmap(Bitmap bitmap)
    {
      ClientRect = bitmap.ClientRect;
      Attach = bitmap.Attach;
      Size = bitmap.Size;
      WindowName = bitmap.WindowName;
      BorderType = bitmap.BorderType;
      Data = null;

      if (Size > 0)
      {
        Data = new byte[Size];
        Array.Copy(bitmap.Data, Data, (int)Size);
      }
    }
  }

  /// <summary>Color scale settings for color-mapped plots.</summary>
  public class ColorScale
  {
    /// <summary>Whether the color scale is visible.</summary>
    public bool IsVisible;

    /// <summary>Whether the color scale order is reversed.</summary>
    public bool ReverseOrder;

    /// <summary>Gap between labels on the color scale.</summary>
    public short LabelGap;

    /// <summary>Thickness of the color bar in the scale.</summary>
    public short ColorBarThickness;

    /// <summary>Color used for the labels on the color scale.</summary>
    public Color LabelsColor;

    /// <summary>Initializes a new <see cref="ColorScale"/> with defaults.</summary>
    public ColorScale()
    {
      IsVisible = true;
      ReverseOrder = false;
      LabelGap = 5;
      ColorBarThickness = 3;
      LabelsColor = new Color(ColorType.Regular, RegularColor.Black);
    }
  }

  /// <summary>Represents a single graph layer containing plotting elements and axes.</summary>
  public struct GraphLayer
  {
    /// <summary>Client rectangle for the layer.</summary>
    public Rect ClientRect;

    /// <summary>Legend text box for the layer.</summary>
    public TextBox Legend;

    /// <summary>Background color for the layer.</summary>
    public Color BackgroundColor;

    /// <summary>Border type used around the layer.</summary>
    public BorderType BorderType;

    /// <summary>X axis configuration for the layer.</summary>
    public GraphAxis XAxis;

    /// <summary>Y axis configuration for the layer.</summary>
    public GraphAxis YAxis;

    /// <summary>Z axis configuration for the layer.</summary>
    public GraphAxis ZAxis;

    /// <summary>Break settings for the X axis.</summary>
    public GraphAxisBreak XAxisBreak;

    /// <summary>Break settings for the Y axis.</summary>
    public GraphAxisBreak YAxisBreak;

    /// <summary>Break settings for the Z axis.</summary>
    public GraphAxisBreak ZAxisBreak;

    /// <summary>Histogram bin size used when drawing histograms.</summary>
    public double HistogramBin;

    /// <summary>Beginning value used for histograms.</summary>
    public double HistogramBegin;

    /// <summary>Ending value used for histograms.</summary>
    public double HistogramEnd;

    /// <summary>Percentile and box-plot rendering properties.</summary>
    public PercentileProperties Percentile;

    /// <summary>Color scale used by the layer.</summary>
    public ColorScale ColorScale;

    /// <summary>Color map used by the layer.</summary>
    public ColorMap ColorMap;

    /// <summary>Text boxes on the layer.</summary>
    public List<TextBox> Texts;

    /// <summary>Text boxes used by pie charts on the layer.</summary>
    public List<TextBox> PieTexts;

    /// <summary>Lines drawn on the layer.</summary>
    public List<Line> Lines;

    /// <summary>Figures/annotations on the layer.</summary>
    public List<Figure> Figures;

    /// <summary>Embedded bitmaps on the layer.</summary>
    public List<Bitmap> Bitmaps;

    /// <summary>Curves plotted on the layer.</summary>
    public List<GraphCurve> Curves;

    /// <summary>Rotation angle around the X axis for 3D views.</summary>
    public float XAngle;

    /// <summary>Rotation angle around the Y axis for 3D views.</summary>
    public float YAngle;

    /// <summary>Rotation angle around the Z axis for 3D views.</summary>
    public float ZAngle;

    /// <summary>Length scale along the X axis for 3D views.</summary>
    public float XLength;

    /// <summary>Length scale along the Y axis for 3D views.</summary>
    public float YLength;

    /// <summary>Length scale along the Z axis for 3D views.</summary>
    public float ZLength;

    /// <summary>Identifier for an image profile tool associated with the layer.</summary>
    public int ImageProfileTool;

    /// <summary>Vertical reference line position.</summary>
    public double VLine;

    /// <summary>Horizontal reference line position.</summary>
    public double HLine;

    /// <summary>Whether the layer is rendered as a waterfall plot.</summary>
    public bool IsWaterfall;

    /// <summary>Offset applied to X coordinates for waterfall rendering.</summary>
    public double XOffset;

    /// <summary>Offset applied to Y coordinates for waterfall rendering.</summary>
    public double YOffset;

    /// <summary>Whether grid lines are drawn on top of plotted elements.</summary>
    public bool GridOnTop;

    /// <summary>Whether axes are exchanged (X/Y) for the layer.</summary>
    public bool ExchangedAxes;

    /// <summary>Whether the layer is an XYY 3D type.</summary>
    public bool IsXYY3D;

    /// <summary>Whether orthographic projection is used for 3D rendering.</summary>
    public bool Orthographic3D;

    /// <summary>Initializes a new <see cref="GraphLayer"/> with default values and empty collections.</summary>
    public GraphLayer()
    {
      BackgroundColor = new Color(ColorType.Regular, RegularColor.White);
      BorderType = BorderType.BlackLine;
      XAxis = new GraphAxis();
      YAxis = new GraphAxis();
      ZAxis = new GraphAxis();
      XAxisBreak = new GraphAxisBreak();
      YAxisBreak = new GraphAxisBreak();
      ZAxisBreak = new GraphAxisBreak();
      HistogramBin = 0.5;
      HistogramBegin = 0.0;
      HistogramEnd = 10.0;
      Percentile = new PercentileProperties();
      ColorMap = new ColorMap();
      XAngle = 0;
      YAngle = 0;
      ZAngle = 0;
      XLength = 10;
      YLength = 10;
      ZLength = 10;
      ImageProfileTool = 0;
      VLine = 0.0;
      HLine = 0.0;
      IsWaterfall = false;
      XOffset = 10;
      YOffset = 10;
      GridOnTop = false;
      ExchangedAxes = false;
      IsXYY3D = false;
      Orthographic3D = false;
      ColorScale = new ColorScale()
      {
        IsVisible = false,
      };

      Texts = new();
      PieTexts = new();
      Lines = new();
      Figures = new();
      Bitmaps = new();
      Curves = new();
    }

    //bool threeDimensional;
    /// <summary>Determines whether any curve in the layer is a 3D plot.</summary>
    /// <returns>True if at least one curve is a 3D plot; otherwise false.</returns>
    public bool Is3D()
    {
      foreach (GraphCurve curve in Curves)
      {
        switch ((GraphCurve.Plot)curve.PlotType)
        {
          case GraphCurve.Plot.Scatter3D:
          case GraphCurve.Plot.Surface3D:
          case GraphCurve.Plot.Vector3D:
          case GraphCurve.Plot.ScatterAndErrorBar3D:
          case GraphCurve.Plot.TernaryContour:
          case GraphCurve.Plot.Line3D:
          case GraphCurve.Plot.Mesh3D:
          case GraphCurve.Plot.XYZContour:
          case GraphCurve.Plot.XYZTriangular:
            return true;
          default:
            break;
        }
      }
      return false;
    }
  }

  /// <summary>Simple range structure used for histogram or axis ranges.</summary>
  public class GraphLayerRange
  {
    /// <summary>Minimum value for the range.</summary>
    public double Min;

    /// <summary>Maximum value for the range.</summary>
    public double Max;

    /// <summary>Step size used for the range.</summary>
    public double Step;

    /// <summary>Initializes a new instance of <see cref="GraphLayerRange"/>.</summary>
    /// <param name="_min">The minimum value.</param>
    /// <param name="_max">The maximum value.</param>
    /// <param name="_step">The step value.</param>
    public GraphLayerRange(double _min = 0.0, double _max = 0.0, double _step = 0.0)
    {
      Min = _min;
      Max = _max;
      Step = _step;
    }
  }

  /// <summary>Represents a graph window containing one or more layers.</summary>
  public class Graph : Window
  {
    /// <summary>Layers contained in the graph.</summary>
    public List<GraphLayer> Layers;

    /// <summary>Width of the graph in pixels.</summary>
    public short Width;

    /// <summary>Height of the graph in pixels.</summary>
    public short Height;

    /// <summary>Whether the graph contains 3D content.</summary>
    public bool Is3D;

    /// <summary>Whether the graph is a layout rather than a single plot area.</summary>
    public bool IsLayout;

    /// <summary>Whether missing data points should be connected by lines.</summary>
    public bool ConnectMissingData;

    /// <summary>Name of the template used for the graph (if any).</summary>
    public string TemplateName;

    /// <summary>Initializes a new <see cref="Graph"/> instance with defaults.</summary>
    /// <param name="name">The graph name.</param>
    public Graph(string name = "")
      : base(name)
    {
      Width = 400;
      Height = 300;
      Is3D = false;
      IsLayout = false;
      ConnectMissingData = false;
      Layers = new List<GraphLayer>();
    }
  }

  /// <summary>A simple note window containing text.</summary>
  public class Note : Window
  {
    /// <summary>Text content of the note.</summary>
    public string Text;

    /// <summary>Initializes a new <see cref="Note"/> instance.</summary>
    /// <param name="name">The note name.</param>
    public Note(string name = "")
        : base(name)
    {
    }
  }

  /// <summary>Node types used in the project tree.</summary>
  public enum ProjectNodeType
  {
    /// <summary>Spreadsheet node.</summary>
    SpreadSheet,

    /// <summary>Matrix node.</summary>
    Matrix,

    /// <summary>Excel workbook node.</summary>
    Excel,

    /// <summary>Graph node.</summary>
    Graph,

    /// <summary>3D graph node.</summary>
    Graph3D,

    /// <summary>Note node.</summary>
    Note,

    /// <summary>Folder node.</summary>
    Folder
  };

  /// <summary>Represents a node in the project tree with optional parent and children.</summary>
  public class ProjectNode : ITreeNodeWithParent<ProjectNode>
  {
    /// <inheritdoc/>
    public ProjectNode? ParentNode { get; set; }

    /// <summary>The project-node type represented by this node.</summary>
    public ProjectNodeType NodeType;

    /// <summary>Name of the project node.</summary>
    public string Name;

    /// <summary>Creation timestamp of the node.</summary>
    public DateTimeOffset CreationDate;

    /// <summary>Last modification timestamp of the node.</summary>
    public DateTimeOffset ModificationDate;

    /// <summary>True when this node is active.</summary>
    public bool IsActive;
    private List<ProjectNode>? _children = null;

    /// <summary>The object associated with this project node.</summary>
    public object? ValueAsObject;

    /// <inheritdoc/>
    IEnumerable<ProjectNode> ITreeNode<ProjectNode>.ChildNodes => (IEnumerable<ProjectNode>?)_children ?? Array.Empty<ProjectNode>();

    /// <summary>Gets the child nodes as a read-only list.</summary>
    public IReadOnlyList<ProjectNode> ChildNodes => (IReadOnlyList<ProjectNode>?)_children ?? Array.Empty<ProjectNode>();

    /// <summary>Creates a new <see cref="ProjectNode"/> with default values.</summary>
    public ProjectNode()
 : this("")
    {
    }

    /// <summary>Creates a new <see cref="ProjectNode"/> with the specified name.</summary>
    /// <param name="name">The node name.</param>
    public ProjectNode(string name)
  : this(name, ProjectNodeType.Folder)
    {
    }


    /// <summary>Creates a new <see cref="ProjectNode"/> with the specified name and type.</summary>
    /// <param name="name">The node name.</param>
    /// <param name="type">The node type.</param>
    public ProjectNode(string name, ProjectNodeType type)
      : this(name, type, DateTimeOffset.Now)
    {
    }


    /// <summary>Creates a new <see cref="ProjectNode"/> with creation date.</summary>
    /// <param name="name">The node name.</param>
    /// <param name="nodeType">The node type.</param>
    /// <param name="creationDate">The creation timestamp.</param>
    public ProjectNode(string name, ProjectNodeType nodeType, DateTimeOffset creationDate)
      : this(name, nodeType, creationDate, DateTimeOffset.Now)
    {
    }


    /// <summary>Creates a new <see cref="ProjectNode"/> with full metadata.</summary>
    /// <param name="name">The node name.</param>
    /// <param name="type">The node type.</param>
    /// <param name="creationDate">The creation timestamp.</param>
    /// <param name="modificationDate">The modification timestamp.</param>
    /// <param name="active">If <see langword="true"/>, the node is active.</param>
    public ProjectNode(string name, ProjectNodeType type, DateTimeOffset creationDate, DateTimeOffset modificationDate, bool active = false)
    {
      NodeType = type;
      Name = name;
      CreationDate = creationDate;
      ModificationDate = modificationDate;
      IsActive = active;
    }

    /// <summary>Appends a child node to this node and returns the appended child.</summary>
    /// <param name="node">Child node to append.</param>
    /// <returns>The appended child node.</returns>
    public ProjectNode AppendChild(ProjectNode node)
    {
      _children ??= new List<ProjectNode>();
      _children.Add(node);
      node.ParentNode = this;
      return node;
    }
  }

}
