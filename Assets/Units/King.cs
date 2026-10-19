using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


namespace AncientEmpires.Units
{
	public sealed class King : Unit
	{
		[SerializeField] private SerializableDictionaryBase<Color, Sprite> sleepSprites;
		protected override Sprite sleepSprite
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => sleepSprites[color];
			set => throw new InvalidOperationException();
		}


		public static readonly IReadOnlyDictionary<Color, King> kings = new Dictionary<Color, King>();
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init() => Battle.onAwake += () => (kings as Dictionary<Color, King>).Clear();


		private new void OnEnable()
		{
			base.OnEnable();
			(kings as Dictionary<Color, King>)[color] = this;
		}
	}
}