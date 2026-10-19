using AncientEmpires.Units;
using Cysharp.Threading.Tasks;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace AncientEmpires.Terrains
{
	[CreateAssetMenu(fileName = "Castle", menuName = "Terrains/Castle")]
	public sealed class Castle : Terrain
	{
		public Color? color { get; private set; }

		[SerializeField] private SerializableDictionaryBase<Color, Sprite> sprites;
		[SerializeField] private Sprite free;
		[SerializeField] private Tile roof;

		protected override Terrain New()
		{
			castleRoofMap.SetTile(position + Vector3Int.up, roof);
			var castle = Instantiate(this);
			castle.defend = 15;
			if (id == 4) return castle;

			var army = Battle.armies[(castle.color = (Color)id).Value];
			army.castles.Add(castle);
			army.focusPoint = position;
			return castle;
		}


		public void Occupy(King king)
		{
			throw new System.NotImplementedException();
		}


		public async UniTask<Unit> Buy()
		{
			throw new System.NotImplementedException();
		}


		public T Buy<T>() where T : Unit
		{
			throw new System.NotImplementedException();
		}


		public override string ToString() => $"Castle: {(color != null ? color : "Free")}, {base.ToString()}";
	}
}