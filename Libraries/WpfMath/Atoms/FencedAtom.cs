using System;
using WpfMath.Boxes;

namespace WpfMath.Atoms
{
  // Atom representing base atom surrounded by delimeters.
  internal class FencedAtom : Atom
  {
    private const int delimeterFactor = 901;
    private const double delimeterShortfall = 0.5;

    private static void CentreBox(Box box, double axis)
    {
      var totalHeight = box.Height + box.Depth;
      box.Shift = -(totalHeight / 2 - box.Height) - axis;
    }

    public FencedAtom(SourceSpan source, Atom baseAtom, SymbolAtom leftDelimeter, SymbolAtom rightDelimeter)
        : base(source)
    {
      BaseAtom = baseAtom ?? new RowAtom(null);
      LeftDelimeter = leftDelimeter;
      RightDelimeter = rightDelimeter;
    }

    public Atom BaseAtom { get; }

    internal SymbolAtom LeftDelimeter { get; }

    internal SymbolAtom RightDelimeter { get; }

    protected override Box CreateBoxCore(TexEnvironment environment)
    {
      var texFont = environment.MathFont;
      var style = environment.Style;

      // Create box for base atom.
      var baseBox = BaseAtom.CreateBox(environment);

      // Create result box.
      var resultBox = new HorizontalBox();
      var axis = texFont.GetAxisHeight(style);
      var delta = Math.Max(baseBox.Height - axis, baseBox.Depth + axis);
      var minHeight = Math.Max((delta / 500) * delimeterFactor, 2 * delta - delimeterShortfall);

      // Create and add box for left delimeter.
      if (LeftDelimeter != null && LeftDelimeter.Name != SymbolAtom.EmptyDelimiterName)
      {
        var leftDelimeterBox = DelimiterFactory.CreateBox(LeftDelimeter.Name, minHeight, environment);
        leftDelimeterBox.Source = LeftDelimeter.Source;
        CentreBox(leftDelimeterBox, axis);
        resultBox.Add(leftDelimeterBox);
      }

      // add glueElement between left delimeter and base Atom, unless base Atom is whitespace.
      if (!(BaseAtom is SpaceAtom))
        resultBox.Add(Glue.CreateBox(TexAtomType.Opening, BaseAtom.GetLeftType(), environment));

      // add box for base Atom.
      resultBox.Add(baseBox);

      // add glueElement between right delimeter and base Atom, unless base Atom is whitespace.
      if (!(BaseAtom is SpaceAtom))
        resultBox.Add(Glue.CreateBox(BaseAtom.GetRightType(), TexAtomType.Closing, environment));

      // Create and add box for right delimeter.
      if (RightDelimeter != null && RightDelimeter.Name != SymbolAtom.EmptyDelimiterName)
      {
        var rightDelimeterBox = DelimiterFactory.CreateBox(RightDelimeter.Name, minHeight, environment);
        rightDelimeterBox.Source = RightDelimeter.Source;
        CentreBox(rightDelimeterBox, axis);
        resultBox.Add(rightDelimeterBox);
      }

      return resultBox;
    }

    public override TexAtomType GetLeftType()
    {
      return TexAtomType.Opening;
    }

    public override TexAtomType GetRightType()
    {
      return TexAtomType.Closing;
    }
  }
}
