using UnityEngine;

public enum BuildType
{
    Editor,
    Windows,
    OSX,
    Linux,
    WebGL
} 

public class BuildTypeFilter : MonoBehaviour
{
    private BuildType m_CurrentBuildType;

    [Tooltip("When true the object will be available within the specified build")]
    [SerializeField] private bool m_Editor, m_Windows, m_OSX, m_Linux, m_WebGL; 
    
    // Start is called before the first frame update
    void Awake()
    {
        #if UNITY_EDITOR
            m_CurrentBuildType = BuildType.Editor;
        #elif UNITY_STANDALONE_WIN
            m_CurrentBuildType = BuildType.Windows;
        #elif UNITY_STANDALONE_OSX
            m_CurrentBuildType = BuildType.OSX;
        #elif UNITY_STANDALONE_LINUX
            m_CurrentBuildType = BuildType.Linux;
        #elif UNITY_WEBGL
            m_CurrentBuildType = BuildType.WebGL;
        #endif

            if (m_CurrentBuildType == BuildType.Editor && !m_Editor)
            {
                Destroy(gameObject);
                return;
            }
            
            if (m_CurrentBuildType == BuildType.Windows && !m_Windows)
            {
                Destroy(gameObject);
                return;
            }
            
            if (m_CurrentBuildType == BuildType.OSX && !m_OSX)
            {
                Destroy(gameObject);
                return;
            }
            
            if (m_CurrentBuildType == BuildType.Linux && !m_Linux)
            {
                Destroy(gameObject);
                return;
            }
            
            if (m_CurrentBuildType == BuildType.WebGL && !m_WebGL)
            {
                Destroy(gameObject);
                return;
            }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
