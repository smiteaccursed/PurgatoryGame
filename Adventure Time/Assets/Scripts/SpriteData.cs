using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VariantWeight
{
    public string name;
    public double weight;
}
[System.Serializable]
public class SpriteData 
{
    public string tileTag;
    public string path;
    public string name;
    public List<VariantWeight> variantWeights = new List<VariantWeight>();
    [System.NonSerialized]
    public List<Sprite> loadedSprite = new List<Sprite>();

    public void LoadSprite()
    {
        loadedSprite.Clear();
        foreach (var variant in variantWeights)
        {
            Sprite sprite = Resources.Load<Sprite>(path + "/" + variant.name);
            if (sprite != null)
            {
                loadedSprite.Add(sprite);
            }
            else
            {
                Debug.LogWarning($"Sprite not found at path: {path}/{variant.name}");
            }
        }
    }

    public Sprite GetSprite()
    {
        if (loadedSprite.Count == 0)
        {
            Debug.LogError("No loaded sprites available!");
            return null;
        }

        double totalWeight = 0;
        foreach (var variant in variantWeights)
        {
            totalWeight += variant.weight;
        }

        double randomValue = Random.Range(0f, (float)totalWeight);
        double cumulativeWeight = 0;
        for(int i=0; i< variantWeights.Count; i++)
        {
            cumulativeWeight += variantWeights[i].weight;
            if (randomValue < cumulativeWeight)
            {
                var index = loadedSprite[i];
                if (index != null)
                {
                    return index;
                }
                else
                {
                    Debug.LogError($"Sprite not found in loadedSprites for variant: {variantWeights[i].name} + loadedSpriteSize: {loadedSprite.Count}");
                    return loadedSprite[0];
                }
            }
        }

        return loadedSprite[0];
    }
}
