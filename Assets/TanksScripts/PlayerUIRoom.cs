using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerUIRoom : MonoBehaviour
{
    public PhotonView pview;
    public Text text;
    public Button ready;
    public bool bready;
    public Image readyToStart;
    public Image buttonColor;
    // Start is called before the first frame update
    void Start()
    {
        if(!pview.IsMine)
        {
            ready.interactable = false;
        }

        text.text = pview.Owner.NickName;
    }

    public void ReadyChange()
    {
        bready = true; ;
        pview.RPC("StatusChanged", RpcTarget.AllBufferedViaServer, bready);
    }
    [PunRPC]
    void StatusChanged(bool mybready)
    {
        print("entrou no status changed");
        bready = mybready;
        readyToStart.enabled = true;
        ready.interactable = false;
        buttonColor.color = Color.red;
    }
}
