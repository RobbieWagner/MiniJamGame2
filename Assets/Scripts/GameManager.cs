using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private TileGenerator tileGenerator;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject signalFire;

    // Start is called before the first frame update
    void Start()
    {
        tileGenerator.GenerateGame();
        Vector3 startingTilePos = tileGenerator.FindNonWaterTile();
        Vector3 playerPos = startingTilePos - new Vector3(1,1,0);
        player.transform.position = playerPos;

        GameObject fire = Instantiate(signalFire);
        fire.transform.position = startingTilePos + new Vector3(1,1,0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
