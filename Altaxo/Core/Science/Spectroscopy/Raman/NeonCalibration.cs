using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Altaxo.Calc;
using Altaxo.Calc.Interpolation;

namespace Altaxo.Science.Spectroscopy.Raman
{
  /// <summary>
  /// Represents the results of a calibration of a Raman device with light from a Neon lamp.
  /// </summary>
  public class NeonCalibration
  {
    #region Nist Neon Peak data

    /// <summary>
    /// The neon peaks from NIST. Reference: <see href="https://physics.nist.gov/PhysRefData/Handbook/Tables/neontable2.htm"/>
    /// </summary>
    public static readonly (double Wavelength_Nanometer, double Intensity)[] NistNeonPeaks = new (double Wavelength_Nanometer, double Intensity)[]
    {
      (35.29549, 90),
      (35.4962, 60),
      (36.14321, 90),
      (36.24544, 60),
      (40.58538, 150),
      (40.71377, 120),
      (44.50393, 200),
      (44.62552, 300),
      (44.65902, 250),
      (44.78146, 200),
      (45.4654, 150),
      (45.5273, 200),
      (45.62728, 10),
      (45.63485, 120),
      (45.68962, 90),
      (46.07284, 1000),
      (46.23908, 500),
      (58.72127, 30),
      (58.71792, 30),
      (58.99114, 30),
      (59.18306, 70),
      (59.592, 100),
      (59.87056, 70),
      (59.88897, 30),
      (60.00365, 70),
      (60.27263, 130),
      (61.56283, 170),
      (61.86716, 170),
      (61.91023, 130),
      (62.68232, 200),
      (62.97388, 200),
      (73.58962, 1000),
      (74.37195, 400),
      (99.38825, 60),
      (106.86488, 70),
      (113.17224, 90),
      (113.1849, 100),
      (122.98367, 90),
      (141.83779, 90),
      (142.85822, 90),
      (143.60813, 90),
      (168.1684, 120),
      (168.83553, 200),
      (188.81064, 100),
      (188.9712, 100),
      (190.7494, 200),
      (191.60818, 500),
      (193.00345, 300),
      (193.88269, 200),
      (194.54521, 100),
      (200.7009, 80),
      (202.556, 80),
      (208.5466, 150),
      (209.6106, 200),
      (209.6248, 120),
      (256.2123, 80),
      (256.7121, 90),
      (262.3107, 80),
      (262.9885, 80),
      (263.6069, 90),
      (263.8289, 80),
      (264.4097, 80),
      (276.2921, 80),
      (279.2019, 90),
      (279.4221, 80),
      (280.9485, 100),
      (290.6592, 80),
      (290.6816, 80),
      (291.0061, 90),
      (291.0408, 90),
      (291.1138, 80),
      (291.5122, 80),
      (292.5618, 80),
      (293.2103, 80),
      (294.0653, 80),
      (294.6044, 90),
      (295.5725, 150),
      (296.3236, 150),
      (296.7184, 150),
      (297.2997, 100),
      (297.47189, 30),
      (297.9461, 100),
      (298.26696, 30),
      (300.1668, 150),
      (301.7311, 120),
      (302.7016, 300),
      (302.8864, 300),
      (303.0787, 100),
      (303.4461, 120),
      (303.5923, 100),
      (303.772, 100),
      (303.9586, 100),
      (304.4088, 100),
      (304.5556, 100),
      (304.7556, 120),
      (305.4345, 100),
      (305.4677, 100),
      (305.73907, 30),
      (305.9106, 100),
      (306.2491, 100),
      (306.3301, 100),
      (307.0887, 100),
      (307.1529, 100),
      (307.5731, 100),
      (308.8166, 120),
      (309.2092, 100),
      (309.2901, 120),
      (309.4006, 100),
      (309.5103, 100),
      (309.7131, 100),
      (311.798, 100),
      (311.816, 120),
      (314.1332, 300),
      (314.3721, 100),
      (314.8681, 100),
      (316.4429, 100),
      (316.5648, 100),
      (318.8743, 100),
      (319.4579, 120),
      (319.8586, 500),
      (320.8965, 60),
      (320.9356, 120),
      (321.3735, 120),
      (321.4329, 150),
      (321.8193, 150),
      (322.4818, 120),
      (322.9573, 120),
      (323.007, 200),
      (323.0419, 120),
      (323.2022, 120),
      (323.2372, 150),
      (324.3396, 100),
      (324.4095, 100),
      (324.8345, 100),
      (325.0355, 100),
      (329.7726, 150),
      (330.974, 150),
      (331.9722, 300),
      (332.3745, 1000),
      (332.7153, 150),
      (332.9158, 100),
      (333.4836, 200),
      (334.4395, 150),
      (334.5453, 300),
      (334.5829, 150),
      (335.5016, 200),
      (335.782, 120),
      (336.0597, 200),
      (336.2161, 120),
      (336.2707, 100),
      (336.7218, 120),
      (336.98076, 50),
      (336.99072, 70),
      (337.1799, 100),
      (337.8216, 500),
      (338.8417, 150),
      (338.8945, 120),
      (339.2798, 300),
      (340.4822, 100),
      (340.6947, 120),
      (341.3148, 100),
      (341.6914, 120),
      (341.7688, 120),
      (341.79031, 50),
      (341.80055, 5),
      (342.8687, 120),
      (344.77024, 20),
      (345.41944, 10),
      (345.661, 100),
      (345.9321, 100),
      (346.05237, 10),
      (346.43382, 10),
      (346.65781, 20),
      (347.25706, 50),
      (347.9519, 150),
      (348.0718, 200),
      (348.1933, 200),
      (349.80636, 10),
      (350.12159, 20),
      (351.51902, 20),
      (352.04711, 100),
      (354.2847, 120),
      (355.7805, 120),
      (356.1198, 100),
      (356.8502, 250),
      (357.4181, 100),
      (357.4612, 200),
      (359.35257, 50),
      (359.36389, 30),
      (360.01685, 10),
      (363.3664, 10),
      (364.3927, 150),
      (366.4073, 200),
      (368.22421, 10),
      (368.57352, 10),
      (369.4213, 200),
      (370.12244, 4),
      (370.9622, 150),
      (371.3079, 250),
      (372.7107, 250),
      (376.6259, 800),
      (377.7133, 1000),
      (381.8427, 100),
      (382.9749, 120),
      (421.9745, 150),
      (423.385, 100),
      (425.0649, 120),
      (436.9862, 120),
      (437.94, 70),
      (437.955, 150),
      (438.5059, 100),
      (439.1991, 200),
      (439.799, 150),
      (440.9299, 150),
      (441.3215, 100),
      (442.1389, 100),
      (442.8516, 100),
      (442.8634, 100),
      (443.0904, 150),
      (443.0942, 150),
      (445.7049, 120),
      (452.272, 100),
      (453.77545, 100),
      (456.9057, 100),
      (470.43949, 150),
      (470.88594, 120),
      (471.0065, 100),
      (471.20633, 150),
      (471.5344, 150),
      (475.2732, 50),
      (478.89258, 100),
      (479.02195, 50),
      (482.7338, 100),
      (488.4917, 100),
      (500.51587, 50),
      (503.77512, 50),
      (514.49384, 50),
      (533.07775, 60),
      (534.10938, 100),
      (534.32834, 60),
      (540.05618, 200),
      (556.27662, 50),
      (565.66588, 50),
      (571.92248, 50),
      (574.82985, 50),
      (576.44188, 70),
      (580.44496, 50),
      (582.01558, 50),
      (585.24879, 200),
      (587.28275, 50),
      (588.18952, 100),
      (590.24623, 5),
      (590.64294, 5),
      (594.48342, 50),
      (596.5471, 50),
      (597.46273, 50),
      (597.5534, 60),
      (598.79074, 15),
      (602.99969, 100),
      (607.43377, 100),
      (609.61631, 30),
      (612.84499, 10),
      (614.30626, 100),
      (616.35939, 100),
      (618.2146, 15),
      (621.72812, 100),
      (626.6495, 100),
      (630.47889, 10),
      (632.81646, 30),
      (633.44278, 100),
      (638.29917, 100),
      (640.2248, 200),
      (650.65281, 150),
      (653.28822, 10),
      (659.89529, 100),
      (665.20927, 15),
      (667.82762, 50),
      (671.7043, 7),
      (692.94673, 1000),
      (702.40504, 300),
      (703.24131, 800),
      (705.12923, 20),
      (705.91074, 100),
      (717.39381, 800),
      (721.32, 150),
      (723.5188, 150),
      (724.51666, 800),
      (734.3945, 150),
      (747.24386, 30),
      (748.88712, 300),
      (749.2102, 100),
      (752.2818, 150),
      (753.57741, 300),
      (754.40443, 130),
      (772.46233, 1),
      (774.0738, 120),
      (783.90529, 2),
      (792.6201, 120),
      (792.71177, 3),
      (793.69961, 13),
      (794.31814, 80),
      (808.2458, 60),
      (808.4345, 100),
      (811.85492, 40),
      (812.89108, 12),
      (813.64054, 170),
      (825.9379, 30),
      (826.4807, 100),
      (826.60772, 70),
      (826.71162, 10),
      (830.03258, 300),
      (831.4995, 100),
      (836.57466, 50),
      (837.2106, 100),
      (837.7608, 800),
      (841.71606, 30),
      (841.84274, 250),
      (846.33575, 40),
      (848.44435, 13),
      (849.53598, 700),
      (854.46958, 15),
      (857.13524, 30),
      (859.12584, 400),
      (863.4647, 350),
      (864.70411, 60),
      (865.43831, 600),
      (865.5522, 80),
      (866.8256, 100),
      (867.94925, 130),
      (868.19211, 150),
      (870.41116, 30),
      (877.16563, 100),
      (878.06226, 600),
      (878.37533, 400),
      (883.09072, 6),
      (885.38668, 300),
      (886.53063, 20),
      (886.57552, 150),
      (891.95006, 60),
      (898.85564, 20),
      (907.9462, 100),
      (914.86716, 120),
      (920.17591, 90),
      (922.00601, 60),
      (922.15801, 20),
      (922.66903, 20),
      (927.55196, 9),
      (928.7563, 200),
      (930.08527, 80),
      (931.05839, 8),
      (931.39726, 30),
      (932.65068, 70),
      (937.33078, 15),
      (942.53788, 50),
      (945.92095, 30),
      (948.66818, 50),
      (953.41629, 60),
      (954.74049, 30),
      (957.7013, 120),
      (966.54197, 180),
      (980.886, 100),
      (1029.54174, 4),
      (1056.24075, 80),
      (1079.80429, 60),
      (1084.44772, 90),
      (1114.302, 300),
      (1117.7524, 500),
      (1139.04339, 150),
      (1140.91343, 90),
      (1152.27459, 300),
      (1152.50194, 150),
      (1153.63445, 90),
      (1160.15366, 30),
      (1161.40807, 130),
      (1168.80017, 30),
      (1176.67924, 150),
      (1178.90435, 130),
      (1178.98891, 30),
      (1198.4912, 70),
      (1206.6334, 200),
      (1245.9389, 40),
      (1268.9201, 60),
      (1291.2014, 80),
      (1321.9241, 40),
      (1523.0714, 50),
      (1716.1929, 20),
      (1803.5812, 20),
      (1808.3181, 40),
      (1808.3263, 9),
      (1822.1087, 15),
      (1822.7016, 13),
      (1827.6642, 140),
      (1828.2614, 100),
      (1830.3967, 70),
      (1835.9094, 20),
      (1838.4826, 60),
      (1838.9937, 90),
      (1840.2836, 40),
      (1842.2402, 60),
      (1845.864, 13),
      (1847.58, 40),
      (1859.1541, 70),
      (1859.7698, 100),
      (1861.8908, 16),
      (1862.5159, 20),
      (2104.127, 30),
      (2170.811, 30),
      (2224.736, 13),
      (2242.814, 13),
      (2253.038, 80),
      (2266.179, 13),
      (2310.048, 25),
      (2326.027, 40),
      (2337.296, 50),
      (2356.533, 30),
      (2363.648, 170),
      (2370.166, 12),
      (2370.913, 60),
      (2395.14, 110),
      (2395.643, 50),
      (2397.816, 60),
      (2409.857, 11),
      (2416.143, 20),
      (2424.961, 30),
      (2436.501, 70),
      (2437.161, 40),
      (2444.786, 20),
      (2445.939, 30),
      (2477.649, 17),
      (2492.889, 30),
      (2516.17, 13),
      (2552.433, 50),
      (2838.62, 6),
      (3020.049, 6),
      (3317.309, 8),
      (3335.238, 17),
      (3389.981, 5),
      (3390.302, 4),
      (3391.31, 12),
      (3413.134, 4),
      (3447.143, 6),
      (3583.481, 8),
    };

