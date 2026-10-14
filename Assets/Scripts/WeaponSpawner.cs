using UnityEngine;
using System.Linq;

public class WeaponSpawner : MonoBehaviour
{
    public GameObject[] dumpsters;
    public WeaponSwitcher.Weapon[] locked;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        locked = WeaponSwitcher.Instance.GetLockedWeapons();
        int randIndex;
        for (int i = 0; i < dumpsters.Length; i++)
        {
            if(locked != null && locked.Length > 0)
            {
                randIndex = Random.Range(0, locked.Length + 2); //chance to not spawn any weapon
                GarbageCan gb = dumpsters[i].GetComponent<GarbageCan>();
                if(randIndex < locked.Length)
                {
                    gb.weaponPickup = locked[randIndex].gunPrefab;
                    locked = locked.Where((val, idx) => idx != randIndex).ToArray();
                }
                
            }
            

        }
    }

}
