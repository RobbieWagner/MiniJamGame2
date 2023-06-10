using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] protected float speed;
    protected float currentSpeed;
    protected PlayerMovement playerMovement;
    [HideInInspector] public Rigidbody2D rb2d;
    protected Vector2 moveInput;
    public static Player Instance {get; protected set;}
    public bool canMove = true;
    public Animator animator;

    protected bool running;

    public Transform spriteT;

    protected AudioSource currentFootstepsSound;
    [SerializeField] protected AudioSource footstepsSound;
    [SerializeField] protected AudioSource runSound;
    [SerializeField] protected AudioSource swimSound;

    protected virtual void Awake()
    {
        //Debug.Log("h1");
        playerMovement = new PlayerMovement();

        rb2d = GetComponent<Rigidbody2D>();

        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 

        currentSpeed = speed;
        running = false;

        currentFootstepsSound = footstepsSound;
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

    protected void OnMovement(InputValue value) 
    {
        if(canMove)
        {
            moveInput = value.Get<Vector2>();
            rb2d.velocity = moveInput * currentSpeed;
            if(moveInput.x != 0 || moveInput.y != 0) animator.SetBool("walking", true);
            else animator.SetBool("walking", false);
            UpdateRotation();
            if(!currentFootstepsSound.isPlaying && (moveInput.x != 0 || moveInput.y != 0)) currentFootstepsSound.Play();
            else if(moveInput.x == 0 && moveInput.y == 0) currentFootstepsSound.Stop();
        }
    }

    protected void ChangeSpeed(float newSpeed)
    {
        currentSpeed = newSpeed;
        if(running) currentSpeed *= 2;
        rb2d.velocity = moveInput * currentSpeed;
    }

    public void StopMovement()
    {
        rb2d.velocity = Vector2.zero;
        moveInput = Vector2.zero;
        currentFootstepsSound.Stop();
    }

    private void UpdateRotation()
    {
        if(moveInput.x > 0)
        {
            if(moveInput.y > 0) spriteT.rotation = Quaternion.Euler(0, 0, -45);
            else if(moveInput.y < 0) spriteT.rotation = Quaternion.Euler(0, 0, -135);
            else spriteT.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if(moveInput.x < 0)
        {
            if(moveInput.y > 0)spriteT.rotation = Quaternion.Euler(0, 0, 45);
            else if(moveInput.y < 0) spriteT.rotation = Quaternion.Euler(0, 0, 135);
            else spriteT.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if(moveInput.y > 0)
        {
            spriteT.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if(moveInput.y < 0)
        {
            spriteT.rotation = Quaternion.Euler(0, 0, 180);
        }
    }

    protected virtual void OnRun()
    {
        running = !running;
        if(running) currentSpeed *= 2;
        else currentSpeed /= 2;

        rb2d.velocity = moveInput * currentSpeed;
    }

    protected void SwitchFootstepSound(AudioSource sound)
    {
        currentFootstepsSound.Stop();
        currentFootstepsSound = sound;
        currentFootstepsSound.Play();
    }
}
