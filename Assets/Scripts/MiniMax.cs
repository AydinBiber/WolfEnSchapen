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
			if (PieceOnBottomEdge() == true) {
			} else if (PieceOnRightEdge() == true) {
				if(
					gridSpaces[position + 7].GetGridSpaceStatus() != GridSpaceStatus.WOLF &&
					gridSpaces[position + 7].GetGridSpaceStatus() != GridSpaceStatus.SHEEP
				) { possibleMoves.Add (position + 7); }
			} else if (PieceOnLeftEdge() == true) {
				if(
					gridSpaces[position + 9].GetGridSpaceStatus() != GridSpaceStatus.WOLF &&
					gridSpaces[position + 9].GetGridSpaceStatus() != GridSpaceStatus.SHEEP
				) { possibleMoves.Add (position + 9); }
			} else {
				if(
					gridSpaces[position + 7].GetGridSpaceStatus() != GridSpaceStatus.WOLF &&
					gridSpaces[position + 7].GetGridSpaceStatus() != GridSpaceStatus.SHEEP
				) { possibleMoves.Add (position + 7); }
				if(
					gridSpaces[position + 9].GetGridSpaceStatus() != GridSpaceStatus.WOLF &&
					gridSpaces[position + 9].GetGridSpaceStatus() != GridSpaceStatus.SHEEP
				) { possibleMoves.Add (position + 9); }
			}
			break;
		case GridSpaceStatus.WOLF:
			if (PieceOnBottomEdge() == true) {
			} else if (PieceOnRightEdge() == true) {
				if(gridSpaces[position + 7].GetGridSpaceStatus() != GridSpaceStatus.SHEEP) { possibleMoves.Add (position + 7); }
				if(gridSpaces[position - 9].GetGridSpaceStatus() != GridSpaceStatus.SHEEP) { possibleMoves.Add (position - 9); }
			} else if (PieceOnLeftEdge() == true) {
				if(gridSpaces[position - 7].GetGridSpaceStatus() != GridSpaceStatus.SHEEP) { possibleMoves.Add (position - 7); }
				if(gridSpaces[position + 9].GetGridSpaceStatus() != GridSpaceStatus.SHEEP) { possibleMoves.Add (position + 9); }
			} else {
				if(gridSpaces[position + 7].GetGridSpaceStatus() != GridSpaceStatus.SHEEP) { possibleMoves.Add (position + 7); }
				if(gridSpaces[position - 9].GetGridSpaceStatus() != GridSpaceStatus.SHEEP) { possibleMoves.Add (position - 9); }
				if(gridSpaces[position - 7].GetGridSpaceStatus() != GridSpaceStatus.SHEEP) { possibleMoves.Add (position - 7); }
				if(gridSpaces[position + 9].GetGridSpaceStatus() != GridSpaceStatus.SHEEP) { possibleMoves.Add (position + 9); }
			}
			break;
		default:
			break;
		}

		return this.possibleMoves;
	}

	// Check if the picked up piece was standing on the left edge
	public bool PieceOnLeftEdge() {
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
	public bool PieceOnRightEdge() {
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
	public bool PieceOnBottomEdge() {
		if (position > 56) {
			return true;
		} else {
			return false;
		}
	}
}

public class Wolf : Piece {
	new public int position;
	public int bestMove;

	public Wolf(int position) {
		this.position = position;
	}
}

public class Sheep : Piece {
	new public int position;
	public int bestMove;

	public Sheep(int position) {
		this.position = position;
	}
}

public class MiniMax {
	private List<GridSpace> gridSpaces;

	private List<Sheep> sheepList;
	private Wolf wolf;
	private Sheep bestSheep = null;

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

		foreach(Sheep sheep in sheepList) {
			Node sheepNode = GenerateTree(tree, 0, true, sheep, wolf, GridSpaceStatus.SHEEP);
			sheepNode.movePosition = sheep.position;

			// Generate all possible move paths for this sheep and the enemy wolf.
			// Terminal depth is 7, at which point a given sheep has reached the bottom and can no longer move.
			// Calculate which of these paths has the best value using the Minimax Algorithm.

			tree.children.Add(sheepNode);
		}

		// Compare the 4 sheeps best moves against eachother and pick the highest value move.
		// 

		gridSpaces [bestSheep.position].SetGridSpaceStatus(GridSpaceStatus.EMPTY);
		gridSpaces [bestSheep.bestMove].SetGridSpaceStatus(GridSpaceStatus.SHEEP);

		return gridSpaces;
	}

	public List<GridSpace> MoveWolf() {
		// Check what the best wolf move is.
		// Check which of the 2 possible moves brings the wolf closest to the finish point and farthest from the sheep/edge.
		// The wolf gets to make the move.
		// The move is based on possible moves of the sheep.

		gridSpaces [wolf.position].SetGridSpaceStatus(GridSpaceStatus.EMPTY);
		gridSpaces [wolf.bestMove].SetGridSpaceStatus(GridSpaceStatus.WOLF);

		return gridSpaces;
	}

	/*
	 * Minimax:
	 * 	The first move (the AI) takes the best possible move. The enemy then makes the worst possible move.
	 *  The set of moves with the highest result is the most optimal.
	 * 
	 *  This algorithm is used to minimize loss.
	 * */
	private int MinimaxAlgorithm(Node node, int depth, bool maximizingPlayer) {
		// If the depth is 0 (root) or the node is a terminal node
		if(depth == 0 || node.children.Count == 0) {
			return node.value;
		}

		if (maximizingPlayer) {
			int value = -1000;
			foreach(Node childNode in node.children) {
				value = Mathf.Max (value, MinimaxAlgorithm(childNode, depth -1, false));
			}
			return value;
		} else {
			int value = +1000;
			foreach(Node childNode in node.children) {
				value = Mathf.Min (value, MinimaxAlgorithm(childNode, depth -1, true));
			}
			return value;
		}
	}

	private Node GenerateTree(Node node, int depth, bool aiMove, Sheep sheep, Wolf wolf, GridSpaceStatus gridSpaceStatus) {
		if(depth <= 14) {
			return node;
		}

		switch(gridSpaceStatus) {
		case GridSpaceStatus.SHEEP:
			if (aiMove) {
				foreach(int possibleMove in sheep.GetPossibleMoves(gridSpaces, GridSpaceStatus.SHEEP, sheep.position)) {
					Node moveNode = new Node ();
					node.children.Add (GenerateTree(moveNode, depth+1, true, sheep, wolf, gridSpaceStatus));
				}
			} else {
				foreach(int possibleMove in wolf.GetPossibleMoves(gridSpaces, GridSpaceStatus.WOLF, wolf.position)) {
					Node moveNode = new Node ();
					node.children.Add (GenerateTree(moveNode, depth+1, false, sheep, wolf, gridSpaceStatus));
				}
			}
			break;
		case GridSpaceStatus.WOLF:
			if (aiMove) {
				foreach(int possibleMove in sheep.GetPossibleMoves(gridSpaces, GridSpaceStatus.WOLF, sheep.position)) {
					Node moveNode = new Node ();
					node.children.Add (GenerateTree(moveNode, depth+1, true, sheep, wolf, gridSpaceStatus));
				}
			} else {
				foreach(int possibleMove in wolf.GetPossibleMoves(gridSpaces, GridSpaceStatus.SHEEP, wolf.position)) {
					Node moveNode = new Node ();
					node.children.Add (GenerateTree(moveNode, depth+1, false, sheep, wolf, gridSpaceStatus));
				}
			}
			break;
		default:
			break;
		}
	}
}
