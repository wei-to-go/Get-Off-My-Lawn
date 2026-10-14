using UnityEngine;

public class RocketLauncherPickup : MonoBehaviour
{
    //repurposing for broader utility
    public int weaponIndex;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WeaponSwitcher.Instance.UnlockWeapon(weaponIndex);
            Destroy(gameObject);
        }
    }
}