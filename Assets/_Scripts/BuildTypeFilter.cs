using UnityEngine;

public class BuildTypeFilter : MonoBehaviour
{
    [Tooltip("When true the object will be available within the specified build")]
    [SerializeField] private bool m_Editor, m_Windows, m_OSX, m_Linux, m_WebGL; 
    
    // Start is called before the first frame update
    void Awake()
    {
        #if UNITY_EDITOR
            if (!m_Editor)
            {
                Destroy(gameObject);
            }
        #elif UNITY_STANDALONE_WIN
            if (!m_Windows)
            {
                Destroy(gameObject);
            }
        #elif UNITY_STANDALONE_OSX
            if (!m_OSX)
            {
                Destroy(gameObject);
            }
        #elif UNITY_STANDALONE_LINUX
            if (!m_Linux)
            {
                Destroy(gameObject);
            }
        #elif UNITY_WEBGL
            if (!m_WebGL)
            {
                Destroy(gameObject);
            }
        #endif
    }
}
