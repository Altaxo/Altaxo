//This file is part of MathML.Rendering, a library for displaying mathml
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
//(slightly obfuscated for spam mail harvesters)
//andy[at]epsilon3[dot]net

using System;
using System.Windows.Forms;
using System.Xml;
using MathML.Rendering.GlyphMapper;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections;

namespace MathML.Rendering
{
	/// <summary>
	/// Specifies the action that raised a MathMLControlEventArgs event.
	/// </summary>
	public enum MathMLControlAction
	{
		/// <summary>
		/// The event was caused by a keystroke. 
		/// </summary>
		ByKeyboard, 
 
		/// <summary>
		/// The event was caused by a mouse
		/// </summary>
		ByMouse 
	}

	/// <summary>
	/// provides data for all of the After events of the MathMLControl
	/// </summary>
	public class MathMLControlEventArgs : EventArgs
	{
		private MathMLControlAction action;
		private MathMLElement element;

		/// <summary>
		/// creates a new MathMLControlEventArgs
		/// </summary>
		public MathMLControlEventArgs(MathMLElement e, MathMLControlAction a)
		{
			element = e;
			action = a;
		}

		/// <summary>
		/// the action that intiated this event
		/// </summary>
		public MathMLControlAction Action
		{
			get { return action; }
		}

		/// <summary>
		/// the element that was selected
		/// </summary>
		public MathMLElement Element
		{
			get { return element; }
		}
	}

	/// <summary>
	/// represents the method that will handle the After events of the MathMLControl
	/// </summary>
	public delegate void MathMLControlEventHandler(object sender, MathMLControlEventArgs e);
 
	/// <summary>
	/// A control that displays and allows user updating of a mathml document
	/// Big new with this release is element selection is now working
	/// and enabled. Just double click on an element area with the mouse
	/// and it will become selected. An 'AfterSelect' event will be fired
	/// by the control.
	/// 
	/// Experimental mathml editing is also included. This is disabled
	/// by default as it currently does not work very well, to enable
	/// editing, set the 'ReadOnly' property to false. Just select an
	/// element with the mouse to position the cursor. Move the cursor 
	/// with the arrow keys, and start typing to insert items. The 
	/// delete key will delete the item currently after the cursor. 
	/// 
	/// NOTE, editing is VERY EXPERIMENTAL, it WILL CRASH!!! Do not 
	/// enable editing in any sort of released application.
	/// 
	/// This control is work in progress, so  there are
	/// bound to be some problems. PLEASE let me know of ANY problems, 
	/// and let me known if you have ANY questions. send mail to
	/// andy@epsilon3.net
	/// </summary>
	public class MathMLControl : System.Windows.Forms.UserControl
	{
		#region private variables

		// the current root element
		private MathMLMathElement mathElement = null;

		// the root of the area tree
		private Area area = null;

		// root of the formatting tree
		private Area format = null;        

		private int fontSize = FormattingContext.DefaultFontPointSize;

		// can the control edit a document
		private bool readOnly = true;

		private AreaRegion currentRegion = null;

		// is an element selected
		private bool selected = false;

		private BoundingBox box = BoundingBox.New();
		private MathMLElementFinder elementFinder = new MathMLElementFinder();

		// when an element is changed, should automatic re-formatting be suppressed
		private bool suppressFormat = false;

		// the element where the control will position the cursor AFTER 
		// a node has been removed.
		// should allways be null except the time in between a node removing
		// and a node removed event.
		private Selection afterNodeRemovedSelection = null;

		// color of the selected element background
		private Color selectionColor = Color.LightBlue;

		// the element the mouse is in
		private MathMLElement mouseElement = null;

		// the action element the mouse is in
		// can have nested action elements, so need to store
		// as a collection.
		private ArrayList mouseActionElements = new ArrayList(10);

		private ToolTip toolTip = new ToolTip();

		#endregion

		#region events

		/// <summary>
		/// Occurs after a element is selected.
		/// </summary>
		public event MathMLControlEventHandler ElementSelected;	
		
		/// <summary>
		/// Occurs when the mouse pointer hovers enters a mathml element region.
		/// </summary>
		public event MathMLControlEventHandler ElementMouseEnter;

		/// <summary>
		/// Occurs when the mouse pointer hovers over a mathml element region.
		/// </summary>
		public event MathMLControlEventHandler ElementMouseHover;

		/// <summary>
		/// Occurs when the mouse pointer hovers leaves a mathml element region.
		/// </summary>
		public event MathMLControlEventHandler ElementMouseLeave;

		/// <summary>
		/// Occurs when the mouse pointer single clicks on an element
		/// </summary>
		public event MathMLControlEventHandler ElementMouseClick;

		/// <summary>
		/// Occurs when the mouse pointer single clicks on a mathml action element.
		/// </summary>
		/// <remarks>
		/// The Element property of MathMLControlEventArgs will be a MathMLActionElement type.
		/// </remarks>
		public event MathMLControlEventHandler ActionElementMouseClick;

		/// <summary>
		/// Occurs when the mouse pointer enters a mathml action element 
		/// </summary>
		/// <remarks>
		/// The Element property of MathMLControlEventArgs will be a MathMLActionElement type.
		/// </remarks>
		public event MathMLControlEventHandler ActionElementMouseEnter;

		/// <summary>
		/// Occurs when the mouse pointer leaves a mathml action element
		/// </summary>
		/// <remarks>
		/// The Element property of MathMLControlEventArgs will be a MathMLActionElement type.
		/// </remarks>
		public event MathMLControlEventHandler ActionElementMouseLeave;

		#endregion

