using System;

namespace Altaxo.Calc.Random
{
  /// <summary>
  /// Provides helper methods for generating integer seeds for pseudo-random number generators.
  /// </summary>
  public static class RandomSeed
  {
    private static readonly object Lock = new object();
    private static readonly System.Security.Cryptography.RandomNumberGenerator MasterRng = System.Security.Cryptography.RandomNumberGenerator.Create();

    /// <summary>
    /// Provides a time-dependent seed value, matching the default behavior of System.Random.
    /// WARNING: There is no randomness in this seed and quick repeated calls can cause
    /// the same seed value. Do not use for cryptography!
    /// </summary>
    /// <returns>
    /// An integer seed value based on the current system tick count.
    /// </returns>
    public static int Time()
    {
      return Environment.TickCount;
    }

    /// <summary>
    /// Provides a seed based on time and unique GUIDs.
    /// WARNING: There is only low randomness in this seed, but at least quick repeated
    /// calls will result in different seed values. Do not use for cryptography!
    /// </summary>
    /// <returns>
    /// An integer seed value based on the current system tick count and a new GUID hash code.
    /// </returns>
    public static int Guid()
    {
      return Environment.TickCount ^ System.Guid.NewGuid().GetHashCode();
    }

    /// <summary>
    /// Provides a seed based on an internal random number generator (crypto if available), time and unique GUIDs.
    /// WARNING: There is only medium randomness in this seed, but quick repeated
    /// calls will result in different seed values. Do not use for cryptography!
    /// </summary>
    /// <returns>
    /// An integer seed value based on a cryptographically secure random number generator, time, and a new GUID hash code.
    /// </returns>
    public static int Robust()
    {
      lock (Lock)
      {
        var bytes = new byte[4];
        MasterRng.GetBytes(bytes);
        return BitConverter.ToInt32(bytes, 0);
      }
    }
  }
}
