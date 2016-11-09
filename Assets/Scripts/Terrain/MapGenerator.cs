using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Threading;

public class MapGenerator : MonoBehaviour {

	public DrawMode drawMode;

	public NoiseType noiseType;


	static MapGenerator instance;
	public static int mapChunkSize
	{
		get {
			if (instance == null) {
				instance = FindObjectOfType<MapGenerator>();
			}

			if (instance.useFlatShading) {
				return 95;
			} else {
				return 241;
			}
		}
	}

	[Range(0, 6)]
	public int editorPreviewLOD;
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

	public bool useFlatShading;

	[Range(1, 10)]
	public int lineSpacing;

	public TerrainType[] regions;

	Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
	Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

	public void DrawMapInEditor() {

		MapData mapData = GenerateMapData(Vector2.zero);

		MapDisplay display = FindObjectOfType<MapDisplay>();
		if (drawMode == DrawMode.NoiseMap) {
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
		} else if (drawMode == DrawMode.ColourMap) {
			display.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
		} else if (drawMode == DrawMode.Mesh) {
			display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, editorPreviewLOD, useFlatShading), TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
		}
	}

	private Color[] GenerateColourMap(float[,] noiseMap, NoiseType _noiseType, int lineSpacing) {

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

					if (y % lineSpacing == 0) {
						colourMap[y * mapChunkSize + x] = Color.white;
					} else {
						colourMap[y * mapChunkSize + x] = Color.black;
					}
				}
			}
		}

		return colourMap;
	}

	public void RequestMapData(Vector2 centre, Action<MapData> callback) {
		ThreadStart threadStart = delegate {
			MapDataThread(centre, callback);
		};

		new Thread(threadStart).Start();
	}

	void MapDataThread(Vector2 centre, Action<MapData> callback) {
		MapData mapData = GenerateMapData(centre);
		lock (mapDataThreadInfoQueue) {
			mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
		}
	}

	public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback) {
		ThreadStart threadStart = delegate {
			MeshDataThread(mapData, lod, callback);
		};

		new Thread(threadStart).Start();
	}

	void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback) {
		MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, lod, useFlatShading);
		lock (meshDataThreadInfoQueue) {
			meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
		}
	}

	void Update() {
		if (mapDataThreadInfoQueue.Count > 0) {
			for (int i = 0; i < mapDataThreadInfoQueue.Count; i++) {
				MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
				threadInfo.callback(threadInfo.parameter);
			}
		}

		if (meshDataThreadInfoQueue.Count > 0) {
			for (int i = 0; i < meshDataThreadInfoQueue.Count; i++) {
				MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
				threadInfo.callback(threadInfo.parameter);
			}
		}
	}

	MapData GenerateMapData(Vector2 centre) {


		float[,] noiseMap = Noise.GenerateNoiseMap(noiseType, mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, centre + offset, noiseFactorCurve);

		Color[] colourMap = GenerateColourMap(noiseMap, noiseType, lineSpacing);

		return new MapData(noiseMap, colourMap);
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
		if (lineSpacing <= 0) {
			lineSpacing = 1;
		}
	}

	struct MapThreadInfo<T> {
		public readonly Action<T> callback;
		public readonly T parameter;

		public MapThreadInfo(Action<T> callback, T parameter) {
			this.callback = callback;
			this.parameter = parameter;
		}

	}
}

[System.Serializable]
public struct TerrainType {
	public string name;
	public float height;
	public Color colour;
}

public struct MapData {
	public readonly float[,] heightMap;
	public readonly Color[] colourMap;

	public MapData(float[,] heightMap, Color[] colourMap) {
		this.heightMap = heightMap;
		this.colourMap = colourMap;
	}
}