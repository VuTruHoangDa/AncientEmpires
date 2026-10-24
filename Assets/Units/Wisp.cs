using AncientEmpires.Terrains;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;


namespace AncientEmpires.Units
{
	public sealed class Wisp : Unit
	{
		private new void OnEnable()
		{
			base.OnEnable();

			// Quét đồng minh tăng aura
			foreach (var target in FindTargets(transform.position.Convert(), 1, 2))
			{
				var unit = Get(target);
				if (unit && unit.army.team == army.team) ++unit.aura;
			}
		}


		public override Tombstone Die()
		{
			// Quét đồng minh giảm aura
			foreach (var target in FindTargets(transform.position.Convert(), 1, 2))
			{
				var unit = Get(target);
				if (unit && unit.army.team == army.team) --unit.aura;
			}

			throw new NotImplementedException();
		}


		public override UniTask Move(Vector3Int dest)
		{
			// Giảm aura đồng minh xung quanh vị trí cũ
			// Tăng aura đồng minh xung quanh dest

			throw new NotImplementedException();
		}
	}
}