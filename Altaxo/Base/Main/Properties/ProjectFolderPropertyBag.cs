#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

namespace Altaxo.Main.Properties
{
	/// <summary>
	/// A property bag that holds the properties of a project folder.
	/// </summary>
	public class ProjectFolderPropertyBag
		:
		IPropertyBagOwner,
		Altaxo.Main.IDocumentNode,
		Main.IEventIndicatedDisposable,
		Main.INameOwner,
		Main.ICopyFrom
	{
		private object _parent;
		private string _name;
		private PropertyBag _propertyBag;
		private DateTime _creationTimeUtc;
		private DateTime _changeTimeUtc;

		/// <summary>
		/// The event that is fired when the object is disposed. First argument is the sender, second argument is the original source, and third argument is the event arg.
		/// </summary>
		public event Action<object, object, TunnelingEventArgs> TunneledEvent;

		/// <summary>
		/// Fired if the name has changed. Arguments are the name owner (which has already the new name), and the old name.
		/// </summary>
		public event Action<INameOwner, string> NameChanged;

		/// <summary>
		/// Fired before the name will change. Arguments are the name owner (which has still the old name, the new name, and CancelEventArgs.
		/// If any of the listeners set Cancel to true, the name will not be changed.
		/// </summary>
		public event Action<INameOwner, string, System.ComponentModel.CancelEventArgs> PreviewNameChange;

		#region Serialization

		/// <summary>
		/// 2014-01-22 Initial version
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ProjectFolderPropertyBag), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ProjectFolderPropertyBag)obj;

				info.AddValue("Name", s._name);
				info.AddValue("CreationTimeUtc", s._creationTimeUtc);
				info.AddValue("ChangeTimeUtc", s._changeTimeUtc);
				info.AddValue("Properties", s._propertyBag);
			}

			public void Deserialize(ProjectFolderPropertyBag s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				s._name = info.GetString("Name");
				s._creationTimeUtc = info.GetDateTime("CreationTimeUtc");
				s._changeTimeUtc = info.GetDateTime("ChangeTimeUtc");
				s._propertyBag = (Main.Properties.PropertyBag)info.GetValue("Properties");
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (ProjectFolderPropertyBag)o : new ProjectFolderPropertyBag(string.Empty);
				Deserialize(s, info, parent);
				return s;
			}
		}

		#endregion Serialization

		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectFolderPropertyBag"/> class.
		/// </summary>
		/// <param name="folderName">Name of the folder.</param>
		public ProjectFolderPropertyBag(string folderName)
		{
			this.Name = folderName;
			_creationTimeUtc = _changeTimeUtc = DateTime.UtcNow;
			_propertyBag = new PropertyBag();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectFolderPropertyBag"/> class.
		/// </summary>
		/// <param name="from">Another instance to copy the name of the bag and the properties from.</param>
		public ProjectFolderPropertyBag(ProjectFolderPropertyBag from)
		{
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

			var from = (ProjectFolderPropertyBag)obj;
			if (null != from)
			{
				this._name = from._name;
				this._changeTimeUtc = from._changeTimeUtc;
				this._propertyBag = null;
				if (null != from._propertyBag)
				{
					this._propertyBag = this.PropertyBagNotNull;
					this._propertyBag.Clear();
					this._propertyBag.CopyFrom(from._propertyBag);
				}
				return true;
			}
			return false;
		}

		object ICloneable.Clone()
		{
			return new ProjectFolderPropertyBag(this);
		}

		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns>Clone of this instance.</returns>
		public ProjectFolderPropertyBag Clone()
		{
			return new ProjectFolderPropertyBag(this);
		}

		/// <summary>
		/// Gets or sets the name of the property bag. This has to be a valid project folder name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				Main.ProjectFolder.ThrowExceptionOnInvalidSingleFolderName(value);

				var oldName = _name;
				if (oldName == value)
					return;

				var e = new System.ComponentModel.CancelEventArgs();
				if (null != PreviewNameChange)
					PreviewNameChange(this, value, e);

				if (e.Cancel)
					return;

				_name = value;

				if (null != NameChanged)
					NameChanged(this, _name);
			}
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
		public PropertyBag PropertyBag
		{
			get { return _propertyBag; }
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
				if (null == _propertyBag)
					_propertyBag = new PropertyBag();
				return _propertyBag;
			}
		}

		/// <summary>
		/// Retrieves the parent object.
		/// </summary>
		public object ParentObject
		{
			get { return _parent; }
			set { _parent = value; }
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (null != _propertyBag)
			{
				_propertyBag.Dispose();
				_propertyBag = null;
			}

			if (null != TunneledEvent)
				TunneledEvent(this, null, DisposeEventArgs.Empty);
		}
	}
}