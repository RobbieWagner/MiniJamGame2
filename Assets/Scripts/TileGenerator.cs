//References:
//Use of noise mapping: https://www.youtube.com/watch?v=DBjd7NHMgOE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
﻿using NavMeshPlus.Components;

public class TileGenerator : MonoBehaviour
{
    [SerializeField]private TileLayer[] baseGameTileLayers;
    [SerializeField]private TileLayer[] mapFeatureLayers;
    [SerializeField]private MapFeature[] mapFeatures;
    [HideInInspector] public static int firstMapFeatureLayer;
    private int[,] selectedTileLayers;
    private bool[,] generatedTiles;
    [SerializeField] private Transform[] manuallySelectedTiles;
    [SerializeField] private int maxRows;
    [SerializeField] private int maxColumns;
    [SerializeField] private float tileSize = 10f;
    [SerializeField] private int borderLimit;
    float[,] noiseMap;
    float[,] fallOffMap;
    private Vector3 startingPosition;
    List<int> baseGameLayers;

    [SerializeField] int firstNonWaterTileLayer;

    [SerializeField] Transform layout;
    [SerializeField] private NavMeshSurface navMesh;
    [SerializeField] private float PERLIN_SCALE = .05f;

    public void GenerateGame()
    {
        //tileSize = baseGameTileLayers[0].tiles[0].transform.localScale.x;
        float startingXPosition = (-maxRows * tileSize / 2) - (-maxRows * tileSize / 2) % tileSize;
        float startingYPosition = 0;
        startingPosition = new Vector3(startingXPosition, startingYPosition, 0f);

        baseGameLayers = new List<int>();
        AssignBaseGameLayers();

        selectedTileLayers = new int[maxColumns, maxRows];
        generatedTiles = new bool[maxColumns, maxRows];
        for (int y = 0; y < maxRows; y++)
        {
            for (int x = 0; x < maxColumns; x++)
            {
                generatedTiles[x,y] = false;
            }
        }

        noiseMap = InitializeNoiseMap();
        fallOffMap = InitializeFallOffMap();

        GenerateInitialTilemap();
        //PrintCurrentState();
        AssignBorderCells();
        MarkManuallySelectedTiles();

        firstMapFeatureLayer = baseGameTileLayers.Length;

        for(int i = baseGameTileLayers.Length; i < baseGameTileLayers.Length + mapFeatures.Length; i++)
        {
            mapFeatures[i-baseGameTileLayers.Length].AddMapFeature(i, maxColumns, maxRows, selectedTileLayers, baseGameLayers, firstMapFeatureLayer);
        }

        //PrintCurrentState();
        PlaceTiles();
        navMesh.BuildNavMeshAsync();
    }

    public Vector2 CalculateWorldBorder()
    {
        return new Vector2(maxColumns * tileSize / 2, maxRows * tileSize);
    }

    private void AssignBaseGameLayers()
    {
        //Checks if each tile layer in the base layers is of the default type. Adds them to a list if they are of the default type.
        for(int i = 0; i < baseGameTileLayers.Length; i++)
        {
            TileLayer tileLayer = baseGameTileLayers[i];
            if(!(tileLayer.GetType().IsSubclassOf(typeof(TileLayer)))) baseGameLayers.Add(i);
        }
    }

    private void GenerateInitialTilemap()
    {
        int tileList;

        for (int y = 0; y < maxRows; y++)
        {
            for (int x = 0; x < maxColumns; x++)
            {
                //Debug.Log(y * 10 + x + 1);
                tileList = (int)noiseMap[x, y] - (int)fallOffMap[x,y];
                if (tileList < 0) tileList = 0;
                else if (tileList == baseGameTileLayers.Length) tileList = baseGameTileLayers.Length - 1;
                selectedTileLayers[x,y] = tileList;
            }
        }
    }

    private float[,] InitializeNoiseMap()
    {
        float[,] initialNoiseMap = new float[maxColumns, maxRows];
        float xOffset = Random.Range(-10000f, 10000f);
        float yOffset = Random.Range(-10000f, 10000f);

        for(int y = 0; y < maxRows; y++)
        {
            for(int x = 0; x < maxColumns; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * PERLIN_SCALE + xOffset, y * PERLIN_SCALE + yOffset);
                initialNoiseMap[x,y] = noiseValue * baseGameTileLayers.Length;
            }
        }