		/// <summary>
		/// Can this control edit a mathml document. If true, the user can edit a document
		/// via keyboard input, false, the document is treated as readonly and keyboard edit
		/// input is ignored.
		/// 
		/// Editing is very experimential, and will probably crash, use at your own risk.
		/// </summary>
		public bool ReadOnly 
		{
			get { return readOnly; }
			set 
			{
				selected = false;
                readOnly = value;

				if(mathElement != null)
				{
					currentRegion = area.GetRegion(0, box.Height, mathElement, 0);
				}
				ShowCaret();
			}
		}

		/// <summary>
		/// a old lame way to shift the mathml rendering
		/// control now auto-scrolls.
		/// <see cref="AutoScroll"/>
		/// </summary>
		[Obsolete("Will be removed in next version, control now auto-scrolls")]
		public float VerticalShift
		{
			get { return AutoScrollPosition.Y; }
			set 
			{ 
			}
		}

		/// <summary>
		/// a old lame way to shift the mathml rendering
		/// control now auto-scrolls.
		/// <see cref="AutoScroll"/>
		/// </summary>
		[Obsolete("Will be removed in next version, control now auto-scrolls")]
		public float HorizontalShift
		{
			get { return AutoScrollPosition.X; }
			set 
			{ 
			}
		}

		/// <summary>
		/// get or set the current base font size in points. This is the point size
		/// of a standard base level text.
		/// </summary>
		public int MathFontSizeInPoints
		{
			get { return fontSize; }
			set 
			{
				fontSize = value;
				// re-set the math element, this causes a re-layout
				MathElement = mathElement;
			}
		}

		/// <summary>
		/// create the control
		/// sets up the stule so this control is selectable
		/// </summary>
		public MathMLControl()
		{
			SetStyle(ControlStyles.Selectable, true);
			SetStyle(ControlStyles.DoubleBuffer, false);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
		}

		/// <summary>
		/// get or set the current mathml document
		/// Note, this documeht is live, all external updates to the document will be reflected in the 
		/// control.
		/// </summary>
		public MathMLMathElement MathElement
		{
			get { return mathElement; }
			set 
			{
				mathElement = value;
				selected = false;
				currentRegion = null;		

				if(mathElement != null)
				{
					// make sure we do not have any cached areas
					ClearChildAreas(mathElement);

					// listen for document changes.
					mathElement.OwnerDocument.NodeChanged += new XmlNodeChangedEventHandler(OnNodeChanged);
					mathElement.OwnerDocument.NodeRemoved += new XmlNodeChangedEventHandler(OnNodeRemoved);
					mathElement.OwnerDocument.NodeRemoving += new XmlNodeChangedEventHandler(OnNodeRemoving);
					mathElement.OwnerDocument.NodeInserted += new XmlNodeChangedEventHandler(OnNodeInserted);

					// build the formatting tree
					MathMLFormatter formatter = new MathMLFormatter();
					FormattingContext ctx = new FormattingContext(fontSize);
					format = formatter.Format(mathElement, ctx);

					// build the are tree
					BoundingBox box = format.BoundingBox;
					area = format.Fit(box);
				}

				// redraw the control
				Refresh();
			}
		}

		/// <summary>
		/// The currently selected element. Null if no element is selected. 
		/// Can be set a valid element to select an element or null if no 
		/// element is to be selected.
		/// </summary>
		public MathMLElement SelectedElement
		{
			get 
			{ 
				return selected && currentRegion != null ? currentRegion.Element : null;
			}
			set 
			{
				if(value != null)
				{
					if(area != null)
					{
						selected = true;
						currentRegion = area.GetRegion(0, box.Height, value, 0);

						// redraw the control
						Refresh();
						ShowCaret();
					}
				}
				else
				{
					selected = false;
				}
			} 
		}

		/// <summary>
		/// The input location or caret location. The caret is position directly
		/// before this element. Can be set to null to hide the caret.
		/// 
		/// Currently not implemented
		/// </summary>
		public MathMLElement InputLocation
		{
			get { return null; }
			set {}
		}
	
		/// <summary>
		/// override the IsInputKeys so we get arrow key notifications
		/// </summary>
		protected override bool IsInputKey(Keys keyData)
		{
			switch(keyData)
			{
				case Keys.Left:
				case Keys.Right:
				case Keys.Up:
				case Keys.Down:
					return true;
				default:
					return base.IsInputKey (keyData);
			}
		}

		/// <summary>
		/// save the current rendered image of a mathml document to a image file.
		/// </summary>
		/// <param name="filename">The file name to save to</param>
		/// <param name="format">the image file format</param>
		public void Save(string filename, ImageFormat format)
		{
			Image img = GetImage(typeof(Bitmap));
			if(img != null)
			{
				img.Save(filename, format);
			}
		}

		/// <summary>
		/// save the mathml rendering to an image file. uses default
		/// image file format, see Image class
		/// </summary>
		/// <param name="filename"></param>
		public void Save(string filename)
		{
			Save(filename, ImageFormat.Bmp);
		}

        /// <summary>
        /// get or sets the background color of a selected element.
        /// a set causes an automatic redraw.
        /// </summary>
		public Color SelectionColor 
		{
			get { return selectionColor; }
			set 
			{
				selectionColor = value;
				Refresh();
			}
		}

        /// <summary>
        /// copies the current mathml rendering to the clipboard in
        /// bitmap format.
        /// 
        /// This name was ill-chosen, before selection and rendering were 
        /// implmemented, and this will be re-named in future versions.
        /// </summary>
		public void CopyToClipboard()
		{
			CopyToClipboard(typeof(Bitmap));
		}

