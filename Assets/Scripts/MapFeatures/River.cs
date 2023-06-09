using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class River : MapFeature
{
    // Start is called before the first frame update
    [SerializeField][Range(3,12)] private int maxXTiles;
    [SerializeField][Range(1,5)] private int maxYTiles;

    public override int[,] AddMapFeature(int riverLayer, int maxColumns, int maxRows, int[,] layers, List<int> layersToConsider, int firstMapFeatureLayer)
    {
        int row = Random.Range(maxRows/3, 2 * maxRows/3);

        layers[0, row] = riverLayer;

        bool rowIsAdded = false;
        int column = 1;
        while(column < maxColumns-1)
        {
            for(int i = (int) Random.Range(3,maxXTiles); i > 0; i--)
            {
                if(column < maxColumns-1) 
                {
                    layers[column, row] = riverLayer;
                    column++;
                }
                
            }
            if(column < maxColumns-1)
            {
                //Debug.Log("hi");
                if(row < maxRows/3) rowIsAdded = true;
                else if(row > 2 * maxRows/3) rowIsAdded = false;
                else rowIsAdded = (int) Random.Range(0, 2) == 1;
                for(int i = (int) Random.Range(1, maxYTiles); i > 0; i--)
                {
                    layers[column, row] = riverLayer;
                    if(rowIsAdded) row++;
                    else row--;
                }
            }
        }

        layers[maxColumns - 1, row] = riverLayer;
        
        return layers;
    }
}
