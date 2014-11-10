// DescriptorCollection.cs
// Copyright (C) 2004 Mike Krueger
// 
// This program is free software. It is dual licensed under GNU GPL and GNU LGPL.
// See COPYING_GPL.txt and COPYING_LGPL.txt for details.
//

using System;
using System.Collections;

namespace ICSharpCode.USBlib
{
	/// <summary>
	///   A collection that stores <see cref='Descriptor'/> objects.
	/// </summary>
	[Serializable()]
	public class DescriptorCollection : CollectionBase {
		
		/// <summary>
		///   Initializes a new instance of <see cref='DescriptorCollection'/>.
		/// </summary>
		public DescriptorCollection()
		{
		}
		
		/// <summary>
		///   Initializes a new instance of <see cref='DescriptorCollection'/> based on another <see cref='DescriptorCollection'/>.
		/// </summary>
		/// <param name='val'>
		///   A <see cref='DescriptorCollection'/> from which the contents are copied
		/// </param>
		public DescriptorCollection(DescriptorCollection val)
		{
			this.AddRange(val);
		}
		
		/// <summary>
		///   Initializes a new instance of <see cref='DescriptorCollection'/> containing any array of <see cref='Descriptor'/> objects.
		/// </summary>
		/// <param name='val'>
		///       A array of <see cref='Descriptor'/> objects with which to intialize the collection
		/// </param>
		public DescriptorCollection(Descriptor[] val)
		{
			this.AddRange(val);
		}
		
		/// <summary>
		///   Represents the entry at the specified index of the <see cref='Descriptor'/>.
		/// </summary>
		/// <param name='index'>The zero-based index of the entry to locate in the collection.</param>
		/// <value>The entry at the specified index of the collection.</value>
		/// <exception cref='ArgumentOutOfRangeException'><paramref name='index'/> is outside the valid range of indexes for the collection.</exception>
		public Descriptor this[int index] {
			get {
				return ((Descriptor)(List[index]));
			}
			set {
				List[index] = value;
			}
		}
		
		/// <summary>
		///   Adds a <see cref='Descriptor'/> with the specified value to the 
		///   <see cref='DescriptorCollection'/>.
		/// </summary>
		/// <param name='val'>The <see cref='Descriptor'/> to add.</param>
		/// <returns>The index at which the new element was inserted.</returns>
		/// <seealso cref='DescriptorCollection.AddRange'/>
		public int Add(Descriptor val)
		{
			return List.Add(val);
		}
		
		/// <summary>
		///   Copies the elements of an array to the end of the <see cref='DescriptorCollection'/>.
		/// </summary>
		/// <param name='val'>
		///    An array of type <see cref='Descriptor'/> containing the objects to add to the collection.
		/// </param>
		/// <seealso cref='DescriptorCollection.Add'/>
		public void AddRange(Descriptor[] val)
		{
			for (int i = 0; i < val.Length; i++) {
				this.Add(val[i]);
			}
		}
		
		/// <summary>
		///   Adds the contents of another <see cref='DescriptorCollection'/> to the end of the collection.
		/// </summary>
		/// <param name='val'>
		///    A <see cref='DescriptorCollection'/> containing the objects to add to the collection.
		/// </param>
		/// <seealso cref='DescriptorCollection.Add'/>
		public void AddRange(DescriptorCollection val)
		{
			for (int i = 0; i < val.Count; i++)
			{
				this.Add(val[i]);
			}
		}
		
		/// <summary>
		///   Gets a value indicating whether the 
		///    <see cref='DescriptorCollection'/> contains the specified <see cref='Descriptor'/>.
		/// </summary>
		/// <param name='val'>The <see cref='Descriptor'/> to locate.</param>
		/// <returns>
		/// <see langword='true'/> if the <see cref='Descriptor'/> is contained in the collection; 
		///   otherwise, <see langword='false'/>.
		/// </returns>
		/// <seealso cref='DescriptorCollection.IndexOf'/>
		public bool Contains(Descriptor val)
		{
			return List.Contains(val);
		}
		
