using UnityEngine;

public class SunLookAt : MonoBehaviour
{
    Transform player;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;
        transform.LookAt(player);
    }
}