using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSpace : MonoBehaviour {

	public Sprite emptySprite;
	public Sprite wolfSprite;
	public Sprite sheepSprite;

	[SerializeField]
	private int gridSpaceNumber;
	[SerializeField]
	private GridSpaceStatus gridSpaceStatus = GridSpaceStatus.EMPTY;

	public int GetGridSpaceNumber() {
		return this.gridSpaceNumber;
	}

	public void SetGridSpaceNumber(int gridSpaceNumber) {
		this.gridSpaceNumber = gridSpaceNumber;
	}

	public GridSpaceStatus GetGridSpaceStatus() {
		return this.gridSpaceStatus;
	}

	public void SetGridSpaceStatus(GridSpaceStatus gridSpaceStatus) {
		//Debug.Log (this.ToString());
		this.gridSpaceStatus = gridSpaceStatus;
		if (gridSpaceStatus == GridSpaceStatus.WOLF) {
			this.GetComponent<Image>().sprite = wolfSprite;
		} else if (gridSpaceStatus == GridSpaceStatus.SHEEP) { 
			this.GetComponent<Image>().sprite = sheepSprite;
		} else {
			this.GetComponent<Image>().sprite = emptySprite;
		}
	}

	public override string ToString() {
		return "GridSpaceNumber: " + gridSpaceNumber + " - GridSpaceStatus: " + gridSpaceStatus.ToString();
	}

	public void ButtonClick() {
		GameObject scripts = GameObject.Find ("/Scripts");
		GameController gameController = scripts.GetComponent<GameController> ();
		if(gameController.GetPlayerTurn() == false) {
			return;
		}

		Player player = GameObject.Find("/Player").GetComponent<Player> ();
		CursorController cursorController = scripts.GetComponent<CursorController> ();
		CursorStatus cursorStatus = cursorController.GetCursorStatus();

		// The playerRole is a GridSpaceStatus to easily check if the clicked GridSpace can be moved by the player.
		if(cursorStatus == CursorStatus.EMPTY && player.playerRole == this.gridSpaceStatus) {
			// Pick up current piece
			cursorController.SetCursorStatus(CursorStatus.OCCUPIED);
			cursorController.setPreviousLocation (this.gridSpaceNumber);
			this.SetGridSpaceStatus (GridSpaceStatus.EMPTY);

			// Color the tiles where a player can put the piece.
			switch(player.playerRole) {
			case GridSpaceStatus.WOLF:
				if (PieceOnLeftEdge (gridSpaceNumber) == true) {
					gameController.SetGridSpaceAvailableColor (gridSpaceNumber);
					gameController.SetGridSpaceAvailableColor (gridSpaceNumber - 7);
					gameController.SetGridSpaceAvailableColor (gridSpaceNumber + 9);
				} else if (PieceOnRightEdge (gridSpaceNumber) == true) {
					gameController.SetGridSpaceAvailableColor (gridSpaceNumber);
					gameController.SetGridSpaceAvailableColor (gridSpaceNumber - 9);
					gameController.SetGridSpaceAvailableColor (gridSpaceNumber + 7);
				} else {
					gameController.SetGridSpaceAvailableColor (gridSpaceNumber);
					gameController.SetGridSpaceAvailableColor (gridSpaceNumber - 7);
					gameController.SetGridSpaceAvailableColor (gridSpaceNumber - 9);
					gameController.SetGridSpaceAvailableColor (gridSpaceNumber + 7);
					gameController.SetGridSpaceAvailableColor (gridSpaceNumber + 9);
				}
				break;
			case GridSpaceStatus.SHEEP:
				if (PieceOnLeftEdge (gridSpaceNumber) == true) {
					gameController.SetGridSpaceAvailableColor (gridSpaceNumber);
					gameController.SetGridSpaceAvailableColor (gridSpaceNumber + 9);
				} else if (PieceOnRightEdge (gridSpaceNumber) == true) {
					gameController.SetGridSpaceAvailableColor (gridSpaceNumber);
					gameController.SetGridSpaceAvailableColor (gridSpaceNumber + 7);
				} else {
					gameController.SetGridSpaceAvailableColor (gridSpaceNumber);
					gameController.SetGridSpaceAvailableColor (gridSpaceNumber + 7);
					gameController.SetGridSpaceAvailableColor (gridSpaceNumber + 9);
				}
				break;
			default:
				Debug.LogError ("PlayerRole set to UNKNOWN. Check Player.cs and MainMenu.cs");
				break;
			}
		}

		if(cursorStatus == CursorStatus.OCCUPIED) {
			switch (player.playerRole) {
			case GridSpaceStatus.WOLF:
				if (PieceOnLeftEdge (cursorController.getPreviousLocation ())) {
					if (
						this.gridSpaceNumber == cursorController.getPreviousLocation() ||
						this.gridSpaceNumber == (cursorController.getPreviousLocation () - 7) ||
						this.gridSpaceNumber == (cursorController.getPreviousLocation () + 9)
					) {
						if(this.gridSpaceStatus != GridSpaceStatus.SHEEP) {
							PlacePiece (cursorController, gameController, player);
							gameController.SetWolfPosition (this.gridSpaceNumber);
						}
					}
				} else if (PieceOnRightEdge (cursorController.getPreviousLocation ())) {
					if (
						this.gridSpaceNumber == cursorController.getPreviousLocation() ||
						this.gridSpaceNumber == (cursorController.getPreviousLocation () - 9) ||
						this.gridSpaceNumber == (cursorController.getPreviousLocation () + 7)
					) {
						if(this.gridSpaceStatus != GridSpaceStatus.SHEEP) {
							PlacePiece (cursorController, gameController, player);
							gameController.SetWolfPosition (this.gridSpaceNumber);
						}
					}
				} else {
					if (
						this.gridSpaceNumber == cursorController.getPreviousLocation() ||
						this.gridSpaceNumber == (cursorController.getPreviousLocation () - 7) ||
						this.gridSpaceNumber == (cursorController.getPreviousLocation () - 9) ||
						this.gridSpaceNumber == (cursorController.getPreviousLocation () + 7) ||
						this.gridSpaceNumber == (cursorController.getPreviousLocation () + 9)
					) {
						if(this.gridSpaceStatus != GridSpaceStatus.SHEEP) {
							PlacePiece (cursorController, gameController, player);
							gameController.SetWolfPosition (this.gridSpaceNumber);
						}
					}
				}
				break;
			case GridSpaceStatus.SHEEP:
				if(this.gridSpaceStatus != GridSpaceStatus.WOLF) {
					if (PieceOnLeftEdge (cursorController.getPreviousLocation ())) {
						if (
							this.gridSpaceNumber == cursorController.getPreviousLocation() ||
							this.gridSpaceNumber == (cursorController.getPreviousLocation () + 9)
						) {
							PlacePiece (cursorController, gameController, player);
						}
					} else if (PieceOnRightEdge (cursorController.getPreviousLocation ())) {
						if (
							this.gridSpaceNumber == cursorController.getPreviousLocation() ||
							this.gridSpaceNumber == (cursorController.getPreviousLocation () + 7)
						) {
							PlacePiece (cursorController, gameController, player);
						}
					} else {
						if (
							this.gridSpaceNumber == cursorController.getPreviousLocation() ||
							this.gridSpaceNumber == (cursorController.getPreviousLocation () + 7) ||
							this.gridSpaceNumber == (cursorController.getPreviousLocation () + 9)
						) {
							PlacePiece (cursorController, gameController, player);
						}
					}
				}
				break;
			default:
				Debug.LogError ("PlayerRole set to UNKNOWN. Check Player.cs and MainMenu.cs");
				break;
			}
		}
	}

	private void PlacePiece(CursorController cursorController, GameController gameController, Player player) {
		cursorController.SetCursorStatus (CursorStatus.EMPTY);
		this.SetGridSpaceStatus (player.playerRole);

		gameController.SetGridSpaceUnavailableColor (cursorController.getPreviousLocation());
		gameController.SetGridSpaceUnavailableColor (cursorController.getPreviousLocation() - 7);
		gameController.SetGridSpaceUnavailableColor (cursorController.getPreviousLocation() - 9);
		gameController.SetGridSpaceUnavailableColor (cursorController.getPreviousLocation() + 7);
		gameController.SetGridSpaceUnavailableColor (cursorController.getPreviousLocation() + 9);

		if(this.gridSpaceNumber != cursorController.getPreviousLocation()) {
			gameController.AITurn ();
		}
	}

	// Check if the picked up piece was standing on the left edge
	public bool PieceOnLeftEdge(int position) {
		if (
			position == 0 ||
			position == 8 ||
			position == 16 ||
			position == 24 ||
			position == 32 ||
			position == 40 ||
			position == 48 ||
			position == 56
		) {
			return true;
		} else {
			return false;
		}
	}

	// Check if the picked up piece was standing on the right edge
	public bool PieceOnRightEdge(int position) {
		if (
			position == 7 ||
			position == 15 ||
			position == 23 ||
			position == 31 ||
			position == 39 ||
			position == 47 ||
			position == 55 ||
			position == 63
		) {
			return true;
		} else {
			return false;
		}
	}

	// Check if the picked up piece was standing on the right edge
	public bool PieceOnBottomEdge(int position) {
		if (position > 56) {
			return true;
		} else {
			return false;
		}
	}
}
