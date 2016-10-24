using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LeagueSharp.Loader.Class
{
	public static class ListExtensions
	{
		private readonly static Random Rng;

		static ListExtensions()
		{
			ListExtensions.Rng = new Random();
		}

		public static void ShuffleRandom<T>(this IList<T> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = ListExtensions.Rng.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
	}
}