using UnityEngine;
using System.Collections.Generic;

public class PickupBehavior : MonoBehaviour
{
    public float relaxVal = -10f;
    public AudioClip pickupSound;
    public AudioClip burp;
    LevelManager levelManager;
    List<GameObject> skins = new List<GameObject> {};

    void Start()
    {
        levelManager = FindAnyObjectByType<LevelManager>();
        RandomSkin();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            DestroyPickup();
        }
    }

    void PlayBurp()
    {
        AudioSource.PlayClipAtPoint(burp, transform.position);
    }

    void DestroyPickup()
    {
        if (pickupSound)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        if (burp)
            Invoke("PlayBurp", 0.8f);

        if (levelManager)
            levelManager.changeCortisolLevel(relaxVal);

        GetComponent<Collider>().enabled = false;
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
        {
            mr.enabled = false;
        }
        Destroy(gameObject, 2f);
    }

    void RandomSkin()
    {
        Transform[] children = GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.CompareTag("Skin"))
            {
                skins.Add(child.gameObject);
            }
        }

        int random = Random.Range(0, skins.Count);
        GameObject selectedSkin = skins[random];

        selectedSkin.SetActive(true);
    }

}