    #endregion

    #region Wavelength converter

    public record WavelengthConverter
    {
      public double NistWL_Left { get; init; }
      public double MeasWL_Left { get; init; }
      public double NistWL_Right { get; init; }
      public double MeasWL_Right { get; init; }



      public double ConvertWavelengthNistToMeas(double x)
      {
        var r = (x - NistWL_Left) / (NistWL_Right - NistWL_Left);
        return (1 - r) * MeasWL_Left + r * MeasWL_Right;
      }

      public double ConvertWavelengthMeasToNist(double x)
      {
        var r = (x - MeasWL_Left) / (MeasWL_Right - MeasWL_Left);
        return (1 - r) * NistWL_Left + r * NistWL_Right;
      }

    }

    #endregion

    #region Operational data

    /// <summary>
    /// The x-vales of the Neon measurement, converted to nm, presumed that the laser has the wavelength
    /// </summary>
    private double[]? _xOriginal_nm;

    /// <summary>
    /// The x-values (after preprocessing) of the Neon measurement, converted to nm, presumed that the laser has the wavelength.
    /// </summary>
    private double[]? _xPreprocessed_nm;

    /// <summary>
    /// The y (signal) values of the preprocessed spectrum.
    /// </summary>
    private double[]? _yPreprocessed;


    private double _assumedLaserWavelength_nm = double.NaN;

