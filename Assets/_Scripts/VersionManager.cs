using TMPro;
using UnityEngine;

public class VersionManager : MonoBehaviour
{
	private string m_GameVersion = "0.2";
	private string m_SaveVersion = "1";
	private string m_BuildType;

	[SerializeField] private TMP_Text m_VersionText;

	void Start()
	{
		FindBuildType();
		m_VersionText.SetText($"{m_BuildType}\nv{m_GameVersion}");
	}

	private void FindBuildType()
	{
		        #if UNITY_EDITOR
			        m_BuildType = "DEV";
		        #elif UNITY_STANDALONE_WIN
			        m_BuildType = "WIN";
		        #elif UNITY_STANDALONE_OSX
			        m_BuildType = "OSX";
		        #elif UNITY_STANDALONE_LINUX
			        m_BuildType = "LINUX";
		        #elif UNITY_WEBGL
			        m_BuildType = "WEBGL";
		        #endif
	}
}