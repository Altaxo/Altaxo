// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.SharpDevelop.Services 
{
	public delegate void ConfigurationEventHandler(object sender, ConfigurationEventArgs e);
	
	public class ConfigurationEventArgs : EventArgs
	{ 
		IConfiguration config;
		
		public IConfiguration ActiveConfiguration
		{
			get {
				return config;
			}
		}

		public ConfigurationEventArgs(IConfiguration config)
		{
			this.config = config;
		}
	}
}
