#region Disclaimer
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) Dr. Dirk Lellinger
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
#endregion

using System;

namespace Altaxo.Serialization
{
	/// <summary>
	/// This class is only intended to group some static functions for parsing of strings.
	/// </summary>
	public class Parsing
	{

		/// <summary>
		/// Is the provided string a date/time?
		/// </summary>
		/// <param name="s">The string to parse</param>
		/// <returns>True if the string can successfully parsed to a DateTime object.</returns>
		public static bool IsDateTime(string s)
		{
			bool bRet=false;
			try
			{
				System.Convert.ToDateTime(s);
				bRet=true;
			}
			catch(Exception)
			{
			}
			return bRet;
		}

		/// <summary>
		/// Tests if the provided string represents a number.
		/// </summary>
		/// <param name="s">The string to test.</param>
		/// <returns>True if the string represents a number.</returns>
		public static bool IsNumeric(string s)
		{
			bool bRet=false;
			try
			{
				System.Convert.ToDouble(s);
				bRet=true;
			}
			catch(Exception)
			{
			}
			return bRet;
		}
	}


	public class AsciiLineStructure 
	{
		protected System.Collections.ArrayList mylist = new System.Collections.ArrayList();
		protected int nLineNumber;
		protected bool bContainsDBNull=false;
		protected bool bDirty=true;
		protected int prty=0;
		protected int hash=0;
		protected int m_CountDecimalSeparatorDot=0; // used for statistics of use of decimal separator
		protected int m_CountDecimalSeparatorComma=0; // used for statistics of use of decimal separator

		static char[] sm_ExponentChars = { 'e', 'E' };


		public int Count
		{
			get
			{
				return mylist.Count;
			}
		}
		public void Add(object o)
		{
			mylist.Add(o);
			bDirty=true;
		}


		public int DecimalSeparatorDotCount
		{
			get { return m_CountDecimalSeparatorDot; }
		}

		public int DecimalSeparatorCommaCount
		{
			get { return m_CountDecimalSeparatorComma; }
		}



		public object this[int i]
		{
			get
			{
				return mylist[i];
			}
			set
			{
				mylist[i]=value;
				bDirty=true;
			}
		}
		
		public int LineNumber
		{
			get
			{
				return nLineNumber;
			}
			set
			{
				nLineNumber=value;
			}
		}

		public bool ContainsDBNull
		{
			get
			{
				if(bDirty)
					ResetDirty();
				return bContainsDBNull;
			}
		}

		public int Priority
		{
			get
			{
				if(bDirty)
					ResetDirty();
				return prty;
			}
		}
		
		public void ResetDirty()
		{
			bDirty = false;

			// Calculate priority and hash

			int len = Count;
			prty = 0;
			for(int i=0;i<len;i++)
			{
				Type t = (Type) this[i];
				if(t==typeof(DateTime))
					prty += 10;
				else if(t==typeof(Double))
					prty += 5;
				else if(t==typeof(String))
					prty += 2;
				else if(t==typeof(DBNull))
				{
					prty += 1;
					bContainsDBNull=true;
				}
			} // for

			// calculate hash

			hash = Count.GetHashCode();
			for(int i=0;i<len;i++)
				hash = ((hash<<1) | 1) ^ this[i].GetHashCode();
		}

		public override int GetHashCode()
		{
			if(bDirty)
				ResetDirty();
			return hash;
		}
		public bool IsCompatibleWith(AsciiLineStructure ano)
		{
			// our structure can have more columns, but not lesser than ano
			if(this.Count<ano.Count)
				return false;

			for(int i=0;i<ano.Count;i++)
			{
				if(this[i]==typeof(DBNull) || ano[i]==typeof(DBNull))
					continue;
				if(this[i]!=ano[i])
					return false;
			}
			return true;
		}

