using UnityEngine;
using System.Collections;
using System;

namespace Terrain {
	public class MapGenerator : MonoBehaviour {

		public enum DrawMode { NoiseMap, ColourMap, Mesh };
		public DrawMode drawMode;

		public int mapWidth;
		public int mapHeight;
		public float noiseScale;

		public int octaves;
		[Range(0, 1)]
		public float persistance;
		public float lacunarity;

		public int seed;
		public Vector2 offset;

		public bool autoUpdate;


		[Tooltip("Texture color for each height range. Click on Default Regions button to generate defaults.")]
		public TerrainType[] regions;

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

		public void GenerateMap() {
			float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

			Color[] colourMap = new Color[mapWidth * mapHeight];
			for (int y = 0; y < mapHeight; y++) {
				for (int x = 0; x < mapWidth; x++) {
					float currentHeight = noiseMap[x, y];
					for (int i = 0; i < regions.Length; i++) {
						if (currentHeight <= regions[i].height) {
							colourMap[y * mapWidth + x] = regions[i].colour;
							break;
						}
					}
				}
			}

			MapDisplay display = FindObjectOfType<MapDisplay>();
			if (drawMode == DrawMode.NoiseMap) {
				display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
			} else if (drawMode == DrawMode.ColourMap) {
				display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
			} else if (drawMode == DrawMode.Mesh) {
				display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap), TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
			}
		}

		void OnValidate() {
			if (mapWidth < 1) {
				mapWidth = 1;
			}
			if (mapHeight < 1) {
				mapHeight = 1;
			}
			if (lacunarity < 1) {
				lacunarity = 1;
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
}