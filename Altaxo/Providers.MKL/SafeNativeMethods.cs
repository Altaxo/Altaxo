﻿// <copyright file="SafeNativeMethods.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// https://numerics.mathdotnet.com
//
// Copyright (c) 2009-2016 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using System.Runtime.InteropServices;
using System.Security;
using Altaxo.Calc.Providers.LinearAlgebra;
using Complex = System.Numerics.Complex;

namespace Altaxo.Calc.Providers.MKL
{
  /// <summary>
  /// P/Invoke methods to the native math libraries.
  /// </summary>
  [SuppressUnmanagedCodeSecurity]
  [SecurityCritical]
  internal static class SafeNativeMethods
  {
    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Name of the native DLL.
    /// </summary>
    internal const string DllName = "libMathNetNumericsMKL";

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int query_capability(int capability);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void set_consistency_mode(int mode);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void set_vml_mode(uint mode);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void set_max_threads(int num_threads);

    #region Memory
    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void free_buffers();

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void thread_free_buffers();

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int disable_fast_mm();

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern long mem_stat([Out] out int allocatedBuffers);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern long peak_mem_usage(int mode);

    #endregion Memory

    #region BLAS

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void s_axpy(int n, float alpha, float[] x, [In, Out] float[] y);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void d_axpy(int n, double alpha, double[] x, [In, Out] double[] y);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void c_axpy(int n, Complex32 alpha, Complex32[] x, [In, Out] Complex32[] y);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void z_axpy(int n, Complex alpha, Complex[] x, [In, Out] Complex[] y);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void s_scale(int n, float alpha, [Out] float[] x);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void d_scale(int n, double alpha, [Out] double[] x);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void c_scale(int n, Complex32 alpha, [In, Out] Complex32[] x);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void z_scale(int n, Complex alpha, [In, Out] Complex[] x);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern float s_dot_product(int n, float[] x, float[] y);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern double d_dot_product(int n, double[] x, double[] y);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern Complex32 c_dot_product(int n, Complex32[] x, Complex32[] y);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern Complex z_dot_product(int n, Complex[] x, Complex[] y);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void s_matrix_multiply(Transpose transA, Transpose transB, int m, int n, int k, float alpha, float[] x, float[] y, float beta, [In, Out] float[] c);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void d_matrix_multiply(Transpose transA, Transpose transB, int m, int n, int k, double alpha, double[] x, double[] y, double beta, [In, Out] double[] c);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void c_matrix_multiply(Transpose transA, Transpose transB, int m, int n, int k, Complex32 alpha, Complex32[] x, Complex32[] y, Complex32 beta, [In, Out] Complex32[] c);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void z_matrix_multiply(Transpose transA, Transpose transB, int m, int n, int k, Complex alpha, Complex[] x, Complex[] y, Complex beta, [In, Out] Complex[] c);

    #endregion BLAS

