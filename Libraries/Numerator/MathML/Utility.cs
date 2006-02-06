//This file is part of the gNumerator MathML DOM library, a complete 
//implementation of the w3c mathml dom specification
//Copyright (C) 2003, Andy Somogyi
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//For details, see http://numerator.sourceforge.net, or send mail to
//andy@epsilon3.net

using System;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Globalization;

namespace MathML
{
	/// <summary>
	/// general utility functions
	/// </summary>
	internal class Utility
	{
		static readonly String[] forms = 
		{
			"prefix", "infix", "postfix"
		};


		/// <summary>
		/// converts a string in the form of "#rgb" or "#rrggbb" into a 
		/// system color. If the string is ill formed, Color.Empty is returned
		/// also deal with string of format '#rgb', here r becomes rr, g becomes gg, 
		/// and b becomes bb
		/// </summary>
		internal static Color ParseColor(String s)
		{
			if(s.Length == 0)
			{
				return Color.Empty;
			}
			if(s.Length == 4 && s[0] == '#')
			{
				byte r = Byte.Parse(s.Substring(1,1), NumberStyles.HexNumber);
				byte g = Byte.Parse(s.Substring(2,1), NumberStyles.HexNumber);
				byte b = Byte.Parse(s.Substring(3,1), NumberStyles.HexNumber);
				// shift left 4 places (1 hex digit = 4 binary digits), and add
				// the digit back on the right
				return Color.FromArgb((r << 4) + r, (g << 4) + g, (b << 4) + b);
			}
			else if(s.Length == 7 && s[0] == '#')
			{
				byte r = Byte.Parse(s.Substring(1,2), NumberStyles.HexNumber);
				byte g = Byte.Parse(s.Substring(3,2), NumberStyles.HexNumber);
				byte b = Byte.Parse(s.Substring(5,2), NumberStyles.HexNumber);
				return Color.FromArgb(r, g, b);
			}
			else
			{
				return Color.FromName(s);
			}
		}

		internal static int ParseInt(String s, int deFault)
		{
			try
			{
				if (s.Length == 0)
				{
					return deFault;
				}
				else
				{
					return int.Parse(s);
				}
			}
			catch(Exception)
			{
				return deFault;
			}
		}

		internal static ushort ParseUnsignedShort(String s, ushort deFault)
		{
			try
			{
				if (s.Length == 0)
				{
					return deFault;
				}
				else
				{
					return ushort.Parse(s);
				}
			}
			catch(Exception)
			{
				return deFault;
			}
		}

		internal static bool ParseBool(String s, bool deFault)
		{
			try
			{
				if (s.Length == 0)
				{
					return deFault;
				}
				else
				{
					return bool.Parse(s);
				}
			}
			catch(Exception)
			{
				return deFault;
			}
		}

		internal static Display ParseDisplay(String s, Display deFault)
		{
			if(s == "block")
			{
				return Display.Block;
			}
			else if(s == "inline")
			{
				return Display.Block;
			}
			else
			{
				return deFault;
			}
		}

		internal static Form ParseForm(String s, Form deFault)
		{
			if(s == "infix")
			{
				return Form.Infix;
			}
			else if(s == "prefix")
			{
				return Form.Prefix;
			}
			else if(s == "postfix")
			{
				return Form.Postfix;
			}
			else
			{
				return deFault;
			}
		}

		internal static String UnparseForm(Form f)
		{
			return forms[(int)f];
		}

