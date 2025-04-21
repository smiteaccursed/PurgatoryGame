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
    public string configStructPath;
    public string configHubStructPath;
    private Vector3 lastPlayerPosition;
    public int seed=0;
    private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();
    public TileArray tileManager = new TileArray();
    private static WorldManager instance;
    private StructureManager structures = new StructureManager();
    private StructureManager hubStuct = new StructureManager();
    private int counter = 0;
    public Material material4Tilemap;
    public Transform parentObject;

    public Chunk GetChunkByCoords(Vector2Int coords)
    {
        if (chunks.ContainsKey(coords))
        {
            return chunks[coords];
        }
        else
        {
            return null; 
        }
    }

    public class TileArray // ��� ������ ��������� ����� ������
    {
        public List<TileData> tiles;
        public SpriteData[] SpriteArr= new SpriteData[258];
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
        public bool IsBuf(int mask)
        {
            if (SpriteArr[mask] != null)
            {
                return false;
            }
            else
            {
                return true;
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
        public Tilemap grassTilemap; // �����
        private bool grassCheck = false;
        public GameObject WallPrefab;
        private Vector2Int Position; // ������ �����
        private int chunkSize; // ������ �����
        private TileArray tileManager; // ���� ����� ����������� ������� �� ������ ��� ������������� ��������� ( ���� ��� ������� ���������� ....?)
        private int seed;
        private int[,] wallsMap; // ������� ����� � ������� �� 1 ���� 
        public bool isPreload = false;
        private GameObject chunkObject;
        public SpriteData[] SpriteArr;

        public Chunk(Vector2Int position, Tilemap grassmap, GameObject wall, int chunkSize, TileArray tileArray, int seed, GameObject chunkObject)  
        {
            Position = position;
            grassTilemap = grassmap;
            WallPrefab = wall;
            this.chunkSize = chunkSize;
            tileManager = tileArray;
            this.seed = seed;
            this.chunkObject = chunkObject;
        }

        public void Generates()
        {
            if (!grassCheck) { GenerateGrass(); }
            else { grassTilemap.gameObject.SetActive(true); }
            
            if(wallsMap==null || isPreload) { GenerateWalls(); }    
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
            if (wallsMap == null)
            {
                wallsMap = new int[chunkSize + 2, chunkSize + 2];
            }

            var wallsData = new List<(Vector3 position, int mask)>();
            var createdObject = new List<(Vector3 position, int mask)>();
            
            if(!isPreload)
            {
                await WallGeneration();
                await PostGen();
            }

            await Task.Run(() =>
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    for (int y = 0; y < chunkSize; y++)
                    {
                        if (wallsMap[x + 1, y + 1] > 0)
                        {
                            var position = new Vector3(x + Position.x * chunkSize, y + Position.y * chunkSize, 0);
                            int mask = GetBitMask(new Vector2Int(x, y),wallsMap[x+1,y+1] );
                            wallsData.Add((position, mask));
                        }
                        else if(wallsMap[x+1,y+1] <0)
                        {

                            //Debug.Log("��������� ������������ �������");
                            var position = new Vector3(x + Position.x * chunkSize, y + Position.y * chunkSize, 0);
                            createdObject.Add((position, wallsMap[x + 1, y + 1] * -1));
                            
                        }
                    }
                }
            });

            foreach(var objData in createdObject)
            {
                GameObject wall = Instantiate(WallPrefab, objData.position, Quaternion.identity);
                wall.transform.parent = grassTilemap.transform;


                SpriteRenderer s = wall.GetComponent<SpriteRenderer>();
                SpriteData StrSpr = SpriteArr[objData.mask];
                s.sprite = StrSpr.GetSprite();

                BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
                collider.size = Vector2.one; // ������ ������ ����� (1x1)
                collider.offset = new Vector2(0.5f, 0.5f);
                TileClass wallComponent = wall.GetComponent<TileClass>();
                wallComponent.bit = 0;
                wallComponent.isbuf = false ;
                wall.transform.SetParent(chunkObject.transform.Find("Blocks"));
            }

            //��������� �������� ���������
            foreach (var wallData in wallsData)
            {
                GameObject wall = Instantiate(WallPrefab, wallData.position, Quaternion.identity);
                //wall.transform.parent = grassTilemap.transform;

              
                SpriteRenderer s = wall.GetComponent<SpriteRenderer>();
                s.sprite = tileManager.GetSpriteByMask(wallData.mask);
                
                BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
                TileClass wallComponent = wall.GetComponent<TileClass>();
                if (wallComponent != null)
                {
                    wallComponent.bit = wallData.mask;
                    if(tileManager.IsBuf(wallData.mask))
                    {
                        wallComponent.isbuf = true;
                    }
                    else
                    {
                        wallComponent.isbuf = false;
                    }
                }
                wall.transform.SetParent(chunkObject.transform.Find("Blocks"));
            }
        }
        public int GetBitMask(Vector2Int pos, int baseLayer)
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

                        if ((wallsMap[x + i, y + j] >= baseLayer))
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

            // ��������� ���� ������� �� �����
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (!(i == 0 && j == 0)) // ���������� ����������� ����
                    {
                        Vector2Int neighborPos = new Vector2Int(pos.x + i, pos.y + j);

                        // ���� �������� ���� ����������, ��������� ��� � �����
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
        public void Print2DArray(int[,] array)
        {
            string row = "";
            // ���������� ��� ������ �������
            for (int i = 0; i < array.GetLength(0); i++)
            {
                 // ������ ��� �������� �������� ������� ������

                // ���������� ��� ������� ������� ������
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    row += array[i, j] + " "; // ��������� �������� � ������ � ����������
                }
                row += "\n";
                 // ������� ������
            }
            Debug.Log(row);
        }


        private void UpdateTileSprite(Vector2Int tileCoord)
        {
            int newMask = GetNewBitMask(tileCoord); // �������� ����� ������� �����
            Sprite newSprite = tileManager.GetSpriteByMask(newMask); // ��������� ������
            // Debug.Log("Updating tile");
            // ������� ������ �����
            Collider2D collider = Physics2D.OverlapPoint(new Vector2(tileCoord.x, tileCoord.y));
            if (collider.CompareTag("Tile"))
            {
                var spriteRenderer = collider.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = newSprite; // ��������� ������
                }
            }
        }

        public void UpdateTileAndNeighbors(Vector2Int tileCoord)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (!(i == 0 && j == 0))
                    {
                        Vector2Int neighborPos = new Vector2Int(tileCoord.x + i, tileCoord.y + j);
                        UpdateTileSprite(neighborPos);
                    }
                }
            }
        }
        public async void ChunkPreGen(int[][] structMap, SpriteData[] StructSprites, bool shift)
        {
            //Debug.Log("������ ������� �����");
            //ClearChunk();
            Debug.Log("���� ������, ������ �����������������");
            SpriteArr = StructSprites;
            wallsMap = new int[chunkSize + 2, chunkSize + 2];
            Debug.Log("������ ��������");
            await WallGeneration();
            Debug.Log("�������� ���������");
            int tempShift = 2;
            if (shift)
                tempShift = 1;
            await Task.Run(() =>
            {
                for (int i = 0; i < structMap.Length; i++)
                {
                    for (int j = 0; j < structMap.Length; j++)
                    {
                        if (structMap[i][j] >= 0)
                        {
                            wallsMap[i + tempShift, j + tempShift] = -1 * structMap[i][j];
                        }
                    }
                }
            });
            isPreload = true;
            //Debug.Log("����������������� ������� ���������, �������� ��������� ���������");
            // Debug.Log($"{Position.x}  {Position.y}");
            // Print2DArray(wallsMap);
            await PostGen();
            Generates();

        }

        private async Task WallGeneration()
        {
            await Task.Run(() =>
            {
                System.Random rand = new System.Random(seed); // ������ ������ ��� ��������� ��������� �����

                for (int i = -1; i <= chunkSize; i++)
                {
                    for (int j = -1; j <= chunkSize; j++)
                    {
                        int realX = i + Position.x * chunkSize;
                        int realY = j + Position.y * chunkSize;
                        float buf = (float)ChunkNoiseGenerator.GenerateNoise(
                            1, 1, seed, 20.0f, 4, 0.5f, 2.0f, new Vector2Int(realX, realY))[0, 0];

                        //wallsMap[i + 1, j + 1] = Mathf.RoundToInt(buf);
                        if (buf > 0.7)
                            wallsMap[i + 1, j + 1] = 2;
                        else if (buf > 0.5)
                        {
                            wallsMap[i + 1, j + 1] = 1;
                        }

                        // ���� �������� ������ 0.2, ������ 1 (�����)
                        if (buf < 0.2)
                        {
                            wallsMap[i + 1, j + 1] = 1;
                        }
                        else if (wallsMap[i + 1, j + 1] == 0 && rand.NextDouble() < 0.02)
                        {
                            wallsMap[i + 1, j + 1] = 1;
                        }
                    }
                }
                 
            });
        }

        private async Task PostGen()
        {
            await Task.Run(() =>
            {
                for (int i = 1; i < chunkSize; i++)
                {
                    for (int j = 1; j < chunkSize; j++)
                    {
                        int buf = wallsMap[i, j];

                        if (buf - wallsMap[i, j - 1] > 1)
                           wallsMap[i, j] = buf - 1;
                    }
                }
            });
        }
        public void ClearChunk()
        {
            // ������� �����
            grassTilemap.ClearAllTiles();

            // �������� ���� �������� ����
            //foreach (Transform child in grassTilemap.transform)
            //{
            //    GameObject.Destroy(child.gameObject);
            //}

            // ����� ������� ����� ����
            wallsMap = null;

            grassCheck = false; // ��� ���������� �������� �����
        }

    }

    void Start()
    {
        instance = this;
        lastPlayerPosition = player.transform.position;
        UnityMainThreadDispatcher.Instance();

        LoadJsonData(configFilePath);

        structures.LoadJson(configStructPath);
        structures.seed = seed;
        structures.chunkSize = chunkSize-1;//������ �� �������
        structures.ChunkingAll();

        hubStuct.LoadJson(configHubStructPath);
        hubStuct.chunkSize = chunkSize;
        hubStuct.seed = seed;
        hubStuct.ChunkingAll();


        GenerateChunk(new Vector2Int(0, 0), hubStuct.GetStructureByName("Hub 0 0"));
        GenerateChunk(new Vector2Int(0, -1), hubStuct.GetStructureByName("Hub 0 -1"));
        GenerateChunk(new Vector2Int(-1, -1), hubStuct.GetStructureByName("Hub -1 -1"));
        GenerateChunk(new Vector2Int(-1, 0), hubStuct.GetStructureByName("Hub -1 0"));

        GenerateChunks();
    }

    void Update()
    {
        if (Vector3.Distance(player.transform.position, lastPlayerPosition) >= chunkSize)
        {
            lastPlayerPosition = player.transform.position;
            GenerateChunks();
            UnloadChunks();
        }
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

   
    public Chunk FindChunkAtPosition(Vector2Int position)//������� � ��������-�����������
    {
        // �������� ����� ���� �� �����������
        Vector2Int chunkCoord = new Vector2Int(Mathf.FloorToInt(position.x / chunkSize), Mathf.FloorToInt(position.y / chunkSize));

        if (chunks.ContainsKey(chunkCoord))
        {
            return chunks[chunkCoord];
        }
        else
        {
            return null; // ���� ���� �� ������, ���������� null
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
        bool isStruct = false;
        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(playerChunkCoord.x + x, playerChunkCoord.y + y);
                if (!chunks.ContainsKey(chunkCoord))
                {
                    GameObject chunkObject = new GameObject("Chunk (" + chunkCoord.x + " ; " + chunkCoord.y+" )");
                    chunkObject.transform.SetParent(parentObject);
                    GameObject blocks = new GameObject("Blocks");
                    blocks.transform.SetParent(chunkObject.transform);
                    chunkObject.transform.position = new Vector3(chunkCoord.x * chunkSize, chunkCoord.y * chunkSize, 0);
                    Grid grid = chunkObject.AddComponent<Grid>();
                    Tilemap newGrassTilemap = chunkObject.AddComponent<Tilemap>();
                    TilemapRenderer renderer = chunkObject.AddComponent<TilemapRenderer>();
                    renderer.sortingOrder = -1;
                    Chunk newChunk = new Chunk(chunkCoord, newGrassTilemap, WallPrefab, chunkSize, tileManager, seed, chunkObject);
                    chunks[chunkCoord] = newChunk;
                     if(!isStruct)
                    {
                        var random = new System.Random(seed);
                        if (random.Next(0+counter, 10) == 10) // 10% �����������. ������������
                        {
                            Debug.Log($"Structure spawned at chunk: ({chunkCoord.x}, {chunkCoord.y})");
                            structures.GetRndStruct().SpawnStruct(chunkCoord, false);
                            isStruct = true;
                            counter = 0;
                        }
                        else
                        {
                            counter++;
                        }
                    }
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

    void GenerateChunk(Vector2Int coords, StructureData structure)
    {
        Vector2Int chunkCoord =coords;
        GameObject chunkObject = new GameObject("Chunk (" + chunkCoord.x + " ; " + chunkCoord.y + " )");
        GameObject blocks = new GameObject("Blocks");
        blocks.transform.SetParent(chunkObject.transform);
        chunkObject.transform.SetParent(parentObject);
        chunkObject.transform.position = new Vector3(chunkCoord.x * chunkSize, chunkCoord.y * chunkSize, 0);
        Grid grid = chunkObject.AddComponent<Grid>();
        Tilemap newGrassTilemap = chunkObject.AddComponent<Tilemap>();
        TilemapRenderer renderer = chunkObject.AddComponent<TilemapRenderer>();
        renderer.sortingOrder = -1;
        Chunk newChunk = new Chunk(chunkCoord, newGrassTilemap, WallPrefab, chunkSize, tileManager, seed, chunkObject);
        chunks[chunkCoord] = newChunk;
        structure.SpawnStruct(chunkCoord, true);
        Debug.Log($"{structure.name} - ������ ");
        newChunk.Generates();
        Debug.Log("���������� ���� �� �������");
    }

}
