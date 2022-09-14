using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCam : MonoBehaviour
{
    public GameObject target;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(target)
        {
            transform.LookAt(target.transform);
            transform.position = target.transform.position + offset;
        }
        
    }

    public void SetTarget(GameObject ptarget)
    {
        target = ptarget;
    }
}
