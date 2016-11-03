using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(VideoController))]
public class VideoControllerEditor : Editor {

	public override void OnInspectorGUI() {
		VideoController vidController = (VideoController)target;

		if (DrawDefaultInspector()) {
			vidController.UpdateFromEditor();
		}

		//if (GUILayout.Button("Generate")) {
		//	mapGen.DrawMapInEditor();
		//}
	}

}
