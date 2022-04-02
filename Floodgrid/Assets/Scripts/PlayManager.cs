using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class PlayManager : MonoBehaviour
{
    public int audioVolume;
	public TextMeshProUGUI audioDisplay;
	public bool pause;
	public GameObject pauseMenu;

	//Set this class to singleton
	public static PlayManager i {get{if(_i==null){_i = GameObject.FindObjectOfType<PlayManager>();}return _i;}} static PlayManager _i;

	void Awake()
	{
		//Set the audio volume as game manager
		audioVolume = GameManager.i.audioVolume;
		//Update the sound display
		audioDisplay.text = (audioVolume*10).ToString();
	}

	public void SoundModify(bool increase)
	{
		//Increase sound up to ten when reset to zero
		if(increase) {audioVolume++; if(audioVolume > 10) {audioVolume = 0;}}
		//Increase sound down to zero when reset to ten
		if(!increase) {audioVolume--; if(audioVolume < 0) {audioVolume = 10;}}
		//Update the audio counter display
		audioDisplay.text = (audioVolume*10).ToString();
		//Set the game manager audio volume
		GameManager.i.SetVolume(audioVolume);
	}

	public void PauseToggle()
	{
		//Toggle the pause menu
		pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
		//Toggle pause
		pause = !pause;
		//Toggle between time scale
		if(Time.timeScale == 1){Time.timeScale = 0;} else {Time.timeScale = 1;}
	}

    public void LoadSceneIndex(int i) {SceneManager.LoadScene(i, LoadSceneMode.Single);}
    public void ToQuit()  {Application.Quit();}
}