        /// <summary>
        /// copy the current mathml rendering to the clipboard in a specified
        /// format. The format can be either Bitmap or Metafile for an image 
        /// copy, or it can be String to copy the current selected element
        /// as a string.
        /// </summary>
        /// <param name="type"></param>
		public bool CopyToClipboard(Type type)
		{
			bool result = true;
			if(mathElement != null)
			{
				if(type == typeof(Bitmap))
				{
					Clipboard.SetDataObject(GetImage(type), false);
					result = true;
				}
				else if(type == typeof(Metafile))
				{
					Metafile mf = (Metafile)GetImage(type);
					CopyMetafileToClipboard(mf);
					result = true;
				}
				else if(type == typeof(String))
				{
					if(this.selected && this.currentRegion != null)
					{
						Clipboard.SetDataObject(currentRegion.Element.OuterXml);
					}
					else
					{
						Clipboard.SetDataObject(mathElement.OuterXml);
					}
					result = true;
				}
			}
			return result;
		}

        /// <summary>
        /// copy the currently selected mathml element to the clipboard
        /// </summary>
		public void CopySelectedElementToClipboard()
		{
			if(selected && currentRegion != null)
			{
				Clipboard.SetDataObject(currentRegion.Element, true);
			}
		}

		
		private void InsertFromClipboard()
		{			
			if(!readOnly && currentRegion != null)
			{
				IDataObject dataObj = Clipboard.GetDataObject();
				String[] formats = dataObj.GetFormats();
				foreach(String fmt in dataObj.GetFormats())
				{
					Object obj =  dataObj.GetData(fmt);
					Object o2 = dataObj.GetData(fmt, true);
					if(Insert(dataObj.GetData(fmt))) return;
				}
			}
		}	
	
        /// <summary>
        /// draw the current mathml element (if we have one) to the given
        /// device context clipping to the given size
        /// </summary>
		private void Draw(IntPtr dc, int width, int height, 
			int scrollPosX, int scrollPosY)
		{
			Debug.WriteLine("Draw(width: " + width + ", height: " + height + ")");

			// clear the background in all cases
			IntPtr backBrush = IntPtr.Zero;
			IntPtr bmpHandle = IntPtr.Zero;
			if(BackgroundImage != null)
			{
				Bitmap bmp = BackgroundImage as Bitmap;
				bmpHandle = bmp != null ? bmp.GetHbitmap(BackColor) : new Bitmap(BackgroundImage).GetHbitmap(BackColor);
				backBrush = Win32.CreatePatternBrush(bmpHandle);
			}
			else
			{
				backBrush = Win32.CreateSolidBrush(Win32.RGB(BackColor));
			}			
			IntPtr oldBrush = Win32.SelectObject(dc, backBrush);
			Win32.RECT rect = new Win32.RECT();
			rect.left = 0; rect.top = 0; rect.right = width; rect.bottom = height;
			Win32.FillRect(dc, ref rect, backBrush);
			Win32.SelectObject(dc, oldBrush);
			Win32.DeleteObject(backBrush);
			if(bmpHandle != IntPtr.Zero)
			{
				Win32.DeleteObject(bmpHandle);
			}

			if(area != null)
			{								
				// save the text align mode, and set it to baseline
				uint textAlign = Win32.SetTextAlign(dc, Win32.TA_BASELINE);	
				
				// draw the selection rectangle
				if(selected && currentRegion != null)
				{
					IntPtr pen = Win32.CreatePen(Win32.PS_SOLID, 0, Win32.RGB(selectionColor));
					IntPtr brush = Win32.CreateSolidBrush(Win32.RGB(selectionColor));
					IntPtr b = Win32.SelectObject(dc, brush);
					IntPtr p = Win32.SelectObject(dc, pen);

					BoundingBox areaBox = currentRegion.Area.BoundingBox;

					Win32.RoundRect(dc, 
						scrollPosX+ currentRegion.X, 
						scrollPosY + (currentRegion.Y - areaBox.Height), 
						scrollPosX + (currentRegion.X + areaBox.Width), 
						scrollPosY + (currentRegion.Y + areaBox.Depth), 
						7, 7);

					Win32.SelectObject(dc, b);
					Win32.SelectObject(dc, p);
					Win32.DeleteObject(brush);
					Win32.DeleteObject(pen);
				}	
			
				Win32.SetBkMode(dc, Win32.TRANSPARENT);	

                // draw the mathml elememt to the backbuffer
				GraphicDevice graphics = new GraphicDevice(dc);				
				area.Render(graphics, scrollPosX, scrollPosY + box.Height);
			}
		}

		/// <summary>
		/// prevent clearing of the background, causes flicker
		/// </summary>
		/// <param name="pevent"></param>
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
		}

