using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GridSpaceStatus {
	EMPTY, 
	WOLF,
	SHEEP
}

public enum CursorStatus {
	EMPTY,
	OCCUPIED
}

public class Player : MonoBehaviour {

	public GridSpaceStatus playerRole = GridSpaceStatus.WOLF;

	void Awake() {
		DontDestroyOnLoad (this);
	}

}
