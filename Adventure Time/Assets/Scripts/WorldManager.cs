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
    private static WorldManager instance;

    public Material material4Tilemap;

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
        private int[,] wallsMap; // битовая карта с запасом на 1 блок 

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
            if (grassCheck) return;

            grassCheck = true;

            var positions = new List<Vector3Int>(chunkSize * chunkSize);
            var tiles = new List<Tile>(chunkSize * chunkSize);

            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    positions.Add(new Vector3Int(x, y, 0));
                    tiles.Add(tileManager.GetTilebyName("Grass"));
                }
            }

            TilemapRenderer renGrass = grassTilemap.GetComponent<TilemapRenderer>();
            renGrass.material = WorldManager.GetInstance().material4Tilemap ;
            grassTilemap.SetTiles(positions.ToArray(), tiles.ToArray());
            grassTilemap.CompressBounds();
        }

        private async void GenerateWalls()
        {
            if (wallsMap != null) return;

            wallsMap = new int[chunkSize + 2, chunkSize + 2];
            var wallsData = new List<(Vector3 position, int mask)>();

            
            await Task.Run(() =>
            {
                
                for (int i = -1; i <= chunkSize; i++)
                {
                    for (int j = -1; j <= chunkSize; j++)
                    {
                        int realX = i + Position.x * chunkSize;
                        int realY = j + Position.y * chunkSize;
                        wallsMap[i + 1, j + 1] = Mathf.RoundToInt((float)ChunkNoiseGenerator.GenerateNoise(
                            1, 1, seed, 20.0f, 4, 0.5f, 2.0f, new Vector2Int(realX, realY))[0, 0]);
                    }
                }

                
                for (int x = 0; x < chunkSize; x++)
                {
                    for (int y = 0; y < chunkSize; y++)
                    {
                        if (wallsMap[x + 1, y + 1] == 1)
                        {
                            var position = new Vector3(x + Position.x * chunkSize, y + Position.y * chunkSize, 0);
                            int mask = GetBitMask(new Vector2Int(x, y));
                            wallsData.Add((position, mask));
                        }
                    }
                }
            });

            
            foreach (var wallData in wallsData)
            {
                GameObject wall = Instantiate(WallPrefab, wallData.position, Quaternion.identity);
                wall.transform.parent = grassTilemap.transform;

              
                SpriteRenderer s = wall.GetComponent<SpriteRenderer>();
                s.sprite = tileManager.GetSpriteByMask(wallData.mask);
                
                BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
                TileClass wallComponent = wall.GetComponent<TileClass>();
                if (wallComponent != null)
                {
                    wallComponent.bit = wallData.mask;
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

        public int GetNewBitMask(Vector2Int pos)
        {
            int con = 0;
            int bitmask = 0;

            // Проверяем всех соседей по кругу
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (!(i == 0 && j == 0)) // Пропускаем центральный тайл
                    {
                        Vector2Int neighborPos = new Vector2Int(pos.x + i, pos.y + j);

                        // Если соседний тайл существует, добавляем его к маске
                        if (IsTilePresent(neighborPos))
                        {
                            bitmask += bitMask[con];
                        }

                        con++;
                    }
                }
            }

            return bitmask;
        }

        private bool IsTilePresent(Vector2Int position)
        {
            Collider2D collider = Physics2D.OverlapPoint(new Vector2(position.x, position.y));

            if (collider != null && collider.CompareTag("Tile"))
            {
                return true;
            }

            return false;
        }

        private void UpdateTileSprite(Vector2Int tileCoord)
        {
            int newMask = GetNewBitMask(tileCoord); // Получаем новую битовую маску
            Sprite newSprite = tileManager.GetSpriteByMask(newMask); // Подбираем спрайт
            // Debug.Log("Updating tile");
            // Находим объект тайла
            Collider2D collider = Physics2D.OverlapPoint(new Vector2(tileCoord.x, tileCoord.y));
            if (collider != null && collider.CompareTag("Tile"))
            {
                var spriteRenderer = collider.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = newSprite; // Обновляем спрайт
                }
            }
        }

        public void UpdateTileAndNeighbors(Vector2Int tileCoord)
        {
            // Обновляем текущий тайл

            // Обновляем соседние тайлы
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (!(i == 0 && j == 0)) // Пропускаем центральный тайл
                    {
                        Vector2Int neighborPos = new Vector2Int(tileCoord.x + i, tileCoord.y + j);
                        UpdateTileSprite(neighborPos); // Обновляем спрайт соседа
                    }
                }
            }
        }

    }

    void Start()
    {
        instance = this;
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

    public Chunk FindChunkAtPosition(Vector2Int position)
    {
        // Пытаемся найти чанк по координатам
        Vector2Int chunkCoord = new Vector2Int(Mathf.FloorToInt(position.x / chunkSize), Mathf.FloorToInt(position.y / chunkSize));

        if (chunks.ContainsKey(chunkCoord))
        {
            return chunks[chunkCoord];
        }
        else
        {
            return null; // Если чанк не найден, возвращаем null
        }
    }
    public static WorldManager GetInstance()
    {
        return instance;
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
