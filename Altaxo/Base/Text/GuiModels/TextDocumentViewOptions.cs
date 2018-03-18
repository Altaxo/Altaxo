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

using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Text.GuiModels
{
	public class TextDocumentViewOptions : IProjectItemPresentationModel
	{
		public TextDocument Document { get; protected set; }
		public ViewerConfiguration WindowConfiguration { get; protected set; }
		public bool IsViewerSelected { get; protected set; }

		/// <summary>
		/// The fraction of the width (when shown in left-right configuration) or height (when shown in top-bottom configuration) of the source editor window in relation to the available width/height.
		/// </summary>
		private double _fractionOfSourceEditorWindowVisible = 0.5;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(TextDocumentViewOptions), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			private AbsoluteDocumentPath _pathToDocument;
			private TextDocumentViewOptions _deserializedInstance;

			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (TextDocumentViewOptions)obj;
				info.AddValue("Document", AbsoluteDocumentPath.GetAbsolutePath(s.Document));
				info.AddEnum("WindowConfiguration", s.WindowConfiguration);
				info.AddValue("IsViewerSelected", s.IsViewerSelected);
				info.AddValue("FractionSourceEditor", s._fractionOfSourceEditorWindowVisible);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (TextDocumentViewOptions)o : new TextDocumentViewOptions(info);

				var pathToDocument = (AbsoluteDocumentPath)info.GetValue("Document", s);
				s.WindowConfiguration = (ViewerConfiguration)info.GetEnum("WindowConfiguration", typeof(ViewerConfiguration));
				s.IsViewerSelected = info.GetBoolean("IsViewerSelected");
				s._fractionOfSourceEditorWindowVisible = info.GetDouble("FractionSourceEditor");

				XmlSerializationSurrogate0 surr = new XmlSerializationSurrogate0
				{
					_deserializedInstance = s,
					_pathToDocument = pathToDocument,
				};

				info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.EhDeserializationFinished);

				return s;
			}

			private void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, Main.IDocumentNode documentRoot, bool isFinallyCall)
			{
				object o = AbsoluteDocumentPath.GetObject(_pathToDocument, documentRoot);
				if (o is TextDocument textDoc)
				{
					_deserializedInstance.Document = textDoc;
					info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhDeserializationFinished);
				}
			}
		}

		#endregion Serialization

		protected TextDocumentViewOptions(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
		}

		public TextDocumentViewOptions(TextDocument doc)
		{
			Document = doc ?? throw new ArgumentNullException(nameof(doc));
		}

		IProjectItem IProjectItemPresentationModel.Document => Document;
	}

	/// <summary>
	/// Designates the viewers configuration.
	/// </summary>
	public enum ViewerConfiguration
	{
		/// <summary>The editor window is left, the viewer window is right.</summary>
		EditorLeftViewerRight = 0,

		/// <summary>The editor window is right, the viewer window is left.</summary>
		EditorRightViewerLeft = 1,

		/// <summary>The editor window is top, the viewer window is bottom.</summary>
		EditorTopViewerBottom = 2,

		/// <summary>The editor window is bottom, the viewer window is top.</summary>
		EditorBottomViewerTop = 3,

		/// <summary>The editor window and the viewer window are inside a tabbed control.</summary>
		Tabbed = 4
	}
}
