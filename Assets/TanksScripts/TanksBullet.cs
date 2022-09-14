using UnityEngine;
using Photon.Realtime;
public class TanksBullet : MonoBehaviour
{

    public Player Owner { get; private set; }

    public GameObject prt;
    public void Start()
    {
        Destroy(gameObject, 5.0f);
    }

    public void OnCollisionEnter(Collision collision)
    {
        //print("Colidiu municao");
        if(collision.gameObject.tag == "Player")
        {
            
            
            collision.gameObject.GetComponentInParent<TanksController>().CallDamage(Owner);
        }
        Instantiate(prt, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }


    public void InitializeBullet(Player owner, Vector3 originalDirection, float lag)
    {
        Owner = owner;

        transform.up = originalDirection;

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = originalDirection * 10.0f;
        rigidbody.position += rigidbody.velocity * lag;
    }

}
