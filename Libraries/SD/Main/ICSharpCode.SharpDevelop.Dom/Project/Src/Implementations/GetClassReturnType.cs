// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2949 $</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// The GetClassReturnType is used when the class should be resolved on demand, but the
	/// full name is already known.
	/// </summary>
	public sealed class GetClassReturnType : ProxyReturnType
	{
		IProjectContent content;
		string fullName;
		string shortName;
		int typeArgumentCount;
		
		public GetClassReturnType(IProjectContent content, string fullName, int typeParameterCount)
		{
			this.content = content;
			this.typeArgumentCount = typeParameterCount;
			SetFullyQualifiedName(fullName);
		}
		
		public override bool IsDefaultReturnType {
			get {
				return true;
			}
		}
		
		public override int TypeArgumentCount {
			get {
				return typeArgumentCount;
			}
		}
		
		public override IReturnType BaseType {
			get {
				IClass c = content.GetClass(fullName, typeArgumentCount);
				return (c != null) ? c.DefaultReturnType : null;
			}
		}
		
		public override string FullyQualifiedName {
			get {
				return fullName;
			}
		}
		
		void SetFullyQualifiedName(string fullName)
		{
			if (fullName == null)
				throw new ArgumentNullException("fullName");
			this.fullName = fullName;
			int pos = fullName.LastIndexOf('.');
			if (pos < 0)
				shortName = fullName;
			else
				shortName = fullName.Substring(pos + 1);
		}
		
		public override string Name {
			get {
				return shortName;
			}
		}
		
		public override string Namespace {
			get {
				string tmp = base.Namespace;
				if (tmp == "?") {
					if (fullName.IndexOf('.') > 0)
						return fullName.Substring(0, fullName.LastIndexOf('.'));
					else
						return "";
				}
				return tmp;
			}
		}
		
		public override string DotNetName {
			get {
				string tmp = base.DotNetName;
				if (tmp == "?") {
					return fullName;
				}
				return tmp;
			}
		}
		
		public override string ToString()
		{
			return String.Format("[GetClassReturnType: {0}]", fullName);
		}
	}
}
