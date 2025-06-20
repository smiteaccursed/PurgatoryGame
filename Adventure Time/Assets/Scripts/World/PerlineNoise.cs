using UnityEngine;

public class ChunkNoiseGenerator
{
    public static double[,] GenerateNoise(int x, int y, int seed, float scale, int octaves, float persostence, float lacunary, Vector2Int offset)
    {
        double[,] noise = new double[x, y];
        System.Random rand = new System.Random(seed);

        Vector2[] octavesOffset = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float xOffset = rand.Next(-100000, 100000) + offset.x * (x / scale);
            float yOffset = rand.Next(-100000, 100000) + offset.y * (y / scale);

            octavesOffset[i] = new Vector2(xOffset / x, yOffset / y);
        }

        if (scale <= 0) scale = 0.0001f;

        float halfX = x / 2.0f;
        float halfY = y / 2.0f;

        for (int yy = 0; yy < y; yy++)
        {
            for(int xx=0; xx<x; xx++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                float superpositionCompensation = 0;

                for(int i=0; i<octaves; i++)
                {
                    float xResult = (x - halfX) / scale * frequency + octavesOffset[i].x * frequency;
                    float yResult = (y - halfY) / scale * frequency + octavesOffset[i].y * frequency;

                    float generateValue = Mathf.PerlinNoise(xResult, yResult);

                    noiseHeight += generateValue * amplitude;
                    noiseHeight -= superpositionCompensation;

                    amplitude *= persostence;
                    frequency *= lacunary;
                    superpositionCompensation = amplitude / 2;
                }
                noise[xx, yy] = Mathf.Clamp01(noiseHeight);
            }
        }
        return noise;
    }
}
