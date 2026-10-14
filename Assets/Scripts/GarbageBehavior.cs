using UnityEngine;
using UnityEngine.UI;

public class GarbageCan : MonoBehaviour
{
    public GameObject weaponPickup;
    public GameObject openEffect;
    public float interactDistance = 10f;
    public float holdTime = 1.5f;
    public GameObject promptUI;
    public Image holdProgressRing; // the radial fill image

    private Transform player;
    private bool hasBeenOpened = false;
    private float holdTimer = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (promptUI) promptUI.SetActive(false);
        if (holdProgressRing) holdProgressRing.fillAmount = 0f;
    }

    void Update()
    {
        if (hasBeenOpened) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool inRange = distance <= interactDistance;

        if (promptUI) promptUI.SetActive(inRange);

        if (inRange && Input.GetKey(KeyCode.E))
        {
            holdTimer += Time.deltaTime;
            if (holdProgressRing)
                holdProgressRing.fillAmount = holdTimer / holdTime;

            if (holdTimer >= holdTime)
                Open();
        }
        else
        {
            holdTimer = 0f;
            if (holdProgressRing)
                holdProgressRing.fillAmount = 0f;
        }
    }

    void Open()
    {
        hasBeenOpened = true;
        if (openEffect)
            Instantiate(openEffect, transform.position, Quaternion.identity);
        if (weaponPickup)
            Instantiate(weaponPickup, transform.position + Vector3.up * 1.5f, Quaternion.identity);
        gameObject.SetActive(false);
    }
}