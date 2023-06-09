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

    [SerializeField] Player player;
    bool isChasingPlayer;
    bool isMoving;
    Vector3 destination;
    [SerializeField] float destinationReachDistance;

    private void Awake()
    {
        transform.position = tileGenerator.CalculateWorldBorder() - new Vector2(7,7);
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
            lookDirection = player.transform.position - transform.position;
        }

        lookDirection = navAgent.nextPosition - transform.position;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, lookDirection);
        transform.position = navAgent.nextPosition;
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
    }
}
