using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private int fightNum;
    private int moveNum;
    private int[] moves1 = new int[]{1, 1, 2, 1, 2};
    private int[] moves2 = new int[]{1, 3, 2, 3, 1, 1, 3, 2, 2, 2};
    private List<int> moves = new List<int>();
    private int[] fightLengths = new int[]{5, 10, 6};
    private string[] fightStart = new string[]{"\"Hey, this is my first fight, so go easy on me!\"", "Your opponent just stares at you.", "\"Good luck.\""};
    private string[] victory/*enemy victory*/ = new string[]{"\"Yay, I won!\"", "She leaves without a word.", "\"Good fight.\""};
    private string[] defeat/*enemy defeat*/ = new string[]{"\"Aww, that was mean!\"", "She leaves without a word.", "\"Impressive. Thank you.\""};
    private string[] pronouns = new string[]{"He", "She", "He"};

    private int playerScore;
    private TMPro.TextMeshProUGUI playerScoreTxt;
    private int enemyScore;
    private TMPro.TextMeshProUGUI enemyScoreTxt;
    
    [SerializeField] private Color32 winColor;
    [SerializeField] private Color32 loseColor;
    [SerializeField] private Color32 buttonWin;
    [SerializeField] private Color32 buttonLose;
    private Color32 buttonColor;

    private GameObject moveTxt;
    private GameObject fightTxt;
    private GameObject enemyTxt;
    private GameObject nextFight;
    private GameObject buttons;
    public GameObject moveBoxPrefab;
    private GameObject enemyBoxes;
    private GameObject playerBoxes;
    private TMPro.TextMeshProUGUI narrativeTxt;

    private bool wonLast; //whether enemy won the last round

    void Start()
    {
        moveTxt = GameObject.Find("Round Text");
        fightTxt = GameObject.Find("Fight Text");
        enemyTxt = GameObject.Find("Enemy Move");
        nextFight = GameObject.Find("Next Fight");
        buttons = GameObject.Find("Buttons");
        playerScoreTxt = GameObject.Find("Player Score").GetComponent<TMPro.TextMeshProUGUI>();
        enemyScoreTxt = GameObject.Find("Enemy Score").GetComponent<TMPro.TextMeshProUGUI>();
        enemyBoxes = GameObject.Find("Enemy Boxes");
        playerBoxes = GameObject.Find("Player Boxes");
        narrativeTxt = GameObject.Find("Narrative Text").GetComponent<TMPro.TextMeshProUGUI>();
        Restart(false);
    }

    public void Restart(bool progress)
    {
        StartCoroutine(RestartCor(progress));
    }

    private IEnumerator RestartCor(bool progress)
    {
        nextFight.SetActive(false);
        if (moveNum == 0)
        {
            GameObject.Find("Fader").GetComponent<Animator>().Play("FadeIn");
            yield return null;
        }
        else
        {
            moveNum = 0;
            GameObject.Find("Fader").GetComponent<Animator>().Play("FullFade");
            yield return new WaitForSeconds(0.5f);
        }
        //When you move to the next fight
        if (progress)
        {
            fightNum++;
            fightTxt.GetComponent<TMPro.TextMeshProUGUI>().text = "Fight:  " + (fightNum+1);
        }
        //Recreate move boxes
        foreach (Transform child in playerBoxes.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in enemyBoxes.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < fightLengths[fightNum]; i++)
        {
            GameObject playerObj = Instantiate(moveBoxPrefab, Vector3.zero, Quaternion.identity, playerBoxes.transform);
            playerObj.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 114-28*i, 0);
            GameObject enemyObj = Instantiate(moveBoxPrefab, Vector3.zero, Quaternion.identity, enemyBoxes.transform);
            enemyObj.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 114-28*i, 0);
        }
        //Reset everything else
        moves = new List<int>();
        foreach (Transform child in buttons.transform)
        {
            child.GetComponent<Button>().interactable = true;
            child.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        playerScore = 0;
        playerScoreTxt.text = "" + playerScore;
        enemyScore = 0;
        enemyScoreTxt.text = "" + enemyScore;
        moveTxt.GetComponent<TMPro.TextMeshProUGUI>().text = "Move:  " + 1 + " / " + fightLengths[fightNum];
        enemyTxt.GetComponent<CanvasGroup>().alpha = 0;
        StartCoroutine(PlayDialogue(fightStart[fightNum]));
    }

    private void EnemyMove()
    {
        if (fightNum == 0)
        {
            moves.Add(moves1[moveNum]);
        }
        else if (fightNum == 1)
        {
            moves.Add(moves2[moveNum]);
        }
        else if (fightNum == 2)
        {
            if (wonLast || moveNum == 0)
                moves.Add(1);
            else
                moves.Add(3);
        }
    }

    public void ChooseMove(int n)
    {
        EnemyMove();
        if (n%3 + 1 == moves[moveNum])
        {
            wonLast = false;
            playerScore++;
            playerScoreTxt.text = "" + playerScore;
            buttonColor = buttonWin;
            playerBoxes.transform.GetChild(moveNum).GetChild(1).GetComponent<Image>().color = winColor;
            enemyBoxes.transform.GetChild(moveNum).GetChild(1).GetComponent<Image>().color = loseColor;
        }
        else
        {
            wonLast = true;
            enemyScore++;
            enemyScoreTxt.text = "" + enemyScore;
            buttonColor = buttonLose;
            playerBoxes.transform.GetChild(moveNum).GetChild(1).GetComponent<Image>().color = loseColor;
            enemyBoxes.transform.GetChild(moveNum).GetChild(1).GetComponent<Image>().color = winColor;
        }
        playerBoxes.transform.GetChild(moveNum).GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = IntToChar(n);
        enemyBoxes.transform.GetChild(moveNum).GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = IntToChar(moves[moveNum]);
        moveNum++;
        StartCoroutine(ShowResult(n));
    }

    private IEnumerator ShowResult(int n)
    {
        foreach (Transform child in buttons.transform)
        {
            child.GetComponent<Button>().interactable = false;
        }
        enemyTxt.GetComponent<TMPro.TextMeshProUGUI>().text = pronouns[fightNum] + " used:  <b><color=#97EEFF>" + IntToStr(moves[moveNum-1]) + "</color></b>";
        enemyTxt.GetComponent<CanvasGroup>().alpha = 1;
        for (float i = 0; i < 1; i += 0.01f)
        {
            enemyTxt.GetComponent<CanvasGroup>().alpha = 1-i;
            buttons.transform.GetChild(n-1).GetComponent<Image>().color = Color.Lerp(buttonColor, Color.white, i);
            yield return new WaitForSeconds(0.01f);
        }
        enemyTxt.GetComponent<CanvasGroup>().alpha = 0;
        if (moveNum < fightLengths[fightNum])
        {
            moveTxt.GetComponent<TMPro.TextMeshProUGUI>().text = "Move:  " + (moveNum+1) + " / " + fightLengths[fightNum];
            foreach (Transform child in buttons.transform)
            {
                child.GetComponent<Button>().interactable = true;
                child.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
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
        if (playerScore > enemyScore)
        {
            enemyTxt.GetComponent<TMPro.TextMeshProUGUI>().text = "<b>You  Won!</b>";
            StartCoroutine(PlayDialogue(defeat[fightNum]));
            nextFight.SetActive(true);
        }
        else
        {
            enemyTxt.GetComponent<TMPro.TextMeshProUGUI>().text = "<b>You  Lost</b>";
            StartCoroutine(PlayDialogue(victory[fightNum]));
        }
        enemyTxt.GetComponent<CanvasGroup>().alpha = 1;
    }

    private IEnumerator PlayDialogue(string line)
    {
        narrativeTxt.text = "";
        foreach (char c in line)
        {
            if (c != '*')
                narrativeTxt.text += c;

            if (c == ' ')
                yield return new WaitForSeconds(0.05f);
            else if (c == '.')
                yield return new WaitForSeconds(0.2f);
            else if (c == ',')
                yield return new WaitForSeconds(0.15f);
            else if (c == '*')
                yield return new WaitForSeconds(0.1f);
            else if (line.Length > 30)
                yield return new WaitForSeconds(0.075f);
            else
                yield return new WaitForSeconds(0.1f);
        }
    }
}