        return initialNoiseMap;
    }

    private float[,] InitializeFallOffMap()
    {
        float[,] initialFallOffMap = new float[maxColumns, maxRows];
        for(int y = 0; y < maxRows; y++)
        {
            for(int x = 0; x < maxColumns; x++)
            {
                float xv = x / (float)maxColumns * 2 - 1;
                float yv = y / (float)maxRows * 2 - 1;
                float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));
                initialFallOffMap[x,y] = Mathf.Pow(v,3f)/(Mathf.Pow(v,3f) + Mathf.Pow(9f - 9f * v, 3f)) * baseGameTileLayers.Length;
            }
        }

        return initialFallOffMap;
    }

    private void AssignBorderCells()
    {
        for(int y =  borderLimit-1; y < maxRows - borderLimit; y++)
        {
            for(int x =  borderLimit-1; x < maxColumns - borderLimit; x++)
            {
                if(selectedTileLayers[x,y] == 0) selectedTileLayers[x,y] = 1;
            }
        }
        
        FillInBorderHoles();
        //FillInBorderHoles();
    }

    private void PlaceTiles()
    {
        for(int i = 0; i < baseGameTileLayers.Length; i++)
        {
            baseGameTileLayers[i].PlaceLayerTiles(maxRows, maxColumns, generatedTiles, selectedTileLayers, i, tileSize, startingPosition, layout);
        }
        
        for(int i = baseGameTileLayers.Length; i < baseGameTileLayers.Length + mapFeatureLayers.Length; i++)
        {
            mapFeatureLayers[i-baseGameTileLayers.Length].PlaceLayerTiles(maxRows, maxColumns, generatedTiles, selectedTileLayers, i, tileSize, startingPosition, layout);
        }
    }

    private void FillInBorderHoles()
    {
        for(int y = 0; y < maxRows; y++)
        {
            for(int x = 0; x < maxColumns; x++)
            {
                if(selectedTileLayers[x, y] != 0 && IsTileInaccessible(x, y, borderLimit))
                {
                    //Debug.Log(x + " " + y);
                    selectedTileLayers[x,y] = 0;
                }
                else if(selectedTileLayers[x, y] == 0 && !CheckForConnectionToBorder(x,y))
                {
                    //.Log(x + " " + y);
                    selectedTileLayers[x,y] = 1;
                }
            }
        }
    }

    private bool IsTileInaccessible(int column, int row, int searchLimit)
    {
        bool right = false;
        bool left = false;
        bool down = false;
        bool up = false;

        for(int i = 0; i < searchLimit; i++)
        {
            if(column + i >= maxColumns || selectedTileLayers[column + i, row] == 0) right = true;
            if(column - i < 0 || selectedTileLayers[column - i, row] == 0) left = true;
            if(row + i >= maxRows || selectedTileLayers[column, row + i] == 0) up = true;
            if(row - i < 0 || selectedTileLayers[column, row - i] == 0) down = true;
        }

        return right && left && down && up;
    }

    private bool CheckForConnectionToBorder(int column, int row)
    {
        if(column < maxColumns/2 && column < row && column != 0) return selectedTileLayers[column-1, row] == 0;
        else if(column > maxColumns/2 && column >= row && column != maxColumns - 1) return selectedTileLayers[column+1, row] == 0;
        else if(row < maxRows/2 && column >= row && row != 0) return selectedTileLayers[column, row-1] == 0;
        else if(row > maxRows/2 && column < row && row != maxRows - 1) return selectedTileLayers[column, row+1] == 0;
        else return true;
    }

    private void PrintCurrentState()
    {
        string print = "";
        for(int x = 0; x < maxColumns; x++)
        {
            for(int y = 0; y < maxRows; y++)
            {
                print += selectedTileLayers[x,y] + " ";
            }
            print += "\n";
        }
        Debug.Log(print);
    }

    private void MarkManuallySelectedTiles()
    {
        for(int i = 0; i < manuallySelectedTiles.Length; i++)
        {
            int x = (int) ((manuallySelectedTiles[i].position.x + .5 * maxColumns));
            int y = (int) (manuallySelectedTiles[i].position.y/tileSize);

            Debug.Log(x + " " + y);

            if(x < maxColumns && x >= 0 && y < maxRows && y >= 0) 
            {
                generatedTiles[x,y] = true;
                selectedTileLayers[x,y] = -4;
            }
        }
    }

    public Vector3 FindNonWaterTile()
    {

        for(int i = maxColumns/2; i<maxColumns; i++)
        {
            for(int j = maxRows/2; j<maxRows; j++)
            {
                if(selectedTileLayers[i,j] >= firstNonWaterTileLayer)
                {
                    //Debug.Log(i + " " +j);
                    return new Vector3((i-maxColumns/2)*tileSize, j*tileSize, 0);
                } 
                else if(selectedTileLayers[maxColumns - i,maxRows - j] >= firstNonWaterTileLayer) 
                {
                    //Debug.Log(i + " " +j);
                    return new Vector3((maxColumns/2-i)*tileSize, (maxRows-j)*tileSize, 0);
                }
            }
        }

        return new Vector3(maxColumns/2*tileSize,maxRows/2*tileSize,0);
    }

    public float[] GetXPositionRange() {return new float[] {(-maxColumns * tileSize/2 + (borderLimit * tileSize) + tileSize), (maxColumns * tileSize/2 - (borderLimit * tileSize) - tileSize)};}

    public float[] GetYPositionRange() {return new float[] {(tileSize * borderLimit + tileSize), (maxRows * tileSize - (borderLimit * tileSize) - tileSize)};}
}