        /// <summary>
        /// clear the given graphics object and draw the mathml rendering into it
        /// </summary>
        /// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{			
			if(!DesignMode)
			{				
				IntPtr dc = e.Graphics.GetHdc();
			
				IntPtr mathBmpDc = Win32.CreateCompatibleDC(dc);				

				int width = this.Size.Width;
				int height = this.Size.Height;

				IntPtr mathBmpHandle = Win32.CreateCompatibleBitmap(dc, width, height);										

				IntPtr stockHandle = Win32.SelectObject(mathBmpDc, mathBmpHandle);	

				Draw(mathBmpDc, Size.Width, Size.Height, AutoScrollPosition.X, AutoScrollPosition.Y);

				
				Win32.BitBlt(dc, 0, 0, width, height, mathBmpDc, 0, 0, Win32.SRCCOPY);
				e.Graphics.ReleaseHdc(dc);

				Win32.SelectObject(mathBmpDc, stockHandle);
				Win32.DeleteObject(mathBmpHandle);
				Win32.DeleteDC(mathBmpDc);
			}
			else
			{
				// in design mode
				if(BackgroundImage != null)
				{
					e.Graphics.FillRectangle(new TextureBrush(BackgroundImage), 0, 0, Size.Width, Size.Height);
				}
				else
				{
					e.Graphics.Clear(BackColor);
				}
			}
		}

		/// <summary>
		/// Find the MathMLElement from which the item at the given point was
		/// created.
		/// </summary>
		/// <param name="point">Screen coordinated of the point</param>
		/// <returns>A MathMLElement if it under the point, null if no item is under the point</returns>
		public MathMLElement GetElementAtPoint(Point point)
		{
			if(area != null)
			{
				AreaRegion region = area.GetRegion(0, box.Height, (float)point.X - AutoScrollPosition.X, (float)point.Y - AutoScrollPosition.Y);
				if(region != null) return region.Element;
			}
			return null;
		}

        /// <summary>
        /// handle a mouse click event
        /// if we have a single click, move the cursor to the mouse location if we are in edit mode.
        /// In a double click, select the element under the mouse click.
        /// </summary>
        /// <param name="e">mouse event args</param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			selected = false;
			if(area != null)
			{				
				AreaRegion region = area.GetRegion(0, box.Height, e.X - AutoScrollPosition.X, e.Y - AutoScrollPosition.Y);
				if(region != null)
				{
					Debug.WriteLine("Mouse Click, found element: " + region.Element.GetType().ToString() +  region.Element.OuterXml);
					currentRegion = region;
					ShowCaret();

					// fire an event if we select a region
					if(e.Clicks >= 2)
					{
						selected = true;
						OnElementSelected(new MathMLControlEventArgs(region.Element, MathMLControlAction.ByMouse));
					}	
					else
					{						
						OnElementMouseClick(new MathMLControlEventArgs(region.Element, MathMLControlAction.ByMouse));

						// deal with action elements
						// grab all the decenedent action areas
						XmlNode p = region.Element;
						MathMLActionElement action = null;
						while(p != null)
						{
							if((action = p as MathMLActionElement) != null)
							{
								OnActionElementMouseClick(
									new MathMLControlEventArgs(action, MathMLControlAction.ByMouse));
							}
							p = p.ParentNode;
						}
					}
			
					// redraw the control
					Refresh();	
				}							
			}	
			base.OnMouseDown (e);
		}

		private void ShowCaret()
		{
			if(currentRegion != null && !readOnly)
			{
				BoundingBox box = currentRegion.Area.BoundingBox;
				float y = currentRegion.Y - box.Height;
				Win32.DestroyCaret();
				Win32.CreateCaret(Handle, IntPtr.Zero, 1, (int)box.VerticalExtent);
				Win32.SetCaretPos((int)(currentRegion.X + AutoScrollPosition.X), (int)(y + AutoScrollPosition.Y));

				if(!selected)
				{
					Win32.ShowCaret(Handle);
				}
			}
			else
			{
				Win32.DestroyCaret();
			}
		}

        /// <summary>
        /// handle a key press
        /// If we are in edit mode, insert a char at the proper location.
        /// </summary>
        /// <param name="e">KeyPressEventArgs</param>
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			if(!readOnly && currentRegion != null && (Char.IsWhiteSpace(e.KeyChar) || Char.IsLetterOrDigit(e.KeyChar) || Char.IsPunctuation(e.KeyChar)))
			{
				if(currentRegion.Element is MathMLPresentationToken)
				{
					XmlNode textNode = currentRegion.Element.FirstChild;
					textNode.Value = textNode.Value.Substring(0, currentRegion.CharIndex) + 
						e.KeyChar + textNode.Value.Substring(currentRegion.CharIndex);
					currentRegion = GetNextSelection();
					ShowCaret();
				}
				else if(currentRegion.Element is MathMLPresentationContainer)
				{
					XmlNode newNode = currentRegion.Element.OwnerDocument.CreateElement("mi");
					XmlNode textNode = currentRegion.Element.OwnerDocument.CreateTextNode(e.KeyChar.ToString());
					newNode.AppendChild(textNode);
					currentRegion.Element.InsertAfter(newNode, null);
					currentRegion = area.GetRegion(0, box.Height, (MathMLElement)newNode, 1);
					currentRegion.CharIndex = 1;
					ShowCaret();
				}

			}
			base.OnKeyPress (e);
		}

        /// <summary>
        /// handle a key down event (control keys)
        /// if we are in edit mode, take the appropriate action
        /// </summary>
        /// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			AreaRegion r = null;
			this.selected = false;	

			if(!readOnly)
			{		
				switch(e.KeyCode)
				{
					case Keys.Delete:
						DeleteCurrentElement();
						break;
					case Keys.Right:
					{
						if((r = GetNextSelection()) != null)
						{
							currentRegion = r;
							ShowCaret();
						}
					} break;
					case Keys.Left:
					{
						if((r = GetPrevSelection()) != null)
						{
							currentRegion = r;
							ShowCaret();
						}
					} break;
					case Keys.C:
					{
						if(e.Control)
						{
							CopySelectedElementToClipboard();
						}
					} break;
					case Keys.V:
					{
						if(e.Control)
						{
							InsertFromClipboard();
						}
					} break;
				}

				if(r != null) PrintSelection(r);
			}
			
			base.OnKeyDown (e);	

			Refresh();				
		}


		/// <summary>
		/// insert an object at the current edit location.
		/// The object can currently be of type character, string or MathMLElement.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>true if the insert was successfull, false otherwise</returns>
		public virtual bool Insert(Object obj)
		{
			bool result = false;
			if(!readOnly && currentRegion != null)
			{
				MathMLElement elm = null;
				if(obj is System.Char)
				{
				}
				else if((elm = obj as MathMLElement) != null)
				{
					XmlNode parent = currentRegion.Element.ParentNode;
					if(selected)
					{					
						parent.InsertAfter((XmlNode)obj, currentRegion.Element);
					}
					else
					{
						parent.ReplaceChild((XmlNode)obj, currentRegion.Element);
					}
					return result = true;
				}
			}
			return result;
		}

        /// <summary>
        /// delete the element at the current insert (caret) location
        /// </summary>
		protected void DeleteCurrentElement()
		{
			if(currentRegion != null)
			{
				XmlNode p = currentRegion.Element.ParentNode;
				if(p is MathMLPresentationContainer)
				{
					p.RemoveChild(currentRegion.Element);
				}
				else
				{
                    XmlNode row = currentRegion.Element.OwnerDocument.CreateElement("mrow");
					XmlNode sibling = currentRegion.Element.PreviousSibling;
					suppressFormat = true;
					p.RemoveChild(currentRegion.Element);
					suppressFormat = false;
					p.InsertAfter(row, sibling);
					AreaRegion r = area.GetRegion(0, box.Height, (MathMLElement)row, 0);
					currentRegion = r;
					ShowCaret();
				}				
			}
		}

		private void PrintSelection(AreaRegion region)
		{

			if(region.Element != null)
			{
				String s = "<" + region.Element.Name +">";
				XmlNode n = region.Element.ParentNode;
				while(n != null)
				{
					s = "<" + n.Name + ">" + s;
					n = n.ParentNode;
				}
				Debug.WriteLine(s);
			}
			else
			{
				Debug.WriteLine("no node selected");
			}
		}

		private AreaRegion GetNextSelection()
		{
			AreaRegion r = null;
			if(area != null)
			{
				if(currentRegion != null)
				{
					Selection sel = null;
					Debug.WriteLine("NextSel, current element: " + currentRegion.Element.GetType().ToString()  + currentRegion.Element.InnerText + ", index: " + currentRegion.CharIndex);
					if((sel = elementFinder.GetNextSelection(currentRegion.Element, currentRegion.CharIndex)) != null)
					{
						Debug.WriteLine("NextSel, next element: " + sel.Element.GetType().ToString() +  sel.Element.OuterXml + ", index: " + sel.CharIndex);
						r = area.GetRegion(0, box.Height, sel.Element, sel.CharIndex);
						if(r != null)
						{
							Debug.Assert(r != null && r.Area != null && r.Element != null, "invalid region from element");
							r.CharIndex = sel.CharIndex;
						}
						else
						{
							Debug.WriteLine("Error, no area found for " + sel.Element.OuterXml);
						}
					}
				}
			}
			return r;				
		}

		private AreaRegion GetPrevSelection()
		{
			AreaRegion r = null;
			if(area != null)
			{
				if(currentRegion != null)
				{
					Selection sel = null;
					Debug.WriteLine("PrevSel, current element: " + currentRegion.Element.GetType().ToString()  + currentRegion.Element.InnerText + ", index: " + currentRegion.CharIndex);
					if((sel = elementFinder.GetPrevSelection(currentRegion.Element, currentRegion.CharIndex)) != null)
					{
						Debug.WriteLine("PrevSel, prev element: " + sel.Element.GetType().ToString() +  sel.Element.OuterXml + ", index: " + sel.CharIndex);
						r = area.GetRegion(0, box.Height, sel.Element, sel.CharIndex);
						if(r != null)
						{
							Debug.Assert(r != null && r.Area != null && r.Element != null, "invalid region from element");
							r.CharIndex = sel.CharIndex;
						}
						else
						{
							Debug.WriteLine("Error, no area found for " + sel.Element.OuterXml);
						}
					}
				}
			}
			return r;				
		}

		/// <summary>
		/// Draw the current mathml equation to an image object.
		/// This method replaces the Metafile property.
		/// </summary>
		/// <param name="type">The type of image to return, currently this can be
		/// either Bitmap or Metafile</param>
		/// <returns>A new image, null if an invalid type is given or there is no current element</returns>
		public Image GetImage(Type type)
		{
			Image image = null;
			int height = (int)Math.Ceiling(box.VerticalExtent);
			int width = (int)Math.Ceiling(box.HorizontalExtent);

			if(type.Equals(typeof(Bitmap)))
			{
				image = new Bitmap(width, height);
			}
			else if(type.Equals(typeof(Metafile)))
			{
				IntPtr screenDc = Win32.GetDC(IntPtr.Zero);
				image = new Metafile(new MemoryStream(), screenDc, EmfType.EmfOnly);
				Win32.ReleaseDC(IntPtr.Zero, screenDc);
			}

			if(image != null && area != null)
			{
				IntPtr screenDc = Win32.GetDC(IntPtr.Zero);			
				IntPtr mathBmpDc = Win32.CreateCompatibleDC(screenDc);	
				IntPtr mathBmpHandle = Win32.CreateCompatibleBitmap(screenDc, width, height);	
				Win32.ReleaseDC(IntPtr.Zero, screenDc);						

				Win32.SetBkMode(mathBmpDc, Win32.TRANSPARENT);							

				IntPtr stockHandle = Win32.SelectObject(mathBmpDc, mathBmpHandle);	

        Graphics gMF =  Graphics.FromImage(image);
        IntPtr hMF = gMF.GetHdc();
				//Draw(mathBmpDc, width, height, 0, 0);						
        Draw(hMF, width, height, 0, 0);						
        gMF.ReleaseHdc(hMF);
        gMF.Dispose();

        /*
				Graphics g = Graphics.FromImage(image);				

				g.FillRectangle(new SolidBrush(Color.White), 0, 0, width, height);
				
				IntPtr dc = g.GetHdc();

				Win32.BitBlt(dc, 0, 0, width, height, mathBmpDc, 0, 0, Win32.SRCCOPY);				

				g.ReleaseHdc(dc);

				g.Dispose();
				*/

				Win32.SelectObject(mathBmpDc, stockHandle);
				Win32.DeleteObject(mathBmpHandle);
				Win32.DeleteDC(mathBmpDc);
        
			}

			return image;
		}

		/// <summary>
		/// Draw the current mathml document to a metafile and return it.
		/// This method is obsolete, has been replaced by GetImage(ImageFormat.Wmf).
		/// </summary>
		[Obsolete("Will be removed in next version, replaced by GetImage(ImageFormat.Wmf)")]
		public Metafile Metafile
		{
			get
			{
				return (Metafile)GetImage(typeof(Metafile));
			}
		}	

		private static void ClearChildAreas(XmlNode e)
		{
			foreach(XmlNode n in e.ChildNodes)
			{
				ClearChildAreas(n);
			}
			MathMLElement elm = e as MathMLElement;
			if(elm != null) 
			{
				Area.SetArea(elm, null);
			}
		}

		private static void ClearArea(XmlNode e)
		{
			while(e != null)
			{
				MathMLElement element = e as MathMLElement;
				MathMLTableElement table = e as MathMLTableElement;

				if(element != null)
				{
					Area.SetArea(element, null);
				}

				if(table != null)
				{
					foreach(MathMLTableRowElement row in table.Rows)
					{
						foreach(MathMLTableCellElement cell in row.Cells)
						{
							Area.SetArea(cell, null);
						}
						Area.SetArea(row, null);
					}
				}
				XmlAttribute attr = e as XmlAttribute;
				e = attr != null ? attr.OwnerElement : e.ParentNode;
			}
		}

        /// <summary>
        /// override the mouse move event to determine which element the mouse is entering
        /// or leaving
        /// </summary>
        /// <param name="e">event args</param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if(area != null)
			{
				ArrayList actions = new ArrayList(10);
				AreaRegion region = area.GetRegion(0, box.Height, e.X - AutoScrollPosition.X, e.Y - AutoScrollPosition.Y);
				MathMLElement newElement = region != null ? region.Element : null;				

				if(mouseElement != newElement)
				{
					OnElementMouseLeave(new MathMLControlEventArgs(mouseElement, MathMLControlAction.ByMouse));
					mouseElement = newElement;
					if(mouseElement != null)
					{
						OnElementMouseEnter(new MathMLControlEventArgs(mouseElement, MathMLControlAction.ByMouse));
					}
				}


				// grab all the decenedent action areas
				XmlNode p = newElement;
				MathMLActionElement action = null;
				while(p != null)
				{
					if((action = p as MathMLActionElement) != null)
					{
						actions.Add(action);
					}
					p = p.ParentNode;
				}

				// look if we have any new areas
				for(int i = 0; i < actions.Count; i++)
				{
					if(!mouseActionElements.Contains(actions[i]))
					{
						OnActionElementMouseEnter(
							new MathMLControlEventArgs((MathMLElement)actions[i], MathMLControlAction.ByMouse));
					}
				}

				// look for old areas
				for(int i = 0; i < mouseActionElements.Count; i++)
				{
					if(!actions.Contains(mouseActionElements[i]))
					{
						OnActionElementMouseLeave(
							new MathMLControlEventArgs((MathMLElement)mouseActionElements[i], MathMLControlAction.ByMouse));
					}
				}

				// new elements
				mouseActionElements = actions;				
			}
			base.OnMouseMove (e);
		}


		#region mathml dom event handlers

        /// <summary>
        /// called whenever a node is changed in the currently loaded mathml document
        /// </summary>
        /// <param name="sender">the sender of the event</param>
        /// <param name="e">event args</param>
		protected virtual void OnNodeChanged(object sender, XmlNodeChangedEventArgs e)
		{
			Debug.WriteLine("node changed: <" + e.Node.Name + ">, " + e.Node.InnerXml);
			ReformatNode(e.Node);			
		}

        /// <summary>
        /// called when a node is being removed.
        /// This sets up the next node to be selected after the current node is removed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnNodeRemoving(object sender, XmlNodeChangedEventArgs e)
		{
			// find the next caret location
			MathMLElement element = e.Node as MathMLElement;
			if(element != null)
			{
				Selection prevSel = elementFinder.GetPrevSelection(element, 0);
				Debug.WriteLineIf(prevSel == null, "Strange, prevSel is null when an element is removing, this should not happen");
				if(prevSel != null)
				{
					afterNodeRemovedSelection = prevSel;	
				}
			}
		}

        /// <summary>
        /// called whenever a node is removed from currently loaded mathml document.
        /// </summary>
        /// <param name="sender">the sender of the event</param>
        /// <param name="e">arguments for event</param>
		protected virtual void OnNodeRemoved(object sender, XmlNodeChangedEventArgs e)
		{
            Debug.WriteLine("node removed: <" + e.Node.Name + ">, " + e.Node.InnerXml);
		
			if(mathElement != null)
			{
				ClearArea(e.OldParent);

				if(!suppressFormat)
				{
					// build the formatting tree
					MathMLFormatter formatter = new MathMLFormatter();
					FormattingContext ctx = new FormattingContext(fontSize);
					format = formatter.Format(mathElement, ctx);

					// build the are tree
					BoundingBox box = format.BoundingBox;
					area = format.Fit(box);

					// set the new insert location
					if(afterNodeRemovedSelection != null)
					{
						// find the next selection, location where area was removed
						Selection newSel = elementFinder.GetNextSelection(afterNodeRemovedSelection.Element, afterNodeRemovedSelection.CharIndex);
						Debug.WriteLineIf(newSel == null, "Strange, newSel is null when an element is removing, this should not happen");
						if(newSel != null)
						{
							AreaRegion region = area.GetRegion(0, box.Height, newSel.Element, 0);
							Debug.Assert(region != null, "error, area returned null region after node was removed");
							if(region != null)
							{
								currentRegion = region;
								ShowCaret();
							}			
						}						
					}
				}
			}

			// redraw the control
			Refresh();			
		}

        /// <summary>
        /// called whenever a node is being inserted
        /// causes a re-layout 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnNodeInserted(object sender, XmlNodeChangedEventArgs e)
		{
			Debug.WriteLine("node inserted: <" + e.Node.Name + ">, " + e.Node.InnerXml);
			ReformatNode(e.Node);				
		}

