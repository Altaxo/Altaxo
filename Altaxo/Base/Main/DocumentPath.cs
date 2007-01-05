#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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

namespace Altaxo.Main
{
  /// <summary>
  /// DocumentPath holds a path to a document
  /// </summary>
  public class DocumentPath : System.Collections.Specialized.StringCollection, System.ICloneable
  {
    protected bool _IsAbsolutePath;
    
    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DocumentPath),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        DocumentPath s = (DocumentPath)obj;

        info.AddValue("IsAbsolute",s._IsAbsolutePath);

        info.CreateArray("Path",s.Count);
        for(int i=0;i<s.Count;i++)
          info.AddValue("e",s[i]);
        info.CommitArray();
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        DocumentPath s = null!=o ? (DocumentPath)o : new DocumentPath();
        
        
        s._IsAbsolutePath = info.GetBoolean("IsAbsolute");

        int count = info.OpenArray();
        for(int i=0;i<count;i++)
          s.Add(info.GetString());
        info.CloseArray(count);

        return s;
      }
    }

    #endregion
    
    public DocumentPath(bool isAbsolutePath)
    {
      _IsAbsolutePath = isAbsolutePath;
    }

    public DocumentPath()
      : this(false)
    {
    }

    public DocumentPath(DocumentPath from)
    {
      this._IsAbsolutePath = from._IsAbsolutePath;
      foreach(string s in from)
        Add(s);
    }
    public bool IsAbsolutePath
    {
      get { return _IsAbsolutePath; }
      set { _IsAbsolutePath = value; }
    }

    public override string ToString()
    {
      System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(128);
      if(this.IsAbsolutePath)
        stringBuilder.Append("/");

      if(this.Count>0)
        stringBuilder.Append(this[0]);

      for(int i=1;i<this.Count;i++)
      {
        stringBuilder.Append("/");
        stringBuilder.Append(this[i]);
      }
      return stringBuilder.ToString();
    }

    #region static navigation methods

    /// <summary>
    /// Get the root node of the node <code>node</code>. The root node is defined as last node in the hierarchie that
    /// either has not implemented the <see cref="IDocumentNode"/> interface or has no parent.
    /// </summary>
    /// <param name="node">The node from where the search begins.</param>
    /// <returns>The root node, i.e. the last node in the hierarchie that
    /// either has not implemented the <see cref="IDocumentNode"/> interface or has no parent</returns>
    public static object GetRootNode(IDocumentNode node)
    {
      object root = node.ParentObject;
      while(root!= null && root is IDocumentNode)
        root = ((IDocumentNode)root).ParentObject;
      
      return root;
    }

    /// <summary>
    /// Get the first parent node of the node <code>node</code> that implements the given type <code>type.</code>.
    /// </summary>
    /// <param name="node">The node from where the search begins.</param>
    /// <param name="type">The type to search for.</param>
    /// <returns>The first parental node that implements the type <code>type.</code>
    /// </returns>
    public static object GetRootNodeImplementing(IDocumentNode node, System.Type type)
    {
      object root = node.ParentObject;
      while(root!= null && root is IDocumentNode && !type.IsInstanceOfType(root))
        root = ((IDocumentNode)root).ParentObject;
      
      return type.IsInstanceOfType(root) ? root : null;
    }

    public static T GetRootNodeImplementing<T>(IDocumentNode node)
    {
      object root = node.ParentObject;
      while (root != null && root is IDocumentNode && !(root is T))
        root = ((IDocumentNode)root).ParentObject;

      return (root is T) ?  (T)root : default(T);
    }
   

    /// <summary>
    /// Get the absolute path of the node <code>node</code> starting from the root.
    /// </summary>
    /// <param name="node">The node for which the path is retrieved.</param>
    /// <returns>The absolute path of the node. The first element is a "/" to mark this as absolute path.</returns>
    public static DocumentPath GetAbsolutePath(IDocumentNode node)
    {
      DocumentPath path = GetPath(node,int.MaxValue);
      path.IsAbsolutePath = true;
      return path;
    }

    /// <summary>
    /// Get the absolute path of the node <code>node</code> starting either from the root, or from the object in the depth
    /// <code>maxDepth</code>, whatever is reached first.
    /// </summary>
    /// <param name="node">The node for which the path is to be retrieved.</param>
    /// <param name="maxDepth">The maximal hierarchie depth (the maximal number of path elements returned).</param>
    /// <returns>The path from the root or from the node in the depth <code>maxDepth</code>, whatever is reached first. The path is <b>not</b> prepended
    /// by a "/".
    /// </returns>
    public static DocumentPath GetPath(IDocumentNode node, int maxDepth)
    {
      DocumentPath path = new DocumentPath();
      
      int depth=0;
      object root = node;
      while(root!= null && root is IDocumentNode)
      {
        if(depth>=maxDepth)
          break;

        string name = ((IDocumentNode)root).Name;
        path.Insert(0,name);
        root = ((IDocumentNode)root).ParentObject;
        ++depth;
      }

      return path;
    }

    public static string GetPathString(IDocumentNode node, int maxDepth)
    {
      return GetPath(node,maxDepth).ToString();
    }

    /// <summary>
    /// Retrieves the relative path from the node <code>startnode</code> to the node <code>endnode</code>.
    /// </summary>
    /// <param name="startnode">The node where the path begins.</param>
    /// <param name="endnode">The node where the path ends.</param>
    /// <returns>If the two nodes share a common root, the function returns the relative path from <code>startnode</code> to <code>endnode</code>.
    /// If the nodes have no common root, then the function returns the absolute path of the endnode.</returns>
    public static DocumentPath GetRelativePathFromTo(IDocumentNode startnode, IDocumentNode endnode)
    {
      return GetRelativePathFromTo(startnode,endnode,null);
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
    public static DocumentPath GetRelativePathFromTo(IDocumentNode startnode, IDocumentNode endnode, object stoppernode)
    {
      System.Collections.Hashtable hash = new System.Collections.Hashtable();

      if (startnode == null || endnode == null)
        return null;

      // store the complete hierarchie of objects as keys in the hash, (values are the hierarchie depth)
      int endnodedepth=0;
      object root = endnode;
      while(root!= null && root is IDocumentNode)
      {
        hash.Add(root,endnodedepth);
        root = ((IDocumentNode)root).ParentObject;
        ++endnodedepth;
      }

      // the whole endnode hierarchie is now in the hash, now look to find the first node starting from startnode, which has the same root
      // i.e. which is contained in the hash table

      int startnodedepth=0;
      root = startnode;
      while(root!=null && root is IDocumentNode)
      {
        if(hash.ContainsKey(root))
        {
          endnodedepth = (int)hash[root]; // the depth of the endnode to this root
          break;
        }

        if(root.Equals(stoppernode)) // stop also if the stopper node is reached
        {
          break;
        }

        root = ((IDocumentNode)root).ParentObject;
        ++startnodedepth;
      }


      if(!hash.ContainsKey(root))
      {
        return GetAbsolutePath(endnode); // no common root -> return AbsolutePath
      }
      else
      {
        DocumentPath path;
        path = GetPath(endnode,endnodedepth); // path of endnode depth
        for(int i=0;i<startnodedepth;i++)
          path.Insert(0,".."); // insert "root dir entries" 

        return path;
      }

    }


    /// <summary>
    /// Resolves the path by first using startnode as starting point. If that failed, try using documentRoot as starting point.
    /// </summary>
    /// <param name="path">The path to resolve.</param>
    /// <param name="startnode">The node object which is considered as the starting point of the path.</param>
    /// <param name="documentRoot">An alternative node which is used as starting point of the path if the first try failed.</param>
    /// <returns>The resolved object. If the resolving process failed, the return value is null.</returns>
    public static object GetObject(DocumentPath path, object startnode, object documentRoot)
    {
      object retval = GetObject(path,startnode);
      
      if(null==retval && null!=documentRoot)
        retval = GetObject(path,documentRoot);  

      return retval;
    }
    

    public static object GetObject(DocumentPath path, object startnode)
    {
      object node = startnode;

      if(path.IsAbsolutePath && node is IDocumentNode)
        node = GetRootNode((IDocumentNode)node);

      for(int i=0;i<path.Count;i++)
      {
        if(path[i]=="..")
        {
          if(node is Main.IDocumentNode)
            node = ((Main.IDocumentNode)node).ParentObject;
          else
            return null;
        }
        else
        {
          if(node is Main.INamedObjectCollection)
            node = ((Main.INamedObjectCollection)node).GetChildObjectNamed(path[i]);
          else
            return null;
        }
      } // end for
      return node;
    }
    #endregion

    #region ICloneable Members

    object System.ICloneable.Clone()
    {
      return new DocumentPath(this);
    }
    public DocumentPath Clone()
    {
      return new DocumentPath(this);
    }

    #endregion
  }
}
