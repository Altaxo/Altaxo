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

using Altaxo.Graph.Scales.Rescaling;
using Altaxo.Main;
using NUnit.Framework;
using System;

namespace AltaxoTest.Graph.Scales.Rescaling
{
	/// <summary>
	/// Summary description for DoubleColumn_Test.
	/// </summary>
	[TestFixture]
	public class LinearScaleRescaleConditions_Test
	{
		#region Tests for RelativeTo = Absolute

		[Test]
		public void T001_RescalingDefaultAuto()
		{
			var s = new LinearScaleRescaleConditions();
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);

			s.OnDataBoundsChanged(1000, 4000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(4000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);

			s.OnDataBoundsChanged(-1000, -500);
			Assert.AreEqual(-1000, s.ResultingOrg);
			Assert.AreEqual(-500, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);

			s.OnUserZoomed(100, 200);
			Assert.AreEqual(100, s.ResultingOrg);
			Assert.AreEqual(200, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.AutoTempFixed, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.AutoTempFixed, s.EndRescaling);
			Assert.AreEqual(100, s.UserProvidedOrgValue);
			Assert.AreEqual(200, s.UserProvidedEndValue);

			// here before rescaling we still have autoTempFixed
			s.OnUserRescaled();
			Assert.AreEqual(-1000, s.ResultingOrg);
			Assert.AreEqual(-500, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.Auto, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Auto, s.EndRescaling);

			// Rescale again, now with Auto
			s.OnUserRescaled();
			Assert.AreEqual(-1000, s.ResultingOrg);
			Assert.AreEqual(-500, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.Auto, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Auto, s.EndRescaling);

			// Setting user parameters to some values with Auto should not show any changes in the results
			s.SetUserParameters(BoundaryRescaling.Auto, 5000, BoundaryRescaling.Auto, 6000);
			Assert.AreEqual(-1000, s.ResultingOrg);
			Assert.AreEqual(-500, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.Auto, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Auto, s.EndRescaling);
		}

		[Test]
		public void T002_RescalingAutoTempFixed()
		{
			var s = new LinearScaleRescaleConditions();
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);

			s.SetUserParameters(BoundaryRescaling.AutoTempFixed, 0, BoundaryRescaling.AutoTempFixed, 3000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);

			// Expected when data changed: recalculation of bounds according to data, fall back to auto rescaling
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(BoundaryRescaling.Auto, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Auto, s.EndRescaling);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);

			s.SetUserParameters(BoundaryRescaling.AutoTempFixed, 0, BoundaryRescaling.AutoTempFixed, 3000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);

			// Expected when zoomed by user: use zoom values as values in AutoTempFixed user parameters
			s.OnUserZoomed(1500, 1600);
			Assert.AreEqual(1500, s.ResultingOrg);
			Assert.AreEqual(1600, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.AutoTempFixed, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.AutoTempFixed, s.EndRescaling);
			Assert.AreEqual(1500, s.UserProvidedOrgValue);
			Assert.AreEqual(1600, s.UserProvidedEndValue);

