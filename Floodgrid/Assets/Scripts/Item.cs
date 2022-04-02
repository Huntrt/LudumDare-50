using UnityEngine;

public class Item : MonoBehaviour
{
	public enum Type {none, jump, bomb, freezeRay}
	public Type type;
	[TextArea(0,10)]
	public string Description;
}
