using UnityEngine;
using System.Collections;

public class ChessButton : MonoBehaviour {
	/// <summary>
	/// The chess _index of this button.
	/// </summary>
	public int _index;
	public GameController _gameController;

	/// <summary>
	/// Player clicked on the board.
	/// Check if it's player's valid turn.
	/// then try to place a chess at corresponding cell.
	/// </summary>
	void OnMouseUp () {
		if (_gameController.IsPlayerTurn()) {
			_gameController.Place (_index);
		}
	}
}