		/// <summary>
		/// make a statistics on the use of the decimal separator
		/// the aim is to recognize automatically which is the decimal separator
		/// the function analyses the string for commas and dots and adds a statistics
		/// </summary>
		/// <param name="numstring">a string which represents a numeric value</param>
		public void AddToDecimalSeparatorStatistics(string numstring)
		{
			// some rules:
			// 1.) if more than one comma (dot) is existant, it can not be the decimal separator -> it seems that the alternative character is then the decimal separator
			// 2.) if only one comma (dot) is existent, and three digits before or back is not a comma (dot), than this is a good hint for the character to be the decimal separator

			int ds,de,cs,ce;

			ds=numstring.IndexOf('.'); // ds -> dot start
			de= ds<0 ? -1 : numstring.LastIndexOf('.'); // de -> dot end
			cs=numstring.IndexOf(','); // cs -> comma start
			ce= cs<0 ? -1 : numstring.LastIndexOf(','); // ce -> comma end

			if(ds>=0 && de!=ds)
			{
				m_CountDecimalSeparatorComma++;
			}
			if(cs>=0 && ce!=cs)
			{
				m_CountDecimalSeparatorDot++;
			}

			// if there is only one dot and no comma
			if(ds>=0 && de==ds && cs<0)
			{
				if(numstring.IndexOfAny(sm_ExponentChars)>0) // if there is one dot, but no comma, and a Exponent char (e, E), than dot is the decimal separator
					m_CountDecimalSeparatorDot++;
				else if((ds>=4 && Char.IsDigit(numstring,ds-4)) || ((ds+4)<numstring.Length && Char.IsDigit(numstring,ds+4)))
					m_CountDecimalSeparatorDot++; 			// analyze the digits before and back, if 4 chars before or back is a digit (no separator), than dot is the decimal separator
			}

			// if there is only one comma and no dot
			if(cs>=0 && ce==cs && ds<0)
			{
				if(numstring.IndexOfAny(sm_ExponentChars)>0) // if there is one dot, but no comma, and a Exponent char (e, E), than dot is the decimal separator
					m_CountDecimalSeparatorComma++;
				else if((cs>=4 && Char.IsDigit(numstring,cs-4)) || ((cs+4)<numstring.Length && Char.IsDigit(numstring,cs+4)))
					m_CountDecimalSeparatorComma++; 			// analyze the digits before and back, if 4 chars before or back is a digit (no separator), than dot is the decimal separator
			}

		}
			
	} // end class AsciiLineStructure


	public struct NumberAndStructure
	{
		public int nLines;
		public AsciiLineStructure structure;
	} // end class


	public class AsciiLineAnalyzer
	{
		public enum Separation { Tab=0, Comma=1, Semicolon=2 };

		public int nNumberOfTabs=0;
		public int nNumberOfCommas=0;
		public int nNumberOfPoints=0;
		public int nNumberOfSemicolons=0;
		public int nPositionTab4=0;
		public int nPositionTab8=0;

		public System.Collections.ArrayList wordStartsTab4 = new System.Collections.ArrayList();
		public System.Collections.ArrayList wordStartsTab8 = new System.Collections.ArrayList();

		public AsciiLineStructure[] structure; 
		public AsciiLineStructure structWithCommas = null; 
		public AsciiLineStructure structWithSemicolons = null; 


		public AsciiLineAnalyzer(int nLine,string sLine)
		{
			structure = new AsciiLineStructure[3];
			structure[(int)Separation.Tab] = AssumeSeparator(nLine,sLine,"\t");
			structure[(int)Separation.Comma] = AssumeSeparator(nLine,sLine,",");
			structure[(int)Separation.Semicolon] = AssumeSeparator(nLine,sLine,";");
		}
		
		public AsciiLineAnalyzer(string sLine, bool bDummy)
		{
			int nLen = sLine.Length;
			if(nLen==0)
				return;

			bool bInWord=false;
			bool bInString=false; // true when starting with " char
			for(int i=0;i<nLen;i++)
			{
				char cc = sLine[i];

				if(cc=='\t')
				{
					nNumberOfTabs++;
					nPositionTab8 += 8-(nPositionTab8%8);
					nPositionTab4 += 4-(nPositionTab4%4);
					bInWord &= bInString;
					continue;
				}
				else if(cc==' ')
				{
					bInWord &= bInString;
					nPositionTab4++;
					nPositionTab8++;
				}
				else if(cc=='\"')
				{
					bInWord = !bInWord;
					bInString = !bInString;
					if(bInWord) 
					{
						wordStartsTab4.Add(nPositionTab4);
						wordStartsTab8.Add(nPositionTab8);
					}

					nPositionTab4++;
					nPositionTab8++;
				}
				else if(cc>' ') // all other chars are no space chars
				{
					if(!bInWord)
					{
						bInWord=true;
						wordStartsTab4.Add(nPositionTab4);
						wordStartsTab8.Add(nPositionTab8);
					}
					nPositionTab4++;
					nPositionTab8++;
					
					if(cc=='.') nNumberOfPoints++;
					if(cc==',') nNumberOfCommas++;
					if(cc==';') nNumberOfSemicolons++;
				}
			}
		}

