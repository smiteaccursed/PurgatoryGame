using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[System.Serializable]
public class TileData
{
    public string tileTag;
    public string path;
    public string name;
    private bool sameWeight=true;
    public List<VariantWeight> variantWeights = new List<VariantWeight>();
    [System.NonSerialized]
    public List<Tile> loadedTiles = new List<Tile>();

    public void LoadTiles()
    {
        loadedTiles.Clear();
        foreach (var variant in variantWeights)
        {
            Tile tile = Resources.Load<Tile>(path + "/" + variant.name);
            if (tile != null)
            {
                loadedTiles.Add(tile);
            }
            else
            {
                Debug.LogWarning($"Tile not found at path: {path}/{variant.name}");
            }
        }
        double firstValue = variantWeights[0].weight;
        for(int i=0; i<variantWeights.Count;i++)
        {
            if(firstValue!=variantWeights[i].weight)
            {
                sameWeight = false;
            }
        }
    }

    public Tile GetTile()
    {
        if (loadedTiles.Count == 0)
        {
            Debug.LogError("No loaded tiles available!");
            return null;
        }
        if(sameWeight==true)
        {
            int rnd = Random.Range(0, loadedTiles.Count);
            return loadedTiles[rnd];
        }
        double totalWeight = 0;
        foreach (var variant in variantWeights)
        {
            totalWeight += variant.weight;
        }

        double randomValue = Random.Range(0f, (float)totalWeight);
        double cumulativeWeight = 0;

        foreach (var variant in variantWeights)
        {
            cumulativeWeight += variant.weight;
            if (randomValue < cumulativeWeight)
            {
                int index = loadedTiles.FindIndex(t => t.name.Equals(variant.name));
                if (index >= 0)
                {
                    return loadedTiles[index];
                }
                else
                {
                    Debug.LogError($"Tile not found in loadedTiles for variant: {variant.name}");
                    return null;
                }
            }
        }

        return loadedTiles[0];
    }
}