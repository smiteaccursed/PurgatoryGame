using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;


// НЕ ЗАБЫВАЕМ ЧТО РАЗМЕР СТРУКТУРЫ РАВЕН РАЗМЕР ЧАНКА + ОТСТУП 1 БЛОК С КАЖДОЙ СТОРОНЫ
// У МЕНЯ  16 размер чанка, поэтому размер структуры 14
public class StructureManager
{
    public StructureContainer structure;
    public int chunkSize;
    public int seed;
    public void LoadJson(string path)
    {
        string jsonText = File.ReadAllText(path);
        structure = JsonUtility.FromJson<StructureContainer>(jsonText);
        Debug.Log($"Loaded {structure.structures.Count} struct ");
        if (structure !=null)
        {
            foreach( var s in structure.structures)
            {
                foreach (var dataSheet in s.spriteData.spriteSheets)
                {
                    dataSheet.LoadSpriteSheet();
                }
                
                foreach (var dataSprite in s.spriteData.sprites)
                {
                    dataSprite.LoadSprite(s.spriteData.spriteSheets[0]);
                }
                s.spriteData.PutSpritesInArr();
            }
        }
    }

    public void ChunkingAll() // не нужно оказалось
    {
        Debug.Log("It's chunking time");
        foreach(var s in structure.structures)
        {
            s.GetMaskData();
            s.ChunkingStruct(chunkSize);
            Debug.Log(chunkSize + " Размер чанка");
        }
    }

    public StructureData GetStructureByName(string name)
    {
        foreach(var s in structure.structures)
        {
            if (s.name == name)
                return s;
        }
        return GetRndStruct();
    }


    public StructureData GetRndStruct()
    {
        System.Random random = new System.Random(seed);
        int randomIndex = random.Next(structure.structures.Count); // Генерация случайного индекса
        return structure.structures[randomIndex];
    }
}

[System.Serializable]
public class StructureContainer
{
    public List<StructureData> structures;
}


[System.Serializable]
public class StructureData
{
    public string name;
    public int[][] maskData;
    public int[] rawData;
    public int size;
    public StructSpriteData spriteData;
    
    public List<List<int[][]>> chunkStruct; // Куски структур если не помещаются в чанк
    private int chunkRows;
    private int chunkCols;
    public void ChunkingStruct(int chunkSize)//Дробление структур It's chunking time
    {
        Debug.Log($"Chunking {name}");
        if (maskData == null || maskData.Length == 0 || maskData[0].Length == 0)
        {
            Debug.Log("MaskData initialization: " + (maskData != null ? "Initialized" : "Null"));

            chunkStruct = null;
            return;
        }

        int rows = maskData.Length;         // Количество строк в исходном массиве
        int cols = maskData[0].Length;      // Количество столбцов в исходном массиве
        chunkRows = (rows + chunkSize - 1) / chunkSize; // Количество чанков по строкам
        chunkCols = (cols + chunkSize - 1) / chunkSize; // Количество чанков по столбцам
        // Инициализация массива чанков
        chunkStruct = new List<List<int[][]>>();

        for (int chunkRow = 0; chunkRow < chunkRows; chunkRow++)
        {
            var rowChunks = new List<int[][]>();

            for (int chunkCol = 0; chunkCol < chunkCols; chunkCol++)
            {
                int[][] chunk = new int[chunkSize][];

                for (int i = 0; i < chunkSize; i++)
                {
                    int sourceRow = chunkRow * chunkSize + i;
                    chunk[i] = new int[chunkSize];

                    for (int j = 0; j < chunkSize; j++)
                    {
                        int sourceCol = chunkCol * chunkSize + j;
                        if (sourceRow < rows && sourceCol < cols)
                        {
                            chunk[i][j] = maskData[sourceRow][sourceCol];
                        }
                        else
                        {
                            chunk[i][j] = -1; 
                        }
                    }
                }

                rowChunks.Add(chunk);
            }

            chunkStruct.Add(rowChunks);
        }

        Debug.Log($"{name} {chunkCols}  {chunkRows} chunking result");

    }
    public void SpawnStruct(Vector2Int cords, bool Shift)
    {
        Debug.Log("Готовность к спавну");
        int startX = cords.x;
        int startY = cords.y;
        WorldManager wm = WorldManager.GetInstance();
        for (int i = 0; i < chunkCols; i++) // Индексы чанков по столбцам
        {
            for (int j = 0; j < chunkRows; j++) // Индексы чанков по строкам
            {
                int chunkX = startX + i;
                int chunkY = startY + j;

                Debug.Log("Поиск чанка");
                WorldManager.Chunk chnk = wm.GetChunkByCoords(new Vector2Int(chunkX, chunkY));

                if (chnk != null)
                {
                    Debug.Log($"Чанк найден, запуск генерации");
                    chnk.ChunkPreGen(chunkStruct[i][j], spriteData.SpriteArr, Shift);
                    //PrintChunkStruct(chunkStruct[i][j]);
                    //chnk.ChunkPreGen(maskData, spriteData.SpriteArr);
                }
                else
                {
                    Debug.Log($"Чанк на позиции ({chunkX}, {chunkY}) не найден!");
                }
            }
        }
    }

    void PrintChunkStruct(int[][] chunkStruct)
    {
        string output = "chunkStruct:\n";

        for (int i = 0; i < chunkStruct.Length; i++)
        {
            output += string.Join(", ", chunkStruct[i]) + "\n"; // Преобразуем каждую строку в текст
        }

        Debug.Log(output);
    }

    public void GetMaskData()
    {
        maskData = new int[size + 2][];
        for (int i = 0; i < size + 2; i++)
        {
            maskData[i] = new int[size + 2];
        }

        Debug.Log("rawData initialization: " + (rawData != null ? "Initialized" : "Null") + "size" + (size +2));

        // Заполняем центральную часть массива значениями из rawData
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                maskData[j + 1][size - i] = rawData[i * size + j];
            }
        }

        string output = "";
        for (int i = 0; i < size + 2; i++)
        {
            output += string.Join(", ", maskData[i]) + "\n"; // Добавляем строку массива
        }

        Debug.Log(output);
    }
}

[System.Serializable]
public class StructSpriteData
{
    [System.NonSerialized]
    public SpriteData[] SpriteArr;
    
    public List<SpriteData> sprites;
    public int spriteMaxNum;
    public List<SpriteSheet> spriteSheets;
   
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
            return null;
        }
    }
    public void PutSpritesInArr()
    {
        SpriteArr = new SpriteData[spriteMaxNum];
        if (sprites.Count == 0 || sprites == null)
        {
            Debug.LogError("Sprites count not number ");
        }
        else
        {
            Debug.Log($"{sprites.Count}");
            foreach (var sprite in sprites)
            {
                foreach (var i in sprite.bitMask)
                {
                    SpriteArr[i] = sprite;
                }

            }
        }
    }

     
}


