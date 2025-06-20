using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.IO;

public class WorldGeneration : MonoBehaviour
{
    public int chunkSize = 16;
    public int chunksX = 3;
    public int chunksY = 3;
    public Tilemap tilemap;
    public string configFilePath;
    public TileArray tileData;
    

    [System.Serializable]
    public class VariantWeight
    {
        public string name; 
        public double weight;
    }
    [System.Serializable]
    public class Neighbors
    {
        public string NSWE;
        public List<string> tiles;
    }
    [System.Serializable]
    public class TileArray
    {
        public List<DataTile> tileArray;
        public List<string> variants = new List<string>();
        public void AllTilesName()
        {
            foreach( var i in tileArray)
            {
                variants.Add(i.name);
            }
        }
        public DataTile GetTileByName(string name)
        {
            foreach(var i in tileArray)
            {
                if (i.name == name)
                    return i;
            }
            return null;
        }
        public DataTile GetTileByTile(Tile tile)
        {
            foreach (var i in tileArray)
            {
                foreach (var j in i.loadedTiles)
                    if (j == tile)
                        return i;
            }
            return null;
        }

        public Tile GetTileFromVariants(List<DataTile> arr)
        {
            if(arr.Count==0)
            {
                Debug.LogError("СОСЕДИ ПУСТЫЕ ЧТО ТАКОЕ !!!!!!!!!!");
                return null;
            }
            Tile result = arr[0].GetTile();
            double totalWeight = 0;
            foreach (var variant in arr)
            {
                totalWeight += variant.weight;
            }

            double randomValue = Random.Range(0f, (float)totalWeight);
            double cumulativeWeight = 0;
            foreach (var variant in arr)
            {
                cumulativeWeight += variant.weight;
                if (randomValue < cumulativeWeight)
                {
                    return variant.GetTile();
                }
            }
            return result;
        }
    }
    [System.Serializable]
    public class DataTile
    {
        public string path;
        public string name;
        public double weight;
        public List<VariantWeight> variantWeights=new List<VariantWeight>();
        public List<Neighbors> neighbors = new List<Neighbors>();
        [System.NonSerialized]
        public List<Tile> loadedTiles = new List<Tile>();

