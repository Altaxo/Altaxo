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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Drawing;
using Altaxo.Main;
using Altaxo.Main.Properties;

namespace Altaxo.Text
{
  /// <summary>
  /// Stores notes as markdown annotated text.
  /// </summary>
  /// <seealso cref="Altaxo.Main.SuspendableDocumentLeafNodeWithSingleAccumulatedData{T}" />
  /// <seealso cref="Altaxo.Main.IProjectItem" />
  /// <seealso cref="System.ICloneable" />
  /// <seealso cref="Altaxo.Main.IChangedEventSource" />
  /// <seealso cref="Altaxo.Main.INameOwner" />
  /// <seealso cref="Altaxo.Main.Properties.IPropertyBagOwner" />
  public class TextDocument :
    Main.SuspendableDocumentNodeWithSingleAccumulatedData<EventArgs>,
    IProjectItem,
    Main.ICopyFrom,
    IChangedEventSource,
    Main.INameOwner,
    Main.Properties.IPropertyBagOwner
  {
    /// <summary>
    /// The markdown source text.
    /// </summary>
    private string _sourceText;

    private bool? _isHyphenationEnabled;

    /// <summary>
    /// Local images for this markdown, stored in a dictionary. The key is a Guid which is created when the image is pasted into the markdown document.
    /// The value is a memory stream image proxy.
    /// </summary>
    private Dictionary<string, MemoryStreamImageProxy> _images;

    /// <summary>
    /// Gets or sets the collection of all referenced image Urls.
    /// We use this only in the serialization code to serialize only those local images which are referenced in the markdown.
    /// </summary>
    public IEnumerable<(string Url, int urlSpanStart, int urlSpanEnd)> ReferencedImageUrls { get; set; }

    /// <summary>
    /// The name of the style used to visualize the markdown. If this string is null or empty, the current global
    /// defined style of the current Altaxo instance will be used.
    /// </summary>
    private string _styleName;

    /// <summary>
    /// The name of the document.
    /// </summary>
    protected string _name;

    /// <summary>
    /// The date/time of creation of this document.
    /// </summary>
    protected DateTime _creationTime;

    /// <summary>
    /// The date/time when this document was changed.
    /// </summary>
    protected DateTime _lastChangeTime;

    /// <summary>
    /// Notes concerning this document.
    /// </summary>
    protected Main.TextBackedConsole _notes;

    /// <summary>
    /// The graph properties, key is a string, value is a property (arbitrary object) you want to store here.
    /// </summary>
    /// <remarks>The properties are saved on disc (with exception of those that starts with "tmp/".
    /// If the property you want to store is only temporary, the properties name should therefore
    /// start with "tmp/".</remarks>
    protected Main.Properties.PropertyBag _documentProperties;

    #region "Serialization"

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(TextDocument), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (TextDocument)obj;

        info.AddValue("Name", s._name);
        info.AddValue("CreationTime", s._creationTime.ToLocalTime());
        info.AddValue("LastChangeTime", s._lastChangeTime.ToLocalTime());
        info.AddValue("Notes", s._notes.Text);
        info.AddValue("Properties", s._documentProperties);
        info.AddValue("StyleName", s._styleName);
        info.AddValue("SourceText", s._sourceText);
        info.AddValue("IsHyphenationEnabled", s._isHyphenationEnabled);

        // we need to calculate in advance the number of referenced local images

        HashSet<string> allNames;
        if (null != s.ReferencedImageUrls)
        {
          allNames = new HashSet<string>();
          foreach (var entry in s.ReferencedImageUrls)
          {
            if (entry.Url.StartsWith(ImagePretext.LocalImagePretext))
            {
              string localImageName = entry.Url.Substring(ImagePretext.LocalImagePretext.Length);
              if (s._images.ContainsKey(localImageName))
                allNames.Add(localImageName);
            }
          }
        }
        else
        {
          allNames = new HashSet<string>(s._images.Keys);
        }

        info.CreateArray("Images", allNames.Count);
        {
          foreach (var name in allNames)
          {
            info.CreateElement("e");
            {
              info.AddValue("Name", name);
              info.AddValue("Image", s._images[name]);
            }
            info.CommitElement();
          }
        }
        info.CommitArray();
      }

