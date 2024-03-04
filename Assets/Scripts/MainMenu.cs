using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject lobbyMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private Button playButton;
    void Update()
    {
        if (lobbyMenu.activeSelf == true)
        {
        }
        if (optionsMenu.activeSelf == true)
        {
            if (Input.GetKeyDown("escape") || Input.GetKeyDown(KeyCode.JoystickButton1))
            {
                playButton.Select();
                lobbyMenu.SetActive(true);
                optionsMenu.SetActive(false);
            }
        }

    }
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
