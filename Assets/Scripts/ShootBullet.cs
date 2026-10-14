using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ShootBullet : MonoBehaviour
{
    public AudioClip reloadSFX;

    public float animationSpeed = 5;
    public Image crossHairImage;
    public TMP_Text magText;
    public Slider cooldown;
    public float sunDmg;

    WeaponSwitcher.Weapon currentWeapon;
    Vector3 ogCrossHairScale;
    Color ogCrossHairColor;
    float lastFireTime;
    LevelManager levelManager;
    Animator anim;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RebindSceneReferences();
    }

    void Start()
    {
        anim = transform.parent.GetComponent<Animator>();
        RebindSceneReferences();
        if (!crossHairImage)
        {
            Debug.LogWarning("No Cross Hair Image Specified");
            return;
        }
        ogCrossHairColor = crossHairImage.color;
        ogCrossHairScale = crossHairImage.transform.localScale;
    }

    void Update()
    {
        if (crossHairImage != null)
            crossHairImage.gameObject.SetActive(LevelManager.isPlaying);
        if (cooldown != null)
            cooldown.gameObject.SetActive(LevelManager.isPlaying);

        if (WeaponSwitcher.Instance != null)
        {
            currentWeapon = WeaponSwitcher.Instance.currentWeapon; // establish the current weapon datatype according to the weapon switcher
        }
        if (WeaponSwitcher.Instance == null || currentWeapon == null)
            return;

        if (cooldown != null && cooldown.gameObject.activeInHierarchy)
            UpdateCooldownSlider(currentWeapon);

        if (levelManager != null && levelManager.IsDebuffActive())
        {
            currentWeapon.currentFireRate = currentWeapon.fireRate * 1.75f;
        }
        else
        {
            currentWeapon.currentFireRate = currentWeapon.fireRate;
        }

        if(LevelManager.isPlaying)
        {
            if (Input.GetButtonDown("Fire1") && CooldownOver(currentWeapon.currentFireRate) && currentWeapon.currentMagRounds > 0)
            {
                Shoot(currentWeapon);
                lastFireTime = Time.time;
            }
            if (Input.GetButtonDown("Fire2"))
            {
                anim.SetBool("reloading", true);
                AudioSource.PlayClipAtPoint(reloadSFX, transform.position);
                Invoke("Reload", 2f);
            }
        }

        
    }

    void FixedUpdate()
    {
        VisualEffect(); // always runs :p

    }

    void RebindSceneReferences()
    {
        levelManager = FindAnyObjectByType<LevelManager>();

        crossHairImage = null;
        foreach (Image img in Resources.FindObjectsOfTypeAll<Image>())
        {
            // skip prefab assets, only want objects actually in a loaded scene
            if (!img.gameObject.scene.IsValid()) continue;
            if (img.CompareTag("CrossHair"))
            {
                crossHairImage = img;
                break;
            }
        }

        if (crossHairImage != null)
        {
            ogCrossHairColor = crossHairImage.color;
            ogCrossHairScale = crossHairImage.transform.localScale;
        }

        //Debug.Log("Rebind — crosshair: " + (crossHairImage != null) + " | magText: " + (magText != null));
    }

    void Shoot(WeaponSwitcher.Weapon gun)
    {

        if (gun.projectile == null)
        {
            Debug.LogWarning("No projectile assigned for weapon " + gun.name);
            return;
        }

        gun.currentMagRounds--;

        Vector3 spawnPos = transform.position + transform.forward * 3f;
        Quaternion spawnRot = gun.name == "Rocket Launcher" ? transform.rotation * Quaternion.Euler(0f, 180f, 0f) : transform.rotation * Quaternion.Euler(90f, 0f, 0f);
        GameObject round = Instantiate(gun.projectile, spawnPos, spawnRot);

        Rigidbody rb = round.GetComponent<Rigidbody>();
        if (rb) rb.linearVelocity = transform.forward * gun.bulletSpeed;

        if (gun.shootSound)
            AudioSource.PlayClipAtPoint(gun.shootSound, transform.position);
    }

    void UpdateCooldownSlider(WeaponSwitcher.Weapon gun)
    {
        cooldown.maxValue = gun.fireRate;
        cooldown.value = (Time.time - lastFireTime) / gun.fireRate;
    }

    public void UpdateMagText()
    {
        if (WeaponSwitcher.Instance == null || WeaponSwitcher.Instance.currentWeapon == null)
            return;

        currentWeapon = WeaponSwitcher.Instance.currentWeapon;

        if (magText != null)
            magText.text = currentWeapon.currentMagRounds + "/" + currentWeapon.maxMagRounds;
    }

    void VisualEffect()
    {
        if (crossHairImage == null || crossHairImage.gameObject == null)
            return;

        if (WeaponSwitcher.Instance == null || WeaponSwitcher.Instance.weapons == null)
            return;

        int i = WeaponSwitcher.Instance.currentWeaponIndex;

        if (i < 0 || i >= WeaponSwitcher.Instance.weapons.Length)
            return;

        currentWeapon = WeaponSwitcher.Instance.weapons[i];

        if (magText != null)
            magText.text = currentWeapon.currentMagRounds.ToString() + "/" + currentWeapon.maxMagRounds.ToString();
        if (crossHairImage == null)
            return;

        var step = animationSpeed * Time.deltaTime;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, currentWeapon.bulletRange))
        {
            if (hit.collider.CompareTag("Boss"))
            {
                levelManager.changeCortisolLevel(sunDmg);
            }
            if (hit.collider.CompareTag("Enemy") && CooldownOver(currentWeapon.fireRate))
            {
                crossHairImage.color = Color.Lerp(crossHairImage.color, Color.red, step);
                crossHairImage.transform.localScale = Vector3.Lerp(crossHairImage.transform.localScale, ogCrossHairScale / 2, step);
            }
            else
            {
                crossHairImage.color = Color.Lerp(crossHairImage.color, ogCrossHairColor, step);
                crossHairImage.transform.localScale = Vector3.Lerp(crossHairImage.transform.localScale, ogCrossHairScale, step);
            }
        }
        else
        {
            crossHairImage.color = Color.Lerp(crossHairImage.color, ogCrossHairColor, step);
            crossHairImage.transform.localScale = Vector3.Lerp(crossHairImage.transform.localScale, ogCrossHairScale, step);
        }
    }

    void Reload()
    {
        currentWeapon.currentMagRounds = currentWeapon.maxMagRounds;
        levelManager.changeCortisolLevel(10);
        anim.SetBool("reloading", false);
    }

    bool CooldownOver(float time)
    {
        return Time.time >= lastFireTime + time;
    }

    public void GetLevelManager()
    {
        levelManager = FindAnyObjectByType<LevelManager>();
    }
}