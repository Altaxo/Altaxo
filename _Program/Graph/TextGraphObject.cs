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
		public override void Paint(Graphics g)
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
		protected internal bool m_bBold; // true if bold should be used
		protected internal bool m_bGreek; // true if greek charset should be used
		protected internal int  m_SubIndex;

		public TextItem()
		{
			m_Text="";
		}

		public TextItem(TextItem from)
		{
			m_Text="";
			m_bUnderlined = from.m_bUnderlined;
			m_bBold       = from.m_bBold;
			m_bGreek = from.m_bGreek;
			m_SubIndex = from.m_SubIndex;
		}
	}

	public class TextLine
	{
		protected internal System.Collections.ArrayList m_TextItems; // holds a collection of textitems
	

		public TextItem this[int i]
		{
			get { return (TextItem)m_TextItems[i]; }
			set 
			{
				if(i<m_TextItems.Count)
					m_TextItems[i] = value;
				else if(i==m_TextItems.Count)
					m_TextItems.Add(value);
				else
					throw new System.ArgumentOutOfRangeException("i",i,"The index was not in the valid range");
			}
		}

		public class TextLineCollection : System.Collections.ArrayList
		{
			public TextItem this[int i]
			{
				get { return (TextItem)base[i]; }
				set { base[i] = value; }
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

		





#region "Constructors"

		public ExtendedTextGraphObject()
		{
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


		public void Interpret()
		{
			char[] searchchars = new Char[] { '\\', '\n', ')' };
				
			TextLine currTextLine = new TextLine();
				
			// create a new text line first
			m_TextLines.Add(currTextLine);
			int currTxtIdx = 0;

			TextItem currTextItem = new TextItem();
			TextItem previousTextItem = currTextItem;

			currTextLine.m_TextItems.Add(currTextItem);

			// search for the first occurence of a backslash
			int bi = m_Text.IndexOfAny(searchchars,currTxtIdx);

			if(bi<0) // nothing was found
			{
				// move the rest of the text to the current item
				currTextItem.m_Text += m_Text.Substring(currTxtIdx,m_Text.Length-currTxtIdx);
			}
			else // something was found
			{
				if('\n'==m_Text[bi] && (bi+1)<m_Text.Length)
				{
					// then finish the current item by moving the text from
					// currTxtIdx to (bi-1) to the current text item
					currTextItem.m_Text += m_Text.Substring(currTxtIdx,bi-currTxtIdx-1);
					currTxtIdx = bi+1;
					// create a new line
					currTextLine = new TextLine();
					m_TextLines.Add(currTextLine);
					// create also a new text item
					previousTextItem = currTextItem;
					currTextItem = new TextItem(currTextItem);
					currTextLine.m_TextItems.Add(currTextItem);
				}
				else if('\\'==m_Text[bi])
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
								if(currTextItem.m_bBold==false)
								{
									currTextItem = new TextItem(currTextItem);
									currTextLine.m_TextItems.Add(currTextItem);
									currTextItem.m_bBold = true;
								}
								break; // bold
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
				else if(')'==m_Text[bi])
				{
					// the formating is finished, we can return to the formating of the previous section
					TextItem preservedprevious = previousTextItem;
					previousTextItem = currTextItem;
					currTextItem = new TextItem(preservedprevious);
					currTxtIdx = bi+1;
				}
			}
		}
	






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
		public override void Paint(Graphics g)
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



}
