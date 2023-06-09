using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private TileGenerator tileGenerator;
    [SerializeField] private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        tileGenerator.GenerateGame();
        Vector3 playerPos = tileGenerator.FindNonWaterTile();
        player.transform.position = playerPos;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
