using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWithRaft : Player
{
    [SerializeField] protected float waterSpeed = 3f;
    HashSet<Collider2D> waterTilesTouched;

    [HideInInspector] public bool canSpawnRaft;
    [SerializeField] private GameObject raftGO;

    bool isRaftOut;
    public bool isInWater;

    protected override void Awake()
    {
        base.Awake();

        waterTilesTouched = new HashSet<Collider2D>();

        isRaftOut = false;
        isInWater = false;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.CompareTag("Water"))
        {
            if(!waterTilesTouched.Contains(other)) waterTilesTouched.Add(other);
            ChangeSpeed(waterSpeed);
            isInWater = true;
            SwitchToSwimSound(swimSound);
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.gameObject.CompareTag("Water"))
        {
            if(waterTilesTouched.Contains(other)) waterTilesTouched.Remove(other);
            if(waterTilesTouched.Count == 0) 
            {
                ChangeSpeed(speed);
                isInWater = false;
                if(running) SwitchFootstepSound(runSound);
                else SwitchFootstepSound(footstepsSound);
            }
        }
    }

    private void OnRaft()
    {
        if(waterTilesTouched.Count > 0 && !isRaftOut) SpawnRaft();
        else if(isRaftOut) 
        {
            Raft raft = raftGO.GetComponent<Raft>();
            if(raft != null && !raft.isInRaft) RemoveRaft();
        }
    }

    protected override void OnMovement(InputValue value) 
    {
        if(canMove)
        {
            moveInput = value.Get<Vector2>();
            rb2d.velocity = moveInput * currentSpeed;
            if(moveInput.x != 0 || moveInput.y != 0) animator.SetBool("walking", true);
            else animator.SetBool("walking", false);
            UpdateRotation();
            if(!currentFootstepsSound.isPlaying && (moveInput.x != 0 || moveInput.y != 0)) currentFootstepsSound.Play();
            else if((moveInput.x == 0 && moveInput.y == 0) || Raft.Instance.isInRaft) currentFootstepsSound.Stop();
        }
    }

    private void SpawnRaft()
    {
        isRaftOut = true;
        List<Collider2D> raftPlacements = waterTilesTouched.ToList<Collider2D>();
        raftGO.transform.position = raftPlacements[0].transform.position;
    }

    private void RemoveRaft()
    {
        isRaftOut = false;
        raftGO.transform.position = new Vector2(-500, -500);
    }

    protected override void OnRun()
    {
        running = !running;
        if(running) currentSpeed *= 2;
        else currentSpeed /= 2;

        rb2d.velocity = moveInput * currentSpeed;

        if(!Raft.Instance.isInRaft)
        {
            if(!isInWater && running) SwitchFootstepSound(runSound);
            else if(!isInWater) SwitchFootstepSound(footstepsSound);
            else SwitchFootstepSound(swimSound);
        }
    }

    private void SwitchToSwimSound(AudioSource sound)
    {
        if(!Raft.Instance.isInRaft)
        {
            bool soundWasPlaying = currentFootstepsSound.isPlaying;
            currentFootstepsSound.Stop();
            currentFootstepsSound = sound;
            if(soundWasPlaying)currentFootstepsSound.Play();
        }
    }
}
