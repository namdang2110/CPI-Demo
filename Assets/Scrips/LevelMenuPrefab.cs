using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenuPrefab : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public int maxLevel;
    
    private void Start()
    {
        for (int i = 1; i <= maxLevel; i++)
        {
            GameObject button = Instantiate(buttonPrefab, buttonContainer);
            int levelNumber = i;
            button.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() => LoadLevel(levelNumber));
            button.GetComponentInChildren<TextMeshProUGUI>().text = levelNumber.ToString();
            
            if (i > PlayerPrefs.GetInt("UnlockedLevel", 1))
            {
                button.transform.GetChild(1).gameObject.SetActive(true);
                button.transform.GetChild(0).gameObject.SetActive(false);
                button.gameObject.GetComponent<Button>().interactable = false;
            }
        }
    }

    private void LoadLevel(int levelNumber)
    {
        string sceneName = "Man " + levelNumber;
        SceneManager.LoadScene(sceneName);
    }
}
