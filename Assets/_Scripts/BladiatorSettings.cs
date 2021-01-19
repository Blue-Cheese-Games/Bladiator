using System;
using _Scripts;
using UnityEngine;

namespace Bladiator.Settings
{
	public class BladiatorSettings : MonoBehaviour
	{
		public static BladiatorSettings Instance;
		
		private SettingsData m_Settings;

		public SettingsData Settings => m_Settings;

		public Action<SettingsData> OnSettingsChanged;

		private void Awake()
		{
			Instance = this;
			m_Settings = new SettingsData()
			{
				Volume = 1
			};
		}
		
		public void OnVolumeChanged(Single value)
		{ 
			float.TryParse(value.ToString(), out m_Settings.Volume);
			SaveSettings();
		}

		private void SaveSettings()
		{
			//TODO Save this as a json
			// PlayerPrefs.SetString("SettingsData",);
			OnSettingsChanged?.Invoke(m_Settings);
		}
	}
}