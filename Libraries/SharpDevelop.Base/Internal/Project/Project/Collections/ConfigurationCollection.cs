// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;

namespace ICSharpCode.SharpDevelop.Internal.Project.Collections
{
	
	/// <summary>
	///     <para>
	///       A collection that stores <see cref='.IConfiguration'/> objects.
	///    </para>
	/// </summary>
	/// <seealso cref='.ConfigurationCollection'/>
	[Serializable()]
	public class ConfigurationCollection : CollectionBase {
		
		public event EventHandler ItemAdded;
		public event EventHandler ItemRemoved;
		
		protected void OnItemRemoved()
		{
			if(ItemRemoved != null) {
				ItemRemoved(this, EventArgs.Empty);
			}
		}
		
		protected void OnItemAdded()
		{
			if(ItemAdded != null) {
				ItemAdded(this, EventArgs.Empty);
			}
		}
		
		/// <summary>
		///     <para>
		///       Initializes a new instance of <see cref='.ConfigurationCollection'/>.
		///    </para>
		/// </summary>
		public ConfigurationCollection()
		{
		}
		
		/// <summary>
		///     <para>
		///       Initializes a new instance of <see cref='.ConfigurationCollection'/> based on another <see cref='.ConfigurationCollection'/>.
		///    </para>
		/// </summary>
		/// <param name='value'>
		///       A <see cref='.ConfigurationCollection'/> from which the contents are copied
		/// </param>
		public ConfigurationCollection(ConfigurationCollection val)
		{
			this.AddRange(val);
			OnItemAdded();
		}
		
		/// <summary>
		///     <para>
		///       Initializes a new instance of <see cref='.ConfigurationCollection'/> containing any array of <see cref='.IConfiguration'/> objects.
		///    </para>
		/// </summary>
		/// <param name='value'>
		///       A array of <see cref='.IConfiguration'/> objects with which to intialize the collection
		/// </param>
		public ConfigurationCollection(IConfiguration[] val)
		{
			this.AddRange(val);
		}
		
		/// <summary>
		/// <para>Represents the entry at the specified index of the <see cref='.IConfiguration'/>.</para>
		/// </summary>
		/// <param name='index'><para>The zero-based index of the entry to locate in the collection.</para></param>
		/// <value>
		///    <para> The entry at the specified index of the collection.</para>
		/// </value>
		/// <exception cref='System.ArgumentOutOfRangeException'><paramref name='index'/> is outside the valid range of indexes for the collection.</exception>
		public IConfiguration this[int index] {
			get {
				return ((IConfiguration)(List[index]));
			}
			set {
				List[index] = value;
			}
		}
		
		/// <summary>
		///    <para>Adds a <see cref='.IConfiguration'/> with the specified value to the 
		///    <see cref='.ConfigurationCollection'/> .</para>
		/// </summary>
		/// <param name='value'>The <see cref='.IConfiguration'/> to add.</param>
		/// <returns>
		///    <para>The index at which the new element was inserted.</para>
		/// </returns>
		/// <seealso cref='.ConfigurationCollection.AddRange'/>
		public int Add(IConfiguration val)
		{
			int index = List.Add(val);
			OnItemAdded();
			return index;
		}
		
		/// <summary>
		/// <para>Copies the elements of an array to the end of the <see cref='.ConfigurationCollection'/>.</para>
		/// </summary>
		/// <param name='value'>
		///    An array of type <see cref='.IConfiguration'/> containing the objects to add to the collection.
		/// </param>
		/// <returns>
		///   <para>None.</para>
		/// </returns>
		/// <seealso cref='.ConfigurationCollection.Add'/>
		public void AddRange(IConfiguration[] val)
		{
			for (int i = 0; i < val.Length; i++) {
				this.Add(val[i]);
			}
			OnItemAdded();
		}
		
		/// <summary>
		///     <para>
		///       Adds the contents of another <see cref='.ConfigurationCollection'/> to the end of the collection.
		///    </para>
		/// </summary>
		/// <param name='value'>
		///    A <see cref='.ConfigurationCollection'/> containing the objects to add to the collection.
		/// </param>
		/// <returns>
		///   <para>None.</para>
		/// </returns>
		/// <seealso cref='.ConfigurationCollection.Add'/>
		public void AddRange(ConfigurationCollection val)
		{
			for (int i = 0; i < val.Count; i++)
			{
				this.Add(val[i]);
			}
			OnItemAdded();
		}
		
		/// <summary>
		/// <para>Gets a value indicating whether the 
		///    <see cref='.ConfigurationCollection'/> contains the specified <see cref='.IConfiguration'/>.</para>
		/// </summary>
		/// <param name='value'>The <see cref='.IConfiguration'/> to locate.</param>
		/// <returns>
		/// <para><see langword='true'/> if the <see cref='.IConfiguration'/> is contained in the collection; 
		///   otherwise, <see langword='false'/>.</para>
		/// </returns>
		/// <seealso cref='.ConfigurationCollection.IndexOf'/>
		public bool Contains(IConfiguration val)
		{
			return List.Contains(val);
		}
		
