using UnityEngine;


namespace AncientEmpires
{
	[RequireComponent(typeof(UnityEngine.Camera))]
	public sealed class Camera : MonoBehaviour
	{
		private void Awake()
		{
			transform.position = new(Main.map.width / 2f - 0.5f, Main.map.height / 2f - 0.5f, -10);
		}
	}
}