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

using Altaxo.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Main
{
	/// <summary>
	/// DocumentPath holds a path to a document. This path reflects the internal organization of the class instances in Altaxo. Do not mix this
	/// concept with the concept of the folder in which a project item virtually exists  (see <see cref="ProjectFolder"/>).
	/// </summary>
	[Serializable]
	public sealed class RelativeDocumentPath : System.ICloneable, Main.IImmutable
	{
		private int _numberOfLevelsDown;
		private string[] _pathParts;

		/// <summary>
		/// The path that designates identity, i.e. the start node is identical to the destination node.
		/// </summary>
		public static readonly RelativeDocumentPath IdentityPath = new RelativeDocumentPath(0, new string[0]);

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RelativeDocumentPath), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (RelativeDocumentPath)obj;

				info.AddValue("LevelsDown", s._numberOfLevelsDown);

				info.CreateArray("Path", s._pathParts.Length);
				for (int i = 0; i < s._pathParts.Length; i++)
					info.AddValue("e", s._pathParts[i]);
				info.CommitArray();
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				int numberOfLevelsDown = info.GetInt32("LevelsDown");

				int count = info.OpenArray("Path");
				var arr = new string[count];
				for (int i = 0; i < count; i++)
					arr[i] = info.GetString();
				info.CloseArray(count);

				return new RelativeDocumentPath(numberOfLevelsDown, arr);
			}
		}

		#endregion Serialization

		public RelativeDocumentPath(RelativeDocumentPath from)
			: this(from._numberOfLevelsDown, from._pathParts)
		{
		}

		public RelativeDocumentPath(int numberOfLevelsDown, IEnumerable<string> path)
		{
			if (numberOfLevelsDown < 0)
				throw new ArgumentOutOfRangeException("numberOfLevelsDown shall be >=0");

			this._numberOfLevelsDown = numberOfLevelsDown;
			_pathParts = path.ToArray();
		}

		public bool IsIdentity
		{
			get
			{
				return 0 == _numberOfLevelsDown && 0 == _pathParts.Length;
			}
		}

		public int NumberOfStepsDown
		{
			get
			{
				return _numberOfLevelsDown;
			}
		}

		public int Count
		{
			get
			{
				return _pathParts.Length;
			}
		}

		public string this[int idx]
		{
			get
			{
				return _pathParts[idx];
			}
		}

		public override string ToString()
		{
			var stringBuilder = new System.Text.StringBuilder(128);

			if (this._numberOfLevelsDown > 0)
				stringBuilder.Append("../");

			for (int i = 1; i < _numberOfLevelsDown; ++i)
			{
				stringBuilder.Append("../");
			}

			if (_pathParts.Length > 0)
				stringBuilder.Append(_pathParts[0]);

			for (int i = 1; i < _pathParts.Length; i++)
			{
				stringBuilder.Append("/");
				stringBuilder.Append(_pathParts[i]);
			}
			return stringBuilder.ToString();
		}

		public override bool Equals(object obj)
		{
			var o = obj as RelativeDocumentPath;
			if (null == o)
				return false;
			if (this._numberOfLevelsDown != o._numberOfLevelsDown)
				return false;

			if (_pathParts.Length != o._pathParts.Length)
				return false;

			for (int i = _pathParts.Length - 1; i >= 0; --i)
			{
				if (!(_pathParts[i] == o._pathParts[i]))
					return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		#region static navigation methods

		/// <summary>
		/// Retrieves the relative path from the node <code>startnode</code> to the node <code>endnode</code>.
		/// </summary>
		/// <param name="startnode">The node where the path begins.</param>
		/// <param name="endnode">The node where the path ends.</param>
		/// <returns>If the two nodes share a common root, the function returns the relative path from <code>startnode</code> to <code>endnode</code>.
		/// If the nodes have no common root, then the function returns the absolute path of the endnode.</returns>
		public static RelativeDocumentPath GetRelativePathFromTo(IDocumentLeafNode startnode, IDocumentLeafNode endnode)
		{
			return GetRelativePathFromTo(startnode, endnode, null);
		}

		/// <summary>
		/// Retrieves the relative path from the node <code>startnode</code> to the node <code>endnode</code>.
		/// </summary>
		/// <param name="startnode">The node where the path begins.</param>
		/// <param name="endnode">The node where the path ends.</param>
		/// <param name="stoppernode">A object which is used as stopper. If the relative path would step down below this node in the hierarchie,
		/// not the relative path, but the absolute path of the endnode is returned. This is usefull for instance for serialization purposes.You can set the stopper node
		/// to the root object of serialization, so that path in the inner of the serialization tree are relative paths, whereas paths to objects not includes in the
		/// serialization tree are returned as absolute paths. The stoppernode can be null.</param>
		/// <returns>If the two nodes share a common root, the function returns the relative path from <code>startnode</code> to <code>endnode</code>.
		/// If the nodes have no common root, then the function returns the absolute path of the endnode.
		/// <para>If either startnode or endnode is null, then null is returned.</para></returns>
		public static RelativeDocumentPath GetRelativePathFromTo(IDocumentLeafNode startnode, IDocumentLeafNode endnode, IDocumentNode stoppernode)
		{
			if (startnode == null)
				throw new ArgumentNullException("startnode");
			if (endnode == null)
				throw new ArgumentNullException("endnode");

			if (object.ReferenceEquals(startnode, endnode))
				return IdentityPath; // Start node and end node are identical

			var currStart = startnode;
			var currEnd = endnode;

			var startNodesList = new List<IDocumentLeafNode>(Altaxo.Collections.TreeNodeExtensions.TakeFromHereToRoot(startnode));
			var endNodesList = new List<IDocumentLeafNode>(Altaxo.Collections.TreeNodeExtensions.TakeFromHereToRoot(endnode));

			var commonNodes = new HashSet<IDocumentLeafNode>(startNodesList);
			commonNodes.IntersectWith(new HashSet<IDocumentLeafNode>(endNodesList));

			if (commonNodes.Count == 0)
				return null; // happens if either startnode or endnode are not rooted (this can happen temporarily during instance creation)

			int numberOfNodesDown = 0;
			IDocumentLeafNode commonNode = null;
			for (int i = 0; i < startNodesList.Count; ++i)
			{
				if (commonNodes.Contains(startNodesList[i]))
				{
					commonNode = startNodesList[i];
					break;
				}
				else
				{
					numberOfNodesDown++;
				}
			}

			if (null == commonNodes)
				throw new InvalidOperationException("commonNode should always be != null");

			var idx = endNodesList.IndexOf(commonNode);

			if (idx < 0)
				throw new InvalidOperationException("idx of commonNode in endNodesList should always be >=0");
			else if (idx == 0)
				throw new InvalidOperationException("idx=0 should not happen because this means identical startnode and endnode");

			return new RelativeDocumentPath(numberOfNodesDown, endNodesList.TakeFromUpperIndexDownToLowerIndex(idx - 1, 0).Select(x => x.ParentObject.GetNameOfChildObject(x)));
		}

		public static IDocumentLeafNode GetObject(RelativeDocumentPath path, IDocumentLeafNode startnode)
		{
			if (null == path)
				throw new ArgumentNullException("path");
			if (null == startnode)
				throw new ArgumentNullException("startnode");

			var node = startnode;

			for (int i = 0; i < path._numberOfLevelsDown && null != node; ++i)
			{
				node = node.ParentNode;
			}

			if (null == node)
				return null; // Path not resolveable

			for (int i = 0; i < path._pathParts.Length; i++)
			{
				if (node is Main.IDocumentNode)
					node = ((Main.IDocumentNode)node).GetChildObjectNamed(path._pathParts[i]);
				else
					return null;
			} // end for

			return node;
		}

		/// <summary>
		/// Gets the node that is designated by the provided <paramref name="path"/>  or the least resolveable node.
		/// </summary>
		/// <param name="path">The document path to resolve.</param>
		/// <param name="startnode">The startnode.</param>
		/// <param name="pathWasCompletelyResolved">If set to <c>true</c> on return, the path was completely resolved. Otherwise, <c>false</c>.</param>
		/// <returns>The resolved node, or the least node on the path that could be resolved.</returns>
		/// <exception cref="System.ArgumentNullException">
		/// path
		/// or
		/// startnode
		/// </exception>
		public static IDocumentLeafNode GetNodeOrLeastResolveableNode(RelativeDocumentPath path, IDocumentLeafNode startnode, out bool pathWasCompletelyResolved)
		{
			if (null == path)
				throw new ArgumentNullException("path");
			if (null == startnode)
				throw new ArgumentNullException("startnode");

			var node = startnode;
			var prevNode = startnode;

			for (int i = 0; i < path._numberOfLevelsDown && null != node; ++i)
			{
				prevNode = node;
				node = node.ParentNode;
			}

			if (null == node)
			{
				pathWasCompletelyResolved = false;
				return prevNode;
			}

			pathWasCompletelyResolved = true;
			for (int i = 0; i < path._pathParts.Length && null != node; i++)
			{
				prevNode = node;

				if (node is Main.IDocumentNode)
					node = ((Main.IDocumentNode)node).GetChildObjectNamed(path._pathParts[i]);
				else
					node = null;
			} // end for

			pathWasCompletelyResolved = null != node;
			return node ?? prevNode;
		}

		#endregion static navigation methods

		#region ICloneable Members

		object System.ICloneable.Clone()
		{
			return new RelativeDocumentPath(this);
		}

		public RelativeDocumentPath Clone()
		{
			return new RelativeDocumentPath(this);
		}

		#endregion ICloneable Members

		public static RelativeDocumentPath FromOldDeprecated(AbsoluteDocumentPath absPath)
		{
			if (null == absPath)
				throw new ArgumentNullException("absPath");

			int numberOfLevelsDown = 0;
			var pathList = new List<string>();

			for (int i = 0; i < absPath.Count; ++i)
			{
				if (absPath[i] == "..")
				{
					numberOfLevelsDown++;
				}
				else
				{
					pathList.Add(absPath[i]);
				}
			}
			return new RelativeDocumentPath(numberOfLevelsDown, pathList);
		}
	}
}