#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Altaxo.Main.Properties;
using Altaxo.Serialization.Xml;

namespace Altaxo.Main
{
  /// <summary>
  /// Defines an action that can be executed. The action can report progress to a progress reporter and can accept optional arguments for additional information.
  /// </summary>
  public interface IAction : Main.IImmutable
  {
    /// <summary>
    /// Executes the action. The action can report progress to the <paramref name="reporter" />. The <paramref name="args" /> are optional arguments that can be used to pass additional information to the action.
    /// </summary>
    /// <param name="reporter">The progress reporter.</param>
    /// <param name="args">Optional arguments that can be used to pass additional information to the action.</param>
    public void Execute(IProgressReporter reporter, params object[] args);
  }

  /// <summary>
  /// Represents a document that contains an action to be executed. 
  /// </summary>
  public class ActionDocument
    :
    Main.SuspendableDocumentNodeWithSingleAccumulatedData<EventArgs>,
    IProjectItem,
    Main.INameOwner,
    Main.ICopyFrom,
    IPropertyBagOwner
  {
    private string _name;
    private DateTime _creationTimeUtc;
    private DateTime _lastModifiedTimeUtc;
    private IAction _action;

    /// <summary>
    /// Notes concerning this action.
    /// </summary>
    protected Main.TextBackedConsole _notes;

    /// <summary>
    /// The properties, key is a string, value is a property (arbitrary object) you want to store here.
    /// </summary>
    /// <remarks>The properties are saved on disc (with exception of those that starts with "tmp/".
    /// If the property you want to store is only temporary, the properties name should therefore
    /// start with "tmp/".</remarks>
    protected Main.Properties.PropertyBag? _properties;

    #region Serialization

    /// <summary>
    /// 2026-08-10 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ActionDocument), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ActionDocument)o;

        info.AddValue("Name", s._name);
        info.AddValue("CreationTimeUtc", s._creationTimeUtc);
        info.AddValue("ChangeTimeUtc", s._lastModifiedTimeUtc);
        info.AddValue("Action", s._action);
        info.AddValue("Notes", s._notes.Text);
        info.AddValueOrNull("Properties", s._properties);
      }

      /// <summary>
      /// Deserializes into an existing instance.
      /// </summary>
      public void Deserialize(ActionDocument s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        s._name = info.GetString("Name");
        s._creationTimeUtc = info.GetDateTime("CreationTimeUtc");
        s._lastModifiedTimeUtc = info.GetDateTime("ChangeTimeUtc");
        s._action = info.GetValue<IAction>("Action", s);
        s._notes ??= new Main.TextBackedConsole() { ParentObject = s };
        s._notes.Text = info.GetString("Notes");
        s.ChildSetMember(ref s._properties, info.GetValueOrNull<Main.Properties.PropertyBag>("Properties", s));
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = o is not null ? (ActionDocument)o : new ActionDocument(info);
        Deserialize(s, info, parent);
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Deserialization constructor. This constructor is used by the XML deserialization surrogate to create an instance of the class during deserialization.
    /// </summary>
    /// <param name="info">The XML deserialization information.</param>
    protected ActionDocument(IXmlDeserializationInfo info)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionDocument"/> class.
    /// </summary>
    /// <param name="name">Name of the action document.</param>
    /// <param name="action">The action to be executed.</param>
    public ActionDocument(string name, IAction action)
    {
      _name = string.Empty;
      Name = name;
      _action = action;
      _creationTimeUtc = _lastModifiedTimeUtc = DateTime.UtcNow;
      _action = action ?? throw new ArgumentNullException("Action cannot be null");
      _notes = new TextBackedConsole() { ParentObject = this };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionDocument"/> class.
    /// </summary>
    /// <param name="from">Another instance to copy the name of the bag and the properties from.</param>
    public ActionDocument(ActionDocument from)
    {
      _name = string.Empty;
      _creationTimeUtc = _lastModifiedTimeUtc = DateTime.UtcNow;
      CopyFrom(from);
    }

    /// <summary>
    /// Copies name and properties from another instance.
    /// </summary>
    /// <param name="obj">The object to copy from.</param>
    /// <returns><c>true</c> if anything could be copied.</returns>
    public virtual bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      var from = (ActionDocument)obj;
      if (from is not null)
      {
        _name = from._name;
        _lastModifiedTimeUtc = from._lastModifiedTimeUtc;
        _action = from._action;
        ChildCopyToMember(ref _notes, from._notes);
        PropertyBagNotNull.CopyFrom(from._properties);
        return true;
      }
      return false;
    }

    /// <inheritdoc/>
    object ICloneable.Clone()
    {
      return new ActionDocument(this);
    }

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>Clone of this instance.</returns>
    public ActionDocument Clone()
    {
      return new ActionDocument(this);
    }

    /// <summary>
    /// Tests if this item already has a name.
    /// </summary>
    /// <param name="name">On success, returns the name of the item.</param>
    /// <returns>
    /// True if the item already has a name; otherwise false.
    /// </returns>
    public override bool TryGetName([MaybeNullWhen(false)] out string name)
    {
      name = _name;
      return !(name is null);
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
        return _name ?? throw new InvalidOperationException($"Name is not set yet. Use '{nameof(TryGetName)}' to test for this condition");
      }
      [MemberNotNull(nameof(_name))]
      set
      {
        if (value is null)
          throw new ArgumentNullException("New name is null");
        if (_name == value)
          return; // nothing changed

        var canBeRenamed = true;
        var parentAs = _parent as Main.IParentOfINameOwnerChildNodes;
        if (parentAs is not null)
        {
          canBeRenamed = parentAs.EhChild_CanBeRenamed(this, value);
        }

        if (canBeRenamed)
        {
          var oldName = _name!;
          _name = value;

          if (parentAs is not null)
            parentAs.EhChild_HasBeenRenamed(this, oldName);

          OnNameChanged(oldName);
        }
        else
        {
          throw new ApplicationException($"Renaming of action {_name} into {value} not possible, because name already exists!");
        }
      }
    }

