using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class RandyController : MonoBehaviour
{
    public float speed = 8f;
    //public float jumpHeight = 3f; jumping may be implemented in a later version, depending on if Randy gets the knee surgery
    //public float gravity = 9.81f;
    //public float crouchHeight = 0.5f; crouching may be implemented if Randy takes his iron supplements
    //public float standHeight = 2f;
    //public float crouchSpeed = 4f;
    public float debuffSpeedMultiplier = 0.5f;
    public Vector3 spawnPos;

    Vector3 moveDirection;
    float verticalVelocity;
    //bool isCrouching = false;
    CharacterController controller;
    LevelManager levelManager;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        controller.enabled = false;
        transform.position = spawnPos;
        controller.enabled = true;
        levelManager = FindAnyObjectByType<LevelManager>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        moveDirection = transform.right * moveX + transform.forward * moveZ;
        moveDirection.Normalize();

        float currentSpeed = speed;
        if(levelManager && levelManager.IsDebuffActive())
        {
            currentSpeed *= debuffSpeedMultiplier;
        }

        Vector3 finalMove = moveDirection * currentSpeed + Vector3.up * verticalVelocity;
        controller.Move(finalMove * Time.deltaTime);
    }


}