#endregion

		#region event invokers

		/// <summary>
		/// Raises the ElementSelected event.
		/// called whenever a user selects a element. Currently a user can only select an
		/// element with a double click.
		/// the base implementation raises the AfterSelect event.
		/// </summary>
		/// <param name="args">event arguments</param>
		/// <remarks>
		/// Notes to Inheritors:  When overriding OnElementSelected in a derived class, be sure to call 
		/// the base class's OnElementSelected method so that registered delegates receive the event.
		/// </remarks>
		protected virtual void OnElementSelected(MathMLControlEventArgs args)
		{
			if(ElementSelected != null)
			{
				ElementSelected(this, args);
			}
		}

        /// <summary>
        /// Called when the mouse pointer enters a mathml element region.
        /// Raises the ElementMouseEnter event. 
        /// </summary>
        /// <param name="args">event arguments</param>
        /// <remarks>
        /// Notes to Inheritors:  When overriding OnElementMouseEnter in a derived class, be sure to call 
        /// the base class's OnElementMouseEnter method so that registered delegates receive the event.
        /// </remarks>
		protected virtual void OnElementMouseEnter(MathMLControlEventArgs args)
		{
			if(ElementMouseEnter != null)
			{
				ElementMouseEnter(this, args);
			}
		}

		/// <summary>
		/// Called when the mouse pointer leaves a mathml element region.
		/// Raises the ElementMouseLeave event. 
		/// </summary>
		/// <param name="args">event arguments</param>
		/// <remarks>
		/// Notes to Inheritors:  When overriding ElementMouseLeave in a derived class, be sure to call 
		/// the base class's ElementMouseLeave method so that registered delegates receive the event.
		/// </remarks>
		protected virtual void OnElementMouseLeave(MathMLControlEventArgs args)
		{
			if(ElementMouseLeave != null)
			{
				ElementMouseLeave(this, args);
			}
		}

		/// <summary>
		/// Called when the mouse pointer hovers over a mathml element region.
		/// Raises the ElementMouseHover event. 
		/// </summary>
		/// <param name="args">event arguments</param>
		/// <remarks>
		/// Notes to Inheritors:  When overriding ElementMouseHover in a derived class, be sure to call 
		/// the base class's ElementMouseHover method so that registered delegates receive the event.
		/// </remarks>
		protected virtual void OnElementMouseHover(MathMLControlEventArgs args)
		{
			if(ElementMouseHover != null)
			{
				ElementMouseHover(this, args);
			}
		}

		/// <summary>
		/// Called when the mouse pointer hovers over a mathml element region.
		/// Raises the ElementMouseHover event. 
		/// </summary>
		/// <param name="args">event arguments</param>
		/// <remarks>
		/// Notes to Inheritors:  When overriding ElementMouseHover in a derived class, be sure to call 
		/// the base class's ElementMouseHover method so that registered delegates receive the event.
		/// </remarks>
		protected virtual void OnElementMouseClick(MathMLControlEventArgs args)
		{
			if(ElementMouseClick != null)
			{
				ElementMouseClick(this, args);
			}
		}

		/// <summary>
		/// Called when the mouse pointer leaves a mathml action element region.
		/// If an action element has an action type of "highlight", this will turn
		/// highlighting off. If the action element has an action type of "tooltip", the
		/// tooltip will be de-activated.
		/// Raises the ActionElementMouseLeave event. 
		/// </summary>
		/// <param name="args">event arguments</param>
		/// <remarks>
		/// Notes to Inheritors:  When overriding OnActionElementMouseLeave in a derived class, be sure to call 
		/// the base class's OnActionElementMouseLeave method so that registered delegates receive the event.
		/// </remarks>
		protected virtual void OnActionElementMouseLeave(MathMLControlEventArgs args)
		{
			MathMLActionElement action = (MathMLActionElement)args.Element;			
			if(action != null && action.ActionType != null && String.Compare(action.ActionType, "highlight", true) == 0)
			{
				// kind of lame, but can't think of a better way to notify formatter of 
				// mouse state of an action element
				action.SetUserData("mouseenter", false, null);
				ReformatNode(action);
			}

			if(action != null && action.ActionType != null && String.Compare(action.ActionType, "tooltip", true) == 0)
			{
				toolTip.SetToolTip(this, "");
				toolTip.Active = false;
			}

			if(ActionElementMouseLeave != null)
				ActionElementMouseLeave(this, args);
		}

		/// <summary>
		/// Called when the mouse pointer enters a mathml action element region.
		/// If the action element has an action type of "highlight", this will turn
		/// highlighting on. If the action element has an action type of "tooltip" and
		/// there is a second child element that contains text, the text will be
		/// displayed as a tool tip over the action element. 
		/// Raises the ActionElementMouseEnter event. 
		/// </summary>
		/// <param name="args">event arguments</param>
		/// <remarks>
		/// Notes to Inheritors:  When overriding OnActionElementMouseEnter in a derived class, be sure to call 
		/// the base class's OnActionElementMouseEnter method so that registered delegates receive the event.
		/// </remarks>
		protected virtual void OnActionElementMouseEnter(MathMLControlEventArgs args)
		{
			MathMLActionElement action = (MathMLActionElement)args.Element;			
            if(action != null && action.ActionType != null && String.Compare(action.ActionType, "highlight", true) == 0)
			{
				// kind of lame, but can't think of a better way to notify formatter of 
				// mouse state of an action element
				action.SetUserData("mouseenter", true, null);
				ReformatNode(action);
			}
	
			if(action != null && action.ActionType != null && String.Compare(action.ActionType, "tooltip", true) == 0)
			{
				String text = "";
				XmlNode mtext = action.ChildNodes.Count >= 2 ? action.ChildNodes[1] : null;
				if(mtext != null)
				{
					XmlNode textNode = mtext.FirstChild;
					if(textNode is XmlText)
						text = textNode.Value;
				}
				toolTip.ReshowDelay = 0;
				toolTip.InitialDelay = 0;
				toolTip.SetToolTip(this, text);
				toolTip.Active = true;
			}
		
			if(ActionElementMouseEnter != null)
				ActionElementMouseEnter(this, args);
		}

		/// <summary>
		/// Called when there is a single click in side a region bordered by an action element.
		/// If an action elememnt has an action type of "toggle", this will toggle the element.
		/// Raises the ActionElementMouseClick event. 
		/// </summary>
		/// <param name="args">event arguments</param>
		/// <remarks>
		/// Notes to Inheritors:  When overriding OnActionElementMouseClick in a derived class, be sure to call 
		/// the base class's OnActionElementMouseClick method so that registered delegates receive the event.
		/// </remarks>
		protected virtual void OnActionElementMouseClick(MathMLControlEventArgs args)
		{
			MathMLActionElement action = args.Element as MathMLActionElement;
            
			if(action != null && action.ActionType != null && String.Compare(action.ActionType, "toggle", true) == 0)
			{
				if(action.ChildNodes != null && action.Selection + 1 < action.ChildNodes.Count)
					action.Selection++;
				else
					action.Selection = 0;
			}

			if(ActionElementMouseClick != null)
                ActionElementMouseClick(this, args);
		}

		#endregion

        /// <summary>
        /// Override the Refresh method to set the new scroll min size for the current
        /// mathml element and also set the cached area size.
        /// </summary>
		public override void Refresh()
		{
			Debug.WriteLine("Refresh()");

			if(area != null)
			{
				box = area.BoundingBox;
				// set the auto scroll min size, this determines how much shift is in the
				// AutoScrollPosition values.
				AutoScrollMinSize = new Size(Convert.ToInt32(box.HorizontalExtent), Convert.ToInt32(box.VerticalExtent));
			}
			base.Refresh ();
		}

		private void ReformatNode(XmlNode e)
		{
			if(mathElement != null)
			{
				ClearArea(e);

				// build the formatting tree
				MathMLFormatter formatter = new MathMLFormatter();
				FormattingContext ctx = new FormattingContext(fontSize);
				format = formatter.Format(mathElement, ctx);

				// build the area tree
				Size size = this.Size;
				BoundingBox box = format.BoundingBox;
				area = format.Fit(box);
			}

			// redraw the control
			Refresh();		
		}


		/// <summary>
		/// Due to a bug in the .net Clipboard object, we can not use it to
		/// past a metafile to the clipboard. We need to do it the old fasioned way.
		/// Metafile mf is set to a state that is not valid inside this function.
		/// </summary>
		/// <param name="mf"></param>
		/// <returns></returns>
		private bool CopyMetafileToClipboard(Metafile mf)
		{
			bool bResult = false;
			IntPtr hEMF, hEMF2;
			hEMF = mf.GetHenhmetafile(); // invalidates mf
			if( ! hEMF.Equals( new IntPtr(0) ) )
			{
				hEMF2 = Win32.CopyEnhMetaFile( hEMF, new IntPtr(0) );
				if( ! hEMF2.Equals( new IntPtr(0) ) )
				{
					if( Win32.OpenClipboard( this.Handle ) )
					{
						if( Win32.EmptyClipboard() )
						{
							IntPtr hRes = Win32.SetClipboardData( 14 /*CF_ENHMETAFILE*/, hEMF2 );
							bResult = hRes.Equals( hEMF2 );
							Win32.CloseClipboard();
						}
					}
				}
				Win32.DeleteEnhMetaFile( hEMF );
			}
			return bResult;
		}

        /// <summary>
        /// Need to override the winproc so the control is fully invalited at each
        /// message that can cause scroll event.
        /// </summary>
        /// <param name="m"></param>
		protected override void WndProc(ref Message m)
		{
			const int SBM_SETSCROLLINFO = 0x00E9;
			const int WM_HSCROLL = 0x115;
			const int WM_VSCROLL = 0x114;
			const int WM_MOUSEWHEEL = 0x020A;			

			if (m.Msg == WM_HSCROLL || m.Msg == WM_VSCROLL || m.Msg == SBM_SETSCROLLINFO || m.Msg == WM_MOUSEWHEEL)
			{
				Invalidate();
			}

			base.WndProc(ref m);
		}
	}
}
