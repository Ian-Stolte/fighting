using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int moveNum;
    private int[] moves = new int[]{1, 3, 2, 3, 1, 1, 3, 2, 2, 2};

    private GameObject moveTxt;
    private GameObject enemyTxt;
    private GameObject enemyBoxes;
    private GameObject playerBoxes;

    void Start()
    {
        moveNum = 0;
        moveTxt = GameObject.Find("Round Text");
        enemyTxt = GameObject.Find("Enemy Move");
        enemyTxt.GetComponent<TMPro.TextMeshProUGUI>().text = "She is using: <color=green>" + IntToStr(moves[0]) + "</color>";
        enemyBoxes = GameObject.Find("EnemyBoxes");
        playerBoxes = GameObject.Find("PlayerBoxes");
        foreach (Transform child in enemyBoxes.transform)
        {
            child.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = "";
        }
        foreach (Transform child in playerBoxes.transform)
        {
            child.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = "";
        }

    }

    public void ChooseMove(int n)
    {
        if (n%3 + 1 == moves[moveNum])
        {
            Debug.Log("You win!");
        }
        else
        {
            Debug.Log("You lose!");
        }
        playerBoxes.transform.GetChild(moveNum).GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = IntToChar(n);
        enemyBoxes.transform.GetChild(moveNum).GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = IntToChar(moves[moveNum]);
        moveNum++;
        if (moveNum < 10)
        {
            moveTxt.GetComponent<TMPro.TextMeshProUGUI>().text = "Move: " + (moveNum+1);
            enemyTxt.GetComponent<TMPro.TextMeshProUGUI>().text = "She is using: <color=green>" + IntToStr(moves[moveNum]) + "</color>";
        }
        else
        {
            EndGame();
        }
    }

    private string IntToChar(int n)
    {
        if (n == 1)
        {
            return "P";
        }
        else if (n == 2)
        {
            return "K";
        }
        else
        {
            return "B";
        }
    }

    private string IntToStr(int n)
    {
        if (n == 1)
        {
            return "punch";
        }
        else if (n == 2)
        {
            return "kick";
        }
        else
        {
            return "block";
        }
    }

    private void EndGame()
    {
        enemyTxt.GetComponent<TMPro.TextMeshProUGUI>().text = "Round Over!";
        moveTxt.SetActive(false);
    }
}
