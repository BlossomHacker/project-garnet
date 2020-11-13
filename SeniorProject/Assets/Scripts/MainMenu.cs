using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public void PlayGame () {
		SceneManager.LoadScene("Game_field_2");
	}

	public void QuitGame () {
		Application.Quit();
	}
}