    private List<PeakSearching.PeakDescription>? _peakSearchingDescriptions;
    private IReadOnlyList<PeakFitting.PeakDescription>? _peakFittingDescriptions;

    public WavelengthConverter Converter { get; protected set; }

    /// <summary>
    /// The x-values of the Neon measurement, converted to nm, presumed that the laser has the wavelength
    /// </summary>
    public double[]? XOriginal_nm => _xOriginal_nm;

    /// <summary>
    /// The x-values (after preprocessing) of the Neon measurement, converted to nm, presumed that the laser has the wavelength
    /// </summary>
    public double[]? XPreprocessed_nm => _xPreprocessed_nm;


    /// <summary>
    /// The y (signal) values of the preprocessed spectrum.
    /// </summary>
    public double[]? YPreprocessed => _yPreprocessed;

    /// <summary>
    /// Gets the peak matchings, i.e. the correspondence between official Nist peak position, and measured peak position.
    /// </summary>
    public List<(double NistWL, double MeasWL, double MeasWLStdDev)> PeakMatchings { get; private set; } = new();

    /// <summary>
    /// Gets an interpolation function that maps the measured wavelength to the difference between Nist and measured wavelength.
    /// </summary>
    public Func<double, double>? MeasuredWavelengthToWavelengthDifference { get; private set; }

