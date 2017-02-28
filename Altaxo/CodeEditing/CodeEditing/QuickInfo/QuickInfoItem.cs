// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, EditorFeatures, Extensibility\QuickInfo\QuickInfoItem.cs

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Altaxo.CodeEditing.QuickInfo
{
	public sealed class QuickInfoItem
	{
		private readonly Func<object> _contentFactory;

		public TextSpan TextSpan { get; }

		public object Create() => _contentFactory();

		internal QuickInfoItem(TextSpan textSpan, Func<object> contentFactory)
		{
			TextSpan = textSpan;
			_contentFactory = contentFactory;
		}
	}
}