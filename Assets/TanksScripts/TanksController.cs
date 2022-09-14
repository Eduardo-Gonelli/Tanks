using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine.UI;
using Photon.Realtime;

public class TanksController : MonoBehaviour
{
 
    public float RotationSpeed = 90.0f;
    public float MovementSpeed = 2.0f;
    public float MaxSpeed = 0.2f;

    public ParticleSystem Destruction;
    public GameObject EngineTrail, EngineTrail2;
    private ParticleSystem engine1, engine2;
    public GameObject BulletPrefab;

    private PhotonView photonView;

    private new Rigidbody rigidbody;
    public GameObject spawnBulletPoint;
    private new Collider collider;
    private Animator anim;

    // Audio  settings
    public AudioSource audEffects;
    public AudioSource audEngine;
    public AudioClip shot;
    public AudioClip hurt;

    //private new Renderer renderer;
    public SpriteRenderer tankBase;
    public SpriteRenderer tankCannon;
    public Collider hurtCollider;

    private float rotation = 0.0f;
    private float acceleration = 0.0f;
    private float shootingTimer = 0.0f;

    private bool controllable = true;

    // HP Manager
    public Image hpSliderColor;
    private int HP = 100;
    private int HPLost = 0;
    public Slider hpSlider;
    public RectTransform hpSliderRect;
    public Text nomePlayer;

    private ArenaManager am;

    private GameManager gm;

    #region UNITY

    public void Awake()
    {
        photonView = GetComponent<PhotonView>();
        engine1 = EngineTrail.GetComponent<ParticleSystem>();
        engine2 = EngineTrail2.GetComponent<ParticleSystem>();
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        anim = GetComponentInChildren<Animator>();
        hpSliderRect = hpSlider.GetComponent<RectTransform>();
        am = GameObject.FindGameObjectWithTag("ArenaManager").GetComponent<ArenaManager>();
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        
    }

    [PunRPC]
    public void EnableView()
    {
        tankBase.enabled = true;
        tankCannon.enabled = true;
    }

    public void Start()
    {
        hpSliderColor.color = Color.green;
        hpSliderColor.color = new Color (hpSliderColor.color.r, hpSliderColor.color.g, hpSliderColor.color.b, 1f);
        photonView.RPC("StartName", RpcTarget.AllViaServer);
        gm.SetUINames(photonView.Owner.NickName);
        photonView.RPC("EnableView", RpcTarget.AllBufferedViaServer);
        
        /*
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.material.color = AsteroidsGame.GetColor(photonView.Owner.GetPlayerNumber());
        }
        */
    }

