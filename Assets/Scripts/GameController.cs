using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	public enum Cell {
		EMPTY = 0,
		X = -1,
		O = 1,
	};

	public enum State {
		// Menu
		MAIN_MENU = -2,
		// Single player menu
		SP_MENU = -1,
		// X/O's turn
		X = 0,
		O = 1,
		// start as X, win/lose as X
		X_WIN = 2,
		O_WIN = 3,
		DRAW = 4,
	};

	public enum Mode {
		// Single player, play X:
		X = 0,
		// Single player, play O:
		O = 1,

		// hot seat
		DOUBLE = 2,
	}

	// Game play mode, current state and board state:
	public Mode GameMode = Mode.X;
	public State GameState = State.X;
	private Cell[] _board = null;	

	// Game prefab/object in the asset/scene:
	public Menu _menu;
	public GameObject _player1_prefab;
	public GameObject _player2_prefab;
	public Transform _boardObject;
	public Transform[] _chessButtons = new Transform[9];
	private GameObject[] _chessObjects = new GameObject[9];

	// AI Player
	private AI _ai = null;

	/// <summary>
	/// Gets or sets the ai instance.
	/// </summary>
	/// <value>The ai.</value>
	public AI ai {
		get { 
			if (_ai == null) {
				_ai = new AI();
			} 
			return _ai;
		}

		set {
			_ai = value;
		}
	}

	/// <summary>
	/// Gets the _ai chess type.
	/// </summary>
	/// <value>The _ai chess type.</value>
	private Cell _aiChess {
		get {
			return GameMode == Mode.O ? Cell.X : Cell.O;
		}
	}

	/// <summary>
	/// Place a chess at specified cell.
	/// </summary>
	/// <param name="index">Index.</param>
	/// <returns>bool: placement success/fail</returns>
	public bool Place(int index) {
		// Not empty:
		if (_board[index] != Cell.EMPTY) {
			return false;
		}

		// Game in progress:
		if (GameState == State.X || GameState == State.O) {
			_board[index] = GameState == State.X ? Cell.X : Cell.O;
			GameObject prefab = GameState == State.X ? _player1_prefab : _player2_prefab;
			if (GameMode != Mode.DOUBLE) {
				prefab = IsAiTurn() ? _player2_prefab : _player1_prefab;
			}
			_chessObjects[index] = (GameObject)GameObject.Instantiate(prefab);
			// Place at the same location:
			_chessObjects[index].transform.parent = _boardObject;
			_chessObjects[index].transform.localPosition = _chessButtons[index].localPosition;
			advanceGame();
			return true;
		}

		// Game is over:
		return false;
	}

	/// <summary>
	/// Determines if it's player's turn or not.
	/// </summary>
	/// <returns><c>true</c> if it's player turn; otherwise, <c>false</c>.</returns>
	public bool IsPlayerTurn(){
		return (GameMode == Mode.X && GameState == State.X) 
			|| (GameMode == Mode.O && GameState == State.O) 
			|| GameMode == Mode.DOUBLE;
	}

	/// <summary>
	/// Determines if it's AI's turn or not.
	/// </summary>
	/// <returns><c>true</c> if it's AI's turn; otherwise, <c>false</c>.</returns>
	public bool IsAiTurn() {
		return (GameMode != Mode.DOUBLE) && 
			((GameMode == Mode.X && GameState == State.O) 
			 || (GameMode == Mode.O && GameState == State.X));
	}
	
	/// <summary>
	/// Starts the game.
	/// </summary>
	private void startGame() {
		_board = new Cell[9];
		_menu.DisplayTable();
		clearTable();
		GameState = State.X;
		if (IsAiTurn()){
			Place(ai.BestMove(_board, _aiChess));
		}
	}

	/// <summary>
	/// Clears the table.
	/// </summary>
	private void clearTable() {
		foreach (GameObject go in _chessObjects) {
			GameObject.Destroy(go);
		}
	}

	/// <summary>
	/// Handles the menu button event.
	/// </summary>
	/// <param name="button">Button.</param>
	public void HandleButtonEvent(string button) {
		if (button == "SinglePlayer") {
			_menu.DisplaySpMenu ();
		} else if (button == "HotSeat") {
			GameMode = Mode.DOUBLE;
			startGame();
		} else if (button == "PlayFirst") {
			GameMode = Mode.X;
			startGame();
		} else if (button == "PlaySecond") {
			GameMode = Mode.O;
			startGame();
		} else if (button == "Again") {
			startGame();
		} else if (button == "Menu") {
			clearTable();
			GameState = State.MAIN_MENU;
			_menu.DisplayMainMenu();
		}
	}

	/// <summary>
	/// Advances the state of the game after each Placement.
	///   Handle AI's turn here
	/// </summary>
	private void advanceGame() {
		int state = ai.EvaluateBoard(_board);
		if (state == AI.DRAW) {
			GameState = State.DRAW;
			_menu.DisplayResult(GameState);
			return;
		} else if (state == AI.X_WIN) {
			GameState = State.X_WIN;
			_menu.DisplayResult(GameState);
			return;
		} else if (state == AI.O_WIN) {
			GameState = State.O_WIN;
			_menu.DisplayResult(GameState);
			return;
		}

		// Advance State:
		if (GameState == State.X) {
			GameState = State.O;
		} else if (GameState == State.O) {
			GameState = State.X;
		}

		// Single player, AI's round:
		if (IsAiTurn()) {
			Place(ai.BestMove(_board, _aiChess));
		}
	}
}