    #endregion


    /// <summary>
    /// Evaluates the Neon calibration. After the calibration is evaluated, properties like <see cref="PeakMatchings"/>
    /// and <see cref="MeasuredWavelengthToWavelengthDifference"/> are useable.
    /// </summary>
    /// <param name="options">The calibration options.</param>
    /// <param name="x">The array of x-values of the Neon spectrum.</param>
    /// <param name="y">The array of y-values of the Neon spectrum.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the evaluation was successfull; otherwise, if no peaks could be matched, false.</returns>
    public bool Evaluate(NeonCalibrationOptions options, double[] x, double[] y, CancellationToken cancellationToken)
    {
      if (options is null)
        throw new ArgumentNullException(nameof(options));
      if (x is null)
        throw new ArgumentNullException(nameof(x));
      if (y is null)
        throw new ArgumentNullException(nameof(y));
      if (x.Length != y.Length)
        throw new ArgumentException($"Length of {nameof(x)}-array ({x.Length}) does not match length of {nameof(y)}-array ({y.Length})");

      var coarse = FindCoarseMatch(options, x, y, cancellationToken);

      if (coarse is null)
      {
        PeakMatchings = new();
        return false;
      }

      if (_peakFittingDescriptions is not null)
        PeakMatchings = GetPeakMatchingsBasedOnPeakFittingResults(coarse.Value);
      else
        PeakMatchings = GetPeakMatchingsBasedOnPeakSearchingResults(coarse.Value);

      if (options.FilterOutPeaksCorrespondingToMultipleNistPeaks)
      {
        PeakMatchings = FilterOutMatchesOfMeasuredPeaksCorrespondsToMultipleNistPeaks(PeakMatchings);
      }

      MeasuredWavelengthToWavelengthDifference = GetSplineMeasuredWavelengthToWavelengthDifference(options, PeakMatchings);

      return true;
    }

