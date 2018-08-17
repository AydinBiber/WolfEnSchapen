using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece {
	public int position;
	private List<int> possibleMoves = new List<int>();
	public List<int> GetPossibleMoves(List<GridSpace> gridSpaces, GridSpaceStatus gridSpaceStatus, int position) {
		possibleMoves = new List<int>();

		switch (gridSpaceStatus) {
		case GridSpaceStatus.SHEEP:
			if (PieceOnBottomEdge(position) == true) {
			} else if (PieceOnRightEdge(position) == true) {
				if(
					gridSpaces[position + 7].GetGridSpaceStatus() != GridSpaceStatus.WOLF &&
					gridSpaces[position + 7].GetGridSpaceStatus() != GridSpaceStatus.SHEEP &&
					!((position + 7) >= 64)
				) { possibleMoves.Add (position + 7); }
			} else if (PieceOnLeftEdge(position) == true) {
				if(
					gridSpaces[position + 9].GetGridSpaceStatus() != GridSpaceStatus.WOLF &&
					gridSpaces[position + 9].GetGridSpaceStatus() != GridSpaceStatus.SHEEP &&
					!((position + 9) >= 64)
				) { possibleMoves.Add (position + 9); }
			} else {
				if(
					gridSpaces[position + 7].GetGridSpaceStatus() != GridSpaceStatus.WOLF &&
					gridSpaces[position + 7].GetGridSpaceStatus() != GridSpaceStatus.SHEEP &&
					!((position + 7) >= 64)
				) { possibleMoves.Add (position + 7); }
				if(
					gridSpaces[position + 9].GetGridSpaceStatus() != GridSpaceStatus.WOLF &&
					gridSpaces[position + 9].GetGridSpaceStatus() != GridSpaceStatus.SHEEP &&
					!((position + 9) >= 64)
				) { possibleMoves.Add (position + 9); }
			}
			break;
		case GridSpaceStatus.WOLF:
			if (PieceOnBottomEdge(position) == true) {
				if (position == 56) {
					possibleMoves.Add (49);
				} else {
					if(gridSpaces[position - 7].GetGridSpaceStatus() != GridSpaceStatus.SHEEP) { possibleMoves.Add (position - 7); }
					if(gridSpaces[position - 9].GetGridSpaceStatus() != GridSpaceStatus.SHEEP) { possibleMoves.Add (position - 9); }
				}
			} else if (PieceOnRightEdge(position) == true) {
				if(gridSpaces[position + 7].GetGridSpaceStatus() != GridSpaceStatus.SHEEP && !(position + 7 >= 64) ) { possibleMoves.Add (position + 7); }
				if(gridSpaces[position - 9].GetGridSpaceStatus() != GridSpaceStatus.SHEEP && !(position - 9 < 0) ) { possibleMoves.Add (position - 9); }
			} else if (PieceOnLeftEdge(position) == true) {
				if(gridSpaces[position - 7].GetGridSpaceStatus() != GridSpaceStatus.SHEEP && !(position - 7 < 0) ) { possibleMoves.Add (position - 7); }
				if(gridSpaces[position + 9].GetGridSpaceStatus() != GridSpaceStatus.SHEEP && !(position + 9 >= 64) ) { possibleMoves.Add (position + 9); }
			} else {
				if(gridSpaces[position + 7].GetGridSpaceStatus() != GridSpaceStatus.SHEEP && !(position + 7 >= 64) ) { possibleMoves.Add (position + 7); }
				if(gridSpaces[position - 9].GetGridSpaceStatus() != GridSpaceStatus.SHEEP && !(position - 9 < 0) ) { possibleMoves.Add (position - 9); }
				if(gridSpaces[position - 7].GetGridSpaceStatus() != GridSpaceStatus.SHEEP && !(position - 7 < 0) ) { possibleMoves.Add (position - 7); }
				if(gridSpaces[position + 9].GetGridSpaceStatus() != GridSpaceStatus.SHEEP && !(position + 9 >= 64) ) { possibleMoves.Add (position + 9); }
			}
			break;
		default:
			break;
		}

		return possibleMoves;
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
		if (position >= 56) {
			return true;
		} else {
			return false;
		}
	}
}

public class Wolf : Piece {
	public int position;
	public int bestMove;

	public Wolf(int position) {
		this.position = position;
	}
}

public class Sheep : Piece {
	public int position;
	public int bestMove;
	public int bestMoveDepth = 9999999;

	public Sheep(int position) {
		this.position = position;
	}
}

public class MiniMax {
	private List<GridSpace> gridSpaces;

	private List<Sheep> sheepList;
	private Wolf wolf;
	private Sheep bestSheep;

	public MiniMax(List<GridSpace> gridSpaces) {
		this.gridSpaces = gridSpaces;
		sheepList = new List<Sheep> ();

		foreach(GridSpace gridSpace in gridSpaces) {
			if(gridSpace.GetGridSpaceStatus() == GridSpaceStatus.SHEEP) {
				sheepList.Add (new Sheep(gridSpace.GetGridSpaceNumber()));
			} else if(gridSpace.GetGridSpaceStatus() == GridSpaceStatus.WOLF) {
				wolf = new Wolf (gridSpace.GetGridSpaceNumber());
			}
		}
	}

