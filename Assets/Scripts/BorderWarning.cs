using UnityEngine;
using TMPro;

public class BorderWarning : MonoBehaviour
{
    public GameObject warningText;

    public float mapLimit = 45f;

    void Update()
    {
        Vector3 pos = transform.position;

        bool nearBorder =
            Mathf.Abs(pos.x) > mapLimit ||
            Mathf.Abs(pos.z) > mapLimit;

        warningText.SetActive(nearBorder);
    }
}