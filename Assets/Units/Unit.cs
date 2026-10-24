using AncientEmpires.Terrains;
using Cysharp.Threading.Tasks;
using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace AncientEmpires.Units
{
	[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
	public abstract class Unit : MonoBehaviour
	{
		public int hp { get; set; }

		[SerializeField] private int minAttack, maxAttack, defend;

		[field: SerializeField] public int moveRange { get; private set; }

		[field: SerializeField] public int cost { get; private set; }

		private float level;

		public int poison { get; set; }

		public int aura { get; set; }

		[field: SerializeField] protected virtual Sprite sleepSprite { get; set; }
		private bool m_awake = true;
		public bool awake
		{
			get => m_awake;

			set
			{
				animator.enabled = m_awake = value;
				if (!value) spriteRenderer.sprite = sleepSprite;
			}
		}

		[SerializeField] private SerializableDictionaryBase<Color, RuntimeAnimatorController> anims;

		[SerializeField] private SpriteRenderer spriteRenderer;

		[SerializeField] private Animator animator;

		public Battle.Army army { get; private set; }

		private static Unit[][] array;
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init() => Battle.onAwake += () =>
			{
				array = Util.NewArray<Unit>(Main.map.width, Main.map.height);
				moveRanges = Util.NewArray<int>(Main.map.width, Main.map.height);
			};


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Unit Get(Vector3 position) => array[(int)position.x][(int)position.y];


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Unit Get(Vector3Int position) => array[position.x][position.y];


		public static T New<T>(Vector3 position, Color color) where T : Unit
		{
			var unit = Addressables.InstantiateAsync($"Assets/Units/Prefab/{typeof(T).Name}.prefab",
				position, Quaternion.identity).WaitForCompletion().GetComponent<T>();
#if DEBUG
			if (unit.enabled) throw new Exception($"{typeof(T)} phải tắt script !");
#endif
			unit.army = Battle.armies[color];
			unit.enabled = true;
			return unit;
		}


		protected void OnEnable()
		{
			animator.runtimeAnimatorController = anims[army.color];
			army.money -= cost;
			army.units.Add(this);
			transform.parent = army.unitAnchor;
			var pos = transform.position.Convert();

			// Quét Wisp đồng minh cài aura
			foreach (var target in FindTargets(pos, 1, 2))
				if (array[target.x][target.y] is Wisp wisp && wisp.army.team == army.team) ++aura;

			if (!array[pos.x][pos.y]) array[pos.x][pos.y] = this;
		}


		protected void OnDisable()
		{
			var pos = transform.position.Convert();
			if (this == array[pos.x][pos.y]) array[pos.x][pos.y] = null;
			army.units.Remove(this);
		}


		private static readonly Vector3Int[] directions = new Vector3Int[]
		{
			Vector3Int.up, Vector3Int.right, Vector3Int.down,Vector3Int.left
		};
		private static int[][] moveRanges;
		/// <summary>
		/// Tìm điểm di chuyển (ô trống)<para/>
		/// Rồng (Dragon) không bị cản trở bởi địa hình, cá (Elemental) di chuyển tự do trong nước<br/>
		/// Có thể đi xuyên qua quân đồng minh nhưng bị chặn bởi quân địch
		/// </summary>
		public static IReadOnlyList<Vector3Int> FindMoves(Unit unit, Vector3Int position, Team team)
		{
			// moveRanges: cài -1 trong phạm vi hình vuông xung quanh unit, cạnh 2 * unit.moveRange
			var min = new Vector2Int(position.x - unit.moveRange - 1, position.y - unit.moveRange - 1);
			var max = new Vector2Int(position.x + unit.moveRange + 1, position.y + unit.moveRange + 1);
			if (min.x < 0) min.x = 0;
			if (min.y < 0) min.y = 0;
			if (max.x >= Main.map.width) max.x = Main.map.width - 1;
			if (max.y >= Main.map.height) max.y = Main.map.height - 1;
			moveRanges[position.x][position.y] = unit.moveRange;
			for (int x = min.x; x <= max.x; ++x)
				for (int y = min.y; y <= max.y; ++y) moveRanges[x][y] = -1;

			var result = new List<Vector3Int>();
			DFS(position, unit.moveRange);
			return result;


			void DFS(Vector3Int start, int range)
			{
				foreach (var dir in directions)
				{
					var pos = start + dir;
					if (pos.x < 0 || pos.x >= Main.map.width || pos.y < 0 || pos.y >= Main.map.height) continue;

					int posRange = range - 1;
					if (unit is not Dragon)
					{
						var terrain = Terrain.Get(pos);
						if (unit is not Elemental || terrain is not Other other || other.type != Other.Type.Water)
							posRange -= terrain.limitMove;
					}

					int lastRange = moveRanges[pos.x][pos.y];
					if (posRange <= lastRange) continue;

					moveRanges[pos.x][pos.y] = posRange;
					var posUnit = array[pos.x][pos.y];
					if (!posUnit)
					{
						// Lần thăm đầu tiên ?
						if (lastRange == -1) result.Add(pos);
					}
					else if (posUnit.army.team != team) continue;

					if (posRange > 0) DFS(pos, posRange);
				}
			}
		}


		private static List<Vector3Int> source = new(), target = new();
		private static readonly List<Vector3Int> visited = new();
		/// <summary>
		/// Tìm điểm tấn công/ phá nhà/ hồi sinh bia mộ/ aura<para/>
		/// Chú ý: minRange >= 1
		/// </summary>
		public static IReadOnlyList<Vector3Int> FindTargets(IReadOnlyList<Vector3Int> starts, int minRange, int maxRange)
		{
			var result = new List<Vector3Int>();
			source.Clear();
			source.AddRange(starts);
			target.Clear();
			visited.Clear();
			visited.AddRange(starts);

			for (int range = 1; range <= maxRange; ++range)
			{
				foreach (var s in source)
					foreach (var dir in directions)
					{
						var pos = s + dir;
						if (pos.x < 0 || pos.x >= Main.map.width || pos.y < 0 || pos.y >= Main.map.height || visited.Contains(pos)) continue;

						visited.Add(pos);
						target.Add(pos);
					}

				if (range >= minRange) result.AddRange(target);
				(source, target) = (target, source);
				target.Clear();
				if (source.Count == 0) break;
			}

			return result;
		}


		private static readonly List<Vector3Int> tmp = new(new Vector3Int[] { new(-1, -1) });
		/// <summary>
		/// Tìm điểm tấn công/ phá nhà/ hồi sinh bia mộ/ aura<para/>
		/// Chú ý: minRange >= 1
		/// </summary>
		public static IReadOnlyList<Vector3Int> FindTargets(Vector3Int start, int minRange, int maxRange)
		{
			tmp[0] = start;
			return FindTargets(tmp, minRange, maxRange);
		}


		public virtual Tombstone Die()
		{
			throw new NotImplementedException();
		}


		public virtual async UniTask Move(Vector3Int dest)
		{
			// reset aura=0 và quét Wisp cài aura tại dest
			throw new NotImplementedException();
		}


		public virtual async UniTask Attack(Vector3Int target)
		{
			throw new NotImplementedException();
		}
	}
}