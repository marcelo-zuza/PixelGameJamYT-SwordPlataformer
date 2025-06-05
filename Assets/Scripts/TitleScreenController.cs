using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenController : MonoBehaviour
{
    [SerializeField] GameObject logoScreen;
    [SerializeField] float fadeSpeed = 5f;
    [SerializeField] float logoExibitionTime = 3f;
    private Button startButton;
    [SerializeField] float blockTime = 1f;
    private bool screenLocked = true;

    private bool isFading = false;
    void Start()
    {
        StartCoroutine(FaseStart());
        StartCoroutine(LockScreen());

    }

    void Update()
    {
        StartGame();
    }

    IEnumerator LockScreen()
    {
        yield return new WaitForSeconds(blockTime);
        screenLocked = false;
    }

    IEnumerator FaseStart()
    {
        yield return new WaitForSeconds(logoExibitionTime);
        CanvasGroup canvasGroup = logoScreen.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("O objeto " + logoScreen.name + " não tem um componente CanvasGroup anexado.");
            yield break;
        }
        canvasGroup.alpha = 1;
        isFading = true;
        while (isFading)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            if (canvasGroup.alpha <= 0)
            {
                isFading = false;
            }
            yield return null;
        }
    }
    
    void StartGame()
    {
        if (!screenLocked)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene(1);
            }
        }
    }
}
