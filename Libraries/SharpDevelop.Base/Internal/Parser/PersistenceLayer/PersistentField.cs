// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.IO;
using System.Reflection;
using ICSharpCode.SharpDevelop.Services;

namespace SharpDevelop.Internal.Parser
{
	public sealed class PersistentField : AbstractField
	{
		public PersistentField(BinaryReader reader, ClassProxyCollection classProxyCollection)
		{
			FullyQualifiedName = reader.ReadString();
			Documentation      = reader.ReadString();
			modifiers          = (ModifierEnum)reader.ReadUInt32();
			
			returnType         = new PersistentReturnType(reader, classProxyCollection);
			if (returnType.Name == null) {
				returnType = null;
			}
		}
		
		public void WriteTo(BinaryWriter writer)
		{
			writer.Write(FullyQualifiedName);
			writer.Write(Documentation);
			writer.Write((uint)modifiers);
			((PersistentReturnType)returnType).WriteTo(writer);
		}
		
		public PersistentField(ClassProxyCollection classProxyCollection, IField field)
		{
			modifiers          = field.Modifiers;
			FullyQualifiedName = field.Name;
			
			if (field.Documentation != null) {
				Documentation = field.Documentation;
			}  else {
				Documentation = String.Empty;
			}
			returnType         = new PersistentReturnType(classProxyCollection, field.ReturnType);
		}
	}
}
