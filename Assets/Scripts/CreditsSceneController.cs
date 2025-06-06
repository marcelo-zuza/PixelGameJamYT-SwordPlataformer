using System.Collections;
using System.Collections.Generic;
// using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsSceneController : MonoBehaviour
{
    [SerializeField] private GameObject pressEnterWindow;
    [SerializeField] float lockScreenTime = 2f;
    private bool screenLocked = true;

    void Start()
    {
        StartCoroutine(LockScreen());
    }

    // Update is called once per frame
    void Update()
    {
        RestartGame();
    }

    IEnumerator LockScreen()
    {
        yield return new WaitForSeconds(lockScreenTime);
        screenLocked = false;
        pressEnterWindow.SetActive(true);
    }

    void RestartGame()
    {
        if (!screenLocked)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene(0);
            }
        }
    }
}