      public void Deserialize(TextDocument s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        s._name = info.GetString("Name");
        s._creationTime = info.GetDateTime("CreationTime").ToUniversalTime();
        s._lastChangeTime = info.GetDateTime("LastChangeTime").ToUniversalTime();
        s._notes.Text = info.GetString("Notes");
        s.PropertyBag = (Main.Properties.PropertyBag)info.GetValue("Properties", s);
        s._styleName = info.GetString("StyleName");
        s._sourceText = info.GetString("SourceText");
        s._isHyphenationEnabled = info.GetNullableBoolean("IsHyphenationEnabled");

        int count = info.OpenArray("Images");
        {
          for (int i = 0; i < count; ++i)
          {
            info.OpenElement();
            {
              string key = info.GetString("Name");
              var value = (MemoryStreamImageProxy)info.GetValue("Image", s);
              s._images.Add(key, value);
            }
            info.CloseElement();
          }
        }
        info.CloseArray(count);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (TextDocument)o ?? new TextDocument();
        Deserialize(s, info, parent);
        return s;
      }
    }

    #endregion "Serialization"

    /// <summary>
    /// Initializes a new instance of the <see cref="TextDocument"/> class.
    /// </summary>
    public TextDocument()
    {
      _creationTime = _lastChangeTime = DateTime.UtcNow;
      _notes = new TextBackedConsole() { ParentObject = this };
      _images = new Dictionary<string, MemoryStreamImageProxy>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextDocument"/> class by copying the content from another instance.
    /// </summary>
    /// <param name="from">Notes to copy from</param>
    public TextDocument(TextDocument from)
    {
      using (var suppressToken = SuspendGetToken())
      {
        _creationTime = _lastChangeTime = DateTime.UtcNow;
        _images = new Dictionary<string, MemoryStreamImageProxy>();
        CopyFrom(from);

        suppressToken.ResumeSilently();
      }
    }

    /// <inheritdoc/>
    public virtual bool CopyFrom(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;

      if (obj is TextDocument from)
      {
        using (var suspendToken = SuspendGetToken())
        {
          ChildCopyToMember(ref _notes, from._notes);

          // Clone also the properties
          if (from._documentProperties != null && from._documentProperties.Count > 0)
          {
            PropertyBagNotNull.CopyFrom(from._documentProperties);
          }
          else
          {
            _documentProperties = null;
          }

          _styleName = from._styleName;
          _sourceText = from._sourceText;
          _images.Clear();
          foreach (var entry in from._images)
          {
            _images.Add(entry.Key, entry.Value);
          }

          EhSelfChanged(EventArgs.Empty);
        }

        return true;
      }
      return false;
    }

    /// <inheritdoc/>
    public object Clone()
    {
      return new TextDocument(this);
    }

    /// <inheritdoc/>
    public void VisitDocumentReferences(DocNodeProxyReporter ProxyProcessing)
    {
    }

    /// <inheritdoc/>
    protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      yield break;
    }

    /// <summary>
    /// The date/time of creation of this graph.
    /// </summary>
    public DateTime CreationTimeUtc
    {
      get
      {
        return _creationTime;
      }
    }

    /// <summary>
    /// The date/time when this graph was changed.
    /// </summary>
    public DateTime LastChangeTimeUtc
    {
      get
      {
        return _lastChangeTime;
      }
    }

    /// <summary>
    /// Notes concerning this graph.
    /// </summary>
    public Main.ITextBackedConsole Notes
    {
      get
      {
        return _notes;
      }
    }

    /// <summary>
    /// Gets the directory part of the document name with trailing <see cref="Main.ProjectFolder.DirectorySeparatorChar"/>.
    /// If the document is located in the root folder, the <see cref="Main.ProjectFolder.RootFolderName"/>  (an empty string) is returned.
    /// </summary>
    public string Folder
    {
      get
      {
        return Main.ProjectFolder.GetFolderPart(Name);
      }
    }

    /// <summary>
    /// Gets the short name (i.e. without the folder name) of this document.
    /// </summary>
    public string ShortName
    {
      get
      {
        return Main.ProjectFolder.GetNamePart(Name);
      }
    }


    #region IPropertyBagOwner

    /// <inheritdoc/>
    public Main.Properties.PropertyBag PropertyBag
    {
      get { return _documentProperties; }
      protected set
      {
        _documentProperties = value;
        if (null != _documentProperties)
          _documentProperties.ParentObject = this;
      }
    }

    /// <inheritdoc/>
    public Main.Properties.PropertyBag PropertyBagNotNull
    {
      get
      {
        if (null == _documentProperties)
          PropertyBag = new Main.Properties.PropertyBag();
        return _documentProperties;
      }
    }

    #endregion IPropertyBagOwner

    #region Parent and Name

    /// <summary>
    /// Get / sets the parent object of this table.
    /// </summary>
    public override Main.IDocumentNode ParentObject
    {
      get
      {
        return _parent;
      }
      set
      {
        if (object.ReferenceEquals(_parent, value))
          return;

        var oldParent = _parent;
        base.ParentObject = value;

        (_parent as Main.IParentOfINameOwnerChildNodes)?.EhChild_ParentChanged(this, oldParent);
      }
    }

    /// <inheritdoc/>
    public override string Name
    {
      get { return _name; }
      set
      {
        if (null == value)
          throw new ArgumentNullException("New name is null");
        if (_name == value)
          return; // nothing changed

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
          throw new ApplicationException(string.Format("Renaming of graph {0} into {1} not possible, because name exists already", _name, value));
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

    #endregion Parent and Name

    #region SourceText and Style

    /// <summary>
    /// The name of the style used to visualize the markdown. If this string is null or empty, the current global
    /// defined style of the current Altaxo instance will be used.
    /// </summary>
    /// <value>
    /// The name of the style.
    /// </value>
    public string StyleName
    {
      get
      {
        return _styleName;
      }
      set
      {
        if (!(_styleName == value))
        {
          _styleName = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets the source text of the markdown document.
    /// </summary>
    /// <value>
    /// The source text.
    /// </value>
    public string SourceText
    {
      get
      {
        return _sourceText;
      }
      set
      {
        if (!(_sourceText == value))
        {
          _sourceText = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether hyphenation is enabled.
    /// </summary>
    public bool? IsHyphenationEnabled
    {
      get
      {
        return _isHyphenationEnabled;
      }
      set
      {
        if (!(_isHyphenationEnabled == value))
        {
          _isHyphenationEnabled = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Local images for this markdown, stored in a dictionary. The key is a Guid which is created when the image is pasted into the markdown document.
    /// The value is a memory stream image proxy.
    /// </summary>
    public IReadOnlyDictionary<string, MemoryStreamImageProxy> Images
    {
      get
      {
        return _images;
      }
    }

    #endregion SourceText and Style

    #region Images

    /// <summary>
    /// Adds the provided image to the document, and returns a name for that image.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <returns>The name of the image added.</returns>
    /// <exception cref="ArgumentNullException">image</exception>
    public string AddImage(MemoryStreamImageProxy image)
    {
      if (null == image)
        throw new ArgumentNullException(nameof(image));

      if (!_images.ContainsKey(image.ContentHash))
        _images.Add(image.ContentHash, image);

      return image.ContentHash;
    }

    /// <summary>
    /// Adds the local images from another <see cref="TextDocument"/> to the local images of this instance.
    /// </summary>
    /// <param name="textDocument">The text document to copy the images from.</param>
    public void AddImagesFrom(TextDocument textDocument)
    {
      foreach (var entry in textDocument._images)
        AddImage(entry.Value);
    }

    #endregion Images
  }
}
