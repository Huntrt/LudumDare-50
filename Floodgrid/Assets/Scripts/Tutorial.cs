using UnityEngine;

public class Tutorial : MonoBehaviour
{
	[SerializeField] GameObject tutorialPanel;
    [SerializeField] GameObject[] panels;
	int currentPanel;

	void Start()
	{
		if(!GameManager.i.completeTutorial)
		{
			NextTutorial();
			tutorialPanel.SetActive(true);
		}
	}

	public void NextTutorial()
	{
		if(currentPanel == panels.Length) 
		{
			GameManager.i.completeTutorial = true;
			tutorialPanel.SetActive(false);
			return;
		}
		for (int p = 0; p < panels.Length; p++) {panels[p].SetActive(false);}
		panels[currentPanel].SetActive(true);
		currentPanel++;
	}
}