using System;
using System.Text.RegularExpressions;

namespace entop_parser
{
	/**
	 * index and attributes for a mathml operator "mo" element
	 * 
	 * default values
	 * form prefix | infix | postfix set by position of operator in an mrow (rule given below); 
	 *		used with mo content to index operator dictionary		 * 
	 * fence true | false set by dictionary (false)
	 * separator true | false set by dictionary (false)
	 * lspace number h-unit | namedspace set by dictionary (thickmathspace)
	 * rspace number h-unit | namedspace set by dictionary (thickmathspace)
	 * stretchy true | false set by dictionary (false)
	 * symmetric true | false set by dictionary (true)
	 * maxsize number [ v-unit | h-unit ] | namedspace | infinity set by dictionary (infinity)
	 * minsize number [ v-unit | h-unit ] | namedspace set by dictionary (1)
	 * largeop true | false set by dictionary (false)
	 * movablelimits true | false set by dictionary (false)
	 * accent true | false set by dictionary (false)
	 */
	public class Operator
	{
		private static Regex commentRe =	new Regex("^\"&*(\\w+|[^\\s\"]+|;);*\"");
		private static Regex formRe =		new Regex("form\\s*=\\s*\"(\\w+)\\s*\"");
		private static Regex lspaceRe =		new Regex("lspace\\s*=\\s*\"(\\w+)\\s*\"");
		private static Regex rspaceRe =		new Regex("rspace\\s*=\\s*\"(\\w+)\\s*\"");
		private static Regex stretchyRe =	new Regex("stretchy\\s*=\\s*\"(\\w+)\\s*\"");
		private static Regex fenceRe =		new Regex("fence\\s*=\\s*\"(\\w+)\\s*\"");
		private static Regex accentRe =		new Regex("accent\\s*=\\s*\"(\\w+)\\s*\"");
		private static Regex largeOpRe =	new Regex("largeop\\s*=\\s*\"(\\w+)\\s*\"");
		private static Regex mlimitsRe =	new Regex("movablelimits\\s*=\\s*\"(\\w+)\\s*\"");
		private static Regex sepRe =		new Regex("separator\\s*=\\s*\"(\\w+)\\s*\"");
		private static Regex minSizeRe =	new Regex("minsize\\s*=\\s*\"(\\w+)\\s*\"");


		public string Comment;
		public string Name;
		public readonly string Form;
		public readonly string LSpace;
		public readonly string RSpace;
		public readonly string Stretchy;
		public readonly string Fence;
		public readonly string Accent;
		public readonly string LargeOp;
		public readonly string MoveableLimits;
		public readonly string Separator;
		public readonly string MinSize;
		public readonly string MaxSize = "infinity";
		public readonly string Symmetric = "true";

		public Operator(string line)
		{
			Comment = GetComment(line);
			Form = GetForm(line);
			LSpace = GetLSpace(line);
			RSpace = GetRSpace(line);
			Stretchy = GetStretchy(line);
			Fence = GetFence(line);
			Accent = GetAccent(line);
			LargeOp = GetLargeOp(line);
			MoveableLimits = GetMoveableLimits(line);
			Separator = GetSeparator(line);
			MinSize = GetMinSize(line);
		}

		private static string GetComment(string line)
		{
			return commentRe.Match(line).Groups[1].ToString();
		}

		private static string GetForm(string line)
		{
			string s = formRe.Match(line).Groups[1].ToString();
			if(s == "prefix") return "Form.Prefix";
			else if(s == "postfix") return "Form.Postfix";
			else return "Form.Infix";
		}

		private static string GetLSpace(string line)
		{
			Match m = lspaceRe.Match(line);
			if(m.Success)
			{
				return GetSpace(m.Groups[1].ToString());
			}
			else
			{
				return GetSpace("thickmathspace");
			}
		}

		private static string GetRSpace(string line)
		{
			Match m = rspaceRe.Match(line);
			if(m.Success)
			{
				return GetSpace(m.Groups[1].ToString());
			}
			else
			{
				return GetSpace("thickmathspace");
			}
		}

		private static string GetStretchy(string line)
		{
			Match m = stretchyRe.Match(line);
			if(m.Success)
			{
				return m.Groups[1].ToString().ToLower();
			}
			else
			{
				return "false";
			}
		}

		private static string GetFence(string line)
		{
			Match m = fenceRe.Match(line);
			if(m.Success)
			{
				return m.Groups[1].ToString().ToLower();
			}
			else
			{
				return "false";
			}
		}

		private static string GetAccent(string line)
		{
			Match m = accentRe.Match(line);
			if(m.Success)
			{
				return m.Groups[1].ToString().ToLower();
			}
			else
			{
				return "false";
			}
		}

		private static string GetLargeOp(string line)
		{
			Match m = largeOpRe.Match(line);
			if(m.Success)
			{
				return m.Groups[1].ToString().ToLower();
			}
			else
			{
				return "false";
			}
		}

		private static string GetMoveableLimits(string line)
		{
			Match m = mlimitsRe.Match(line);
			if(m.Success)
			{
				return m.Groups[1].ToString().ToLower();
			}
			else
			{
				return "false";
			}
		}

		private static string GetSeparator(string line)
		{
			Match m = sepRe.Match(line);
			if(m.Success)
			{
				return m.Groups[1].ToString().ToLower();
			}
			else
			{
				return "false";
			}
		}

		private static string GetMinSize(string line)
		{
			Match m = minSizeRe.Match(line);
			String s = "1";
			if(m.Success)
			{
				s =  m.Groups[1].ToString();
			}
			return "new Length(LengthType.Px, " + s + ")";
		}

		public override string ToString()
		{
			return String.Format("// {0} \nnew Operator({1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}),", 
				Comment, Name, Form, LSpace, RSpace, Stretchy, Fence, Accent, LargeOp, MoveableLimits, Separator, MinSize, MaxSize, Symmetric);

		}

		private static string GetSpace(string space)
		{
			string result;
			switch(space)
			{
				case "thickmathspace":
					result = "thickMathSpace";
					break;
				case "mediumthickmathspace":
					result = "mediumThickMathSpace";
					break;
				case "mediummathspace":
					result = "mediumMathSpace";
					break;
				case "thinMathspace":
					result = "thinMathSpace";
					break;
				case "verythinmathspace":
					result = "veryThinMathSpace";
					break;
				case "veryverythinmathspace":
					result = "veryVeryThinMathSpace";
					break;
				case "verythickmathspace":
					result = "veryThickMathSpace";
					break;
				case "0em":
					result = "zeroEM";
					break;
				default:
					result = "thickMathSpace";
					break;
			}
			return result;

		}

	}
}
