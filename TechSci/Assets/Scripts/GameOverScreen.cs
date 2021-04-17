using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            SceneManager.LoadScene("TheEnd");
        }

        else if(collision.gameObject.name == "Player 2")
        {
            SceneManager.LoadScene("Win2");
        }
    }
}