		internal static Length ParseLength(String s, Length def)
		{
			Length result = def;
			try
			{
				if(s.Length > 0)
				{
					if(s.EndsWith("%"))
					{
						result.Value = Int32.Parse(s.TrimEnd('%'));
						result.Type = LengthType.Percentage;
					}
					else if(Char.IsDigit(s[0]) && Char.IsDigit(s[s.Length - 1]))
					{
						result.Value = Int32.Parse(s);
						result.Type = LengthType.Percentage;
					}
					else if(s.EndsWith("em"))
					{
						result.Value = Single.Parse(s.TrimEnd(em));
						result.Type = LengthType.Em;
					}					
					else if(s.EndsWith("px"))
					{
						result.Value = Single.Parse(s.TrimEnd(px));
						result.Type = LengthType.Px;
					}
					else if(s.EndsWith("in"))
					{
						result.Value = Single.Parse(s.TrimEnd(inch));
						result.Type = LengthType.In;
					}
					else if(s.EndsWith("cm"))
					{
						result.Value = Single.Parse(s.TrimEnd(cm));
						result.Type = LengthType.Cm;
					}
					else if(s.EndsWith("mm"))
					{
						result.Value = Single.Parse(s.TrimEnd(mm));
						result.Type = LengthType.Mm;
					}
					else if(s.EndsWith("pt"))
					{
						result.Value = Single.Parse(s.TrimEnd(pt));
						result.Type = LengthType.Pt;
					}
					else if(s.EndsWith("pc"))
					{
						result.Value = Single.Parse(s.TrimEnd(pc));
						result.Type = LengthType.Pc;
					}
					else if(s.EndsWith("ex"))
					{
						result.Value = Single.Parse(s.TrimEnd(ex));
						result.Type = LengthType.Ex;
					}
					else
					{
						result.Value = 0;
						switch(s)
						{
							case "undefined": result.Type = LengthType.Undefined; break;
							case "pure": result.Type = LengthType.Pure; break; 
							case "infinity": result.Type = LengthType.Infinity; break; 
							case "veryverythin": result.Type = LengthType.VeryVeryThin; break; 
							case "verythin": result.Type = LengthType.VeryThin; break; 
							case "thin": result.Type = LengthType.Thin; break; 
							case "medium": result.Type = LengthType.Medium; break; 
							case "thick": result.Type = LengthType.Thick; break; 
							case "verythick": result.Type = LengthType.VeryThick; break; 
							case "veryverythick": result.Type = LengthType.VeryVeryThick; break; 
							case "negativeveryverythin": result.Type = LengthType.NegativeVeryVeryThin; break; 
							case "negativeverythin": result.Type = LengthType.NegativeVeryThin; break; 
							case "negativethin": result.Type = LengthType.NegativeThin; break; 
							case "negativemedium": result.Type = LengthType.NegativeMedium; break; 		
							case "negativethick": result.Type = LengthType.NegativeThick; break; 
							case "negativeverythick": result.Type = LengthType.NegativeVeryThick; break;
							case "negativeveryverythick": result.Type = LengthType.NegativeVeryVeryThick; break; 
							case "small": result.Type = LengthType.Small; break; 		
							case "normal": result.Type = LengthType.Normal; break; 
							case "big": result.Type = LengthType.Big; break; 
							case "auto": result.Type = LengthType.Auto; break; 
							case "fit": result.Type = LengthType.Fit; break;
							default: result = def; break;
						}
					}
				}
			}
			catch(Exception e)
			{
				Debug.WriteLine("Warning, invalid length format: \"" + s + "\", " + e.StackTrace);
				result = def;
			}
			return result;
		}

		internal static LineStyle ParseLinestyle(string s, LineStyle def)
		{
			switch(s)
			{
				case "solid" : def = LineStyle.Solid; break;
				case "dashed" : def = LineStyle.Dashed; break;
				case "node" : def = LineStyle.None; break;
			}
			return def;
		}

		internal static LineStyle[] ParseLinestyles(string s, LineStyle[] def)
		{
			LineStyle[] result = def;
			if(s.Length > 0)
			{			
				string[] styles = s.Split(' ');
				int count = 0;
				int li = 0;

				// although technically wrong, the string may be separated by more than
				// one space between strings, so deal with it.
				for(int i = 0; i < styles.Length; i++)
				{
					if(styles[i].Length > 0) count++;
				}

				if(count > 0)
				{ 
					result = new LineStyle[count];
					for(int i = 0; i < result.Length; i++)
					{
						while(styles[li].Length == 0) li++;
						result[i] = ParseLinestyle(styles[li++], def[i < def.Length ? i : def.Length - 1]);
					}
				}
			}
			return result;
		}

		internal static string UnparseLinestyles(LineStyle[] val)
		{
			return "";
		}

		internal static string UnparseLinestyle(LineStyle val)
		{
			return val.ToString().ToLower();
		}

		/// <summary>
		/// parse a list of length types, each one separated by one or more space
		/// </summary>
		internal static Length[] ParseLengths(string s, Length[] def)
		{
			Length[] result = def;
			if(s.Length > 0)
			{			
				string[] lengths = s.Split(' ');
				int count = 0;
				int li = 0;

				// although technically wrong, the string may be separated by more than
				// one space between strings, so deal with it.
				for(int i = 0; i < lengths.Length; i++)
				{
					if(lengths[i].Length > 0) count++;
				}

				if(count > 0)
				{ 
					result = new Length[count];
					for(int i = 0; i < result.Length; i++)
					{
						while(lengths[li].Length == 0) li++;
						result[i] = ParseLength(lengths[li++], def[i < def.Length ? i : def.Length - 1]);
					}
				}
			}
			return result;
		}

		internal static string UnparseLengths(Length[] lengths)
		{
			return "";
		}

