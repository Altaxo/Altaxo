// OutputFormatter.cs
// Copyright (C) 2003 Mike Krueger (mike@icsharpcode.net)
// 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Text;
using System.Collections;
using System.Diagnostics;

using ICSharpCode.SharpRefactory.Parser;
using ICSharpCode.SharpRefactory.Parser.AST;

namespace ICSharpCode.SharpRefactory.PrettyPrinter
{
	public class OutputFormatter
	{
		int indentationLevel     = 0;
		StringBuilder text       = new StringBuilder();
		
		public string Text {
			get {
				return text.ToString();
			}
		}
		
		public int IndentationLevel {
			get {
				return indentationLevel;
			}
			set {
				indentationLevel = value;
			}
		}
		
		public void Indent()
		{
			for (int i = 0; i < indentationLevel; ++i) {
				text.Append('\t');
			}
		}
		
		public void Space()
		{
			text.Append(' ');
		}
		
		public void NewLine()
		{
			text.Append("\n");
		}
		
		public void PrintToken(int token)
		{
		}
		
		public void PrintIdentifier(string identifier)
		{
			text.Append(identifier);
		}
		
		public void PrintTrailingComment()
		{
			
		}
	}
}
