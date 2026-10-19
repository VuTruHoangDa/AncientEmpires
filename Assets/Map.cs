using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.InputSystem.Utilities;


namespace AncientEmpires
{
	public readonly struct Map
	{
		public readonly ReadOnlyArray<ReadOnlyArray<byte>> terrains;

		public int width => terrains.Count;

		public int height => terrains[0].Count;

		public byte this[int x, int y] => terrains[x][y];


		private static readonly Stack<byte> stack = new();
		public Map(string data)
		{
			using var reader = new StringReader(data);
			var line = reader.ReadLine();
			int height = line.Split(' ').Length;
			int width = 0;

			// data -> stack
			do
			{
				++width;
				foreach (string id in line.Split(' ')) stack.Push(Convert.ToByte(id));
			} while ((line = reader.ReadLine()) != null);

			// stack -> array
			var rows = new ReadOnlyArray<byte>[width];
			for (int x = width - 1; x >= 0; --x)
			{
				var column = new byte[height];
				rows[x] = new(column);
				for (int y = height - 1; y >= 0; --y) column[y] = stack.Pop();
			}

			terrains = new(rows);
		}
	}
}