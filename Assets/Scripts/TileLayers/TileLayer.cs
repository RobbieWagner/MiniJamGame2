using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileLayer : MonoBehaviour
{
    [SerializeField] protected GameObject[] tiles;
    protected float tileSize;

    public virtual void PlaceLayerTiles(int maxRows, int maxColumns, bool[,] generatedTiles, int[,] selectedTileLayers, int layer, float size, Vector3 startingPosition)
    { 
        tileSize = size;
        Vector3 tilePosition;
        for(int y = 0; y < maxRows; y++)
        {
            for(int x = maxColumns - 1; x >= 0; x--)
            {
                if(selectedTileLayers[x,y] == layer)
                {
                    tilePosition = new Vector3(tileSize * x, tileSize * y, 0) + startingPosition;
                    // if(PositionHasNoTile(tilePosition, tileSize)) 
                    // {
                    //     AddTile(tilePosition, tiles[(int)Random.Range(0,tiles.Length)]);
                    // }

                    AddTile(tilePosition, tiles[(int)Random.Range(0,tiles.Length)]);
                }
            }
        }

        //PrintCurrentState(maxColumns,maxRows, selectedTileLayers);
    }

    protected void AddTile(Vector3 position, GameObject tile)
    {
        Transform tileT = Instantiate(tile).transform;
        tileT.position = position;
    }

    protected void AddTile(int column, int row, GameObject tile, Vector3 startingPosition)
    {
        Vector3 tilePosition = new Vector3(column * tileSize, row * tileSize, 0) + startingPosition;
        AddTile(tilePosition, tile);
    }

    protected void AddTile(int column, int row, GameObject tile, Vector3 startingPosition, bool[,] generated, int[,] layers, int layer)
    {
        Vector3 tilePosition = new Vector3(column * tileSize, row * tileSize, 0) + startingPosition;
        AddTile(tilePosition, tile);
        generated[column, row] = true; 
        layers[column, row] = layer;
    }

    protected bool PositionHasNoTile(Vector3 position, float tileSize)
    {
        Vector2 raycastDirection = new Vector2(1, 1);
        Vector3 RAYCAST_OFFSET = new Vector3(.01f, .01f, 0f);
        RaycastHit2D hit = Physics2D.Raycast(position + RAYCAST_OFFSET, raycastDirection, tileSize / 10f);

        if (hit.collider != null)
        {
            return false;
        }
        return true;
    }

    protected bool[] CheckForAdjacentTiles(int column, int row, int maxColumns, int maxRows, int[,] layers, int layer)
    {
        bool[] adjacentTilesShareLayer = {false, false, false, false};

        if(row < maxRows-1) adjacentTilesShareLayer[0] = layers[column, row+1] == layer;
        if(column < maxColumns-1) adjacentTilesShareLayer[1] = layers[column+1, row] == layer;
        if(row > 0) adjacentTilesShareLayer[2] = layers[column, row-1] == layer;
        if(column > 0) adjacentTilesShareLayer[3] = layers[column-1, row] == layer;

        return adjacentTilesShareLayer;
    }

    protected bool[] CheckForGeneratedTiles(int column, int row, int maxColumns, int maxRows, bool[,] generated, int layer)
    {
        bool[] adjacentTilesAreGenerated = {false, false, false, false};

        if(row < maxRows-1) adjacentTilesAreGenerated[0] = generated[column, row+1];
        if(column < maxColumns-1) adjacentTilesAreGenerated[1] = generated[column+1, row];
        if(row > 0) adjacentTilesAreGenerated[2] = generated[column, row-1];
        if(column > 0) adjacentTilesAreGenerated[3] = generated[column-1, row];

        return adjacentTilesAreGenerated;
    }

    protected void PrintCurrentState(int maxColumns, int maxRows, int[,] layers)
    {
        string print = "";
        for(int x = maxColumns - 1; x >= 0 ; x--)
        {
            for(int y = 0; y < maxRows; y++)
            {
                print+=layers[y,x];
            }
            print +="\n";
        }

        Debug.Log(print);
    }
}
