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
	using Camera;

	public class SceneSettings : Main.SuspendableDocumentLeafNodeWithEventArgs, Main.ICopyFrom
	{
		private CameraBase _camera;

		private LightSettings _lighting;

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

			_camera = new Camera.OrthographicCamera();
			_lighting = new LightSettings();
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
				this._camera = from.Camera;
				this._lighting = from._lighting;
				EhSelfChanged();
				return true;
			}

			return false;
		}

		public object Clone()
		{
			return new SceneSettings(this);
		}

		public CameraBase Camera
		{
			get
			{
				return _camera;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				var oldValue = _camera;
				_camera = value;

				if (!object.ReferenceEquals(oldValue, value))
					EhSelfChanged(CameraChangedEventArgs.Empty);
			}
		}

		public LightSettings Lighting
		{
			get
			{
				return _lighting;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				var oldValue = _lighting;
				_lighting = value;

				if (!object.ReferenceEquals(oldValue, value))
					EhSelfChanged(CameraChangedEventArgs.Empty);
			}
		}
	}
}