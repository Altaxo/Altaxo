// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, EditorFeatures, Host/IStreamingFindReferencesPresenter.cs

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.FindUsages;
using System;

namespace Altaxo.CodeEditing.GoToDefinition
{
	internal static class IStreamingFindUsagesPresenterExtensions
	{
		/// <summary>
		/// If there's only a single item, navigates to it.  Otherwise, presents all the
		/// items to the user.
		/// </summary>
		public static async Task<bool> TryNavigateToOrPresentItemsAsync(
				object /* this IStreamingFindUsagesPresenter */ presenter,
				string title,
				ImmutableArray<DefinitionItem> items, bool alwaysShowDeclarations)
		{
			// Ignore any definitions that we can't navigate to.
			var definitions = items.WhereAsArray(d => d.CanNavigateTo());

			// See if there's a third party external item we can navigate to.  If so, defer
			// to that item and finish.
			var externalItems = definitions.WhereAsArray(d => d.IsExternal);
			foreach (var item in externalItems)
			{
				if (item.TryNavigateTo())
				{
					return true;
				}
			}

			var nonExternalItems = definitions.WhereAsArray(d => !d.IsExternal);
			if (nonExternalItems.Length == 0)
			{
				return false;
			}

			if (nonExternalItems.Length == 1 &&
					nonExternalItems[0].SourceSpans.Length <= 1)
			{
				// There was only one location to navigate to.  Just directly go to that location.
				return nonExternalItems[0].TryNavigateTo();
			}

			if (presenter != null)
			{
				throw new NotImplementedException("Found multiple locations, this is not implemented yet");

				/*
				// We have multiple definitions, or we have definitions with multiple locations.
				// Present this to the user so they can decide where they want to go to.

				var context = presenter.StartSearch(title, alwaysShowDeclarations);
				foreach (var definition in nonExternalItems)
				{
					await context.OnDefinitionFoundAsync(definition).ConfigureAwait(false);
				}

				// Note: we don't need to put this in a finally.  The only time we might not hit
				// this is if cancellation or another error gets thrown.  In the former case,
				// that means that a new search has started.  We don't care about telling the
				// context it has completed.  In the latter case somethign wrong has happened
				// and we don't want to run any more code code in this particular context.
				await context.OnCompletedAsync().ConfigureAwait(false);
				*/
			}

			return true;
		}
	}

	public static class ImmutableArrayExtensions
	{
		/// <summary>
		/// Creates a new immutable array based on filtered elements by the predicate. The array must not be null.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array">The array to process</param>
		/// <param name="predicate">The delegate that defines the conditions of the element to search for.</param>
		/// <returns></returns>
		public static ImmutableArray<T> WhereAsArray<T>(this ImmutableArray<T> array, Func<T, bool> predicate)
		{
			ImmutableArray<T>.Builder builder = null;
			bool none = true;
			bool all = true;

			int n = array.Length;
			for (int i = 0; i < n; i++)
			{
				var a = array[i];
				if (predicate(a))
				{
					none = false;
					if (all)
					{
						continue;
					}

					if (builder == null)
					{
						builder = ImmutableArray.CreateBuilder<T>();
					}

					builder.Add(a);
				}
				else
				{
					if (none)
					{
						all = false;
						continue;
					}

					if (all)
					{
						all = false;
						builder = ImmutableArray.CreateBuilder<T>();
						for (int j = 0; j < i; j++)
						{
							builder.Add(array[j]);
						}
					}
				}
			}

			if (builder != null)
			{
				return builder.ToImmutable();
			}
			else if (all)
			{
				return array;
			}
			else
			{
				return ImmutableArray<T>.Empty;
			}
		}
	}
}