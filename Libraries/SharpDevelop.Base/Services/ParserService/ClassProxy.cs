// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

using ICSharpCode.Core.AddIns;

using SharpDevelop.Internal.Parser;
using ICSharpCode.SharpDevelop.Internal.Project;

using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Services
{
	public class ClassProxy : AbstractClass, IComparable
	{
		uint offset = 0;
		
		public uint Offset {
			get {
				return offset;
			}
			set {
				offset = value;
			}
		}
		
		/// <value>
		/// Class Proxies clases don't have a compilation unit.
		/// </value>
		public override ICompilationUnit CompilationUnit {
			get {
				return null;
			}
		}
		
		public int CompareTo(object obj) 
		{
			return FullyQualifiedName.CompareTo(((ClassProxy)obj).FullyQualifiedName);
		}
		
		public ClassProxy(BinaryReader reader)
		{
			FullyQualifiedName = reader.ReadString();
			documentation      = reader.ReadString();
			offset             = reader.ReadUInt32();
			modifiers          = (ModifierEnum)reader.ReadUInt32();
			classType          = (ClassType)reader.ReadInt16();
		}
		
		public void WriteTo(BinaryWriter writer)
		{
			writer.Write(FullyQualifiedName);
			writer.Write(documentation);
			writer.Write(offset);
			writer.Write((uint)modifiers);
			writer.Write((short)classType);
		}
		
		public ClassProxy(IClass c)
		{
			this.FullyQualifiedName  = c.FullyQualifiedName;
			this.documentation       = c.Documentation;
			this.modifiers           = c.Modifiers;
			this.classType           = c.ClassType;
		}
	}
}
