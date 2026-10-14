using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class WeaponSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class Weapon
    {
        public string name;
        public GameObject gunHeld;
        public GameObject gunPrefab;
        public GameObject projectile;
        public AudioClip shootSound;
        public float bulletSpeed;
        public float bulletRange;
        public int bulletDMG;
        public float fireRate;
        public float currentFireRate;
        public int maxMagRounds;
        public int currentMagRounds;
        public bool unlocked;
    }
    public TMP_Text weaponNameText;
    public Weapon[] weapons;
    public int currentWeaponIndex = 0;
    public Weapon currentWeapon;
    public static WeaponSwitcher Instance;

    private List<int> unlockOrder = new List<int>();

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject); //gets rid of extraneous instances
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        ResetWeaponLockState();
        InitUnlockOrder();
    }

    void Start()
    {
        if (weaponNameText != null)
            weaponNameText.gameObject.SetActive(false);
            
        currentWeaponIndex = 0;
        SelectWeapon(0);
    }

    void InitUnlockOrder()
    {
        unlockOrder.Clear();
        for (int i = 0; i < 2; i++) //unlock only the first two!!!
        {
            weapons[i].unlocked = true;
            unlockOrder.Add(i);
        }
        currentWeapon = weapons[0];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) TrySelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TrySelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TrySelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) TrySelectSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) TrySelectSlot(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) TrySelectSlot(5);
        if (Input.GetKeyDown(KeyCode.Alpha7)) TrySelectSlot(6);
        if (Input.GetKeyDown(KeyCode.Alpha8)) TrySelectSlot(7);
    }

    void TrySelectSlot(int slot)
    {
        if (slot >= 0 && slot < unlockOrder.Count)
            SelectWeapon(unlockOrder[slot]);
    }

    void SelectWeapon(int index)
    {
        currentWeapon = weapons[index];
        weapons[currentWeaponIndex].gunHeld.SetActive(false);
        currentWeaponIndex = index;
        weapons[index].gunHeld.SetActive(true);

        if (weaponNameText != null)
        {
            weaponNameText.text = currentWeapon.name;
            CancelInvoke(nameof(HideWeaponName));
            weaponNameText.gameObject.SetActive(true);
            Invoke(nameof(HideWeaponName), 2f);
        }
    }

    public void UnlockWeapon(int index)
    {
        if (index >= 0 && index < weapons.Length)
        {
            if (!weapons[index].unlocked)
            {
                weapons[index].unlocked = true;
                unlockOrder.Add(index);
            }
            SelectWeapon(index); //auto switch when picked up
        }
    }

    public void ResetWeaponLockState()
    {
        for (int i = 2; i < weapons.Length; i++)
        {
            weapons[i].unlocked = false;
        }
        InitUnlockOrder();
    }

    public Weapon[] GetLockedWeapons()
    {
        List<Weapon> locked = new List<Weapon>{};
        foreach(Weapon w in weapons)
        {
            if (!w.unlocked)
            {
                locked.Add(w);
            }
        }
        return locked.ToArray();
    }

    void HideWeaponName()
    {
        if (weaponNameText != null)
            weaponNameText.gameObject.SetActive(false);
    }
}