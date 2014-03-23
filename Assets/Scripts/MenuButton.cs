using UnityEngine;
using System.Collections;

public class MenuButton : MonoBehaviour {
	public GameController _gameController;
	public string _buttonName;

	/// <summary>
	/// Player clicked on the board.
	/// Check if it's player's valid turn.
	/// then try to place a chess at corresponding cell.
	/// </summary>
	void OnMouseUp () {
		_gameController.HandleButtonEvent(_buttonName);
	}
}
