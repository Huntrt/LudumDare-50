using UnityEngine;

public class PanelToggle : MonoBehaviour
{
    [SerializeField] GameObject panel; public void Toggle() => panel.SetActive(!panel.activeInHierarchy);
}
