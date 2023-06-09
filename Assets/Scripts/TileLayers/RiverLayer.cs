using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverLayer : TileLayer
{

    [SerializeField] private GameObject bridgeTile;
    [SerializeField][Range(1,3)] private int bridges;
    [SerializeField] private int distanceToCheck;

    enum RIVER_TURN
    {
        horizontal,
        turnDown,
        turnUp,
        verticalDown,
        verticalUp,
        turnLeft,
        turnRight
    }

    public override void PlaceLayerTiles(int maxRows, int maxColumns, bool[,] generatedTiles, int[,] selectedTileLayers, int layer, float size, Vector3 startingPosition)
    {
        tileSize = size;
        int column = 0;
        int row = 0;
        while(selectedTileLayers[0, row] != layer && row < maxRows) row++;

        AddTile(column, row, tiles[tiles.Length-1], startingPosition, generatedTiles, selectedTileLayers, layer);
        column++;

        int lastplacement = 0;

        List<List<int>> possibleBridgeLocations = new List<List<int>>();

        while(column < maxColumns-1)
        {
            bool[] riverTilesNESW = CheckForAdjacentTiles(column, row, maxColumns, maxRows, selectedTileLayers, layer);
            if(riverTilesNESW[3] && riverTilesNESW[1]) 
            {
                if(bridgeTile == null) AddTile(column, row, tiles[(int) RIVER_TURN.horizontal], startingPosition, generatedTiles, selectedTileLayers, layer);
                else possibleBridgeLocations.Add(new List<int>(){column, row});
                column++;
            }
            else if(riverTilesNESW[3] && riverTilesNESW[2])
            {
                AddTile(column, row, tiles[(int) RIVER_TURN.turnDown], startingPosition, generatedTiles, selectedTileLayers, layer);
                row--;
            }
            else if(riverTilesNESW[3] && riverTilesNESW[0])
            {
                AddTile(column, row, tiles[(int) RIVER_TURN.turnUp], startingPosition, generatedTiles, selectedTileLayers, layer);
                row++;
            }
            else if(riverTilesNESW[0] && riverTilesNESW[2])
            {
                bool[] tilesGenerated = CheckForGeneratedTiles(column, row, maxColumns, maxRows, generatedTiles, layer);
                bool nextRiverIsUp = !tilesGenerated[0];
                if(nextRiverIsUp) 
                {
                    AddTile(column, row, tiles[(int) RIVER_TURN.verticalUp], startingPosition, generatedTiles, selectedTileLayers, layer);
                    row++;
                }
                else 
                {
                    AddTile(column, row, tiles[(int) RIVER_TURN.verticalDown], startingPosition, generatedTiles, selectedTileLayers, layer);
                    row--;
                }
            }
            else if(riverTilesNESW[0] && riverTilesNESW[1])
            {
                AddTile(column, row, tiles[(int) RIVER_TURN.turnLeft], startingPosition, generatedTiles, selectedTileLayers, layer);
                column++;
            }
            else if(riverTilesNESW[2] && riverTilesNESW[1])
            {
                AddTile(column, row, tiles[(int) RIVER_TURN.turnRight], startingPosition, generatedTiles, selectedTileLayers, layer);
                column++;
            }
            else 
            {
                column++;
            }
        }

        if(bridgeTile != null)
        {
            ShuffleCoordinates(possibleBridgeLocations);
            int tileToBridge = 0;
            List<int> bridgeYCoordinates = new List<int>();
            
            //adds bridges
            for(int i = 0; i < bridges; i++)
            {
                bool bridgeCheck = HasNearbyBridge(possibleBridgeLocations[tileToBridge][0], bridgeYCoordinates);
                bool[] borderChecks = CheckForAdjacentTiles(possibleBridgeLocations[tileToBridge][0], possibleBridgeLocations[tileToBridge][1], maxColumns, maxRows, selectedTileLayers, 0);
                while((borderChecks[0] || borderChecks[2]) && tileToBridge < possibleBridgeLocations.Count && !bridgeCheck)
                {
                    tileToBridge++;
                    borderChecks = CheckForAdjacentTiles(possibleBridgeLocations[tileToBridge][0], possibleBridgeLocations[tileToBridge][1], maxColumns, maxRows, selectedTileLayers, 0);
                    bridgeCheck = HasNearbyBridge(possibleBridgeLocations[tileToBridge][1], bridgeYCoordinates);
                }
                AddTile(possibleBridgeLocations[tileToBridge][0], possibleBridgeLocations[tileToBridge][1], bridgeTile, startingPosition, generatedTiles, selectedTileLayers, layer);
                bridgeYCoordinates.Add(possibleBridgeLocations[tileToBridge][1]);
                possibleBridgeLocations.RemoveAt(tileToBridge);
            }
        }

         for(int i = 0; i < possibleBridgeLocations.Count; i++)
        {
            AddTile(possibleBridgeLocations[i][0], possibleBridgeLocations[i][1], tiles[(int) RIVER_TURN.horizontal], startingPosition, generatedTiles, selectedTileLayers, layer);
        }

        AddTile(column, row, tiles[tiles.Length-1], startingPosition, generatedTiles, selectedTileLayers, layer);
    }

    public void ShuffleCoordinates(List<List<int>> coordinates)  
    {  
        int n = coordinates.Count;  
        while (n > 1) {  
            n--;  
            int k = (int) UnityEngine.Random.Range(0, n + 1);  
            List<int> value = coordinates[k];  
            coordinates[k] = coordinates[n];  
            coordinates[n] = value;  
        }   
    }

    private bool HasNearbyBridge(int x, List<int> bridgeYCoordinates)
    {
        for(int i = 0; i < bridgeYCoordinates.Count; i++)
        {
            int coordinate = bridgeYCoordinates[i];
            if(coordinate == x) return true;

            //Debug.Log(coordinate + " " + x);
        }
        return false;
    }
}
