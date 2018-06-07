﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Science.Thermodynamics.Fluids
{
	[TestFixture]
	internal class Test_Carbondioxide
	{
		// TestData contains:
		// 0. Pressure (Pa)
		// 1. Temperature (Kelvin)
		// 2. Density
		// 3. Internal energy (kJ/kg)
		// 4. Enthalpy (kJ/kg)
		// 5. Entropy (kJ/kg)
		// 6. Isochoric heat capacity (kJ/(kg K))
		// 7. Isobaric heat capacity (kJ/(kg K))
		// 8. Speed of sound (m/s)

		private double[][] _testData = new double[][]{
new double[]{100000, 194.525, 2.7796, -120.24, -84.267, -0.34184, 0.56013, 0.76998, 219.98, },
new double[]{100000, 200, 2.698, -117.11, -80.049, -0.32046, 0.56339, 0.77091, 223, },
new double[]{100000, 210, 2.5617, -111.36, -72.323, -0.28276, 0.57062, 0.77476, 228.33, },
new double[]{100000, 220, 2.4394, -105.54, -64.547, -0.24659, 0.57907, 0.78067, 233.45, },
new double[]{100000, 230, 2.3288, -99.645, -56.705, -0.21173, 0.58833, 0.78795, 238.38, },
new double[]{100000, 240, 2.2282, -93.664, -48.785, -0.17803, 0.59811, 0.79618, 243.15, },
new double[]{100000, 250, 2.1363, -87.589, -40.78, -0.14535, 0.60818, 0.80501, 247.79, },
new double[]{100000, 260, 2.0519, -81.419, -32.684, -0.1136, 0.61843, 0.81424, 252.31, },
new double[]{100000, 270, 1.9741, -75.151, -24.494, -0.08269, 0.62872, 0.82371, 256.72, },
new double[]{100000, 280, 1.9021, -68.784, -16.209, -0.05256, 0.639, 0.8333, 261.03, },
new double[]{100000, 290, 1.8352, -62.317, -7.8279, -0.02315, 0.64922, 0.84293, 265.25, },
new double[]{100000, 300, 1.773, -55.751, 0.64941, 0.00559, 0.65932, 0.85253, 269.39, },
new double[]{100000, 325, 1.6348, -38.911, 22.26, 0.07477, 0.68392, 0.87619, 279.42, },
new double[]{100000, 350, 1.5167, -21.479, 44.452, 0.14054, 0.70743, 0.89903, 289.04, },
new double[]{100000, 375, 1.4147, -3.4815, 67.203, 0.20332, 0.72978, 0.9209, 298.31, },
new double[]{100000, 400, 1.3257, 15.056, 90.488, 0.26342, 0.75099, 0.94173, 307.28, },
new double[]{100000, 425, 1.2472, 34.105, 114.28, 0.32111, 0.77109, 0.96156, 315.97, },
new double[]{100000, 450, 1.1776, 53.64, 138.56, 0.37661, 0.79017, 0.98041, 324.41, },
new double[]{100000, 475, 1.1154, 73.639, 163.29, 0.43011, 0.80829, 0.99835, 332.62, },
new double[]{100000, 500, 1.0594, 94.076, 188.47, 0.48175, 0.82552, 1.0154, 340.62, },
new double[]{100000, 525, 1.0088, 114.93, 214.06, 0.53169, 0.84192, 1.0317, 348.43, },
new double[]{100000, 550, 0.96283, 136.19, 240.05, 0.58005, 0.85755, 1.0473, 356.06, },
new double[]{100000, 575, 0.92087, 157.82, 266.42, 0.62693, 0.87245, 1.0621, 363.52, },
new double[]{100000, 600, 0.88242, 179.82, 293.15, 0.67243, 0.88668, 1.0762, 370.83, },
new double[]{100000, 700, 0.75619, 271.13, 403.38, 0.84226, 0.93752, 1.1269, 398.68, },
new double[]{100000, 800, 0.66158, 367.09, 518.24, 0.99558, 0.97997, 1.1692, 424.68, },
new double[]{100000, 900, 0.58803, 466.93, 636.99, 1.1354, 1.0155, 1.2046, 449.19, },
new double[]{100000, 1000, 0.52921, 570.02, 758.98, 1.2639, 1.0452, 1.2343, 472.43, },
new double[]{100000, 1100, 0.48109, 675.83, 883.69, 1.3827, 1.0702, 1.2593, 494.61, },
new double[]{1000000, 216.695, 1179.1, -427.26, -426.41, -2.218, 0.97514, 1.9503, 977.76, },
new double[]{1000000, 220, 1167.03, -420.8, -419.95, -2.1884, 0.97034, 1.9589, 953.55, },
new double[]{1000000, 225, 1148.32, -410.99, -410.11, -2.1442, 0.96337, 1.9751, 916.83, },
new double[]{1000000, 230, 1128.97, -401.08, -400.19, -2.1006, 0.9568, 1.9959, 879.82, },
new double[]{1000000, 233.0282, 1116.9, -395.02, -394.12, -2.0744, 0.95303, 2.0111, 857.18, },
new double[]{1000000, 233.0282, 26.006, -109.94, -71.484, -0.68986, 0.68026, 1.0322, 223.5, },
new double[]{1000000, 235, 25.665, -108.42, -69.459, -0.6812, 0.67819, 1.022, 224.93, },
new double[]{1000000, 240, 24.857, -104.64, -64.408, -0.65993, 0.67332, 0.99915, 228.46, },
new double[]{1000000, 245, 24.117, -100.92, -59.46, -0.63953, 0.66959, 0.98058, 231.84, },
new double[]{1000000, 250, 23.435, -97.266, -54.595, -0.61987, 0.66716, 0.96579, 235.08, },
new double[]{1000000, 255, 22.803, -93.65, -49.797, -0.60087, 0.66588, 0.95411, 238.19, },
new double[]{1000000, 260, 22.215, -90.065, -45.05, -0.58243, 0.66557, 0.94495, 241.19, },
new double[]{1000000, 265, 21.664, -86.503, -40.344, -0.5645, 0.66605, 0.93783, 244.1, },
new double[]{1000000, 270, 21.147, -82.957, -35.669, -0.54703, 0.66718, 0.93235, 246.91, },
new double[]{1000000, 275, 20.66, -79.422, -31.018, -0.52996, 0.66884, 0.92821, 249.66, },
new double[]{1000000, 280, 20.199, -75.892, -26.385, -0.51326, 0.67092, 0.92518, 252.33, },
new double[]{1000000, 285, 19.763, -72.364, -21.765, -0.49691, 0.67335, 0.92307, 254.93, },
new double[]{1000000, 290, 19.349, -68.836, -17.153, -0.48087, 0.67607, 0.92172, 257.49, },
new double[]{1000000, 295, 18.955, -65.303, -12.547, -0.46512, 0.67902, 0.92103, 259.98, },
new double[]{1000000, 300, 18.579, -61.765, -7.942, -0.44964, 0.68217, 0.92089, 262.43, },
new double[]{1000000, 305, 18.221, -58.22, -3.337, -0.43442, 0.68547, 0.92121, 264.83, },
new double[]{1000000, 310, 17.878, -54.665, 1.2707, -0.41943, 0.6889, 0.92192, 267.2, },
new double[]{1000000, 315, 17.549, -51.1, 5.8828, -0.40467, 0.69243, 0.92298, 269.52, },
new double[]{1000000, 320, 17.234, -47.523, 10.501, -0.39013, 0.69605, 0.92433, 271.8, },
new double[]{1000000, 325, 16.932, -43.934, 15.126, -0.37578, 0.69974, 0.92594, 274.06, },
new double[]{1000000, 330, 16.641, -40.331, 19.761, -0.36163, 0.70349, 0.92777, 276.28, },
new double[]{1000000, 335, 16.361, -36.715, 24.405, -0.34767, 0.70729, 0.92981, 278.46, },
new double[]{1000000, 340, 16.092, -33.084, 29.059, -0.33387, 0.71112, 0.932, 280.62, },
new double[]{1000000, 345, 15.832, -29.438, 33.725, -0.32025, 0.71498, 0.93435, 282.76, },
new double[]{1000000, 350, 15.581, -25.777, 38.403, -0.30679, 0.71885, 0.93681, 284.86, },
new double[]{1000000, 360, 15.105, -18.406, 47.797, -0.28033, 0.72664, 0.94206, 289, },
new double[]{1000000, 370, 14.659, -10.97, 57.245, -0.25444, 0.73442, 0.94763, 293.05, },
new double[]{1000000, 380, 14.241, -3.4677, 66.75, -0.22909, 0.74217, 0.95345, 297.02, },
new double[]{1000000, 390, 13.848, 4.1027, 76.315, -0.20425, 0.74988, 0.95944, 300.91, },
new double[]{1000000, 400, 13.477, 11.741, 85.939, -0.17988, 0.75751, 0.96555, 304.72, },
new double[]{1000000, 410, 13.127, 19.449, 95.626, -0.15596, 0.76505, 0.97174, 308.47, },
new double[]{1000000, 420, 12.796, 27.224, 105.37, -0.13247, 0.77251, 0.97798, 312.16, },
new double[]{1000000, 430, 12.482, 35.068, 115.19, -0.10939, 0.77986, 0.98423, 315.79, },
new double[]{1000000, 440, 12.183, 42.979, 125.06, -0.08669, 0.7871, 0.99049, 319.37, },
new double[]{1000000, 450, 11.899, 50.957, 135, -0.06436, 0.79424, 0.99673, 322.89, },
new double[]{1000000, 460, 11.629, 59.001, 144.99, -0.04238, 0.80126, 1.0029, 326.37, },
new double[]{1000000, 470, 11.371, 67.112, 155.05, -0.02075, 0.80817, 1.0091, 329.79, },
new double[]{1000000, 480, 11.125, 75.287, 165.18, 0.00056, 0.81497, 1.0152, 333.18, },
new double[]{1000000, 490, 10.889, 83.526, 175.36, 0.02156, 0.82166, 1.0213, 336.52, },
new double[]{1000000, 500, 10.664, 91.829, 185.6, 0.04225, 0.82823, 1.0273, 339.81, },
new double[]{1000000, 525, 10.141, 112.86, 211.47, 0.09273, 0.84418, 1.042, 347.9, },
new double[]{1000000, 550, 9.6675, 134.26, 237.7, 0.14154, 0.85946, 1.0563, 355.76, },
new double[]{1000000, 575, 9.2375, 156.02, 264.28, 0.18879, 0.87408, 1.07, 363.42, },
new double[]{1000000, 600, 8.8449, 178.14, 291.19, 0.23462, 0.88808, 1.0833, 370.9, },
new double[]{1000000, 625, 8.4849, 200.58, 318.44, 0.2791, 0.90149, 1.0961, 378.2, },
new double[]{1000000, 650, 8.1535, 223.35, 345.99, 0.32233, 0.91432, 1.1084, 385.36, },
new double[]{1000000, 675, 7.8474, 246.42, 373.85, 0.36438, 0.9266, 1.1202, 392.37, },
new double[]{1000000, 700, 7.5638, 269.79, 402, 0.40533, 0.93835, 1.1315, 399.24, },
new double[]{1000000, 800, 6.6102, 365.98, 517.26, 0.55918, 0.98053, 1.1725, 425.54, },
new double[]{1000000, 900, 5.8718, 465.99, 636.29, 0.69934, 1.0159, 1.2071, 450.22, },
new double[]{1000000, 1000, 5.2826, 569.2, 758.5, 0.82807, 1.0455, 1.2362, 473.59, },
new double[]{1000000, 1100, 4.8014, 675.12, 883.39, 0.94708, 1.0704, 1.2608, 495.84, },
new double[]{101325, 194.685, 2.8147, -120.18, -84.18, -0.34383, 0.56049, 0.77056, 220.03, },
new double[]{101325, 200, 2.7345, -117.14, -80.083, -0.32307, 0.56362, 0.77141, 222.97, },
new double[]{101325, 210, 2.5963, -111.38, -72.352, -0.28535, 0.5708, 0.77516, 228.3, },
new double[]{101325, 220, 2.4722, -105.56, -64.573, -0.24916, 0.57921, 0.78098, 233.42, },
new double[]{101325, 230, 2.3601, -99.661, -56.728, -0.21429, 0.58845, 0.78821, 238.36, },
new double[]{101325, 240, 2.2581, -93.678, -48.806, -0.18057, 0.5982, 0.79639, 243.14, },
new double[]{101325, 250, 2.1649, -87.602, -40.798, -0.14789, 0.60826, 0.80519, 247.78, },
new double[]{101325, 260, 2.0793, -81.431, -32.701, -0.11613, 0.61849, 0.8144, 252.3, },
new double[]{101325, 270, 2.0004, -75.162, -24.509, -0.08522, 0.62878, 0.82384, 256.71, },
new double[]{101325, 280, 1.9274, -68.793, -16.223, -0.05508, 0.63905, 0.83341, 261.02, },
new double[]{101325, 290, 1.8597, -62.326, -7.841, -0.02567, 0.64925, 0.84303, 265.24, },
new double[]{101325, 300, 1.7966, -55.76, 0.63726, 0.00307, 0.65935, 0.85262, 269.38, },
new double[]{101325, 325, 1.6565, -38.918, 22.25, 0.07226, 0.68394, 0.87625, 279.41, },
new double[]{101325, 350, 1.5369, -21.486, 44.443, 0.13803, 0.70745, 0.89908, 289.03, },
new double[]{101325, 375, 1.4335, -3.4869, 67.196, 0.20082, 0.7298, 0.92094, 298.31, },
new double[]{101325, 400, 1.3433, 15.051, 90.482, 0.26092, 0.751, 0.94177, 307.28, },
new double[]{101325, 425, 1.2638, 34.1, 114.28, 0.31862, 0.7711, 0.96159, 315.97, },
new double[]{101325, 450, 1.1932, 53.637, 138.55, 0.37412, 0.79018, 0.98044, 324.41, },
new double[]{101325, 475, 1.1302, 73.635, 163.29, 0.42761, 0.80829, 0.99837, 332.62, },
new double[]{101325, 500, 1.0735, 94.073, 188.46, 0.47926, 0.82552, 1.0155, 340.62, },
new double[]{101325, 525, 1.0222, 114.93, 214.06, 0.5292, 0.84192, 1.0317, 348.43, },
new double[]{101325, 550, 0.97559, 136.18, 240.04, 0.57756, 0.85755, 1.0473, 356.06, },
new double[]{101325, 575, 0.93308, 157.82, 266.41, 0.62444, 0.87246, 1.0621, 363.52, },
new double[]{101325, 600, 0.89412, 179.82, 293.14, 0.66994, 0.88668, 1.0762, 370.83, },
new double[]{101325, 700, 0.76621, 271.13, 403.38, 0.83977, 0.93752, 1.1269, 398.68, },
new double[]{101325, 800, 0.67035, 367.09, 518.24, 0.99309, 0.97997, 1.1692, 424.69, },
new double[]{101325, 900, 0.59582, 466.93, 636.99, 1.1329, 1.0155, 1.2046, 449.19, },
new double[]{101325, 1000, 0.53622, 570.02, 758.98, 1.2614, 1.0452, 1.2343, 472.44, },
new double[]{101325, 1100, 0.48746, 675.83, 883.69, 1.3803, 1.0702, 1.2593, 494.61, },
new double[]{15000000, 219.644, 1196.11, -428.91, -416.37, -2.226, 0.98604, 1.8799, 1030.9, },
new double[]{15000000, 220, 1194.96, -428.25, -415.7, -2.223, 0.98548, 1.8801, 1028.7, },
new double[]{15000000, 225, 1178.64, -419.02, -406.29, -2.1807, 0.97789, 1.8841, 997.14, },
new double[]{15000000, 230, 1162.02, -409.76, -396.86, -2.1392, 0.9707, 1.89, 965.81, },
new double[]{15000000, 235, 1145.06, -400.49, -387.39, -2.0985, 0.96394, 1.898, 934.61, },
new double[]{15000000, 240, 1127.73, -391.17, -377.87, -2.0584, 0.9576, 1.9081, 903.5, },
new double[]{15000000, 245, 1109.98, -381.82, -368.3, -2.019, 0.95169, 1.9206, 872.4, },
new double[]{15000000, 250, 1091.77, -372.4, -358.66, -1.98, 0.94623, 1.9358, 841.27, },
new double[]{15000000, 255, 1073.03, -362.92, -348.94, -1.9415, 0.94121, 1.9539, 810.04, },
new double[]{15000000, 260, 1053.71, -353.35, -339.12, -1.9034, 0.93667, 1.9752, 778.68, },
new double[]{15000000, 265, 1033.73, -343.69, -329.18, -1.8655, 0.93264, 2.0002, 747.13, },
new double[]{15000000, 270, 1013.01, -333.92, -319.11, -1.8279, 0.92914, 2.0294, 715.36, },
new double[]{15000000, 275, 991.45, -324.01, -308.88, -1.7903, 0.92619, 2.0635, 683.32, },
new double[]{15000000, 280, 968.93, -313.95, -298.47, -1.7528, 0.92381, 2.1034, 650.96, },
new double[]{15000000, 285, 945.3, -303.7, -287.83, -1.7152, 0.92211, 2.1501, 618.23, },
new double[]{15000000, 290, 920.4, -293.25, -276.95, -1.6773, 0.92132, 2.2055, 585.07, },
new double[]{15000000, 295, 894, -282.54, -265.76, -1.639, 0.92183, 2.2727, 551.45, },
new double[]{15000000, 300, 865.82, -271.52, -254.2, -1.6002, 0.9238, 2.3557, 517.4, },
new double[]{15000000, 305, 835.48, -260.12, -242.17, -1.5604, 0.92683, 2.4583, 483.07, },
new double[]{15000000, 310, 802.54, -248.27, -229.58, -1.5195, 0.93036, 2.583, 448.77, },
new double[]{15000000, 315, 766.51, -235.86, -216.3, -1.477, 0.93449, 2.7343, 414.97, },
new double[]{15000000, 320, 726.83, -222.81, -202.18, -1.4325, 0.93978, 2.9188, 382.22, },
new double[]{15000000, 325, 683.09, -209.02, -187.06, -1.3857, 0.94662, 3.128, 351.34, },
new double[]{15000000, 330, 635.51, -194.51, -170.91, -1.3363, 0.95475, 3.3309, 323.89, },
new double[]{15000000, 335, 585.4, -179.47, -153.85, -1.285, 0.96004, 3.4748, 301.72, },
new double[]{15000000, 340, 535.55, -164.42, -136.41, -1.2333, 0.95711, 3.4738, 286.11, },
new double[]{15000000, 345, 489.42, -150.03, -119.38, -1.1836, 0.94688, 3.3164, 276.63, },
new double[]{15000000, 350, 449.2, -136.79, -103.39, -1.1376, 0.93439, 3.0688, 271.76, },
new double[]{15000000, 360, 387.08, -114.04, -75.292, -1.0584, 0.91214, 2.5672, 270.06, },
new double[]{15000000, 370, 343.43, -95.274, -51.596, -0.99347, 0.89468, 2.1949, 273.34, },
new double[]{15000000, 380, 311.48, -79.159, -31.001, -0.93853, 0.88143, 1.9402, 278.46, },
new double[]{15000000, 390, 286.97, -64.804, -12.533, -0.89055, 0.87178, 1.7638, 284.22, },
new double[]{15000000, 400, 267.42, -51.652, 4.4406, -0.84757, 0.86502, 1.6376, 290.12, },
new double[]{15000000, 410, 251.33, -39.353, 20.329, -0.80833, 0.86053, 1.5445, 295.97, },
new double[]{15000000, 420, 237.78, -27.678, 35.405, -0.772, 0.85782, 1.4739, 301.68, },
new double[]{15000000, 430, 226.14, -16.471, 49.86, -0.73798, 0.8565, 1.4192, 307.22, },
new double[]{15000000, 440, 215.98, -5.6217, 63.828, -0.70587, 0.8563, 1.3761, 312.59, },
new double[]{15000000, 450, 207.01, 4.9509, 77.41, -0.67534, 0.85698, 1.3416, 317.79, },
new double[]{15000000, 460, 199, 15.307, 90.682, -0.64617, 0.85839, 1.3138, 322.83, },
new double[]{15000000, 470, 191.79, 25.492, 103.7, -0.61817, 0.86037, 1.2911, 327.7, },
new double[]{15000000, 480, 185.24, 35.541, 116.52, -0.59119, 0.86283, 1.2725, 332.43, },
new double[]{15000000, 490, 179.26, 45.484, 129.16, -0.56511, 0.86568, 1.2572, 337.03, },
new double[]{15000000, 500, 173.76, 55.342, 141.67, -0.53985, 0.86884, 1.2445, 341.5, },
new double[]{15000000, 525, 161.74, 79.733, 172.48, -0.47972, 0.87776, 1.2218, 352.18, },
new double[]{15000000, 550, 151.63, 103.91, 202.83, -0.42323, 0.88763, 1.208, 362.24, },
new double[]{15000000, 575, 142.97, 128.01, 232.93, -0.36972, 0.89803, 1.2002, 371.77, },
new double[]{15000000, 600, 135.43, 152.12, 262.88, -0.31873, 0.90867, 1.1963, 380.86, },
new double[]{15000000, 625, 128.78, 176.29, 292.77, -0.26992, 0.91936, 1.1953, 389.55, },
new double[]{15000000, 650, 122.86, 200.57, 322.66, -0.22303, 0.92998, 1.1963, 397.91, },
new double[]{15000000, 675, 117.54, 224.98, 352.59, -0.17784, 0.94044, 1.1987, 405.96, },
new double[]{15000000, 700, 112.72, 249.53, 382.6, -0.13418, 0.95067, 1.2022, 413.74, },
new double[]{15000000, 800, 97.199, 349.41, 503.73, 0.02753, 0.98876, 1.2213, 442.71, },
new double[]{15000000, 900, 85.74, 452, 626.95, 0.17264, 1.0218, 1.243, 469.01, },
new double[]{15000000, 1000, 76.856, 557.14, 752.31, 0.3047, 1.0501, 1.2639, 493.37, },
new double[]{15000000, 1100, 69.73, 664.55, 879.66, 0.42606, 1.0741, 1.2828, 516.21, },
new double[]{200000, 203.314, 5.4054, -116.97, -79.973, -0.4475, 0.5821, 0.80842, 222.32, },
new double[]{200000, 210, 5.2116, -112.95, -74.577, -0.42138, 0.58456, 0.80597, 226.1, },
new double[]{200000, 220, 4.9495, -106.93, -66.522, -0.38391, 0.58993, 0.80556, 231.52, },
new double[]{200000, 230, 4.7152, -100.87, -58.456, -0.34806, 0.59692, 0.80819, 236.7, },
new double[]{200000, 240, 4.5039, -94.758, -50.352, -0.31357, 0.60502, 0.81291, 241.68, },
new double[]{200000, 250, 4.312, -88.575, -42.193, -0.28026, 0.61383, 0.81906, 246.49, },
new double[]{200000, 260, 4.1369, -82.313, -33.967, -0.248, 0.6231, 0.82618, 251.16, },
new double[]{200000, 270, 3.9761, -75.967, -25.667, -0.21668, 0.63264, 0.83396, 255.69, },
new double[]{200000, 280, 3.828, -69.534, -17.286, -0.1862, 0.64231, 0.84219, 260.1, },
new double[]{200000, 290, 3.6909, -63.01, -8.8222, -0.1565, 0.65203, 0.8507, 264.42, },
new double[]{200000, 300, 3.5636, -56.394, -0.27188, -0.12751, 0.66174, 0.85938, 268.64, },
new double[]{200000, 325, 3.2819, -39.454, 21.487, -0.05786, 0.68562, 0.88131, 278.83, },
new double[]{200000, 350, 3.0423, -21.947, 43.792, 0.00825, 0.70867, 0.903, 288.58, },
new double[]{200000, 375, 2.836, -3.8914, 66.631, 0.07127, 0.73072, 0.92405, 297.95, },
new double[]{200000, 400, 2.6562, 14.692, 89.987, 0.13156, 0.7517, 0.94429, 306.99, },
new double[]{200000, 425, 2.4981, 33.778, 113.84, 0.18939, 0.77166, 0.96367, 315.74, },
new double[]{200000, 450, 2.358, 53.344, 138.16, 0.245, 0.79062, 0.98219, 324.23, },
new double[]{200000, 475, 2.2328, 73.368, 162.94, 0.29858, 0.80865, 0.99986, 332.49, },
new double[]{200000, 500, 2.1204, 93.827, 188.15, 0.3503, 0.82582, 1.0167, 340.53, },
new double[]{200000, 525, 2.0188, 114.7, 213.77, 0.4003, 0.84217, 1.0329, 348.37, },
new double[]{200000, 550, 1.9265, 135.97, 239.79, 0.44871, 0.85776, 1.0482, 356.02, },
new double[]{200000, 575, 1.8424, 157.62, 266.18, 0.49563, 0.87263, 1.063, 363.51, },
new double[]{200000, 600, 1.7653, 179.63, 292.93, 0.54117, 0.88684, 1.077, 370.83, },
new double[]{200000, 700, 1.5124, 270.99, 403.22, 0.71109, 0.93761, 1.1274, 398.74, },
new double[]{200000, 800, 1.323, 366.97, 518.13, 0.86447, 0.98003, 1.1696, 424.78, },
new double[]{200000, 900, 1.1759, 466.82, 636.91, 1.0043, 1.0155, 1.2049, 449.3, },
new double[]{200000, 1000, 1.0582, 569.93, 758.93, 1.1329, 1.0452, 1.2345, 472.56, },
new double[]{200000, 1100, 0.96197, 675.75, 883.66, 1.2517, 1.0702, 1.2594, 494.75, },
new double[]{4000000, 217.3339, 1182.98, -427.67, -424.29, -2.22, 0.97796, 1.933, 989.53, },
new double[]{4000000, 220, 1173.53, -422.54, -419.13, -2.1964, 0.97402, 1.9384, 970.66, },
new double[]{4000000, 225, 1155.43, -412.87, -409.41, -2.1527, 0.96691, 1.951, 935.29, },
new double[]{4000000, 230, 1136.8, -403.13, -399.62, -2.1096, 0.9602, 1.9673, 899.79, },
new double[]{4000000, 235, 1117.56, -393.31, -389.73, -2.0671, 0.95391, 1.9878, 864.02, },
new double[]{4000000, 240, 1097.6, -383.37, -379.73, -2.025, 0.9481, 2.0134, 827.8, },
new double[]{4000000, 245, 1076.81, -373.3, -369.59, -1.9832, 0.94281, 2.0452, 790.92, },
new double[]{4000000, 250, 1055.01, -363.06, -359.26, -1.9415, 0.93814, 2.0849, 753.1, },
new double[]{4000000, 255, 1032, -352.6, -348.72, -1.8997, 0.93435, 2.1348, 713.89, },
new double[]{4000000, 260, 1007.49, -341.86, -337.89, -1.8577, 0.93199, 2.1986, 672.67, },
new double[]{4000000, 265, 981.06, -330.78, -326.7, -1.815, 0.93201, 2.2823, 628.62, },
new double[]{4000000, 270, 952.1, -319.22, -315.02, -1.7714, 0.93573, 2.3972, 580.72, },
new double[]{4000000, 275, 919.56, -306.99, -302.64, -1.7259, 0.94504, 2.566, 527.52, },
new double[]{4000000, 278.4497, 894.05, -297.98, -293.51, -1.6929, 0.95655, 2.7401, 486.42, },
new double[]{4000000, 278.4497, 115.74, -114.09, -79.534, -0.92449, 0.91069, 2.1642, 208.78, },
new double[]{4000000, 280, 113.08, -111.66, -76.288, -0.91286, 0.89131, 2.0294, 211.36, },
new double[]{4000000, 285, 106.02, -104.65, -66.92, -0.87969, 0.84827, 1.7461, 218.6, },
new double[]{4000000, 290, 100.47, -98.453, -58.641, -0.85089, 0.82129, 1.578, 224.72, },
new double[]{4000000, 295, 95.884, -92.766, -51.048, -0.82493, 0.80299, 1.4657, 230.14, },
new double[]{4000000, 300, 91.965, -87.427, -43.932, -0.80101, 0.79011, 1.385, 235.04, },
new double[]{4000000, 305, 88.543, -82.34, -37.165, -0.77864, 0.78082, 1.3244, 239.56, },
new double[]{4000000, 310, 85.508, -77.445, -30.665, -0.7575, 0.77402, 1.2773, 243.77, },
new double[]{4000000, 315, 82.781, -72.697, -24.376, -0.73737, 0.76906, 1.2397, 247.73, },
new double[]{4000000, 320, 80.306, -68.066, -18.256, -0.71809, 0.76551, 1.2092, 251.49, },
new double[]{4000000, 325, 78.043, -63.529, -12.275, -0.69955, 0.76308, 1.1842, 255.07, },
new double[]{4000000, 330, 75.958, -59.068, -6.4078, -0.68163, 0.76156, 1.1633, 258.49, },
new double[]{4000000, 335, 74.028, -54.67, -0.63601, -0.66427, 0.76076, 1.1459, 261.78, },
new double[]{4000000, 340, 72.231, -50.322, 5.0556, -0.64741, 0.76054, 1.1312, 264.95, },
new double[]{4000000, 345, 70.552, -46.017, 10.679, -0.63099, 0.7608, 1.1187, 268.01, },
new double[]{4000000, 350, 68.976, -41.746, 16.245, -0.61497, 0.76145, 1.108, 270.98, },
new double[]{4000000, 360, 66.092, -33.284, 27.237, -0.584, 0.76371, 1.0912, 276.67, },
new double[]{4000000, 370, 63.509, -24.899, 38.084, -0.55428, 0.76695, 1.0789, 282.06, },
new double[]{4000000, 380, 61.173, -16.562, 48.826, -0.52564, 0.77091, 1.07, 287.22, },
new double[]{4000000, 390, 59.045, -8.2521, 59.493, -0.49793, 0.77542, 1.0637, 292.17, },
new double[]{4000000, 400, 57.094, 0.04711, 70.107, -0.47105, 0.78032, 1.0595, 296.93, },
new double[]{4000000, 410, 55.294, 8.3478, 80.688, -0.44493, 0.78553, 1.0569, 301.53, },
new double[]{4000000, 420, 53.627, 16.66, 91.249, -0.41948, 0.79097, 1.0555, 305.99, },
new double[]{4000000, 430, 52.076, 24.99, 101.8, -0.39465, 0.79657, 1.0552, 310.31, },
new double[]{4000000, 440, 50.628, 33.346, 112.35, -0.37039, 0.80228, 1.0556, 314.52, },
new double[]{4000000, 450, 49.27, 41.732, 122.92, -0.34665, 0.80808, 1.0568, 318.63, },
new double[]{4000000, 460, 47.995, 50.151, 133.49, -0.32341, 0.81391, 1.0585, 322.64, },
new double[]{4000000, 470, 46.794, 58.607, 144.09, -0.30062, 0.81978, 1.0607, 326.55, },
new double[]{4000000, 480, 45.659, 67.102, 154.71, -0.27826, 0.82564, 1.0633, 330.39, },
new double[]{4000000, 490, 44.585, 75.638, 165.35, -0.25631, 0.8315, 1.0661, 334.15, },
new double[]{4000000, 500, 43.566, 84.217, 176.03, -0.23474, 0.83733, 1.0692, 337.84, },
new double[]{4000000, 525, 41.233, 105.86, 202.87, -0.18237, 0.85173, 1.0779, 346.77, },
new double[]{4000000, 550, 39.16, 127.79, 229.93, -0.13201, 0.8658, 1.0873, 355.36, },
new double[]{4000000, 575, 37.302, 150, 257.24, -0.08346, 0.87948, 1.0971, 363.63, },
new double[]{4000000, 600, 35.625, 172.51, 284.79, -0.03655, 0.89272, 1.1072, 371.63, },
new double[]{4000000, 625, 34.103, 195.3, 312.6, 0.00885, 0.90551, 1.1174, 379.39, },
new double[]{4000000, 650, 32.712, 218.38, 340.66, 0.05287, 0.91784, 1.1274, 386.93, },
new double[]{4000000, 675, 31.437, 241.73, 368.97, 0.09561, 0.92971, 1.1374, 394.28, },
new double[]{4000000, 700, 30.262, 265.35, 397.52, 0.13715, 0.94111, 1.1471, 401.45, },
new double[]{4000000, 800, 26.355, 362.32, 514.09, 0.29274, 0.98236, 1.1835, 428.62, },
new double[]{4000000, 900, 23.367, 462.88, 634.07, 0.43402, 1.0172, 1.2153, 453.84, },
new double[]{4000000, 1000, 21.001, 566.52, 756.99, 0.5635, 1.0465, 1.2425, 477.55, },
new double[]{4000000, 1100, 19.077, 672.77, 882.44, 0.68306, 1.0712, 1.2658, 500.01, },
new double[]{50000, 186.436, 1.437, -123.74, -88.944, -0.23757, 0.54404, 0.74495, 216.936, },
new double[]{50000, 190, 1.4089, -121.78, -86.286, -0.22345, 0.54661, 0.7466, 218.9, },
new double[]{50000, 200, 1.3359, -116.22, -78.792, -0.18501, 0.55478, 0.75266, 224.26, },
new double[]{50000, 210, 1.2704, -110.59, -71.228, -0.14811, 0.56398, 0.76029, 229.41, },
new double[]{50000, 220, 1.2112, -104.86, -63.582, -0.11254, 0.57386, 0.76897, 234.38, },
new double[]{50000, 230, 1.1575, -99.044, -55.846, -0.07816, 0.58417, 0.77834, 239.2, },
new double[]{50000, 240, 1.1084, -93.125, -48.014, -0.04483, 0.59473, 0.78815, 243.88, },
new double[]{50000, 250, 1.0634, -87.104, -40.082, -0.01245, 0.60542, 0.79823, 248.44, },
new double[]{50000, 260, 1.0219, -80.978, -32.049, 0.01906, 0.61612, 0.80845, 252.88, },
new double[]{50000, 270, 0.9836, -74.747, -23.913, 0.04976, 0.62679, 0.81871, 257.23, },
new double[]{50000, 280, 0.9481, -68.412, -15.675, 0.07972, 0.63737, 0.82895, 261.49, },
new double[]{50000, 290, 0.9151, -61.973, -7.3344, 0.10899, 0.64782, 0.83912, 265.67, },
new double[]{50000, 300, 0.88434, -55.432, 1.1072, 0.13761, 0.65812, 0.84917, 269.77, },
new double[]{50000, 325, 0.81585, -38.641, 22.645, 0.20655, 0.68307, 0.87366, 279.71, },
new double[]{50000, 350, 0.75726, -21.246, 44.781, 0.27216, 0.70681, 0.89707, 289.27, },
new double[]{50000, 375, 0.70656, -3.2771, 67.489, 0.33481, 0.72932, 0.91933, 298.5, },
new double[]{50000, 400, 0.66224, 15.237, 90.738, 0.39483, 0.75063, 0.94046, 307.42, },
new double[]{50000, 425, 0.62317, 34.268, 114.5, 0.45245, 0.77081, 0.96051, 316.08, },
new double[]{50000, 450, 0.58847, 53.788, 138.76, 0.50789, 0.78995, 0.97953, 324.5, },
new double[]{50000, 475, 0.55743, 73.774, 163.47, 0.56134, 0.80811, 0.9976, 332.69, },
new double[]{50000, 500, 0.52951, 94.201, 188.63, 0.61295, 0.82537, 1.0148, 340.67, },
new double[]{50000, 525, 0.50426, 115.05, 214.2, 0.66286, 0.84179, 1.0312, 348.46, },
new double[]{50000, 550, 0.4813, 136.29, 240.18, 0.7112, 0.85744, 1.0468, 356.08, },
new double[]{50000, 575, 0.46035, 157.92, 266.54, 0.75806, 0.87236, 1.0616, 363.53, },
new double[]{50000, 600, 0.44115, 179.92, 293.26, 0.80354, 0.8866, 1.0758, 370.83, },
new double[]{50000, 700, 0.37809, 271.21, 403.45, 0.97331, 0.93747, 1.1266, 398.65, },
new double[]{50000, 800, 0.33081, 367.15, 518.3, 1.1266, 0.97994, 1.169, 424.64, },
new double[]{50000, 900, 0.29404, 466.98, 637.03, 1.2664, 1.0155, 1.2045, 449.13, },
new double[]{50000, 1000, 0.26463, 570.06, 759.01, 1.3949, 1.0452, 1.2342, 472.37, },
new double[]{50000, 1100, 0.24057, 675.87, 883.71, 1.5137, 1.0702, 1.2592, 494.54, },
new double[]{600000000, 305.996, 1448.66, -391.42, 22.754, -2.2128, 1.1462, 1.5467, 1910.7, },
new double[]{600000000, 310, 1444.64, -386.39, 28.939, -2.1927, 1.1442, 1.5427, 1903, },
new double[]{600000000, 315, 1439.67, -380.12, 36.64, -2.1681, 1.1419, 1.5377, 1893.5, },
new double[]{600000000, 320, 1434.75, -373.88, 44.317, -2.1439, 1.1397, 1.5329, 1884.2, },
new double[]{600000000, 325, 1429.88, -367.65, 51.97, -2.1201, 1.1376, 1.5283, 1875.1, },
new double[]{600000000, 330, 1425.06, -361.44, 59.6, -2.0969, 1.1357, 1.5238, 1866.1, },
new double[]{600000000, 335, 1420.28, -355.24, 67.208, -2.074, 1.1338, 1.5194, 1857.3, },
new double[]{600000000, 340, 1415.56, -349.07, 74.794, -2.0515, 1.1321, 1.5152, 1848.6, },
new double[]{600000000, 345, 1410.88, -342.91, 82.36, -2.0294, 1.1305, 1.5111, 1840.1, },
new double[]{600000000, 350, 1406.25, -336.76, 89.905, -2.0077, 1.129, 1.5071, 1831.7, },
new double[]{600000000, 360, 1397.12, -324.52, 104.94, -1.9653, 1.1263, 1.4996, 1815.4, },
new double[]{600000000, 370, 1388.16, -312.33, 119.9, -1.9243, 1.124, 1.4925, 1799.7, },
new double[]{600000000, 380, 1379.38, -300.19, 134.79, -1.8846, 1.1219, 1.4858, 1784.5, },
new double[]{600000000, 390, 1370.75, -288.1, 149.62, -1.8461, 1.1201, 1.4796, 1769.8, },
new double[]{600000000, 400, 1362.28, -276.05, 164.38, -1.8087, 1.1186, 1.4738, 1755.6, },
new double[]{600000000, 410, 1353.97, -264.05, 179.09, -1.7724, 1.1174, 1.4683, 1741.9, },
new double[]{600000000, 420, 1345.79, -252.08, 193.75, -1.7371, 1.1163, 1.4633, 1728.7, },
new double[]{600000000, 430, 1337.76, -240.15, 208.36, -1.7027, 1.1154, 1.4585, 1715.9, },
new double[]{600000000, 440, 1329.87, -228.25, 222.92, -1.6692, 1.1147, 1.4541, 1703.6, },
new double[]{600000000, 450, 1322.1, -216.38, 237.44, -1.6366, 1.1142, 1.45, 1691.6, },
new double[]{600000000, 460, 1314.46, -204.54, 251.92, -1.6048, 1.1139, 1.4462, 1680.1, },
new double[]{600000000, 470, 1306.94, -192.72, 266.37, -1.5737, 1.1136, 1.4427, 1668.9, },
new double[]{600000000, 480, 1299.54, -180.92, 280.78, -1.5434, 1.1135, 1.4394, 1658.1, },
new double[]{600000000, 490, 1292.26, -169.15, 295.16, -1.5137, 1.1135, 1.4363, 1647.7, },
new double[]{600000000, 500, 1285.08, -157.39, 309.5, -1.4847, 1.1137, 1.4335, 1637.5, },
new double[]{600000000, 525, 1267.6, -128.07, 345.26, -1.4149, 1.1144, 1.4275, 1613.7, },
new double[]{600000000, 550, 1250.74, -98.828, 380.89, -1.3487, 1.1157, 1.4226, 1591.6, },
new double[]{600000000, 575, 1234.45, -69.645, 416.4, -1.2855, 1.1174, 1.4187, 1571.3, },
new double[]{600000000, 600, 1218.69, -40.503, 451.83, -1.2252, 1.1195, 1.4157, 1552.5, },
new double[]{600000000, 625, 1203.43, -11.386, 487.19, -1.1675, 1.1218, 1.4134, 1535.1, },
new double[]{600000000, 650, 1188.63, 17.72, 522.5, -1.1121, 1.1243, 1.4117, 1519.1, },
new double[]{600000000, 675, 1174.27, 46.825, 557.78, -1.0588, 1.1271, 1.4106, 1504.2, },
new double[]{600000000, 700, 1160.32, 75.938, 593.04, -1.0075, 1.1299, 1.4099, 1490.5, },
new double[]{600000000, 800, 1108.23, 192.61, 734.02, -0.81926, 1.1419, 1.4103, 1445.3, },
new double[]{600000000, 900, 1061.27, 309.84, 875.2, -0.65298, 1.1538, 1.4134, 1412.4, },
new double[]{600000000, 1000, 1018.66, 427.73, 1016.74, -0.50386, 1.1649, 1.4175, 1388.8, },
new double[]{600000000, 1100, 979.76, 546.3, 1158.69, -0.36856, 1.1749, 1.4216, 1371.9, },
};

		[Test]
		public void TestAllData()
		{
			var material = Altaxo.Science.Thermodynamics.Fluids.CarbonDioxide.Instance;
			double pressureRelTolerance = 1E-3;

			var methods = new(string colName, Func<double, double, double> call, double scale, int index)[]
			{
								("InternalEnergy", material.InternalEnergy_FromDensityAndTemperature, 1E-3, 3),
								("Enthalpy", material.Enthalpy_FromDensityAndTemperature,1E-3,4),
								("Entropy", material.Entropy_FromDensityAndTemperature, 1E-3,5),
								("Cv", material.IsochoricHeatCapacity_FromDensityAndTemperature,1E-3, 6),
								("Cp", material.IsobaricHeatCapacity_FromDensityAndTemperature,1E-3, 7),
								("SpeedOfSound", material.SpeedOfSound_FromDensityAndTemperature, 1, 8),
		};

			for (int i = 0; i < _testData.Length; ++i)
			{
				var item = _testData[i];
				var pressure = item[0];
				var temperature = item[1];
				var density = item[2];

				Assert.IsFalse(!(temperature > 0 && temperature < 2000));
				Assert.IsFalse(!(density > 0 && density < 10000));

				var densityCalculated = material.Density_FromPressureAndTemperature(pressure, temperature, 1E-6, density);

				Assert.IsFalse(!IsDoubleValueMatch(density, densityCalculated), "Density deviation");

				density = densityCalculated;

				var pressureCalculated = material.Pressure_FromDensityAndTemperature(density, temperature);

				var pressureRelDev = Math.Abs((pressureCalculated - pressure) / pressure);
				Assert.IsFalse(pressureRelDev > pressureRelTolerance, "Pressure deviation, expected: {0}, but is: {1}", pressure, pressureCalculated);

				foreach (var (colName, call, scale, index) in methods)
				{
					double valueCalculated = scale * call(density, temperature);
					var valueStored = _testData[i][index];

					Assert.IsFalse(double.IsNaN(valueStored), "Row[{0}] : {1} defect", i, colName);

					Assert.IsFalse(!IsDoubleValueMatch(valueStored, valueCalculated), "Row[{0}]: {1} deviation, expected {2}, current {3}", i, colName, valueStored, valueCalculated);
				}
			}
		}

		private bool IsDoubleValueMatch(double expected, double calculated)
		{
			const int maxAccuracy = 6;
			int accuracy;
			for (accuracy = 0; accuracy <= maxAccuracy; ++accuracy)
				if (Math.Round(expected, accuracy) == Math.Round(expected, maxAccuracy))
					break;

			return Math.Round(expected, accuracy) == Math.Round(calculated, accuracy);
		}
	}
}