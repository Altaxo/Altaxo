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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph.Graph3D
{
	public class SceneSettings : Main.SuspendableDocumentLeafNodeWithEventArgs, Main.ICopyFrom
	{
		private Camera.CameraBase _camera;

		#region Serialization

		/// <summary>
		/// 2015-11-18 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SceneSettings), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (SceneSettings)obj;

				info.AddValue("Camera", s._camera);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (SceneSettings)o ?? new SceneSettings();
				s._camera = (Camera.CameraBase)info.GetValue("Camera", s);
				return s;
			}
		}

		#endregion Serialization

		public SceneSettings()
		{
			//_camera = new Camera.PerspectiveCamera();

			_camera = new Camera.OrthographicCamera() { Scale = 1000 };
		}

		public SceneSettings(SceneSettings from)
		{
			CopyFrom(from);
		}

		public virtual bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as SceneSettings;

			if (null != from)
			{
				this._camera = (Camera.CameraBase)from._camera.Clone();
				EhSelfChanged();
				return true;
			}

			return false;
		}

		public object Clone()
		{
			return new SceneSettings(this);
		}

		public Camera.CameraBase Camera
		{
			get
			{
				return _camera;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				_camera = value;

				EhSelfChanged();
			}
		}
	}
}