		/// <summary>
		///   Copies the <see cref='DescriptorCollection'/> values to a one-dimensional <see cref='Array'/> instance at the 
		///    specified index.
		/// </summary>
		/// <param name='array'>The one-dimensional <see cref='Array'/> that is the destination of the values copied from <see cref='DescriptorCollection'/>.</param>
		/// <param name='index'>The index in <paramref name='array'/> where copying begins.</param>
		/// <exception cref='ArgumentException'>
		///   <para><paramref name='array'/> is multidimensional.</para>
		///   <para>-or-</para>
		///   <para>The number of elements in the <see cref='DescriptorCollection'/> is greater than
		///         the available space between <paramref name='arrayIndex'/> and the end of
		///         <paramref name='array'/>.</para>
		/// </exception>
		/// <exception cref='ArgumentNullException'><paramref name='array'/> is <see langword='null'/>. </exception>
		/// <exception cref='ArgumentOutOfRangeException'><paramref name='arrayIndex'/> is less than <paramref name='array'/>'s lowbound. </exception>
		/// <seealso cref='Array'/>
		public void CopyTo(Descriptor[] array, int index)
		{
			List.CopyTo(array, index);
		}
		
		/// <summary>
		///    Returns the index of a <see cref='Descriptor'/> in 
		///       the <see cref='DescriptorCollection'/>.
		/// </summary>
		/// <param name='val'>The <see cref='Descriptor'/> to locate.</param>
		/// <returns>
		///   The index of the <see cref='Descriptor'/> of <paramref name='val'/> in the 
		///   <see cref='DescriptorCollection'/>, if found; otherwise, -1.
		/// </returns>
		/// <seealso cref='DescriptorCollection.Contains'/>
		public int IndexOf(Descriptor val)
		{
			return List.IndexOf(val);
		}
		
		/// <summary>
		///   Inserts a <see cref='Descriptor'/> into the <see cref='DescriptorCollection'/> at the specified index.
		/// </summary>
		/// <param name='index'>The zero-based index where <paramref name='val'/> should be inserted.</param>
		/// <param name='val'>The <see cref='Descriptor'/> to insert.</param>
		/// <seealso cref='DescriptorCollection.Add'/>
		public void Insert(int index, Descriptor val)
		{
			List.Insert(index, val);
		}
		
		/// <summary>
		///  Returns an enumerator that can iterate through the <see cref='DescriptorCollection'/>.
		/// </summary>
		/// <seealso cref='IEnumerator'/>
		public new DescriptorEnumerator GetEnumerator()
		{
			return new DescriptorEnumerator(this);
		}
		
		/// <summary>
		///   Removes a specific <see cref='Descriptor'/> from the <see cref='DescriptorCollection'/>.
		/// </summary>
		/// <param name='val'>The <see cref='Descriptor'/> to remove from the <see cref='DescriptorCollection'/>.</param>
		/// <exception cref='ArgumentException'><paramref name='val'/> is not found in the Collection.</exception>
		public void Remove(Descriptor val)
		{
			List.Remove(val);
		}
		
		/// <summary>
		///   Enumerator that can iterate through a DescriptorCollection.
		/// </summary>
		/// <seealso cref='IEnumerator'/>
		/// <seealso cref='DescriptorCollection'/>
		/// <seealso cref='Descriptor'/>
		public class DescriptorEnumerator : IEnumerator
		{
			IEnumerator baseEnumerator;
			IEnumerable temp;
			
			/// <summary>
			///   Initializes a new instance of <see cref='DescriptorEnumerator'/>.
			/// </summary>
			public DescriptorEnumerator(DescriptorCollection mappings)
			{
				this.temp = ((IEnumerable)(mappings));
				this.baseEnumerator = temp.GetEnumerator();
			}
			
			/// <summary>
			///   Gets the current <see cref='Descriptor'/> in the <seealso cref='DescriptorCollection'/>.
			/// </summary>
			public Descriptor Current {
				get {
					return ((Descriptor)(baseEnumerator.Current));
				}
			}
			
			object IEnumerator.Current {
				get {
					return baseEnumerator.Current;
				}
			}
			
			/// <summary>
			///   Advances the enumerator to the next <see cref='Descriptor'/> of the <see cref='DescriptorCollection'/>.
			/// </summary>
			public bool MoveNext()
			{
				return baseEnumerator.MoveNext();
			}
			
			/// <summary>
			///   Sets the enumerator to its initial position, which is before the first element in the <see cref='DescriptorCollection'/>.
			/// </summary>
			public void Reset()
			{
				baseEnumerator.Reset();
			}
		}
	}
}
