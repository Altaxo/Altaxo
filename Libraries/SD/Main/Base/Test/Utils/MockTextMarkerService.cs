// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 5674 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;

namespace ICSharpCode.SharpDevelop.Tests.Utils
{
	/// <summary>
	/// Description of MockTextEditor.
	/// </summary>
	public class MockTextMarkerService : ITextMarkerService
	{
		public static IDocument CreateDocumentWithMockService()
		{
			ServiceContainer container = new ServiceContainer();
			container.AddService(typeof(ITextMarkerService), new MockTextMarkerService());
			
			return new AvalonEditDocumentAdapter(new ICSharpCode.AvalonEdit.Document.TextDocument(), container);
		}
		
		List<ITextMarker> markers;
		
		public MockTextMarkerService()
		{
			this.markers = new List<ITextMarker>();
		}
		
		public System.Collections.Generic.IEnumerable<ITextMarker> TextMarkers {
			get {
				return this.markers;
			}
		}
		
		public ITextMarker Create(int startOffset, int length)
		{
			ITextMarker m = new MockTextMarker(this.markers, startOffset, startOffset + length, length);
			this.markers.Add(m);
			return m;
		}
		
		public void Remove(ITextMarker marker)
		{
			marker.Delete();
		}
		
		public void RemoveAll(Predicate<ITextMarker> predicate)
		{
			foreach (ITextMarker m in markers.ToArray()) {
				if (predicate(m))
					m.Delete();
			}
		}
	}
}
