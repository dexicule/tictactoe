using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
	public Texture _tableTexture;
	public Texture _menuTexture;
	public Texture _spTexture;
	public Texture _resultTexture;
	public GameObject _surface;

	public GameObject _mainMenu;
	public GameObject _spMenu;
	public GameObject _table;
	public GameObject _resultMenu;

	public TextMesh _result;

	// Use this for initialization
	void Start () {
		DisplayMainMenu ();
	}

	private void disableAllViews() {
		_result.text = "";
		_mainMenu.SetActive (false);
		_spMenu.SetActive (false);
		_table.SetActive (false);
		_resultMenu.SetActive (false);
	}

	public void DisplayMainMenu() {
		_surface.renderer.material.mainTexture = _menuTexture;
		disableAllViews();
		_mainMenu.SetActive (true);
	}

	public void DisplaySpMenu() {
		_surface.renderer.material.mainTexture = _spTexture;
		disableAllViews();
		_spMenu.SetActive (true);
	}

	public void DisplayTable() {
		_surface.renderer.material.mainTexture = _tableTexture;
		disableAllViews();
		_table.SetActive (true);
	}

	public void DisplayResult(GameController.State state) {
		_surface.renderer.material.mainTexture = _resultTexture;
		disableAllViews();
		_resultMenu.SetActive (true);
		if (state == GameController.State.DRAW) {
			_result.text = "Draw !";
		} else if (state == GameController.State.X_WIN) {
			_result.text = "Player 1 Wins!";
		} else if (state == GameController.State.O_WIN) {
			_result.text = "Player 2 Wins!";
		}
	}
}
