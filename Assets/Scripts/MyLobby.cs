using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Linq;
using System.Diagnostics.Tracing;

public class MyLobby : MonoBehaviourPunCallbacks
{
    public InputField nameField;
    public GameObject roomPanel;
    public GameObject nickNamePanel;
    private string field;
    public GameObject gameRoomContent;
    public List<PlayerUIRoom> playerUIRooms;
    public PhotonView pview;
    public GameObject joinRoomButton;
    public GameObject startMatchButton;
    public GameObject waitingforMaster;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        InvokeRepeating("CheckAllReady", 1, 1);
    }

    public void PlayGame()
    {
        field = nameField.text; //captura o nome que esta no input field
        if(string.IsNullOrWhiteSpace(field))
        {
            //field = "Fulano" + Guid.NewGuid();
            //field = "Player " + PhotonNetwork.LocalPlayer.GetPlayerNumber();
            field = "";
            //Esse guid.newguid cria um identificador unico, que nao se repete
        }
        PhotonNetwork.LocalPlayer.NickName = "P0 "+field; //cria um local player com o nome
        PhotonNetwork.ConnectUsingSettings(); //realiza a conexao
    }

    bool temAlguemNaSala = false;
    public void JoinRoom()
    {
        joinRoomButton.SetActive(false);
        PhotonNetwork.JoinRandomRoom();
        temAlguemNaSala = true;
        CheckAllReady();
    }

    public override void OnConnectedToMaster()
    {
        //print("Conectado!");
        roomPanel.SetActive(true); //Ativa o painel se estiver desativado
        nickNamePanel.SetActive(false);
    }
       

    public override void OnJoinedRoom()
    {
        //print("Conectado na room: " + PhotonNetwork.CurrentRoom.Name);
        
        //PhotonNetwork.LoadLevel("3_TankArena");

        //Esse trecho verifica se nao foi dado nome ao player
        //Se nao foi, ele colocara Player + Numero do Player.
        //Nao funciona se um player sair e entrar novamente.
        //Pode gerar nomes repetidos. Verificar formas de contornar o problema.
        if(PhotonNetwork.LocalPlayer.NickName[1] == '0')
        {
            bool nameAccepted = false;
            bool nameAlreadyExist = false;
            int control = 1;
            int playerList = PhotonNetwork.PlayerList.Length;
            while(!nameAccepted)
            {
                for (int i = 0; i < playerList; i++)
                {
                    if (PhotonNetwork.PlayerList[i].NickName[1] == control.ToString()[0])
                    {
                        //O nome ja existe e nao pode ser usado
                        nameAlreadyExist = true;
                        print("O nome" + "P" + control + " já existe");
                        break;
                    }
                }
                if(nameAlreadyExist)
                {
                    control++;
                    nameAlreadyExist = false;
                }
                else
                {
                    //O nome nao existe, entao esta disponivel
                    nameAccepted = true;                    
                    PhotonNetwork.LocalPlayer.NickName = "P" + control +" " + field;
                    print("Criando o nome" + PhotonNetwork.LocalPlayer.NickName);
                    control = 1;
                }
            }            
        }
        Invoke("FixName", 0);
        GameObject ob = PhotonNetwork.Instantiate("PlayerUIRoom", gameRoomContent.transform.position, Quaternion.identity);
        ob.transform.SetParent(gameRoomContent.transform, false);

        waitingforMaster.SetActive(true);
        waitingforMaster.GetComponent<Text>().text = "Waiting for all Player to be Ready!";
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        print("other entered");
        Invoke("FixName", 2);
    }

    void FixName()
    {
        print("searching " + gameRoomContent.name);
        GameObject[] obs = GameObject.FindGameObjectsWithTag("UIPlayer");
        foreach (GameObject ob in obs)
        {
            ob.transform.SetParent(gameRoomContent.transform, false);
            print("founded");
            if (!playerUIRooms.Contains(ob.GetComponent<PlayerUIRoom>()))
            {
                playerUIRooms.Add(ob.GetComponent<PlayerUIRoom>());
            }
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("Não tem nenhuma sala, criando uma...\n"); //Debug para informar que esta criando a sala
        string nomeSala = "Sala" + Guid.NewGuid(); //Cria uma string com o valor Sala0 a Sala999
        RoomOptions op = new RoomOptions(); //Gerencia opcoes da sala
        op.MaxPlayers = 8;
        PhotonNetwork.CreateRoom(nomeSala, op, null); //Conecta a sala com o nome e as opcoes
        print("Nome da sala: " + nomeSala);
    }

    string mestreDaSala = "";
    void CheckAllReady()
    {
        if (!temAlguemNaSala)
            return;
        if (PhotonNetwork.LocalPlayer.NickName[1] == '1')
            mestreDaSala = PhotonNetwork.LocalPlayer.NickName;
        
        bool allready = false;        
        if (playerUIRooms.Count > 1)
        {
            allready = playerUIRooms.All(x => x.bready);//Soluçao LucasTeles

            if (allready)
            {
                //Aqui eu quero que apareça o botão de iniciar a partida
                //para o player P1. Se não existir o P1, o P2 inicia
                //E assim por diante.
                if(pview.IsMine)
                {
                    if(PhotonNetwork.LocalPlayer.NickName[1] == '1')
                    {
                        startMatchButton.SetActive(true);
                        waitingforMaster.SetActive(false);
                    }
                }
                else if (PhotonNetwork.LocalPlayer.NickName[1] != '1')
                {
                    waitingforMaster.SetActive(true);
                    waitingforMaster.GetComponent<Text>().text = "Waiting for the room master to start the match!";
                }
            }
            else
            {
                //waitingforMaster.SetActive(false);
                waitingforMaster.GetComponent<Text>().text = "Waiting for all Players to be Ready!";
                startMatchButton.SetActive(false);
            }
        }
    }

    public void StartMatch()
    {
        //Esse broadcast só pode ser chamado pelo p1.
        pview.RPC("BroadcastLoadScene", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void BroadcastLoadScene()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("3_TankArena");
    }

    public void SairDoJogo()
    {
        Application.Quit();
    }
}
