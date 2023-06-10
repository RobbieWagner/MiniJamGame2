using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWithRaft : Player
{
    [SerializeField] protected float waterSpeed = 3f;
    HashSet<Collider2D> waterTilesTouched;

    [HideInInspector] public bool canSpawnRaft;
    [SerializeField] private GameObject raftGO;

    bool isRaftOut;

    protected override void Awake()
    {
        base.Awake();

        waterTilesTouched = new HashSet<Collider2D>();

        isRaftOut = false;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.CompareTag("Water"))
        {
            if(!waterTilesTouched.Contains(other)) waterTilesTouched.Add(other);
            ChangeSpeed(waterSpeed);
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
}