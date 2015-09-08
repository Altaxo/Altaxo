#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

using Altaxo.Collections;
using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Graph3D
{
	public class HostLayer3D :
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		ITreeListNodeWithParent<HostLayer3D>,
		IGraphicBase3D,
		Altaxo.Main.INamedObjectCollection
	{
		#region Cached member variables

		/// <summary>
		/// The cached size of the parent layer. If this here is the root layer, and hence no parent layer exist, the cached size is set to 100 x 100 mm².
		/// </summary>
		protected VectorD3D _cachedParentLayerSize = new VectorD3D((1000 * 72) / 254.0, (1000 * 72) / 254.0, (1000 * 72) / 254.0);

		/// <summary>
		/// The cached layer position in points (1/72 inch) relative to the upper left corner
		/// of the parent layer (upper left corner of the printable area).
		/// </summary>
		protected PointD3D _cachedLayerPosition;

		/// <summary>
		/// The absolute size of the layer in points (1/72 inch).
		/// </summary>
		protected VectorD3D _cachedLayerSize;

		protected MatrixD3D _transformation = MatrixD3D.Identity;

		/// <summary>
		/// The child layers of this layers (this is a partial view of the <see cref="_graphObjects"/> collection).
		/// </summary>
		protected IObservableList<HostLayer3D> _childLayers;

		/// <summary>
		/// The number of this layer in the parent's layer collection.
		/// </summary>
		protected int _cachedLayerNumber;

		#endregion Cached member variables

		#region Event definitions

		/// <summary>Fired when the size of the layer changed.</summary>
		[field: NonSerialized]
		public event System.EventHandler SizeChanged;

		/// <summary>Fired when the position of the layer changed.</summary>
		[field: NonSerialized]
		public event System.EventHandler PositionChanged;

		/// <summary>Fired when the child layer collection changed.</summary>
		[field: NonSerialized]
		public event System.EventHandler LayerCollectionChanged;

		#endregion Event definitions

		protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			throw new NotImplementedException();
		}

		#region ITreeListNodeWithParent implementation

		IList<HostLayer3D> ITreeListNode<HostLayer3D>.ChildNodes
		{
			get { return _childLayers; }
		}

		IEnumerable<HostLayer3D> ITreeNode<HostLayer3D>.ChildNodes
		{
			get { return _childLayers; }
		}

		Main.IDocumentLeafNode INodeWithParentNode<Main.IDocumentLeafNode>.ParentNode
		{
			get { return _parent; }
		}

		HostLayer3D INodeWithParentNode<HostLayer3D>.ParentNode
		{
			get { return _parent as HostLayer3D; }
		}

		#endregion ITreeListNodeWithParent implementation

		#region IGraphicBase3D

		public PointD3D Position
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public void SetParentSize(VectorD3D parentSize, bool isTriggeringChangedEvent)
		{
			throw new NotImplementedException();
		}

		public void FixupInternalDataStructures()
		{
			throw new NotImplementedException();
		}

		public void PaintPreprocessing(IGraphicContext3D context)
		{
			throw new NotImplementedException();
		}

		public void Paint(IGraphicContext3D context)
		{
			throw new NotImplementedException();
		}

		public bool IsCompatibleWithParent(object parentObject)
		{
			throw new NotImplementedException();
		}

		public bool CopyFrom(object obj)
		{
			throw new NotImplementedException();
		}

		public object Clone()
		{
			throw new NotImplementedException();
		}

		#endregion IGraphicBase3D
	}
}