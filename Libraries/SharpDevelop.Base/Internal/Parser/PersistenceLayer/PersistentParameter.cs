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
	public sealed class PersistentParameter : AbstractParameter
	{
		public PersistentParameter(BinaryReader reader, ClassProxyCollection classProxyCollection)
		{
			name     = reader.ReadString();
			documentation      = reader.ReadString();

			modifier = (ParameterModifier)reader.ReadByte();
						
			returnType = new PersistentReturnType(reader, classProxyCollection);
			if (returnType.Name == null) {
				returnType = null;
			}
		}
		
		public void WriteTo(BinaryWriter writer)
		{
			writer.Write(name);
			writer.Write(documentation);
			writer.Write((byte)modifier);
			((PersistentReturnType)returnType).WriteTo(writer);
		}
		
		public PersistentParameter(ClassProxyCollection classProxyCollection, IParameter param)
		{
			name          = param.Name;
			documentation = param.Documentation;
			if (documentation == null) {
				documentation = String.Empty;
			}
			
			returnType = new PersistentReturnType(classProxyCollection, param.ReturnType);
			modifier   = param.Modifier;
		}
	}
}
