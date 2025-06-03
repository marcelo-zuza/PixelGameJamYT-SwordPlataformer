using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private GameObject[] hearts;
    [SerializeField] private PlayerController playerController;

    void Start()
    {

    }

    void Update()
    {
        UpdateHeartsUI();
    }

    void UpdateHeartsUI()
    {
        float currentLives = playerController.playerLives;
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentLives)
            {
                hearts[i].SetActive(true);
            }
            else
            {
                hearts[i].SetActive(false);
            }
        }
    }
}
