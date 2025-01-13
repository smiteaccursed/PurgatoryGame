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
        public SpriteData[] SpriteArr= new SpriteData[257];
        public List<SpriteData> sprites;
        public List<SpriteSheet> spriteSheets;
        public Tile GetTilebyName(string name)
        {
            foreach (var tile in tiles)
            {
                if (tile.name == name)
                    return tile.GetTile();
            }
            return null;
        }
        public Sprite GetSpritebyName(string name)
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
        public Sprite GetSpriteByMask(int mask)
        {
            if (SpriteArr[mask] != null)
            {
                return SpriteArr[mask].GetSprite();
            }
            else
            {
                //Debug.LogWarning($"No sprite found for mask: {mask}");
                return SpriteArr[256].GetSprite(); 
            }
        }
        public void PutSpritesInDict()
        {
            if(sprites.Count<=0 || sprites==null)
            {
                Debug.LogError("Sprites < 0");
                
            }
            else
            {
                Debug.Log($"{sprites.Count}");
                foreach (var sprite in sprites)
                {
                    foreach(var i in sprite.bitMask)
                    {
                        SpriteArr[i] = sprite;
                    }
                     
                }
            }
        }
    }

    public class Chunk
    {
        private int[] bitMask = new int[] { 32,8,1,64,2,128,16,4 };
        public Tilemap grassTilemap; // ковер
        private bool grassCheck = false;
        public GameObject WallPrefab;
        private Vector2Int Position; // коорды чанка
        private int chunkSize; // размер чанка
        private TileArray tileManager;
        private int seed;
        private int[,] wallsMap; // битова€ карта с запасом на 1 блок 

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
        }

        public void GenerateGrass()
        {
            grassCheck = true;
            //Debug.Log("делаем траву - мураву ");
            for (int x = 0; x < chunkSize; x++)  
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    grassTilemap.SetTile(new Vector3Int(x, y, 0), tileManager.GetTilebyName("Grass"));
                }
            }
            grassTilemap.CompressBounds();
        }
        private void GenerateWalls()
        {
            wallsMap = new int[chunkSize + 2, chunkSize + 2];

            //Debug.LogWarning($"{noiseArray[0,0]} {noiseArray[1,0]} {noiseArray[0,1]}") ;
            for(int i=-1; i <= chunkSize; i++)
            {
                for(int j=-1; j <= chunkSize; j++)
                {
                    int realX = i + Position.x * chunkSize;
                    int realY = j + Position.y * chunkSize;
                    wallsMap[i+1, j+1] = Mathf.RoundToInt((float)ChunkNoiseGenerator.GenerateNoise(1, 1, seed, 20.0f, 4, 0.5f, 2.0f, new Vector2Int(realX, realY))[0, 0]);
                    
                }
            }


            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    int realX = x  + Position.x * chunkSize;
                    int realY = y  + Position.y * chunkSize;
                     
                    //double noiseResult = Mathf.PerlinNoise(realX * 0.1f, realY * 0.1f);
                    if (wallsMap[x + 1, y +1] == 1)
                    {
                        CreateWall(new Vector2Int(realX, realY), x, y);
                    }
                }
            }
        }
        
        public int GetBitMask(Vector2Int pos)
        {
            //Debug.Log($"info about {pos.x} {pos.y}");
            int x = pos.x+1;
            int y = pos.y+1;
            int con = 0;
            int bitmask = 0;
            for(int i=-1; i<=1;i++)
            {
                for(int j=-1; j<=1; j++)
                {
                    if (!(i == 0 && j == 0))
                    {
                        
                        if ((wallsMap[x + i, y + j] == 1))
                        {
                            //Debug.Log($"Block {x + i - 1} {y + j -1}  {bitMask[con]}");
                            bitmask += bitMask[con];
                        }
                        con++;
                    }
                }
            }
            //Debug.Log($"{x} {y}");
            //Debug.Log($"Bitmask {bitmask} and con {con} for {x-1} {y-1}" );
            return bitmask;
        }
        private void CreateWall(Vector2Int position, int x, int y)
        {
            //Debug.Log($"Local coord {x} {y}");
            //Debug.Log($"Global coord {position.x} {position.y}");
            GameObject wall = Instantiate(WallPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
            SpriteRenderer s = wall.GetComponent<SpriteRenderer>();
            int mask = GetBitMask(new Vector2Int(x,y));
            s.sprite = tileManager.GetSpriteByMask(mask);
            wall.transform.parent = grassTilemap.transform;

            wall.AddComponent<BoxCollider2D>();
            TileClass wallComponent = wall.GetComponent<TileClass>();
            if (wallComponent != null)
            {
                wallComponent.health = 228; // ѕример значени€ здоровь€
                wallComponent.bit = mask;  // ”становка битовой маски
            }
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
            Debug.Log("Loaded " + tileManager.spriteSheets.Count + " sprite sheet JSON");
            foreach(var dataSheet in tileManager.spriteSheets)
            {
                dataSheet.LoadSpriteSheet();
            }
            foreach (var dataTile in tileManager.tiles)
            {
                dataTile.LoadTiles();
            }
            foreach(var dataSprite in tileManager.sprites)
            {
                dataSprite.LoadSprite(tileManager.spriteSheets[0]);
            }
            tileManager.PutSpritesInDict();
        }
        else
        {
            //Debug.LogError("Failed to load tiles/sprites from JSON.");
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

    public void Debug2DArray(int[,] array)
    {
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);
        string output = "";

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                output += array[i, j] + " ";
            }
            output += "\n"; // ѕереход на новую строку дл€ каждого р€да
        }

        //Debug.Log($"2D Array:\n{output}"); // ¬ыводим массив в удобном виде
    }
}
