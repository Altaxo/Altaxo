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

namespace Altaxo.Science.Thermodynamics.Fluids
{

  /// <summary>
  /// Tests and test data for <see cref="Trichlorofluoromethane"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  [TestFixture]
  public class Test_Trichlorofluoromethane : FluidTestBase
  {

    public Test_Trichlorofluoromethane()
    {
      _fluid = Trichlorofluoromethane.Instance;

      _testDataMolecularWeight = 0.137368;

      _testDataTriplePointTemperature = 162.68;

      _testDataTriplePointPressure = 6.51;

      _testDataTriplePointLiquidMoleDensity = 12874.405675062;

      _testDataTriplePointVaporMoleDensity = 0.00481334371794067;

      _testDataCriticalPointTemperature = 471.11;

      _testDataCriticalPointPressure = 4407638.00005723;

      _testDataCriticalPointMoleDensity = 4032.962;

      _testDataNormalBoilingPointTemperature = 296.858072364623;

      _testDataNormalSublimationPointTemperature = null;

      _testDataIsMeltingCurveImplemented = false;

      _testDataIsSublimationCurveImplemented = false;

      // TestData contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. Internal energy (J/mol)
      // 4. Enthalpy (J/mol)
      // 5. Entropy (J/mol K)
      // 6. Isochoric heat capacity (J/(mol K))
      // 7. Isobaric heat capacity (J/(mol K))
      // 8. Speed of sound (m/s)
      _testDataEquationOfState = new(double temperature, double moleDensity, double pressure, double internalEnergy, double enthalpy, double entropy, double isochoricHeatCapacity, double isobaricHeatCapacity, double speedOfSound)[]
      {
      ( 164.593069599154, 12846.5395154869, 9.76499924588285, 15236.8390139855, 15236.8397741123, 80.5304070801245, 74.641151311121, 105.484212321144, 1221.70169583912 ),
      ( 225, 0.00521985329678632, 9.76500230490861, 48371.0474067861, 50241.7900789835, 288.506619333413, 60.6023546618504, 68.9177954291699, 124.4464794211 ),
      ( 275, 0.00427075690127703, 9.76500086304427, 51581.2112586882, 53867.6914046606, 303.035532138585, 67.5559060824617, 75.8706441057877, 136.724138014725 ),
      ( 325, 0.00361370994505909, 9.76500046920622, 55104.3724324038, 57806.5816903874, 316.182631215884, 73.1699612478818, 81.4845633909059, 148.008766214177 ),
      ( 375, 0.00313187915144041, 9.76500029796654, 58880.1502750556, 61998.0867680634, 328.171061646683, 77.6990338207683, 86.0135970516383, 158.513501497875 ),
      ( 425, 0.00276342139477191, 9.76500020194, 62859.7643776019, 66393.4274737965, 339.168981509662, 81.3561535143965, 89.6707004011532, 168.383540048343 ),
      ( 475, 0.00247253414770241, 9.76500014119202, 67004.2487644951, 70953.6381592851, 349.310172385656, 84.3210400638348, 92.6355777836792, 177.722724822464 ),
      ( 525, 0.00223705422769439, 9.7650001003337, 71282.7977516318, 75647.91325909, 358.704539127565, 86.7405886655431, 95.0551203225751, 186.608553458851 ),
      ( 575, 0.00204252746902564, 9.76500007182806, 75671.1786420434, 80452.0201337303, 367.443929467002, 88.7314765891367, 97.0460038908434, 195.100902838464 ),
      ( 625, 0.00187912507131108, 9.76500005142975, 80150.3308177202, 85346.8981992712, 375.605828099027, 90.3847521338959, 98.699276169606, 203.247323329469 ),
      ( 200, 12316.5739055549, 999.999982500959, 19086.1879918951, 19086.2691833035, 101.694681908956, 76.496357735707, 111.281553506983, 1099.11506071815 ),
      ( 208.976958017799, 12179.638990988, 999.999997203585, 20089.4431092357, 20089.5252134752, 106.601494290228, 77.1164295258009, 112.213764590911, 1061.78340504789 ),
      ( 225, 0.535189458181656, 1000.00000000036, 48360.4002000923, 50228.8973924659, 249.97182844158, 60.8631624883327, 69.2738159665924, 124.350229118532 ),
      ( 275, 0.437549726485026, 1000.00000000002, 51578.027894605, 53863.4824143309, 264.536498139426, 67.6056347526061, 75.9435741856148, 136.678149644918 ),
      ( 325, 0.370157778895276, 1000, 55102.8531975349, 57804.4038794751, 277.690499662403, 73.1817261512662, 81.5056862419631, 147.979968749337 ),
      ( 375, 0.320774578506492, 1000, 58879.1379076257, 61996.5919298738, 289.680905138897, 77.7024243088767, 86.0223893157947, 158.493613118169 ),
      ( 425, 0.283022135264253, 999.999999999999, 62858.969414748, 66392.261957631, 300.679654134479, 81.3574327274798, 89.6757215814124, 168.369272046881 ),
      ( 475, 0.253222263327896, 1000, 67003.5811158209, 70952.6809574296, 310.82130990603, 84.3217295513896, 92.6390788849996, 177.712326156798 ),
      ( 525, 0.229100948134652, 1000, 71282.2212369945, 75647.1093273408, 320.215984075038, 86.7410906840749, 95.0578188364501, 186.600939865889 ),
      ( 575, 0.209175999626788, 1000, 75670.6742101219, 80451.337392247, 328.955595244288, 88.7319023183327, 97.0481843103572, 195.095349746703 ),
      ( 625, 0.192439861167837, 1000, 80149.8860263895, 85346.3146349502, 337.117659467539, 90.3851330118489, 98.7010804675585, 203.243325067675 ),
      ( 200, 12316.6120957644, 5356.65224713856, 19086.1006402729, 19086.5355530724, 101.694245146845, 76.4958103853952, 111.281278744548, 1099.14645173734 ),
      ( 225, 11933.1204016715, 5356.65225336752, 21899.0497828567, 21899.4986723401, 114.944308756731, 78.1269657829883, 113.641503121779, 996.438445612972 ),
      ( 232.982245552803, 11809.1066687873, 5356.65224937869, 22808.8223349253, 22809.2759384365, 118.917624231908, 78.5920450987681, 114.307368580987, 964.873862198837 ),
      ( 250, 2.58669137722675, 5356.65225500051, 49902.897127559, 51973.7480617526, 243.399374377124, 64.8669175263775, 73.4248124977333, 130.382727599762 ),
      ( 300, 2.15125800409921, 5356.65225494751, 53296.4352482543, 55786.4444636717, 257.286506506271, 70.64164652925, 79.0322036370395, 142.281896613781 ),
      ( 350, 1.84263941596843, 5356.65225494297, 56957.4723126335, 59864.5262964579, 269.849560043664, 75.5893718879799, 83.9412790299256, 153.219868779444 ),
      ( 400, 1.61173908155364, 5356.65225494211, 60842.3369800764, 64165.8601956197, 281.33082983679, 79.6354086080624, 87.9739394285201, 163.429864546969 ),
      ( 450, 1.43235220333941, 5356.65225494187, 64909.5797245678, 68649.3388721107, 291.888595409591, 82.9200480113659, 91.2520104594792, 173.048431965992 ),
      ( 500, 1.28893742330924, 5356.6522549418, 69125.0729941306, 73280.9398019924, 301.645883358281, 85.5941203141926, 93.9220372703846, 182.170172070966 ),
      ( 550, 1.17164931277977, 5356.65225494177, 73461.6481823868, 78033.5385906464, 310.703647744053, 87.7858531843549, 96.1109652210345, 190.865547441884 ),
      ( 600, 1.07393929271277, 5356.65225494176, 77897.8750334579, 82885.7289012597, 319.146421523012, 89.5976542960659, 97.9207025468996, 199.189257980571 ),
      ( 200, 12316.6527969287, 9999.99998301063, 19086.007544007, 19086.8194529037, 101.693779655215, 76.4952271708729, 111.280985931954, 1099.17990552546 ),
      ( 225, 11933.169920225, 9999.99999608627, 21898.9361958387, 21899.7741961406, 114.943803913673, 78.1266060551496, 113.6411142544, 996.46827604042 ),
      ( 243.656797842176, 11641.817375703, 10000.0000023312, 24033.6308686533, 24034.4898410697, 124.057727880469, 79.1869168636357, 115.204476749964, 923.848841507716 ),
      ( 250, 4.84497632579053, 10000.0000007602, 49876.5185559354, 51940.5121295528, 238.103203748592, 65.4021334400768, 74.1823972206626, 130.07843283135 ),
      ( 300, 4.02216790811859, 10000.0000000715, 53286.5363106853, 55772.7577287138, 252.06315387438, 70.7548989898533, 79.2128731183467, 142.112886565409 ),
      ( 350, 3.44302129275178, 10000.0000000149, 56951.797416761, 59856.2232537626, 264.643033336592, 75.6186682863401, 84.0033249019176, 153.107988078273 ),
      ( 400, 3.01064828121939, 10000.0000000042, 60838.172860199, 64159.7166162016, 276.130116234904, 79.6448914100451, 88.0043503877437, 163.350939561149 ),
      ( 450, 2.67506747566025, 10000.0000000014, 64906.176512442, 68644.3999744626, 286.690732163149, 82.9242837953283, 91.2714184751054, 172.991309053936 ),
      ( 500, 2.40693413412595, 10000.0000000005, 69122.1682306616, 73276.8311515196, 296.449774272617, 85.5968095981881, 93.9363695647053, 182.128415527139 ),
      ( 550, 2.18773134088959, 10000.0000000002, 73459.1214042314, 78030.066571536, 305.508754457026, 87.7879987959771, 96.1223125716129, 190.835008487485 ),
      ( 600, 2.00516730891121, 10000.0000000001, 77895.655083263, 82882.770101326, 313.95242264918, 89.5995365785205, 97.9299926628068, 199.167101556381 ),
      ( 200, 12317.4412411984, 99999.9999935516, 19084.2038428135, 19092.3224120874, 101.684759720291, 76.4839545062803, 111.27531677556, 1099.82772321121 ),
      ( 225, 11934.1291434412, 99999.9999974113, 21896.7356667461, 21905.1149962133, 114.934022138196, 78.1196599821117, 113.633586305002, 997.045931218325 ),
      ( 250, 11542.60130574, 99999.9999998916, 24763.4489184273, 24772.1124767839, 127.014810146382, 79.5260696283753, 115.746362240958, 900.624933153761 ),
      ( 275, 11138.2840978103, 100000.000002442, 27686.2869192116, 27695.2649627885, 138.157213024705, 80.8566329798418, 118.189460480533, 810.803620211598 ),
      ( 295.487069321078, 10792.7898537555, 99999.9999973259, 30131.6493081746, 30140.9147531015, 146.733749817706, 81.954482618829, 120.639263207466, 740.798714538682 ),
      ( 300, 41.5226292722132, 100000, 53077.9055284167, 55486.2308621623, 232.214030820678, 73.2937756724719, 83.3842557050443, 138.614795772981 ),
      ( 350, 35.0598824187798, 99999.9999999999, 56837.7144518859, 59689.9772976516, 245.170167869943, 76.2497054951844, 85.3354226284982, 150.879012919709 ),
      ( 400, 30.4624738618471, 100000.000046304, 60756.1133051567, 64038.8407848691, 256.779444761036, 79.8428716060439, 88.6283611337445, 161.799375045889 ),
      ( 450, 26.9668438643989, 100000.000014525, 64839.6575847785, 68547.9150478086, 267.397785943851, 83.0099488822627, 91.6594246795946, 171.874785942311 ),
      ( 500, 24.205884574297, 100000.00000501, 69065.6001300515, 73196.8269686358, 277.19165295292, 85.6498687415078, 94.2190876898651, 181.314586039427 ),
      ( 550, 21.9656285662998, 100000.000001805, 73410.0072348583, 77962.5744286434, 286.274529154207, 87.8298090761955, 96.344608957788, 190.240714054735 ),
      ( 600, 20.109559102098, 100000.000000661, 77852.5522661884, 82825.3117132231, 294.735684165139, 89.6360426263189, 98.111293269732, 198.736251700001 ),
      ( 200, 12317.4528424815, 101324.999987027, 19084.1772988788, 19092.4034314472, 101.684626962132, 76.4837889948687, 111.275233402434, 1099.83725197635 ),
      ( 225, 11934.1432572007, 101325.000001044, 21896.7032853839, 21905.1936309258, 114.933878176695, 78.1195580945808, 113.6334756088, 997.054427994351 ),
      ( 250, 11542.6186133113, 101325.000001058, 24763.4094523622, 24772.1877897045, 127.014652229815, 79.5259982582236, 115.746203394618, 900.633166603475 ),
      ( 275, 11138.3055443498, 101324.999998921, 27686.2384436053, 27695.3354287432, 138.157036686495, 80.8565735035625, 118.189226012104, 810.812102578756 ),
      ( 295.858072364623, 10786.41072762, 101325.000002036, 30176.3522253852, 30185.7459896837, 146.884959099726, 81.9745455509089, 120.687793833709, 739.561563403497 ),
      ( 300, 42.0940574807158, 101325, 53074.5761635404, 55481.6855291895, 232.093237140844, 73.3364159262282, 83.4563924031506, 138.559841769303 ),
      ( 350, 35.5342189732022, 101325, 56835.9743758855, 59687.4511475127, 245.055695624355, 76.259929129832, 85.3569885087928, 150.84529997871 ),
      ( 400, 30.8715372621315, 101325.000048874, 60754.8855778254, 64037.0350603251, 256.666913694371, 79.8459926903953, 88.6380574548888, 161.776215394005 ),
      ( 450, 27.3274269274652, 101325.000015321, 64838.6702567867, 68546.4836665258, 267.286141036523, 83.0112615399529, 91.6653091101021, 171.858213865518 ),
      ( 500, 24.5286697671456, 101325.000005283, 69064.7634991745, 73195.6438513166, 277.080532702535, 85.6506633120029, 94.2233209682545, 181.302540601294 ),
      ( 550, 22.2579999216983, 101325.000001903, 73409.2821801858, 77961.5780000419, 286.163765468507, 87.8304278099858, 96.3479154797922, 190.231930992382 ),
      ( 600, 20.3768788165816, 101325.000000697, 77851.9166330671, 82824.464240223, 294.625180086051, 89.6365804012758, 98.1139800799332, 198.72988878144 ),
      ( 200, 12325.2792586185, 999999.999990281, 19066.2441142982, 19147.3781786068, 101.594819331396, 76.3744765267924, 111.219277582708, 1106.24361945479 ),
      ( 225, 11943.6620635053, 1000000.00000415, 21874.8423956075, 21958.5688100596, 114.836555512172, 78.0529132881326, 113.559267759232, 1002.7672340422 ),
      ( 250, 11554.2834379941, 1000000.00000042, 24736.7900538434, 24823.3380431988, 126.907982365292, 79.4799102946251, 115.639899862822, 906.167069704826 ),
      ( 275, 11152.7434411233, 999999.99999679, 27653.5819831986, 27743.2460201484, 138.03805374176, 80.8186709696119, 118.032704959764, 816.508398635911 ),
      ( 300, 10732.6535938082, 1000000.00000036, 30636.68732465, 30729.8609271762, 148.430957887612, 82.1608881768405, 121.009694347247, 731.854355427485 ),
      ( 325, 10285.3906576519, 999999.999998438, 33702.9789497799, 33800.2042310826, 158.259299537065, 83.5454716326828, 124.766948210418, 649.988156962365 ),
      ( 350, 9798.49508743176, 1000000.00032096, 36875.1366495444, 36977.1931379539, 167.674695767059, 85.0046337685782, 129.614952361205, 568.498990640688 ),
      ( 375, 9251.23151658742, 999999.999908793, 40187.8717543861, 40295.9654712189, 176.830922233553, 86.6114312833559, 136.306124190512, 484.246620938134 ),
      ( 381.42736754705, 9096.50293806221, 1000000.00000005, 41069.126075767, 41179.0584318393, 179.165829717542, 87.0682214315646, 138.532532153482, 461.571755360273 ),
      ( 400, 353.22124355005, 1000000.00000026, 59743.0898015707, 62574.176601305, 235.007213637814, 83.8349924500571, 100.799630406964, 143.189413319357 ),
      ( 450, 295.505821323894, 1000000.00000001, 64104.8783216985, 67488.9064112972, 246.588569170415, 84.3159827378991, 97.1838865780084, 159.572698768763 ),
      ( 500, 257.379928722108, 1000000, 68468.7048046009, 72354.0116541482, 256.839894168714, 86.2912508982362, 97.6620390757834, 172.67456541374 ),
      ( 550, 229.219190399957, 999999.999999999, 72903.1926413511, 77265.8291128225, 266.201917165628, 88.273492681312, 98.8444525479418, 184.047450529794 ),
      ( 600, 207.229427876532, 999999.999999998, 77413.2805506588, 82238.8500185039, 274.85533456689, 90.0037731481201, 100.06372455428, 194.286624058774 ),
      ( 200, 12356.06110827, 4628019.90006385, 18995.2314553022, 19369.7860855772, 101.236926243538, 75.9891005469705, 111.004839023979, 1131.01934669187 ),
      ( 222, 12026.3195819144, 4628019.90006281, 21450.5685522163, 21835.3928426852, 112.931036278329, 77.6316362778532, 113.03628743839, 1037.17117964723 ),
      ( 244, 11692.1456754281, 4628019.89968855, 23945.3813570761, 24341.2043332998, 123.692316600148, 78.9937819943933, 114.758462771217, 950.086006545966 ),
      ( 266, 11351.1690313806, 4628019.90006047, 26478.0153161334, 26885.7283796307, 133.675894725315, 80.2229635241413, 116.604520048186, 869.698397564444 ),
      ( 288, 11000.0357604488, 4628019.90006128, 29053.5200196106, 29474.2477336697, 143.024503182738, 81.4099618791048, 118.780627974899, 794.63883665421 ),
      ( 310, 10634.5921847273, 4628019.9000611, 31680.0623992552, 32115.2478600299, 151.86009120235, 82.5947062647319, 121.388614656312, 723.436457162096 ),
      ( 332, 10249.6983552796, 4628019.9000605, 34367.5640573861, 34819.0914818665, 160.285358601409, 83.7928394548264, 124.508596499567, 654.730181178582 ),
      ( 354, 9838.64956256166, 4628019.90006028, 37127.8593097942, 37598.2510917793, 168.389358128422, 85.0154403679435, 128.263161173654, 587.224387803026 ),
      ( 376, 9391.92276353712, 4628019.89990914, 39976.3189513132, 40469.0849180306, 176.255636952406, 86.2837298065559, 132.901720604796, 519.549565854082 ),
      ( 398, 8894.35303693668, 4628019.90006051, 42936.025726345, 43456.3581081204, 183.975135813403, 87.6466786542464, 138.987858995661, 450.012704421683 ),
      ( 420, 8317.68952013327, 4628019.90005997, 46048.8956095089, 46605.3025166022, 191.673969513211, 89.2203114229623, 147.992167447306, 376.054090596469 ),
      ( 442, 7594.37768913838, 4628019.90006, 49414.3472491171, 50023.7480571226, 199.603481220918, 91.3306348365911, 165.106905316332, 292.609451858039 ),
      ( 464, 6439.45100925655, 4628019.90006013, 53446.0693773335, 54164.7672746244, 208.734606217747, 95.5453165590013, 232.835323287178, 184.222169032562 ),
      ( 486, 2204.16048220347, 4628019.90006009, 62701.7135927862, 64801.3882477975, 231.138274376348, 96.6274430178056, 220.13686087666, 111.64688641429 ),
      ( 508, 1706.21544534922, 4628019.90006009, 65819.9551430115, 68532.4026904037, 238.657971943485, 92.1865931740745, 143.412818601649, 131.835316575885 ),
      ( 530, 1473.78804611944, 4628019.90006009, 68321.0261005394, 71461.2469160399, 244.304651877159, 90.965759849343, 125.73968428775, 145.501956085791 ),
      ( 552, 1322.38180107745, 4628019.90006841, 70635.0133284238, 74134.774060383, 249.24818637687, 90.7894946646231, 118.198667095461, 156.340486260146 ),
      ( 574, 1210.88917388158, 4628019.90006189, 72865.4693888039, 76687.4706089705, 253.783360455465, 91.0401809641885, 114.243919025646, 165.519737282045 ),
      ( 596, 1123.25175087384, 4628019.90006055, 75053.1417621626, 79173.3400137657, 258.033481695651, 91.4710391274515, 111.935269987722, 173.582728542129 ),
      ( 618, 1051.49971628992, 4628019.9000602, 77217.5574002591, 81618.9089444987, 262.063023731696, 91.9704177424723, 110.496024071955, 180.831800276181 ),
      ( 200, 12399.4742660179, 10000000.0000016, 18893.9339454318, 19700.4197516411, 100.720116096602, 75.5615075317001, 110.71788499289, 1164.84040489119 ),
      ( 222, 12077.7182305173, 9999999.99999649, 21331.0178048092, 22158.9887682921, 112.380908635685, 77.4006715388099, 112.67541902372, 1067.65647528811 ),
      ( 244, 11753.0992308736, 9999999.99999795, 23804.771198251, 24655.6106069518, 123.102809530402, 78.8699832224962, 114.275717786207, 979.3530783198 ),
      ( 266, 11423.6528231776, 9999999.99999978, 26312.3151157889, 27187.6918586561, 133.0376708584, 80.1567751877004, 115.946178622078, 899.096037818689 ),
      ( 288, 11086.68663331, 9999999.9999986, 28857.1989493895, 29759.1816905588, 142.324897063426, 81.3696075891723, 117.877349024068, 825.210506946579 ),
      ( 310, 10739.0557806805, 10000000.0000013, 31445.5680819733, 32376.7486442338, 151.082232530436, 82.5555523595667, 120.141686715086, 756.154660749878 ),
      ( 332, 10377.1908427505, 9999999.99950049, 34084.4063121521, 35048.0582436849, 159.406301403945, 83.7303466430662, 122.76626205549, 690.662997811313 ),
      ( 354, 9996.95673102155, 9999999.99988061, 36780.9655218268, 37781.2699413558, 167.376550841609, 84.897963715355, 125.774258483118, 627.709923137897 ),
      ( 376, 9593.34002034509, 9999999.9999712, 39542.8857848321, 40585.2756072259, 175.060044145562, 86.0614027145623, 129.213575588021, 566.447324428865 ),
      ( 398, 9159.88821516217, 9999999.99999466, 42378.8463903257, 43470.5627699472, 182.516505819273, 87.2286281029443, 133.187401154118, 506.154955489297 ),
      ( 420, 8687.71650057859, 9999999.99999936, 45299.8635617609, 46450.9140132039, 189.804050402017, 88.4163525955533, 137.902782992216, 446.221177474815 ),
      ( 442, 8163.70827957313, 9999999.99999979, 48321.6498748929, 49546.5834047963, 196.98686875817, 89.6529097983686, 143.761330253141, 386.189355372185 ),
      ( 464, 7567.28698482557, 10000000.0000001, 51468.7262808885, 52790.2038487478, 204.147066362945, 90.9784569624255, 151.518398408189, 325.994639572396 ),
      ( 486, 6865.96787300094, 10000000.0000006, 54779.5582265741, 56236.0171242808, 211.400774497605, 92.4249622901327, 162.344287507895, 266.87823298004 ),
      ( 508, 6021.4948521022, 10000000, 58296.092183099, 59956.8093713747, 218.886234262695, 93.8775060535899, 176.100975509085, 214.014821287034 ),
      ( 530, 5056.87215511021, 10000000, 61970.5328433641, 63948.0398265878, 226.576332682383, 94.7544469584763, 184.24386952165, 176.809926544699 ),
      ( 552, 4170.93487027387, 9999999.99997325, 65500.7919700339, 67898.3360005684, 233.880955335582, 94.6002657424918, 171.830484377825, 162.706072521629 ),
      ( 574, 3527.53035798286, 9999999.99999948, 68639.3421826156, 71474.1866730023, 240.23549386806, 94.0769374172164, 153.824990118773, 162.296254555268 ),
      ( 596, 3079.01602538947, 9999999.99999998, 71457.3617062079, 74705.1525320887, 245.760697731024, 93.8268638638449, 140.796856159509, 166.72917507719 ),
      ( 618, 2754.20315967731, 9999999.99999999, 74072.0544792112, 77702.8687003276, 250.700738937227, 93.8820001037335, 132.336403466758, 172.883051457541 ),
      };

      // TestData contains:
      // 0. Temperature (Kelvin)
      // 1. Pressure (Pa)
      // 2. Saturated liquid density (mol/m³
      // 3. Saturated vapor density (mol/m³)
      _testDataSaturatedProperties = new(double temperature, double pressure, double saturatedLiquidMoleDensity, double saturatedVaporMoleDensity)[]
      {
      ( 201.23375, 482.854322916652, 12297.7984215237, 0.288925170598288 ),
      ( 239.7875, 7582.8021574675, 11702.6639275636, 3.82852495378098 ),
      ( 278.34125, 49990.0812040187, 11082.1073361581, 22.0999437208235 ),
      ( 316.895, 195872.422384644, 10414.8276515494, 78.8727007912667 ),
      ( 355.44875, 552789.310352998, 9668.7234930888, 212.163146621017 ),
      ( 394.0025, 1256787.4136743, 8787.68496026752, 486.57648456095 ),
      ( 432.55625, 2466714.20981762, 7626.11416352488, 1056.40870244035 ),
      };
    }

    [Test]
    public override void CASNumberAttribute_Test()
    {
      base.CASNumberAttribute_Test();
    }

    [Test]
    public override void ConstantsAndCharacteristicPoints_Test()
    {
      base.ConstantsAndCharacteristicPoints_Test();
    }

    [Test]
    public override void EquationOfState_Test()
    {
      base.EquationOfState_Test();
    }

    [Test]
    public override void SaturatedVaporPressure_TestMonotony()
    {
      base.SaturatedVaporPressure_TestMonotony();
    }

    [Test]
    public override void SaturatedVaporPressure_TestInverseIteration()
    {
      base.SaturatedVaporPressure_TestInverseIteration();
    }

    [Test]
    public override void SaturatedVaporProperties_TestData()
    {
      base.SaturatedVaporProperties_TestData();
    }

    [Test]
    public override void MeltingPressure_TestImplemented()
    {
      base.MeltingPressure_TestImplemented();
    }

    [Test]
    public override void SublimationPressure_TestImplemented()
    {
      base.SublimationPressure_TestImplemented();
    }
  }
}