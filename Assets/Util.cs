using Cysharp.Threading.Tasks;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;


namespace AncientEmpires
{
	public static class Util
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int Convert(this Vector3 v) => new((int)v.x, (int)v.y);


		public static T[][] NewArray<T>(int sizeX, int sizeY, Func<int, int, T> initialize = null)
		{
			var array = new T[sizeX][];
			for (int x = 0; x < sizeX; ++x)
			{
				array[x] = new T[sizeY];
				if (initialize != null)
					for (int y = 0; y < sizeY; ++y) array[x][y] = initialize(x, y);
			}

			return array;
		}


		public static bool isRunning(this UniTask task)
		{
			try
			{
				if (task.Status == UniTaskStatus.Faulted) goto FAULTED;
				return task.Status == UniTaskStatus.Pending;
			}
			catch (InvalidOperationException) { return false; }

		FAULTED:
			task.Forget();
			return false;
		}


		public static bool isRunning<T>(this UniTask<T> task)
		{
			try
			{
				if (task.Status == UniTaskStatus.Faulted) goto FAULTED;
				return task.Status == UniTaskStatus.Pending;
			}
			catch (InvalidOperationException) { return false; }

		FAULTED:
			task.Forget();
			return false;
		}
	}



	public enum Color
	{
		Black, Blue, Green, Red
	}



	public enum Team
	{
		One = 1, Two = 2, Three = 3, Four = 4
	}
}