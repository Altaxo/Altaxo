﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Description of GoToMemberAction.
	/// </summary>
	public class GoToMemberAction : IContextAction
	{
		public string Title { get; private set; }
		public IMember Member { get; private set; }
		
		public GoToMemberAction(IMember member, IAmbience ambience)
		{
			if (ambience == null)
				throw new ArgumentNullException("ambience");
			if (member == null)
				throw new ArgumentNullException("member");
			this.Member = member;
			this.Title = ambience.Convert(member);
		}
		
		public void Execute()
		{
			var cu = this.Member.CompilationUnit;
			var region = this.Member.Region;
			if (cu == null || cu.FileName == null || region == null || region.IsEmpty)
				return;
			FileService.JumpToFilePosition(cu.FileName, region.BeginLine, region.BeginColumn);
		}
	}
}