	public List<GridSpace> MoveSheep() {
		// Create a new tree
		Node tree = new Node ();
		tree.isRoot = true;
		tree.value = 0;

		bestSheep = sheepList [0];
		foreach(Sheep sheep in sheepList) {
			// A little Dolly Parton joke there. I don't care if it's against best practices.
			Sheep dolly = sheep;
			Wolf wolfClone = wolf;

			Node sheepNode = GenerateTree(tree, 0, true, dolly, wolfClone, GridSpaceStatus.SHEEP);
			sheepNode.movePosition = sheep.position;

			tree.children.Add(sheepNode);
			sheep.bestMove = dolly.bestMove;
			sheep.bestMoveDepth = dolly.bestMoveDepth;

			if(sheep.bestMoveDepth < bestSheep.bestMoveDepth) {
				this.bestSheep = sheep;
			}
		}

		gridSpaces [bestSheep.position].SetGridSpaceStatus(GridSpaceStatus.EMPTY);
		gridSpaces [bestSheep.bestMove].SetGridSpaceStatus(GridSpaceStatus.SHEEP);

		return gridSpaces;
	}

	public List<GridSpace> MoveWolf() {
		Node tree = new Node ();
		tree.isRoot = true;
		tree.value = 0;

		foreach(Sheep sheep in sheepList) {
			// A little Dolly Parton joke there. I don't care if it's against best practices.
			Sheep dolly = sheep;
			Wolf wolfClone = wolf;

			Node wolfNode = GenerateTree(tree, 0, true, dolly, wolfClone, GridSpaceStatus.WOLF);
			wolfNode.movePosition = sheep.position;

			tree.children.Add(wolfNode);
		}

		gridSpaces [wolf.position].SetGridSpaceStatus(GridSpaceStatus.EMPTY);
		gridSpaces [wolf.bestMove].SetGridSpaceStatus(GridSpaceStatus.WOLF);

		return gridSpaces;
	}

	private Node GenerateTree(Node node, int depth, bool aiMove, Sheep sheep, Wolf wolf, GridSpaceStatus gridSpaceStatus) {
		switch(gridSpaceStatus) {
		case GridSpaceStatus.SHEEP:
			// Wolf loses
			if(wolf.GetPossibleMoves(gridSpaces, GridSpaceStatus.WOLF, wolf.position).Count == 0) {
				node.value = +1;
				sheep.bestMove = node.movePosition;
				sheep.bestMoveDepth = depth;
				return node;
			}

			// Wolf wins
			if(
				wolf.position == 1 ||
				wolf.position == 3 ||
				wolf.position == 5 ||
				wolf.position == 7
			) {
				node.value = -1;
				return node;
			}

			if (aiMove == true) {
				foreach(int possibleMove in sheep.GetPossibleMoves(gridSpaces, GridSpaceStatus.SHEEP, sheep.position)) {
					Node moveNode = new Node ();
					sheep.position = possibleMove;
					node.children.Add (GenerateTree(moveNode, depth+1, false, sheep, wolf, gridSpaceStatus));
				}
			} else {
				foreach(int possibleMove in wolf.GetPossibleMoves(gridSpaces, GridSpaceStatus.WOLF, wolf.position)) {
					Node moveNode = new Node ();
					wolf.position = possibleMove;
					node.children.Add (GenerateTree(moveNode, depth+1, true, sheep, wolf, gridSpaceStatus));
				}
			}
			break;
		case GridSpaceStatus.WOLF:
			// Wolf wins
			if(
				wolf.position == 1 ||
				wolf.position == 3 ||
				wolf.position == 5 ||
				wolf.position == 7
			) {
				node.value = +1;
				wolf.bestMove = node.movePosition;
				return node;
			}

			// Wolf loses
			if(wolf.GetPossibleMoves(gridSpaces, GridSpaceStatus.WOLF, wolf.position).Count == 0) {
				node.value = -1;
				return node;
			}

			if (aiMove == true) {
				foreach(int possibleMove in wolf.GetPossibleMoves(gridSpaces, GridSpaceStatus.WOLF, sheep.position)) {
					Node moveNode = new Node ();
					wolf.position = possibleMove;
					node.children.Add (GenerateTree(moveNode, depth+1, false, sheep, wolf, gridSpaceStatus));
				}
			} else {
				foreach(int possibleMove in sheep.GetPossibleMoves(gridSpaces, GridSpaceStatus.SHEEP, wolf.position)) {
					// Generate possible move for each sheep?
					Node moveNode = new Node ();
					sheep.position = possibleMove;
					node.children.Add (GenerateTree(moveNode, depth+1, true, sheep, wolf, gridSpaceStatus));
				}
			}
			break;
		default:
			break;
		}

		return node;
	}
}
