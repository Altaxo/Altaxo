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
	public sealed class PersistentEvent : AbstractEvent
	{
		public PersistentEvent(BinaryReader reader, ClassProxyCollection classProxyCollection)
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

		public PersistentEvent(ClassProxyCollection classProxyCollection, IEvent e)
		{
			modifiers          = e.Modifiers;
			FullyQualifiedName = e.Name;
			if (e.Documentation != null) {
				Documentation = e.Documentation;
			} else {
				Documentation = String.Empty;
			}

			returnType         = new PersistentReturnType(classProxyCollection, e.ReturnType);
		}
	}
}
