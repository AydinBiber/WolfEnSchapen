using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

	public bool isRoot = false;
	public int value = 0;
	public int movePosition;
	public List<Node> children = new List<Node>();

}
