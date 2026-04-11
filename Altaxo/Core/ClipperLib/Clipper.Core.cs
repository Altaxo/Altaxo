/*******************************************************************************
* Author    :  Angus Johnson                                                   *
* Date      :  12 October 2025                                                 *
* Website   :  https://www.angusj.com                                          *
* Copyright :  Angus Johnson 2010-2025                                         *
* Purpose   :  Core structures and functions for the Clipper Library           *
* License   :  https://www.boost.org/LICENSE_1_0.txt                           *
*******************************************************************************/

#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if USINGZ
namespace Clipper2ZLib
#else
namespace Clipper2Lib
#endif
{
  /// <summary>
  /// Defines a point in 2D space using 64-bit integer coordinates.
  /// </summary>
  public struct Point64
  {
    /// <summary>
    /// The X coordinate of the point.
    /// </summary>
    public long X;
    /// <summary>
    /// The Y coordinate of the point.
    /// </summary>
    public long Y;

#if USINGZ
    /// <summary>
    /// The Z coordinate of the point (optional, using if USINGZ defined).
    /// </summary>
    public long Z;
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="Point64"/> struct.
    /// </summary>
    /// <param name="pt">The point to copy.</param>
    public Point64(Point64 pt)
    {
      X = pt.X;
      Y = pt.Y;
#if USINGZ
      Z = pt.Z;
#endif
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Point64"/> struct.
    /// </summary>
    /// <param name="pt">The point to copy.</param>
    /// <param name="scale">The scale factor for the coordinates.</param>
    public Point64(Point64 pt, double scale)
    {
      X = (long)Math.Round(pt.X * scale, MidpointRounding.AwayFromZero);
      Y = (long)Math.Round(pt.Y * scale, MidpointRounding.AwayFromZero);
#if USINGZ
      Z = (long) Math.Round(pt.Z * scale, MidpointRounding.AwayFromZero);
#endif
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Point64"/> struct.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
#if USINGZ
    /// <param name="z">The Z coordinate (optional, default is 0).</param>
#endif
    public Point64(long x, long y
#if USINGZ
      , long z = 0
#endif
    )
    {
      X = x;
      Y = y;
#if USINGZ
      Z = z;
#endif
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Point64"/> struct.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
#if USINGZ
   /// <param name="z">The Z coordinate (optional, default is 0.0).</param>
#endif
    public Point64(double x, double y
#if USINGZ
      , double z = 0.0
#endif
    )
    {
      X = (long)Math.Round(x, MidpointRounding.AwayFromZero);
      Y = (long)Math.Round(y, MidpointRounding.AwayFromZero);
#if USINGZ
      Z = (long) Math.Round(z, MidpointRounding.AwayFromZero);
#endif
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Point64"/> struct from a <see cref="PointD"/> instance.
    /// </summary>
    /// <param name="pt">The <see cref="PointD"/> instance to copy.</param>
    public Point64(PointD pt)
    {
      X = (long)Math.Round(pt.x, MidpointRounding.AwayFromZero);
      Y = (long)Math.Round(pt.y, MidpointRounding.AwayFromZero);
#if USINGZ
      Z = pt.z;
#endif
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Point64"/> struct from a <see cref="PointD"/> instance, applying a scale.
    /// </summary>
    /// <param name="pt">The <see cref="PointD"/> instance to copy.</param>
    /// <param name="scale">The scale factor for the coordinates.</param>
    public Point64(PointD pt, double scale)
    {
      X = (long)Math.Round(pt.x * scale, MidpointRounding.AwayFromZero);
      Y = (long)Math.Round(pt.y * scale, MidpointRounding.AwayFromZero);
#if USINGZ
      Z = pt.z;
#endif
    }

    /// <summary>
    /// Determines if two <see cref="Point64"/> instances are equal.
    /// </summary>
    /// <param name="lhs">The first point.</param>
    /// <param name="rhs">The second point.</param>
    /// <returns><see langword="true"/> if the points are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(Point64 lhs, Point64 rhs)
    {
      return lhs.X == rhs.X && lhs.Y == rhs.Y;
    }

    /// <summary>
    /// Determines if two <see cref="Point64"/> instances are not equal.
    /// </summary>
    /// <param name="lhs">The first point.</param>
    /// <param name="rhs">The second point.</param>
    /// <returns><see langword="true"/> if the points are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(Point64 lhs, Point64 rhs)
    {
      return lhs.X != rhs.X || lhs.Y != rhs.Y;
    }

    /// <summary>
    /// Adds two <see cref="Point64"/> instances.
    /// </summary>
    /// <param name="lhs">The first point.</param>
    /// <param name="rhs">The second point.</param>
    /// <returns>The sum of the points.</returns>
    public static Point64 operator +(Point64 lhs, Point64 rhs)
    {
      return new Point64(lhs.X + rhs.X, lhs.Y + rhs.Y
#if USINGZ
        , lhs.Z + rhs.Z
#endif
      );
    }

    /// <summary>
    /// Subtracts one <see cref="Point64"/> instance from another.
    /// </summary>
    /// <param name="lhs">The point to subtract from.</param>
    /// <param name="rhs">The point to subtract.</param>
    /// <returns>The difference of the points.</returns>
    public static Point64 operator -(Point64 lhs, Point64 rhs)
    {
      return new Point64(lhs.X - rhs.X, lhs.Y - rhs.Y
#if USINGZ
        , lhs.Z - rhs.Z
#endif
      );
    }

    /// <summary>
    /// Returns a string representation of the point.
    /// </summary>
    /// <returns>A string in the format "X,Y[,(Z)]", where (Z) is optional.</returns>
    public readonly override string ToString()
    {
      // nb: trailing space
#if USINGZ
      return $"{X},{Y},{Z} ";
#else
      return $"{X},{Y} ";
#endif

    }

    /// <summary>
    /// Determines if the specified object is equal to the current <see cref="Point64"/>.
    /// </summary>
    /// <param name="obj">The object to compare with the current point.</param>
    /// <returns><see langword="true"/> if the object is a <see cref="Point64"/> and is equal to the current point; otherwise, <see langword="false"/>.</returns>
    public readonly override bool Equals(object? obj)
    {
      if (obj != null && obj is Point64 p)
        return this == p;
      return false;
    }

    /// <summary>
    /// Returns the hash code for the current <see cref="Point64"/>.
    /// </summary>
    /// <returns>A hash code for the current <see cref="Point64"/>.</returns>
    public readonly override int GetHashCode()
    {
      return HashCode.Combine(X, Y); //#599
    }

  }

  /// <summary>
  /// Defines a point in 2D space using double-precision floating-point coordinates.
  /// </summary>
  public struct PointD
  {
    /// <summary>
    /// The X coordinate of the point.
    /// </summary>
    public double x;
    /// <summary>
    /// The Y coordinate of the point.
    /// </summary>
    public double y;

#if USINGZ
    /// <summary>
    /// The Z coordinate of the point (optional, using if USINGZ defined).
    /// </summary>
    public long z;
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="PointD"/> struct.
    /// </summary>
    /// <param name="pt">The point to copy.</param>
    public PointD(PointD pt)
    {
      x = pt.x;
      y = pt.y;
#if USINGZ
      z = pt.z;
#endif
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PointD"/> struct from a <see cref="Point64"/> instance.
    /// </summary>
    /// <param name="pt">The <see cref="Point64"/> instance to copy.</param>
    public PointD(Point64 pt)
    {
      x = pt.X;
      y = pt.Y;
#if USINGZ
      z = pt.Z;
#endif
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PointD"/> struct from a <see cref="Point64"/> instance, applying a scale.
    /// </summary>
    /// <param name="pt">The <see cref="Point64"/> instance to copy.</param>
    /// <param name="scale">The scale factor for the coordinates.</param>
    public PointD(Point64 pt, double scale)
    {
      x = pt.X * scale;
      y = pt.Y * scale;
#if USINGZ
      z = pt.Z;
#endif
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PointD"/> class from another <see cref="PointD"/> instance, applying a scale.
    /// </summary>
    /// <param name="pt">The <see cref="PointD"/> instance to copy.</param>
    /// <param name="scale">The scale factor for the coordinates.</param>
    public PointD(PointD pt, double scale)
    {
      x = pt.x * scale;
      y = pt.y * scale;
#if USINGZ
      z = pt.z;
#endif
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PointD"/> struct.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
#if USINGZ
   /// <param name="z">The Z coordinate (optional, default is 0).</param>
#endif
    public PointD(long x, long y
#if USINGZ
      , long z = 0
#endif
    )
    {
      this.x = x;
      this.y = y;
#if USINGZ
      this.z = z;
#endif
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PointD"/> struct.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
#if USINGZ
   /// <param name="z">The Z coordinate (optional, default is 0.0).</param>
#endif
    public PointD(double x, double y
#if USINGZ
      , long z = 0
#endif
    )
    {
      this.x = x;
      this.y = y;
#if USINGZ
      this.z = z;
#endif
    }

    /// <summary>
    /// Returns a string representation of the point with the specified precision.
    /// </summary>
    /// <param name="precision">The number of decimal places.</param>
    /// <returns>A string representation of the point.</returns>
    public readonly string ToString(int precision = 2)
    {
#if USINGZ
      return string.Format($"{{0:F{precision}}},{{1:F{precision}}},{{2:D}}", x,y,z);
#else
      return string.Format($"{{0:F{precision}}},{{1:F{precision}}}", x, y);
#endif
    }

    /// <summary>
    /// Determines if two <see cref="PointD"/> instances are equal.
    /// </summary>
    /// <param name="lhs">The first point.</param>
    /// <param name="rhs">The second point.</param>
    /// <returns><see langword="true"/> if the points are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(PointD lhs, PointD rhs)
    {
      return InternalClipper.IsAlmostZero(lhs.x - rhs.x) &&
        InternalClipper.IsAlmostZero(lhs.y - rhs.y);
    }

    /// <summary>
    /// Determines if two <see cref="PointD"/> instances are not equal.
    /// </summary>
    /// <param name="lhs">The first point.</param>
    /// <param name="rhs">The second point.</param>
    /// <returns><see langword="true"/> if the points are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(PointD lhs, PointD rhs)
    {
      return !InternalClipper.IsAlmostZero(lhs.x - rhs.x) ||
        !InternalClipper.IsAlmostZero(lhs.y - rhs.y);
    }

    /// <summary>
    /// Negates the coordinates of the point.
    /// </summary>
    public void Negate() { x = -x; y = -y; }

    /// <summary>
    /// Returns a string representation of the point.
    /// </summary>
    /// <returns>A string in the format "X,Y", with no trailing comma.</returns>
    public override string ToString()
    {
      return $"{x},{y}";
    }

    /// <summary>
    /// Determines if the specified object is equal to the current <see cref="PointD"/>.
    /// </summary>
    /// <param name="obj">The object to compare with the current point.</param>
    /// <returns><see langword="true"/> if the object is a <see cref="PointD"/> and is equal to the current point; otherwise, <see langword="false"/>.</returns>
    public readonly override bool Equals(object? obj)
    {
      if (obj != null && obj is PointD p)
        return this == p;
      return false;
    }

    /// <summary>
    /// Returns the hash code for the current <see cref="PointD"/>.
    /// </summary>
    /// <returns>A hash code for the current <see cref="PointD"/>.</returns>
    public readonly override int GetHashCode()
    {
      return HashCode.Combine(x, y); //#599
    }

  }

  /// <summary>
  /// Defines a rectangular area in 2D space using 64-bit integer coordinates.
  /// </summary>
  public struct Rect64
  {
    /// <summary>
    /// The X coordinate of the left side of the rectangle.
    /// </summary>
    public long left;
    /// <summary>
    /// The Y coordinate of the top side of the rectangle.
    /// </summary>
    public long top;
    /// <summary>
    /// The X coordinate of the right side of the rectangle.
    /// </summary>
    public long right;
    /// <summary>
    /// The Y coordinate of the bottom side of the rectangle.
    /// </summary>
    public long bottom;

    /// <summary>
    /// Initializes a new instance of the <see cref="Rect64"/> struct.
    /// </summary>
    /// <param name="l">The X coordinate of the left side.</param>
    /// <param name="t">The Y coordinate of the top side.</param>
    /// <param name="r">The X coordinate of the right side.</param>
    /// <param name="b">The Y coordinate of the bottom side.</param>
    public Rect64(long l, long t, long r, long b)
    {
      left = l;
      top = t;
      right = r;
      bottom = b;
    }

    /// <summary>
    /// Initializes a new invalid or empty instance of the <see cref="Rect64"/> struct.
    /// </summary>
    /// <param name="isValid"><see langword="true"/> to create a valid rectangle with zero size; <see langword="false"/> to create an invalid rectangle.</param>
    public Rect64(bool isValid)
    {
      if (isValid)
      {
        left = 0; top = 0; right = 0; bottom = 0;
      }
      else
      {
        left = long.MaxValue; top = long.MaxValue;
        right = long.MinValue; bottom = long.MinValue;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Rect64"/> struct from another rectangle.
    /// </summary>
    /// <param name="rec">The rectangle to copy.</param>
    public Rect64(Rect64 rec)
    {
      left = rec.left;
      top = rec.top;
      right = rec.right;
      bottom = rec.bottom;
    }

    /// <summary>
    /// Gets or sets the width of the rectangle.
    /// </summary>
    public long Width
    {
      readonly get => right - left;
      set => right = left + value;
    }

    /// <summary>
    /// Gets or sets the height of the rectangle.
    /// </summary>
    public long Height
    {
      readonly get => bottom - top;
      set => bottom = top + value;
    }

    /// <summary>
    /// Determines if the rectangle is empty (has no area).
    /// </summary>
    /// <returns><see langword="true"/> if the rectangle is empty; otherwise, <see langword="false"/>.</returns>
    public readonly bool IsEmpty()
    {
      return bottom <= top || right <= left;
    }

    /// <summary>
    /// Determines if the rectangle is valid (not equal to <see cref="long.MaxValue"/>).
    /// </summary>
    /// <returns><see langword="true"/> if the rectangle is valid; otherwise, <see langword="false"/>.</returns>
    public readonly bool IsValid()
    {
      return left < long.MaxValue;
    }

    /// <summary>
    /// Calculates the midpoint of the rectangle.
    /// </summary>
    /// <returns>A <see cref="Point64"/> representing the midpoint of the rectangle.</returns>
    public readonly Point64 MidPoint()
    {
      return new Point64((left + right) / 2, (top + bottom) / 2);
    }

    /// <summary>
    /// Determines if a point is inside the rectangle.
    /// </summary>
    /// <param name="pt">The point to check.</param>
    /// <returns><see langword="true"/> if the point is inside the rectangle; otherwise, <see langword="false"/>.</returns>
    public readonly bool Contains(Point64 pt)
    {
      return pt.X > left && pt.X < right &&
        pt.Y > top && pt.Y < bottom;
    }

    /// <summary>
    /// Determines if another rectangle is completely contained within this rectangle.
    /// </summary>
    /// <param name="rec">The rectangle to check.</param>
    /// <returns><see langword="true"/> if the other rectangle is contained; otherwise, <see langword="false"/>.</returns>
    public readonly bool Contains(Rect64 rec)
    {
      return rec.left >= left && rec.right <= right &&
        rec.top >= top && rec.bottom <= bottom;
    }

    /// <summary>
    /// Determines if another rectangle intersects with this rectangle.
    /// </summary>
    /// <param name="rec">The rectangle to check.</param>
    /// <returns><see langword="true"/> if the rectangles intersect; otherwise, <see langword="false"/>.</returns>
    public readonly bool Intersects(Rect64 rec)
    {
      return (Math.Max(left, rec.left) <= Math.Min(right, rec.right)) &&
        (Math.Max(top, rec.top) <= Math.Min(bottom, rec.bottom));
    }

    /// <summary>
    /// Converts the rectangle to a <see cref="Path64"/> representation.
    /// </summary>
    /// <returns>A <see cref="Path64"/> representing the outline of the rectangle.</returns>
    public readonly Path64 AsPath()
    {
      Path64 result = new Path64(4)
      {
        new Point64(left, top),
        new Point64(right, top),
        new Point64(right, bottom),
        new Point64(left, bottom)
      };
      return result;
    }

  }

  /// <summary>
  /// Defines a rectangular area in 2D space using double-precision floating-point coordinates.
  /// </summary>
  public struct RectD
  {
    /// <summary>
    /// The X coordinate of the left side of the rectangle.
    /// </summary>
    public double left;
    /// <summary>
    /// The Y coordinate of the top side of the rectangle.
    /// </summary>
    public double top;
    /// <summary>
    /// The X coordinate of the right side of the rectangle.
    /// </summary>
    public double right;
    /// <summary>
    /// The Y coordinate of the bottom side of the rectangle.
    /// </summary>
    public double bottom;

    /// <summary>
    /// Initializes a new instance of the <see cref="RectD"/> struct.
    /// </summary>
    /// <param name="l">The X coordinate of the left side.</param>
    /// <param name="t">The Y coordinate of the top side.</param>
    /// <param name="r">The X coordinate of the right side.</param>
    /// <param name="b">The Y coordinate of the bottom side.</param>
    public RectD(double l, double t, double r, double b)
    {
      left = l;
      top = t;
      right = r;
      bottom = b;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RectD"/> struct from another rectangle.
    /// </summary>
    /// <param name="rec">The rectangle to copy.</param>
    public RectD(RectD rec)
    {
      left = rec.left;
      top = rec.top;
      right = rec.right;
      bottom = rec.bottom;
    }

    /// <summary>
    /// Initializes a new invalid or empty instance of the <see cref="RectD"/> struct.
    /// </summary>
    /// <param name="isValid"><see langword="true"/> to create a valid rectangle with zero size; <see langword="false"/> to create an invalid rectangle.</param>
    public RectD(bool isValid)
    {
      if (isValid)
      {
        left = 0; top = 0; right = 0; bottom = 0;
      }
      else
      {
        left = double.MaxValue; top = double.MaxValue;
        right = -double.MaxValue; bottom = -double.MaxValue;
      }
    }
    /// <summary>
    /// Gets or sets the width of the rectangle.
    /// </summary>
    public double Width
    {
      readonly get => right - left;
      set => right = left + value;
    }

    /// <summary>
    /// Gets or sets the height of the rectangle.
    /// </summary>
    public double Height
    {
      readonly get => bottom - top;
      set => bottom = top + value;
    }

    /// <summary>
    /// Determines if the rectangle is empty (has no area).
    /// </summary>
    /// <returns><see langword="true"/> if the rectangle is empty; otherwise, <see langword="false"/>.</returns>
    public readonly bool IsEmpty()
    {
      return bottom <= top || right <= left;
    }

    /// <summary>
    /// Calculates the midpoint of the rectangle.
    /// </summary>
    /// <returns>A <see cref="PointD"/> representing the midpoint of the rectangle.</returns>
    public readonly PointD MidPoint()
    {
      return new PointD((left + right) / 2, (top + bottom) / 2);
    }

    /// <summary>
    /// Determines if a point is inside the rectangle.
    /// </summary>
    /// <param name="pt">The point to check.</param>
    /// <returns><see langword="true"/> if the point is inside the rectangle; otherwise, <see langword="false"/>.</returns>
    public readonly bool Contains(PointD pt)
    {
      return pt.x > left && pt.x < right &&
        pt.y > top && pt.y < bottom;
    }

    /// <summary>
    /// Determines if another rectangle is completely contained within this rectangle.
    /// </summary>
    /// <param name="rec">The rectangle to check.</param>
    /// <returns><see langword="true"/> if the other rectangle is contained; otherwise, <see langword="false"/>.</returns>
    public readonly bool Contains(RectD rec)
    {
      return rec.left >= left && rec.right <= right &&
        rec.top >= top && rec.bottom <= bottom;
    }

    /// <summary>
    /// Determines if another rectangle intersects with this rectangle.
    /// </summary>
    /// <param name="rec">The rectangle to check.</param>
    /// <returns><see langword="true"/> if the rectangles intersect; otherwise, <see langword="false"/>.</returns>
    public readonly bool Intersects(RectD rec)
    {
      return (Math.Max(left, rec.left) < Math.Min(right, rec.right)) &&
        (Math.Max(top, rec.top) < Math.Min(bottom, rec.bottom));
    }

    /// <summary>
    /// Converts the rectangle to a <see cref="PathD"/> representation.
    /// </summary>
    /// <returns>A <see cref="PathD"/> representing the outline of the rectangle.</returns>
    public readonly PathD AsPath()
    {
      PathD result = new PathD(4)
      {
        new PointD(left, top),
        new PointD(right, top),
        new PointD(right, bottom),
        new PointD(left, bottom)
      };
      return result;
    }

  }

  /// <summary>
  /// Represents a series of points forming a path in 2D space using 64-bit integer coordinates.
  /// </summary>
  public class Path64 : List<Point64>
  {
    /// <summary>
    /// Initializes a new, empty instance of the <see cref="Path64"/> class.
    /// </summary>
    public Path64() : base() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="Path64"/> class with a specified capacity.
    /// </summary>
    /// <param name="capacity">The initial capacity of the list.</param>
    public Path64(int capacity = 0) : base(capacity) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="Path64"/> class from an existing collection of points.
    /// </summary>
    /// <param name="path">The collection of points to copy.</param>
    public Path64(IEnumerable<Point64> path) : base(path) { }
    /// <summary>
    /// Returns a string representation of the path.
    /// </summary>
    /// <returns>A string listing the points in the path.</returns>
    public override string ToString()
    {
      string s = "";
      foreach (Point64 p in this)
        s = s + p.ToString() + ", ";
      if (s != "") s = s.Remove(s.Length - 2);
      return s;
    }
  }

  /// <summary>
  /// Represents a series of paths, each formed by a series of points in 2D space using 64-bit integer coordinates.
  /// </summary>
  public class Paths64 : List<Path64>
  {
    /// <summary>
    /// Initializes a new, empty instance of the <see cref="Paths64"/> class.
    /// </summary>
    public Paths64() : base() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="Paths64"/> class with a specified capacity.
    /// </summary>
    /// <param name="capacity">The initial capacity of the list.</param>
    public Paths64(int capacity = 0) : base(capacity) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="Paths64"/> class from an existing collection of paths.
    /// </summary>
    /// <param name="paths">The collection of paths to copy.</param>
    public Paths64(IEnumerable<Path64> paths) : base(paths) { }
    /// <summary>
    /// Returns a string representation of the collection of paths.
    /// </summary>
    /// <returns>A string listing each path in the collection.</returns>
    public override string ToString()
    {
      string s = "";
      foreach (Path64 p in this)
        s = s + p + "\n";
      return s;
    }
  }

  /// <summary>
  /// Represents a series of points forming a path in 2D space using double-precision floating-point coordinates.
  /// </summary>
  public class PathD : List<PointD>
  {
    /// <summary>
    /// Initializes a new, empty instance of the <see cref="PathD"/> class.
    /// </summary>
    public PathD() : base() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="PathD"/> class with a specified capacity.
    /// </summary>
    /// <param name="capacity">The initial capacity of the list.</param>
    public PathD(int capacity = 0) : base(capacity) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="PathD"/> class from an existing collection of points.
    /// </summary>
    /// <param name="path">The collection of points to copy.</param>
    public PathD(IEnumerable<PointD> path) : base(path) { }
    /// <summary>
    /// Returns a string representation of the path with the specified precision.
    /// </summary>
    /// <param name="precision">The number of decimal places.</param>
    /// <returns>A string representation of the path.</returns>
    public string ToString(int precision = 2)
    {
      string s = "";
      foreach (PointD p in this)
        s = s + p.ToString(precision) + ", ";
      if (s != "") s = s.Remove(s.Length - 2);
      return s;
    }
  }

  /// <summary>
  /// Represents a series of paths, each formed by a series of points in 2D space using double-precision floating-point coordinates.
  /// </summary>
  public class PathsD : List<PathD>
  {
    /// <summary>
    /// Initializes a new, empty instance of the <see cref="PathsD"/> class.
    /// </summary>
    public PathsD() : base() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="PathsD"/> class with a specified capacity.
    /// </summary>
    /// <param name="capacity">The initial capacity of the list.</param>
    public PathsD(int capacity = 0) : base(capacity) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="PathsD"/> class from an existing collection of paths.
    /// </summary>
    /// <param name="paths">The collection of paths to copy.</param>
    public PathsD(IEnumerable<PathD> paths) : base(paths) { }
    /// <summary>
    /// Returns a string representation of the collection of paths with the specified precision.
    /// </summary>
    /// <param name="precision">The number of decimal places.</param>
    /// <returns>A string listing each path in the collection.</returns>
    public string ToString(int precision = 2)
    {
      string s = "";
      foreach (PathD p in this)
        s = s + p.ToString(precision) + "\n";
      return s;
    }
  }

  // Note: all clipping operations except for Difference are commutative.
  /// <summary>
  /// Specifies the type of clipping operation to perform.
  /// </summary>
  public enum ClipType
  {
    /// <summary>
    /// No clipping.
    /// </summary>
    NoClip,
    /// <summary>
    /// Intersection clipping (common area).
    /// </summary>
    Intersection,
    /// <summary>
    /// Union clipping (combined area).
    /// </summary>
    Union,
    /// <summary>
    /// Difference clipping (subtracting second shape from first).
    /// </summary>
    Difference,
    /// <summary>
    /// Xor clipping (exclusive or, areas not overlapping).
    /// </summary>
    Xor
  }

  /// <summary>
  /// Specifies the type of path (subject or clip).
  /// </summary>
  public enum PathType
  {
    /// <summary>
    /// The path is a subject path.
    /// </summary>
    Subject,
    /// <summary>
    /// The path is a clipping path.
    /// </summary>
    Clip
  }

  // By far the most widely used filling rules for polygons are EvenOdd
  // and NonZero, sometimes called Alternate and Winding respectively.
  // https://en.wikipedia.org/wiki/Nonzero-rule
  /// <summary>
  /// Specifies the filling rule for polygons.
  /// </summary>
  public enum FillRule
  {
    /// <summary>
    /// Even-odd filling rule.
    /// </summary>
    EvenOdd,
    /// <summary>
    /// Non-zero winding filling rule.
    /// </summary>
    NonZero,
    /// <summary>
    /// Positive winding number rule.
    /// </summary>
    Positive,
    /// <summary>
    /// Negative winding number rule.
    /// </summary>
    Negative
  }

  /// <summary>
  /// Results of the PointInPolygon function.
  /// </summary>
  internal enum PipResult
  {
    /// <summary>
    /// The point is inside the polygon.
    /// </summary>
    Inside,
    /// <summary>
    /// The point is outside the polygon.
    /// </summary>
    Outside,
    /// <summary>
    /// The point is on the edge of the polygon.
    /// </summary>
    OnEdge
  }

  /// <summary>
  /// Provides internal clipping functions and constants.
  /// </summary>
  public static class InternalClipper
  {
    /// <summary>
    /// The largest supported 64-bit signed integer value.
    /// </summary>
    internal const long MaxInt64 = 9223372036854775807;

    /// <summary>
    /// The largest coordinate value considered valid during clipping.
    /// </summary>
    internal const long MaxCoord = MaxInt64 / 4;

    /// <summary>
    /// The largest coordinate value considered valid during floating-point operations.
    /// </summary>
    internal const double max_coord = MaxCoord;

    /// <summary>
    /// The smallest coordinate value considered valid during floating-point operations.
    /// </summary>
    internal const double min_coord = -MaxCoord;

    /// <summary>
    /// The sentinel value used to mark invalid 64-bit coordinates.
    /// </summary>
    internal const long Invalid64 = MaxInt64;

    /// <summary>
    /// The tolerance used for floating-point comparisons.
    /// </summary>
    internal const double floatingPointTolerance = 1E-12;

    /// <summary>
    /// The default minimum edge length used during path cleanup.
    /// </summary>
    internal const double defaultMinimumEdgeLength = 0.1;

    private static readonly string
      precision_range_error = "Error: Precision is out of range.";

    /// <summary>
    /// Calculates the cross product of three points.
    /// </summary>
    /// <param name="pt1">The first point.</param>
    /// <param name="pt2">The second point.</param>
    /// <param name="pt3">The third point.</param>
    /// <returns>The cross product value.</returns>
    public static double CrossProduct(Point64 pt1, Point64 pt2, Point64 pt3)
    {
      // typecast to double to avoid potential int overflow
      return ((double)(pt2.X - pt1.X) * (pt3.Y - pt2.Y) -
              (double)(pt2.Y - pt1.Y) * (pt3.X - pt2.X));
    }

    /// <summary>
    /// Determines the sign of the cross product of two vectors.
    /// </summary>
    /// <param name="pt1">The base point of the first vector.</param>
    /// <param name="pt2">The endpoint of the first vector and base point of the second vector.</param>
    /// <param name="pt3">The endpoint of the second vector.</param>
    /// <returns>-1, 0, or 1 indicating the sign of the cross product.</returns>
    public static int CrossProductSign(Point64 pt1, Point64 pt2, Point64 pt3)
    {
      long a = pt2.X - pt1.X;
      long b = pt3.Y - pt2.Y;
      long c = pt2.Y - pt1.Y;
      long d = pt3.X - pt2.X;
      UInt128Struct ab = MultiplyUInt64((ulong)Math.Abs(a), (ulong)Math.Abs(b));
      UInt128Struct cd = MultiplyUInt64((ulong)Math.Abs(c), (ulong)Math.Abs(d));
      var signAB = TriSign(a) * TriSign(b);
      var signCD = TriSign(c) * TriSign(d);

      if (signAB == signCD)
      {
        int result;
        if (ab.hi64 == cd.hi64)
        {
          if (ab.lo64 == cd.lo64) return 0;
          result = (ab.lo64 > cd.lo64) ? 1 : -1;
        }
        else result = (ab.hi64 > cd.hi64) ? 1 : -1;
        return (signAB > 0) ? result : -result;
      }
      return (signAB > signCD) ? 1 : -1;
    }

#if USINGZ
    /// <summary>
    /// Sets the Z coordinate for each point in a path.
    /// </summary>
    /// <param name="path">The path whose points will have their Z coordinate set.</param>
    /// <param name="Z">The Z coordinate value.</param>
    /// <returns>A new <see cref="Path64"/> with the Z coordinate set for each point.</returns>
    public static Path64 SetZ(Path64 path, long Z)
    {
      Path64 result = new Path64(path.Count);
      foreach (Point64 pt in path) result.Add(new Point64(pt.X, pt.Y, Z));
      return result;
    }
#endif

    /// <summary>
    /// Validates that the supplied decimal precision is within the supported range.
    /// </summary>
    /// <param name="precision">The decimal precision to validate.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void CheckPrecision(int precision)
    {
      if (precision < -8 || precision > 8)
        throw new Exception(precision_range_error);
    }

    /// <summary>
    /// Determines whether the specified floating-point value is effectively zero.
    /// </summary>
    /// <param name="value">The value to test.</param>
    /// <returns><see langword="true"/> if the value is within the floating-point tolerance; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsAlmostZero(double value)
    {
      return (Math.Abs(value) <= floatingPointTolerance);
    }

    /// <summary>
    /// Returns the sign of the supplied integer value.
    /// </summary>
    /// <param name="x">The value to evaluate.</param>
    /// <returns><c>-1</c>, <c>0</c>, or <c>1</c> depending on the sign of <paramref name="x"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int TriSign(long x) // returns 0, 1 or -1
    {
      return (x < 0) ? -1 : (x > 0) ? 1 : 0;
    }

    /// <summary>
    /// A structure representing a 128-bit unsigned integer as two 64-bit parts.
    /// </summary>
    public struct UInt128Struct
    {
      /// <summary>
      /// The low 64 bits of the 128-bit integer.
      /// </summary>
      public ulong lo64;

      /// <summary>
      /// The high 64 bits of the 128-bit integer.
      /// </summary>
      public ulong hi64;
    }

    /// <summary>
    /// Multiplies two 64-bit unsigned integers and returns the 128-bit result as two 64-bit parts.
    /// </summary>
    /// <param name="a">The first multiplicand.</param>
    /// <param name="b">The second multiplicand.</param>
    /// <returns>The <see cref="UInt128Struct"/> representing the 128-bit product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt128Struct MultiplyUInt64(ulong a, ulong b) // #834,#835
    {
      ulong x1 = (a & 0xFFFFFFFF) * (b & 0xFFFFFFFF);
      ulong x2 = (a >> 32) * (b & 0xFFFFFFFF) + (x1 >> 32);
      ulong x3 = (a & 0xFFFFFFFF) * (b >> 32) + (x2 & 0xFFFFFFFF);
      UInt128Struct result;
      result.lo64 = (x3 & 0xFFFFFFFF) << 32 | (x1 & 0xFFFFFFFF);
      result.hi64 = (a >> 32) * (b >> 32) + (x2 >> 32) + (x3 >> 32);
      return result;
    }

    /// <summary>
    /// Determines whether two products are equal without overflowing 64-bit arithmetic.
    /// </summary>
    /// <param name="a">The first factor of the first product.</param>
    /// <param name="b">The second factor of the first product.</param>
    /// <param name="c">The first factor of the second product.</param>
    /// <param name="d">The second factor of the second product.</param>
    /// <returns><see langword="true"/> if <paramref name="a"/> multiplied by <paramref name="b"/> equals <paramref name="c"/> multiplied by <paramref name="d"/>; otherwise, <see langword="false"/>.</returns>
    // returns true if (and only if) a * b == c * d
    internal static bool ProductsAreEqual(long a, long b, long c, long d)
    {
      // nb: unsigned values will be needed for CalcOverflowCarry()
      ulong absA = (ulong)Math.Abs(a);
      ulong absB = (ulong)Math.Abs(b);
      ulong absC = (ulong)Math.Abs(c);
      ulong absD = (ulong)Math.Abs(d);

      UInt128Struct mul_ab = MultiplyUInt64(absA, absB);
      UInt128Struct mul_cd = MultiplyUInt64(absC, absD);

      // nb: it's important to differentiate 0 values here from other values
      int sign_ab = TriSign(a) * TriSign(b);
      int sign_cd = TriSign(c) * TriSign(d);

      return mul_ab.lo64 == mul_cd.lo64 && mul_ab.hi64 == mul_cd.hi64 && sign_ab == sign_cd;
    }

    /// <summary>
    /// Determines whether three integer points are collinear.
    /// </summary>
    /// <param name="pt1">The first point.</param>
    /// <param name="sharedPt">The shared middle point.</param>
    /// <param name="pt2">The third point.</param>
    /// <returns><see langword="true"/> if the points are collinear; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsCollinear(Point64 pt1, Point64 sharedPt, Point64 pt2)
    {
      long a = sharedPt.X - pt1.X;
      long b = pt2.Y - sharedPt.Y;
      long c = sharedPt.Y - pt1.Y;
      long d = pt2.X - sharedPt.X;
      // When checking for collinearity with very large coordinate values
      // then ProductsAreEqual is more accurate than using CrossProduct.
      return ProductsAreEqual(a, b, c, d);
    }

    /// <summary>
    /// Calculates the dot product formed by three integer points.
    /// </summary>
    /// <param name="pt1">The first point.</param>
    /// <param name="pt2">The shared middle point.</param>
    /// <param name="pt3">The third point.</param>
    /// <returns>The dot product value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static double DotProduct(Point64 pt1, Point64 pt2, Point64 pt3)
    {
      // typecast to double to avoid potential int overflow
      return ((double)(pt2.X - pt1.X) * (pt3.X - pt2.X) +
              (double)(pt2.Y - pt1.Y) * (pt3.Y - pt2.Y));
    }

    /// <summary>
    /// Calculates the cross product of two floating-point vectors.
    /// </summary>
    /// <param name="vec1">The first vector.</param>
    /// <param name="vec2">The second vector.</param>
    /// <returns>The cross product value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static double CrossProduct(PointD vec1, PointD vec2)
    {
      return (vec1.y * vec2.x - vec2.y * vec1.x);
    }

    /// <summary>
    /// Calculates the dot product of two floating-point vectors.
    /// </summary>
    /// <param name="vec1">The first vector.</param>
    /// <param name="vec2">The second vector.</param>
    /// <returns>The dot product value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static double DotProduct(PointD vec1, PointD vec2)
    {
      return (vec1.x * vec2.x + vec1.y * vec2.y);
    }

    /// <summary>
    /// Converts a floating-point value to a 64-bit integer when it is within the valid coordinate range.
    /// </summary>
    /// <param name="val">The value to convert.</param>
    /// <returns>The rounded 64-bit integer value, or <see cref="Invalid64"/> when the value is out of range.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long CheckCastInt64(double val)
    {
      if ((val >= max_coord) || (val <= min_coord)) return Invalid64;
      return (long)Math.Round(val, MidpointRounding.AwayFromZero);
    }

    // GetLineIntersectPt - a 'true' result is non-parallel. The 'ip' will also
    // be constrained to seg1. However, it's possible that 'ip' won't be inside
    // seg2, even when 'ip' hasn't been constrained (ie 'ip' is inside seg1).

    /// <summary>
    /// Calculates the intersection point of two lines defined by pairs of points.
    /// </summary>
    /// <param name="ln1a">The start point of the first line.</param>
    /// <param name="ln1b">The end point of the first line.</param>
    /// <param name="ln2a">The start point of the second line.</param>
    /// <param name="ln2b">The end point of the second line.</param>
    /// <param name="ip">The intersection point, if found.</param>
    /// <returns><see langword="true"/> if the lines intersect; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GetLineIntersectPt(Point64 ln1a,
      Point64 ln1b, Point64 ln2a, Point64 ln2b, out Point64 ip)
    {
      double dy1 = (ln1b.Y - ln1a.Y);
      double dx1 = (ln1b.X - ln1a.X);
      double dy2 = (ln2b.Y - ln2a.Y);
      double dx2 = (ln2b.X - ln2a.X);
      double det = dy1 * dx2 - dy2 * dx1;
      if (det == 0.0)
      {
        ip = new Point64();
        return false;
      }

      double t = ((ln1a.X - ln2a.X) * dy2 - (ln1a.Y - ln2a.Y) * dx2) / det;
      if (t <= 0.0) ip = ln1a;
      else if (t >= 1.0) ip = ln1b;
      else
      {
        // avoid using constructor (and rounding too) as they affect performance //664
        ip.X = (long)(ln1a.X + t * dx1);
        ip.Y = (long)(ln1a.Y + t * dy1);
#if USINGZ
        ip.Z = 0;
#endif
      }
      return true;
    }

    /// <summary>
    /// Calculates the intersection point of two lines defined by pairs of points.
    /// </summary>
    /// <param name="ln1a">The start point of the first line.</param>
    /// <param name="ln1b">The end point of the first line.</param>
    /// <param name="ln2a">The start point of the second line.</param>
    /// <param name="ln2b">The end point of the second line.</param>
    /// <param name="ip">The intersection point, if found.</param>
    /// <returns><see langword="true"/> if the lines intersect; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GetLineIntersectPt(PointD ln1a,
      PointD ln1b, PointD ln2a, PointD ln2b, out PointD ip)
    {
      double dy1 = (ln1b.y - ln1a.y);
      double dx1 = (ln1b.x - ln1a.x);
      double dy2 = (ln2b.y - ln2a.y);
      double dx2 = (ln2b.x - ln2a.x);
      double det = dy1 * dx2 - dy2 * dx1;
      if (det == 0.0)
      {
        ip = new PointD();
        return false;
      }

      double t = ((ln1a.x - ln2a.x) * dy2 - (ln1a.y - ln2a.y) * dx2) / det;
      if (t <= 0.0) ip = ln1a;
      else if (t >= 1.0) ip = ln1b;
      else
      {
        // avoid using constructor (and rounding too) as they affect performance //664
        ip.x = (ln1a.x + t * dx1);
        ip.y = (ln1a.y + t * dy1);
#if USINGZ
        ip.Z = 0;
#endif
      }
      return true;
    }

    /// <summary>
    /// Determines whether two line segments intersect.
    /// </summary>
    /// <param name="seg1a">The first endpoint of the first segment.</param>
    /// <param name="seg1b">The second endpoint of the first segment.</param>
    /// <param name="seg2a">The first endpoint of the second segment.</param>
    /// <param name="seg2b">The second endpoint of the second segment.</param>
    /// <param name="inclusive"><see langword="true"/> to treat touching endpoints as intersections; otherwise, <see langword="false"/>.</param>
    /// <returns><see langword="true"/> if the segments intersect; otherwise, <see langword="false"/>.</returns>
    internal static bool SegsIntersect(Point64 seg1a,
      Point64 seg1b, Point64 seg2a, Point64 seg2b, bool inclusive = false)
    {
      double dy1 = (seg1b.Y - seg1a.Y);
      double dx1 = (seg1b.X - seg1a.X);
      double dy2 = (seg2b.Y - seg2a.Y);
      double dx2 = (seg2b.X - seg2a.X);
      double cp = dy1 * dx2 - dy2 * dx1;
      if (cp == 0) return false; // ie parallel segments

      if (inclusive)
      {
        //result **includes** segments that touch at an end point
        double t = ((seg1a.X - seg2a.X) * dy2 - (seg1a.Y - seg2a.Y) * dx2);
        if (t == 0) return true;
        if (t > 0)
        {
          if (cp < 0 || t > cp) return false;
        }
        else if (cp > 0 || t < cp) return false; // false when t more neg. than cp

        t = ((seg1a.X - seg2a.X) * dy1 - (seg1a.Y - seg2a.Y) * dx1);
        if (t == 0) return true;
        if (t > 0) return (cp > 0 && t <= cp);
        else return (cp < 0 && t >= cp);        // true when t less neg. than cp
      }
      else
      {
        //result **excludes** segments that touch at an end point
        double t = ((seg1a.X - seg2a.X) * dy2 - (seg1a.Y - seg2a.Y) * dx2);
        if (t == 0) return false;
        if (t > 0)
        {
          if (cp < 0 || t >= cp) return false;
        }
        else if (cp > 0 || t <= cp) return false; // false when t more neg. than cp

        t = ((seg1a.X - seg2a.X) * dy1 - (seg1a.Y - seg2a.Y) * dx1);
        if (t == 0) return false;
        if (t > 0) return (cp > 0 && t < cp);
        else return (cp < 0 && t > cp); // true when t less neg. than cp
      }
    }

    /// <summary>
    /// Gets the bounding rectangle of a path.
    /// </summary>
    /// <param name="path">The path whose bounding rectangle is to be calculated.</param>
    /// <returns>A <see cref="Rect64"/> representing the bounding rectangle of the path.</returns>
    public static Rect64 GetBounds(Path64 path)
    {
      if (path.Count == 0) return new Rect64();
      Rect64 result = Clipper.InvalidRect64;
      foreach (Point64 pt in path)
      {
        if (pt.X < result.left) result.left = pt.X;
        if (pt.X > result.right) result.right = pt.X;
        if (pt.Y < result.top) result.top = pt.Y;
        if (pt.Y > result.bottom) result.bottom = pt.Y;
      }
      return result;
    }

    /// <summary>
    /// Gets the closest point on a segment to a given point.
    /// </summary>
    /// <param name="offPt">The point from which the closest point is to be determined.</param>
    /// <param name="seg1">The start point of the segment.</param>
    /// <param name="seg2">The end point of the segment.</param>
    /// <returns>The closest point on the segment to the given point.</returns>
    public static Point64 GetClosestPtOnSegment(Point64 offPt,
    Point64 seg1, Point64 seg2)
    {
      if (seg1.X == seg2.X && seg1.Y == seg2.Y) return seg1;
      double dx = (seg2.X - seg1.X);
      double dy = (seg2.Y - seg1.Y);
      double q = ((offPt.X - seg1.X) * dx +
        (offPt.Y - seg1.Y) * dy) / ((dx * dx) + (dy * dy));
      if (q < 0) q = 0; else if (q > 1) q = 1;
      return new Point64(
        // use MidpointRounding.ToEven in order to explicitly match the nearbyint behaviour on the C++ side
        seg1.X + Math.Round(q * dx, MidpointRounding.ToEven),
        seg1.Y + Math.Round(q * dy, MidpointRounding.ToEven)
      );
    }

    /// <summary>
    /// Determines the position of a point relative to a polygon.
    /// </summary>
    /// <param name="pt">The point to check.</param>
    /// <param name="polygon">The polygon to check against.</param>
    /// <returns>The <see cref="PointInPolygonResult"/> indicating the position of the point.</returns>
    public static PointInPolygonResult PointInPolygon(Point64 pt, Path64 polygon)
    {
      int len = polygon.Count, start = 0;
      if (len < 3) return PointInPolygonResult.IsOutside;

      while (start < len && polygon[start].Y == pt.Y) start++;
      if (start == len) return PointInPolygonResult.IsOutside;

      bool isAbove = polygon[start].Y < pt.Y, startingAbove = isAbove;
      int val = 0, i = start + 1, end = len;
      while (true)
      {
        if (i == end)
        {
          if (end == 0 || start == 0) break;
          end = start;
          i = 0;
        }

        if (isAbove)
        {
          while (i < end && polygon[i].Y < pt.Y) i++;
        }
        else
        {
          while (i < end && polygon[i].Y > pt.Y) i++;
        }

        if (i == end) continue;

        Point64 curr = polygon[i], prev;
        if (i > 0) prev = polygon[i - 1];
        else prev = polygon[len - 1];

        if (curr.Y == pt.Y)
        {
          if (curr.X == pt.X || (curr.Y == prev.Y &&
            ((pt.X < prev.X) != (pt.X < curr.X))))
            return PointInPolygonResult.IsOn;
          i++;
          if (i == start) break;
          continue;
        }

        if (pt.X < curr.X && pt.X < prev.X)
        {
          // we're only interested in edges crossing on the left
        }
        else if (pt.X > prev.X && pt.X > curr.X)
        {
          val = 1 - val; // toggle val
        }
        else
        {
          int cps2 = CrossProductSign(prev, curr, pt);
          if (cps2 == 0) return PointInPolygonResult.IsOn;
          if ((cps2 < 0) == isAbove) val = 1 - val;
        }
        isAbove = !isAbove;
        i++;
      }

      if (isAbove == startingAbove) return val == 0 ? PointInPolygonResult.IsOutside : PointInPolygonResult.IsInside;
      if (i == len) i = 0;
      int cps = (i == 0) ?
        CrossProductSign(polygon[len - 1], polygon[0], pt) :
        CrossProductSign(polygon[i - 1], polygon[i], pt);

      if (cps == 0) return PointInPolygonResult.IsOn;
      if ((cps < 0) == isAbove) val = 1 - val;
      return val == 0 ? PointInPolygonResult.IsOutside : PointInPolygonResult.IsInside;
    }

    /// <summary>
    /// Determines if one path is completely contained within another.
    /// </summary>
    /// <param name="path1">The outer path.</param>
    /// <param name="path2">The inner path.</param>
    /// <returns><see langword="true"/> if path1 contains path2; otherwise, <see langword="false"/>.</returns>
    public static bool Path2ContainsPath1(Path64 path1, Path64 path2)
    {
      // we need to make some accommodation for rounding errors
      // so we won't jump if the first vertex is found outside
      PointInPolygonResult pip = PointInPolygonResult.IsOn;
      foreach (Point64 pt in path1)
      {
        switch (PointInPolygon(pt, path2))
        {
          case PointInPolygonResult.IsOutside:
            if (pip == PointInPolygonResult.IsOutside) return false;
            pip = PointInPolygonResult.IsOutside;
            break;
          case PointInPolygonResult.IsInside:
            if (pip == PointInPolygonResult.IsInside) return true;
            pip = PointInPolygonResult.IsInside;
            break;
          default: break;
        }
      }
      // since path1's location is still equivocal, check its midpoint
      Point64 mp = GetBounds(path1).MidPoint();
      return InternalClipper.PointInPolygon(mp, path2) != PointInPolygonResult.IsOutside;
    }


  } // InternalClipper

} // namespace
