using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillZone : MonoBehaviour { // Doesn't work because raycasting won't do anything except land on things

	private void OnTriggerEnter2D () { 
        SceneManager.LoadScene("RayPlatforming");
	}
}