		/// <summary>
		/// <para>Copies the <see cref='.ConfigurationCollection'/> values to a one-dimensional <see cref='System.Array'/> instance at the 
		///    specified index.</para>
		/// </summary>
		/// <param name='array'><para>The one-dimensional <see cref='System.Array'/> that is the destination of the values copied from <see cref='.ConfigurationCollection'/> .</para></param>
		/// <param name='index'>The index in <paramref name='array'/> where copying begins.</param>
		/// <returns>
		///   <para>None.</para>
		/// </returns>
		/// <exception cref='System.ArgumentException'><para><paramref name='array'/> is multidimensional.</para> <para>-or-</para> <para>The number of elements in the <see cref='.ConfigurationCollection'/> is greater than the available space between <paramref name='arrayIndex'/> and the end of <paramref name='array'/>.</para></exception>
		/// <exception cref='System.ArgumentNullException'><paramref name='array'/> is <see langword='null'/>. </exception>
		/// <exception cref='System.ArgumentOutOfRangeException'><paramref name='arrayIndex'/> is less than <paramref name='array'/>'s lowbound. </exception>
		/// <seealso cref='System.Array'/>
		public void CopyTo(IConfiguration[] array, int index)
		{
			List.CopyTo(array, index);
		}
		
		/// <summary>
		///    <para>Returns the index of a <see cref='.IConfiguration'/> in 
		///       the <see cref='.ConfigurationCollection'/> .</para>
		/// </summary>
		/// <param name='value'>The <see cref='.IConfiguration'/> to locate.</param>
		/// <returns>
		/// <para>The index of the <see cref='.IConfiguration'/> of <paramref name='value'/> in the 
		/// <see cref='.ConfigurationCollection'/>, if found; otherwise, -1.</para>
		/// </returns>
		/// <seealso cref='.ConfigurationCollection.Contains'/>
		public int IndexOf(IConfiguration val)
		{
			return List.IndexOf(val);
		}
		
		/// <summary>
		/// <para>Inserts a <see cref='.IConfiguration'/> into the <see cref='.ConfigurationCollection'/> at the specified index.</para>
		/// </summary>
		/// <param name='index'>The zero-based index where <paramref name='value'/> should be inserted.</param>
		/// <param name=' value'>The <see cref='.IConfiguration'/> to insert.</param>
		/// <returns><para>None.</para></returns>
		/// <seealso cref='.ConfigurationCollection.Add'/>
		public void Insert(int index, IConfiguration val)
		{
			List.Insert(index, val);
			OnItemAdded();
		}
		
		/// <summary>
		///    <para>Returns an enumerator that can iterate through 
		///       the <see cref='.ConfigurationCollection'/> .</para>
		/// </summary>
		/// <returns><para>None.</para></returns>
		/// <seealso cref='System.Collections.IEnumerator'/>
		public new IConfigurationEnumerator GetEnumerator()
		{
			return new IConfigurationEnumerator(this);
		}
		
		/// <summary>
		///    <para> Removes a specific <see cref='.IConfiguration'/> from the 
		///    <see cref='.ConfigurationCollection'/> .</para>
		/// </summary>
		/// <param name='value'>The <see cref='.IConfiguration'/> to remove from the <see cref='.ConfigurationCollection'/> .</param>
		/// <returns><para>None.</para></returns>
		/// <exception cref='System.ArgumentException'><paramref name='value'/> is not found in the Collection. </exception>
		public void Remove(IConfiguration val)
		{
			List.Remove(val);
			OnItemRemoved();
		}
		
		public class IConfigurationEnumerator : IEnumerator
		{
			IEnumerator baseEnumerator;
			IEnumerable temp;
			
			public IConfigurationEnumerator(ConfigurationCollection mappings)
			{
				this.temp = ((IEnumerable)(mappings));
				this.baseEnumerator = temp.GetEnumerator();
			}
			
			public IConfiguration Current {
				get {
					return ((IConfiguration)(baseEnumerator.Current));
				}
			}
			
			object IEnumerator.Current {
				get {
					return baseEnumerator.Current;
				}
			}
			
			public bool MoveNext()
			{
				return baseEnumerator.MoveNext();
			}
			
			bool IEnumerator.MoveNext()
			{
				return baseEnumerator.MoveNext();
			}
			
			public void Reset()
			{
				baseEnumerator.Reset();
			}
			
			void IEnumerator.Reset()
			{
				baseEnumerator.Reset();
			}
		}
	}
}
