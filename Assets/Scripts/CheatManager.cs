﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatManager : MonoBehaviour {
	
	[SerializeField]
	GameObject graphy;

	[SerializeField]
	GameUI gameUI;
    bool inverse = true;

	void Start()
	{
		if (gameUI == null)
			gameUI = FindObjectOfType<GameUI>();
	}

	// Update is called once per frame
	void Update () {
		if (SceneManager.GetActiveScene().buildIndex == 0) {
			if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.T)) {
				SceneManager.LoadScene("PlayerTestScene");
			}
		}
		
		if (gameUI != null) {
			if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L)) {
				PlayerController pc = gameUI.GetPlayerController();
				pc.curLife = pc.maxLife;
			} else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.M)) {
				PlayerController pc = gameUI.GetPlayerController();
				pc.curMana = pc.maxMana;
			} else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C))
            {
                PlayerController pc = gameUI.GetPlayerController();
                pc.cam.GetComponent<CameraControl>().SetInverse(inverse);
                if (inverse)
                {
                    inverse = false;
                }
                else
                {
                    inverse = true;
                }
                
            } else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
            {
				PlayerController pc = gameUI.GetPlayerController();

				pc.cam.transform.SetParent(null);

                pc.cam.GetComponent<CameraControl>().enabled = false;
                pc.cam.GetComponent<FreeCamera>().enabled = true;
			} else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V))
            {
				PlayerController pc = gameUI.GetPlayerController();

				pc.cam.transform.SetParent(pc.transform);

                pc.cam.GetComponent<CameraControl>().enabled = true;
                pc.cam.GetComponent<FreeCamera>().enabled = false;
			} else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.U))
            {
				gameUI.gameObject.SetActive(!gameUI.gameObject.activeSelf);
			}
        }

		if (graphy != null && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D)) {
			graphy.SetActive(!graphy.activeSelf);
		}
	}
}
