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

#endregion Copyright

using Altaxo.Graph.Graph3D;
using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph3D.GuiModels
{
	/// <summary>
	/// Stores information about how a graph is shown in the graph view.
	/// </summary>
	public class GraphViewOptions
	{
		private Altaxo.Graph.Graph3D.GraphDocument _graphDocument;

		/// <summary>
		/// If null, the global settings are used to decide to show root layer markers. If true, root layer markers will be shown. If false, root layer markers will not be shown.
		/// </summary>
		private RootLayerMarkersVisibility? _rootLayerMarkersVisibility;

		/// <summary>Initializes a new instance of the <see cref="GraphViewOptions"/> class.</summary>
		/// <param name="graphDocument">The graph document.</param>
		/// <param name="rootLayerMarkersVisibility">The visibility of the root layer markers.</param>
		public GraphViewOptions(GraphDocument graphDocument, RootLayerMarkersVisibility? rootLayerMarkersVisibility)
		{
			_graphDocument = graphDocument;
			_rootLayerMarkersVisibility = rootLayerMarkersVisibility;
		}

		/// <summary>Private constructor for deserialization.</summary>
		private GraphViewOptions(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
		}

		#region Serialization

		/// <summary>
		/// 2013-12-01: added _positionOfViewportsUpperLeftCornerInRootLayerCoordinates. Zoom and ViewPortOffset is serialized only if AutoZoom is false.
		/// 2015-11-14 Version 2 Moved to Altaxo.Graph.Gdi namespace
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphViewOptions), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			private AbsoluteDocumentPath _pathToGraphDocument;
			private GraphViewOptions _deserializedObject;

			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (GraphViewOptions)obj;
				info.AddValue("Graph", AbsoluteDocumentPath.GetAbsolutePath(s._graphDocument));
				info.AddNullableEnum("RootLayerMarkersVisibility", s._rootLayerMarkersVisibility);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (GraphViewOptions)o ?? new GraphViewOptions(info);

				var pathToGraph = (AbsoluteDocumentPath)info.GetValue("Graph", s);
				s._rootLayerMarkersVisibility = info.GetNullableEnum<RootLayerMarkersVisibility>("RootLayerMarkersVisibility");

				var surr = new XmlSerializationSurrogate0() { _deserializedObject = s, _pathToGraphDocument = pathToGraph };
				info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.EhDeserializationFinished);

				return s;
			}

			private void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, IDocumentNode documentRoot, bool isFinallyCall)
			{
				var o = AbsoluteDocumentPath.GetObject(_pathToGraphDocument, documentRoot);
				if (o is GraphDocument)
				{
					_deserializedObject._graphDocument = (GraphDocument)o;
				}

				if (null != _deserializedObject._graphDocument || isFinallyCall)
				{
					info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhDeserializationFinished);
				}
			}
		}

		#endregion Serialization

		/// <summary>Get the instance of the graph document that is shown in the view.</summary>
		public GraphDocument GraphDocument { get { return _graphDocument; } }

		public RootLayerMarkersVisibility? RootLayerMarkersVisibility { get { return _rootLayerMarkersVisibility; } }
	}
}