using System;
using System.Text;
using System.Drawing;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class SharpDevelopTextEditorProperties : ITextEditorProperties
	{
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		IProperties properties;
		
		static SharpDevelopTextEditorProperties()
		{
			IProperties properties2 = ((IProperties)propertyService.GetProperty("ICSharpCode.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new DefaultProperties()));
			FontContainer.DefaultFont = FontContainer.ParseFont(properties2.GetProperty("DefaultFont", new Font("Courier New", 10).ToString()));
			properties2.PropertyChanged += new PropertyEventHandler(CheckFontChange);
		}
		
		static void CheckFontChange(object sender, PropertyEventArgs e)
		{
			if (e.Key == "DefaultFont") {
				FontContainer.DefaultFont = FontContainer.ParseFont(e.NewValue.ToString());
			}
		}
		
		public SharpDevelopTextEditorProperties()
		{
			properties                      = ((IProperties)propertyService.GetProperty("ICSharpCode.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new DefaultProperties()));
			
		}
		
		public int TabIndent {
			get {
				return properties.GetProperty("TabIndent", 4);

			}
			set {
				properties.SetProperty("TabIndent", value);
			}
		}
		public IndentStyle IndentStyle {
			get {
				return (IndentStyle)properties.GetProperty("IndentStyle", IndentStyle.Smart);
			}
			set {
				properties.SetProperty("IndentStyle", value);
			}
		}
		
		public DocumentSelectionMode DocumentSelectionMode {
			get {
				return (DocumentSelectionMode)properties.GetProperty("DocumentSelectionMode", DocumentSelectionMode.Normal);
			}
			set {
				properties.SetProperty("DocumentSelectionMode", value);
			}
		}
		public bool AllowCaretBeyondEOL {
			get {
				return properties.GetProperty("CursorBehindEOL", false);
			}
			set {
				properties.SetProperty("CursorBehindEOL", value);
			}
		}
		public bool ShowMatchingBracket {
			get {
				return properties.GetProperty("ShowBracketHighlight", true);
			}
			set {
				properties.SetProperty("ShowBracketHighlight", value);
			}
		}
		public bool ShowLineNumbers {
			get {
				return properties.GetProperty("ShowLineNumbers", true);
			}
			set {
				properties.SetProperty("ShowLineNumbers", value);
			}
		}
		public bool ShowSpaces {
			get {
				return properties.GetProperty("ShowSpaces", false);
			}
			set {
				properties.SetProperty("ShowSpaces", value);
			}
		}
		public bool ShowTabs {
			get {
				return properties.GetProperty("ShowTabs", false);
			}
			set {
				properties.GetProperty("ShowTabs", value);
			}
		}
		public bool ShowEOLMarker {
			get {
				return properties.GetProperty("ShowEOLMarkers", false);
			}
			set {
				properties.SetProperty("ShowEOLMarkers", value);
			}
		}
		public bool ShowInvalidLines {
			get {
				return properties.GetProperty("ShowInvalidLines", false);
			}
			set {
				properties.SetProperty("ShowInvalidLines", value);
			}
		}
		public bool IsIconBarVisible {
			get {
				return properties.GetProperty("IconBarVisible", true);
			}
			set {
				properties.SetProperty("IconBarVisible", value);
			}
		}
		public bool EnableFolding {
			get {
				return properties.GetProperty("EnableFolding", true);
			}
			set {
				properties.SetProperty("EnableFolding", value);
			}
		}
		public bool ShowHorizontalRuler {
			get {
				return properties.GetProperty("ShowHRuler", false);
			}
			set {
				properties.SetProperty("ShowHRuler", value);
			}
		}
		public bool ShowVerticalRuler {
			get {
				return properties.GetProperty("ShowVRuler", false);
			}
			set {
				properties.SetProperty("ShowVRuler", value);
			}
		}
		public bool ConvertTabsToSpaces {
			get {
				return properties.GetProperty("TabsToSpaces", false);
			}
			set {
				properties.SetProperty("TabsToSpaces", value);
			}
		}
		public bool UseAntiAliasedFont {
			get {
				return properties.GetProperty("UseAntiAliasFont", false);
			}
			set {
				properties.SetProperty("UseAntiAliasFont", value);
			}
		}
		public bool CreateBackupCopy {
			get {
				return properties.GetProperty("CreateBackupCopy", false);
			}
			set {
				properties.SetProperty("CreateBackupCopy", value);
			}
		}
		public bool MouseWheelScrollDown {
			get {
				return properties.GetProperty("MouseWheelScrollDown", false);
			}
			set {
				properties.SetProperty("MouseWheelScrollDown", value);
			}
		}
		public bool HideMouseCursor {
			get {
				return properties.GetProperty("HideMouseCursor", false);
			}
			set {
				properties.SetProperty("HideMouseCursor", value);
			}
		}
		public Encoding Encoding {
			get {
				return Encoding.GetEncoding(properties.GetProperty("Encoding", 1252));
			}
			set {
				properties.SetProperty("Encoding", value.CodePage);
			}
		}
		
		public int VerticalRulerRow {
			get {
				return properties.GetProperty("VRulerRow", 80);
			}
			set {
				properties.SetProperty("VRulerRow", value);
			}
		}
		public LineViewerStyle LineViewerStyle {
			get {
				return (LineViewerStyle)properties.GetProperty("LineViewerStyle", LineViewerStyle.None);
			}
			set {
				properties.SetProperty("LineViewerStyle", value);
			}
		}
		public string LineTerminator {
			get {
				LineTerminatorStyle lineTerminatorStyle = (LineTerminatorStyle)propertyService.GetProperty("SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Windows);
				switch (lineTerminatorStyle) {
					case LineTerminatorStyle.Windows:
						return "\r\n";
					case LineTerminatorStyle.Macintosh:
						return "\r";
				}
				return "\n";
			}
			set {
				throw new System.NotImplementedException();
			}
		}
		public bool AutoInsertCurlyBracket {
			get {
				return properties.GetProperty("AutoInsertCurlyBracket", true);
			}
			set {
				properties.SetProperty("AutoInsertCurlyBracket", value);
			}
		}
		
		public Font Font {
			get {
				return FontContainer.ParseFont(properties.GetProperty("DefaultFont", new Font("Courier New", 10).ToString()));
			}
			set {
				properties.SetProperty("DefaultFont", value.ToString());
				FontContainer.DefaultFont = value;
			}
		}
		/*
		<Property key="DoubleBuffer" value="True" />
        <Property key="ShowErrors" value="True" />
        <Property key="EnableCodeCompletion" value="True" />
        <Property key="AutoInsertTemplates" value="True" />
        <Property key="IndentationSize" value="4" />		 * */
		
	}
}
