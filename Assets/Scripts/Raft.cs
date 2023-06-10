using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Raft : MonoBehaviour
{
    bool canEnterRaft;
    bool canExitRaft;

    [HideInInspector] public bool isInRaft;

    [SerializeField] float raftWaterSpeed = 11f;
    [SerializeField] float raftGroundSpeed = 2f;
    float raftCurrentSpeed;

    HashSet<Collider2D> beachTilesTouched;

    private Vector2 moveInput;

    protected PlayerMovement playerMovement;

    [SerializeField] private Rigidbody2D rb2d;

    [SerializeField] private Sprite movingSprite;
    [SerializeField] private Sprite idleSprite;
    private SpriteRenderer spriteR;

    public static Raft Instance{get; private set;}

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 

        canEnterRaft = false;
        canExitRaft = false;
        isInRaft = false;

        beachTilesTouched = new HashSet<Collider2D>();
        raftCurrentSpeed = raftWaterSpeed;

        playerMovement = new PlayerMovement();
        spriteR = GetComponent<SpriteRenderer>();
        spriteR.sprite = idleSprite;
    }

    protected void OnEnable() 
    {
        //Debug.Log("h2");
        playerMovement.Player.Enable();
    }

    protected void OnDisable() 
    {
        //Debug.Log("h3");
        playerMovement.Player.Disable();
    }

    // Allow player to enter raft if they are interacting
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.CompareTag("Player"))
        {
            canEnterRaft = true;
        }
    }

    // Allow player to exit raft if the raft is touching land
    private void OnTriggerStay2D(Collider2D other) 
    {
        if(other.gameObject.CompareTag("Ground"))
        {
            canExitRaft = true;
            if(!beachTilesTouched.Contains(other)) beachTilesTouched.Add(other);
            ChangeSpeed(raftGroundSpeed);
        }
    }

    // Disallow player from entering or exiting the raft under certain circumstances
    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.gameObject.CompareTag("Player"))
        {
            canEnterRaft = false;
        }

        if(other.gameObject.CompareTag("Ground"))
        {
            if(beachTilesTouched.Contains(other)) beachTilesTouched.Remove(other);
            if(beachTilesTouched.Count == 0) 
            {
                canExitRaft = false;
                ChangeSpeed(raftWaterSpeed);
            }
        }
    }

    // Enter/Exit raft when interacting
    private void OnInteract()
    {
        if(canEnterRaft && !isInRaft)
        {
            EnterRaft();
        }
        else if(canExitRaft && isInRaft && beachTilesTouched.Count > 0)
        {
            ExitRaft();
        }
    }
    
    // Enter the raft
    private void EnterRaft()
    {
        isInRaft = true;
        Player.Instance.transform.position = transform.position;
        Player.Instance.canMove = false;
        Player.Instance.transform.SetParent(rb2d.transform);

        Player.Instance.StopMovement();

        Player.Instance.animator.SetBool("walking", false);

        Player.Instance.spriteT.rotation = Quaternion.Euler(0, 0, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    //Exit the raft
    private void ExitRaft()
    {
        isInRaft = false;
        List<Collider2D> playerPlacements = beachTilesTouched.ToList<Collider2D>();
        Player.Instance.transform.position = playerPlacements[0].transform.position;
        Player.Instance.canMove = true;
        Player.Instance.transform.SetParent(null);

        rb2d.velocity = Vector2.zero;
        Player.Instance.rb2d.velocity = Vector2.zero;
        moveInput = Vector2.zero;
        //Debug.Log(Player.Instance.rb2d.velocity);
        //Debug.Log(rb2d.velocity);
    }

    private void OnMovement(InputValue value)
    {
        if(isInRaft)
        {
            moveInput = value.Get<Vector2>();
            rb2d.velocity = moveInput * raftCurrentSpeed;
            //Player.Instance.rb2d.velocity = moveInput * raftCurrentSpeed;
            UpdateRotation();

            if(moveInput.x != 0 || moveInput.y != 0) spriteR.sprite = movingSprite;
            else spriteR.sprite = idleSprite;
        }
        else
        {
           rb2d.velocity = Vector2.zero; 
        }
    }

    private void ChangeSpeed(float newSpeed)
    {
        raftCurrentSpeed = newSpeed;
        rb2d.velocity = moveInput * raftCurrentSpeed;
    }

     private void UpdateRotation()
    {
        Quaternion rotation;

        if(moveInput.x > 0)
        {
            if(moveInput.y > 0) rotation = Quaternion.Euler(0, 0, -45);
            else if(moveInput.y < 0) rotation = Quaternion.Euler(0, 0, -135);
            else rotation = Quaternion.Euler(0, 0, -90);
        }
        else if(moveInput.x < 0)
        {
            if(moveInput.y > 0)rotation = Quaternion.Euler(0, 0, 45);
            else if(moveInput.y < 0) rotation = Quaternion.Euler(0, 0, 135);
            else rotation = Quaternion.Euler(0, 0, 90);
        }
        else if(moveInput.y > 0)
        {
            rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            rotation = Quaternion.Euler(0, 0, 180);
        }

        transform.rotation = rotation;
        Player.Instance.spriteT.rotation = rotation;

    }
}
