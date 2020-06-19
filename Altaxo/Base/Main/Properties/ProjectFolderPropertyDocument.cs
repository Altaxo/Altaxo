#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Altaxo.Main.Properties
{
  /// <summary>
  /// A property bag that holds the properties of a project folder.
  /// </summary>
  public class ProjectFolderPropertyDocument
    :
    Main.SuspendableDocumentNodeWithSingleAccumulatedData<EventArgs>,
    IProjectItem,
    IPropertyBagOwner,
    Main.INameOwner,
    Main.ICopyFrom
  {
    private string _name;
    private PropertyBag? _propertyBag;
    private DateTime _creationTimeUtc;
    private DateTime _changeTimeUtc;

    #region Serialization

    /// <summary>
    /// 2014-01-22 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ProjectFolderPropertyDocument), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ProjectFolderPropertyDocument)obj;

        info.AddValue("Name", s._name);
        info.AddValue("CreationTimeUtc", s._creationTimeUtc);
        info.AddValue("ChangeTimeUtc", s._changeTimeUtc);
        info.AddValueOrNull("Properties", s._propertyBag);
      }

      public void Deserialize(ProjectFolderPropertyDocument s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        s._name = info.GetString("Name");
        s._creationTimeUtc = info.GetDateTime("CreationTimeUtc");
        s._changeTimeUtc = info.GetDateTime("ChangeTimeUtc");
        s.PropertyBag = info.GetValueOrNull<Main.Properties.PropertyBag>("Properties", s);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = null != o ? (ProjectFolderPropertyDocument)o : new ProjectFolderPropertyDocument(string.Empty);
        Deserialize(s, info, parent);
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectFolderPropertyDocument"/> class.
    /// </summary>
    /// <param name="folderName">Name of the folder.</param>
    public ProjectFolderPropertyDocument(string folderName)
    {
      _name = string.Empty;
      Name = folderName;
      _creationTimeUtc = _changeTimeUtc = DateTime.UtcNow;
      PropertyBag = new PropertyBag();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectFolderPropertyDocument"/> class.
    /// </summary>
    /// <param name="from">Another instance to copy the name of the bag and the properties from.</param>
    public ProjectFolderPropertyDocument(ProjectFolderPropertyDocument from)
    {
      _name = string.Empty;
      _creationTimeUtc = _changeTimeUtc = DateTime.UtcNow;
      CopyFrom(from);
    }

    /// <summary>
    /// Copies name and properties from another instance.
    /// </summary>
    /// <param name="obj">The object to copy from.</param>
    /// <returns><c>True</c> if anything could be copyied.</returns>
    public virtual bool CopyFrom(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;

      var from = (ProjectFolderPropertyDocument)obj;
      if (null != from)
      {
        _name = from._name;
        _changeTimeUtc = from._changeTimeUtc;
        _propertyBag = null;
        if (null != from._propertyBag)
        {
          _propertyBag = PropertyBagNotNull;
          _propertyBag.Clear();
          _propertyBag.CopyFrom(from._propertyBag);
        }
        return true;
      }
      return false;
    }

    object ICloneable.Clone()
    {
      return new ProjectFolderPropertyDocument(this);
    }

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>Clone of this instance.</returns>
    public ProjectFolderPropertyDocument Clone()
    {
      return new ProjectFolderPropertyDocument(this);
    }

    /// <summary>
    /// Gets or sets the name of the property bag. This has to be a valid project folder name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public override string Name
    {
      get
      {
        return _name;
      }
      [MemberNotNull(nameof(_name))]
      set
      {
        if (value is null)
          throw new ArgumentNullException("New name is null");
        if (_name == value)
          return; // nothing changed

        Main.ProjectFolder.ThrowExceptionOnInvalidFullFolderPath(value);

        var canBeRenamed = true;
        var parentAs = _parent as Main.IParentOfINameOwnerChildNodes;
        if (null != parentAs)
        {
          canBeRenamed = parentAs.EhChild_CanBeRenamed(this, value);
        }

        if (canBeRenamed)
        {
          var oldName = _name;
          _name = value;

          if (null != parentAs)
            parentAs.EhChild_HasBeenRenamed(this, oldName);

          OnNameChanged(oldName);
        }
        else
        {
          throw new ApplicationException(string.Format("Renaming of table {0} into {1} not possible, because name exists already", _name, value));
        }
      }
    }

    /// <summary>
    /// Fires both a Changed and a TunnelingEvent when the name has changed.
    /// The event arg of the Changed event is an instance of <see cref="T:Altaxo.Main.NamedObjectCollectionChangedEventArgs"/>.
    /// The event arg of the Tunneling event is an instance of <see cref="T:Altaxo.Main.DocumentPathChangedEventArgs"/>.
    /// </summary>
    /// <param name="oldName">The name of the table before it has changed the name.</param>
    protected virtual void OnNameChanged(string oldName)
    {
      EhSelfTunnelingEventHappened(Main.DocumentPathChangedEventArgs.Empty);
      EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemRenamed(this, oldName));
    }

    /// <summary>
    /// Gets the creation time in UTC.
    /// </summary>
    /// <value>
    /// The creation time in UTC.
    /// </value>
    public DateTime CreationTimeUtc
    {
      get
      {
        return _creationTimeUtc;
      }
    }

    /// <summary>
    /// Gets the change time in UTC.
    /// </summary>
    /// <value>
    /// The change time in UTC.
    /// </value>
    public DateTime ChangeTimeUtc
    {
      get
      {
        return _creationTimeUtc;
      }
    }

    /// <summary>
    /// Gets the property bag. If the property bag is empty or not created, it is allowed to return null.
    /// </summary>
    /// <value>
    /// The property bag, or <c>null</c> if there is no property bag.
    /// </value>
    public PropertyBag? PropertyBag
    {
      get
      {
        return _propertyBag;
      }
      protected set
      {
        ChildSetMember(ref _propertyBag, value);
      }
    }

    /// <summary>
    /// Gets the property bag. If there is no property bag, a new bag is created and then returned.
    /// </summary>
    /// <value>
    /// The property bag.
    /// </value>
    public PropertyBag PropertyBagNotNull
    {
      get
      {
        if (_propertyBag is null)
          PropertyBag = new PropertyBag();
        return _propertyBag!;
      }
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (null != _propertyBag)
        yield return new Main.DocumentNodeAndName(_propertyBag, () => _propertyBag = null, "PropertyBag");
    }

    /// <summary>
    /// Has to enumerate all references to other items in the project (<see cref="DocNodeProxy" />) which are used in this project item and in all childs of this project item. The references
    /// has to be reported to the <paramref name="ProxyProcessing" /> function. This function is responsible for processing of the proxies, for instance to relocated the path.
    /// </summary>
    /// <param name="ProxyProcessing">Function that processes  the found <see cref="DocNodeProxy" /> instances.</param>
    public void VisitDocumentReferences(DocNodeProxyReporter ProxyProcessing)
    {
      // currently there is nothing to do here
    }

    #region Suspend

    protected override void AccumulateChangeData(object sender, EventArgs e)
    {
      _accumulatedEventData = EventArgs.Empty;
    }

    #endregion Suspend
  }
}