		public static AsciiLineStructure AssumeSeparator(int nLine, string sLine, string separator)
		{
			AsciiLineStructure tabStruc = new AsciiLineStructure();
			tabStruc.LineNumber = nLine;

			int len =sLine.Length;
			int ix=0;
			for(int start=0; start<=len; start=ix+1)
			{
				ix = sLine.IndexOf(separator,start,len-start);
				if(ix==-1)
				{
					ix = len;
				}

				// try to interpret ix first as DateTime, then as numeric and then as string
				string substring = sLine.Substring(start,ix-start);
				if(ix==start) // just this char is a tab, so nothing is between the last and this
				{
					tabStruc.Add(typeof(DBNull));
				}
				else if(IsNumeric(substring))
				{
					tabStruc.Add(typeof(double));
					tabStruc.AddToDecimalSeparatorStatistics(substring); // make a statistics of the use of decimal separator
				}
				else if(IsDateTime(substring))
				{
					tabStruc.Add(typeof(System.DateTime));
				}
				else
				{
					tabStruc.Add(typeof(string));
				}
			} // end for
			return tabStruc;
		}

		public static bool IsDateTime(string s)
		{
			bool bRet=false;
			try
			{
				System.Convert.ToDateTime(s);
				bRet=true;
			}
			catch(Exception)
			{
			}
			return bRet;
		}
		public static bool IsNumeric(string s)
		{
			bool bRet=false;
			try
			{
				System.Convert.ToDouble(s);
				bRet=true;
			}
			catch(Exception)
			{
			}
			return bRet;
		}

		public int WordStartsTab4_GetHashCode()
		{
			return GetHashOf(wordStartsTab4);
		}

		public int WordStartsTab8_GetHashCode()
		{
			return GetHashOf(wordStartsTab4);
		}


		public static int GetHashOf(System.Collections.ArrayList al)
		{
			int len = al.Count;
			int hash = al.Count.GetHashCode();
			for(int i=0;i<len;i++)
				hash ^= al[i].GetHashCode();
			
			return hash;
		}


		public static int GetPriorityOf(System.Collections.ArrayList al)
		{
			int len = al.Count;
			int prty = 0;
			for(int i=0;i<len;i++)
			{
				Type t = (Type) al[i];
				if(t==typeof(DateTime))
					prty += 10;
				else if(t==typeof(Double))
					prty += 5;
				else if(t==typeof(String))
					prty += 2;
				else if(t==typeof(DBNull))
					prty += 1;
			} // for
			return prty;
		} 
	} // end class



	public class AltaxoAsciiImporter
	{
		protected System.IO.Stream stream;

		public AltaxoAsciiImporter(System.IO.Stream _stream)
		{
			this.stream = _stream;
		}


		/// <summary>
		/// calculates the priority of the result
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		public static int GetPriorityOf(System.Collections.ArrayList result, AsciiLineAnalyzer.Separation sep, ref AsciiLineStructure bestLine)
		{
			System.Collections.Hashtable sl = new System.Collections.Hashtable();
			bestLine=null;
			for(int i=0;i<result.Count;i++)
			{
				AsciiLineAnalyzer ala = (AsciiLineAnalyzer)result[i];
				int p  = ((AsciiLineAnalyzer)result[i]).structure[(int)sep].GetHashCode(); // and hash code
				if(null==sl[p])
					sl.Add(p,1);
				else 
					sl[p] = 1+(int)sl[p];
			}
			// get the count with the topmost frequency
			int nNumberOfMaxSame = 0;
			int nHashOfMaxSame = 0;
			foreach(System.Collections.DictionaryEntry ohash in sl)
			{
				int hash = (int)ohash.Key;
				int cnt = (int)ohash.Value;
				if(nNumberOfMaxSame<cnt)
				{
					nNumberOfMaxSame  = cnt;
					nHashOfMaxSame = hash;
				}
			} // for each
			// search for the max priority of the hash
			int nMaxPriorityOfMaxSame=0;
			for(int i=0;i<result.Count;i++)
			{
				AsciiLineAnalyzer ala = (AsciiLineAnalyzer)result[i];
				if(nHashOfMaxSame == ((AsciiLineAnalyzer)result[i]).structure[(int)sep].GetHashCode())
				{
					int prty = ((AsciiLineAnalyzer)result[i]).structure[(int)sep].Priority;
					if(prty>nMaxPriorityOfMaxSame)
					{
						nMaxPriorityOfMaxSame = prty;
						bestLine = ((AsciiLineAnalyzer)result[i]).structure[(int)sep];
					}
				}// if
			} // for
			return nNumberOfMaxSame;
		}


