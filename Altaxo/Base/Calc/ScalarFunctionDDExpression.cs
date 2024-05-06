#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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

using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Altaxo.Calc
{
  /// <summary>
  /// A scalar expression given as a string which takes a single <see cref="System.Double"/> value as argument and returns a <see cref="System.Double"/> value.
  /// </summary>
  /// <seealso cref="Altaxo.Calc.IScalarFunctionDD" />
  /// <remarks>
  /// <para>System.Math is in the usings, thus functions like Sqrt, Sin, Abs can be used without prepending System.Math.</para>
  /// <para>References: <see href="https://github.com/dotnet/roslyn/blob/main/docs/wiki/Scripting-API-Samples.md"/></para>
  /// </remarks>
  public record ScalarFunctionDDExpression : IScalarFunctionDD, Main.IImmutable
  {
    /// <summary>
    /// Arguments of the expression (must be made public for the script to be used).
    /// </summary>
    public class Arguments
    {
      /// <summary>
      /// The expression has only one single argument named 'x'.
      /// </summary>
      public double x;
    }

    /// <summary>
    /// Gets the expression.
    /// </summary>
    public string Expression { get; }

    /// <summary>
    /// Gets the precompiled script.
    /// </summary>
    private Script<double> _script;

    /// <summary>
    /// The argument is made thread-local in order to avoid allocation during repeated evaluations.
    /// </summary>
    private ThreadLocal<Arguments> _arguments = new(() => new Arguments());


    #region Serialization

    /// <summary>
    /// V0: 2024-05-05
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ScalarFunctionDDExpression), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ScalarFunctionDDExpression)obj;
        info.AddValue("Expression", s.Expression);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var expression = info.GetString("Expression");
        return new ScalarFunctionDDExpression(expression);
      }
    }
    #endregion


    public ScalarFunctionDDExpression(string expression)
    {
      if (string.IsNullOrEmpty(expression))
        throw new System.ArgumentNullException(nameof(expression));

      Expression = expression;
      var options = ScriptOptions.Default.WithImports("System.Math");
      _script = CSharpScript.Create<double>(expression, options, globalsType: typeof(Arguments));
      _script.Compile();
    }

    public double Evaluate(double x)
    {
      var arguments = this._arguments.Value;
      arguments!.x = x;
      return _script.RunAsync((object)arguments, CancellationToken.None).Result.ReturnValue;
    }
  }
}
