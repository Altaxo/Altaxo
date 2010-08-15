﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 5529 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ICSharpCode.SharpDevelop.Dom
{
	public class ParameterListComparer : IEqualityComparer<IMethod>
	{
		public bool Equals(IMethod x, IMethod y)
		{
			if (GetHashCode(x) != GetHashCode(y))
				return false;
			var paramsX = x.Parameters;
			var paramsY = y.Parameters;
			if (paramsX.Count != paramsY.Count)
				return false;
			if (x.TypeParameters.Count != y.TypeParameters.Count)
				return false;
			for (int i = 0; i < paramsX.Count; i++) {
				IParameter px = paramsX[i];
				IParameter py = paramsY[i];
				if ((px.IsOut || px.IsRef) != (py.IsOut || py.IsRef))
					return false;
				if (!object.Equals(px.ReturnType, py.ReturnType))
					return false;
			}
			return true;
		}
		
		Dictionary<IMethod, int> cachedHashes = new Dictionary<IMethod, int>();
		
		public int GetHashCode(IMethod obj)
		{
			int hashCode;
			if (cachedHashes.TryGetValue(obj, out hashCode))
				return hashCode;
			hashCode = obj.TypeParameters.Count;
			unchecked {
				foreach (IParameter p in obj.Parameters) {
					hashCode *= 1000000579;
					if (p.IsOut || p.IsRef)
						hashCode += 1;
					if (p.ReturnType != null) {
						hashCode += p.ReturnType.GetHashCode();
					}
				}
			}
			cachedHashes[obj] = hashCode;
			return hashCode;
		}
	}
}
