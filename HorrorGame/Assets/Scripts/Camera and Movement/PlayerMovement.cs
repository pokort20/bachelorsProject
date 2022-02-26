using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController playerController;
    private float movementSpeed;
    public float gravity;
    public float jumpHeight;
    public float sprintMult;
    public float crouchMult;
    public float crouchHeight;
    public float standardHeight;

    private bool isSprinting;
    private bool isCrouching;
    private bool wasCrouching;
    private Vector3 moveVec;
    Vector3 velocity;
    public Transform groundCollisionCheck;
    public float groundCollisionCheckSphereRadius = 0.2f;
    public LayerMask groundMask;
    bool isGrounded;
    private enum playerStates : int
    {
        NORMAL = 0,
        CROUCHING = 1,
        UNCROUCHING = 2,
        SPRINTING = 3
    }
    private playerStates playerState = playerStates.NORMAL;
    // Start is called before the first frame update
    void Start()
    {
        movementSpeed = GameManager.instance.movementSpeed;
        gravity = -19.62f;
        jumpHeight = 0.7f;
        sprintMult = 1.5f;
        crouchMult = 0.75f;
        crouchHeight = 1.0f;
        standardHeight = 1.7f;
        isCrouching = false;
        isSprinting = false;
        isCrouching = !!isCrouching; //remove this once isCrouching is used, just to get rid of the warning
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCollisionCheck.position, groundCollisionCheckSphereRadius, groundMask);

        if (isGrounded && velocity.y < 0.0f) velocity.y = -1.5f;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        moveVec = transform.right * horizontal + transform.forward * vertical;
        if(playerState != playerStates.UNCROUCHING)
        {
            FindPlayerState();
        }
        else
        {
            Vector3 bottomSphere = new Vector3(playerController.transform.position.x, playerController.transform.position.y - 0.5f, playerController.transform.position.z);
            Vector3 topSphere = new Vector3(playerController.transform.position.x, playerController.transform.position.y + 0.5f, playerController.transform.position.z);

            LayerMask mask =~ LayerMask.GetMask("Player");
            if (Physics.CapsuleCast(bottomSphere, topSphere, 2.0f, playerController.transform.forward, mask)) 
            {
                Debug.Log("Can't uncrouch yet");
            }
            else
            {
                Debug.Log("Uncrouching");
                playerState = playerStates.NORMAL;
            }
        }
        isCrouching = false;
        isSprinting = false;
        switch (playerState)
        {
            case playerStates.NORMAL:
                //Debug.Log("Normal state");
                moveVec *= movementSpeed;
                playerController.height = standardHeight;
                break;
            case playerStates.CROUCHING:
                //Debug.Log("Crouching state");
                moveVec *= movementSpeed * crouchMult;
                playerController.height = crouchHeight;
                isCrouching = true;
                break;
            case playerStates.UNCROUCHING:
                //Debug.Log("Uncrouching state");
                break;
            case playerStates.SPRINTING:
                //Debug.Log("Sprinting state");
                moveVec *= movementSpeed * sprintMult;
                playerController.height = standardHeight;
                isSprinting = true;
                break;
            default:
                //Debug.Log("Default");
                break;
        }
        playerController.Move(moveVec * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        playerController.Move(velocity * Time.deltaTime);
        updateStamina();
    }
    private void FindPlayerState()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            playerState = playerStates.CROUCHING;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            playerState = playerStates.UNCROUCHING;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && canSprint())
        {
            playerState = playerStates.SPRINTING;
        }
        else
        {
            playerState = playerStates.NORMAL;
        }
    }
    public Vector3 getPlayerMoveVec()
    {
        return moveVec;
    }
    private bool canSprint()
    {
        if(isSprinting && GameManager.instance.stamina > 0.0f)
        {
            return true;
        }
        else if(GameManager.instance.stamina > 5.0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void updateStamina()
    {
        if(playerState == playerStates.SPRINTING)
        {
            GameManager.instance.stamina -= 1.0f * Time.deltaTime;
        }
        else if(playerState == playerStates.CROUCHING || playerState == playerStates.UNCROUCHING)
        {
            GameManager.instance.stamina += 0.7f * Time.deltaTime;
        }
        else
        {
            GameManager.instance.stamina += 0.6f * Time.deltaTime;
        }

        if(GameManager.instance.stamina > GameManager.instance._maxStamina)
        {
            GameManager.instance.stamina = GameManager.instance._maxStamina;
        }
        if(GameManager.instance.stamina < 0.0f)
        {
            GameManager.instance.stamina = 0.0f;
        }
        //Debug.Log("Stamina: " + GameManager.instance.stamina);
    }
}