    public void Update()
    {
        hpSliderRect.transform.rotation = Quaternion.Euler(0, 0, -transform.rotation.z);
        if (!photonView.IsMine || !controllable)
        {
            return;
        }

        rotation = Input.GetAxis("Horizontal");
        acceleration = Input.GetAxis("Vertical");

        if (Input.GetButton("Jump") && shootingTimer <= 0.0)
        {
            shootingTimer = 0.5f;

            //photonView.RPC("Fire", RpcTarget.AllViaServer, rigidbody.position, rigidbody.rotation);
            photonView.RPC("Fire", RpcTarget.AllViaServer, spawnBulletPoint.transform.position, spawnBulletPoint.transform.rotation);
            audEffects.clip = shot;
            audEffects.Play();
        }

        if (shootingTimer > 0.0f)
        {
            shootingTimer -= Time.deltaTime;
        }
        if(gm.gameOver == true)
        {
            photonView.RPC("StopAllPlayers", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    public void StopAllPlayers()
    {
        controllable = false;
        audEngine.Stop();
        PhotonNetwork.Disconnect();
    }

    public void FixedUpdate()
    {
        
        if (!photonView.IsMine)
        {
            return;
        }

        if (!controllable)
        {
            return;
        }

        Quaternion rot = rigidbody.rotation * Quaternion.Euler(0, 0, -rotation * RotationSpeed * Time.fixedDeltaTime);
        rigidbody.MoveRotation(rot);
        
        Vector3 force = (rot * Vector3.up) * acceleration * 500.0f * MovementSpeed * Time.fixedDeltaTime;
        rigidbody.AddForce(force);

        if (rigidbody.velocity.magnitude > (MaxSpeed * 500.0f))
        {
            rigidbody.velocity = rigidbody.velocity.normalized * MaxSpeed * 500.0f;
        }

        if (rigidbody.velocity.magnitude > 0.5f)
        {
            if(engine1.emission.enabled == false)
            {
                engine1.enableEmission = true;
                engine2.enableEmission = true;
                anim.SetBool("walk", true);
            }
            
        }
        else
        {
            if(engine1.emission.enabled == true)
            {
                engine1.enableEmission = false;
                engine2.enableEmission = false;
                anim.SetBool("walk", false);
            } 
        }
        

        //Set audio pitch
        float pitchRange = 0.2f;
        float rigidBodyMangintude = rigidbody.velocity.magnitude;
        float pitch = mapValue(rigidBodyMangintude, 0f, 10f, 1.1f, 2f);
        audEngine.pitch = Random.Range(pitch - pitchRange, pitch + pitchRange);
        //Debug.Log("Pitch: " + pitch);
        //CheckExitScreen();
    }

    #endregion

    #region COROUTINES
    /*
    private IEnumerator WaitForRespawn()
    {
        yield return new WaitForSeconds(AsteroidsGame.PLAYER_RESPAWN_TIME);

        photonView.RPC("RespawnSpaceship", RpcTarget.AllViaServer);
    }
    */
    #endregion

    #region PUN CALLBACKS
    
    [PunRPC]
    public void DestroySpaceship()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        collider.enabled = false;
        hurtCollider.enabled = false;

        tankBase.enabled = false;
        tankCannon.enabled = false;

        controllable = false;

        EngineTrail.SetActive(false);
        EngineTrail2.SetActive(false);
        Destruction.Play();
        Invoke(nameof(PrepareToRespawn), 5f);
        /*
        if (photonView.IsMine)
        {
            object lives;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_LIVES, out lives))
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { AsteroidsGame.PLAYER_LIVES, ((int)lives <= 1) ? 0 : ((int)lives - 1) } });

                if (((int)lives) > 1)
                {
                    StartCoroutine("WaitForRespawn");
                }
            }
        }
        */
    }
    
    [PunRPC]
    public void Fire(Vector3 position, Quaternion rotation, PhotonMessageInfo info)
    {
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
        GameObject bullet;

        /** Use this if you want to fire one bullet at a time **/
        bullet = Instantiate(BulletPrefab, position, Quaternion.identity) as GameObject;
        bullet.GetComponent<TanksBullet>().InitializeBullet(photonView.Owner, (rotation * Vector3.up), Mathf.Abs(lag));


        /** Use this if you want to fire two bullets at once **/
        //Vector3 baseX = rotation * Vector3.right;
        //Vector3 baseZ = rotation * Vector3.forward;

        //Vector3 offsetLeft = -1.5f * baseX - 0.5f * baseZ;
        //Vector3 offsetRight = 1.5f * baseX - 0.5f * baseZ;

        //bullet = Instantiate(BulletPrefab, rigidbody.position + offsetLeft, Quaternion.identity) as GameObject;
        //bullet.GetComponent<Bullet>().InitializeBullet(photonView.Owner, baseZ, Mathf.Abs(lag));
        //bullet = Instantiate(BulletPrefab, rigidbody.position + offsetRight, Quaternion.identity) as GameObject;
        //bullet.GetComponent<Bullet>().InitializeBullet(photonView.Owner, baseZ, Mathf.Abs(lag));
    }

    private void PrepareToRespawn()
    {
        photonView.RPC("RespawnSpaceship", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void RespawnSpaceship()
    {
        am.SetPlayerPosition(this.gameObject, nomePlayer);
        collider.enabled = true;
        hurtCollider.enabled = true;

        tankBase.enabled = true;
        tankCannon.enabled = true;

        controllable = true;

        EngineTrail.SetActive(true);
        EngineTrail2.SetActive(true);
        HP = 100;
        hpSliderColor.color = Color.green;
        hpSlider.value = HP;
        Destruction.Stop();
    }

    public void CallDamage(Player lastAttacker)
    {
        photonView.RPC("Damage", RpcTarget.AllViaServer, lastAttacker);
    }


    [PunRPC]
    public void Damage(Player lAttacker)
    {
        print(photonView.Owner.NickName + " recebeu dano de" + lAttacker.NickName);
        HP -= 5;
        HPLost += 5;
        //Se HP e maior do que 50, so mexe no red.
        //Se HP e menor ou igual a 50, so mexe no green
        if(HP > 50)
            hpSliderColor.color = new Color(HPLost * 0.02f, 1 , 0, 1f);
        else
            hpSliderColor.color = new Color(1 , HP * 0.02f, 0, 1f);
        hpSlider.value = HP;
        gm.SetScore(lAttacker.NickName[1], 5);
        
        if(HP <= 0)
        {
            gm.SetScore(lAttacker.NickName[1], 100);
            photonView.RPC("DestroySpaceship", RpcTarget.AllViaServer);
        }
        /*
        if (photonView.IsMine)
        {
            
        }   */     
    }

    [PunRPC]

    public void StartName()
    {
        nomePlayer.text = photonView.Owner.NickName;
    }

    #endregion
    /*
    private void CheckExitScreen()
    {
        if (Camera.main == null)
        {
            return;
        }

        if (Mathf.Abs(rigidbody.position.x) > (Camera.main.orthographicSize * Camera.main.aspect))
        {
            rigidbody.position = new Vector3(-Mathf.Sign(rigidbody.position.x) * Camera.main.orthographicSize * Camera.main.aspect, 0, rigidbody.position.z);
            rigidbody.position -= rigidbody.position.normalized * 0.1f; // offset a little bit to avoid looping back & forth between the 2 edges 
        }

        if (Mathf.Abs(rigidbody.position.z) > Camera.main.orthographicSize)
        {
            rigidbody.position = new Vector3(rigidbody.position.x, rigidbody.position.y, -Mathf.Sign(rigidbody.position.z) * Camera.main.orthographicSize);
            rigidbody.position -= rigidbody.position.normalized * 0.1f; // offset a little bit to avoid looping back & forth between the 2 edges 
        }
    }
    */

    float mapValue(float mainValue, float inValueMin, float inValueMax, float outValueMin, float outValueMax)
    {
        return (mainValue - inValueMin) * (outValueMax - outValueMin) / (inValueMax - inValueMin) + outValueMin;
    }
}


