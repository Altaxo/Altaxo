// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 4142 $</version>
// </file>

using System;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Rendering
{
	/// <summary>
	/// Contains weak event managers for the TextView events.
	/// </summary>
	public static class TextViewWeakEventManager
	{
		/// <summary>
		/// Weak event manager for the <see cref="TextView.DocumentChanged"/> event.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public sealed class DocumentChanged : WeakEventManagerBase<DocumentChanged, TextView>
		{
			/// <inheritdoc/>
			protected override void StartListening(TextView source)
			{
				source.DocumentChanged += DeliverEvent;
			}
			
			/// <inheritdoc/>
			protected override void StopListening(TextView source)
			{
				source.DocumentChanged -= DeliverEvent;
			}
		}
		
		/// <summary>
		/// Weak event manager for the <see cref="TextView.VisualLinesChanged"/> event.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public sealed class VisualLinesChanged : WeakEventManagerBase<VisualLinesChanged, TextView>
		{
			/// <inheritdoc/>
			protected override void StartListening(TextView source)
			{
				source.VisualLinesChanged += DeliverEvent;
			}
			
			/// <inheritdoc/>
			protected override void StopListening(TextView source)
			{
				source.VisualLinesChanged -= DeliverEvent;
			}
		}
		
		/// <summary>
		/// Weak event manager for the <see cref="TextView.ScrollOffsetChanged"/> event.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public sealed class ScrollOffsetChanged : WeakEventManagerBase<ScrollOffsetChanged, TextView>
		{
			/// <inheritdoc/>
			protected override void StartListening(TextView source)
			{
				source.ScrollOffsetChanged += DeliverEvent;
			}
			
			/// <inheritdoc/>
			protected override void StopListening(TextView source)
			{
				source.ScrollOffsetChanged -= DeliverEvent;
			}
		}
	}
}
