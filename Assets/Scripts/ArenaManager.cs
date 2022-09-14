using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("StartGame", 3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartGame()
    {
        //Instancia o player na cena
        GameObject Player = PhotonNetwork.Instantiate("TankPlayer", Vector3.zero, Quaternion.identity, 0);
        Player.GetComponent<TanksController>().tankCannon.enabled = false;
        Player.GetComponent<TanksController>().tankBase.enabled = false;
        //Captura o texto filho do player e muda para o nome do player.
        Text name = Player.GetComponentInChildren<Text>(); 
        name.text = Player.GetComponent<PhotonView>().Owner.NickName;

        SetPlayerPosition(Player, name);
            
    }

    public void SetPlayerPosition(GameObject pl, Text referencia)
    {
        switch (referencia.text[1])
        {
            case '1':
                pl.transform.position = spawnPoints[0].transform.position;
                break;
            case '2':
                pl.transform.position = spawnPoints[1].transform.position;
                break;
            case '3':
                pl.transform.position = spawnPoints[2].transform.position;
                break;
            case '4':
                pl.transform.position = spawnPoints[3].transform.position;
                break;
            case '5':
                pl.transform.position = spawnPoints[4].transform.position;
                break;
            case '6':
                pl.transform.position = spawnPoints[5].transform.position;
                break;
            case '7':
                pl.transform.position = spawnPoints[6].transform.position;
                break;
            case '8':
                pl.transform.position = spawnPoints[7].transform.position;
                break;
            default:
                break;
        }
    }
}
