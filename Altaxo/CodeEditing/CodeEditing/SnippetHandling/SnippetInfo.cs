// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, Features, Snippets\SnippetInfo.cs

namespace Altaxo.CodeEditing.SnippetHandling
{
	public sealed class SnippetInfo
	{
		public string Shortcut { get; }

		public string Title { get; }

		public string Description { get; }

		public SnippetInfo(string shortcut, string title, string description)
		{
			Shortcut = shortcut;
			Title = title;
			Description = description;
		}
	}
}