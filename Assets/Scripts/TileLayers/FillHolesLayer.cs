using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillHolesLayer : TileLayer
{
    public virtual void PlaceLayerTiles(int maxRows, int maxColumns, bool[,] generatedTiles, int[,] selectedTileLayers, int layer, float size, Vector3 startingPosition)
        { 
            tileSize = size;
            Vector3 tilePosition;
            for(int y = 0; y < maxRows; y++)
            {
                for(int x = 0; x < maxColumns; x++)
                {
                    if(selectedTileLayers[x,y] == layer || !generatedTiles[x,y])
                    {
                        tilePosition = new Vector3(tileSize * y, tileSize * x, 0) + startingPosition;
                        if(PositionHasNoTile(tilePosition, tileSize)) 
                        {
                            AddTile(tilePosition, tiles[0]);
                        }
                    }
                }
            }
        }
}
