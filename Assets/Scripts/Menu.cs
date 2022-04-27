using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Camera menuCamera;
    public Camera viewCamera;
    bool isOpen;
    float speed = 1;

    public void NewGame()
    {
        SceneManager.LoadScene(0);
        /*
        AsyncOperation op = SceneManager.LoadSceneAsync(0);
        op.completed += handle =>
        {
            foreach (GameObject g in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                print("find menu");
                Menu menu = g.GetComponent<Menu>();
                if (menu)
                {
                    print("menu");
                    menu.SetOpen(false);
                }
            }
        };
        */
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ResumeGame()
    {
        Time.timeScale = speed;
    }

    public void SetOpen(bool open)
    {
        isOpen = open;
        if (open)
        {
            Time.timeScale = 0;
            Cursor.visible = true;
            menuCamera.enabled = true;
            viewCamera.enabled = false;
        } else
        {
            Time.timeScale = speed;
            Cursor.visible = false;
            menuCamera.enabled = false;
            viewCamera.enabled = true;
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void Start()
    {
        SetOpen(true);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SetOpen(!isOpen);
        }
    }
}
