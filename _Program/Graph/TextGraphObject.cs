/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002 Dr. Dirk Lellinger
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

using System;
using System.Drawing;

namespace Altaxo
{
	/// <summary>
	/// Summary description for TextGraphObject.
	/// </summary>
	public class TextGraphObject : GraphObject
	{
		protected Font m_Font;
		protected string m_Text = "";
		protected Color m_Color = Color.Black;

#region "Constructors"

		public TextGraphObject()
		{
		}
		public TextGraphObject(PointF graphicPosition, string text, 
			Font textFont, Color textColor)
		{
			this.SetPosition(graphicPosition);
			this.Font = textFont;
			this.Text = text;
			this.Color = textColor;
		}


		public TextGraphObject(	float posX, float posY, 
			string text, Font textFont, Color textColor)
			: this(new PointF(posX, posY), text, textFont, textColor)
		{
		}


		public TextGraphObject(PointF graphicPosition, 
			string text, Font textFont, 
			Color textColor, float Rotation)
			: this(graphicPosition, text, textFont, textColor)
		{
			this.Rotation = Rotation;
		}

		public TextGraphObject(float posX, float posY, 
			string text, 
			Font textFont, 
			Color textColor, float Rotation)
			: this(new PointF(posX, posY), text, textFont, textColor, Rotation)
		{
		}

#endregion


		public Font Font
		{
			get
			{
				return m_Font;
			}
			set
			{
				m_Font = value;
			}
		}

		public string Text
		{
			get
			{
				return m_Text;
			}
			set
			{
				m_Text = value;
			}
		}
		public System.Drawing.Color Color
		{
			get
			{
				return m_Color;
			}
			set
			{
				m_Color = value;
			}
		}
		public override void Paint(Graphics g, object obj)
		{

			System.Drawing.Drawing2D.GraphicsState gs = g.Save();
			g.TranslateTransform(X,Y);
			g.RotateTransform(m_Rotation);
			
			// Modification of StringFormat is necessary to avoid 
			// too big spaces between successive words
			StringFormat strfmt = StringFormat.GenericTypographic;
			strfmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

			strfmt.LineAlignment = StringAlignment.Near;
			strfmt.Alignment = StringAlignment.Near;

			// next statement is necessary to have a consistent string length both
			// on 0 degree rotated text and rotated text
			// without this statement, the text is fitted to the pixel grid, which
			// leads to "steps" during scaling
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

			if(this.AutoSize)
			{
				SizeF mySize = g.MeasureString(m_Text, m_Font);
				this.Width = mySize.Width;
				this.Height = mySize.Height;
				g.DrawString(m_Text, m_Font, new SolidBrush(m_Color), 0, 0, strfmt);
			}
			else
			{
				System.Drawing.RectangleF rect = new RectangleF(0, 0, this.Width, this.Height);
				g.DrawString(m_Text, m_Font, new SolidBrush(m_Color), rect, strfmt);
			}
			
			g.Restore(gs);
		}
	}



	public class TextItem
	{
		protected internal string m_Text; // the text to display

		protected internal bool m_bUnderlined;
		protected internal bool m_bItalic;
		protected internal bool m_bBold; // true if bold should be used
		protected internal bool m_bGreek; // true if greek charset should be used
		protected internal int  m_SubIndex;

		protected internal int m_LayerNumber=-1; // number of the layer or -1 for the current layer
		protected internal int m_PlotNumber=-1; // number of the plot curve or -1 in case this is disabled
		protected internal int m_PlotPointNumber=-1; // number of the plot point or -1 for the whole curve

		protected internal float m_cyLineSpace; // cached linespace value of the font
		protected internal float m_cyAscent;    // cached ascent value of the font
		protected internal float m_cyDescent; /// cached descent value of the font
		protected internal float m_Width; // cached width of the item


		// help items
		protected Font	m_Font;
		public    float	m_yShift=0; 

		public TextItem(Font ft)
		{
			m_Text="";
			m_Font = (null==ft)? null: (Font)ft.Clone();
		}