    /// <summary>
    /// Creates a spline that corresponds the measured wavelength to the wavelength difference between the
    /// official Nist wavelength and measured wavelength.
    /// </summary>
    public static Func<double, double> GetSplineMeasuredWavelengthToWavelengthDifference(NeonCalibrationOptions options, List<(double NistWL, double MeasWL, double MeasWLStdDev)> PeakMatchings)
    {
      var x = PeakMatchings.Select(p => p.NistWL).ToArray();
      var p = PeakMatchings.ToArray();
      Array.Sort(x, p);
      var y = p.Select(p => (p.NistWL - p.MeasWL)).ToArray();
      var dy = p.Select(p => p.MeasWLStdDev).ToArray();
      // spline difference Nist wavelength - Measured wavelength versus the Nist wavelength
      // why x is Nist wavelength (and not measured wavelength)? Because it has per definition no error, whereas measured wavelength has
      IInterpolationFunction spline;
      if (!options.InterpolationIgnoreStdDev && dy.Max() > 0 && dy.Select(v => RMath.IsFinite(v)).Count() >= 1)
      {
        // first, sanitize the standard deviations:
        // if for some values it is Infinity, we replace those with the mean of the finite standard deviations
        var meanStdDev = dy.Where(v => RMath.IsFinite(v)).Average();
        for (int i = 0; i < dy.Length; i++)
        {
          if (!RMath.IsFinite(dy[i]))
            dy[i] = meanStdDev;
        }
        spline = options.InterpolationMethod.Interpolate(x, y, dy);
      }
      else
      {
        spline = options.InterpolationMethod.Interpolate(x, y);
      }


      // now, we calculate the splined measured wavelength in dependence on the Nist wavelength
      var xx = new double[x.Length];
      for (var i = 0; i < xx.Length; i++)
      {
        var diff = spline.GetYOfX(x[i]);
        xx[i] = x[i] - diff; // we calculate the splined measured wavelengh
      }
      // new spline y=(Nist wavelength - Measured wavelength) versus x = (splined) measured wavelength
      if (options.InterpolationIgnoreStdDev)
        spline = options.InterpolationMethod.Interpolate(xx, y); // out spline now contains a function that has the measured wavelength as argument, and returns the correction offset to get the calibrated wavelength
      else
        spline = options.InterpolationMethod.Interpolate(xx, y, dy); // out spline now contains a function that has the measured wavelength as argument, and returns the correction offset to get the calibrated wavelength

      //spline = new RationalCubicSpline();
      //spline.Interpolate(xx, y);

      return spline.GetYOfX;
    }


    List<(double NistWL, double MeasWL, double MeasWLStdDev)> FilterOutMatchesOfMeasuredPeaksCorrespondsToMultipleNistPeaks(List<(double NistWL, double MeasWL, double MeasWLStdDev)> list)
    {
      var countDict = new Dictionary<double, int>();
      foreach (var pair in list)
      {
        if (!countDict.TryGetValue(pair.MeasWL, out var cnt))
        {
          cnt = 0;
        }
        countDict[pair.MeasWL] = cnt + 1;
      }

      var result = new List<(double NistWL, double MeasWL, double MeasWLStdDev)>();
      foreach (var pair in list)
      {
        if (countDict[pair.MeasWL] == 1)
          result.Add(pair);
      }

      return result;
    }

