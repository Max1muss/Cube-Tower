using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float speed = 10f;
    private Transform m_rotator;

    private void Start()
    {
        m_rotator = GetComponent<Transform>();
    }

    private void Update()
    {
        m_rotator.Rotate(0, speed * Time.deltaTime, 0);
    }
}
