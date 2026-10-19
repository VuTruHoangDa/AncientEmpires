using System;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace AncientEmpires.Terrains
{
	[CreateAssetMenu(fileName = "Other", menuName = "Terrains/Other")]
	public sealed class Other : Terrain
	{
		public enum Type
		{
			Land, Temple, Fence, Plane, Water
		}
		public Type type { get; private set; }

		protected override Terrain New()
		{
			var other = Instantiate(this);
			if (id == 11)
			{
				// Thân máy bay
				other.type = Type.Plane;
				other.defend = 15;
			}
			else if (12 <= id && id <= 14)
				// Xung quanh máy bay
				other.defend = 10;
			else if (id == 15)
			{
				// Ngôi đền
				other.type = Type.Temple;
				other.defend = 15;
			}
			else if (id == 16)
			{
				// Hàng rào
				other.type = Type.Fence;
				other.defend = 15;
			}
			else if (id == 17)
			{
				// Núi
				other.defend = 15;
				other.limitMove = 2;
			}
			else if (18 <= id && id <= 20)
			{
				// Rừng và gò đất
				other.defend = 10;
				other.limitMove = 1;
			}
			else if (28 <= id && id <= 41)
			{
				// Nước
				other.type = Type.Water;
				other.limitMove = 2;
			}
			else if (42 <= id && id <= 44)
				// Đất hoặc cây cầu
				other.defend = 5;
			else if (id == 45)
			{
				// Biên giới
				other.limitMove = int.MaxValue;
#if !UNITY_EDITOR
				other.sprite = null;
#endif
			}

			return other;
		}


		#region Water Animation
		[SerializeField] private AnimationData waterAnim;


		public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData outData)
		{
			if (id != 28) return true;

			outData.animatedSprites = waterAnim.animatedSprites;
			outData.animationStartTime = waterAnim.animationStartTime;
			outData.animationSpeed = waterAnim.animationSpeed;
			outData.flags = waterAnim.flags;
			return true;
		}



		[Serializable]
		public struct AnimationData
		{
			public Sprite[] animatedSprites;
			public float animationSpeed, animationStartTime;
			public TileAnimationFlags flags;
		}
		#endregion


		public override string ToString() => $"Other Terrain: {type}, {base.ToString()}";
	}
}