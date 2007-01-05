#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
using Altaxo.Calc;
using NUnit.Framework;


namespace AltaxoTest.Calc
{
  [TestFixture]
  public class TestBesselRelated
  {

    [Test]
    public  void TestBesselK1()
    {
      Assert.AreEqual(0.6019072301972345747375400,BesselRelated.BesselK1(1),1E-14);
    }

    [Test]
    public  void TestBesselExpK1()
    {
      Assert.AreEqual(1.636153486263258246513311,BesselRelated.BesselExpK1(1),1E-14);
    }
  }
}
