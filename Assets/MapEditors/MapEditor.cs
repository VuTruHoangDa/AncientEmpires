using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace AncientEmpires.MapEditors
{
	public sealed class MapEditor : MonoBehaviour
	{
		[SerializeField] private Tilemap tilemap;


		/// <summary>
		/// Lưu file
		/// </summary>
		public override string ToString()
		{
			var sb = new StringBuilder();
			using var writer = new StringWriter(sb);
			tilemap.CompressBounds();
			var min = tilemap.cellBounds.min;
			var max = tilemap.cellBounds.max;
			var pos = new Vector3Int();
			for (pos.x = min.x; pos.x < max.x; ++pos.x)
			{
				for (pos.y = min.y; pos.y < max.y; ++pos.y)
				{
					var tile = tilemap.GetTile(pos);
					if (!tile) throw new Exception("Tilemap phải luôn có Tile !");
					writer.Write($"{tile.name} ");
				}

				sb.Remove(sb.Length - 1, 1);
				writer.WriteLine();
			}

			return sb.ToString();
		}
	}
}