    #region LAPACK

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern float s_matrix_norm(byte norm, int rows, int columns, [In] float[] a);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern double d_matrix_norm(byte norm, int rows, int columns, [In] double[] a);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern float c_matrix_norm(byte norm, int rows, int columns, [In] Complex32[] a);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern double z_matrix_norm(byte norm, int rows, int columns, [In] Complex[] a);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int s_cholesky_factor(int n, [In, Out] float[] a);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int d_cholesky_factor(int n, [In, Out] double[] a);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int c_cholesky_factor(int n, [In, Out] Complex32[] a);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int z_cholesky_factor(int n, [In, Out] Complex[] a);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int s_lu_factor(int n, [In, Out] float[] a, [In, Out] int[] ipiv);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int d_lu_factor(int n, [In, Out] double[] a, [In, Out] int[] ipiv);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int c_lu_factor(int n, [In, Out] Complex32[] a, [In, Out] int[] ipiv);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int z_lu_factor(int n, [In, Out] Complex[] a, [In, Out] int[] ipiv);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int s_lu_inverse(int n, [In, Out] float[] a);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int d_lu_inverse(int n, [In, Out] double[] a);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int c_lu_inverse(int n, [In, Out] Complex32[] a);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int z_lu_inverse(int n, [In, Out] Complex[] a);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int s_lu_inverse_factored(int n, [In, Out] float[] a, [In, Out] int[] ipiv);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int d_lu_inverse_factored(int n, [In, Out] double[] a, [In, Out] int[] ipiv);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int c_lu_inverse_factored(int n, [In, Out] Complex32[] a, [In, Out] int[] ipiv);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int z_lu_inverse_factored(int n, [In, Out] Complex[] a, [In, Out] int[] ipiv);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int s_lu_solve_factored(int n, int nrhs, float[] a, [In, Out] int[] ipiv, [In, Out] float[] b);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int d_lu_solve_factored(int n, int nrhs, double[] a, [In, Out] int[] ipiv, [In, Out] double[] b);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int c_lu_solve_factored(int n, int nrhs, Complex32[] a, [In, Out] int[] ipiv, [In, Out] Complex32[] b);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int z_lu_solve_factored(int n, int nrhs, Complex[] a, [In, Out] int[] ipiv, [In, Out] Complex[] b);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int s_lu_solve(int n, int nrhs, float[] a, [In, Out] float[] b);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int d_lu_solve(int n, int nrhs, double[] a, [In, Out] double[] b);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int c_lu_solve(int n, int nrhs, Complex32[] a, [In, Out] Complex32[] b);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int z_lu_solve(int n, int nrhs, Complex[] a, [In, Out] Complex[] b);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int s_cholesky_solve(int n, int nrhs, float[] a, [In, Out] float[] b);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int d_cholesky_solve(int n, int nrhs, double[] a, [In, Out] double[] b);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int c_cholesky_solve(int n, int nrhs, Complex32[] a, [In, Out] Complex32[] b);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int z_cholesky_solve(int n, int nrhs, Complex[] a, [In, Out] Complex[] b);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int s_cholesky_solve_factored(int n, int nrhs, float[] a, [In, Out] float[] b);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int d_cholesky_solve_factored(int n, int nrhs, double[] a, [In, Out] double[] b);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int c_cholesky_solve_factored(int n, int nrhs, Complex32[] a, [In, Out] Complex32[] b);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int z_cholesky_solve_factored(int n, int nrhs, Complex[] a, [In, Out] Complex[] b);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int s_qr_factor(int m, int n, [In, Out] float[] r, [In, Out] float[] tau, [In, Out] float[] q);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int d_qr_factor(int m, int n, [In, Out] double[] r, [In, Out] double[] tau, [In, Out] double[] q);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int c_qr_factor(int m, int n, [In, Out] Complex32[] r, [In, Out] Complex32[] tau, [In, Out] Complex32[] q);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int z_qr_factor(int m, int n, [In, Out] Complex[] r, [In, Out] Complex[] tau, [In, Out] Complex[] q);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int s_qr_thin_factor(int m, int n, [In, Out] float[] q, [In, Out] float[] tau, [In, Out] float[] r);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int d_qr_thin_factor(int m, int n, [In, Out] double[] q, [In, Out] double[] tau, [In, Out] double[] r);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int c_qr_thin_factor(int m, int n, [In, Out] Complex32[] q, [In, Out] Complex32[] tau, [In, Out] Complex32[] r);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int z_qr_thin_factor(int m, int n, [In, Out] Complex[] q, [In, Out] Complex[] tau, [In, Out] Complex[] r);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int s_qr_solve(int m, int n, int bn, float[] r, float[] b, [In, Out] float[] x);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int d_qr_solve(int m, int n, int bn, double[] r, double[] b, [In, Out] double[] x);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int c_qr_solve(int m, int n, int bn, Complex32[] r, Complex32[] b, [In, Out] Complex32[] x);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int z_qr_solve(int m, int n, int bn, Complex[] r, Complex[] b, [In, Out] Complex[] x);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int s_qr_solve_factored(int m, int n, int bn, float[] r, float[] b, float[] tau, [In, Out] float[] x);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int d_qr_solve_factored(int m, int n, int bn, double[] r, double[] b, double[] tau, [In, Out] double[] x);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int c_qr_solve_factored(int m, int n, int bn, Complex32[] r, Complex32[] b, Complex32[] tau, [In, Out] Complex32[] x);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int z_qr_solve_factored(int m, int n, int bn, Complex[] r, Complex[] b, Complex[] tau, [In, Out] Complex[] x);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int s_svd_factor([MarshalAs(UnmanagedType.U1)] bool computeVectors, int m, int n, [In, Out] float[] a, [In, Out] float[] s, [In, Out] float[] u, [In, Out] float[] v);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int d_svd_factor([MarshalAs(UnmanagedType.U1)] bool computeVectors, int m, int n, [In, Out] double[] a, [In, Out] double[] s, [In, Out] double[] u, [In, Out] double[] v);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int c_svd_factor([MarshalAs(UnmanagedType.U1)] bool computeVectors, int m, int n, [In, Out] Complex32[] a, [In, Out] Complex32[] s, [In, Out] Complex32[] u, [In, Out] Complex32[] v);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int z_svd_factor([MarshalAs(UnmanagedType.U1)] bool computeVectors, int m, int n, [In, Out] Complex[] a, [In, Out] Complex[] s, [In, Out] Complex[] u, [In, Out] Complex[] v);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int s_eigen([MarshalAs(UnmanagedType.U1)] bool isSymmetric, int n, [In] float[] a, [In, Out] float[] vectors, [In, Out] Complex[] values, [In, Out] float[] d);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int d_eigen([MarshalAs(UnmanagedType.U1)] bool isSymmetric, int n, [In] double[] a, [In, Out] double[] vectors, [In, Out] Complex[] values, [In, Out] double[] d);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int c_eigen([MarshalAs(UnmanagedType.U1)] bool isSymmetric, int n, [In] Complex32[] a, [In, Out] Complex32[] vectors, [In, Out] Complex[] values, [In, Out] Complex32[] d);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int z_eigen([MarshalAs(UnmanagedType.U1)] bool isSymmetric, int n, [In] Complex[] a, [In, Out] Complex[] vectors, [In, Out] Complex[] values, [In, Out] Complex[] d);

