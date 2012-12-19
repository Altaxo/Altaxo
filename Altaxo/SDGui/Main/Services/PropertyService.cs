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
using System.Linq;
using System.Text;


using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Commands;

using Altaxo.Gui;
using Altaxo.Gui.SharpDevelop;

namespace Altaxo.Main.Services
{
	public class PropertyService : Altaxo.Main.Services.IPropertyService
	{
		/// <summary>Occurs when a property changed, argument is the key to the property that changed.</summary>
		public event Action<string> PropertyChanged;

		public PropertyService()
		{
			ICSharpCode.Core.PropertyService.PropertyChanged += new PropertyChangedEventHandler(PropertyService_PropertyChanged);

		}

		void PropertyService_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (null != PropertyChanged)
				PropertyChanged(e.Key);
		}


		public string ConfigDirectory
		{
			get
			{
				return ICSharpCode.Core.PropertyService.ConfigDirectory;
			}
		}

		public string Get(string property)
		{
			return ICSharpCode.Core.PropertyService.Get(property);
		}

		public T Get<T>(string property, T defaultValue)
		{
			if (Altaxo.Serialization.Xml.FrameworkXmlSerializationWrapper.IsSerializableType(typeof(T)))
			{
				var wrapp = ICSharpCode.Core.PropertyService.Get<Altaxo.Serialization.Xml.FrameworkXmlSerializationWrapper>(property, null);
				if (wrapp != null && wrapp.WrappedObject != null)
					return (T)wrapp.WrappedObject;
				else
					return defaultValue;
			}
			else
				return ICSharpCode.Core.PropertyService.Get<T>(property, defaultValue);
		}


		public void Set<T>(string property, T value)
		{
			if (Altaxo.Serialization.Xml.FrameworkXmlSerializationWrapper.IsSerializableType(typeof(T)))
				ICSharpCode.Core.PropertyService.Set(property, new Altaxo.Serialization.Xml.FrameworkXmlSerializationWrapper(value));
			else
				ICSharpCode.Core.PropertyService.Set(property, value);
		}
	}

}
