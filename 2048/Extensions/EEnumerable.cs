using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace _2048
{
	static class EEnumerable
	{
		private static Random random
		{
			get
			{
				if (_random == null)
					_random = new Random();
				return _random;
			}
		}

		[ThreadStatic]
		private static Random _random;


		/// <summary>
		/// Computes standard deviation of values. (It also computes 
		/// count of values, arithmetic mean, minimal value, maximal value)
		/// </summary>
		/// <param name="values">values to compute standard deviation from.</param>
		public static Statistics.IStatistics Statistics(this IEnumerable<double> values)
		{
			return new Statistics.Statistics(values);
		}


		/// <summary>
		/// Randomly chooses one element.
		/// </summary>
		/// <typeparam name="T">type of element</typeparam>
		/// <param name="source">source collection.</param>
		/// <returns>returns randomly chosen element 
		/// from <paramref name="source"/> collection.</returns>
		/// <remarks>Each element has the same pobability of beeing chosen 
		/// (1 / <paramref name="source"/>.count).</remarks>
		/// <exception cref="ArgumentNullException">
		/// if <paramref name="source"/> is null.</exception>
		/// <exception cref="InvalidOperationException">
		/// if <paramref name="source"/> is empty.</exception>
		public static T Random<T>(this IEnumerable<T> source)
		{
			bool empty;
			var result = source.Random(out empty);
			if (empty)
				throw new InvalidOperationException(
					"Source collection is empty."
				);
			else
				return result;
		}


		/// <summary>
		/// Randomly chooses one element or default value 
		/// if <paramref name="source"/> collection is empty.
		/// </summary>
		/// <typeparam name="T">type of element</typeparam>
		/// <param name="source">source collection.</param>
		/// <returns>returns randomly chosen element 
		/// from <paramref name="source"/> collection or default value
		/// if <paramref name="source"/> collection is empty.</returns>
		/// <remarks>Each element has the same pobability of beeing chosen 
		/// (1 / <paramref name="source"/>.count).</remarks>
		/// <exception cref="ArgumentNullException">
		/// if <paramref name="source"/> is null.</exception>
		public static T RandomOrDefault<T>(this IEnumerable<T> source)
		{
			bool empty;
			return source.Random(out empty);
		}


		private static T Random<T>(this IEnumerable<T> source, out bool empty)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			int count = 0;
			T result = default(T);
			var random = EEnumerable.random;
			foreach (var element in source)
			{
				if (random.Next(++count) == 0)
					result = element;
			}
			empty = count <= 0;
			return result;
		}


		/// <summary>
		/// Returns element with highest value.
		/// </summary>
		/// <typeparam name="T">type of element</typeparam>
		/// <param name="source">source collection.</param>
		/// <param name="comparer">comparer to compare elements 
		/// of <paramref name="source"/> collection.</param>
		/// <exception cref="ArgumentNullException">
		/// if <paramref name="source"/> is null.</exception>
		/// <exception cref="InvalidOperationException">
		/// if <paramref name="source"/> is empty.</exception>
		public static T Max<T>(
			this IEnumerable<T> source,
			IComparer<T> comparer = null
		){

			if (source == null)
				throw new ArgumentNullException("source");
			if (comparer == null)
				comparer = Comparer<T>.Default;
			T result = source.First();
			foreach (var element in source)
			{
				if ( 0 < comparer.Compare(element, result))
					result = element;
			}
			return result;

		}


		/// <summary>
		/// Returns element with highest value.
		/// </summary>
		/// <typeparam name="T">type of element</typeparam>
		/// <param name="source">source collection.</param>
		/// <param name="comparer">comparer to compare elements 
		/// of <paramref name="source"/> collection.</param>
		/// <exception cref="ArgumentNullException">
		/// if <paramref name="source"/> or <param name="comparison"/> is null.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// if <paramref name="source"/> is empty.</exception>
		public static T Max<T>(
			this IEnumerable<T> source,
			Comparison<T> comparison
		){
			return source.Max(Comparer<T>.Create(comparison));
		}


		public static void SeedRandom(int seed)
		{
			_random = new Random(seed);
		}
	}
}
