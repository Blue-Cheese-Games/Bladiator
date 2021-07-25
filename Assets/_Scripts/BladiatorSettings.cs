using System;
using _Scripts;
using Bladiator.Entities.Players;
using Bladiator.UI;
using UnityEngine;

namespace Bladiator.Settings
{
	public class BladiatorSettings : MonoBehaviour
	{
		public static BladiatorSettings Instance;

		private SettingsData m_Settings;

		public SettingsData Settings => m_Settings;

		public Action<SettingsData> OnSettingsChanged;
		private bool m_InitialCall;

		private void Awake()
		{
			Instance = this;
			if (PlayerPrefs.HasKey("SettingsData"))
			{
				m_Settings = JsonUtility.FromJson<SettingsData>(PlayerPrefs.GetString("SettingsData"));
				return;
			}

			m_Settings = new SettingsData()
			{
				Volume = 1
			};
		}

		private void Update()
		{
			if (m_InitialCall) return;
			
			OnSettingsChanged.Invoke(m_Settings);
			m_InitialCall = true;
		}

		public void OnVolumeChanged(Single value)
		{
			VolumeTest.Volume = value;
			float.TryParse(value.ToString(), out m_Settings.Volume);
			SaveSettings();
		}

		private void SaveSettings()
		{
			string json = JsonUtility.ToJson(m_Settings);
			PlayerPrefs.SetString("SettingsData", json);
			OnSettingsChanged?.Invoke(m_Settings);
		}
	}
}