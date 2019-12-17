#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2012 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Calc.Fourier
{
  /// <summary>
  /// Fast correlation of a maximum length sequence with a signal. The result is the response function of the system under test. See remarks for detail.
  /// </summary>
  /// <remarks>
  /// The  response function of a device under test (DUT) can be determined with the help of maximum length sequences as followed. The maximum length sequence is used to generate a input signal for the DUT.
  /// The DUT will respond to this input signal. As a result, it will generate a output signal. To get the response function of the DUT, the output signal of the DUT should be correlated with its input signal.
  /// As a simplification, here the output signal of the DUT is correlated with the maximum length sequence itself. This means that the DUT here includes the generation of the output signal.
  /// </remarks>
  public class FastHadamardTransformation
  {
    private int[] _responsePermutation;
    private int[] _signalPermutation;
    private double[] _permutedSignal;
    private int _numberOfBits;

    /// <summary>Initializes a new instance of the <see cref="FastHadamardTransformation"/> class.</summary>
    /// <param name="mls">The maximum length sequence that is used to generate the input signal for the device under test (DUT).</param>
    public FastHadamardTransformation(Calc.Probability.MaximumLengthSequence mls)
    {
      _responsePermutation = new int[mls.Length];
      _signalPermutation = new int[mls.Length];
      _permutedSignal = new double[mls.Length + 1];
      _numberOfBits = 1 + BinaryMath.Ld(mls.Length);

      bool[] seq = new bool[mls.Length];
      int i = 0;
      foreach (var e in mls.GetSequence(false, true))
        seq[i++] = e;

      GenerateResponsePermutationTable(seq, _responsePermutation, _numberOfBits);
      GenerateSignalPermutationTable(seq, _signalPermutation, _numberOfBits);
    }

    /// <summary>Executes the correlation.</summary>
    /// <param name="signal">The output signal from the device under test (DUT). The length of this signal has to be equal to the length of the maximum length sequence.</param>
    /// <param name="response">As the result of this function, this array contains the response function, i.e. the cross-correlation of the maximum length sequence with the signal given in <paramref name="signal"/>. You must provide an array at least as long as the length of the maximum length sequence.</param>
    /// <param name="isDCCoupled">If the signal you measured is DC coupled, set this parameter to <c>true</c>. Otherwise, set it to <c>false</c>.</param>
    public void Execute(double[] signal, double[] response, bool isDCCoupled)
    {
      PermuteSignal(signal, _permutedSignal, _signalPermutation, isDCCoupled);
      TransformFastHadamard(_permutedSignal, ((uint)_signalPermutation.Length) + 1, _numberOfBits);
      PermuteResponse(_permutedSignal, response, _responsePermutation);
    }

    /// <summary>Generates the table for the signal permutation.</summary>
    /// <param name="mls">The maximum length sequence.</param>
    /// <param name="signalPermutationTable">As the result of this function, this array contains the computed permutation table for the measured signal.</param>
    /// <param name="N">The number of bits (stages) of the maximum length sequence.</param>
    private static void GenerateSignalPermutationTable(bool[] mls, int[] signalPermutationTable, int N)
    {
      var P = mls.Length;
      for (int i = 0; i < P; ++i) // For each column in the S matrix
      {
        signalPermutationTable[i] = 0;
        for (int j = 0; j < N; ++j) // Find the tagS as the value of the columns in the S matrix regarded as a binary number
        {
          if (mls[(P + i - j) % P])
            signalPermutationTable[i] += (1 << (N - 1 - j));
        }
      }
    }

    /// <summary>Generates the table for the response permutation.</summary>
    /// <param name="mls">The maximum length sequence.</param>
    /// <param name="responsePermutationTable">As the result of this function, this array contains the computed permutation table that maps the result of the hadamard transformation to the response.</param>
    /// <param name="N">The number of bits (stages) of the maximum length sequence.</param>
    private static void GenerateResponsePermutationTable(bool[] mls, int[] responsePermutationTable, int N)
    {
      int P = mls.Length;

      int[] colSum = new int[P];
      int[] index = new int[N];
      for (int i = 0; i < P; i++) // Run through all the columns in the autocorr matrix
      {
        colSum[i] = 0;
        for (int j = 0; j < N; j++) // Find colSum as the value of the first N elements regarded as a binary number
        {
          if (mls[(P + i - j) % P])
            colSum[i] += 1 << (N - 1 - j);
        }
        for (int j = 0; j < N; j++) // Figure out if colSum is a 2^j number and store the column as the j'th index
        {
          if (colSum[i] == (1 << j))
            index[j] = i;
        }
      }
      for (int i = 0; i < P; i++) // For each row in the L matrix
      {
        responsePermutationTable[i] = 0;
        for (int j = 0; j < N; j++) // Find the tagL as the value of the rows in the L matrix regarded as a binary number
        {
          if (mls[(P + index[j] - i) % P])
            responsePermutationTable[i] += (1 << j);
        }
      }
    }

    /// <summary>Permutes the measured output signal.</summary>
    /// <param name="signal">The signal.</param>
    /// <param name="permutedSignal">Contains the permuted signal as the result of this function.</param>
    /// <param name="signalPermuationTable">The permutation table for the signal.</param>
    /// <param name="isDCCoupled">If set to <c>true</c>, the signal is considered to be DC coupled. The DC offset is subtracted from the signal.</param>
    private static void PermuteSignal(double[] signal, double[] permutedSignal, int[] signalPermuationTable, bool isDCCoupled)
    {
      int P = signalPermuationTable.Length;

      double dc = 0;
      if (isDCCoupled)
      {
        for (int i = 0; i < P; i++)
          dc += signal[i];
      }
      permutedSignal[0] = -dc;

      for (int i = 0; i < P; i++)
        permutedSignal[signalPermuationTable[i]] = signal[i];
    }

    /// <summary>Permutes the response.</summary>
    /// <param name="permutedSignal">The permuted result of the hadamard transformation.</param>
    /// <param name="response">Contains the response function as the result of this function.</param>
    /// <param name="responsePermutationTable">The response permuatation table..</param>
    private static void PermuteResponse(double[] permutedSignal, double[] response, int[] responsePermutationTable)
    {
      int P = responsePermutationTable.Length;
      double fact = 1 / (double)(P + 1);
      for (int i = 0; i < P; i++) // Just a permutation of the impulse response
      {
        response[i] = permutedSignal[responsePermutationTable[i]] * fact;
      }
    }

    /// <summary>Executes a fast hadamard transformation in-place.</summary>
    /// <param name="x">The array to transform.</param>
    /// <param name="P1">The length of the transformation array (1 + Length of the maximum length sequence).</param>
    /// <param name="N">The number of bits of the maximum length sequence.</param>
    private static void TransformFastHadamard(double[] x, uint P1, int N)
    {
      double temp;
      uint k1 = P1;
      for (int k = 0; k < N; k++)
      {
        var k2 = k1 >> 1;
        for (uint j = 0; j < k2; j++)
        {
          for (uint i = j; i < P1; i = i + k1)
          {
            uint i1 = i + k2;
            temp = x[i] + x[i1];
            x[i1] = x[i] - x[i1];
            x[i] = temp;
          }
        }
        k1 = k1 >> 1;
      }
    }
  }
}
