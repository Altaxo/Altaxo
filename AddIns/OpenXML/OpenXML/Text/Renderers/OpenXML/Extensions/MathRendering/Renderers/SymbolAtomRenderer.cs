#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Math;
using WpfMath.Atoms;

namespace Altaxo.Text.Renderers.OpenXML.Extensions.MathRendering.Renderers
{
  /// <summary>
  /// Renderer for <see cref="SymbolAtom"/> objects, like operator symbols, greek characters,
  /// and special symbols.
  /// </summary>
  /// <seealso cref="Altaxo.Text.Renderers.OpenXML.Extensions.MathRendering.OpenXMLAtomRenderer{WpfMath.Atoms.SymbolAtom}" />
  internal class SymbolAtomRenderer : OpenXMLAtomRenderer<SymbolAtom>
  {
    protected override void Write(OpenXMLWpfMathRenderer renderer, SymbolAtom item)
    {
      bool runCreated = false;
      var run = renderer.Peek() as Run;

      if (run == null)
      {
        runCreated = true;
        run = (Run)renderer.Push(new Run());
      }

      if (!_nameToSymbol.TryGetValue(item.Name, out var textString))
        textString = item.Name;

      var text = new DocumentFormat.OpenXml.Math.Text() { Text = textString };
      run.AppendChild(text);


      if (runCreated)
        renderer.PopTo(run);

    }

    public static bool TryConvert(string symbolName, out string textString)
    {
      var result = _nameToSymbol.TryGetValue(symbolName, out textString);

      if (!result)
        textString = symbolName;

      return result;
    }


