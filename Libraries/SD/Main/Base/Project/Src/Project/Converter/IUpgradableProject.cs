﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Project.Converter
{
	/// <summary>
	/// A project with support for the UpgradeView
	/// </summary>
	public interface IUpgradableProject
	{
		/// <summary>
		/// Gets the project name.
		/// </summary>
		string Name { get; }
		
		/// <summary>
		/// Gets whether an upgrade is desired (controls whether the upgrade view should pop
		/// up automatically)
		/// </summary>
		bool UpgradeDesired { get; }
		
		/// <summary>
		/// Gets the supported compiler versions.
		/// </summary>
		IEnumerable<CompilerVersion> GetAvailableCompilerVersions();
		
		/// <summary>
		/// Gets the current compiler version.
		/// </summary>
		CompilerVersion CurrentCompilerVersion { get; }
		
		/// <summary>
		/// Gets the current target framework.
		/// </summary>
		TargetFramework CurrentTargetFramework { get; }
		
		/// <summary>
		/// Upgrades the selected compiler and target framework.
		/// </summary>
		/// <param name="newVersion">The new compiler version. If this property is null, the compiler version is not changed.</param>
		/// <param name="newFramework">The new target framework. If this property is null, the target framework is not changed.</param>
		void UpgradeProject(CompilerVersion newVersion, TargetFramework newFramework);
	}
	
	public class CompilerVersion
	{
		public Version MSBuildVersion { get; private set; }
		public string DisplayName { get; private set; }
		
		public virtual IEnumerable<TargetFramework> GetSupportedTargetFrameworks()
		{
			return from fx in TargetFramework.TargetFrameworks
				where fx.MinimumMSBuildVersion != null
				where MSBuildVersion >= fx.MinimumMSBuildVersion
				select fx;
		}
		
		public CompilerVersion(Version msbuildVersion, string displayName)
		{
			if (msbuildVersion == null)
				throw new ArgumentNullException("msbuildVersion");
			this.MSBuildVersion = msbuildVersion;
			this.DisplayName = displayName;
		}
		
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (GetType() != obj.GetType())
				return false;
			CompilerVersion v = (CompilerVersion)obj;
			return this.MSBuildVersion == v.MSBuildVersion;
		}
		
		public override int GetHashCode()
		{
			return MSBuildVersion.GetHashCode();
		}
		
		public override string ToString()
		{
			return DisplayName;
		}
	}
}
