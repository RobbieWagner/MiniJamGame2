using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapFeature : MonoBehaviour
{
    public virtual int[,] AddMapFeature(int featureLayer, int maxColumns, int maxRows, int[,] layers, List<int> layersToConsider, int firstMapFeatureLayer) {return layers;}

    protected virtual bool HasAdjacentMapFeature(int column, int row, int[,] layers, int layer, int firstMapFeatureLayer)
    {
        List<int> layersToAvoid = new List<int>();
        for(int i = firstMapFeatureLayer; i < layer; i++)
        {
            layersToAvoid.Add(i);
        }

        if(row + 1 < layers.GetLength(1) && layersToAvoid.Contains(layers[column,row+1])) return true;
        if(row > 0 && layersToAvoid.Contains(layers[column,row-1])) return true;
        if(column + 1 < layers.Length && layersToAvoid.Contains(layers[column+1,row])) return true;
        if(column > 0 && layersToAvoid.Contains(layers[column-1,row])) return true;

        return false;
    }

}