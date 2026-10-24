using AncientEmpires.Terrains;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;


namespace AncientEmpires.Units
{
	public sealed class Crystal : Unit
	{
		public override Tombstone Die()
		{
			throw new NotImplementedException();
		}


		public override UniTask Attack(Vector3Int target) =>
			throw new InvalidOperationException("Crystal không thể tấn công !");
	}
}