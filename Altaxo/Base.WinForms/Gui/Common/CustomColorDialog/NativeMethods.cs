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

/*================================================================================
  File:      NativeMethods.cs

  Summary:   This is part of a sample showing how to place Windows Forms controls
             inside one of the common file dialogs.
----------------------------------------------------------------------------------
Copyright (C) Microsoft Corporation.  All rights reserved.

This source code is intended only as a supplement to Microsoft Development Tools
and/or on-line documentation.  See these other materials for detailed information
regarding Microsoft code samples.

This sample is not intended for production use. Code and policy for a production
application must be developed to meet the specific data and security requirements
of the application.

THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
================================================================================*/

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Defines the shape of hook procedures that can be called by the ChooseColorDialog
	/// </summary>
	internal delegate IntPtr CCHookProc(IntPtr hWnd, UInt16 msg, Int32 wParam, Int32 lParam);
	/// <summary>
	/// Values that can be placed in the CHOOSECOLOR structure, we don't use all of them
	/// </summary>
	internal class ChooseColorFlags
	{
		public const Int32  RGBInit               = 0x00000001;
		public const Int32  FullOpen              = 0x00000002;
		public const Int32  PreventFullOpen       = 0x00000004;
		public const Int32  ShowHelp              = 0x00000008;
		public const Int32  EnableHook            = 0x00000010;
		public const Int32  EnableTemplate        = 0x00000020;
		public const Int32  EnableTemplatehandle  = 0x00000040;
		public const Int32  SoliDcolor            = 0x00000080;
		public const Int32  AnyColor              = 0x00000100;
	};

	/// <summary>
	/// A small subset of the window messages that can be sent to the ChooseColorDialog
	/// These are just the ones that this implementation is interested in
	/// </summary>
	internal class WindowMessage
	{
		public const UInt16 InitDialog =	0x0110;
		public const UInt16 Size =			0x0005;
		public const UInt16 Notify =		0x004E;
	};

	/// <summary>
	/// See the documentation for CHOOSECOLOR
	/// </summary>
	internal struct ChooseColor
	{ 
		public Int32		lStructSize; 
		public IntPtr		hwndOwner; 
		public IntPtr		hInstance; 
		public Int32		rgbResult; 
		public IntPtr		lpCustColors; 
		public Int32		Flags; 
		public IntPtr		lCustData; 
		public CCHookProc	lpfnHook; 
		public IntPtr		lpTemplateName; 
	};
	
	/// <summary>
	/// The rectangle structure used in Win32 API calls
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct RECT 
	{
		public int left;
		public int top;
		public int right;
		public int bottom;
	};

	/// <summary>
	/// The point structure used in Win32 API calls
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct POINT
	{
		public int X;
		public int Y;
	};
	
	/// <summary>
	/// Contains all of the p/invoke declarations for the Win32 APIs used in this sample
	/// </summary>
	public class NativeMethods
	{

		[DllImport("User32.dll", CharSet = CharSet.Unicode)]
		internal static extern IntPtr GetDlgItem( IntPtr hWndDlg, Int16 Id );

		[DllImport("User32.dll", CharSet = CharSet.Unicode)]
		internal static extern IntPtr GetParent( IntPtr hWnd );

		[DllImport("User32.dll", CharSet = CharSet.Unicode)]
		internal static extern IntPtr SetParent( IntPtr hWndChild, IntPtr hWndNewParent );
		
		[DllImport("User32.dll", CharSet = CharSet.Unicode)]
		internal static extern UInt32 SendMessage( IntPtr hWnd, UInt32 msg, UInt32 wParam, StringBuilder buffer );

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		internal static extern int GetWindowRect( IntPtr hWnd, ref RECT rc );

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		internal static extern int GetClientRect( IntPtr hWnd, ref RECT rc );

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		internal static extern bool ScreenToClient( IntPtr hWnd, ref POINT pt );

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		internal static extern bool MoveWindow( IntPtr hWnd, int X, int Y, int Width, int Height, bool repaint );

		[DllImport("ComDlg32.dll", CharSet = CharSet.Unicode)]
		internal static extern Int32 CommDlgExtendedError();

		[DllImport("ComDlg32.dll", CharSet = CharSet.Unicode)]
		internal static extern bool ChooseColor( ref ChooseColor cc );
	}
}
