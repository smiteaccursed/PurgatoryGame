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
public class SpriteData // Õ¿ ¡”ƒ”Ÿ≈≈ ›“Œ  À¿—— ƒÀﬂ ŒƒÕŒ√Œ11111111 ÒÔ‡ÈÚ‡
{
    public string name;
    public List<VariantWeight> variantWeights = new List<VariantWeight>();
    public List<int> bitMask = new List<int>();
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
                //Debug.Log($"Sprite was loaded");
                loadedSprite.Add(sprite);
            }
            else
            {
                Debug.LogWarning($"Sprite not found at path: {variant.name}");
            }
        }

    }

    //public Sprite GetSprite()
    //{
    //    if (loadedSprite.Count == 0)
    //    {
    //        Debug.LogError("No loaded sprites available!");
    //        return null;
    //    }
    //    if(loadedSprite.Count==1)
    //    {
    //        return loadedSprite[0];
    //    }
    //    double totalWeight = 0;
    //    foreach (var variant in variantWeights)
    //    {
    //        totalWeight += variant.weight;
    //    }

    //    double randomValue = Random.Range(0f, (float)totalWeight);
    //    double cumulativeWeight = 0;
    //    for(int i=0; i< variantWeights.Count; i++)
    //    {
    //        cumulativeWeight += variantWeights[i].weight;
    //        if (randomValue < cumulativeWeight)
    //        {
    //            var index = loadedSprite[i];
    //            if (index != null)
    //            {
    //                return index;
    //            }
    //            else
    //            {
    //                Debug.LogError($"Sprite not found in loadedSprites for variant: {variantWeights[i].name} + loadedSpriteSize: {loadedSprite.Count}");
    //                return loadedSprite[0];
    //            }
    //        }
    //    }

    //    return loadedSprite[0];
    //}
    public Sprite GetSprite()
    {
        if (loadedSprite.Count == 0)
            return null;

        double randomValue = Random.value;
        int index = Mathf.FloorToInt((float)(randomValue * loadedSprite.Count));

        return loadedSprite[index];
    }
}
[System.Serializable]
public class SpriteSheet
{
    public List<string> paths = new List<string>();
    public string name;
    public List<Sprite> sprites = new List<Sprite>();
    public Sprite GetSpriteFromSheet(string name)
    {
        foreach(var i in sprites)
        {
            if(name ==i.name)
            {
                return i;
            }
        }
        Debug.LogWarning($"Sprite with name '{name}' not found.");
        return null;
    }
    public void LoadSpriteSheet()
    {
        foreach (var path in paths)
        {
            Sprite[] loadedSprites = Resources.LoadAll<Sprite>(path);
            if (loadedSprites.Length > 0)
            {
                sprites.AddRange(loadedSprites);
            }
            else
            {
                Debug.LogWarning($"No sprites found at path: {path}");
            }
        }
    }
}
