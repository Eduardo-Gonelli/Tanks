using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IPunObservable
{
    public PhotonView pview;

    [SerializeField] public Text p1Text;
    [SerializeField] public Text p2Text;
    [SerializeField] public Text p3Text;
    [SerializeField] public Text p4Text;
    [SerializeField] public Text p5Text;
    [SerializeField] public Text p6Text;
    [SerializeField] public Text p7Text;
    [SerializeField] public Text p8Text;
    [SerializeField] public Text p1ScoreText;
    [SerializeField] public Text p2ScoreText;
    [SerializeField] public Text p3ScoreText;
    [SerializeField] public Text p4ScoreText;
    [SerializeField] public Text p5ScoreText;
    [SerializeField] public Text p6ScoreText;
    [SerializeField] public Text p7ScoreText;
    [SerializeField] public Text p8ScoreText;
    [SerializeField] public int score1 = 0;
    [SerializeField] public int score2 = 0;
    [SerializeField] public int score3 = 0;
    [SerializeField] public int score4 = 0;
    [SerializeField] public int score5 = 0;
    [SerializeField] public int score6 = 0;
    [SerializeField] public int score7 = 0;
    [SerializeField] public int score8 = 0;

    [SerializeField] public Text timer;
    [SerializeField] public Text readyGo;
    int tempo = 300;
    int exitTime = 6;
    public bool gameOver = false;
    void Start()
    {
        timer.text = tempo.ToString();
        StartCoroutine(ReadyGo());
        InvokeRepeating("SubtractTime", 4f, 1f);
    }

    IEnumerator ReadyGo()
    {
        yield return new WaitForSeconds(1f);
        readyGo.text = "Ready";
        yield return new WaitForSeconds(1f);
        readyGo.text = "Set";
        yield return new WaitForSeconds(1f);
        readyGo.text = "Go!";
        yield return new WaitForSeconds(1f);
        readyGo.text = "";

    }
    public void SubtractTime()
    {
        if(tempo > 0)
        {
            tempo--;
            timer.text = tempo.ToString("000");
        }
        else
        {
            CancelInvoke();
            CheckWinner();
        }
    }

    public void CheckWinner()
    {
        int maxScore = 0;
        string winner = "";
        if(maxScore < score1)
        {
            maxScore = score1;
            winner = p1Text.text;
        }
        if(maxScore < score2)
        {
            maxScore = score2;
            winner = p2Text.text;
        }
        if (maxScore < score3)
        {
            maxScore = score3;
            winner = p3Text.text;
        }
        if (maxScore < score4)
        {
            maxScore = score4;
            winner = p4Text.text;
        }
        if (maxScore < score5)
        {
            maxScore = score5;
            winner = p5Text.text;
        }
        if (maxScore < score6)
        {
            maxScore = score6;
            winner = p6Text.text;
        }
        if (maxScore < score7)
        {
            maxScore = score7;
            winner = p7Text.text;
        }
        if (maxScore < score8)
        {
            maxScore = score8;
            winner = p8Text.text;
        }
        readyGo.text = winner + "\nWins!\nScore: " + maxScore + "!";
        gameOver = true;
        InvokeRepeating("ExitRoom", 3, 1);
    }

    public void ExitRoom()
    {        
        exitTime--;
        if(exitTime == -1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
        timer.text = "Exiting room in " + exitTime + " seconds!";
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetScore(char jogador, int pontos)
    {
        switch(jogador)
        {
            case '1':
                score1 += pontos;
                p1ScoreText.text = "Score: " + score1.ToString();
                break;
            case '2':
                score2 += pontos;
                p2ScoreText.text = "Score: " + score2.ToString();
                break;
            case '3':
                score3 += pontos;
                p3ScoreText.text = "Score: " + score3.ToString();
                break;
            case '4':
                score4 += pontos;
                p4ScoreText.text = "Score: " + score4.ToString();
                break;
            case '5':
                score5 += pontos;
                p5ScoreText.text = "Score: " + score5.ToString();
                break;
            case '6':
                score6 += pontos;
                p6ScoreText.text = "Score: " + score6.ToString();
                break;
            case '7':
                score7 += pontos;
                p7ScoreText.text = "Score: " + score7.ToString();
                break;
            case '8':
                score8 += pontos;
                p8ScoreText.text = "Score: " + score8.ToString();
                break;
            default:
                break;
        }

    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(score1);
            stream.SendNext(score2);
            stream.SendNext(score3);
            stream.SendNext(score4);
            stream.SendNext(score5);
            stream.SendNext(score6);
            stream.SendNext(score7);
            stream.SendNext(score8);
            stream.SendNext(tempo);
        }
        else
        {
            score1 = (int)stream.ReceiveNext();
            score2 = (int)stream.ReceiveNext();
            score3 = (int)stream.ReceiveNext();
            score4 = (int)stream.ReceiveNext();
            score5 = (int)stream.ReceiveNext();
            score6 = (int)stream.ReceiveNext();
            score7 = (int)stream.ReceiveNext();
            score8 = (int)stream.ReceiveNext();
            tempo = (int)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void SetUINames(string name)
    {
        switch(name[1])
        {
            case '1':
                p1Text.text = name;
                p1ScoreText.text = "Score: ";
                break;
            case '2':
                p2Text.text = name;
                p2ScoreText.text = "Score: ";
                break;
            case '3':
                p3Text.text = name;
                p3ScoreText.text = "Score: ";
                break;
            case '4':
                p4Text.text = name;
                p4ScoreText.text = "Score: ";
                break;
            case '5':
                p5Text.text = name;
                p5ScoreText.text = "Score: ";
                break;
            case '6':
                p6Text.text = name;
                p6ScoreText.text = "Score: ";
                break;
            case '7':
                p7Text.text = name;
                p7ScoreText.text = "Score: ";
                break;
            case '8':
                p8Text.text = name;
                p8ScoreText.text = "Score: ";
                break;
        }
    }
}
