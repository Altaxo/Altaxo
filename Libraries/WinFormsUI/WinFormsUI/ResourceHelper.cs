// *****************************************************************************
// 
//  Copyright 2004, Weifen Luo
//  All rights reserved. The software and associated documentation 
//  supplied hereunder are the proprietary information of Weifen Luo
//  and are supplied subject to licence terms.
// 
//  WinFormsUI Library Version 1.0
// *****************************************************************************

using System;
using System.Drawing;
using System.Reflection;
using System.Resources;

using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI
{
	/// <summary>
	/// Summary description for ResourceHelper.
	/// </summary>
	internal class ResourceHelper
	{
		private static ResourceManager m_resourceManager;

		static ResourceHelper()
		{
#if !ModifiedForAltaxo
			m_resourceManager = new ResourceManager("WeifenLuo.WinFormsUI.Strings", typeof(DockPanel).Assembly);
#else
			m_resourceManager = new ResourceManager("WeifenLuo.WinFormsUI.Resources.Strings", typeof(DockPanel).Assembly);
#endif
		}
	
		public static Bitmap LoadBitmap(string name)
		{
			Assembly assembly = typeof(ResourceHelper).Assembly;
			string fullNamePrefix = "WeifenLuo.WinFormsUI.Resources.";
			return new Bitmap(assembly.GetManifestResourceStream(fullNamePrefix + name));
		}

		public static string GetString(string name)
		{
			return m_resourceManager.GetString(name);
		}
	}
}