		public AsciiImportOptions Analyze(int nLines, AsciiImportOptions defaultImportOptions)
		{

			string sLine;
			System.IO.StreamReader sr = new System.IO.StreamReader(stream,System.Text.Encoding.ASCII,true);
			System.Collections.ArrayList result = new System.Collections.ArrayList();
		
			for(int i=0;i<nLines;i++)
			{
				sLine = sr.ReadLine();
				if(null==sLine)
					break;
				result.Add(new AsciiLineAnalyzer(i,sLine));
			}
		
			// now view the results
			// calc the frequency o
			System.Collections.SortedList sl= new System.Collections.SortedList();
			int nItems;
			// first the tabs

			/*
			sl.Clear();
			for(int i=0;i<result.Count;i++)
			{
				nItems = ((AsciiLineAnalyzer)result[i]).nNumberOfTabs;
				if(0!=nItems)
				{
					if(null==sl[nItems])
						sl.Add(nItems,1);
					else 
						sl[nItems] = 1+(int)sl[nItems];
				}
			}
			// get the tab count with the topmost frequency
			int nMaxNumberOfSameTabs = 0;
			int nMaxTabsOfSameNumber = 0;
			for(int i=0;i<sl.Count;i++)
			{
				if(nMaxNumberOfSameTabs<(int)sl.GetByIndex(i))
				{
					nMaxNumberOfSameTabs = (int)sl.GetByIndex(i);
					nMaxTabsOfSameNumber = (int)sl.GetKey(i);
				}
			}
*/
			
			
			// Count the commas
			sl.Clear();
			for(int i=0;i<result.Count;i++)
			{
				nItems = ((AsciiLineAnalyzer)result[i]).nNumberOfCommas;
				if(0!=nItems)
				{
					if(null==sl[nItems])
						sl.Add(nItems,1);
					else 
						sl[nItems] = 1+(int)sl[nItems];
				}
			}
			// get the comma count with the topmost frequency
			int nMaxNumberOfSameCommas = 0;
			int nMaxCommasOfSameNumber = 0;
			for(int i=0;i<sl.Count;i++)
			{
				if(nMaxNumberOfSameCommas<(int)sl.GetByIndex(i))
				{
					nMaxNumberOfSameCommas = (int)sl.GetByIndex(i);
					nMaxCommasOfSameNumber = (int)sl.GetKey(i);
				}
			}

			// Count the semicolons
			sl.Clear();
			for(int i=0;i<result.Count;i++)
			{
				nItems = ((AsciiLineAnalyzer)result[i]).nNumberOfSemicolons;
				if(0!=nItems)
				{
					if(null==sl[nItems])
						sl.Add(nItems,1);
					else 
						sl[nItems] = 1+(int)sl[nItems];
				}
			}
			// get the tab count with the topmost frequency
			int nMaxNumberOfSameSemicolons = 0;
			int nMaxSemicolonsOfSameNumber = 0;
			for(int i=0;i<sl.Count;i++)
			{
				if(nMaxNumberOfSameSemicolons<(int)sl.GetByIndex(i))
				{
					nMaxNumberOfSameSemicolons = (int)sl.GetByIndex(i);
					nMaxSemicolonsOfSameNumber = (int)sl.GetKey(i);
				}
			}

		
			NumberAndStructure[] st = new NumberAndStructure[3];

			for(int i=0;i<3;i++)
			{
				st[i].nLines = GetPriorityOf(result,(AsciiLineAnalyzer.Separation)i,ref st[i].structure);
			}

			// look for the top index
		
			int nMaxLines = int.MinValue;
			double maxprtylines=0;
			int nBestSeparator = int.MinValue;
			for(int i=0;i<3;i++)
			{
				double prtylines = (double)st[i].nLines * st[i].structure.Priority;
				if(prtylines==maxprtylines)
				{
					if(st[i].nLines > nMaxLines)
					{
						nMaxLines = st[i].nLines;
						nBestSeparator = i;
					}
				}
				else if(prtylines>maxprtylines)
				{
					maxprtylines = prtylines;
					nBestSeparator = i;
					nMaxLines=st[i].nLines;
				}
			}

			AsciiImportOptions opt = defaultImportOptions.Clone();
			
			opt.bDelimited = true;
			opt.cDelimiter = nBestSeparator==0 ? '\t' : (nBestSeparator==1 ? ',' : ';');
			opt.recognizedStructure = st[nBestSeparator].structure;


			// look how many header lines are in the file by comparing the structure of the first lines  with the recognized structure
			for(int i=0;i<result.Count;i++)
			{
				opt.nMainHeaderLines=i;
				if(((AsciiLineAnalyzer)result[i]).structure[nBestSeparator].IsCompatibleWith(opt.recognizedStructure))
					break;
			}


			// calculate the total statistics of decimal separators
			opt.m_DecimalSeparatorCommaCount=0;
			opt.m_DecimalSeparatorDotCount=0;
			for(int i=0;i<result.Count;i++)
			{
				opt.m_DecimalSeparatorDotCount += ((AsciiLineAnalyzer)result[i]).structure[nBestSeparator].DecimalSeparatorDotCount;
				opt.m_DecimalSeparatorCommaCount += ((AsciiLineAnalyzer)result[i]).structure[nBestSeparator].DecimalSeparatorCommaCount;
			}



			return opt;

		}


