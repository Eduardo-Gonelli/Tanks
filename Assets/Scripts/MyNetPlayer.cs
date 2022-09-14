using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyNetPlayer : MonoBehaviour
{
    public PhotonView pview;
    public TextMesh myname;
    public Rigidbody rbd;
    Vector3 myInput;
    public Animator anim;

    public SkinnedMeshRenderer[] rends;
    // Start is called before the first frame update
    void Start()
    {
        myname.text = pview.Owner.NickName;
        if(pview.InstantiationId % 2 == 0)
        {
            foreach(SkinnedMeshRenderer s in rends)
            {
                s.material.SetColor("_Color", Color.red);
            }
            
        }
        
        
        if(pview.IsMine)
        {
            Camera.main.GetComponent<MyCam>().SetTarget(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        myname.transform.LookAt(Camera.main.transform); //Chama isso de billboard olhar o texto pra frente
        if (pview.IsMine)
        {
            myInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            
        }
        

    }

    private void FixedUpdate()
    {
        //Esse barato fica o personagem para saber se vai pra frente ou para tras.
        Vector3 localVelocity = transform.InverseTransformDirection(rbd.velocity);
        anim.SetFloat("SideVel", localVelocity.x);  
        anim.SetFloat("Velocity", localVelocity.z);
        if(pview.IsMine)
        {
            rbd.velocity = myInput * 6;
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100))
            {
                Vector3 myhit = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                transform.LookAt(myhit);
            }
        }
    }
}
