using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameStart : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI Player1;
    [SerializeField] TextMeshProUGUI Player2;

    void Update()
    {
        if (Input.GetKey("w") == true) 
        {
            Player1.text = "READY";
        }
        else if (Input.GetKeyUp("w") == true)
        {
            Player1.text = "HOLD W to start";
        }

        if (Input.GetKeyDown("up") == true) 
        {
            Player2.text = "READY";
        }
        else if (Input.GetKeyUp("up") == true) 
        {
            Player2.text = "HOLD UP to start";
        }


        if (Input.GetKey("w") == true & Input.GetKey("up") == true) 
        {
            SceneManager.LoadScene("GameScene");
        }


    }
}
