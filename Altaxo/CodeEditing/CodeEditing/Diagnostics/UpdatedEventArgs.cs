// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, Diagnostics/UpdatedEventArgs.cs

using System;
using Microsoft.CodeAnalysis;

namespace Altaxo.CodeEditing.Diagnostics
{
	public class UpdatedEventArgs : EventArgs
	{
		public object Id { get; }

		public Workspace Workspace { get; }

		public ProjectId ProjectId { get; }

		public DocumentId DocumentId { get; }

		internal UpdatedEventArgs(Microsoft.CodeAnalysis.Common.UpdatedEventArgs inner)
		{
			Id = inner.Id;
			Workspace = inner.Workspace;
			ProjectId = inner.ProjectId;
			DocumentId = inner.DocumentId;
		}
	}
}