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
using System.IO;
using System.Linq;
using System.Text;

namespace Altaxo.Serialization.Ascii
{
	/// <summary>
	/// Holds an absolute path, plus the same path but relative to the project file.
	/// </summary>
	public class AbsoluteAndRelativeFilePath : Main.ICopyFrom
	{
		private string _absoluteFilePath;
		private string _relativeFilePath;

		#region Serialization

		/// <summary>
		/// 2014-07-28 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AbsoluteAndRelativeFilePath), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (AbsoluteAndRelativeFilePath)obj;

				s.TrySetRelativeFileName(); // set the relative file name now, because project name may have changed in the last seconds

				info.AddValue("AbsolutePath", s._absoluteFilePath);
				info.AddValue("RelativePath", s._relativeFilePath);
			}

			protected virtual AbsoluteAndRelativeFilePath SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var absoluteFilePath = info.GetString("AbsolutePath");

				var s = (o == null ? new AbsoluteAndRelativeFilePath(absoluteFilePath) : (AbsoluteAndRelativeFilePath)o);

				s._absoluteFilePath = absoluteFilePath;
				s._relativeFilePath = info.GetString("RelativePath");

				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = SDeserialize(o, info, parent);
				return s;
			}
		}

		#endregion Serialization

		public AbsoluteAndRelativeFilePath(string fileName)
		{
			AbsoluteFilePath = fileName;
		}

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as AbsoluteAndRelativeFilePath;
			if (null != from)
			{
				_absoluteFilePath = from._absoluteFilePath;
				_relativeFilePath = from._relativeFilePath;
				return true;
			}
			return false;
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		public string AbsoluteFilePath
		{
			get
			{
				return _absoluteFilePath;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("Path is null or empty");
				if (!Path.IsPathRooted(value))
					throw new ArgumentException("Path has to be an absolute path, i.e. it must be rooted!");

				_absoluteFilePath = value;
			}
		}

		public string GetResolvedFileNameOrNull()
		{
			if (File.Exists(_absoluteFilePath))
				return _absoluteFilePath;

			var projectFile = Current.ProjectService.CurrentProjectFileName;

			if (string.IsNullOrEmpty(projectFile))
				return null;

			if (string.IsNullOrEmpty(_relativeFilePath))
				return null;

			var projectDirectory = Path.GetDirectoryName(projectFile);

			var combinedPath = Path.Combine(projectDirectory, _relativeFilePath);

			if (File.Exists(combinedPath))
			{
				_absoluteFilePath = ConvertToNormalizedAbsolutePath(combinedPath);
				return _absoluteFilePath;
			}

			return null;
		}

		private string ConvertToNormalizedAbsolutePath(string path)
		{
			var root = System.IO.Path.GetPathRoot(path);
			var rest = path.Substring(root.Length);

			var restParted = rest.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
			var stack = new Stack<string>();
			for (int i = 0; i < restParted.Length; ++i)
			{
				if (restParted[i] == "..")
				{
					stack.Pop();
				}
				else if (restParted[i] == ".")
				{
				}
				else
				{
					stack.Push(restParted[i]);
				}
			}

			var result = Path.Combine(root, string.Join(Path.DirectorySeparatorChar.ToString(), stack.Reverse().ToArray()));

			return result;
		}

		private bool TrySetRelativeFileName()
		{
			string projectFile = Current.ProjectService.CurrentProjectFileName;

			if (null == projectFile)
			{
				_relativeFilePath = null;
				return false;
			}

			var partThis = _absoluteFilePath.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
			var partProj = projectFile.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

			int firstDeviatingIdx = 0;
			int len = Math.Min(partThis.Length, partProj.Length);

			for (firstDeviatingIdx = 0; firstDeviatingIdx < len; ++firstDeviatingIdx)
			{
				if (partThis[firstDeviatingIdx] != partProj[firstDeviatingIdx])
					break;
			}

			if (firstDeviatingIdx == 0)
			{
				_relativeFilePath = null;
				return false;
			}

			// i now designates the point where both path diverge for the first time.

			var stb = new System.Text.StringBuilder();

			int stepsBack = partProj.Length - firstDeviatingIdx - 1;

			if (stepsBack > 0)
			{
				for (int i = 0; i < stepsBack; ++i)
				{
					stb.Append("..");
					stb.Append(Path.DirectorySeparatorChar);
				}
			}
			else
			{
				stb.Append(".");
				stb.Append(Path.DirectorySeparatorChar);
			}

			for (int i = firstDeviatingIdx; i < partThis.Length - 1; ++i)
			{
				stb.Append(partThis[i]);
				stb.Append(Path.DirectorySeparatorChar);
			}

			stb.Append(partThis[partThis.Length - 1]);

			_relativeFilePath = stb.ToString();

			return true;
		}

		public override bool Equals(object obj)
		{
			var from = obj as AbsoluteAndRelativeFilePath;
			if (null != from)
				return from._absoluteFilePath == this._absoluteFilePath && from._relativeFilePath == this._relativeFilePath;
			else
				return false;
		}

		public override int GetHashCode()
		{
			return _absoluteFilePath.GetHashCode();
		}
	}
}