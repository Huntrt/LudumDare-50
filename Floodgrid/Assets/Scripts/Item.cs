using UnityEngine;

public class Item : MonoBehaviour
{
	public enum Type {none, run, bomb, freezeRay, scubaTank, froze, teleport, dash, coin}
	public Type type;
	[TextArea(0,10)]
	public string Description;
}
