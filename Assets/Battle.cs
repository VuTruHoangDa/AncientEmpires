using AncientEmpires.Terrains;
using AncientEmpires.Units;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;


namespace AncientEmpires
{
	public sealed class Battle : MonoBehaviour
	{
		public static int day { get; private set; }
		public static event Action onAwake, onStart;
		public static readonly List<int> tasks = new();
		private void Awake()
		{
			day = 1;
			tasks.Clear();
			var armies = Battle.armies as Dictionary<Color, Army>;
			armies.Clear();
			foreach (var config in Main.configs)
			{
				var army = armies[config.color] = new(config, new GameObject().transform);
				army.unitAnchor.name = $"{config.color} Army";
				army.unitAnchor.parent = transform;
			}

			hintTilemap.origin = default;
			hintTilemap.size = new(Main.map.width, Main.map.height);
			onAwake();
		}


		private async void Start()
		{
			onStart();
			do await UniTask.Yield();
			while (tasks.Count != 0);
			await UniTask.NextFrame();

			// Bắt đầu chơi

			// Test
			var unit = Unit.New<Sorceress>(new(4, 6), Color.Blue);
			Unit.New<Soldier>(new(2, 6), Color.Green);
			Unit.New<Archer>(new(6, 5), Color.Red);

			var m = Unit.FindMoves(unit, unit.transform.position.Convert(), unit.army.team);
			foreach (var pos in m)
				hintTilemap.SetTile(pos, greenTile);

			(m as List<Vector3Int>).Add(unit.transform.position.Convert());
			foreach (var pos in Unit.FindTargets(m, 1, 2))
				hintTilemap.SetTile(pos, redTile);


			ready = true;
		}


		// Test
		private bool ready;
		private void Update()
		{
			if (!ready) return;

			if (Mouse.current != null)
			{
				var m = Mouse.current;
				if (m.leftButton.wasPressedThisFrame)
				{
					var w = UnityEngine.Camera.main.ScreenToWorldPoint(m.position.value);
					var pos = new Vector3(Mathf.RoundToInt(w.x), Mathf.RoundToInt(w.y));
					print(Terrain.Get(pos));
					if (Tombstone.Get(pos)) print(Tombstone.Get(pos));
				}
			}
		}



		public sealed class Army
		{
			public readonly Color color;
			public readonly Team team;
			public readonly bool isHuman;
			public readonly int maxUnitCount;
			public int money { get; set; }
			public int house, castle;
			public readonly List<Unit> units = new();
			public readonly Transform unitAnchor;
			public Vector3 focusPoint;


			public Army(Main.Config config, Transform unitAnchor)
			{
				color = config.color;
				team = config.team;
				isHuman = config.isHuman;
				maxUnitCount = config.maxUnitCount;
				money = config.money;
				this.unitAnchor = unitAnchor;
			}
		}
		public static readonly IReadOnlyDictionary<Color, Army> armies = new Dictionary<Color, Army>();

		[SerializeField] private Tilemap hintTilemap;
		[SerializeField] private Tile redTile, greenTile, yellowTile;
	}
}