        public void LoadTiles()
        {
            loadedTiles.Clear(); 
            //Debug.Log($"Tile {name} has {neighbors.Count} neighbors.");
            //foreach (var neighbor in neighbors)
            //{
            //    Debug.Log($"Neighbor NSWE: {neighbor.NSWE}, Tiles: {string.Join(", ", neighbor.tiles)}");
            //}
            foreach (var variant in variantWeights)
            {
                Tile tile = Resources.Load<Tile>(path + "/" + variant.name);
                if (tile != null)
                {
                    loadedTiles.Add(tile);
                    //Debug.Log($"Loaded tile: {variant.name}");
                }
                else
                {
                    Debug.LogWarning($"Tile not found at path: {path}/{variant.name}");
                }
            }
        }
        public List<string> GetVariantByDim(string dimention)
        {
            //Debug.Log("Запуск поиска вариантов");
            //Debug.Log(neighbors.Count);
            List<string> allVar = new List<string>();
            foreach(var i in neighbors)
            {
                //Debug.Log(i.NSWE+" "+ i.tiles.Count);
                if (dimention == i.NSWE)
                    allVar=(i.tiles);
            }
            return allVar;
        }
        public Tile GetTile()
        {
            if (loadedTiles.Count == 0)
            {
                Debug.LogError("No loaded tiles available!");
                return null; 
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

    public class TileWithEntropy : System.IComparable<TileWithEntropy>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Entropy { get; set; }

        public TileWithEntropy(int x, int y, int entropy)
        {
            X = x;
            Y = y;
            Entropy = entropy;
        }
        public int CompareTo(TileWithEntropy other)
        {
            if (this.Entropy != other.Entropy)
            {
                return Entropy.CompareTo(other.Entropy); 
            }
            else
            {
                if (this.X != other.X)
                    return X.CompareTo(other.X);
                else
                    return Y.CompareTo(other.Y);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is TileWithEntropy other && X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            // Используем XOR для комбинирования хеш-кодов X и Y
            return (X * 397) ^ Y;
        }
    }
 
    private void Start()
    {
        LoadJsonData(configFilePath);
        GenerateWorld();
    }
 
    public void LoadJsonData(string path)
    {
        string jsonText = File.ReadAllText(path);
        Debug.Log(jsonText);
        tileData = JsonUtility.FromJson<TileArray>(jsonText);

        if (tileData != null && tileData.tileArray != null)
        {
            Debug.Log("Loaded " + tileData.tileArray.Count + " tiles from JSON.");
        }
        else
        {
            Debug.LogError("Failed to load tiles from JSON.");
        }
        foreach (var dataTile in tileData.tileArray)
        {
            dataTile.LoadTiles();

        }
        tileData.AllTilesName();
    }
  
    public void GenerateWorld()
    {

        for (int cX=0; cX<chunksX; cX++)
        {
            for(int cY=0; cY<chunksY; cY++)
            {
                GenerateChunk(cX, cY);
            }
        }
        tilemap.CompressBounds();
    }

    public void GenerateChunk(int X, int Y)
    {
        int startX = X * chunkSize;
        int startY = Y * chunkSize;
        var debugMessages = new List<string>();
        var debugError = new List<string>();
        int entropy;
        SortedSet<TileWithEntropy> entropyQueueu = new SortedSet<TileWithEntropy>();
        for(int x =0; x<chunkSize; x++)
        {
            for(int y=0; y<chunkSize; y++)
            {
                if (tilemap.GetTile(new Vector3Int(startX + x, startY + y, 0))!=null)
                    continue;
                entropy = GetVariants(startX + x, startY + y).Count;
                entropyQueueu.Add(new TileWithEntropy(x, y, entropy));
                //Debug.Log($"Добавляю в множество клетку с кордами {x} {y} {entropyQueueu.Count}");
            }
        }
        Debug.Log($"Заполнена карта энтропии размером {entropyQueueu.Count} ");
        List<string> mapFil = new List<string>();
        while(entropyQueueu.Count>0)
        {
            TileWithEntropy currentTile = entropyQueueu.Min;
            mapFil.Add($"{currentTile.X}  {currentTile.Y}  {currentTile.Entropy} корды + энтропия");
            if(currentTile.Entropy==0)
            {
                Debug.LogError($"Энтропия ноль !!! Схлопование не произошло на координатах {currentTile.X} {currentTile.Y}");
                //for(int dx=currentTile.X-1; dx<=currentTile.X+1; dx++)
                //{
                //    for(int dy=currentTile.Y-1; dy<=currentTile.Y+1; dy++)
                //    {
                //        tilemap.SetTile(new Vector3Int(dx, dy, 0), null);
                //        entropyQueueu.Add(new TileWithEntropy(dx, dy, GetVariants(dx, dy).Count));
                //    }
                //}
            }
            entropyQueueu.Remove(currentTile);
            int x = currentTile.X + startX;
            int y = currentTile.Y + startY;

            List<DataTile> neighbors = GetVariants(x, y);

            if(neighbors.Count>0)
            {
                Tile selectedTile = tileData.GetTileFromVariants(neighbors);
                tilemap.SetTile(new Vector3Int(x, y, 0), selectedTile);
                UpdateEntropyForNeighbors(entropyQueueu, currentTile.X, currentTile.Y, startX, startY);
                debugMessages.Add($"Для плитки с координатами {x} {y} ");
            }
            else
            {
                debugError.Add("Соседей 0 КОЛЛАПС!!!!!!");
            }
        }
        Debug.Log(string.Join("\n", debugMessages));
        Debug.Log(string.Join("\n", mapFil));
        //Debug.LogError(string.Join("\n", debugError));

    }
    public List<DataTile> GetVariants(int x, int y)
    {
        //Debug.Log("Я вызываю функцию выдачи всех вариантов");
        List<DataTile> neighbors = new List<DataTile>();
        Tile n = (Tile)tilemap.GetTile(new Vector3Int(x, y + 1, 0));
        //Debug.Log("Взял север");
        Tile s = (Tile)tilemap.GetTile(new Vector3Int(x, y - 1, 0));
        //Debug.Log("Взял юг");
        Tile w = (Tile)tilemap.GetTile(new Vector3Int(x - 1, y, 0));
        //Debug.Log("Взял запад");
        Tile e = (Tile)tilemap.GetTile(new Vector3Int(x + 1, y, 0));
        //Debug.Log("Взял восток");
        Tile nw = (Tile)tilemap.GetTile(new Vector3Int(x-1,y+1,0));
        Tile ne = (Tile)tilemap.GetTile(new Vector3Int(x+1, y+1,0));
        Tile sw = (Tile)tilemap.GetTile(new Vector3Int(x-1, y-1,0));
        Tile se = (Tile)tilemap.GetTile(new Vector3Int(x+1, y-1,0));
        List<string> buff;
        List<string> buffName = new List<string>(tileData.variants);
        //Debug.Log("Число элементов общего массива тайл даты "+tileData.tileArray.Count);
        //Debug.Log("Число общего числа вариантов тайлов" + buffName.Count);
        //Debug.Log($"Плитка {x}  {y} " +string.Join(" ", buffName));
        if (n != null)
        {
            buff = tileData.GetTileByTile(n).GetVariantByDim("S");
            buffName.RemoveAll(item => !buff.Contains(item));
            //Debug.Log($"Северная плитка {x}  {y} " + string.Join(" ", buff));

        }
        if (s != null)
        {
            buff = tileData.GetTileByTile(s).GetVariantByDim("N");
            buffName.RemoveAll(item => !buff.Contains(item));
            //Debug.Log($"Южная плитка {x}  {y} " + string.Join(" ", buff));
        }
        if (w != null)
        {
            buff = tileData.GetTileByTile(w).GetVariantByDim("E");
            buffName.RemoveAll(item => !buff.Contains(item));
            //Debug.Log($"Западная плитка {x}  {y} " + string.Join(" ", buff));

        }
        if (e != null)
        {
            buff = tileData.GetTileByTile(e).GetVariantByDim("W");
            buffName.RemoveAll(item => !buff.Contains(item));
            //Debug.Log($"Востночная плитка {x}  {y} " + string.Join(" ", buff));

        }
        if(nw!=null)
        {
            buff = tileData.GetTileByTile(nw).GetVariantByDim("SE");
            buffName.RemoveAll(item => !buff.Contains(item));
        }
        if (ne != null)
        {
            buff = tileData.GetTileByTile(ne).GetVariantByDim("SW");
            buffName.RemoveAll(item => !buff.Contains(item));
        }
        if (sw != null)
        {
            buff = tileData.GetTileByTile(sw).GetVariantByDim("NE");
            buffName.RemoveAll(item => !buff.Contains(item));
        }
        if (se != null)
        {
            buff = tileData.GetTileByTile(se).GetVariantByDim("NW");
            buffName.RemoveAll(item => !buff.Contains(item));
        }
        //Debug.Log($"Итого для  {x}  {y} " + string.Join(" ", buffName));

        foreach (var name in buffName)
        {
            neighbors.Add(tileData.GetTileByName(name));
        }
        return neighbors;
    }

    private void UpdateEntropyForNeighbors(SortedSet<TileWithEntropy> entropyQueue, int currentX, int currentY, int startX, int startY)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int neighborX = currentX + dx;
                int neighborY = currentY + dy;

                if (neighborX >= 0 && neighborX < chunkSize && neighborY >= 0 && neighborY < chunkSize)
                {
                    TileWithEntropy neighborTile = FindTileByCoord(entropyQueue, neighborX, neighborY);
                    if (neighborTile != null)
                    {
                        entropyQueue.Remove(neighborTile); 

                        int newEntropy = GetVariants(neighborX + startX, neighborY + startY).Count;
                        neighborTile.Entropy = newEntropy;

                        entropyQueue.Add(neighborTile); 
                    }
                }
            }
        }
    }
    private TileWithEntropy FindTileByCoord(SortedSet<TileWithEntropy> entQue, int x, int y)
    {
        foreach(var tile in entQue)
        {
            if(tile.X==x&&tile.Y==y)
            {
                return tile;
            }
        }
        return null;
    }
}
