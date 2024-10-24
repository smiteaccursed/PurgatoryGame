using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldManager : MonoBehaviour
{
    public GameObject player;
    public int chunkSize = 16;
    public int viewDistance = 2;
    public GameObject WallPrefab;
    public string configFilePath;
    private Vector3 lastPlayerPosition;
    public int seed=0;
    private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();
    public TileArray tileManager = new TileArray();

    public class TileArray
    {
        public List<TileData> tiles;
        public List<SpriteData> sprites;
        public Tile GetTilebyName(string name)
        {
            foreach (var tile in tiles)
            {
                if (tile.name == name)
                    return tile.GetTile();
            }
            return null;
        }
        public Sprite GetSpitebyName(string name)
        {
            foreach (var sprite in sprites)
            {
                if (sprite.name == name)
                {
                    return sprite.GetSprite();
                }
            }
            return null;
        }
    }

    public class Chunk
    {
        public Tilemap grassTilemap;
        private bool grassCheck = false;
        public GameObject WallPrefab;
        private Vector2Int Position;
        private int chunkSize;
        private TileArray tileManager;
        private int seed;
        private int[,] wallsMap;
        private ChunkNoiseGenerator generator;
        public Chunk(Vector2Int position, Tilemap grassmap, GameObject wall, int chunkSize, TileArray tileArray, int seed)  
        {
            Position = position;
            grassTilemap = grassmap;
            WallPrefab = wall;
            this.chunkSize = chunkSize;
            tileManager = tileArray;
            this.seed = seed;
        }

        public void Generates()
        {
            if(!grassCheck) { GenerateGrass(); }
            else { grassTilemap.gameObject.SetActive(true); }
            if(wallsMap==null) { GenerateWalls(); }
            else { REGenerateWalls(); }
             
             
        }

        public void GenerateGrass()
        {
            grassCheck = true;
            Debug.Log("делаем траву - мураву ");
            for (int x = 0; x < chunkSize; x++)  
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    grassTilemap.SetTile(new Vector3Int(x, y, 0), tileManager.GetTilebyName("Grass"));
                }
            }
            grassTilemap.CompressBounds();
        }

        private async void GenerateWalls()
        {
            wallsMap = new int[chunkSize + 2, chunkSize + 2];
            List<string> ChunkInfo = new List<string>();
            //Debug.LogWarning($"{noiseArray[0,0]} {noiseArray[1,0]} {noiseArray[0,1]}") ;
            await Task.Run(() =>
            {
                for(int i=0; i<chunkSize+2; i++)
                {
                    for(int j=0; j<chunkSize+2; j++)
                    {
                        int realX = i - 1 + Position.x * chunkSize;
                        int realY = j - 1  + Position.y * chunkSize;
                        wallsMap[i,j]= Mathf.RoundToInt((float)ChunkNoiseGenerator.GenerateNoise(1, 1, seed, 20.0f, 4, 0.5f, 2.0f, new Vector2Int(realX, realY))[0,0]);
                    }
                }
                for (int x = 0; x < chunkSize; x++)
                { 
                    for (int y = 0; y < chunkSize; y++)
                    {
                        int realX = x + 1 + Position.x * chunkSize;
                        int realY = y + 1 + Position.y * chunkSize;
                        //double noiseResult = Mathf.PerlinNoise(realX * 0.1f, realY * 0.1f);
                        ChunkInfo.Add((wallsMap[x + 1, y + 1] + "  " + realX + "  "+ realY));
                        if (wallsMap[x + 1,y+1] ==1)
                        {
                            UnityMainThreadDispatcher.Instance().Enqueue(() =>
                            {
                                CreateWall(new Vector2Int(realX, realY));
                            });
                        }
                    }
                }
            });
        }
        private async void REGenerateWalls()
        {
            List<string> ChunkInfo = new List<string>();
            //Debug.LogWarning($"{noiseArray[0,0]} {noiseArray[1,0]} {noiseArray[0,1]}") ;
            await Task.Run(() =>
            {
               

                for (int x = 0; x < chunkSize; x++)
                {
                    for (int y = 0; y < chunkSize; y++)
                    {
                        int realX = x + Position.x * chunkSize;
                        int realY = y + Position.y * chunkSize;

                        if (wallsMap[x + 1, y + 1] ==1)
                        {
                            UnityMainThreadDispatcher.Instance().Enqueue(() =>
                            {
                                CreateWall(new Vector2Int(realX, realY));
                            });
                        }
                    }
                }
            });
        }

        private void CreateWall(Vector2Int position)
        {
            GameObject wall = Instantiate(WallPrefab, new Vector3(position.x+0.5f, position.y+0.5f, 0), Quaternion.identity);
            SpriteRenderer s = wall.GetComponent<SpriteRenderer>();
            s.sprite = tileManager.sprites[0].GetSprite();
            wall.transform.parent = grassTilemap.transform;
            wall.AddComponent<BoxCollider2D>();
        }
    }

    void Start()
    {
        lastPlayerPosition = player.transform.position;
        UnityMainThreadDispatcher.Instance();
        LoadJsonData(configFilePath);
        GenerateChunks();
    }

    public void LoadJsonData(string path)
    {
        string jsonText = File.ReadAllText(path);
        //Debug.Log(jsonText);
        tileManager = JsonUtility.FromJson<TileArray>(jsonText);

        if (tileManager != null && tileManager.tiles != null && tileManager.sprites!=null)
        {
            Debug.Log("Loaded " + tileManager.tiles.Count + " tiles from JSON.");
            Debug.Log("Loaded " + tileManager.sprites.Count + " sprites from JSON.");
            foreach (var dataTile in tileManager.tiles)
            {
                dataTile.LoadTiles();
            }
            foreach(var dataSprite in tileManager.sprites)
            {
                dataSprite.LoadSprite();
            }
        }
        else
        {
            Debug.LogError("Failed to load tiles from JSON.");
        }
    }

    void Update()
    {
        if (Vector3.Distance(player.transform.position, lastPlayerPosition) >= chunkSize)
        {
            lastPlayerPosition = player.transform.position;
            GenerateChunks();
            UnloadChunks( );
        }
    }

    void GenerateChunks()
    {
        Vector2Int playerChunkCoord = new Vector2Int(
            Mathf.FloorToInt(player.transform.position.x / chunkSize),
            Mathf.FloorToInt(player.transform.position.y / chunkSize)
        );

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(playerChunkCoord.x + x, playerChunkCoord.y + y);
                if (!chunks.ContainsKey(chunkCoord))
                {
                    GameObject chunkObject = new GameObject("Chunk (" + chunkCoord.x + " ; " + chunkCoord.y+" )");
                    chunkObject.transform.position = new Vector3(chunkCoord.x * chunkSize, chunkCoord.y * chunkSize, 0);
                    Grid grid = chunkObject.AddComponent<Grid>();
                    Tilemap newGrassTilemap = chunkObject.AddComponent<Tilemap>();
                    TilemapRenderer renderer = chunkObject.AddComponent<TilemapRenderer>();
                    renderer.sortingOrder = -1;
                    Chunk newChunk = new Chunk(chunkCoord, newGrassTilemap, WallPrefab, chunkSize, tileManager, seed);
                    chunks[chunkCoord] = newChunk;
                    newChunk.Generates();
                }
                else
                {
                    chunks[chunkCoord].grassTilemap.gameObject.SetActive(true);
                }
            }
        }
    }


    void UnloadChunks()
    {
        Vector2Int playerChunkCoord = new Vector2Int(
            Mathf.FloorToInt(player.transform.position.x / chunkSize),
            Mathf.FloorToInt(player.transform.position.y / chunkSize)
        );
        List<Vector2Int> chunksToUnload = new List<Vector2Int>();

        foreach (var chunk in chunks)
        {
            if (Mathf.Abs(chunk.Key.x - playerChunkCoord.x) > viewDistance || Mathf.Abs(chunk.Key.y - playerChunkCoord.y) > viewDistance)
            {
                chunksToUnload.Add(chunk.Key);
            }
            else
            {
                if (!chunk.Value.grassTilemap.gameObject.activeSelf)
                {
                    chunk.Value.grassTilemap.gameObject.SetActive(true);
                }
            }
        }

        foreach (var chunkCoord in chunksToUnload)
        {
            if (chunks.TryGetValue(chunkCoord, out Chunk chunk))
            {
                chunk.grassTilemap.gameObject.SetActive(false);  
            }
        }
    }
}
