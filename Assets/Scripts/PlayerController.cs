﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {


	// Update is called once per frame
	void Update () {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		Debug.logger.Log(SceneManager.GetActiveScene().name);
		Debug.logger.Log(SceneManager.GetActiveScene ().name.Equals("LoginWindow"));
		if (SceneManager.GetActiveScene ().name.Equals ("LoginWindow")) {
			if (Input.GetMouseButtonDown (0) || Input.GetMouseButtonDown (1) || Input.GetMouseButtonDown (2)) {
				if (Physics.Raycast (ray, out hit) && hit.transform.tag.Contains("Button")) {
					LoginWindowGUIManager guiManager = 
						GameObject.Find("GUIManager").GetComponentInChildren<LoginWindowGUIManager> ();
					string methodName = hit.transform.name.Replace ("Button", "");
					guiManager.Invoke(methodName, 0);
				}
			}
		} else {
	
			if (Physics.Raycast (ray, out hit) && hit.transform.CompareTag ("Item")) {
				GameObject fruit = hit.transform.gameObject;
				FruitController fruitController = fruit.GetComponentInChildren<FruitController> ();
				GUIManager guiManager = GameObject.Find ("GUIManager").GetComponentInChildren<GUIManager> ();
				Destroy (fruit);
				if (fruitController.IfScoreable ()) {
					guiManager.AddScore (10);
				} else {
					guiManager.LoseScore (10);
				}
			}
		}
	}
}