			// Expected when rescaled by user: fall back to data bounds, and fall back to auto rescaling
			s.OnUserRescaled();
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(BoundaryRescaling.Auto, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Auto, s.EndRescaling);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
		}

		[Test]
		public void T003_RescalingFixed()
		{
			var s = new LinearScaleRescaleConditions();
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);

			s.SetUserParameters(BoundaryRescaling.Fixed, 0, BoundaryRescaling.Fixed, 3000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);

			// Expected when data changed: no change
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.Fixed, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Fixed, s.EndRescaling);

			// Expected when zoomed by user: no change
			s.OnUserZoomed(1500, 1600);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.Fixed, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Fixed, s.EndRescaling);
			Assert.AreEqual(0, s.UserProvidedOrgValue);
			Assert.AreEqual(3000, s.UserProvidedEndValue);

			// Expected when rescaled by user: no change
			s.OnUserRescaled();
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.Fixed, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Fixed, s.EndRescaling);
			Assert.AreEqual(0, s.UserProvidedOrgValue);
			Assert.AreEqual(3000, s.UserProvidedEndValue);
		}

		[Test]
		public void T004_RescalingFixedManually()
		{
			var s = new LinearScaleRescaleConditions();
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);

			s.SetUserParameters(BoundaryRescaling.FixedManually, 0, BoundaryRescaling.FixedManually, 3000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);

			// Expected when data changed: no change
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.FixedManually, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.FixedManually, s.EndRescaling);

			// Expected when zoomed by user: use zoom values as user provided values
			s.OnUserZoomed(1500, 1600);
			Assert.AreEqual(1500, s.ResultingOrg);
			Assert.AreEqual(1600, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.FixedManually, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.FixedManually, s.EndRescaling);
			Assert.AreEqual(1500, s.UserProvidedOrgValue);
			Assert.AreEqual(1600, s.UserProvidedEndValue);

			// Expected when rescaled by user: no change, or ask user to change to Auto or AutoTempFixed
			s.OnUserRescaled();
			Assert.AreEqual(1500, s.ResultingOrg);
			Assert.AreEqual(1600, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.FixedManually, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.FixedManually, s.EndRescaling);
			Assert.AreEqual(1500, s.UserProvidedOrgValue);
			Assert.AreEqual(1600, s.UserProvidedEndValue);
		}

		[Test]
		public void T005_RescalingFixedZoomable()
		{
			var s = new LinearScaleRescaleConditions();
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);

			s.SetUserParameters(BoundaryRescaling.FixedZoomable, 0, BoundaryRescaling.FixedZoomable, 3000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);

			// Expected when data changed: no change
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.FixedZoomable, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.FixedZoomable, s.EndRescaling);

			// Expected when zoomed by user: use zoom values, but keep original user provided values
			s.OnUserZoomed(1500, 1600);
			Assert.AreEqual(1500, s.ResultingOrg);
			Assert.AreEqual(1600, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.FixedZoomable, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.FixedZoomable, s.EndRescaling);
			Assert.AreEqual(0, s.UserProvidedOrgValue);
			Assert.AreEqual(3000, s.UserProvidedEndValue);

			// Expected when rescaled by user: no change, or ask user to change to Auto or AutoTempFixed
			s.OnUserRescaled();
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.FixedZoomable, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.FixedZoomable, s.EndRescaling);
			Assert.AreEqual(0, s.UserProvidedOrgValue);
			Assert.AreEqual(3000, s.UserProvidedEndValue);
		}

		[Test]
		public void T006_RescalingGreaterThan()
		{
			var s = new LinearScaleRescaleConditions();
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);

			s.SetUserParameters(BoundaryRescaling.GreaterOrEqual, 1000, BoundaryRescaling.GreaterOrEqual, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(1200, 2000);
			Assert.AreEqual(1200, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed); // can go down to 1000
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(800, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(1000, 1800);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(1000, 2200);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2200, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);

			s.OnDataBoundsChanged(1000, 2000);
			s.SetUserParameters(BoundaryRescaling.GreaterOrEqual, 1100, BoundaryRescaling.GreaterOrEqual, 3000);

			// Expected when zoomed by user: use zoom values, but keep original user provided values
			s.OnUserZoomed(1500, 1600);
			Assert.AreEqual(1500, s.ResultingOrg);
			Assert.AreEqual(1600, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
			Assert.AreEqual(1100, s.UserProvidedOrgValue);
			Assert.AreEqual(3000, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.GreaterOrEqual, 800, BoundaryRescaling.GreaterOrEqual, 2200);
			s.OnDataBoundsChanged(1000, 2000);
			s.OnUserZoomed(-2000, -1000);

			// Expected when rescaled by user: use data bounds, and take into account user provided values
			s.OnUserRescaled();
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2200, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed); // because ResultingOrg can go down to 800
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
			Assert.AreEqual(800, s.UserProvidedOrgValue);
			Assert.AreEqual(2200, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.GreaterOrEqual, 1200, BoundaryRescaling.GreaterOrEqual, 1800);
			s.OnDataBoundsChanged(1000, 2000);
			s.OnUserZoomed(-2000, -1000);

			// Expected when rescaled by user: use data bounds, and take into account user provided values
			s.OnUserRescaled();
			Assert.AreEqual(1200, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
			Assert.AreEqual(1200, s.UserProvidedOrgValue);
			Assert.AreEqual(1800, s.UserProvidedEndValue);
		}

		[Test]
		public void T007_RescalingLessThan()
		{
			var s = new LinearScaleRescaleConditions();
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);

			s.SetUserParameters(BoundaryRescaling.LessOrEqual, 1000, BoundaryRescaling.LessOrEqual, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(800, 2000);
			Assert.AreEqual(800, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(1200, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(1000, 1800);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(1800, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed); // not fixed because it can go up to 2000
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(1000, 2200);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);

			s.SetUserParameters(BoundaryRescaling.LessOrEqual, 800, BoundaryRescaling.LessOrEqual, 1800);
			s.OnDataBoundsChanged(1000, 2000);

			// Expected when zoomed by user: use zoom values, but keep original user provided values
			s.OnUserZoomed(1500, 4000);
			Assert.AreEqual(1500, s.ResultingOrg);
			Assert.AreEqual(4000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(800, s.UserProvidedOrgValue);
			Assert.AreEqual(1800, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.LessOrEqual, 800, BoundaryRescaling.LessOrEqual, 2200);
			s.OnDataBoundsChanged(1000, 2000);
			s.OnUserZoomed(-2000, -1000);

			// Expected when rescaled by user: use data bounds, and take into account user provided values
			s.OnUserRescaled();
			Assert.AreEqual(800, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed); // because ResultingEnd can go up to 2200
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(800, s.UserProvidedOrgValue);
			Assert.AreEqual(2200, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.LessOrEqual, 1200, BoundaryRescaling.LessOrEqual, 1800);
			s.OnDataBoundsChanged(1000, 2000);
			s.OnUserZoomed(-2000, -1000);

			// Expected when rescaled by user: use data bounds, and take into account user provided values
			s.OnUserRescaled();
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(1800, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(1200, s.UserProvidedOrgValue);
			Assert.AreEqual(1800, s.UserProvidedEndValue);
		}

		#endregion Tests for RelativeTo = Absolute

		#region Tests for RelativeTo = RelativeToOrg

		[Test]
		public void T101_RescalingAuto()
		{
			var s = new LinearScaleRescaleConditions();
			s.SetUserParameters(BoundaryRescaling.Auto, BoundariesRelativeTo.RelativeToDataBoundsOrg, 7, BoundaryRescaling.Auto, BoundariesRelativeTo.RelativeToDataBoundsOrg, 11);
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);

			s.OnDataBoundsChanged(1000, 4000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(4000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);

			s.OnDataBoundsChanged(-1000, -500);
			Assert.AreEqual(-1000, s.ResultingOrg);
			Assert.AreEqual(-500, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);

			s.OnUserZoomed(100, 200);
			Assert.AreEqual(100, s.ResultingOrg);
			Assert.AreEqual(200, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(1100, s.UserProvidedOrgValue);
			Assert.AreEqual(1200, s.UserProvidedEndValue);
			Assert.AreEqual(BoundaryRescaling.AutoTempFixed, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.AutoTempFixed, s.EndRescaling);

			// here before rescaling we still have autoTempFixed
			s.OnUserRescaled();
			Assert.AreEqual(-1000, s.ResultingOrg);
			Assert.AreEqual(-500, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.Auto, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Auto, s.EndRescaling);

			// Rescale again, now with Auto
			s.OnUserRescaled();
			Assert.AreEqual(-1000, s.ResultingOrg);
			Assert.AreEqual(-500, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.Auto, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Auto, s.EndRescaling);

			// Setting user parameters to some values with Auto should not show any changes in the results
			s.SetUserParameters(BoundaryRescaling.Auto, BoundariesRelativeTo.RelativeToDataBoundsOrg, 5000, BoundaryRescaling.Auto, BoundariesRelativeTo.RelativeToDataBoundsOrg, 6000);
			Assert.AreEqual(-1000, s.ResultingOrg);
			Assert.AreEqual(-500, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.Auto, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Auto, s.EndRescaling);
		}

		[Test]
		public void T102_RescalingAutoTempFixed()
		{
			var s = new LinearScaleRescaleConditions();
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);

			s.SetUserParameters(BoundaryRescaling.AutoTempFixed, BoundariesRelativeTo.RelativeToDataBoundsOrg, -1000, BoundaryRescaling.AutoTempFixed, BoundariesRelativeTo.RelativeToDataBoundsOrg, 2000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(2000, s.UserProvidedEndValue);

			// Expected when data changed: recalculation of bounds according to data, fall back to auto rescaling
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(BoundaryRescaling.Auto, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Auto, s.EndRescaling);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);

			s.SetUserParameters(BoundaryRescaling.AutoTempFixed, BoundariesRelativeTo.RelativeToDataBoundsOrg, -1000, BoundaryRescaling.AutoTempFixed, BoundariesRelativeTo.RelativeToDataBoundsOrg, 2000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(2000, s.UserProvidedEndValue);

			// Expected when zoomed by user: use zoom values as values in AutoTempFixed user parameters
			s.OnUserZoomed(1500, 1600);
			Assert.AreEqual(1500, s.ResultingOrg);
			Assert.AreEqual(1600, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.AutoTempFixed, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.AutoTempFixed, s.EndRescaling);
			Assert.AreEqual(500, s.UserProvidedOrgValue);
			Assert.AreEqual(600, s.UserProvidedEndValue);

			// Expected when rescaled by user: fall back to data bounds, and fall back to auto rescaling
			s.OnUserRescaled();
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(BoundaryRescaling.Auto, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Auto, s.EndRescaling);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
		}

		[Test]
		public void T103_RescalingFixed()
		{
			var s = new LinearScaleRescaleConditions();
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);

			s.SetUserParameters(BoundaryRescaling.Fixed, BoundariesRelativeTo.RelativeToDataBoundsOrg, -1000, BoundaryRescaling.Fixed, BoundariesRelativeTo.RelativeToDataBoundsOrg, 2000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(2000, s.UserProvidedEndValue);

			// Expected when data changed: no change
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.Fixed, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Fixed, s.EndRescaling);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(2000, s.UserProvidedEndValue);

			// Expected when zoomed by user: no change
			s.OnUserZoomed(1500, 1600);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.Fixed, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Fixed, s.EndRescaling);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(2000, s.UserProvidedEndValue);

			// Expected when rescaled by user: no change
			s.OnUserRescaled();
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.Fixed, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Fixed, s.EndRescaling);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(2000, s.UserProvidedEndValue);
		}

		[Test]
		public void T104_RescalingFixedManually()
		{
			var s = new LinearScaleRescaleConditions();
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);

			s.SetUserParameters(BoundaryRescaling.FixedManually, BoundariesRelativeTo.RelativeToDataBoundsOrg, -1000, BoundaryRescaling.FixedManually, BoundariesRelativeTo.RelativeToDataBoundsOrg, 2000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(2000, s.UserProvidedEndValue);

			// Expected when data changed: no change
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.FixedManually, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.FixedManually, s.EndRescaling);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(2000, s.UserProvidedEndValue);

			// Expected when zoomed by user: use zoom values as user provided values
			s.OnUserZoomed(1500, 1600);
			Assert.AreEqual(1500, s.ResultingOrg);
			Assert.AreEqual(1600, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.FixedManually, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.FixedManually, s.EndRescaling);
			Assert.AreEqual(500, s.UserProvidedOrgValue);
			Assert.AreEqual(600, s.UserProvidedEndValue);

			// Expected when rescaled by user: no change, or ask user to change to Auto or AutoTempFixed
			s.OnUserRescaled();
			Assert.AreEqual(1500, s.ResultingOrg);
			Assert.AreEqual(1600, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.FixedManually, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.FixedManually, s.EndRescaling);
			Assert.AreEqual(500, s.UserProvidedOrgValue);
			Assert.AreEqual(600, s.UserProvidedEndValue);
		}

		[Test]
		public void T105_RescalingFixedZoomable()
		{
			var s = new LinearScaleRescaleConditions();
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);

			s.SetUserParameters(BoundaryRescaling.FixedZoomable, BoundariesRelativeTo.RelativeToDataBoundsOrg, -1000, BoundaryRescaling.FixedZoomable, BoundariesRelativeTo.RelativeToDataBoundsOrg, 2000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(2000, s.UserProvidedEndValue);

			// Expected when data changed: no change
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.FixedZoomable, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.FixedZoomable, s.EndRescaling);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(2000, s.UserProvidedEndValue);

			// Expected when zoomed by user: use zoom values, but keep original user provided values
			s.OnUserZoomed(1500, 1600);
			Assert.AreEqual(1500, s.ResultingOrg);
			Assert.AreEqual(1600, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.FixedZoomable, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.FixedZoomable, s.EndRescaling);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(2000, s.UserProvidedEndValue);

			// Expected when rescaled by user: no change, or ask user to change to Auto or AutoTempFixed
			s.OnUserRescaled();
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.FixedZoomable, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.FixedZoomable, s.EndRescaling);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(2000, s.UserProvidedEndValue);
		}

		[Test]
		public void T106_RescalingGreaterThan()
		{
			var s = new LinearScaleRescaleConditions();
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);

			s.SetUserParameters(BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 0, BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 1000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(0, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(1200, 2000);
			Assert.AreEqual(1200, s.ResultingOrg);
			Assert.AreEqual(2200, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
			Assert.AreEqual(0, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(800, 2000);
			Assert.AreEqual(800, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
			Assert.AreEqual(0, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(1000, 1800);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
			Assert.AreEqual(0, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(1000, 2200);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2200, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);

			// -----------------------------------------------------------
			// now keep data bounds constant, but change user parameters
			// -----------------------------------------------------------
			s.OnDataBoundsChanged(1000, 2000);
			s.SetUserParameters(BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, -200, BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 1000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed); // can go down to 800
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
			Assert.AreEqual(-200, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 200, BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 1000);
			Assert.AreEqual(1200, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
			Assert.AreEqual(200, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 0, BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 800);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
			Assert.AreEqual(0, s.UserProvidedOrgValue);
			Assert.AreEqual(800, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 0, BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 1200);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2200, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
			Assert.AreEqual(0, s.UserProvidedOrgValue);
			Assert.AreEqual(1200, s.UserProvidedEndValue);

			s.OnDataBoundsChanged(1000, 2000);
			for (int i = -1; i <= 1; ++i)
			{
				for (int j = -1; j <= 1; ++j)
				{
					s.SetUserParameters(BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, i, BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 1000 + j);

					Assert.AreEqual(Math.Max(1000, 1000 + i), s.ResultingOrg);
					Assert.AreEqual(Math.Max(2000, 2000 + j), s.ResultingEnd);
					Assert.AreEqual(!(i < 0), s.IsResultingOrgFixed); // can go up only if i<0
					Assert.AreEqual(false, s.IsResultingEndFixed);
					Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
					Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
					Assert.AreEqual(i, s.UserProvidedOrgValue);
					Assert.AreEqual(1000 + j, s.UserProvidedEndValue);
				}
			}

			// ----------------------------------------------------------------------
			// Test zoom
			// ----------------------------------------------------------------------

			// Expected when zoomed by user: use zoom values, but keep original user provided values
			s.OnDataBoundsChanged(1000, 2000);
			s.SetUserParameters(BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 100, BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 2000);
			s.OnUserZoomed(1500, 1600);
			Assert.AreEqual(1500, s.ResultingOrg);
			Assert.AreEqual(1600, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
			Assert.AreEqual(100, s.UserProvidedOrgValue);
			Assert.AreEqual(2000, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, -200, BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 1200);
			s.OnDataBoundsChanged(1000, 2000);
			s.OnUserZoomed(-2000, -1000);

			// Expected when rescaled by user: use data bounds, and take into account user provided values
			s.OnUserRescaled();
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2200, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed); // because ResultingOrg can go down to 800
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
			Assert.AreEqual(-200, s.UserProvidedOrgValue);
			Assert.AreEqual(1200, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 200, BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 800);
			s.OnDataBoundsChanged(1000, 2000);
			s.OnUserZoomed(-2000, -1000);

			// Expected when rescaled by user: use data bounds, and take into account user provided values
			s.OnUserRescaled();
			Assert.AreEqual(1200, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
			Assert.AreEqual(200, s.UserProvidedOrgValue);
			Assert.AreEqual(800, s.UserProvidedEndValue);
		}

		[Test]
		public void T107_RescalingLessThan()
		{
			var s = new LinearScaleRescaleConditions();
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);

			s.SetUserParameters(BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 0, BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 1000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(0, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(800, 2000);
			Assert.AreEqual(800, s.ResultingOrg);
			Assert.AreEqual(1800, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(0, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(1200, 2000);
			Assert.AreEqual(1200, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed); // because it can go up to 2200
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(0, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(1000, 1800);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(1800, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed); // because it can go up to 2000
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(0, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(1000, 2200);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(0, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			// -----------------------------------------------------------
			// now keep data bounds constant, but change user parameters
			// -----------------------------------------------------------
			s.OnDataBoundsChanged(1000, 2000);
			s.SetUserParameters(BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, -200, BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 1000);
			Assert.AreEqual(800, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed); // can go down to 800
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(-200, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 200, BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 1000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(200, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 0, BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 800);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(1800, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(0, s.UserProvidedOrgValue);
			Assert.AreEqual(800, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 0, BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 1200);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed); // can go up to 2200
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(0, s.UserProvidedOrgValue);
			Assert.AreEqual(1200, s.UserProvidedEndValue);

			s.OnDataBoundsChanged(1000, 2000);
			for (int i = -1; i <= 1; ++i)
			{
				for (int j = -1; j <= 1; ++j)
				{
					s.SetUserParameters(BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, i, BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 1000 + j);

					Assert.AreEqual(Math.Min(1000, 1000 + i), s.ResultingOrg);
					Assert.AreEqual(Math.Min(2000, 2000 + j), s.ResultingEnd);
					Assert.AreEqual(false, s.IsResultingOrgFixed);
					Assert.AreEqual(!(j > 0), s.IsResultingEndFixed); // can go down only if j>0
					Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
					Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
					Assert.AreEqual(i, s.UserProvidedOrgValue);
					Assert.AreEqual(1000 + j, s.UserProvidedEndValue);
				}
			}

			// -----------------------------------------------------------
			// Zoom test
			// -----------------------------------------------------------

			s.SetUserParameters(BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 800, BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 1800);
			s.OnDataBoundsChanged(1000, 2000);

			// Expected when zoomed by user: use zoom values, but keep original user provided values
			s.OnUserZoomed(1500, 4000);
			Assert.AreEqual(1500, s.ResultingOrg);
			Assert.AreEqual(4000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(800, s.UserProvidedOrgValue);
			Assert.AreEqual(1800, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, -200, BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 1200);
			s.OnDataBoundsChanged(1000, 2000);
			s.OnUserZoomed(-2000, -1000);

			// Expected when rescaled by user: use data bounds, and take into account user provided values
			s.OnUserRescaled();
			Assert.AreEqual(800, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed); // because ResultingEnd can go up to 2200
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(-200, s.UserProvidedOrgValue);
			Assert.AreEqual(1200, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 200, BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsOrg, 800);
			s.OnDataBoundsChanged(1000, 2000);
			s.OnUserZoomed(-2000, -1000);

			// Expected when rescaled by user: use data bounds, and take into account user provided values
			s.OnUserRescaled();
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(1800, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(200, s.UserProvidedOrgValue);
			Assert.AreEqual(800, s.UserProvidedEndValue);
		}

		#endregion Tests for RelativeTo = RelativeToOrg

		#region Tests for RelativeTo = RelativeToEnd

		[Test]
		public void T201_RescalingAuto()
		{
			var s = new LinearScaleRescaleConditions();
			s.SetUserParameters(BoundaryRescaling.Auto, BoundariesRelativeTo.RelativeToDataBoundsEnd, 7, BoundaryRescaling.Auto, BoundariesRelativeTo.RelativeToDataBoundsEnd, 11);
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);

			s.OnDataBoundsChanged(1000, 4000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(4000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);

			s.OnDataBoundsChanged(-1000, -500);
			Assert.AreEqual(-1000, s.ResultingOrg);
			Assert.AreEqual(-500, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);

			s.OnUserZoomed(100, 200);
			Assert.AreEqual(100, s.ResultingOrg);
			Assert.AreEqual(200, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(600, s.UserProvidedOrgValue);
			Assert.AreEqual(700, s.UserProvidedEndValue);
			Assert.AreEqual(BoundaryRescaling.AutoTempFixed, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.AutoTempFixed, s.EndRescaling);

			// here before rescaling we still have autoTempFixed
			s.OnUserRescaled();
			Assert.AreEqual(-1000, s.ResultingOrg);
			Assert.AreEqual(-500, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.Auto, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Auto, s.EndRescaling);

			// Rescale again, now with Auto
			s.OnUserRescaled();
			Assert.AreEqual(-1000, s.ResultingOrg);
			Assert.AreEqual(-500, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.Auto, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Auto, s.EndRescaling);

			// Setting user parameters to some values with Auto should not show any changes in the results
			s.SetUserParameters(BoundaryRescaling.Auto, BoundariesRelativeTo.RelativeToDataBoundsEnd, 5000, BoundaryRescaling.Auto, BoundariesRelativeTo.RelativeToDataBoundsEnd, 6000);
			Assert.AreEqual(-1000, s.ResultingOrg);
			Assert.AreEqual(-500, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.Auto, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Auto, s.EndRescaling);
		}

		[Test]
		public void T202_RescalingAutoTempFixed()
		{
			var s = new LinearScaleRescaleConditions();
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);

			s.SetUserParameters(BoundaryRescaling.AutoTempFixed, BoundariesRelativeTo.RelativeToDataBoundsEnd, -2000, BoundaryRescaling.AutoTempFixed, BoundariesRelativeTo.RelativeToDataBoundsEnd, 1000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(-2000, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			// Expected when data changed: recalculation of bounds according to data, fall back to auto rescaling
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(BoundaryRescaling.Auto, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Auto, s.EndRescaling);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);

			s.SetUserParameters(BoundaryRescaling.AutoTempFixed, BoundariesRelativeTo.RelativeToDataBoundsEnd, -2000, BoundaryRescaling.AutoTempFixed, BoundariesRelativeTo.RelativeToDataBoundsEnd, 1000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(-2000, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			// Expected when zoomed by user: use zoom values as values in AutoTempFixed user parameters
			s.OnUserZoomed(1500, 1600);
			Assert.AreEqual(1500, s.ResultingOrg);
			Assert.AreEqual(1600, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.AutoTempFixed, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.AutoTempFixed, s.EndRescaling);
			Assert.AreEqual(-500, s.UserProvidedOrgValue);
			Assert.AreEqual(-400, s.UserProvidedEndValue);

			// Expected when rescaled by user: fall back to data bounds, and fall back to auto rescaling
			s.OnUserRescaled();
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(BoundaryRescaling.Auto, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Auto, s.EndRescaling);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
		}

		[Test]
		public void T203_RescalingFixed()
		{
			var s = new LinearScaleRescaleConditions();
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);

			s.SetUserParameters(BoundaryRescaling.Fixed, BoundariesRelativeTo.RelativeToDataBoundsEnd, -2000, BoundaryRescaling.Fixed, BoundariesRelativeTo.RelativeToDataBoundsEnd, 1000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(-2000, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			// Expected when data changed: no change
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.Fixed, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Fixed, s.EndRescaling);
			Assert.AreEqual(-2000, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			// Expected when zoomed by user: no change
			s.OnUserZoomed(1500, 1600);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.Fixed, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Fixed, s.EndRescaling);
			Assert.AreEqual(-2000, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			// Expected when rescaled by user: no change
			s.OnUserRescaled();
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.Fixed, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.Fixed, s.EndRescaling);
			Assert.AreEqual(-2000, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);
		}

		[Test]
		public void T204_RescalingFixedManually()
		{
			var s = new LinearScaleRescaleConditions();
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);

			s.SetUserParameters(BoundaryRescaling.FixedManually, BoundariesRelativeTo.RelativeToDataBoundsEnd, -2000, BoundaryRescaling.FixedManually, BoundariesRelativeTo.RelativeToDataBoundsEnd, 1000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(-2000, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			// Expected when data changed: no change
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.FixedManually, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.FixedManually, s.EndRescaling);
			Assert.AreEqual(-2000, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			// Expected when zoomed by user: use zoom values as user provided values
			s.OnUserZoomed(1500, 1600);
			Assert.AreEqual(1500, s.ResultingOrg);
			Assert.AreEqual(1600, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.FixedManually, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.FixedManually, s.EndRescaling);
			Assert.AreEqual(-500, s.UserProvidedOrgValue);
			Assert.AreEqual(-400, s.UserProvidedEndValue);

			// Expected when rescaled by user: no change, or ask user to change to Auto or AutoTempFixed
			s.OnUserRescaled();
			Assert.AreEqual(1500, s.ResultingOrg);
			Assert.AreEqual(1600, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.FixedManually, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.FixedManually, s.EndRescaling);
			Assert.AreEqual(-500, s.UserProvidedOrgValue);
			Assert.AreEqual(-400, s.UserProvidedEndValue);
		}

		[Test]
		public void T205_RescalingFixedZoomable()
		{
			var s = new LinearScaleRescaleConditions();
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);

			s.SetUserParameters(BoundaryRescaling.FixedZoomable, BoundariesRelativeTo.RelativeToDataBoundsEnd, -2000, BoundaryRescaling.FixedZoomable, BoundariesRelativeTo.RelativeToDataBoundsEnd, 1000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(-2000, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			// Expected when data changed: no change
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.FixedZoomable, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.FixedZoomable, s.EndRescaling);
			Assert.AreEqual(-2000, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			// Expected when zoomed by user: use zoom values, but keep original user provided values
			s.OnUserZoomed(1500, 1600);
			Assert.AreEqual(1500, s.ResultingOrg);
			Assert.AreEqual(1600, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.FixedZoomable, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.FixedZoomable, s.EndRescaling);
			Assert.AreEqual(-2000, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			// Expected when rescaled by user: no change, or ask user to change to Auto or AutoTempFixed
			s.OnUserRescaled();
			Assert.AreEqual(0, s.ResultingOrg);
			Assert.AreEqual(3000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.FixedZoomable, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.FixedZoomable, s.EndRescaling);
			Assert.AreEqual(-2000, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);
		}

		[Test]
		public void T206_RescalingGreaterThan()
		{
			var s = new LinearScaleRescaleConditions();
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);

			s.SetUserParameters(BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, -1000, BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, 0);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(0, s.UserProvidedEndValue);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(1200, 2000);
			Assert.AreEqual(1200, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed); // can go down to 1000
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(0, s.UserProvidedEndValue);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(800, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(0, s.UserProvidedEndValue);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(1000, 1800);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(1800, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed); // can go down to 800
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(0, s.UserProvidedEndValue);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(1000, 2200);
			Assert.AreEqual(1200, s.ResultingOrg);
			Assert.AreEqual(2200, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);

			// -----------------------------------------------------------
			// now keep data bounds constant, but change user parameters
			// -----------------------------------------------------------
			s.OnDataBoundsChanged(1000, 2000);
			s.SetUserParameters(BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, -1200, BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, 0);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed); // can go down to 800
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
			Assert.AreEqual(-1200, s.UserProvidedOrgValue);
			Assert.AreEqual(0, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, -800, BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, 0);
			Assert.AreEqual(1200, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
			Assert.AreEqual(-800, s.UserProvidedOrgValue);
			Assert.AreEqual(0, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, -1000, BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, -200);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(-200, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, -1000, BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, 200);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2200, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(200, s.UserProvidedEndValue);

			s.OnDataBoundsChanged(1000, 2000);
			for (int i = -1; i <= 1; ++i)
			{
				for (int j = -1; j <= 1; ++j)
				{
					s.SetUserParameters(BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, -1000 + i, BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, j);

					Assert.AreEqual(Math.Max(1000, 1000 + i), s.ResultingOrg);
					Assert.AreEqual(Math.Max(2000, 2000 + j), s.ResultingEnd);
					Assert.AreEqual(!(i < 0), s.IsResultingOrgFixed); // can go up only if i<0
					Assert.AreEqual(false, s.IsResultingEndFixed);
					Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
					Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
					Assert.AreEqual(-1000 + i, s.UserProvidedOrgValue);
					Assert.AreEqual(j, s.UserProvidedEndValue);
				}
			}

			// Expected when zoomed by user: use zoom values, but keep original user provided values
			s.OnDataBoundsChanged(1000, 2000);
			s.SetUserParameters(BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, -900, BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, 1000);
			s.OnUserZoomed(1500, 1600);
			Assert.AreEqual(1500, s.ResultingOrg);
			Assert.AreEqual(1600, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
			Assert.AreEqual(-900, s.UserProvidedOrgValue);
			Assert.AreEqual(1000, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, -1200, BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, 200);
			s.OnDataBoundsChanged(1000, 2000);
			s.OnUserZoomed(-2000, -1000);

			// Expected when rescaled by user: use data bounds, and take into account user provided values
			s.OnUserRescaled();
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2200, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed); // because ResultingOrg can go down to 800
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
			Assert.AreEqual(-1200, s.UserProvidedOrgValue);
			Assert.AreEqual(200, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, -800, BoundaryRescaling.GreaterOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, -200);
			s.OnDataBoundsChanged(1000, 2000);
			s.OnUserZoomed(-2000, -1000);

			// Expected when rescaled by user: use data bounds, and take into account user provided values
			s.OnUserRescaled();
			Assert.AreEqual(1200, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.GreaterOrEqual, s.EndRescaling);
			Assert.AreEqual(-800, s.UserProvidedOrgValue);
			Assert.AreEqual(-200, s.UserProvidedEndValue);
		}

		[Test]
		public void T207_RescalingLessThan()
		{
			var s = new LinearScaleRescaleConditions();
			s.OnDataBoundsChanged(1000, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);

			s.SetUserParameters(BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, -1000, BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, 0);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(0, s.UserProvidedEndValue);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(800, 2000);
			Assert.AreEqual(800, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(0, s.UserProvidedEndValue);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(1200, 2000);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(0, s.UserProvidedEndValue);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(1000, 1800);
			Assert.AreEqual(800, s.ResultingOrg);
			Assert.AreEqual(1800, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(0, s.UserProvidedEndValue);

			// Expected when data changed: recalculation of result according to data and user parameters
			s.OnDataBoundsChanged(1000, 2200);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2200, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(0, s.UserProvidedEndValue);

			// -----------------------------------------------------------
			// now keep data bounds constant, but change user parameters
			// -----------------------------------------------------------
			s.OnDataBoundsChanged(1000, 2000);
			s.SetUserParameters(BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, -1200, BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, 0);
			Assert.AreEqual(800, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed); // can go down to 800
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(-1200, s.UserProvidedOrgValue);
			Assert.AreEqual(0, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, -800, BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, 0);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(-800, s.UserProvidedOrgValue);
			Assert.AreEqual(0, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, -1000, BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, -200);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(1800, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(-200, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, -1000, BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, 200);
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed); // can go up to 2200
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(-1000, s.UserProvidedOrgValue);
			Assert.AreEqual(200, s.UserProvidedEndValue);

			s.OnDataBoundsChanged(1000, 2000);
			for (int i = -1; i <= 1; ++i)
			{
				for (int j = -1; j <= 1; ++j)
				{
					s.SetUserParameters(BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, -1000 + i, BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, j);

					Assert.AreEqual(Math.Min(1000, 1000 + i), s.ResultingOrg);
					Assert.AreEqual(Math.Min(2000, 2000 + j), s.ResultingEnd);
					Assert.AreEqual(false, s.IsResultingOrgFixed);
					Assert.AreEqual(!(j > 0), s.IsResultingEndFixed); // can go down only if j>0
					Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
					Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
					Assert.AreEqual(-1000 + i, s.UserProvidedOrgValue);
					Assert.AreEqual(j, s.UserProvidedEndValue);
				}
			}

			// -----------------------------------------------------------
			// Zoom test
			// -----------------------------------------------------------

			s.SetUserParameters(BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, -200, BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, 800);
			s.OnDataBoundsChanged(1000, 2000);

			// Expected when zoomed by user: use zoom values, but keep original user provided values
			s.OnUserZoomed(1500, 4000);
			Assert.AreEqual(1500, s.ResultingOrg);
			Assert.AreEqual(4000, s.ResultingEnd);
			Assert.AreEqual(true, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(-200, s.UserProvidedOrgValue);
			Assert.AreEqual(800, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, -1200, BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, 200);
			s.OnDataBoundsChanged(1000, 2000);
			s.OnUserZoomed(-2000, -1000);

			// Expected when rescaled by user: use data bounds, and take into account user provided values
			s.OnUserRescaled();
			Assert.AreEqual(800, s.ResultingOrg);
			Assert.AreEqual(2000, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(false, s.IsResultingEndFixed); // because ResultingEnd can go up to 2200
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(-1200, s.UserProvidedOrgValue);
			Assert.AreEqual(200, s.UserProvidedEndValue);

			s.SetUserParameters(BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, -800, BoundaryRescaling.LessOrEqual, BoundariesRelativeTo.RelativeToDataBoundsEnd, -200);
			s.OnDataBoundsChanged(1000, 2000);
			s.OnUserZoomed(-2000, -1000);

			// Expected when rescaled by user: use data bounds, and take into account user provided values
			s.OnUserRescaled();
			Assert.AreEqual(1000, s.ResultingOrg);
			Assert.AreEqual(1800, s.ResultingEnd);
			Assert.AreEqual(false, s.IsResultingOrgFixed);
			Assert.AreEqual(true, s.IsResultingEndFixed);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.OrgRescaling);
			Assert.AreEqual(BoundaryRescaling.LessOrEqual, s.EndRescaling);
			Assert.AreEqual(-800, s.UserProvidedOrgValue);
			Assert.AreEqual(-200, s.UserProvidedEndValue);
		}

		#endregion Tests for RelativeTo = RelativeToEnd
	}
}