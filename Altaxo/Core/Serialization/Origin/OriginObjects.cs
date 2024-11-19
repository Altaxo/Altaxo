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



  public enum ValueType
  {
    Numeric = 0,
    Text = 1,
    Time = 2,
    Date = 3,
    Month = 4,
    Day = 5,
    ColumnHeading = 6,
    TickIndexedDataset = 7,
    TextNumeric = 9,
    Categorical = 10
  };



  // Numeric Format:
  // 1000 | 1E3 | 1k | 1,000
  public enum NumericFormat
  {
    Decimal = 0,
    Scientific = 1,
    Engineering = 2,
    DecimalWithMarks = 3
  };

  // Time Format:
  // hh:mm | hh | hh:mm:ss | hh:mm:ss.zz | hh ap | hh:mm ap | mm:ss
  // mm:ss.zz | hhmm | hhmmss | hh:mm:ss.zzz
  public enum TimeFormat
  {
    TIME_HH_MM = 0,
    TIME_HH = 1,
    TIME_HH_MM_SS = 2,
    TIME_HH_MM_SS_ZZ = 3,
    TIME_HH_AP = 4,
    TIME_HH_MM_AP = 5,
    TIME_MM_SS = 6,
    TIME_MM_SS_ZZ = 7,
    TIME_HHMM = 8,
    TIME_HHMMSS = 9,
    TIME_HH_MM_SS_ZZZ = 10
  };

  // Date Format:
  // dd/MM/yyyy | dd/MM/yyyy HH:mm | dd/MM/yyyy HH:mm:ss | dd.MM.yyyy | y. (year abbreviation) | MMM d
  // M/d | d | ddd | First letter of day | yyyy | yy | dd.MM.yyyy hh:mm | dd.MM.yyyy hh:mm:ss
  // yyMMdd | yyMMdd hh:mm | yyMMdd hh:mm:ss | yyMMdd hhmm | yyMMdd hhmmss | MMM
  // First letter of month | Quartal | M-d-yyyy (Custom1) | hh:mm:ss.zzzz (Custom2)
  public enum DateFormat
  {
    DATE_DD_MM_YYYY = -128,
    DATE_DD_MM_YYYY_HH_MM = -119,
    DATE_DD_MM_YYYY_HH_MM_SS = -118,
    DATE_DDMMYYYY = 0, DATE_Y = 1,
    DATE_MMM_D = 2,
    DATE_M_D = 3,
    DATE_D = 4,
    DATE_DDD = 5,
    DATE_DAY_LETTER = 6,
    DATE_YYYY = 7,
    DATE_YY = 8,
    DATE_DDMMYYYY_HH_MM = 9,
    DATE_DDMMYYYY_HH_MM_SS = 10,
    DATE_YYMMDD = 11,
    DATE_YYMMDD_HH_MM = 12,
    DATE_YYMMDD_HH_MM_SS = 13,
    DATE_YYMMDD_HHMM = 14,
    DATE_YYMMDD_HHMMSS = 15,
    DATE_MMM = 16,
    DATE_MONTH_LETTER = 17,
    DATE_Q = 18,
    DATE_M_D_YYYY = 19,
    DATE_HH_MM_SS_ZZZZ = 20
  };

  // Month Format:
  //  MMM | MMMM | First letter of month
  public enum MonthFormat
  {
    MONTH_MMM = 0,
    MONTH_MMMM = 1,
    MONTH_LETTER = 2
  };

  // ddd | dddd | First letter of day
  public enum DayOfWeekFormat
  {
    DAY_DDD = 0,
    DAY_DDDD = 1,
    DAY_LETTER = 2
  };

  public enum NumericDisplayType
  {
    DefaultDecimalDigits = 0,
    DecimalPlaces = 1,
    SignificantDigits = 2
  };

  public enum Attach
  {
    Frame = 0,
    Page = 1,
    Scale = 2,
    End_
  };

  public enum BorderType
  {
    BlackLine = 0,
    Shadow = 1,
    DarkMarble = 2,
    WhiteOut = 3,
    BlackOut = 4,
    None = -1
  };

  public enum FillPattern : byte
  {
    NoFill = 0,
    BDiagDense = 1,
    BDiagMedium = 2,
    BDiagSparse = 3,
    FDiagDense = 4,
    FDiagMedium = 5,
    FDiagSparse = 6,
    DiagCrossDense = 7,
    DiagCrossMedium = 8,
    DiagCrossSparse = 9,
    HorizontalDense = 10,
    HorizontalMedium = 11,
    HorizontalSparse = 12,
    VerticalDense = 13,
    VerticalMedium = 14,
    VerticalSparse = 15,
    CrossDense = 16,
    CrossMedium = 17,
    CrossSparse = 18
  };
  public enum ColorGradientDirection
  {
    NoGradient = 0,
    TopLeft = 1,
    Left = 2,
    BottomLeft = 3,
    Top = 4,
    Center = 5,
    Bottom = 6,
    TopRight = 7,
    Right = 8,
    BottomRight = 9
  };

  public enum ColorType : byte
  {
    None = 0,
    Automatic = 1,
    Regular = 2,
    Custom = 3,
    Increment = 4,
    Indexing = 5,
    RGB = 6,
    Mapping = 7
  };

  public enum RegularColor : byte
  {
    Black = 0,
    Red = 1,
    Green = 2,
    Blue = 3,
    Cyan = 4,
    Magenta = 5,
    Yellow = 6,
    DarkYellow = 7,
    Navy = 8,
    Purple = 9,
    Wine = 10,
    Olive = 11,
    DarkCyan = 12,
    Royal = 13,
    Orange = 14,
    Violet = 15,
    Pink = 16,
    White = 17,
    LightGray = 18,
    Gray = 19,
    LTYellow = 20,
    LTCyan = 21,
    LTMagenta = 22,
    DarkGray = 23,
    SpecialV7Axis = 0xF7
    /*, Custom = 255*/
  };

  public struct Color
  {
    public Color()
    {
    }

    public Color(ColorType ctype, RegularColor cregular)
    {
      ColorType = ctype;
      Regular = (byte)cregular;
    }

    public ColorType ColorType;
    // do not change the order
    public byte Regular; // also: custom[0]
    public byte Starting; // also: custom[1]
    public byte Column; // also: custom[2]

    public byte Custom0 { get => Regular; set => Regular = value; }
    public byte Custom1 { get => Starting; set => Starting = value; }
    public byte Custom2 { get => Column; set => Column = value; }
  };

  public struct Rect
  {
    public short Left;
    public short Top;
    public short Right;
    public short Bottom;

    public Rect(short width = 0, short height = 0)
    {
      Right = width;
      Bottom = height;
    }

    public int Height()
    {
      return Bottom - Top;
    }

    public int Width()
    {
      return Right - Left;
    }

    public bool IsValid()
    {
      return Height() > 0 && Width() > 0;
    }
  }

  public class ColorMapLevel
  {
    public Color FillColor;
    public byte FillPattern;
    public Color FillPatternColor;
    public double FillPatternLineWidth;

    public bool IsLineVisible;
    public Color LineColor;
    public byte LineStyle;
    public double LineWidth;
    public bool IsLabelVisible;
  };

  public class ColorMap
  {
    public bool IsFillEnabled;
    public ColorMapVector Levels = [];
  };

  public enum WindowState
  {
    Normal,
    Minimized,
    Maximized
  };
  public enum WindowTitle
  {
    Name,
    Label,
    Both
  };

  public class Window
  {
    public string Name;
    public string Label;
    public int ObjectID;
    public bool IsHidden;
    public WindowState State;
    public WindowTitle Title;
    public Rect FrameRect;
    public DateTimeOffset CreationDate;
    public DateTimeOffset ModificationDate;
    public ColorGradientDirection WindowBackgroundColorGradient;
    public Color WindowBackgroundColorBase;
    public Color WindowBackgroundColorEnd;

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

    public enum VType
    {
      V_Empty,
      V_DOUBLE,
      V_STRING
    };

    public VType ValueType()
    {
      return _type;
    }

    public Variant()
    {
    }
    public Variant(double d)
    {
      if (d == -1.23456789E-300)
        d = double.NaN;

      _double = d;
      _type = VType.V_DOUBLE;
    }
    public Variant(string s)
    {
      _string = s;
      _double = double.NaN;
      _type = VType.V_STRING;
    }

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

    public readonly bool IsEmpty => _type == VType.V_Empty;

    public readonly bool IsDouble => _type == VType.V_DOUBLE;
    public readonly bool IsDoubleOrNaN => _type == VType.V_DOUBLE || string.IsNullOrEmpty(_string);

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


    public readonly bool IsString => _type == VType.V_STRING;

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

  public enum SpreadColumnType
  {
    X,
    Y,
    Z,
    XErr,
    YErr,
    Label,
    Ignore,
    Group,
    Subject,
  };

  public class SpreadColumn
  {
    public string Name;
    public string DatasetName;
    public SpreadColumnType ColumnType;
    public ValueType ValueType;
    public int ValueTypeSpecification;
    public int SignificantDigits;
    public int DecimalPlaces;
    public NumericDisplayType NumericDisplayType;
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

    public int Width;
    public int Index;
    public int ColIndex;
    public int Sheet;
    public int NumRows;
    public int BeginRow;
    public int EndRow;
    public List<Variant> Data;

    /// <summary>
    /// The imaginary data. Is only not null if
    /// the data are complex.
    /// </summary>
    public List<double>? ImaginaryData;

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

  public class SpreadSheet : Window
  {
    public int MaxRows;
    public bool Loose;
    public int Sheets;
    public List<SpreadColumn> Columns;

    public SpreadSheet(string name = "")
        : base(name)
    {
      MaxRows = 30;
      Loose = true;
      Sheets = 1;
      Columns = [];
    }
  }

  public class Excel : Window
  {
    public int MaxRows;
    public bool Loose;
    public List<SpreadSheet> Sheets = [];

    public Excel(string name = "", string label = "", int maxRows = 0, bool hidden = false, bool loose = true)
        : base(name, label, hidden)
    {
      MaxRows = maxRows;
      Loose = loose;
    }
  }

  public class MatrixSheet
  {
    public enum ViewType { DataView, ImageView };

    public string Name;
    public short RowCount;
    public short ColumnCount;
    public int ValueTypeSpecification;
    public int SignificantDigits;
    public int DecimalPlaces;
    public NumericDisplayType NumericDisplayType;
    public string Command;
    public short Width;
    public int Index;
    public ViewType View;
    public ColorMap ColorMap;
    public List<double> Data = [];
    public List<double> ImaginaryData = [];

    /// <summary>The x-value that corresponds to the first column of the matrix.</summary>
    public double X1 { get; set; }

    /// <summary>The x-value that corresponds to the last column of the matrix.</summary>
    public double X2 { get; set; }

    /// <summary>The y-value that corresponds to the first row of the matrix.</summary>
    public double Y1 { get; set; }

    /// <summary>The y-value that corresponds to the last row of the matrix.</summary>
    public double Y2 { get; set; }

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

    public double this[int r, int c]
    {
      get
      {
        int i = r * ColumnCount + c;
        return i < Data.Count ? Data[i] : double.NaN;
      }
    }

    public double ImaginaryPart(int r, int c)
    {
      int i = r * ColumnCount + c;
      return i < ImaginaryData.Count ? ImaginaryData[i] : double.NaN;
    }
  }

  public class Matrix : Window
  {
    public enum HeaderViewType
    {
      ColumnRow,
      XY
    };

    public int ActiveSheet;
    public HeaderViewType Header;
    public List<MatrixSheet> Sheets = [];

    public Matrix(string name = "")
        : base(name)
    {
      ActiveSheet = 0;
      Header = HeaderViewType.ColumnRow;
    }
  }

  public class Function
  {
    public enum FunctionType
    {
      Normal,
      Polar
    };

    public string Name;
    public FunctionType Type;
    public string Formula;
    public double Begin;
    public double End;
    public int TotalPoints;
    public int Index;

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

  public class TextBox
  {
    public string Text;
    public Rect ClientRect;
    public Color Color;
    public short FontSize;
    public int Rotation;
    public int Tab;
    public BorderType BorderType;
    public Attach Attach;
    public bool IsShown;

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

  public class PieProperties
  {
    public byte ViewAngle;
    public byte Thickness;
    public bool ClockwiseRotation;
    public short Rotation;
    public short Radius;
    public short HorizontalOffset;
    public int DisplacedSectionCount; // maximum - 32 sections
    public short Displacement;

    //labels
    public bool FormatAutomatic;
    public bool FormatValues;
    public bool FormatPercentages;
    public bool FormatCategories;
    public bool PositionAssociate;
    public short Distance;

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

  public class VectorProperties
  {
    public enum VectorPosition
    {
      Tail,
      Midpoint,
      Head
    };

    public Color Color;
    public double Width;
    public short ArrowLength;
    public byte ArrowAngle;
    public bool ArrowClosed;
    public string EndXColumnName;
    public string EndYColumnName;

    public VectorPosition Position;
    public string AngleColumnName;
    public string MagnitudeColumnName;
    public float Multiplier;
    public int ConstAngle;
    public int ConstMagnitude;

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

  public class TextProperties
  {
    public enum TextJustify
    {
      Left,
      Center,
      Right
    };

    public Color Color;
    public bool FontBold;
    public bool FontItalic;
    public bool FontUnderline;
    public bool WhiteOut;
    public TextJustify Justify;
    public short Rotation;
    public short XOffset;
    public short YOffset;
    public short FontSize;
  }

  public enum SurfaceType
  {
    ColorMap3D,
    ColorFill,
    WireFrame,
    Bars
  };

  public enum SurfaceGrids
  {
    None,
    X,
    Y,
    XY
  };

  public class SurfaceProperties
  {
    public class SurfaceColoration
    {
      public bool Fill;
      public bool Contour;
      public Color LineColor;
      public double LineWidth;
    }



    public byte Type;
    public SurfaceGrids Grids;
    public double GridLineWidth;
    public Color GridColor;

    public bool BackColorEnabled;
    public Color FrontColor;
    public Color BackColor;

    public bool SideWallEnabled;
    public Color XSideWallColor;
    public Color YSideWallColor;

    public SurfaceColoration Surface
    {
      get { return field ??= new(); }
      set { field = value; }
    }

    public SurfaceColoration TopContour
    {
      get { return field ??= new(); }
      set { field = value; }
    }
    public SurfaceColoration BottomContour
    {
      get { return field ??= new(); }
      set { field = value; }
    }

    public ColorMap ColorMap
    {
      get { return field ??= new(); }
      set { field = value; }
    }
  }

  public class PercentileProperties
  {
    public byte MaxSymbolType;
    public byte P99SymbolType;
    public byte MeanSymbolType;
    public byte P1SymbolType;
    public byte MinSymbolType;
    public Color SymbolColor;
    public Color SymbolFillColor;
    public short SymbolSize;
    public byte BoxRange;
    public byte WhiskersRange;
    public double BoxCoeff;
    public double WhiskersCoeff;
    public bool DiamondBox;
    public byte Labels;

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

  public enum LineStyle
  {
    Solid = 0,
    Dash = 1,
    Dot = 2,
    DashDot = 3,
    DashDotDot = 4,
    ShortDash = 5,
    ShortDot = 6,
    ShortDashDot = 7
  }

  public enum LineConnect
  {
    NoLine = 0,
    Straight = 1,
    TwoPointSegment = 2,
    ThreePointSegment = 3,
    BSpline = 8,
    Spline = 9,
    StepHorizontal = 11,
    StepVertical = 12,
    StepHCenter = 13,
    StepVCenter = 14,
    Bezier = 15
  }

  public class GraphCurve
  {
    public enum Plot
    {
      Scatter3D = 101,
      Surface3D = 103,
      Vector3D = 183,
      ScatterAndErrorBar3D = 184,
      TernaryContour = 185,
      PolarXrYTheta = 186,
      SmithChart = 191,
      Polar = 192,
      BubbleIndexed = 193,
      BubbleColorMapped = 194,
      Line = 200,
      Scatter = 201,
      LineSymbol = 202,
      Column = 203,
      Area = 204,
      HiLoClose = 205,
      Box = 206,
      ColumnFloat = 207,
      Vector = 208,
      PlotDot = 209,
      Wall3D = 210,
      Ribbon3D = 211,
      Bar3D = 212,
      ColumnStack = 213,
      AreaStack = 214,
      Bar = 215,
      BarStack = 216,
      FlowVector = 218,
      Histogram = 219,
      MatrixImage = 220,
      Pie = 225,
      Contour = 226,
      Unknown = 230,
      ErrorBar = 231,
      TextPlot = 232,
      XErrorBar = 233,
      SurfaceColorMap = 236,
      SurfaceColorFill = 237,
      SurfaceWireframe = 238,
      SurfaceBars = 239,
      Line3D = 240,
      Text3D = 241,
      Mesh3D = 242,
      XYZContour = 243,
      XYZTriangular = 245,
      LineSeries = 246,
      YErrorBar = 254,
      XYErrorBar = 255
    }



    public bool IsHidden;
    public GraphCurve.Plot PlotType;
    public string DataName;
    public string XDataName;
    public string XColumnName;
    public string YColumnName;
    public string ZColumnName;
    public Color LineColor;
    public byte LineTransparency;
    public byte LineStyle;
    public byte LineConnect;
    public byte BoxWidth;
    public double LineWidth;

    public bool FillArea;
    public byte FillAreaType;
    public byte FillAreaPattern;
    public Color FillAreaColor;
    public byte FillAreaTransparency;
    public bool FillAreaWithLineTransparency;
    public Color FillAreaPatternColor;
    public double FillAreaPatternWidth;
    public byte FillAreaPatternBorderStyle;
    public Color FillAreaPatternBorderColor;
    public double FillAreaPatternBorderWidth;

    public byte SymbolInterior;
    public byte SymbolShape;
    public Color SymbolColor;
    public Color SymbolFillColor;
    public byte SymbolFillTransparency;
    public double SymbolSize;
    public byte SymbolThickness;
    public byte PointOffset;

    public bool ConnectSymbols;

    //pie
    public PieProperties? Pie
    {
      get { return field ??= new(); }
      set { field = value; }
    }

    //vector
    public VectorProperties? Vector
    {
      get { return field ??= new(); }
      set { field = value; }
    }

    //text
    public TextProperties? Text
    {
      get { return field ??= new(); }
      set { field = value; }
    }

    //surface
    public SurfaceProperties? Surface
    {
      get { return field ??= new(); }
      set { field = value; }
    }

    //contour
    public ColorMap? ColorMap
    {
      get { return field ??= new(); }
      set { field = value; }
    }
  }

  public class GraphAxisBreak
  {
    public bool Show;
    public bool Log10;
    public double From;
    public double To;
    public double Position;
    public double ScaleIncrementBefore;
    public double ScaleIncrementAfter;
    public byte MinorTicksBefore;
    public byte MinorTicksAfter;

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
  public class GraphGrid
  {
    public bool Hidden;
    public byte Color;
    public byte Style;
    public double Width;
  }

  public class GraphAxisFormat
  {
    public bool IsHidden;
    public byte Color;
    public double Thickness;
    public double MajorTickLength;
    public int MajorTicksType;
    public int MinorTicksType;
    public int AxisPosition;
    public double AxisPositionValue;
    public TextBox Label;
    public string Prefix;
    public string Suffix;
    public string Factor;
  }

  public class GraphAxisTick
  {
    public bool ShowMajorLabels;
    public byte Color;
    public ValueType ValueType;
    public int ValueTypeSpecification;
    public int DecimalPlaces;
    public short FontSize;
    public bool FontBold;
    public string DataName;
    public string ColumnName;
    public int Rotation;
  }

  public class GraphAxis
  {
    public enum AxisPosition
    {
      Left = 0,
      Bottom,
      Right,
      Top,
      Front,
      Back
    }
    public enum AxisScale
    {
      Linear = 0,
      Log10 = 1,
      Probability = 2,
      Probit = 3,
      Reciprocal = 4,
      OffsetReciprocal = 5,
      Logit = 6,
      Ln = 7,
      Log2 = 8
    }

    public AxisPosition Position;
    public bool ZeroLine;
    public bool OppositeLine;
    public double Min;
    public double Max;
    public double Step;
    public double Anchor;
    public byte MajorTicks;
    public byte MinorTicks;
    public byte Scale;
    public GraphGrid MajorGrid;
    public GraphGrid MinorGrid;
    public GraphAxisFormat[] FormatAxis; // Assuming size of 2
    public GraphAxisTick[] TickAxis; // Assuming size of 2

    public GraphAxis()
    {
      Position = new AxisPosition();
      MajorGrid = new GraphGrid();
      MinorGrid = new GraphGrid();
      FormatAxis = new GraphAxisFormat[2] { new GraphAxisFormat(), new GraphAxisFormat() };
      TickAxis = new GraphAxisTick[2] { new GraphAxisTick(), new GraphAxisTick() };
    }
  }

  public enum FigureType
  {
    Rectangle,
    Circle
  }

  public class Figure
  {
    public FigureType FigureType;
    public Rect ClientRect;
    public Attach Attach;
    public Color Color;
    public byte Style;
    public double Width;
    public Color FillAreaColor;
    public FillPattern FillAreaPattern;
    public Color FillAreaPatternColor;
    public double FillAreaPatternWidth;
    public bool UseBorderColor;

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

  public class LineVertex
  {
    public byte ShapeType;
    public double ShapeWidth;
    public double ShapeLength;
    public double X;
    public double Y;

    public LineVertex()
    {
      ShapeType = 0;
      ShapeWidth = 0;
      ShapeLength = 0;
      X = 0;
      Y = 0;
    }
  }

  public class Line
  {
    public Rect ClientRect;
    public Color Color;
    public Attach Attach;
    public double Width;
    public byte Style;
    public LineVertex Begin;
    public LineVertex End;
  }

  public class Bitmap
  {
    public Rect ClientRect;
    public Attach Attach;
    public long Size;
    public string WindowName;
    public BorderType BorderType;
    public byte[] Data;

    public Bitmap(string name = "")
    {
      Attach = Attach.Frame;
      Size = 0;
      WindowName = name;
      BorderType = BorderType.BlackLine;
      Data = null;
    }

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

  public class ColorScale
  {
    public bool IsVisible;
    public bool ReverseOrder;
    public short LabelGap;
    public short ColorBarThickness;
    public Color LabelsColor;

    public ColorScale()
    {
      IsVisible = true;
      ReverseOrder = false;
      LabelGap = 5;
      ColorBarThickness = 3;
      LabelsColor = new Color(ColorType.Regular, RegularColor.Black);
    }
  }

  public struct GraphLayer
  {
    public Rect ClientRect;
    public TextBox Legend;
    public Color BackgroundColor;
    public BorderType BorderType;

    public GraphAxis XAxis;
    public GraphAxis YAxis;
    public GraphAxis ZAxis;

    public GraphAxisBreak XAxisBreak;
    public GraphAxisBreak YAxisBreak;
    public GraphAxisBreak ZAxisBreak;

    public double HistogramBin;
    public double HistogramBegin;
    public double HistogramEnd;

    public PercentileProperties Percentile;
    public ColorScale ColorScale;
    public ColorMap ColorMap;

    public List<TextBox> Texts;
    public List<TextBox> PieTexts;
    public List<Line> Lines;
    public List<Figure> Figures;
    public List<Bitmap> Bitmaps;
    public List<GraphCurve> Curves;

    public float XAngle;
    public float YAngle;
    public float ZAngle;

    public float XLength;
    public float YLength;
    public float ZLength;

    public int ImageProfileTool;
    public double VLine;
    public double HLine;

    public bool IsWaterfall;
    public double XOffset;
    public double YOffset;

    public bool GridOnTop;
    public bool ExchangedAxes;
    public bool IsXYY3D;
    public bool Orthographic3D;

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

  public class GraphLayerRange
  {
    public double Min;
    public double Max;
    public double Step;

    public GraphLayerRange(double _min = 0.0, double _max = 0.0, double _step = 0.0)
    {
      Min = _min;
      Max = _max;
      Step = _step;
    }
  }

  public class Graph : Window
  {
    public List<GraphLayer> Layers;
    public short Width;
    public short Height;
    public bool Is3D;
    public bool IsLayout;
    public bool ConnectMissingData;
    public string TemplateName;

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

  public class Note : Window
  {
    public string Text;

    public Note(string name = "")
        : base(name)
    {
    }
  }

  public enum ProjectNodeType
  {
    SpreadSheet,
    Matrix,
    Excel,
    Graph,
    Graph3D,
    Note,
    Folder
  };

  public class ProjectNode : ITreeNodeWithParent<ProjectNode>
  {
    public ProjectNode? ParentNode { get; set; }
    public ProjectNodeType NodeType;
    public string Name;
    public DateTimeOffset CreationDate;
    public DateTimeOffset ModificationDate;
    public bool IsActive;
    private List<ProjectNode>? _children = null;
    public object? ValueAsObject;

    IEnumerable<ProjectNode> ITreeNode<ProjectNode>.ChildNodes => (IEnumerable<ProjectNode>?)_children ?? Array.Empty<ProjectNode>();

    public IReadOnlyList<ProjectNode> ChildNodes => (IReadOnlyList<ProjectNode>?)_children ?? Array.Empty<ProjectNode>();

    public ProjectNode()
 : this("")
    {
    }

    public ProjectNode(string name)
  : this(name, ProjectNodeType.Folder)
    {
    }


    public ProjectNode(string name, ProjectNodeType type)
      : this(name, type, DateTimeOffset.Now)
    {
    }


    public ProjectNode(string name, ProjectNodeType nodeType, DateTimeOffset creationDate)
      : this(name, nodeType, creationDate, DateTimeOffset.Now)
    {
    }


    public ProjectNode(string name, ProjectNodeType type, DateTimeOffset creationDate, DateTimeOffset modificationDate, bool active = false)
    {
      NodeType = type;
      Name = name;
      CreationDate = creationDate;
      ModificationDate = modificationDate;
      IsActive = active;
    }

    public ProjectNode AppendChild(ProjectNode node)
    {
      _children ??= new List<ProjectNode>();
      _children.Add(node);
      node.ParentNode = this;
      return node;
    }
  }

}
