using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.InputSystem.Utilities;


namespace AncientEmpires
{
	public readonly struct Map
	{
		public readonly ReadOnlyArray<ReadOnlyArray<byte>> terrains;
		public readonly int width, height;
		public byte this[int x, int y] => terrains[x][y];


		private static readonly Stack<byte> stack = new();
		public Map(string data)
		{
			using var reader = new StringReader(data);
			var line = reader.ReadLine();
			height = line.Split(' ').Length;
			width = 0;

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