    private List<(double NistWL, double MeasWL, double MeasWLStdDev)> GetPeakMatchingsBasedOnPeakSearchingResults((double NistWL_Left, double MeasWL_Left, double NistWL_Right, double MeasWL_Right) coarse)
    {
      var (NistWL_Left, MeasWL_Left, NistWL_Right, MeasWL_Right) = coarse;
      var x_nm = _xPreprocessed_nm;

      var converter = new WavelengthConverter
      {
        NistWL_Left = NistWL_Left,
        MeasWL_Left = MeasWL_Left,
        NistWL_Right = NistWL_Right,
        MeasWL_Right = MeasWL_Right
      };
      Converter = converter;

      var foundPeaks = _peakSearchingDescriptions;

      var result = new List<(double NistWL, double MeasWL, double MeasWLStdDev)>();

      foreach (var peakDesc in foundPeaks)
      {
        var measPeakCenterWL = RMath.InterpolateLinear(peakDesc.PositionIndex, x_nm);
        // Note that for left and right we use full width = 2 x half width
        var measPeakLeftWL = RMath.InterpolateLinear(peakDesc.PositionIndex - peakDesc.Width, x_nm);
        var measPeakRightWL = RMath.InterpolateLinear(peakDesc.PositionIndex + peakDesc.Width, x_nm);

        // convert the wavelength to calibrated wavelength
        var measPeakLeftWLToNist = converter.ConvertWavelengthMeasToNist(measPeakLeftWL);
        var measPeakRightWLToNist = converter.ConvertWavelengthMeasToNist(measPeakRightWL);
        var nistPeaks = NistNeonPeaks.Where(p => p.Wavelength_Nanometer >= measPeakLeftWLToNist && p.Wavelength_Nanometer <= measPeakRightWLToNist);

        foreach (var nistPeak in nistPeaks)
        {
          result.Add((nistPeak.Wavelength_Nanometer, measPeakCenterWL, 0));
        }
      }

      return result;
    }

    private List<(double NistWL, double MeasWL, double MeasWLStdDev)> GetPeakMatchingsBasedOnPeakFittingResults((double NistWL_Left, double MeasWL_Left, double NistWL_Right, double MeasWL_Right) coarse)
    {
      var (NistWL_Left, MeasWL_Left, NistWL_Right, MeasWL_Right) = coarse;

      var converter = new WavelengthConverter
      {
        NistWL_Left = NistWL_Left,
        MeasWL_Left = MeasWL_Left,
        NistWL_Right = NistWL_Right,
        MeasWL_Right = MeasWL_Right
      };
      Converter = converter;

      var foundPeaks = _peakFittingDescriptions;

      var result = new List<(double NistWL, double MeasWL, double MeasWLStdDev)>();

      foreach (var peakDesc in foundPeaks)
      {
        var (measPeakCenterWL, measPeakCenterWLStdDev, _, _, _, _, fwhm, _) = peakDesc.FitFunction.GetPositionAreaHeightFWHMFromSinglePeakParameters(peakDesc.PeakParameter, peakDesc.PeakParameterCovariances);
        // Note that for left and right we use full width = 2 x half width
        var measPeakLeftWL = measPeakCenterWL - fwhm;
        var measPeakRightWL = measPeakCenterWL + fwhm;

        // convert the wavelength to calibrated wavelength
        var measPeakLeftWLToNist = converter.ConvertWavelengthMeasToNist(measPeakLeftWL);
        var measPeakRightWLToNist = converter.ConvertWavelengthMeasToNist(measPeakRightWL);
        var nistPeaks = NistNeonPeaks.Where(p => p.Wavelength_Nanometer >= measPeakLeftWLToNist && p.Wavelength_Nanometer <= measPeakRightWLToNist);

        foreach (var nistPeak in nistPeaks)
        {
          result.Add((nistPeak.Wavelength_Nanometer, measPeakCenterWL, measPeakCenterWLStdDev));
        }
      }

      return result;
    }



