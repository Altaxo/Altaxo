// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 6329 $</version>
// </file>

using System.Windows.Controls;
using System.Windows.Documents;
using ICSharpCode.SharpDevelop.Dom;
using System;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	/// <summary>
	/// An insight item that represents an entity.
	/// </summary>
	public class MethodInsightItem : IInsightItem
	{
		IEntity entity;
		
		public MethodInsightItem(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			this.entity = entity;
			this.highlightParameter = -1;
		}
		
		public IEntity Entity {
			get { return entity; }
		}
		
		string headerText;
		bool descriptionCreated;
		string description;
		int highlightParameter;
		object fancyHeader;
		
		public int HighlightParameter {
			get { return highlightParameter; }
			set {
				if (highlightParameter != value) {
					highlightParameter = value;
					fancyHeader = GenerateHeader();
				}
			}
		}
		
		object GenerateHeader()
		{
			IAmbience ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = ConversionFlags.StandardConversionFlags;
			
			if (headerText == null) {
				headerText = ambience.Convert(entity);
			}
			
			if (highlightParameter < 0)
				return headerText;
			
			if (entity is IMethod) {
				var method = entity as IMethod;
				string param = "";
				if (method.Parameters.Count > highlightParameter)
					param = ambience.Convert(method.Parameters[highlightParameter]);
				
				if (!string.IsNullOrEmpty(param)) {
					string[] parts = headerText.Split(new[] { param }, StringSplitOptions.None);
					if (parts.Length != 2)
						return headerText;
					return new Span() {
						Inlines = {
							parts[0],
							new Bold() { Inlines = { param } },
							parts[1]
						}
					};
				}
			}
			
			return headerText;
		}
		
		public object Header {
			get {
				if (fancyHeader == null)
					fancyHeader = GenerateHeader();
				
				return fancyHeader;
			}
		}
		
		public object Content {
			get {
				if (!descriptionCreated) {
					string entityDoc = entity.Documentation;
					if (!string.IsNullOrEmpty(entityDoc)) {
						description = CodeCompletionItem.ConvertDocumentation(entityDoc);
					}
					descriptionCreated = true;
				}
				return description;
			}
		}
	}
}
