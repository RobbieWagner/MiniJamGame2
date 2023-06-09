using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MapFeature
{
    [SerializeField] private int borderLimit;
    [SerializeField][Range(1,10)] private int numberOfCampfires;

    public override int[,] AddMapFeature(int campfireLayer, int maxColumns, int maxRows, int[,] layers, List<int> layersToConsider, int firstMapFeatureLayer) 
    {
        List<List<int>> possibleCampfirePlacements = new List<List<int>>();
        List<List<int>> campfiresPlaced = new List<List<int>>();

        for(int y = borderLimit; y < maxRows - borderLimit; y++)
        {
            for(int x = borderLimit; x < maxColumns - borderLimit; x++)
            {
                if(layersToConsider.Contains(layers[x,y]) && !HasAdjacentMapFeature(x,y,layers,campfireLayer,firstMapFeatureLayer)) possibleCampfirePlacements.Add(new List<int>(){x,y});
            }
        }

        ShuffleCoordinates(possibleCampfirePlacements);

        int i = 0; 
        while(i < possibleCampfirePlacements.Count && campfiresPlaced.Count < numberOfCampfires)
        {
            List<int> coordinates = possibleCampfirePlacements[i];
            
            AttemptToPlaceCampfire(coordinates, campfiresPlaced, layers, campfireLayer);

            i++;
        }

        return layers;
    }

    //generate campfires in x spots where there are base game layer spots
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

    private void AttemptToPlaceCampfire(List<int> campfireCandidate, List<List<int>> campfiresPlaced, int[,] layers, int campfireLayer)
    {
        bool canPlaceCampfire = true;

        foreach(List<int> campfireCoordinates in campfiresPlaced)
        {
            if(Math.Abs(campfireCandidate[0] - campfireCoordinates[0]) < 10 && Math.Abs(campfireCandidate[1] - campfireCoordinates[1]) < 10) canPlaceCampfire = false;
        }

        if(canPlaceCampfire)
        {
            layers[campfireCandidate[0], campfireCandidate[1]] = campfireLayer;
            campfiresPlaced.Add(campfireCandidate);
        }
    }

}
