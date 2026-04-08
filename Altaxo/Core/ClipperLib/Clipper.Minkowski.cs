/*******************************************************************************
* Author    :  Angus Johnson                                                   *
* Date      :  10 October 2024                                                 *
* Website   :  https://www.angusj.com                                          *
* Copyright :  Angus Johnson 2010-2024                                         *
* Purpose   :  Minkowski Sum and Difference                                    *
* License   :  https://www.boost.org/LICENSE_1_0.txt                           *
*******************************************************************************/

#nullable enable
using System;

#if USINGZ
namespace Clipper2ZLib
#else
namespace Clipper2Lib
#endif
{
  /// <summary>
  /// Provides Minkowski sum and difference operations for integer and floating-point paths.
  /// </summary>
  public static class Minkowski
  {
    private static Paths64 MinkowskiInternal(Path64 pattern, Path64 path, bool isSum, bool isClosed)
    {
      int delta = isClosed ? 0 : 1;
      int patLen = pattern.Count, pathLen = path.Count;
      Paths64 tmp = new Paths64(pathLen);

      foreach (Point64 pathPt in path)
      {
        Path64 path2 = new Path64(patLen);
        if (isSum)
        {
          foreach (Point64 basePt in pattern)
            path2.Add(pathPt + basePt);
        }
        else
        {
          foreach (Point64 basePt in pattern)
            path2.Add(pathPt - basePt);
        }
        tmp.Add(path2);
      }

      Paths64 result = new Paths64((pathLen - delta) * patLen);
      int g = isClosed ? pathLen - 1 : 0;

      int h = patLen - 1;
      for (int i = delta; i < pathLen; i++)
      {
        for (int j = 0; j < patLen; j++)
        {
          Path64 quad = new Path64(4)
          {
            tmp[g][h], tmp[i][h], tmp[i][j], tmp[g][j]
          };
          if (!Clipper.IsPositive(quad))
            result.Add(Clipper.ReversePath(quad));
          else
            result.Add(quad);
          h = j;
        }
        g = i;
      }
      return result;
    }

    /// <summary>
    /// Calculates the Minkowski sum of two integer paths.
    /// </summary>
    /// <param name="pattern">The pattern path.</param>
    /// <param name="path">The path to which the pattern is applied.</param>
    /// <param name="isClosed">If set to <see langword="true"/>, the input path is treated as closed.</param>
    /// <returns>The resulting integer paths.</returns>
    public static Paths64 Sum(Path64 pattern, Path64 path, bool isClosed)
    {
      return Clipper.Union(MinkowskiInternal(pattern, path, true, isClosed), FillRule.NonZero);
    }

    /// <summary>
    /// Calculates the Minkowski sum of two floating-point paths.
    /// </summary>
    /// <param name="pattern">The pattern path.</param>
    /// <param name="path">The path to which the pattern is applied.</param>
    /// <param name="isClosed">If set to <see langword="true"/>, the input path is treated as closed.</param>
    /// <param name="decimalPlaces">The number of decimal places used during internal scaling.</param>
    /// <returns>The resulting floating-point paths.</returns>
    public static PathsD Sum(PathD pattern, PathD path, bool isClosed, int decimalPlaces = 2)
    {
      double scale = Math.Pow(10, decimalPlaces);
      Paths64 tmp = Clipper.Union(MinkowskiInternal(Clipper.ScalePath64(pattern, scale),
        Clipper.ScalePath64(path, scale), true, isClosed), FillRule.NonZero);
      return Clipper.ScalePathsD(tmp, 1 / scale);
    }

    /// <summary>
    /// Calculates the Minkowski difference of two integer paths.
    /// </summary>
    /// <param name="pattern">The pattern path.</param>
    /// <param name="path">The path from which the pattern is subtracted.</param>
    /// <param name="isClosed">If set to <see langword="true"/>, the input path is treated as closed.</param>
    /// <returns>The resulting integer paths.</returns>
    public static Paths64 Diff(Path64 pattern, Path64 path, bool isClosed)
    {
      return Clipper.Union(MinkowskiInternal(pattern, path, false, isClosed), FillRule.NonZero);
    }

    /// <summary>
    /// Calculates the Minkowski difference of two floating-point paths.
    /// </summary>
    /// <param name="pattern">The pattern path.</param>
    /// <param name="path">The path from which the pattern is subtracted.</param>
    /// <param name="isClosed">If set to <see langword="true"/>, the input path is treated as closed.</param>
    /// <param name="decimalPlaces">The number of decimal places used during internal scaling.</param>
    /// <returns>The resulting floating-point paths.</returns>
    public static PathsD Diff(PathD pattern, PathD path, bool isClosed, int decimalPlaces = 2)
    {
      double scale = Math.Pow(10, decimalPlaces);
      Paths64 tmp = Clipper.Union(MinkowskiInternal(Clipper.ScalePath64(pattern, scale),
        Clipper.ScalePath64(path, scale), false, isClosed), FillRule.NonZero);
      return Clipper.ScalePathsD(tmp, 1 / scale);
    }

  }

} // namespace
