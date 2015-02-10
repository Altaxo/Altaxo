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

#endregion Copyright

using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Rescaling;
using Altaxo.Serialization;
using System;

namespace Altaxo.Gui.Graph.Scales.Rescaling
{
	#region Interfaces

	public interface IOrgEndSpanView
	{
		IOrgEndSpanViewEventReceiver Controller { get; set; }

		void SetLabel1(string txt);

		void SetLabel2(string txt);

		void SetLabel3(string txt);

		void SetChoice1(string[] choices, int selected);

		void SetChoice2(string[] choices, int selected);

		void SetChoice3(string[] choices, int selected);

		void SetValue1(string txt);

		void SetValue2(string txt);

		void SetValue3(string txt);

		void EnableChoice1(bool enable);

		void EnableChoice2(bool enable);

		void EnableChoice3(bool enable);

		void EnableValue1(bool enable);

		void EnableValue2(bool enable);

		void EnableValue3(bool enable);
	}

	public interface IOrgEndSpanViewEventReceiver
	{
		void EhChoice1Changed(string txt, int selected);

		void EhChoice2Changed(string txt, int selected);

		void EhChoice3Changed(string txt, int selected);

		bool EhValue1Changed(string txt);

		bool EhValue2Changed(string txt);

		bool EhValue3Changed(string txt);

		bool EhValue4Changed(string txt);

		bool EhValue5Changed(string txt);
	}

	#endregion Interfaces
}