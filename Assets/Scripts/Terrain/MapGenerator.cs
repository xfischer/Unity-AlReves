using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public DrawMode drawMode;

	public NoiseType noiseType;

	const int mapChunkSize = 241;
	[Range(0, 6)]
	public int levelOfDetail;
	public float noiseScale;


	[Range(0, 8)]
	public int octaves;
	[Range(0, 1)]
	public float persistance;

	[Range(0, 20)]
	public float lacunarity;

	public int seed;
	public Vector2 offset;

	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;

	public AnimationCurve noiseFactorCurve;

	public bool autoUpdate;

	public TerrainType[] regions;

	public void GenerateMap() {
		float[,] noiseMap = Noise.GenerateNoiseMap(noiseType, mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset, noiseFactorCurve);

		Color[] colourMap = GenerateColourMap(noiseType, noiseMap);

		MapDisplay display = FindObjectOfType<MapDisplay>();
		if (drawMode == DrawMode.NoiseMap) {
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
		} else if (drawMode == DrawMode.ColourMap) {
			display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
		} else if (drawMode == DrawMode.Mesh) {
			if (noiseType == NoiseType.Terrain) {
				display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
			} else if (noiseType == NoiseType.Vasarely) {
				display.DrawMesh(MeshGenerator.GenerateVasarelyLineMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
			}
		}
	}

	private Color[] GenerateColourMap(NoiseType _noiseType, float[,] noiseMap) {

		Color[] colourMap = new Color[mapChunkSize * mapChunkSize];

		if (_noiseType == NoiseType.Terrain) {
			for (int y = 0; y < mapChunkSize; y++) {
				for (int x = 0; x < mapChunkSize; x++) {
					float currentHeight = noiseMap[x, y];
					for (int i = 0; i < regions.Length; i++) {
						if (currentHeight <= regions[i].height) {
							colourMap[y * mapChunkSize + x] = regions[i].colour;
							break;
						}
					}
				}
			}
		} else if (_noiseType == NoiseType.Vasarely) {
			for (int y = 0; y < mapChunkSize; y++) {
				for (int x = 0; x < mapChunkSize; x++) {
					float currentHeight = noiseMap[x, y];

					if (currentHeight < 0) {
						colourMap[y * mapChunkSize + x] = Color.Lerp(Color.white, Color.clear, -currentHeight);
					} else {
						colourMap[y * mapChunkSize + x] = Color.white;
					}
				}
			}
		}


		return colourMap;
	}

	public void GenerateDefaultRegions() {
		regions = new TerrainType[8];
		int i = 0;
		regions[i] = new TerrainType { height = 0.3f, name = "Water Deep" };
		ColorUtility.TryParseHtmlString("#3263C300", out regions[i].colour);
		i++;
		regions[i] = new TerrainType { height = 0.4f, name = "Water Shallow" };
		ColorUtility.TryParseHtmlString("#3666C600", out regions[i].colour);

		i++;
		regions[i] = new TerrainType { height = 0.45f, name = "Sand" };
		ColorUtility.TryParseHtmlString("#D2CF7D00", out regions[i].colour);

		i++;
		regions[i] = new TerrainType { height = 0.55f, name = "Grass" };
		ColorUtility.TryParseHtmlString("#56971700", out regions[i].colour);

		i++;
		regions[i] = new TerrainType { height = 0.6f, name = "Grass 2" };
		ColorUtility.TryParseHtmlString("#3D6A1200", out regions[i].colour);

		i++;
		regions[i] = new TerrainType { height = 0.7f, name = "Rock" };
		ColorUtility.TryParseHtmlString("#59453C00", out regions[i].colour);

		i++;
		regions[i] = new TerrainType { height = 0.9f, name = "Rock2" };
		ColorUtility.TryParseHtmlString("#4A3B3500", out regions[i].colour);

		i++;
		regions[i] = new TerrainType { height = 1f, name = "Snow" };
		ColorUtility.TryParseHtmlString("#FFFFFF00", out regions[i].colour);
	}


	void OnValidate() {
		if (lacunarity <= 0) {
			lacunarity = 0.001f;
		}
		if (octaves < 0) {
			octaves = 0;
		}
	}
}

[System.Serializable]
public struct TerrainType {
	public string name;
	public float height;
	public Color colour;
}