using UnityEngine;
using System.Collections;

public static class Noise {

	public static float[,] GenerateNoiseMap(NoiseType noiseType, int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, AnimationCurve noiseFactorCurve = null) {
		switch (noiseType) {
			case NoiseType.Vasarely:
				return GenerateVasarelyNoiseMap(mapWidth, mapHeight, seed, scale, octaves, persistance, lacunarity, offset, noiseFactorCurve);
			default:
				return GenerateTerrainNoiseMap(mapWidth, mapHeight, seed, scale, octaves, persistance, lacunarity, offset);
		}
	}

	private static float[,] GenerateTerrainNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, AnimationCurve noiseFactorCurve = null) {

		float[,] noiseMap = new float[mapWidth, mapHeight];

		System.Random prng = new System.Random(seed);
		Vector2[] octaveOffsets = new Vector2[octaves];
		for (int i = 0; i < octaves; i++) {
			float offsetX = prng.Next(-100000, 100000) + offset.x;
			float offsetY = prng.Next(-100000, 100000) + offset.y;
			octaveOffsets[i] = new Vector2(offsetX, offsetY);
		}

		if (scale <= 0) {
			scale = 0.0001f;
		}

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfWidth = mapWidth / 2f;
		float halfHeight = mapHeight / 2f;


		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {

				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < octaves; i++) {
					float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
					float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
					noiseHeight += perlinValue * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}

				if (noiseHeight > maxNoiseHeight) {
					maxNoiseHeight = noiseHeight;
				} else if (noiseHeight < minNoiseHeight) {
					minNoiseHeight = noiseHeight;
				}
				noiseMap[x, y] = noiseHeight;
			}
		}

		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
				noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
			}
		}

		return noiseMap;
	}

	private static float[,] GenerateVasarelyNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, AnimationCurve _noiseFactorCurve) {
		AnimationCurve noiseFactorCurve = new AnimationCurve(_noiseFactorCurve.keys);

		float[,] noiseMap = new float[mapWidth, mapHeight];

		System.Random prng = new System.Random(seed);
		Vector2[] octaveOffsets = new Vector2[octaves];
		for (int i = 0; i < octaves; i++) {
			float offsetX = prng.Next(-100000, 100000) + offset.x;
			float offsetY = prng.Next(-100000, 100000) + offset.y;
			octaveOffsets[i] = new Vector2(offsetX, offsetY);
		}

		if (scale <= 0) {
			scale = 0.0001f;
		}

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfWidth = mapWidth / 2f;
		float halfHeight = mapHeight / 2f;


		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {

				float amplitude = persistance;
				float frequency = lacunarity;
				float noiseHeight = 0;

				for (int i = 0; i < octaves; i++) {
					float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
					float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
					noiseHeight += perlinValue * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}

				if (noiseHeight > maxNoiseHeight) {
					maxNoiseHeight = noiseHeight;
				} else if (noiseHeight < minNoiseHeight) {
					minNoiseHeight = noiseHeight;
				}
				noiseMap[x, y] = noiseFactorCurve.Evaluate(noiseHeight);
			}
		}

		//for (int y = 0; y < mapHeight; y++) {
		//	for (int x = 0; x < mapWidth; x++) {
		//		noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
		//	}
		//}

		return noiseMap;
	}

	//private static float[,] GenerateVasarelyNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset) {

	//	float[,] noiseMap = new float[mapWidth, mapHeight];

	//	System.Random prng = new System.Random(seed);
	//	float offsetX = prng.Next(-100000, 100000) + offset.x;
	//	float offsetY = prng.Next(-100000, 100000) + offset.y;
	//	Vector2 octaveOffset = new Vector2(offsetX, offsetY);


	//	if (scale <= 0) {
	//		scale = 0.0001f;
	//	}

	//	float halfWidth = mapWidth / 2f;
	//	float halfHeight = mapHeight / 2f;

	//	for (int y = 0; y < mapHeight; y++) {
	//		for (int x = 0; x < mapWidth; x++) {

	//			float amplitude = 1;
	//			float frequency =  lacunarity;
	//			float noiseHeight = 0;

	//			float sampleX = (x - halfWidth) / scale * frequency + octaveOffset.x;
	//			float sampleY = (y - halfHeight) / scale * frequency + octaveOffset.y;

	//			float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
	//			noiseHeight += perlinValue * amplitude;

	//			amplitude *= persistance;
	//			frequency *= lacunarity;


	//			noiseMap[x, y] = noiseHeight;
	//		}
	//	}

	//	return noiseMap;
	//}

}
