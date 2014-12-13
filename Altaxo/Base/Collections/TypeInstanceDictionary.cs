using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Collections
{
	public class TypeInstanceDictionary<T> : Dictionary<System.Type, T>
	{
		public void Set(T instance)
		{
			if (null == instance)
				throw new ArgumentNullException("instance");

			this[instance.GetType()] = instance;
		}

		public M GetOrDefault<M>() where M : T
		{
			if (this.ContainsKey(typeof(M)))
				return (M)this[typeof(M)];
			else
				return default(M);
		}

		public M GetOrCreate<M>() where M : T, new()
		{
			if (this.ContainsKey(typeof(M)))
				return (M)this[typeof(M)];
			else
				return new M();
		}

		public M GetOrCreate<M>(Func<M> creation) where M : T
		{
			if (null == creation)
				throw new ArgumentNullException("creation");

			if (this.ContainsKey(typeof(M)))
				return (M)this[typeof(M)];
			else
				return creation();
		}

		public void ModifyOrSet<M>(Action<M> modificationProc, M instanceToSet) where M : T
		{
			if (null == modificationProc)
				throw new ArgumentNullException("modificationProc");

			if (this.ContainsKey(typeof(M)))
			{
				var val = (M)this[typeof(M)];
				modificationProc(val);
				this[typeof(M)] = val;
			}
			else
			{
				this[typeof(M)] = instanceToSet;
			}
		}
	}
}