    /// <summary>
    /// Finds a coarse match bewtween the peaks in the measured Neon spectrum and the Nist table.
    /// </summary>
    /// <param name="options">The options used for calculation.</param>
    /// <param name="x">The x values of the measured Neon spectrum.</param>
    /// <param name="y">The y values of the measured Neon spectrum.</param>
    /// <returns>A tuple of wavelength (in nm): Nist wavelength and Meas wavelength at the left of the range, Nist wavelength and meas wavelength at the right of the range.</returns>
    /// The returned value is null if no peaks could be matched.
    public (double NistWL_Left, double MeasWL_Left, double NistWL_Right, double MeasWL_Right)?
    FindCoarseMatch(NeonCalibrationOptions options, double[] x, double[] y, CancellationToken cancellationToken)
    {
      var tolWL = options.Wavelength_Tolerance_nm;
      var x_nm = _xOriginal_nm = new double[x.Length];
      _assumedLaserWavelength_nm = options.LaserWavelength_Nanometer;

      ConvertXAxisToNanometer(options, x, x_nm);

      Array.Sort(x_nm, y); // Sort x-axis ascending

      var peakOptions = options.PeakFindingOptions;

      (x_nm, y, _) = peakOptions.Preprocessing.Execute(x_nm, y, null);
      _xPreprocessed_nm = x_nm;
      _yPreprocessed = y;

      var peakSearchingResults = peakOptions.PeakSearching.Execute(y, null);
      _peakSearchingDescriptions = peakSearchingResults[0].PeakDescriptions.ToList();
      _peakSearchingDescriptions.Sort((a, b) => Comparer<double>.Default.Compare(a.PositionIndex, b.PositionIndex));

      if (peakOptions.PeakFitting is { } peakFitting && peakOptions.PeakFitting is not PeakFitting.PeakFittingNone)
      {
        var peakFittingDescriptions = peakFitting.Execute(x_nm, y, peakSearchingResults, cancellationToken);
        _peakFittingDescriptions = peakFittingDescriptions[0].PeakDescriptions;
      }

      // The boundaries for the search in the NIST table
      double boundaryWLSearchLeft;
      double boundaryWLSearchRight;
      (double WL, double Height)[] measArr;

      if (_peakFittingDescriptions is not null)
      {
        // The boundaries for the search in the NIST table
        boundaryWLSearchLeft = _peakFittingDescriptions[0].PositionAreaHeightFWHM.Position - options.Wavelength_Tolerance_nm;
        boundaryWLSearchRight = _peakFittingDescriptions[^1].PositionAreaHeightFWHM.Position + options.Wavelength_Tolerance_nm;
        measArr = _peakFittingDescriptions.Select(r => (WL: r.PositionAreaHeightFWHM.Position, r.PositionAreaHeightFWHM.Height)).ToArray();

      }
      else
      {
        boundaryWLSearchLeft = RMath.InterpolateLinear(_peakSearchingDescriptions[0].PositionIndex, x_nm) - options.Wavelength_Tolerance_nm;
        boundaryWLSearchRight = RMath.InterpolateLinear(_peakSearchingDescriptions[^1].PositionIndex, x_nm) + options.Wavelength_Tolerance_nm;
        measArr = _peakSearchingDescriptions.Select(r => (WL: RMath.InterpolateLinear(r.PositionIndex, x_nm), r.Prominence)).ToArray();
      }

      // The inner boundaries are chosen so that the left search is done for 1/4 at the left of the full range,
      // and the right search is done for 1/4 at the right of the full range
      double innerBoundaryWLLeft = boundaryWLSearchLeft + (boundaryWLSearchRight - boundaryWLSearchLeft) / 4.0;
      double innerBoundaryWLRight = boundaryWLSearchRight - (boundaryWLSearchRight - boundaryWLSearchLeft) / 4.0;


      var peaks = NeonCalibration.NistNeonPeaks
                    .Where(pair => pair.Wavelength_Nanometer >= boundaryWLSearchLeft && pair.Wavelength_Nanometer <= boundaryWLSearchRight);

      // Maximum intensity of selected Nist peaks
      var maxIts = peaks.Select(pair => pair.Intensity).Max();

      // Normalize selected Nist peaks to max. intensity of 1
      var nistArr = peaks.Select(pair => (WL: pair.Wavelength_Nanometer, Its: pair.Intensity / maxIts)).ToArray();

      var left = GetNextPeakToGreaterWavelength(nistArr, innerBoundaryWLLeft);
      var right = GetNextPeakToLessWavelength(nistArr, innerBoundaryWLRight);

      var listOfCandidates = new List<(double Sum, double NistLeft, double MeasLeft, double NistRight, double MeasRight)>();
      for (int lr = 0; left.WL < right.WL - tolWL; ++lr)
      {
        // pick up peaks in measured table that are within the tolerance
        var candidatesLeft = measArr.Where(pair => pair.WL >= left.WL - tolWL && pair.WL <= left.WL + tolWL).ToArray();
        var candidatesRight = measArr.Where(pair => pair.WL >= right.WL - tolWL && pair.WL <= right.WL + tolWL).ToArray();

        foreach (var candidateLeft in candidatesLeft)
        {
          foreach (var candidateRight in candidatesRight)
          {
            // calculate the x-axis stretching that would occur when choosing so
            // the streching should be inside the range from 90% to 110%, otherwise it is not reasonable
            var r = (candidateRight.WL - candidateLeft.WL) / (right.WL - left.WL);
            if (!(r >= 0.9 && r <= 1 / 0.9))
              continue;

            // calculate correlation function

            // need a function that translates real wavelength to wavelength of the measurement system, i.e.
            // left => candLeft.WL and right => candRight.WL
            double ConvertWavelengthNistToMeas(double x)
            {
              var r = (x - left.WL) / (right.WL - left.WL);
              return (1 - r) * candidateLeft.WL + r * candidateRight.WL;
            }

            double sum = 0;
            foreach (var nistPeak in nistArr)
            {
              sum += GetIntensityAtWL(x_nm, y, ConvertWavelengthNistToMeas(nistPeak.WL));
            }

            listOfCandidates.Add((sum, left.WL, candidateLeft.WL, right.WL, candidateRight.WL));
          }
        }

        if (lr % 2 == 0)
        {
          left = GetNextPeakToGreaterWavelength(nistArr, left.WL);
        }
        else
        {
          right = GetNextPeakToLessWavelength(nistArr, right.WL);
        }
      }
      // Sort list, so that the largest sum value (the hottest candidate) is at the top of the list
      listOfCandidates.Sort((a, b) => Comparer<double>.Default.Compare(b.Sum, a.Sum));



      return listOfCandidates.Count == 0 ? null : (listOfCandidates[0].NistLeft, listOfCandidates[0].MeasLeft, listOfCandidates[0].NistRight, listOfCandidates[0].MeasRight);
    }


