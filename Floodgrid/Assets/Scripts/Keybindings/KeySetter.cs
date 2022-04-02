using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class KeySetter : MonoBehaviour
{
    public string action;
	public TextMeshProUGUI keyDisplay;
	[SerializeField] Button button;
	[Tooltip("Will auto: \n- Get this object name as action\n- Get button component of object it on \n- Get the 0 child text as key display")]
	[SerializeField] bool autoSetup; bool hasSetup;
	[SerializeField] int displayIndex;
	KeybinderSystem binder;

	void OnValidate() 
	{
		//If enable auto setup while haven't setup
		if(autoSetup && !hasSetup)
		{
			//The object name are it action
			action = gameObject.name;
			//Get the button component of this object
			button = GetComponent<Button>();
			//Get the key display from the child of index given on this object
			keyDisplay = transform.GetChild(displayIndex).GetComponent<TextMeshProUGUI>();
			//Has complete setup
			hasSetup = true;
		}
		//Clear all the button, display and action if disable auto setup while has setup
		if(!autoSetup && hasSetup) {action = ""; button = null; keyDisplay = null; hasSetup = false;}
	}

	void Start()
	{
		//Get the keybinder
		binder = KeybinderSystem.i;
		//Start assign this setter when it button got click 
		button.onClick.AddListener(delegate {binder.StartAssign(this);});
		///If there NO keycode variable that has the same name as this setter's action
		if(binder.GetType().GetField(action) == null)
		{
			//Print an error
			Debug.LogError("There are no keycode variable named '" + action + "' in keybinder");
		}
		///If there IS keycode variable that has the same name as this setter's action
		else
		{
			//Display it keycode
			keyDisplay.text = binder.GetType().GetField(action).GetValue(binder).ToString();
		}
	}
}
