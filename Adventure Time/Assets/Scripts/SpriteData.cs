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
    public int bitMask;
    public int bitCount;
    [System.NonSerialized]
    public List<Sprite> loadedSprite = new List<Sprite>();

    public void LoadSprite(SpriteSheet sprites)
    {
        loadedSprite.Clear();
        foreach (var variant in variantWeights)
        {
            Sprite sprite = sprites.GetSpriteFromSheet(variant.name);
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
        if(loadedSprite.Count==1)
        {
            return loadedSprite[0];
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
[System.Serializable]
public class SpriteSheet
{
    public string path;
    public string name;
    public Sprite[] sprites;
    public Sprite GetSpriteFromSheet(string name)
    {
        foreach(var i in sprites)
        {
            if(name ==i.name)
            {
                return i;
            }
        }
        return null;
    }
    public void LoadSpriteSheet()
    {
        sprites = Resources.LoadAll<Sprite>(path);
    }
}
