// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, Features, SignatureHelp\ISignatureHelpProvider.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using System.Threading;

namespace Altaxo.CodeEditing.SignatureHelp
{
	public interface ISignatureHelpProvider
	{
		bool IsTriggerCharacter(char ch);

		bool IsRetriggerCharacter(char ch);

		Task<SignatureHelpItems> GetItemsAsync(Document document, int position, SignatureHelpTriggerInfo triggerInfo, CancellationToken cancellationToken = default(CancellationToken));
	}
}