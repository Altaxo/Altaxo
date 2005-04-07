//
// SharpDevelop
//
// Copyright (C) 2004 Matthew Ward
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Matthew Ward (mrward@users.sourceforge.net)

using System;
using System.Text.RegularExpressions;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	/// <summary>
	/// Parses output text in the Output Build pad window and extracts source code
	/// file references.
	/// </summary>
	public class OutputTextLineParser
	{
		/// <summary>
		/// Creates a new instance of the <see cref="OutputTextlineParser"/> class.
		/// </summary>
		OutputTextLineParser()
		{
		}

		/// <summary>
		/// Extracts source code file reference from the c# compiler output.
		/// </summary>
		/// <param name="line">The text line to parse.</param>
		/// <returns>A <see cref="LineReference"/> if the line of text contains a
		/// file reference otherwise <see langword="null"/></returns>
		public static FileLineReference GetCSharpCompilerFileLineReference(string lineText)
		{
			if (lineText != null) {
				Match match = Regex.Match(lineText, @"^.*?(\w+:[/\\].*?)\(([\d]*),([\d]*)\)");
				if (match.Success) {
					try	{
						// Take off 1 for line/col since SharpDevelop is zero index based.
						int line = Convert.ToInt32(match.Groups[2].Value) - 1;
						int col = Convert.ToInt32(match.Groups[3].Value) - 1;                     
						
						return new FileLineReference(match.Groups[1].Value, line, col);
					} catch (Exception) {
						// Ignore.
					}
				}
			}
			
			return null;
		}
		
		/// <summary>
		/// Extracts source code file reference.
		/// </summary>
		/// <param name="line">The text line to parse.</param>
		/// <returns>A <see cref="lineReference"/> if the line of text contains a
		/// file reference otherwise <see langword="null"/></returns>
		public static FileLineReference GetFileLineReference(string lineText)
		{
			FileLineReference lineReference = GetCSharpCompilerFileLineReference(lineText);
			
			if (lineReference == null) {
				lineReference = GetNUnitOutputFileLineReference(lineText, false);
			}
			
			if (lineReference == null) {
				// Also works for VB compiler output.
				lineReference = GetCppCompilerFileLineReference(lineText);
			}
			
			return lineReference;
		}
		
		/// <summary>
		/// Extracts source code file reference from NUnit output.
		/// </summary>
		/// <param name="line">The text line to parse.</param>
		/// <param name="multiline">The <paramref name="line"/> text is multilined.</param>
		/// <returns>A <see cref="lineReference"/> if the line of text contains a
		/// file reference otherwise <see langword="null"/></returns>
		public static FileLineReference GetNUnitOutputFileLineReference(string lineText, bool multiline)
		{
			RegexOptions regexOptions = multiline ? RegexOptions.Multiline : RegexOptions.None;
			
			if (lineText != null) {
				Match match = Regex.Match(lineText, @"^.*?\sin\s(.*?):line\s(\d*)?$", regexOptions);
				
				if (match.Success) {
					try	{
						int line = Convert.ToInt32(match.Groups[2].Value) - 1;
						return new FileLineReference(match.Groups[1].Value, line);
					} catch (Exception) {
						// Ignore.
					}
				}
			}
			
			return null;
		}	

		/// <summary>
		/// Extracts source code file reference from the c++ or VB.Net compiler output.
		/// </summary>
		/// <param name="line">The text line to parse.</param>
		/// <returns>A <see cref="LineReference"/> if the line of text contains a
		/// file reference otherwise <see langword="null"/></returns>
		public static FileLineReference GetCppCompilerFileLineReference(string lineText)
		{
			if (lineText != null ) {
				
				Match match = Regex.Match(lineText, @"^.*?(\w+:[/\\].*?)\(([\d]*)\) :");
				
				if (match.Success) {
					try	{
						// Take off 1 for line/pos since SharpDevelop is zero index based.
						int line = Convert.ToInt32(match.Groups[2].Value) - 1;
						                           
						return new FileLineReference(match.Groups[1].Value.Trim(), line);
					} catch (Exception) {
						// Ignore.
					}
				}
			}
			
			return null;			
		}	
	}
}
