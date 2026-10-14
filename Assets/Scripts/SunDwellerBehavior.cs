using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections.Generic;

public class SunDwellerBehavior : MonoBehaviour
{
    public enum State {Wander, Follow, Die}
    public State currentState = State.Wander;
    public float moveSpeed = 3f;
    public float cortisolIncreaseRate = 0.2f;
    public float maxProximityDistance = 35f;
    
    public float maxHP = 20;
    public GameObject deathEffect;
    public GameObject hitEffect;
    public AudioClip deathSFX;
    public Slider hpSlider;
    public Transform eyes;
    Transform player;
    LevelManager levelManager;
    List<GameObject> skins = new List<GameObject> {};
    NavMeshAgent agent;
    float totalRateChange;
    float hp;
    public AudioClip hitSFX;
    public float bobHeight = 0.6f;
    public float bobSpeed = 3f;
    Transform skinTransform;
    float baseSkinY;


    void Start()
    {
        
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.updatePosition = false;
        RandomSkin();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        hp = maxHP;
        hpSlider.maxValue = maxHP;
        hpSlider.value = maxHP;
        levelManager = FindAnyObjectByType<LevelManager>();
    }
    
    void Update()
    {
        if(!LevelManager.isPlaying)
            return;
        if(!player)
            return;

        // sync navmesh X and Z but keep our Y
        Vector3 navPos = agent.nextPosition;
        float bob = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(navPos.x, navPos.y + bob, navPos.z);
        agent.nextPosition = new Vector3(navPos.x, navPos.y, navPos.z);
        agent.nextPosition = transform.position;

        if (Input.GetKeyDown(KeyCode.Q)) //for testing purposes!! instakill the sun dwellers when pressing q
        {
            currentState = State.Die;
        }
        switch (currentState)
        {
            case State.Wander:
                Wander();
                break;
            case State.Follow:
                Follow();
                break;
            case State.Die:
                Die();
                break;
        }
    }

    void Wander() //find the nearest pickup
    {
        GameObject[] Pickups = GameObject.FindGameObjectsWithTag("Pickup");
        GameObject nearestPickup = GameObject.FindGameObjectWithTag("Pickup");
        if(Pickups != null && Pickups.Length > 0)
        {
            foreach(GameObject pickup in Pickups)
            {
                var distance = Vector3.Distance(transform.position, nearestPickup.transform.position);
                if (Vector3.Distance(transform.position, pickup.transform.position) < distance)
                {
                    nearestPickup = pickup;
                }
            }
            agent.SetDestination(nearestPickup.transform.position);
        }

        if(Vector3.Distance(transform.position, player.position) <= maxProximityDistance)
        {
            currentState = State.Follow;
        }
        
    }

    void Follow() //follow the player, only affect player cortisol if there is line of sight. randy hates ppl looking at him
    {
        agent.SetDestination(player.position);
        SpikeCortisol();

        if(Vector3.Distance(transform.position, player.position) > maxProximityDistance)
        {
            currentState = State.Wander;
        }
    }

    void Die()
    {
        levelManager.changeCortisolRate(-1f*totalRateChange); //remove rate increase when dead
        levelManager.changeCortisolLevel(-10); //decrease cortisol when defeated
        levelManager.AddKill();
        if(deathEffect)
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        if(deathSFX)
            AudioSource.PlayClipAtPoint(deathSFX, transform.position);
        Destroy(gameObject);
    }

    void SpikeCortisol()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        float distanceMultiplier = (maxProximityDistance - distance)/maxProximityDistance; //the closer it is the more cortisol rate will increase
        if(HasLineOfSight(player))
        {
            totalRateChange += distanceMultiplier * cortisolIncreaseRate * Time.deltaTime; //keep track of how much rate has changed cumulatively
            levelManager.changeCortisolRate(distanceMultiplier * cortisolIncreaseRate * Time.deltaTime); //change cortisol rate
        }
        else
        {
            //remove rate increase when out of range
            levelManager.changeCortisolRate(-1f*totalRateChange);
            totalRateChange = 0; //reset the total rate change
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bullet"))
        {
            WeaponSwitcher.Weapon currentWeapon = WeaponSwitcher.Instance.currentWeapon;
            TakeDamage(currentWeapon.bulletDMG);
            UpdateHP();
        }
        if (other.CompareTag("Player"))
        {
            levelManager.LoseGame();
        }
        if (other.CompareTag("Pickup"))
        {
            Destroy(other.gameObject);
        }
    }

    bool HasLineOfSight(Transform target)
    {
        RaycastHit hit;
        Vector3 direction = (target.position - eyes.position).normalized;

        if(Physics.Raycast(eyes.position, direction, out hit, maxProximityDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
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

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hitSFX)
            AudioSource.PlayClipAtPoint(hitSFX, transform.position);
        if (hp <= 0)
        {
            currentState = State.Die;
        }
        else
        {
            // Play hit effect
            if (hitEffect)
                Instantiate(hitEffect, new Vector3(transform.position.x, transform.position.y + 5, transform.position.z), Quaternion.identity);
        }
    }

    public void UpdateHP()
    {
        hpSlider.value = hp;
    }

    public void UpdateStats(float spd, float rate, float maxDist, float maxhp)
    {
        moveSpeed = spd;
        cortisolIncreaseRate = rate;
        maxProximityDistance = maxDist;
        maxHP = maxhp;
        hp = maxhp;
    }
}