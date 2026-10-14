using UnityEngine;
using UnityEngine.UI;

public class SunBehavior : MonoBehaviour
{
    public float maxHP = 100;
    public Slider hpSlider;

    public bool isVulnerable = false;
    public GameObject vulnerableEffect;

    float hp;
    LevelManager levelManager;

    void Start()
    {
        hp = maxHP;

        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = maxHP;
        }

        levelManager = FindAnyObjectByType<LevelManager>();

        SetVulnerable(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bullet"))
        {
            WeaponSwitcher.Weapon currentWeapon = WeaponSwitcher.Instance.currentWeapon;
            TakeDamage(currentWeapon.bulletDMG);
            UpdateHP();
        }
    }

    public void SetVulnerable(bool value)
    {
        isVulnerable = value;
        hpSlider.gameObject.SetActive(value); //can only view the hp when sun is vulnerable
        if (vulnerableEffect != null)
            vulnerableEffect.SetActive(value);
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            if (value)
            {
                rend.material.color = Color.red;
            }
            else
            {
                rend.material.color = Color.yellow;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            levelManager.WinLevel();
        }
    }

    public void UpdateHP()
    {
        if (hpSlider != null)
            hpSlider.value = hp;
    }
}