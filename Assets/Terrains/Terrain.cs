using Cysharp.Threading.Tasks;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;


namespace AncientEmpires.Terrains
{
	public abstract class Terrain : TileBase
	{
		public int defend { get; protected set; }

		public int limitMove { get; protected set; }


		private static Terrain[][] array;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Terrain Get(Vector3 pos) => array[(int)pos.x][(int)pos.y];


		protected static byte id { get; private set; }
		protected abstract Terrain New();


		#region sprite
		private static Tilemap terrainMap;
		protected static Tilemap castleRoofMap { get; private set; }
		[field: HideInInspector][field: SerializeField] protected Vector3Int position { get; private set; }
		[HideInInspector][SerializeField] private Sprite m_sprite;
		protected Sprite sprite
		{
			get => m_sprite;

			set
			{
				m_sprite = value;
				if (terrainMap.ContainsTile(this)) terrainMap.RefreshTile(position);
			}
		}


		public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) => tileData.sprite = sprite;
		#endregion


		public static readonly int Task_Terrain_Init = "Terrain.Init".GetHashCode();
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			var atlas = Addressables.LoadAssetAsync<SpriteAtlas>("Assets/Terrains/Atlas.spriteatlasv2").WaitForCompletion();
			Battle.onAwake += () =>
			{
				var maps = Addressables.InstantiateAsync("Assets/Terrains/Prefab/Terrains.prefab")
					.WaitForCompletion().GetComponentsInChildren<Tilemap>();
				castleRoofMap = maps[0];
				terrainMap = maps[1];
				foreach (var map in maps)
				{
					map.origin = default;
					map.size = new(Main.map.width, Main.map.height);
				}

				array = Util.NewArray<Terrain>(Main.map.width, Main.map.height);
			};

			Battle.onStart += async () =>
			{
				Vector3Int pos = default;
				int count = 1;
				Battle.tasks.Add(Task_Terrain_Init);

				// Hiện animation load

				for (pos.x = 0; pos.x < Main.map.width; ++pos.x)
					for (pos.y = 0; pos.y < Main.map.height; ++pos.y)
					{
						if (count++ % 20 == 0) await UniTask.Yield();

						id = Main.map[pos.x, pos.y];
						var terrain = Addressables.LoadAssetAsync<Terrain>("Assets/Terrains/Tiles/" +
							$"{(0 <= id && id <= 4 ? "Castle" : 5 <= id && id <= 10 ? "House" : "Other")}.asset").WaitForCompletion();
						terrain.position = pos;
						terrain.sprite = atlas.GetSprite(id.ToString());
						terrainMap.SetTile(pos, array[pos.x][pos.y] = terrain.New());
					}

				// Tắt animation

				Battle.tasks.Remove(Task_Terrain_Init);
			};
		}


		public override string ToString() => $"defend= {defend}, limitMove= {limitMove}, position= {position}";
	}
}