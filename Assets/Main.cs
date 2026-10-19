using AncientEmpires.MapEditors;
using System;
using UnityEngine;


namespace AncientEmpires
{
	[DefaultExecutionOrder(-1)] // Test
	public sealed class Main : MonoBehaviour
	{
		public static Map map { get; private set; }

		[SerializeField] private MapEditor editor;	// Test


		[Serializable]
		public struct Config
		{
			public Color color;
			public Team team;
			public bool isHuman;
			public int money;
			public int maxUnitCount;
		}
		[SerializeField] private Config[] m_configs;    // Test

		private static Main instance;
		private void Awake()
		{
			DontDestroyOnLoad(instance = instance ? throw new Exception() : this);

			// Test
			map = new(editor.ToString());
			Destroy(editor.gameObject);
		}


		public static Config[] configs => instance.m_configs;   // Test
	}
}