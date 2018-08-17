using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
	public GameObject gridArea;
	public GameObject gridSpaceButton;
	public Material greenGridSpaceButtonMaterial;
	private Material originalGridSpaceButtonMaterial;

	private Player player;

	private List<GridSpace> gridSpaces = new List<GridSpace>();
	public List<GridSpace> GetGridSpaces() {
		return this.gridSpaces;
	}
	public void SetGridSpaceAvailableColor(int gridSpaceNumber) {
		if(!(gridSpaceNumber < 0) && !(gridSpaceNumber > 63)) {
			gridSpaces [gridSpaceNumber].gameObject.GetComponent<Image> ().color = new Color(0, 255, 0, 255);
			gridSpaces [gridSpaceNumber].gameObject.GetComponent<Image> ().material = greenGridSpaceButtonMaterial;
		}
	}
	public void SetGridSpaceUnavailableColor(int gridSpaceNumber) {
		if(!(gridSpaceNumber < 0) && !(gridSpaceNumber > 63)) {
			gridSpaces [gridSpaceNumber].gameObject.GetComponent<Image> ().color = new Color(255, 255, 255, 255);
			gridSpaces [gridSpaceNumber].gameObject.GetComponent<Image> ().material = originalGridSpaceButtonMaterial;
		}
	}

	private bool playerTurn = true;
	public bool GetPlayerTurn() {
		return this.playerTurn;
	}

	private int wolfPosition;
	public int GetWolfPosition() {
		return this.wolfPosition;
	}
	public void SetWolfPosition(int wolfPosition) {
		this.wolfPosition = wolfPosition;
	}

	/*
	 * Initialise the buttons for the grid.
	 * */
	void Awake() {
		originalGridSpaceButtonMaterial = gridSpaceButton.GetComponent<Image> ().material;
		player = FindObjectOfType<Player>();
		wolfPosition = 60;

		int k = 0;
		for(int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				GameObject tempButton = Instantiate (gridSpaceButton);
				tempButton.transform.SetParent(gridArea.transform);
				tempButton.transform.localPosition = new Vector2((-224 + (64 * j)), (224 - (64 * i)));
				tempButton.transform.GetComponentInChildren<Text>().text = k.ToString();

				GridSpace tempGridSpace = tempButton.GetComponent<GridSpace> ();
				tempGridSpace.SetGridSpaceNumber(k);
				tempGridSpace.SetGridSpaceStatus(GridSpaceStatus.EMPTY);
				tempButton.GetComponent<Button> ().onClick.AddListener (tempButton.GetComponent<GridSpace>().ButtonClick);

				gridSpaces.Add (tempButton.GetComponent<GridSpace>());
				k++;
			}
		}

		StartGame ();
	}

	void StartGame() {
		ResetGridSpaces ();
		if(playerTurn == false) {
			AITurn();
		}
	}

	void ResetGridSpaces() {
		for(int i = 0; i < 64; i++) {
			gridSpaces [i].SetGridSpaceStatus (GridSpaceStatus.EMPTY);
		}

		// 1, 3, 5 and 7 are sheep
		// 60 is a wolf
		gridSpaces[1].SetGridSpaceStatus(GridSpaceStatus.SHEEP);
		gridSpaces[3].SetGridSpaceStatus(GridSpaceStatus.SHEEP);
		gridSpaces[5].SetGridSpaceStatus(GridSpaceStatus.SHEEP);
		gridSpaces[7].SetGridSpaceStatus(GridSpaceStatus.SHEEP);

		gridSpaces[wolfPosition].SetGridSpaceStatus(GridSpaceStatus.WOLF);
	}

	public void AITurn() {
		playerTurn = false;
		CheckWinConditions ();
		MiniMax AI = new MiniMax(gridSpaces);
		if (player.playerRole == GridSpaceStatus.WOLF) {
			gridSpaces = AI.MoveSheep ();
		} else {
			gridSpaces = AI.MoveWolf ();
		}
		CheckWinConditions ();
		playerTurn = true;
	}

	public void CheckWinConditions() {
		// First, check if the Wolf has reached the other side of the board.
		// Secondly, check if the wolf can no longer move to a free space.
		if(
			gridSpaces[1].GetGridSpaceStatus() == GridSpaceStatus.WOLF ||
			gridSpaces[3].GetGridSpaceStatus() == GridSpaceStatus.WOLF ||
			gridSpaces[5].GetGridSpaceStatus() == GridSpaceStatus.WOLF ||
			gridSpaces[7].GetGridSpaceStatus() == GridSpaceStatus.WOLF
		) {
			Debug.Log ("The Wolf Wins!");
		}
			
		if(gridSpaces[wolfPosition].PieceOnLeftEdge(wolfPosition) == true) {
			if(
				gridSpaces[wolfPosition - 7].GetGridSpaceStatus() == GridSpaceStatus.SHEEP &&
				gridSpaces[wolfPosition + 9].GetGridSpaceStatus() == GridSpaceStatus.SHEEP
			) {
				Debug.Log ("The Sheep Win!");
			}
		} else if(gridSpaces[wolfPosition].PieceOnRightEdge(wolfPosition) == true) {
			if(
				gridSpaces[wolfPosition + 7].GetGridSpaceStatus() == GridSpaceStatus.SHEEP &&
				gridSpaces[wolfPosition - 9].GetGridSpaceStatus() == GridSpaceStatus.SHEEP
			) {
				Debug.Log ("The Sheep Win!");
			}
		} else if(gridSpaces[wolfPosition].PieceOnBottomEdge(wolfPosition) == true) {
			if(
				gridSpaces[wolfPosition - 7].GetGridSpaceStatus() == GridSpaceStatus.SHEEP &&
				gridSpaces[wolfPosition - 9].GetGridSpaceStatus() == GridSpaceStatus.SHEEP
			) {
				Debug.Log ("The Sheep Win!");
			}
		}
	}
}
