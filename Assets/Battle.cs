using AncientEmpires.Terrains;
using AncientEmpires.Units;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


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
				var army = armies[config.color] = new(config.team, config.isHuman, config.maxUnitCount, config.money, new GameObject().transform);
				army.unitAnchor.name = $"{config.color} Army";
				army.unitAnchor.parent = transform;
			}

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
			var colors = new Color[] { Color.Black, Color.Blue, Color.Green, Color.Red };
			for (int y = 1; y <= 4; ++y)
			{
				int x = 1;
				list.Add(Unit.New<King>(new(x++, y), colors[y - 1]));
				list.Add(Unit.New<Soldier>(new(x++, y), colors[y - 1]));
				list.Add(Unit.New<Archer>(new(x++, y), colors[y - 1]));
				list.Add(Unit.New<Elemental>(new(x++, y), colors[y - 1]));
				list.Add(Unit.New<Sorceress>(new(x++, y), colors[y - 1]));
				list.Add(Unit.New<Wisp>(new(x++, y), colors[y - 1]));
				list.Add(Unit.New<DireWolf>(new(x++, y), colors[y - 1]));
				list.Add(Unit.New<Golem>(new(x++, y), colors[y - 1]));
				list.Add(Unit.New<Catapult>(new(x++, y), colors[y - 1]));
				list.Add(Unit.New<Dragon>(new(x++, y), colors[y - 1]));
				list.Add(Unit.New<Skeleton>(new(x++, y), colors[y - 1]));
				list.Add(Unit.New<Crystal>(new(x++, y), colors[y - 1]));
			}

			ready = true;
		}


		// Test
		private List<Unit> list = new();
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

			if (Keyboard.current != null)
			{
				var k = Keyboard.current;
				if (k.spaceKey.wasPressedThisFrame)
					foreach (var unit in list) unit.awake = !unit.awake;
			}
		}



		public sealed class Army
		{
			public readonly Team team;
			public readonly bool isHuman;
			public readonly int maxUnitCount;
			public int money { get; set; }
			public readonly List<House> houses = new();
			public readonly List<Castle> castles = new();
			public readonly List<Unit> units = new();
			public readonly Transform unitAnchor;
			public Vector3 focusPoint;


			public Army(Team team, bool isHuman, int maxUnitCount, int money, Transform unitAnchor)
			{
				this.team = team;
				this.isHuman = isHuman;
				this.maxUnitCount = maxUnitCount;
				this.money = money;
				this.unitAnchor = unitAnchor;
			}
		}
		public static readonly IReadOnlyDictionary<Color, Army> armies = new Dictionary<Color, Army>();
	}
}