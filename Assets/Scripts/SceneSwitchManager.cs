using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class SceneSwitchManager : MonoBehaviour {

	public Canvas uiCanvas;
	public Image backgroundImage;

	Dictionary<int, KeyCode> functionKeysBySceneIndex;

	// Singleton to ensure object is not duplicated
	private static SceneSwitchManager sceneSwitchManagerInstance;

	void Awake() {

		InitKeyBindings();

		DontDestroyOnLoad(this);

		if (sceneSwitchManagerInstance == null) {
			sceneSwitchManagerInstance = this;
		} else {
			DestroyObject(gameObject);
		}
	}
	
	void InitKeyBindings() {

		// Init F1 - F12 mappings
		functionKeysBySceneIndex = new Dictionary<int, KeyCode>();
		for (int i = 1; i <= 12; i++) {
			KeyCode fKey = (KeyCode)Enum.Parse(typeof(KeyCode), string.Concat("F", i));
			functionKeysBySceneIndex.Add(i, fKey);
		}
	}

	// Update is called once per frame
	void Update() {

		HandleInput();

	}

	private void HandleInput() {

		if (Input.anyKeyDown)
		{
			if (Input.GetKeyDown(KeyCode.Backspace))
			{
				// Back to main
				SceneManager.LoadScene(0);
				uiCanvas.enabled = true;
			}
			else if (Input.GetKeyDown(KeyCode.M))
			{
				// Cursor
				Cursor.visible = !Cursor.visible;
			}
			else if (Input.GetKeyDown(KeyCode.Escape))
			{
				// Quit
				Application.Quit();
			}
			else {
				if (functionKeysBySceneIndex == null)
				{
					InitKeyBindings();
					CustomLogHandler.Logger.Log("SceneSwitch.Update()", "Key bindings not initialized.");
				}
				// F1 -> F12
				foreach (KeyValuePair<int, KeyCode> sceneKeyPair in functionKeysBySceneIndex)
				{
					if (Input.GetKeyDown(sceneKeyPair.Value))
					{

						// Function key pressed. Change scene now

						uiCanvas.enabled = false;
						SceneManager.LoadScene(sceneKeyPair.Key);

						break;
					}
				}
			}
		}
	}
}