    #endregion LAPACK

    #region Vector Functions

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void s_vector_add(int n, float[] x, float[] y, [In, Out] float[] result);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void s_vector_subtract(int n, float[] x, float[] y, [In, Out] float[] result);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void s_vector_multiply(int n, float[] x, float[] y, [In, Out] float[] result);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void s_vector_divide(int n, float[] x, float[] y, [In, Out] float[] result);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void s_vector_power(int n, float[] x, float[] y, [In, Out] float[] result);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void d_vector_add(int n, double[] x, double[] y, [In, Out] double[] result);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void d_vector_subtract(int n, double[] x, double[] y, [In, Out] double[] result);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void d_vector_multiply(int n, double[] x, double[] y, [In, Out] double[] result);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void d_vector_divide(int n, double[] x, double[] y, [In, Out] double[] result);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void d_vector_power(int n, double[] x, double[] y, [In, Out] double[] result);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void c_vector_add(int n, Complex32[] x, Complex32[] y, [In, Out] Complex32[] result);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void c_vector_subtract(int n, Complex32[] x, Complex32[] y, [In, Out] Complex32[] result);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void c_vector_multiply(int n, Complex32[] x, Complex32[] y, [In, Out] Complex32[] result);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void c_vector_divide(int n, Complex32[] x, Complex32[] y, [In, Out] Complex32[] result);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void c_vector_power(int n, Complex32[] x, Complex32[] y, [In, Out] Complex32[] result);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void z_vector_add(int n, Complex[] x, Complex[] y, [In, Out] Complex[] result);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void z_vector_subtract(int n, Complex[] x, Complex[] y, [In, Out] Complex[] result);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void z_vector_multiply(int n, Complex[] x, Complex[] y, [In, Out] Complex[] result);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void z_vector_divide(int n, Complex[] x, Complex[] y, [In, Out] Complex[] result);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void z_vector_power(int n, Complex[] x, Complex[] y, [In, Out] Complex[] result);

    #endregion  Vector Functions

    #region FFT

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int x_fft_free([In] ref IntPtr handle);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int z_fft_create([Out] out IntPtr handle, int n, double forward_scale, double backward_scale);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int c_fft_create([Out] out IntPtr handle, int n, float forward_scale, float backward_scale);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int d_fft_create([Out] out IntPtr handle, int n, double forward_scale, double backward_scale);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int s_fft_create([Out] out IntPtr handle, int n, float forward_scale, float backward_scale);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int z_fft_create_multidim([Out] out IntPtr handle, int dimensions, [In] int[] n, double forward_scale, double backward_scale);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int c_fft_create_multidim([Out] out IntPtr handle, int dimensions, [In] int[] n, float forward_scale, float backward_scale);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int z_fft_forward([In] IntPtr handle, [In, Out] Complex[] x);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int c_fft_forward([In] IntPtr handle, [In, Out] Complex32[] x);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int d_fft_forward([In] IntPtr handle, [In, Out] double[] x);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int s_fft_forward([In] IntPtr handle, [In, Out] float[] x);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int z_fft_backward([In] IntPtr handle, [In, Out] Complex[] x);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int c_fft_backward([In] IntPtr handle, [In, Out] Complex32[] x);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int d_fft_backward([In] IntPtr handle, [In, Out] double[] x);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int s_fft_backward([In] IntPtr handle, [In, Out] float[] x);

    #endregion FFT

    #region Direct Sparse Solver

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int s_dss_solve(int matrixStructure, int matrixType, int systemType,
        int rowCount, int columnCount, int nonZerosCount, int[] rowPointers, int[] columnIndices, float[] values,
        int nRhs, [In, Out] float[] rhs, [In, Out] float[] solution);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int d_dss_solve(int matrixStructure, int matrixType, int systemType,
        int rowCount, int columnCount, int nonZerosCount, int[] rowPointers, int[] columnIndices, double[] values,
        int nRhs, [In, Out] double[] rhs, [In, Out] double[] solution);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int c_dss_solve(int matrixStructure, int matrixType, int systemType,
        int rowCount, int columnCount, int nonZerosCount, int[] rowPointers, int[] columnIndices, Complex32[] values,
        int nRhs, [In, Out] Complex32[] rhs, [In, Out] Complex32[] solution);

    [DllImport(DllName, ExactSpelling = true, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int z_dss_solve(int matrixStructure, int matrixType, int systemType,
        int rowCount, int columnCount, int nonZerosCount, int[] rowPointers, int[] columnIndices, Complex[] values,
        int nRhs, [In, Out] Complex[] rhs, [In, Out] Complex[] solution);

    #endregion Direct Sparse Solver

    // ReSharper restore InconsistentNaming
  }
}
