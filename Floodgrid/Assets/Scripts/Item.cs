using UnityEngine;

public class Item : MonoBehaviour
{
	public enum Type {none, run, bomb, freezeRay}
	public Type type;
	[TextArea(0,10)]
	public string Description;
}
