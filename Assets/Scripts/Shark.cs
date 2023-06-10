using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Shark : MonoBehaviour
{

    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] TileGenerator tileGenerator;

    [SerializeField] float swimSpeed;
    [SerializeField] float chaseSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float chaseDistance;

    [SerializeField] PlayerWithRaft player;
    bool isChasingPlayer;
    bool isMoving;
    Vector3 destination;
    [SerializeField] float destinationReachDistance;

    [SerializeField] int corner;

    [SerializeField] AudioSource chaseMusic;

    private void Awake()
    {
        transform.position = tileGenerator.CalculateWorldBorder(corner);
        isMoving = false;
        navAgent.angularSpeed = 0f;
        navAgent.updatePosition = false;

        transform.rotation = Quaternion.Euler(0,0,0);
    }

    private void Update() 
    {
        Vector2 lookDirection = Vector2.zero;

        if(!isChasingPlayer)
        {
            if(Vector3.Distance(transform.position, destination) < destinationReachDistance)
            {
                destination = FindNewDestination();
                isMoving = false;
            }
            if(!isMoving)
            {
                navAgent.SetDestination(destination);
                navAgent.speed = swimSpeed;
                isMoving = true;
            }
        }
        else
        {
            if(!chaseMusic.isPlaying) chaseMusic.Play();
            navAgent.SetDestination(player.transform.position);
            NavMeshPath navMeshPath = new NavMeshPath();
            if(!(navAgent.CalculatePath(player.transform.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete) || (Raft.Instance.isOnLand && Raft.Instance.isInRaft))
            {
                isChasingPlayer = false;
                chaseMusic.Stop();
                //Debug.Log("stop chasing");
                destination = FindNewDestination();
                navAgent.SetDestination(destination);
                navAgent.speed = swimSpeed;
            }
        }

        lookDirection = navAgent.nextPosition - transform.position;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, lookDirection);
        transform.position = navAgent.nextPosition;

        if(!isChasingPlayer && ((Raft.Instance.isInRaft && !Raft.Instance.isOnLand) || (player.isInWater && !Raft.Instance.isInRaft)) && Vector3.Distance(Player.Instance.transform.position, transform.position) < chaseDistance)
        {
            isChasingPlayer = true;
            navAgent.speed = chaseSpeed;
            //Debug.Log("chasing");
        }
    }

    private Vector3 FindNewDestination()
    {
        float[] xPosRange = tileGenerator.GetXPositionRange();
        float[] yPosRange = tileGenerator.GetYPositionRange();

        NavMeshPath navMeshPath = new NavMeshPath();
        Vector3 point = new Vector3(UnityEngine.Random.Range(xPosRange[0], xPosRange[1]), UnityEngine.Random.Range(yPosRange[0], yPosRange[1]), 0);
        while(!(navAgent.CalculatePath(point, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete))
        point = new Vector3(UnityEngine.Random.Range(xPosRange[0], xPosRange[1]), UnityEngine.Random.Range(yPosRange[0], yPosRange[1]), 0);
        return point;
    }

    private void OnTriggerStay2D(Collider2D other) 
    { 
        if(other.gameObject.CompareTag("Raft")) 
        {
            if(!isChasingPlayer)
            {
                destination = FindNewDestination();
                navAgent.SetDestination(destination);
            }
            else SceneManager.LoadScene("SharkDeathGameOver");
        }

        if(other.gameObject.CompareTag("Shark"))
        {
            destination = FindNewDestination();
            navAgent.SetDestination(destination);
        }

        if(other.gameObject.CompareTag("Player") && player.isInWater) SceneManager.LoadScene("SharkDeathGameOver");
    }
}
