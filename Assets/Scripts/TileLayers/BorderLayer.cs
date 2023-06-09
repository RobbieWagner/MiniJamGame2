using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderLayer : TileLayer
{

    enum ConnectedSides
    {
        north,
        east,
        south,
        west,
        northEast,
        northSouth,
        northWest,
        eastSouth,
        eastWest,
        southWest,
        northEastSouth,
        northEastWest,
        northSouthWest,
        eastSouthWest,
        northEastSouthWest
    }

    public override void PlaceLayerTiles(int maxRows, int maxColumns, bool[,] generatedTiles, int[,] selectedTileLayers, int layer, float size, Vector3 startingPosition, Transform parentTile)
    { 
        PlaceOuterBorder(maxRows, maxColumns, tiles[11], startingPosition, parentTile);
        tileSize = size;
        Vector3 tilePosition;
        for(int y = 0; y < maxRows; y++)
        {
            for(int x = maxColumns - 1; x >= 0; x--)
            {
                if(selectedTileLayers[x,y] == layer)
                {
                    tilePosition = new Vector3(tileSize * x, tileSize * y, 0) + startingPosition;

                    int tile = FindTileToUse(x, y, maxColumns, maxRows, selectedTileLayers, layer);

                    AddTile(tilePosition, tiles[tile], parentTile);
                }
            }
        }

        //PrintCurrentState(maxColumns,maxRows, selectedTileLayers);
    }

    private int FindTileToUse(int x, int y, int maxColumns, int maxRows, int[,] layers, int layer)
    {
        bool[] adjacentBorderTiles = CheckForAdjacentTiles(x, y, maxColumns, maxRows, layers, layer);
        bool north = adjacentBorderTiles[0];
        bool east = adjacentBorderTiles[1];
        bool south = adjacentBorderTiles[2];
        bool west = adjacentBorderTiles[3];

        if(north && east && south && west) return (int) ConnectedSides.northEastSouthWest;
        if(east && south && west) return (int) ConnectedSides.eastSouthWest;
        else if(north && south && west) return (int) ConnectedSides.northSouthWest;
        else if(north && east && west) return (int) ConnectedSides.northEastWest;
        else if(north && east && south) return (int) ConnectedSides.northEastSouth;
        else if(south && west) return (int) ConnectedSides.southWest;
        else if(east && west) return (int) ConnectedSides.eastWest;
        else if(east && south) return (int) ConnectedSides.eastSouth;
        else if(north && west) return (int) ConnectedSides.northWest;
        else if(north && south) return (int) ConnectedSides.northSouth;
        else if(north && east) return (int) ConnectedSides.northEast;
        else if(west) return (int) ConnectedSides.west;
        else if(south) return (int) ConnectedSides.south;
        else if(east) return (int) ConnectedSides.east;
        else if(north) return (int) ConnectedSides.north;
        else return 0; 
    }

    private void PlaceOuterBorder(int maxRows, int maxColumns, GameObject tile, Vector3 startingPosition, Transform parentTile)
    {
        for(int i = 0; i < maxRows; i++) 
        {
            AddTile(-1, i, tile, startingPosition, parentTile);
            AddTile(maxColumns, i,  tile, startingPosition, parentTile);
        }
    }
}
