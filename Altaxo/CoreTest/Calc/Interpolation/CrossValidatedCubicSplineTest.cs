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
using Altaxo.Calc.Interpolation;
using Altaxo.Calc.LinearAlgebra;
using NUnit.Framework;

namespace AltaxoTest.Calc.Interpolation
{
	/// <summary>
	/// Test for the bivariate spline from Akima.
	/// </summary>
	/// <remarks> The test data originates from:
	/// <para>Hiroshi Akima</para>
	/// <para>"Algorithm 474: Bivariate Interpolation and smooth surface fitting based on local procedures"</para>
	/// <para>Communications of the ACM, vol.17 (Jan.1974), Number 1</para>
	/// </remarks>
	[TestFixture]
	public class TestCrossValidatedCubicSpline
	{
		double[][] _testvals = {
new double[]{0.00843301352350958, -0.152243837374951},
new double[]{0.025019470131189, -0.119734055655511},
new double[]{0.0501815571171146, 0.461729355636735},
new double[]{0.0694124844388163, 0.0851220476980274},
new double[]{0.0892763556691863, 0.180380168471979},
new double[]{0.105400639177331, 0.389756960468003},
new double[]{0.13304193020319, 0.757836858329517},
new double[]{0.144587330384143, 0.704785058099651},
new double[]{0.166677919882355, 0.793994884037036},
new double[]{0.192998934131891, 1.01678524670068},
new double[]{0.214620181867521, 0.906192511335025},
new double[]{0.228451359764883, 0.665808010311522},
new double[]{0.251834099563693, 1.04051259747373},
new double[]{0.268998843539661, 0.742630635194807},
new double[]{0.290879450890956, 1.1810765143109},
new double[]{0.30715736285651, 1.02738155522738},
new double[]{0.333202886269367, 1.0417072436816},
new double[]{0.349254425425356, 0.721461550776648},
new double[]{0.367263622515492, 0.830680249332228},
new double[]{0.395793876672378, 1.17334020997532},
new double[]{0.413910668584166, 1.12441984632209},
new double[]{0.43034162488161, 0.920228278925585},
new double[]{0.450030182820448, 1.14215552834455},
new double[]{0.466854705948781, 0.788935908970938},
new double[]{0.496360513884432, 0.542289221010453},
new double[]{0.503539552750783, 0.488133260181762},
new double[]{0.523896284687285, 0.907649946408259},
new double[]{0.556218648496807, 0.668149586712674},
new double[]{0.573949241518951, 0.713550473948648},
new double[]{0.589557848376947, 0.550237275721738},
new double[]{0.609388991802025, 0.151980110647414},
new double[]{0.630538900131611, 0.235846260250003},
new double[]{0.654442123906272, 0.115629584585516},
new double[]{0.671535796514496, -0.227757721400102},
new double[]{0.687882554148269, -0.367649455893867},
new double[]{0.707735100750688, -0.485247286448983},
new double[]{0.732802827163973, -0.0880249669597303},
new double[]{0.744480400706865, -0.646258600064633},
new double[]{0.764294638162306, -0.236593751557764},
new double[]{0.791869410957149, -0.463411501001707},
new double[]{0.808997759337381, -0.347033662234902},
new double[]{0.82560029086452, -0.764451069370005},
new double[]{0.85426094111564, -0.994121364304904},
new double[]{0.870214382736112, -0.659071940610107},
new double[]{0.891374850047151, -0.776223930086174},
new double[]{0.906590191208101, -1.01820147789573},
new double[]{0.933629603868799, -1.18556910376756},
new double[]{0.948752005481511, -0.980323659089009},
new double[]{0.967142143027768, -0.739075684559176},
new double[]{0.992733618149565, -0.756103993847516}
													 };
		// expected y at the x given by testvals
		double[] _refy = {
-0.131090596708801,
-0.025155914936931,
0.130926497660281,
0.241280844329335,
0.356777980825182,
0.454968188797009,
0.619212654824897,
0.679389857839982,
0.774522552179897,
0.851581548749066,
0.887872249139432,
0.904753661837447,
0.932232338574859,
0.950458645325399,
0.970376274499151,
0.977149934487782,
0.976968616138686,
0.977138916832685,
0.984496521419732,
0.998469765185854,
0.990577529585417,
0.966552438538821,
0.919057332471586,
0.865723252921441,
0.772068419648475,
0.753289417541231,
0.706807532356554,
0.611634345283875,
0.533778296253205,
0.446546874234237,
0.316797875562519,
0.169019670834313,
0.000604466751715702,
-0.112043082377132,
-0.203892451883197,
-0.288035351692333,
-0.359437869674954,
-0.387221118156361,
-0.432846107110799,
-0.513327897195207,
-0.57771910267756,
-0.64883122966336,
-0.767658085018699,
-0.82295140017917,
-0.884530305811163,
-0.91766943370851,
-0.938292116897269,
-0.924731111232022,
-0.890753030266603,
-0.831558418471905
};

		[Test]
		public void Test1()
		{
			int len = _testvals.Length;

			double[] x = new double[len];
			double[] y = new double[len];
			double[] dy = new double[len];


			for (int i = 0; i < len; i++)
			{
				x[i] = _testvals[i][0];
				y[i] = _testvals[i][1];
				dy[i] = 1;
			}


			CrossValidatedCubicSpline spline = new CrossValidatedCubicSpline();

			spline.SetErrorVariance(VectorMath.ToROVector(dy), -1);
			spline.Interpolate(VectorMath.ToROVector(x), VectorMath.ToROVector(y));

			for (int i = 0; i < len; i++)
			{
				Assert.AreEqual(_refy[i], spline.Coefficient0[i], 1e-7);
			}

		}
	}
}
