using System;
using Bladiator.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bladiator.UI
{
	[RequireComponent(typeof(HealthDisplay))]
	public class BossUi : MonoBehaviour
	{
		public static BossUi Instance;
		private HealthDisplay m_Display;
		private void Awake()
		{
			if (Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
			m_Display = GetComponent<HealthDisplay>();
		}
		
		public void BossSpawned(EntityBase enemy)
		{
			m_Display.m_EntityOverride = enemy;
			
			TextAsset text = (TextAsset)Resources.Load("LordNames");
			string[] names = text.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
			string name = "Lord " + names[Random.Range(0, names.Length)];

			m_Display.m_BossName = name;
			m_Display.gameObject.SetActive(true);
		}
	}
}