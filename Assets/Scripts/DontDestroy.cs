using UnityEngine;
using System.Collections;
using System;

public class DontDestroy : MonoBehaviour {

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

}
