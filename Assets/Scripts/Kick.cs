using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Kick : MonoBehaviour
{
    public PhotonView pball;
    Rigidbody ball;
    // Start is called before the first frame update
    void Start()
    {
        if(!pball.IsMine)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(ball && Input.GetButtonDown("Fire1"))
        {
            ball.AddForce(transform.forward * 10 + Vector3.up*5, ForceMode.Impulse);
        }
        if (ball && Input.GetButtonDown("Fire2"))
        {
            ball.AddForce(transform.forward * 5, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Rigidbody>())
        {
            ball = other.GetComponent<Rigidbody>();
            pball = other.GetComponent<PhotonView>();
            pball.RequestOwnership();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ball = null;
    }
}
