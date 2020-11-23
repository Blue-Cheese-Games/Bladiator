using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float m_RotateSpeed;
    
    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles += Vector3.up * (Time.deltaTime * m_RotateSpeed);
    }
}
