#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Serialization;
using ICSharpCode.SharpDevelop.Gui;


namespace Altaxo.Gui.SharpDevelop
{
	public class SDGraphViewContent : AbstractViewContent, Altaxo.Gui.IMVCControllerWrapper, IClipboardHandler
	{
		Altaxo.Gui.Graph.Viewing.GraphController _controller;

		/*
				#region Serialization
				[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoSDGui","Altaxo.Graph.GUI.SDGraphController",0)]
					class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
				{
					public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
					{
						throw new NotImplementedException("Serialization of old versions is not supported");
		//        info.AddBaseValueEmbedded(obj,typeof(SDGraphController).BaseType);
					}
					public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
					{
						throw new NotImplementedException();
      
					 var s = new Altaxo.Gui.Graph.Viewing.GraphController(null,true);
						info.GetBaseValueEmbedded(s,typeof(WinFormsGraphController),parent);
        
						return new SDGraphViewContent(s);
      
					}
				}

				[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SDGraphViewContent), 1)]
				class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
				{
					public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
					{
						SDGraphViewContent s = (SDGraphViewContent)obj;
						info.AddValue("Controller", s._guiIndependentController);
					}
					public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
					{
						var wc = (Altaxo.Gui.Graph.Viewing.GraphController)info.GetValue("Controller", parent);
						SDGraphViewContent s = null != o ? (SDGraphViewContent)o : new SDGraphViewContent(wc);
						s._guiIndependentController = wc;
						s._controller = (WinFormsGraphController)wc.InternalGetGuiController();
						return s;
					}
				}
				#endregion
				*/

		#region Constructors
		/// <summary>
		/// Creates a GraphController which shows the <see cref="GraphDocument"/> <paramref name="graphdoc"/>.    
		/// </summary>
		/// <param name="graphdoc">The graph which holds the graphical elements.</param>
		public SDGraphViewContent(GraphDocument graphdoc)
			: this(graphdoc, false)
		{
		}

		/// <summary>
		/// Creates a GraphController which shows the <see cref="GraphDocument"/> <paramref name="graphdoc"/>.
		/// </summary>
		/// <param name="graphdoc">The graph which holds the graphical elements.</param>
		/// <param name="bDeserializationConstructor">If true, this is a special constructor used only for deserialization, where no graphdoc needs to be supplied.</param>
		protected SDGraphViewContent(GraphDocument graphdoc, bool bDeserializationConstructor)
			: this(new Altaxo.Gui.Graph.Viewing.GraphControllerWpf(graphdoc))
		{
		}

		public SDGraphViewContent(Altaxo.Gui.Graph.Viewing.GraphController ctrl)
		{
			_controller = ctrl;
			_controller.TitleNameChanged += new WeakEventHandler(this.EhTitleNameChanged, x => _controller.TitleNameChanged -= x);
			SetTitle();
		}

		public override void Dispose()
		{
			base.Dispose();
			
			if(null!=_controller)
			{
				_controller.Dispose();
			_controller = null;
			}
		}

		void EhTitleNameChanged(object sender, EventArgs e)
		{
			SetTitle();
		}

		void SetTitle()
		{
			if (_controller != null && _controller.Doc != null)
				this.TitleName = _controller.Doc.Name;
		}

		#endregion

		public static implicit operator Altaxo.Gui.Graph.Viewing.GraphController(SDGraphViewContent ctrl)
		{
			return ctrl._controller;
		}

		public Altaxo.Gui.Graph.Viewing.GraphController Controller
		{
			get { return _controller; }
		}

		public Altaxo.Gui.IMVCANController MVCController
		{
			get { return _controller; }
		}

		#region Abstract View Content overrides

		#region Required
		public override object Control
		{
			get { return _controller.ViewObject; }
		}

		public override object InitiallyFocusedControl
		{
			get
			{
				return (_controller.ViewObject as Altaxo.Gui.Graph.Viewing.IGraphView).GuiInitiallyFocusedElement;
			}
		}
		#endregion

		#endregion

		#region IEditable Members

		public string Text
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		#endregion

		#region IClipboardHandler Members

		public bool EnableCut
		{
			get { return true; }
		}

		public bool EnableCopy
		{
			get { return true; }
		}

		public bool EnablePaste
		{
			get { return true; }
		}

		public bool EnableDelete
		{
			get { return true; }
		}

		public bool EnableSelectAll
		{
			get { return false; }
		}

		public void Cut()
		{
			_controller.CutSelectedObjectsToClipboard();
		}

		public void Copy()
		{
			_controller.CopySelectedObjectsToClipboard();
		}

		public void Paste()
		{
			_controller.PasteObjectsFromClipboard();
		}

		public void Delete()
		{
			if (_controller.SelectedObjects.Count > 0)
			{
				_controller.RemoveSelectedObjects();
			}
			else
			{
				// nothing is selected, we assume that the user wants to delete the worksheet itself
				Current.ProjectService.DeleteGraphDocument(_controller.Doc, false);
			}
		}

		public void SelectAll()
		{
		}

		#endregion
	}

}
