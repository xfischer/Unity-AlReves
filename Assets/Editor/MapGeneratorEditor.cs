using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor {

	public override void OnInspectorGUI() {
		MapGenerator mapGen = (MapGenerator)target;

		if (DrawDefaultInspector()) {
			if (mapGen.autoUpdate) {
				mapGen.DrawMapInEditor();
			}
		}

		if (GUILayout.Button("Generate")) {
			mapGen.DrawMapInEditor();
		}

		if (GUILayout.Button("Default Regions") &&
				EditorUtility.DisplayDialog("Reset all defined regions ?",
																		"All previsously created regions will be deleted.",
																		"Reset regions",
																		"Cancel")) {
			mapGen.GenerateDefaultRegions();
		}
	}
}
