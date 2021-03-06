using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	private GameObject playerObject;
	private Player player;

	void Awake() {
		playerObject = GameObject.Find ("/Player");
		player = playerObject.GetComponent<Player> ();
	}

	public void SetPlayerToWolf () {
		player.playerRole = GridSpaceStatus.WOLF;
		LoadGameScene ();
	}

	public void SetPlayerToSheep() {
		player.playerRole = GridSpaceStatus.SHEEP;
		LoadGameScene ();
	}

	private void LoadGameScene() {
		SceneManager.LoadScene ("Game", LoadSceneMode.Single);
	}
}
