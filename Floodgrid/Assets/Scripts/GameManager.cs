using UnityEngine;

public class GameManager : MonoBehaviour
{
	public int audioVolume;
	public AudioSource sound;
	public static GameManager i;

	void Awake()
	{
		//Only don't destroy on load if haven't then destroy any duplicate
		if(i == null) {i = this; DontDestroyOnLoad(this);} else {Destroy(gameObject);}
		SetVolume(audioVolume);
	}

	public void SetVolume(int value)
	{
		//Get the value given then apply it to audio source
		audioVolume = value; sound.volume = (float)audioVolume/10;
	}
}