		public void SetAsSymbol(int args, int[] arg)
		{
			m_Text=null;
			switch(args)
			{
				case 1:
					m_LayerNumber=-1;
					m_PlotNumber=arg[0];
					m_PlotPointNumber=-1;
					break;
				case 2:
					m_LayerNumber = arg[0];
					m_PlotNumber = arg[1];
					m_PlotPointNumber = -1;
					break;
				case 3:
					m_LayerNumber = arg[0];
					m_PlotNumber = arg[1];
					m_PlotPointNumber = arg[2];
					break;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="from"></param>
		/// <param name="ft"></param>
		public TextItem(TextItem from, Font ft)
		{
			m_Text="";
			m_bUnderlined = from.m_bUnderlined;
			m_bItalic     = from.m_bItalic;
			m_bBold       = from.m_bBold;
			m_bGreek = from.m_bGreek;
			m_SubIndex = from.m_SubIndex;
			m_yShift   = from.m_yShift;
			m_Font = null!=ft ? ft : from.m_Font;
		}

		public Font Font
		{
			get { return m_Font; }
		}


		public bool IsEmpty
		{
			get { return (m_Text==null || m_Text.Length==0) && m_LayerNumber<0 && m_PlotNumber<0; }
		}

		public bool IsText
		{
			get { return m_Text!=null && m_Text.Length>0; }
		}

		public bool IsSymbol
		{
			get { return m_PlotNumber>=0; }
		}
	}


	public class TextLine	: System.Collections.CollectionBase
	{
		protected internal float m_cyLineSpace; // linespace value : cyAscent + cyDescent
		protected internal float m_cyAscent;    // height of the items above the ground line
		protected internal float m_cyDescent; /// heigth of the items below the ground line
		protected internal float m_Width; // cached width of the line (sum of width of all items)

		public TextItem this[int i]
		{
			get { return (TextItem)base.InnerList[i]; }
			set 
			{
				if(i<Count)
					base.InnerList[i] = value;
				else if(i==Count)
					base.InnerList.Add(value);
				else
					throw new System.ArgumentOutOfRangeException("i",i,"The index was not in the valid range");
			}
		}

		public void Add(TextItem ti)
		{
			base.InnerList.Add(ti);
		}

		public class TextLineCollection : System.Collections.CollectionBase
		{

			public TextLine this[int i]
			{
				get { return (TextLine)base.InnerList[i]; }
				set { base.InnerList[i] = value; }
			}

			public void Add(TextLine tl)
			{
				base.InnerList.Add(tl);
			}
		}


	} // end of class TextLine
	



	/// <summary>
	/// ExtendedTextGraphObject provides not only simple text on a graph,
	/// but also some formatting of the text, and quite important - the plot symbols
	/// to be used either in the legend or in the axis titles
	/// </summary>
	public class ExtendedTextGraphObject : GraphObject
	{
		protected Font m_Font;
		protected string m_Text = ""; // the text, which contains the formatting symbols
		protected Color m_Color = Color.Black;

		protected TextLine.TextLineCollection m_TextLines;
		protected bool m_bStructureInSync=false; // true when the text was interpretet and the structure created
		protected bool m_bMeasureInSync=false; // true when all items are measured
		

#region "Constructors"

		public ExtendedTextGraphObject()
		{
			m_Font = new Font(FontFamily.GenericSansSerif,18,GraphicsUnit.World);
		}

		public ExtendedTextGraphObject(PointF graphicPosition, string text, 
			Font textFont, Color textColor)
		{
			this.SetPosition(graphicPosition);
			this.Font = textFont;
			this.Text = text;
			this.Color = textColor;
		}


		public ExtendedTextGraphObject(	float posX, float posY, 
			string text, Font textFont, Color textColor)
			: this(new PointF(posX, posY), text, textFont, textColor)
		{
		}


		public ExtendedTextGraphObject(PointF graphicPosition, 
			string text, Font textFont, 
			Color textColor, float Rotation)
			: this(graphicPosition, text, textFont, textColor)
		{
			this.Rotation = Rotation;
		}

		public ExtendedTextGraphObject(float posX, float posY, 
			string text, 
			Font textFont, 
			Color textColor, float Rotation)
			: this(new PointF(posX, posY), text, textFont, textColor, Rotation)
		{
		}

#endregion


		protected void Interpret(Graphics g)
		{
			this.m_bMeasureInSync = false; // if structure is changed, the measure is out of sync

			char[] searchchars = new Char[] { '\\', '\r', '\n', ')' };

			// Modification of StringFormat is necessary to avoid 
			// too big spaces between successive words
			StringFormat strfmt = StringFormat.GenericTypographic;
			strfmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

			strfmt.LineAlignment = StringAlignment.Far;
			strfmt.Alignment = StringAlignment.Near;

			// next statement is necessary to have a consistent string length both
			// on 0 degree rotated text and rotated text
			// without this statement, the text is fitted to the pixel grid, which
			// leads to "steps" during scaling
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

			float baseLineSpace, baseAscent, baseDescent;
			MeasureFont(g, m_Font, out baseLineSpace, out baseAscent, out baseDescent);


			System.Collections.Stack itemstack = new System.Collections.Stack();

			Font currFont = (Font)m_Font.Clone();


			if(null!=m_TextLines)
				m_TextLines.Clear(); // delete old contents 
			else
				m_TextLines = new TextLine.TextLineCollection();


			TextLine currTextLine = new TextLine();
				
			// create a new text line first
			m_TextLines.Add(currTextLine);
			int currTxtIdx = 0;

			TextItem currTextItem = new TextItem(currFont);
//			TextItem firstItem = currTextItem; // preserve the first item
			


			currTextLine.Add(currTextItem);


			while(currTxtIdx<m_Text.Length)
			{

				// search for the first occurence of a backslash
				int bi = m_Text.IndexOfAny(searchchars,currTxtIdx);

				if(bi<0) // nothing was found
				{
					// move the rest of the text to the current item
					currTextItem.m_Text += m_Text.Substring(currTxtIdx,m_Text.Length-currTxtIdx);
					currTxtIdx = m_Text.Length;
				}
				else // something was found
				{
					// first finish the current item by moving the text from
					// currTxtIdx to (bi-1) to the current text item
					currTextItem.m_Text += m_Text.Substring(currTxtIdx,bi-currTxtIdx);
					
					if('\r'==m_Text[bi]) // carriage return character : simply ignore it
					{
						// simply ignore this character, since we search for \n
						currTxtIdx=bi+1;
					}
					else if('\n'==m_Text[bi]) // newline character : create a new line
					{
						currTxtIdx = bi+1;
						// create a new line
						currTextLine = new TextLine();
						m_TextLines.Add(currTextLine);
						// create also a new text item
						currTextItem = new TextItem(currTextItem,null);
						currTextLine.Add(currTextItem);
					}
					else if('\\'==m_Text[bi]) // backslash : look what comes after
					{
						if(bi+1<m_Text.Length && (')'==m_Text[bi+1] || '\\'==m_Text[bi+1])) // if a closing brace or a backslash, take these as chars
						{
							currTextItem.m_Text += m_Text[bi+1];
							currTxtIdx = bi+2;
						}
							// if the backslash not followed by a symbol and than a (, 
						else if(bi+3<m_Text.Length && !char.IsSeparator(m_Text,bi+1) && '('==m_Text[bi+2])
						{
							switch(m_Text[bi+1])
							{
								case 'b':
								case 'B':
								{
									itemstack.Push(currTextItem);
									currTextItem = new TextItem(currTextItem, new Font(currTextItem.Font.FontFamily,currTextItem.Font.Size,currTextItem.Font.Style | FontStyle.Bold, GraphicsUnit.World));
									currTextLine.Add(currTextItem);
									currTxtIdx = bi+3;
								}
									break; // bold
								case 'i':
								case 'I':
								{
									itemstack.Push(currTextItem);
									currTextItem = new TextItem(currTextItem, new Font(currTextItem.Font.FontFamily,currTextItem.Font.Size,currTextItem.Font.Style | FontStyle.Italic, GraphicsUnit.World));
									currTextLine.Add(currTextItem);
									currTxtIdx = bi+3;
								}
									break; // italic
								case 'u':
								case 'U':
								{
									itemstack.Push(currTextItem);
									currTextItem = new TextItem(currTextItem, new Font(currTextItem.Font.FontFamily,currTextItem.Font.Size,currTextItem.Font.Style | FontStyle.Underline, GraphicsUnit.World));
									currTextLine.Add(currTextItem);
									currTxtIdx = bi+3;
								}
									break; // underlined
								case 's':
								case 'S': // strikeout
								{
									itemstack.Push(currTextItem);
									currTextItem = new TextItem(currTextItem, new Font(currTextItem.Font.FontFamily,currTextItem.Font.Size,currTextItem.Font.Style | FontStyle.Strikeout, GraphicsUnit.World));
									currTextLine.Add(currTextItem);
									currTxtIdx = bi+3;
								}
									break; // end strikeout
								case 'g':
								case 'G':
								{
									itemstack.Push(currTextItem);
									currTextItem = new TextItem(currTextItem, new Font("Symbol",currTextItem.Font.Size,currTextItem.Font.Style, GraphicsUnit.World));
									currTextLine.Add(currTextItem);
									currTxtIdx = bi+3;
								}
									break; // underlined
								case '+':
								case '-':
								{
									itemstack.Push(currTextItem);
									// measure the current font size
									float cyLineSpace,cyAscent,cyDescent;
									MeasureFont(g,currTextItem.Font,out cyLineSpace, out cyAscent, out cyDescent);
									
									currTextItem = new TextItem(currTextItem, new Font(currTextItem.Font.FontFamily,0.65f*currTextItem.Font.Size,currTextItem.Font.Style, GraphicsUnit.World));
									currTextLine.Add(currTextItem);
									currTextItem.m_SubIndex += ('+'==m_Text[bi+1] ? 1 : -1);


									if('-'==m_Text[bi+1]) 
										currTextItem.m_yShift += 0.15f*cyAscent; // Carefull: plus (+) means shift down
									else
										currTextItem.m_yShift -= 0.35f*cyAscent; // be carefull: minus (-) means shift up
									
									currTxtIdx = bi+3;
								}
									break; // underlined
								case 'l': // Plot Curve Symbol
								case 'L':
								{
									// parse the arguments
									// either in the Form 
									// \L(PlotCurveNumber) or
									// \L(LayerNumber, PlotCurveNumber) or
									// \L(LayerNumber, PlotCurveNumber, DataPointNumber)


									// find the corresponding closing brace
									int closingbracepos = m_Text.IndexOf(")",bi+1);
									if(closingbracepos<0) // no brace found, so threat this as normal text
									{
										currTextItem.m_Text += m_Text.Substring(bi,3);
										currTxtIdx += 3;
										continue;
									}
									// count the commas between here and the closing brace to get
									// the number of arguments
									int parsepos=bi+3;
									int[] arg = new int[3];
									int args;
									for(args=0;args<3 && parsepos<closingbracepos;args++)
									{
										int commapos = m_Text.IndexOf(",",parsepos,closingbracepos-parsepos);
										int endpos = commapos>0 ? commapos : closingbracepos; // the end of this argument
										try { arg[args]=System.Convert.ToInt32(m_Text.Substring(parsepos,endpos-parsepos)); }
										catch(Exception) { break; }
										parsepos = endpos+1;
									}
									if(args==0) // if not successfully parsed at least one number
									{
										currTextItem.m_Text += m_Text.Substring(bi,3);
										currTxtIdx += 3;
										continue;   // handle it as if it where normal text
									}

									itemstack.Push(currTextItem);
									currTextItem = new TextItem(currTextItem,null);
									currTextLine.Add(currTextItem);
									currTextItem.SetAsSymbol(args,arg);

									currTextItem = new TextItem(currTextItem,null); // create a normal text item behind the symbol item
									currTextLine.Add(currTextItem); // to have room for the following text
									currTxtIdx = closingbracepos+1;
								}
									break; // curve symbol
								default:
									// take the sequence as it is
									currTextItem.m_Text += m_Text.Substring(bi,3);
									currTxtIdx = bi+3;
									break;
							} // end of switch
						}
						else // if no formatting and also no closing brace or backslash, take it as it is
						{
							currTextItem.m_Text += m_Text[bi];
							currTxtIdx = bi+1;
						}
					} // end if it was a backslash
					else if(')'==m_Text[bi]) // closing brace
					{
						// the formating is finished, we can return to the formating of the previous section
						if(itemstack.Count>0)
						{
							TextItem preservedprevious = (TextItem)itemstack.Pop();
							currTextItem = new TextItem(preservedprevious,null);
							currTextLine.Add(currTextItem);
							currTxtIdx = bi+1;
						}
						else // if the stack is empty, take the brace as it is, and use the default style
						{
							currTextItem.m_Text += m_Text[bi];
							currTxtIdx = bi+1;
						}

					}
				}

			} // end of while loop

			this.m_bStructureInSync=true; // now the text was interpreted
		}
	

		protected void MeasureStructure(Graphics g, object obj)
		{
			float currPosX=0;
			float currPosY=0;
			PointF zeroPoint = new PointF(0,0);
			SizeF currSize;
	
			float maxLineWidth=0;
			float maxLineSpace=0;

			
			// Modification of StringFormat is necessary to avoid 
			// too big spaces between successive words
			StringFormat strfmt = StringFormat.GenericTypographic;
			strfmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

			strfmt.LineAlignment = StringAlignment.Far;
			strfmt.Alignment = StringAlignment.Near;

			
			for(int nLine=0;nLine<m_TextLines.Count;nLine++)
			{

				float maxLineAscent=0;
				float maxLineDescent=0;
				float sumItemWidth=0;
				for(int nItem=0;nItem<m_TextLines[nLine].Count;nItem++)
				{

					TextItem ti = m_TextLines[nLine][nItem];

					if(ti.IsEmpty)
					{
						continue;
					}
					else if(ti.IsText)
					{
						MeasureFont(g,ti.Font, out ti.m_cyLineSpace, out ti.m_cyAscent, out ti.m_cyDescent);
						ti.m_Width = g.MeasureString(ti.m_Text, ti.Font, 0, strfmt).Width;
						
						maxLineAscent = Math.Max(ti.m_cyAscent-ti.m_yShift,maxLineAscent);
						maxLineDescent = Math.Max(ti.m_cyDescent+ti.m_yShift,maxLineDescent);
						sumItemWidth += ti.m_Width;
					}

					else if(ti.IsSymbol)
					{
						if(obj is Altaxo.Graph.Layer)
						{
							Graph.Layer layer = (Graph.Layer)obj;
							if(ti.m_LayerNumber>=0 && ti.m_LayerNumber<layer.ParentLayerList.Count)
								layer = layer.ParentLayerList[ti.m_LayerNumber];

							if(ti.m_PlotNumber<layer.PlotAssociations.Count)
							{
								Graph.PlotAssociation pa = layer.PlotAssociations[ti.m_PlotNumber];
								MeasureFont(g,ti.Font,out ti.m_cyLineSpace, out ti.m_cyAscent, out ti.m_cyDescent);
								ti.m_Width = g.MeasureString(ti.m_Text, ti.Font, 0, strfmt).Width;

								maxLineAscent = Math.Max(ti.m_cyAscent-ti.m_yShift,maxLineAscent);
								maxLineDescent = Math.Max(ti.m_cyDescent+ti.m_yShift,maxLineDescent);
								sumItemWidth += ti.m_Width;
							}
						}
					} // end if ti.IsSymbol
				} // end for all items in the line

				// now put the line properties
				m_TextLines[nLine].m_cyAscent = maxLineAscent;
				m_TextLines[nLine].m_cyDescent = maxLineDescent;
				m_TextLines[nLine].m_cyLineSpace = maxLineAscent + maxLineDescent;
				m_TextLines[nLine].m_Width = sumItemWidth;
			} // for all lines

			m_bMeasureInSync = true;
		} // end of function MeasureStructure





		public Font Font
		{
			get
			{
				return m_Font;
			}
			set
			{
				m_Font = value;
				this.m_bStructureInSync=false; // since the font is cached in the structure, it must be renewed
				this.m_bMeasureInSync=false;
			}
		}

		public string Text
		{
			get
			{
				return m_Text;
			}
			set
			{
				m_Text = value;
				this.m_bStructureInSync=false;
			}
		}
		public System.Drawing.Color Color
		{
			get
			{
				return m_Color;
			}
			set
			{
				m_Color = value;
			}
		}


	
		public static void MeasureFont(Graphics g, Font ft, out float cyLineSpace, out float cyAscent, out float cyDescent)
		{	
			// get some properties of the font
			cyLineSpace = ft.GetHeight(g); // space between two lines
			int   iCellSpace  = ft.FontFamily.GetLineSpacing(ft.Style);
			int   iCellAscent = ft.FontFamily.GetCellAscent(ft.Style);
			int   iCellDescent = ft.FontFamily.GetCellDescent(ft.Style);
			cyAscent  = cyLineSpace*iCellAscent/iCellSpace;
			cyDescent = cyLineSpace*iCellDescent/iCellSpace; 
		}

		public override void Paint(Graphics g, object obj)
		{
			if(!this.m_bStructureInSync)
				this.Interpret(g);

			if(!this.m_bMeasureInSync)
				this.MeasureStructure(g,obj);

			System.Drawing.Drawing2D.GraphicsState gs = g.Save();
			
			g.TranslateTransform(X,Y);
			g.RotateTransform(m_Rotation);

			// Modification of StringFormat is necessary to avoid 
			// too big spaces between successive words
			StringFormat strfmt = StringFormat.GenericTypographic;
			strfmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

			strfmt.LineAlignment = StringAlignment.Near;
			strfmt.Alignment = StringAlignment.Near;

			float PlotSymbolWidth = g.MeasureString("MMM",m_Font,new PointF(0,0),strfmt).Width;

			float baseLineSpace, baseAscent, baseDescent;
			MeasureFont(g, m_Font, out baseLineSpace, out baseAscent, out baseDescent);
		

			float currPosX=0;
			float currPosY=0;
			if(m_TextLines.Count>0)
			{
				currPosY = m_TextLines[0].m_cyAscent; 
			}

			SizeF currSize;
			for(int nLine=0;nLine<m_TextLines.Count;nLine++)
			{
				currPosX=0;
				for(int nItem=0;nItem<m_TextLines[nLine].Count;nItem++)
				{

					TextItem ti = m_TextLines[nLine][nItem];

					if(ti.IsEmpty)
						continue;

					if(ti.IsText)
					{
						g.DrawString(ti.m_Text, ti.Font, new SolidBrush(m_Color), new PointF(currPosX, currPosY + ti.m_yShift - ti.m_cyAscent), strfmt);
						Console.WriteLine("{0} {1} {2}",ti.m_Text,ti.m_yShift,ti.m_cyAscent);
						// update positions
						currPosX += ti.m_Width;
					} // end of if ti.IsText

					else if(ti.IsSymbol && obj is Altaxo.Graph.Layer)
					{
						Graph.Layer layer = (Graph.Layer)obj;
						if(ti.m_LayerNumber>=0 && ti.m_LayerNumber<layer.ParentLayerList.Count)
							layer = layer.ParentLayerList[ti.m_LayerNumber];

						if(ti.m_PlotNumber<layer.PlotAssociations.Count)
						{
							Graph.PlotAssociation pa = layer.PlotAssociations[ti.m_PlotNumber];
						
							pa.PlotStyle.PaintSymbol(g, new PointF(currPosX,currPosY + ti.m_yShift - ti.m_cyDescent-ti.m_cyAscent/2+ti.m_cyDescent/4), ti.m_Width);
							currPosX += ti.m_Width;
						}

					} // end if ti.IsSymbol

				} // for all items in a textline
			
			currPosY += baseLineSpace*1.25f;
			} // for all textlines
			


			g.Restore(gs);
		}
	}
}