    private static (double WL, double Its) GetNextPeakToGreaterWavelength((double WL, double Its)[] peaks, double actualWavelength)
    {
      for (int i = 0; i < peaks.Length; ++i)
      {
        if (peaks[i].WL > actualWavelength)
          return peaks[i];
      }
      return (double.NaN, double.NaN);
    }

    private static (double WL, double Its) GetNextPeakToLessWavelength((double WL, double Its)[] peaks, double actualWavelength)
    {
      for (int i = peaks.Length - 1; i >= 0; --i)
      {
        if (peaks[i].WL < actualWavelength)
          return peaks[i];
      }
      return (double.NaN, double.NaN);
    }

    private static double GetIntensityAtWL(double[] WL, double[] Its, double actualWavelength)
    {

      for (int i = 1; i < WL.Length; ++i)
      {
        if (WL[i - 1] <= actualWavelength && WL[i] > actualWavelength)
          return Math.Max(Its[i - 1], Its[i]);
      }
      return 0;
    }

    private static void ConvertXAxisToNanometer(NeonCalibrationOptions options, double[] x, double[] x_nm)
    {
      // convert x to nanometer
      switch (options.XAxisUnit)
      {
        case XAxisUnit.RelativeShiftInverseCentimeter:
          for (int i = 0; i < x.Length; i++)
            x_nm[i] = 1 / (1 / options.LaserWavelength_Nanometer - x[i] / 1E7);
          break;
        case XAxisUnit.AbsoluteWavelengthNanometer:
          for (int i = 0; i < x.Length; i++)
            x_nm[i] = x[i];
          break;
        case XAxisUnit.AbsoluteWavenumberInverseCentimeter:
          for (int i = 0; i < x.Length; i++)
            x_nm[i] = 1E7 / x[i];
          break;
        default:
          break;
      }
    }
  }
}
