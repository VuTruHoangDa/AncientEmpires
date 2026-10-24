using AncientEmpires.Units;
using Cysharp.Threading.Tasks;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;


namespace AncientEmpires.Terrains
{
	[CreateAssetMenu(fileName = "House", menuName = "Terrains/House")]
	public sealed class House : Terrain
	{
		public Battle.Army army { get; private set; }

		[SerializeField] private SerializableDictionaryBase<Color, Sprite> sprites;
		[SerializeField] private Sprite free, broken;

		public bool isBroken { get; private set; }

		protected override Terrain New()
		{
			var house = Instantiate(this);
			house.defend = 10;
			house.isBroken = id == 10;
			if (id == 9 || id == 10) return house;

			(house.army = Battle.armies[(Color)(id - 5)]).house++;
			return house;
		}


		public void Occupy(King king)
		{
			throw new System.NotImplementedException();
		}


		public void Occupy(Soldier soldier)
		{
			throw new System.NotImplementedException();
		}


		public async UniTask Break(Catapult catapult)
		{
			throw new System.NotImplementedException();
		}


		public override string ToString() => $"House: {(army != null ? army.color : isBroken ? "Broken" : "Free")}, {base.ToString()}";
	}
}