    /// <summary>
    /// Gets the short name (i.e. without the folder name) of this item.
    /// </summary>
    public string ShortName
    {
      get
      {
        return Main.ProjectFolder.GetNamePart(Name);
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
    public DateTime LastChangeTimeUtc
    {
      get
      {
        return _lastModifiedTimeUtc;
      }
    }

    /// <summary>
    /// Notes concerning this action.
    /// </summary>
    public Main.ITextBackedConsole Notes
    {
      get
      {
        return _notes;
      }
    }

    #region IPropertyBagOwner Members

    /// <summary>
    /// Gets or sets the item's property bag.
    /// </summary>
    public Main.Properties.PropertyBag? PropertyBag
    {
      get { return _properties; }
      protected set
      {
        _properties = value;
        if (_properties is not null)
          _properties.ParentObject = this;
      }
    }

    /// <summary>
    /// Gets the îtem's property bag, creating it if necessary.
    /// </summary>
    public Main.Properties.PropertyBag PropertyBagNotNull
    {
      get
      {
        if (_properties is null)
        {
          _properties = new Main.Properties.PropertyBag() { ParentObject = this };
        }
        return _properties;
      }
    }


    /// <summary>
    /// Gets an arbitrary object that was stored as item property by <see cref="SetItemProperty" />.
    /// </summary>
    /// <param name="key">Name of the property.</param>
    /// <returns>The object, or null if no object under the provided name was stored here.</returns>
    public object? GetItemProperty(string key)
    {
      object? result = null;
      if (_properties is not null)
        _properties.TryGetValue(key, out result);
      return result;
    }

    /// <summary>
    /// Gets or sets the property value identified by the given key.
    /// </summary>
    /// <typeparam name="T">The property value type.</typeparam>
    /// <param name="key">The property key.</param>
    /// <param name="resultCreationIfNotFound">A factory used when the value is not found.</param>
    /// <returns>The property value.</returns>
    [return: NotNullIfNotNull("resultCreationIfNotFound")]
    [return: MaybeNull]
    public T GetPropertyValue<T>(Altaxo.Main.Properties.PropertyKey<T> key, Func<T>? resultCreationIfNotFound) where T : notnull
    {
      return PropertyExtensions.GetPropertyValue(this, key, resultCreationIfNotFound);
    }

    /// <summary>
    /// Sets a graph property identified by a string key.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="val">The property value to store.</param>
    /// <remarks>The properties are saved on disk, with the exception of those whose name starts with <c>"tmp/"</c>. If the property you want to store is only temporary, the property name should therefore start with <c>"tmp/"</c>.</remarks>
    public void SetItemProperty(string key, object val)
    {
      PropertyBagNotNull.SetValue(key, val);
    }

    #endregion

    /// <summary>
    /// Gets the property bag. If the property bag is empty or not created, it is allowed to return null.
    /// </summary>
    /// <value>
    /// The property bag, or <c>null</c> if there is no property bag.
    /// </value>
    public IAction Action
    {
      get
      {
        return _action;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException("Action cannot be null");

        if (!object.ReferenceEquals(value, this))
        {
          _action = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }



    /// <inheritdoc/>
    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      yield break;
    }

    /// <summary>
    /// Has to enumerate all references to other items in the project (<see cref="DocNodeProxy" />) which are used in this project item and in all childs of this project item. The references
    /// has to be reported to the <paramref name="ReportProxies" /> function. This function is responsible for processing of the proxies, for instance to relocated the path.
    /// </summary>
    /// <param name="ReportProxies">Function that processes  the found <see cref="DocNodeProxy" /> instances.</param>
    public void VisitDocumentReferences(DocNodeProxyReporter ReportProxies)
    {
      // currently there is nothing to do here
    }

    #region Suspend

    /// <inheritdoc/>
    protected override void AccumulateChangeData(object? sender, EventArgs e)
    {
      _accumulatedEventData = EventArgs.Empty;
    }

    #endregion Suspend
  }
}

