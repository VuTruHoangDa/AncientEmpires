using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Tilemaps;


namespace AncientEmpires.Terrains
{
	[CreateAssetMenu(fileName = "Tombstone", menuName = "Terrains/Tombstone")]
	public sealed class Tombstone : TileBase
	{
		[NonSerialized] public int day;

		private static Tombstone[][] array;
		private static Tilemap tilemap;
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			Battle.onAwake += () =>
			{
				array = Util.NewArray<Tombstone>(Main.map.width, Main.map.height);
				tilemap = Addressables.InstantiateAsync("Assets/Terrains/Prefab/Tombstones.prefab")
					.WaitForCompletion().GetComponentInChildren<Tilemap>();
				tilemap.origin = default;
				tilemap.size = new(Main.map.width, Main.map.height);
			};
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Tombstone Get(Vector3 position) => array[(int)position.x][(int)position.y];


		public static Tombstone New(Vector3 position)
		{
			var tombstone = Instantiate(Addressables.LoadAssetAsync<Tombstone>("Assets/Terrains/Tiles/Tombstone.asset").WaitForCompletion());
			tombstone.day = Battle.day;
			var pos = position.Convert();
			tilemap.SetTile(pos, array[pos.x][pos.y] = tombstone);
			return tombstone;
		}


		public static void Clear(Vector3 position)
		{
			var pos = position.Convert();
			tilemap.SetTile(pos, array[pos.x][pos.y] = null);
		}


		[SerializeField] private Sprite sprite;
		public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) => tileData.sprite = sprite;


		public override string ToString() => $"Tombstone: day= {day}";
	}
}