    private static Dictionary<string, string> _nameToSymbol = new Dictionary<string, string>()
    {
      // miscellaneous
      ["%"] = "%",
      ["comma"] = ",",
      ["ldotp"] = ".",
      ["cdotp"] = "·",
      ["normaldot"] = ".",
      ["slash"] = "/",
      //  < !--this symbol is not defined in "TeXSymbols.xml", because it's not meant to be used as a symbol. Use "surd" instead. -->
      ["sqrt"] = "\u221A",

      ["semicolon"] = ";",
      ["faculty"] = "!",
      ["question"] = "?",

      // < !--math accents-- >
      // see https://de.wikipedia.org/wiki/Unicodeblock_Kombinierende_diakritische_Zeichen

      ["acute"] = "\u0301",
      ["grave"] = "\u0300",
      ["ddot"] = "\u0308",
      ["tilde"] = "\u0303",
      ["bar"] = "\u0304",
      ["breve"] = "\u0306",
      ["check"] = "\u030C",
      ["hat"] = "\u0302",
      ["vec"] = "\u20D7",
      ["dot"] = "\u0307",
      ["widehat"] = "\u0361", // Word renders a widehat only if the AccentChar instance is null (this is handled in AccentedAtomRenderer)
      ["widetilde"] = "\u0303", // in literature this is \u0360, but for Word it seems to be \u0303

      // < !--delimiters that can change size-- >
      ["("] = "(",
      [")"] = ")",
      ["lbrace"] = "{",
      ["rbrace"] = "}",
      ["lbrack"] = "[",
      ["rbrack"] = "]",
      ["rsqbrack"] = "]",
      ["lsqbrack"] = "[",
      ["langle"] = "\u2329",
      ["rangle"] = "\u232A",
      ["lfloor"] = "\u230A",
      ["rfloor"] = "\u230B",
      ["lceil"] = "\u2308",
      ["rceil"] = "\u2309",
      ["uparrow"] = "\u2191",
      ["Uparrow"] = "\u21D1",
      ["downarrow"] = "\u2193",
      ["Downarrow"] = "\u21D3",
      ["updownarrow"] = "\u2195",
      ["Updownarrow"] = "\u21D5",
      ["vert"] = "\u007C",
      ["Vert"] = "\u2016",
      ["\u007C"] = "\u2016", // note \| is parsed in WpfMath to SymbolAtom |, so when we render a symbol atom with |, we need a double vertical line
      // ["slashdel" ] = "",

      //lowercase greek letters
      ["alpha"] = "α",
      ["beta"] = "β",
      ["gamma"] = "γ",
      ["delta"] = "δ",
      ["epsilon"] = "ϵ",
      ["varepsilon"] = "ε",
      ["zeta"] = "ζ",
      ["eta"] = "η",
      ["theta"] = "θ",
      ["vartheta"] = "ϑ",
      ["iota"] = "ι",
      ["kappa"] = "κ",
      ["lambda"] = "λ",
      ["mu"] = "μ",
      ["nu"] = "ν",
      ["xi"] = "ξ",
      ["omicron"] = "ο",
      ["pi"] = "π",
      ["varpi"] = "ϖ",
      ["rho"] = "ρ",
      ["varrho"] = "ϱ",
      ["sigma"] = "σ",
      ["varsigma"] = "ς",
      ["tau"] = "τ",
      ["upsilon"] = "υ",
      ["phi"] = "φ",
      ["varphi"] = "ϕ",
      ["chi"] = "χ",
      ["psi"] = "ψ",
      ["omega"] = "ω",

      // greek uppercase letters
      ["Alpha"] = "Α",
      ["Beta"] = "Β",
      ["Gamma"] = "Γ",
      ["Delta"] = "Δ",
      ["Epsilon"] = "Ε",
      ["Zeta"] = "Ζ",
      ["Eta"] = "Η",
      ["Theta"] = "Θ",
      ["Iota"] = "Ι",
      ["Kappa"] = "Κ",
      ["Lambda"] = "Λ",
      ["Mu"] = "Μ",
      ["Nu"] = "Ν",
      ["Xi"] = "Ξ",
      ["Omicron"] = "Ο",
      ["Pi"] = "Π",
      ["Rho"] = "Ρ",
      ["Sigma"] = "Σ",
      ["Tau"] = "Τ",
      ["Upsilon"] = "Υ",
      ["Phi"] = "Φ",
      ["Chi"] = "Χ",
      ["Psi"] = "Ψ",
      ["Omega"] = "Ω",


      // <!--miscellaneous symbols of type "ord"-- >
      ["aleph"] = "ℵ",
      ["imath"] = "ı",
      ["jmath"] = "ȷ",
      ["ell"] = "ℓ",
      ["wp"] = "℘",
      ["Re"] = "ℜ",
      ["Im"] = "ℑ",
      ["partial"] = "∂",
      ["infty"] = "∞",
      ["prime"] = "′",
      ["emptyset"] = "∅",
      ["nabla"] = "∇",
      ["surdsign"] = "√",
      ["top"] = "⊤",
      ["bot"] = "⊥",
      ["triangle"] = "△",
      ["forall"] = "∀",
      ["exists"] = "∃",
      ["neg"] = "¬",
      ["lnot"] = "¬",
      ["flat"] = "\\u266d",
      ["natural"] = "\\u266e",
      ["sharp"] = "\\u266f",
      ["clubsuit"] = "\\u2667",
      ["diamondsuit"] = "\\u2662",
      ["heartsuit"] = "\\u2661",
      ["spadesuit"] = "\\u2664",
      ["lacc"] = "{",
      ["racc"] = "}",

      // < !--"large" operators-- >

      ["bigcap"] = "\u22C2",
      ["bigcup"] = "\u22C3",
      ["bigodot"] = "\u2A00",
      ["bigoplus"] = "\u2A01",
      ["bigotimes"] = "\u2A02",
      ["bigsqcup"] = "\u2A06",
      ["biguplus"] = "\u2A04",
      ["bigvee"] = "\u22C1",
      ["bigwedge"] = "\u22C0",
      ["coprod"] = "\u2210",
      ["int"] = "\u222B",
      ["oint"] = "\u222E",
      ["sum"] = "\u2211",
      ["prod"] = "\u220F",
      ["smallint"] = "\u222B", // TODO no Unicode char found, using int instead
      ["iint"] = "\u222C",
      ["iiint"] = "\u222D",
      ["iiiint"] = "\u222C",

      // < !--binary operations-- >

      ["minus"] = "-",
      ["plus"] = "+",
      ["pm"] = "\u00B1",
      ["mp"] = "\u2213",
      ["setminus"] = "\u29F5",
      ["cdot"] = "\u22C5",
      ["times"] = "\u00D7",
      ["ast"] = "\u2217",
      ["star"] = "\u22C6",
      ["diamond"] = "\u22C4",
      ["circ"] = "\u2218",
      ["bullet"] = "\u2219",
      ["div"] = "\u00F7",
      ["cap"] = "\u2229",
      ["cup"] = "\u222A",
      ["uplus"] = "\u228E",
      ["sqcap"] = "\u2293",
      ["sqcup"] = "\u2294",
      ["triangleleft"] = "\u22B2",
      ["triangleright"] = "\u22B3",
      ["wr"] = "\u2240",
      ["bigcirc"] = "\u25EF",
      ["bigtriangleup"] = "\u25B3",
      ["bigtriangledown"] = "\u25BD",
      ["vee"] = "\u2228",
      ["lor"] = "\u2228",
      ["wedge"] = "\u2227",
      ["land"] = "\u2227",
      ["oplus"] = "\u2295",
      ["ominus"] = "\u2296",
      ["otimes"] = "\u2297",
      ["oslash"] = "\u2298",
      ["odot"] = "\u2299",
      ["dagger"] = "\u2020",
      ["ddagger"] = "\u2021",
      ["amalg"] = "\u2A3F",

      // < !--relations-- >

      ["equals"] = "=",
      ["gt"] = ">",
      ["lt"] = "<",
      ["leq"] = "\u2264",
      ["le"] = "\u2264",
      ["prec"] = "\u227A",
      ["preceq"] = "\u2AAF",
      ["ll"] = "\u226A",
      ["subset"] = "\u2282",
      ["subseteq"] = "\u2286",
      ["sqsubseteq"] = "\u2291",
      ["in"] = "\u2208",
      ["vdash"] = "\u22A2",
      ["smile"] = "\u2323",
      ["frown"] = "\u2322",
      ["geq"] = "\u2265",
      ["ge"] = "\u2265",
      ["succ"] = "\u227B",
      ["succeq"] = "\u2AB0",
      ["gg"] = "\u226B",
      ["supset"] = "\u2283",
      ["supseteq"] = "\u2286",
      ["sqsupseteq"] = "\u2291",
      ["ni"] = "\u220B",
      ["owns"] = "\u220B",
      ["dashv"] = "\u22A3",
      ["mid"] = "\u2223",
      ["parallel"] = "\u2225",
      ["equiv"] = "\u2261",
      ["sim"] = "\u223C",
      ["simeq"] = "\u2243",
      ["asymp"] = "\u224D",
      ["approx"] = "\u2248",
      ["propto"] = "\u221D",
      ["perp"] = "\u27C2",



      // < !--special relation symbol with "width=0"(to overlap other relational symbols)-- >
      ["not"] = "\u0338",


      // < !--arrows = pointing relations-- >

      ["colon"] = "\u003A",
      ["nearrow"] = "\u2197",
      ["searrow"] = "\u2198",
      ["swarrow"] = "\u2199",
      ["nwarrow"] = "\u2196",
      ["leftarrow"] = "\u2190",
      ["gets"] = "\u2190",
      ["Leftarrow"] = "\u21D0",
      ["rightarrow"] = "\u2192",
      ["to"] = "\u2192",
      ["Rightarrow"] = "\u21D2",
      ["leftrightarrow"] = "\u2194",
      ["Leftrightarrow"] = "\u21D4",
      ["leftharpoonup"] = "\u21BC",
      ["leftharpoondown"] = "\u21BD",
      ["rightharpoonup"] = "\u21C0",
      ["rightharpoondown"] = "\u21C1",

    };
  }
}
