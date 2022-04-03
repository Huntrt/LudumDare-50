using UnityEngine;

public class SoundClipStorage : MonoBehaviour
{
	public AudioClip move, swim, pickup, run, scuba, teleport, die, dash, freeze, explosion;

	public static SoundClipStorage i {get{if(_i==null){_i = GameObject.FindObjectOfType<SoundClipStorage>();}return _i;}} static SoundClipStorage _i;
}
