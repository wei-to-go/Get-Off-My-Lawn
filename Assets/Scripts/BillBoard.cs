using UnityEngine;

public class BillBoard : MonoBehaviour
{
    Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - cam.position);
    }
}
