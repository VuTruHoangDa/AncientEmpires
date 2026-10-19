using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static UnityEditor.PlayerSettings;


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

		public int poisonDay { get; set; }

		public int auraDay { get; set; }

		[field: SerializeField] protected virtual Sprite sleepSprite { get; set; }
		private bool m_awake;
		public bool awake
		{
			get => m_awake;

			set
			{
				if (animator.enabled = m_awake = value) animator.runtimeAnimatorController = anims[color];
				else spriteRenderer.sprite = sleepSprite;
			}
		}

		[SerializeField] private SerializableDictionaryBase<Color, RuntimeAnimatorController> anims;

		[SerializeField] private SpriteRenderer spriteRenderer;

		[SerializeField] private Animator animator;

		public Color color { get; private set; }

		private static Unit[][] array;
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init() =>
			Battle.onAwake += () => array = Util.NewArray<Unit>(Main.map.width, Main.map.height);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Unit Get(Vector3 position) => array[(int)position.x][(int)position.y];


		public static T New<T>(Vector3 position, Color color) where T : Unit
		{
			var unit = Addressables.InstantiateAsync($"Assets/Units/Prefab/{typeof(T).Name}.prefab",
				position, Quaternion.identity).WaitForCompletion().GetComponent<T>();
#if DEBUG
			if (unit.enabled) throw new Exception($"{typeof(T)} phải tắt script !");
#endif
			unit.color = color;
			unit.enabled = true;
			return unit;
		}


		protected void OnEnable()
		{
			awake = true;
			var pos = transform.position.Convert();
			if (!array[pos.x][pos.y]) array[pos.x][pos.y] = this;
			var army = Battle.armies[color];
			army.money -= cost;
			army.units.Add(this);
			transform.parent = army.unitAnchor;
		}


		protected void OnDisable()
		{
			var pos = transform.position.Convert();
			if (this == array[pos.x][pos.y]) array[pos.x][pos.y] = null;
			Battle.armies[color].units.Remove(this);
		}
	}
}