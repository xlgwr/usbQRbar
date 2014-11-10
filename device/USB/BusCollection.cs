// BusCollection.cs
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
	///   A collection that stores <see cref='Bus'/> objects.
	/// </summary>
	[Serializable()]
	public class BusCollection : CollectionBase {
		
		/// <summary>
		///   Initializes a new instance of <see cref='BusCollection'/>.
		/// </summary>
		public BusCollection()
		{
		}
		
		/// <summary>
		///   Initializes a new instance of <see cref='BusCollection'/> based on another <see cref='BusCollection'/>.
		/// </summary>
		/// <param name='val'>
		///   A <see cref='BusCollection'/> from which the contents are copied
		/// </param>
		public BusCollection(BusCollection val)
		{
			this.AddRange(val);
		}
		
		/// <summary>
		///   Initializes a new instance of <see cref='BusCollection'/> containing any array of <see cref='Bus'/> objects.
		/// </summary>
		/// <param name='val'>
		///       A array of <see cref='Bus'/> objects with which to intialize the collection
		/// </param>
		public BusCollection(Bus[] val)
		{
			this.AddRange(val);
		}
		
		/// <summary>
		///   Represents the entry at the specified index of the <see cref='Bus'/>.
		/// </summary>
		/// <param name='index'>The zero-based index of the entry to locate in the collection.</param>
		/// <value>The entry at the specified index of the collection.</value>
		/// <exception cref='ArgumentOutOfRangeException'><paramref name='index'/> is outside the valid range of indexes for the collection.</exception>
		public Bus this[int index] {
			get {
				return ((Bus)(List[index]));
			}
			set {
				List[index] = value;
			}
		}
		
		/// <summary>
		///   Adds a <see cref='Bus'/> with the specified value to the 
		///   <see cref='BusCollection'/>.
		/// </summary>
		/// <param name='val'>The <see cref='Bus'/> to add.</param>
		/// <returns>The index at which the new element was inserted.</returns>
		/// <seealso cref='BusCollection.AddRange'/>
		public int Add(Bus val)
		{
			return List.Add(val);
		}
		
		/// <summary>
		///   Copies the elements of an array to the end of the <see cref='BusCollection'/>.
		/// </summary>
		/// <param name='val'>
		///    An array of type <see cref='Bus'/> containing the objects to add to the collection.
		/// </param>
		/// <seealso cref='BusCollection.Add'/>
		public void AddRange(Bus[] val)
		{
			for (int i = 0; i < val.Length; i++) {
				this.Add(val[i]);
			}
		}
		
		/// <summary>
		///   Adds the contents of another <see cref='BusCollection'/> to the end of the collection.
		/// </summary>
		/// <param name='val'>
		///    A <see cref='BusCollection'/> containing the objects to add to the collection.
		/// </param>
		/// <seealso cref='BusCollection.Add'/>
		public void AddRange(BusCollection val)
		{
			for (int i = 0; i < val.Count; i++)
			{
				this.Add(val[i]);
			}
		}
		
		/// <summary>
		///   Gets a value indicating whether the 
		///    <see cref='BusCollection'/> contains the specified <see cref='Bus'/>.
		/// </summary>
		/// <param name='val'>The <see cref='Bus'/> to locate.</param>
		/// <returns>
		/// <see langword='true'/> if the <see cref='Bus'/> is contained in the collection; 
		///   otherwise, <see langword='false'/>.
		/// </returns>
		/// <seealso cref='BusCollection.IndexOf'/>
		public bool Contains(Bus val)
		{
			return List.Contains(val);
		}
		
		/// <summary>
		///   Copies the <see cref='BusCollection'/> values to a one-dimensional <see cref='Array'/> instance at the 
		///    specified index.
		/// </summary>
		/// <param name='array'>The one-dimensional <see cref='Array'/> that is the destination of the values copied from <see cref='BusCollection'/>.</param>
		/// <param name='index'>The index in <paramref name='array'/> where copying begins.</param>
		/// <exception cref='ArgumentException'>
		///   <para><paramref name='array'/> is multidimensional.</para>
		///   <para>-or-</para>
		///   <para>The number of elements in the <see cref='BusCollection'/> is greater than
		///         the available space between <paramref name='arrayIndex'/> and the end of
		///         <paramref name='array'/>.</para>
		/// </exception>
		/// <exception cref='ArgumentNullException'><paramref name='array'/> is <see langword='null'/>. </exception>
		/// <exception cref='ArgumentOutOfRangeException'><paramref name='arrayIndex'/> is less than <paramref name='array'/>'s lowbound. </exception>
		/// <seealso cref='Array'/>
		public void CopyTo(Bus[] array, int index)
		{
			List.CopyTo(array, index);
		}
		
		/// <summary>
		///    Returns the index of a <see cref='Bus'/> in 
		///       the <see cref='BusCollection'/>.
		/// </summary>
		/// <param name='val'>The <see cref='Bus'/> to locate.</param>
		/// <returns>
		///   The index of the <see cref='Bus'/> of <paramref name='val'/> in the 
		///   <see cref='BusCollection'/>, if found; otherwise, -1.
		/// </returns>
		/// <seealso cref='BusCollection.Contains'/>
		public int IndexOf(Bus val)
		{
			return List.IndexOf(val);
		}
		
		/// <summary>
		///   Inserts a <see cref='Bus'/> into the <see cref='BusCollection'/> at the specified index.
		/// </summary>
		/// <param name='index'>The zero-based index where <paramref name='val'/> should be inserted.</param>
		/// <param name='val'>The <see cref='Bus'/> to insert.</param>
		/// <seealso cref='BusCollection.Add'/>
		public void Insert(int index, Bus val)
		{
			List.Insert(index, val);
		}
		
		/// <summary>
		///  Returns an enumerator that can iterate through the <see cref='BusCollection'/>.
		/// </summary>
		/// <seealso cref='IEnumerator'/>
		public new BusEnumerator GetEnumerator()
		{
			return new BusEnumerator(this);
		}
		
		/// <summary>
		///   Removes a specific <see cref='Bus'/> from the <see cref='BusCollection'/>.
		/// </summary>
		/// <param name='val'>The <see cref='Bus'/> to remove from the <see cref='BusCollection'/>.</param>
		/// <exception cref='ArgumentException'><paramref name='val'/> is not found in the Collection.</exception>
		public void Remove(Bus val)
		{
			List.Remove(val);
		}
		
		/// <summary>
		///   Enumerator that can iterate through a BusCollection.
		/// </summary>
		/// <seealso cref='IEnumerator'/>
		/// <seealso cref='BusCollection'/>
		/// <seealso cref='Bus'/>
		public class BusEnumerator : IEnumerator
		{
			IEnumerator baseEnumerator;
			IEnumerable temp;
			
			/// <summary>
			///   Initializes a new instance of <see cref='BusEnumerator'/>.
			/// </summary>
			public BusEnumerator(BusCollection mappings)
			{
				this.temp = ((IEnumerable)(mappings));
				this.baseEnumerator = temp.GetEnumerator();
			}
			
			/// <summary>
			///   Gets the current <see cref='Bus'/> in the <seealso cref='BusCollection'/>.
			/// </summary>
			public Bus Current {
				get {
					return ((Bus)(baseEnumerator.Current));
				}
			}
			
			object IEnumerator.Current {
				get {
					return baseEnumerator.Current;
				}
			}
			
			/// <summary>
			///   Advances the enumerator to the next <see cref='Bus'/> of the <see cref='BusCollection'/>.
			/// </summary>
			public bool MoveNext()
			{
				return baseEnumerator.MoveNext();
			}
			
			/// <summary>
			///   Sets the enumerator to its initial position, which is before the first element in the <see cref='BusCollection'/>.
			/// </summary>
			public void Reset()
			{
				baseEnumerator.Reset();
			}
		}
	}
}
