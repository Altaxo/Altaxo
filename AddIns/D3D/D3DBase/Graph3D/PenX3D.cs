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

using Altaxo.Graph;
using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph3D
{
	public class PenX3D :
		Main.SuspendableDocumentNodeWithEventArgs,
		ICloneable, IDisposable
	{
		public double _thickness1;
		public double _thickness2;
		private NamedColor _color;

		protected Primitives.ICrossSectionOfLine _crossSection;

		public PenX3D(NamedColor color, double thickness)
		{
			_color = color;
			_thickness1 = thickness;
			_thickness2 = thickness;
			_crossSection = Primitives.CrossSectionOfLine.GetSquareCrossSection(_thickness1, _thickness2);
		}

		public object Clone()
		{
			throw new NotImplementedException();
		}

		protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			yield break;
		}

		public double Thickness1
		{
			get
			{
				return _thickness1;
			}
			set
			{
				var oldValue = _thickness1;
				_thickness1 = value;

				if (value != oldValue)
					EhSelfChanged();
			}
		}

		public double Thickness2
		{
			get
			{
				return _thickness2;
			}
			set
			{
				var oldValue = _thickness2;
				_thickness2 = value;

				if (value != oldValue)
					EhSelfChanged();
			}
		}

		public NamedColor Color
		{
			get
			{
				return _color;
			}
			set
			{
				var oldValue = _color;
				_color = value;

				if (value != oldValue)
					EhSelfChanged();
			}
		}

		public Primitives.ICrossSectionOfLine CrossSection
		{
			get
			{
				return _crossSection;
			}
			set
			{
				var oldValue = _crossSection;
				_crossSection = value;

				if (!object.ReferenceEquals(value, oldValue))
					EhSelfChanged();
			}
		}
	}
}