		public void ImportAscii(AsciiImportOptions impopt, Altaxo.Data.DataTable table)
		{
			string sLine;
			stream.Position=0; // rewind the stream to the beginning
			System.IO.StreamReader sr = new System.IO.StreamReader(stream,System.Text.Encoding.ASCII,true);
			Altaxo.Data.DataColumnCollection newcols = new Altaxo.Data.DataColumnCollection();
		
			Altaxo.Data.DataColumnCollection newpropcols = new Altaxo.Data.DataColumnCollection();

			// in case a structure is provided, allocate already the columsn
			
			if(null!=impopt.recognizedStructure)
			{
				for(int i=0;i<impopt.recognizedStructure.Count;i++)
				{
					if(impopt.recognizedStructure[i]==typeof(Double))
						newcols.Add(new Altaxo.Data.DoubleColumn());
					else if(impopt.recognizedStructure[i]==typeof(DateTime))
						newcols.Add(new Altaxo.Data.DateTimeColumn());
					else if(impopt.recognizedStructure[i]==typeof(string))
						newcols.Add(new Altaxo.Data.TextColumn());
					else
						newcols.Add(new Altaxo.Data.DBNullColumn());;
				}
			}

			// add also additional property columns if not enough there
			if(impopt.nMainHeaderLines>1) // if there are more than one header line, allocate also property columns
			{
				int toAdd = impopt.nMainHeaderLines-1;
				for(int i=0;i<toAdd;i++)
					newpropcols.Add(new Data.TextColumn());
			}

			// if decimal separator statistics is provided by impopt, create a number format info object
			System.Globalization.NumberFormatInfo numberFormatInfo=null;
			if(impopt.m_DecimalSeparatorCommaCount>0 || impopt.m_DecimalSeparatorDotCount>0)
			{
				numberFormatInfo = (System.Globalization.NumberFormatInfo)System.Globalization.NumberFormatInfo.CurrentInfo.Clone();

				// analyse the statistics
				if(impopt.m_DecimalSeparatorCommaCount>impopt.m_DecimalSeparatorDotCount) // the comma is the decimal separator
				{
					numberFormatInfo.NumberDecimalSeparator=",";
					if(numberFormatInfo.NumberGroupSeparator==numberFormatInfo.NumberDecimalSeparator)
						numberFormatInfo.NumberGroupSeparator=""; // in case now the group separator is also comma, remove the group separator
				}
				else if(impopt.m_DecimalSeparatorCommaCount<impopt.m_DecimalSeparatorDotCount) // the comma is the decimal separator
				{
					numberFormatInfo.NumberDecimalSeparator=".";
					if(numberFormatInfo.NumberGroupSeparator==numberFormatInfo.NumberDecimalSeparator)
						numberFormatInfo.NumberGroupSeparator=""; // in case now the group separator is also comma, remove the group separator
				}
			}
			else // no decimal separator statistics is provided, so retrieve the numberFormatInfo object from the program options or from the current thread
			{
				numberFormatInfo = System.Globalization.NumberFormatInfo.CurrentInfo;
			}


			char [] splitchar = new char[]{impopt.cDelimiter};

			// first of all, read the header if existent
			for(int i=0;i<impopt.nMainHeaderLines;i++)
			{
				sLine = sr.ReadLine();
				if(null==sLine) break;

				string[] substr = sLine.Split(splitchar);
				int cnt = substr.Length;
				for(int k=0;k<cnt;k++)
				{
					if(substr[k].Length==0)
						continue;
				
					if(i==0) // is it the column name line
					{
						newcols.SetColumnName(k, substr[k]);
					}
					else // this are threated as additional properties
					{
						((Data.DataColumn)newpropcols[i-1])[k] = substr[k]; // set the properties
					}
				}
			}
			
			for(int i=0;true;i++)
			{
				sLine = sr.ReadLine();
				if(null==sLine) break;

				string[] substr = sLine.Split(splitchar);
				int cnt = substr.Length;
				for(int k=0;k<cnt;k++)
				{
					if(substr[k].Length==0)
						continue;

					if(newcols[k] is Altaxo.Data.DoubleColumn)
					{
						try { ((Altaxo.Data.DoubleColumn)newcols[k])[i] = System.Convert.ToDouble(substr[k],numberFormatInfo); }
						catch {}
					}
					else if( newcols[k] is Altaxo.Data.DateTimeColumn)
					{
						try { ((Altaxo.Data.DateTimeColumn)newcols[k])[i] = System.Convert.ToDateTime(substr[k]); }
						catch {}
					}
					else if( newcols[k] is Altaxo.Data.TextColumn)
					{
						((Altaxo.Data.TextColumn)newcols[k])[i] = substr[k];
					}
					else if(null==newcols[k] || newcols[k] is Altaxo.Data.DBNullColumn)
					{
						bool bConverted = false;
						double val=Double.NaN;
						DateTime valDateTime=DateTime.MinValue;

						try
						{ 
							val = System.Convert.ToDouble(substr[k]);
							bConverted=true;
						}
						catch
						{
						}
						if(bConverted)
						{
							Altaxo.Data.DoubleColumn newc = new Altaxo.Data.DoubleColumn();
							newc[i]=val;
							newcols[k] = newc;
						}
						else
						{
							try
							{ 
								valDateTime = System.Convert.ToDateTime(substr[k]);
								bConverted=true;
							}
							catch
							{
							}
							if(bConverted)
							{
								Altaxo.Data.DateTimeColumn newc = new Altaxo.Data.DateTimeColumn();
								newc[i]=valDateTime;
								
								newcols[k] = newc;
							}
							else
							{
								newcols[k] = new Altaxo.Data.TextColumn();
								((Altaxo.Data.TextColumn)newcols[k])[i]=substr[k];
							}
						} // end outer if null==newcol
					}
				} // end of for all cols


			} // end of for all lines
			
			// insert the new columns or replace the old ones
			table.Suspend();
			for(int i=0;i<newcols.ColumnCount;i++)
			{
				if(newcols[i] is Altaxo.Data.DBNullColumn)
					continue;
				table.DataColumns.CopyOrReplaceOrAdd(i,newcols[i], newcols.GetColumnName(i));
			} // end for loop

			// add the property columns
			for(int i=0;i<newpropcols.ColumnCount;i++)
			{
				table.PropCols.CopyOrReplaceOrAdd(i,newpropcols[i], newpropcols.GetColumnName(i));
			}
			table.Resume();
		} // end of function ImportAscii
	} // end class 
	public class AsciiImportOptions
	{
		public bool bRenameColumns; /// rename the columns if 1st line contain  the column names
		public bool bRenameWorksheet; // rename the worksheet to the data file name

		public int nMainHeaderLines; // lines to skip (the main header)
		public bool bDelimited;      // true if delimited by a single char
		public char cDelimiter;      // the delimiter char

		public int m_DecimalSeparatorDotCount=0;
		public int m_DecimalSeparatorCommaCount=0;


		public AsciiLineStructure recognizedStructure=null;




		public AsciiImportOptions Clone()
		{
			return (AsciiImportOptions)MemberwiseClone();
		}

	}

}