		// Top, Bottom, Left, Center, Right, Axis, Baseline
		internal static Align ParseAlign(string s, Align deFault)
		{
			Align result = deFault;
			if(s != null && s.Length > 0)
			{
				switch(s)
				{
					case "top" : result = Align.Top; break;
					case "bottom" : result = Align.Bottom; break;
					case "left" : result = Align.Left; break;
					case "center" : result = Align.Center; break;
					case "right" : result = Align.Right; break;
					case "axis" : result = Align.Axis; break;
					case "baseline" : result = Align.Baseline; break;
				}
			}
			return result;
		}

		/// <summary>
		/// parse a space separated list of align values
		/// </summary>
		/// <returns>
		/// a list of Align values if the string has one or more valid align
		/// strings, an list of length one, containing a single default value
		/// otherwise
		/// </returns>
		internal static Align[] ParseAligns(string s, Align[] def)
		{
			Align[] result = def;
			string[] aligns = s.Split(' ');
			int count = 0;
			int ai = 0;

			if(s.Length > 0)
			{

				// although technically wrong, the string may be separated by more than
				// one space between strings, so deal with it.
				for(int i = 0; i < aligns.Length; i++)
				{
					if(aligns[i].Length > 0) count++;
				}

				if(count > 0)
				{ 
					result = new Align[count];
					for(int i = 0; i < result.Length; i++)
					{
						while(aligns[ai].Length == 0) ai++;
						result[i] = ParseAlign(aligns[ai++], def[i < def.Length ? i : def.Length - 1]);
					}
				}
			}
			return result;
		}

		internal static string UnparseAligns(Align[] list)
		{
			StringBuilder builder = new StringBuilder();
			for(int i = 0; i < list.Length; i++)
			{
				builder.Append(UnparseAlign(list[i]));
				if(i < list.Length - 1)
				{
					builder.Append(' ');				
				}
			}
			return builder.ToString();
		}

		internal static string UnparseAlign(Align a)
		{
			return a.ToString().ToLower();
		}

		/// <summary>
		/// parse a space separated list of bool values
		/// </summary>
		/// <returns>
		/// a list of bool values if the string has one or more valid bool
		/// strings, an list of length one, containing a single default value
		/// otherwise
		/// </returns>
		internal static bool[] ParseBoolList(string s, bool def)
		{
			bool[] result = null;
			string[] bools = s.Split(' ');
			int count = 0;
			int bi = 0;

			// although technically wrong, the string may be separated by more than
			// one space between strings, so deal with it.
			for(int i = 0; i < bools.Length; i++)
			{
				if(bools[i].Length > 0) count++;
			}

			if(count > 0)
			{ 
				result = new bool[count];
				for(int i = 0; i < result.Length; i++)
				{
					while(bools[bi].Length == 0) bi++;
					result[i] = ParseBool(bools[bi++], def);
				}
			}
			else
			{
				result = new bool[] {def};
			}
			return result;
		}

		internal static string UnparseBool(bool b)
		{
			return b ? "true" : "false";
		}

		internal static string UnparseBoolList(bool[] list)
		{
			StringBuilder builder = new StringBuilder();
			for(int i = 0; i < list.Length; i++)
			{
				builder.Append(UnparseBool(list[i]));
				if(i < list.Length - 1)
				{
					builder.Append(' ');				
				}
			}
			return builder.ToString();
		}

		internal static Occurrence ParseOccurrence(String s, Occurrence def)
		{
			Occurrence result = def;
			if(s != null && s.Length > 0)
			{
				switch(s)
				{
					case "prefix": result = Occurrence.Postfix; break;
					case "infix": result = Occurrence.Infix; break;
					case "postfix": result = Occurrence.Postfix; break;
					case "function-model": result = Occurrence.FunctionModel; break;
				}
			}
			return result;
		}

		internal static String UnparseOccurrence(Occurrence o)
		{
			String result = String.Empty;
			switch(o)
			{
				case Occurrence.Prefix: result = "prefix"; break;
				case Occurrence.Infix: result = "infix"; break;
				case Occurrence.Postfix: result = "postfix"; break;
				case Occurrence.FunctionModel: result = "function-model"; break;
			}
			return result;
		}

		private static readonly char[] em = new char[] {'e', 'm'};
		private static readonly char[] px = new char[] {'p', 'x'};
		private static readonly char[] inch = new char[] {'i', 'n'};
		private static readonly char[] cm = new char[] {'c', 'm'};
		private static readonly char[] mm = new char[] {'m', 'm'};
		private static readonly char[] pt = new char[] {'p', 't'};
		private static readonly char[] pc = new char[] {'p', 'c'};
		private static readonly char[] ex = new char[] {'e', 'x'};	
	}	
}