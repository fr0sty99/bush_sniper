using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;


// this class is responsive for the player networking and effects
public class Player : NetworkMessageHandler
{
    private const string PLAYER_TAG = "player";

    [Header("Player Properties")]
    public string playerID;

    [Header("Player Movement Properties")]
    public bool canSendNetworkMovement;
    public float speed;
    public float networkSendRate = 5;
    public float timeBetweenMovementStart;
    public float timeBetweenMovementEnd;
    public PlayerMotor motor;

    [Header("Player Shooting Properties")]
    [SerializeField]
    public PlayerWeapon weapon;
    private float timeToFire = 0;
    [SerializeField]
    private LayerMask mask;

    [Header("Camera Properties")]
    public Camera followCam;

    [Header("Lerping Properties")]
    public bool isLerpingPosition;
    public bool isLerpingRotation;
    public Vector3 realPosition;
    public Quaternion realRotation;
    public Vector3 lastRealPosition;
    public Quaternion lastRealRotation;
    public float timeStartedLerping;
    public float timeToLerp;

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();

        // add the new player to the PlayerManager
        playerID = PLAYER_TAG + GetComponent<NetworkIdentity>().netId.ToString();
        transform.name = playerID;
        PlayerManager.Instance.AddPlayerToConnectedPlayers(playerID, this);

        if (isLocalPlayer)
        {
            PlayerManager.Instance.SetLocalPlayerID(playerID);

            followCam.transform.position = transform.position + new Vector3(0, 0, -11);
            followCam.transform.rotation = Quaternion.Euler(0, 0, 0);

            canSendNetworkMovement = false;
            RegisterNetworkMessages();
        }
        else
        {
            isLerpingPosition = false;
            isLerpingRotation = false;

            realPosition = transform.position;
            realRotation = transform.rotation;
        }
    }

    private void RegisterNetworkMessages()
    {
        NetworkManager.singleton.client.RegisterHandler(movement_msg, OnReceiveMovementMessage);
    }

    private void OnReceiveMovementMessage(NetworkMessage _message)
    {
        PlayerMovementMessage _msg = _message.ReadMessage<PlayerMovementMessage>();

        if (_msg.objectTransformName != transform.name)
        {
            PlayerManager.Instance.ConnectedPlayers[_msg.objectTransformName].GetComponent<Player>().ReceiveMovementMessage(_msg.objectPosition, _msg.objectRotation, _msg.time);
        }
    }

    public void ReceiveMovementMessage(Vector3 _position, Quaternion _rotation, float _timeToLerp)
    {
        lastRealPosition = realPosition;
        lastRealRotation = realRotation;
        realPosition = _position;
        realRotation = _rotation;
        timeToLerp = _timeToLerp;

        if (realPosition != transform.position)
        {
            isLerpingPosition = true;
        }

        if (realRotation.eulerAngles != transform.rotation.eulerAngles)
        {
            isLerpingRotation = true;
        }

        timeStartedLerping = Time.time;
    }

    private void Update()
    {

               


        if (isLocalPlayer)
        {
            //    kills your own player with the "K" button. used for debugging
        if (Input.GetKeyDown(KeyCode.K))
            {
                RpcTakeDamage(999);
            }
            UpdateCameraMovement();
            UpdatePlayerMovement();


// SHOOOOOOTING
            // if firerate is zero
            if (Math.Abs(weapon.fireRate) < float.Epsilon)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Shoot();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time > timeToFire)
                {
                    timeToFire = Time.time + 1 / weapon.fireRate;
                    Shoot();
                }
            }
            // END OF SHOOOOOOTING
        }
        else
        {
            return;
        }
    }

    private void UpdateCameraMovement()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            //    cameraX += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
            //   cameraY -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
        }

        //Quaternion rotation = Quaternion.Euler(cameraY, cameraX, 0);

        //Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        //Vector3 position = rotation * negDistance + transform.position;

        //Camera.main.transform.rotation = rotation;
        //Camera.main.transform.position = position;
    }

    private void UpdatePlayerMovement()
    {
// MOVEMENT        
        // get player input and tell PlayerMotor to move the player

        Vector2 _moveVertical = Vector2.zero;
        Vector2 _moveHorizontal = Vector2.zero;

        // add velocity if user is pressing buttons
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            _moveHorizontal = Vector2.left;
        }
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            _moveHorizontal = Vector2.right;
        }
        if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            _moveVertical = Vector2.down;
        }
        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            _moveVertical = Vector2.up;
        }

        // remove velocity if the user stops pressing buttons
        if (Input.GetKeyUp(KeyCode.A))
        {
            _moveHorizontal = Vector2.zero;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            _moveVertical = Vector2.zero;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            _moveVertical = Vector2.zero;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            _moveHorizontal = Vector2.zero;
        }

        // add up velocities and multiply with speed
        Vector2 _velocity = (_moveVertical + _moveHorizontal) * speed;

        // apply movement
        motor.Move(_velocity);

        // create a vector "diff" from the mouse to the player
        Vector2 _mousePos = followCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 diff = _mousePos - (Vector2)transform.position;

        // shorten the vector to 1
        diff.Normalize();

        // get the angle in float from the vector diff
        float _rotation = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        // apply rotation
        motor.Rotate(_rotation);

// ENDOF MOVEMENT

        if (!canSendNetworkMovement)
        {
            canSendNetworkMovement = true;
            StartCoroutine(StartNetworkSendCooldown());
        }
    }

    private IEnumerator StartNetworkSendCooldown()
    {
        timeBetweenMovementStart = Time.time;
        yield return new WaitForSeconds((1 / networkSendRate));
        SendNetworkMovement();
    }

    private void SendNetworkMovement()
    {
        timeBetweenMovementEnd = Time.time;
        SendMovementMessage(playerID, transform.position, transform.rotation, (timeBetweenMovementEnd - timeBetweenMovementStart));
        canSendNetworkMovement = false;
    }

    public void SendMovementMessage(string _playerID, Vector3 _position, Quaternion _rotation, float _timeTolerp)
    {
        PlayerMovementMessage _msg = new PlayerMovementMessage()
        {
            objectPosition = _position,
            objectRotation = _rotation,
            objectTransformName = _playerID,
            time = _timeTolerp
        };

        NetworkManager.singleton.client.Send(movement_msg, _msg);
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            NetworkLerp();
        }
    }

    private void NetworkLerp()
    {
        if (isLerpingPosition)
        {
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;

            transform.position = Vector3.Lerp(lastRealPosition, realPosition, lerpPercentage);
        }

        if (isLerpingRotation)
        {
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;

            transform.rotation = Quaternion.Lerp(lastRealRotation, realRotation, lerpPercentage);
        }
    }








    #region TODO: refactor this piece of shit

    [Header("Player Effect Prefabs")]
    [SerializeField]
    private GameObject deathEffect;
    [SerializeField]
    private GameObject spawnEffect;

    [SyncVar] // this prefix means, when the value of this variable changes, it will be "pushed out" to all other clients
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }


    private bool firstSetup = true;

    // Serialized fields on top
    [SerializeField]
    private int maxHealth = 100;
    [SerializeField]
    private GameObject bulletTrailPrefab;
    [SerializeField]
    private GameObject muzzleFlashPrefab;
    [SerializeField]
    private float muzzleLifeTime = 0.02f;
    [SerializeField]
    private float bulletTrailLifeTime = 0.1f;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    [SyncVar] // this prefix means, when the value of this variable changes, it will be "pushed out" to all other clients
    private int currentHealt;

    private float muzzleRotationOffset = 180f; // needed to flip the image
    private bool[] wasEnabled;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("collid with other: " + other.name);
    }

    public void SetupPlayer()
    {
        // THIS IS THE LOCAL PLAYER
        // gets called from Playersetup.cs - but only if it's the local Player

    
      
        CmdBroadcastNewPlayerSetup();
    }

    [Command]
    private void CmdBroadcastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }
            firstSetup = false;
        }

        SetDefaults();
    }

    [Client]
    private void Shoot()
    {
        // transform.right means forward in our case. the red axis is the axis which our player is facing

        Vector2 _startPos = weapon.firePoint.position;
        Vector2 _start = transform.position;
        Vector2 _destPos = transform.right * weapon.range;

        // Raycast from _startPos to _destPos with the length of weapon.range, we only hit objects in the Layermask "mask"
        RaycastHit2D _hit = Physics2D.Raycast(_startPos, _destPos, weapon.range, mask);

        // spawn shoot effect at the weapons firePoint
        RpcSpawnShootEffect(weapon.firePoint.position, weapon.firePoint.rotation);
    
        Debug.DrawRay(_startPos, _destPos, Color.cyan);

        // if we hit a player
        try
        {
            // commented out for testing-safety security reasons.. ( big refactor )
          //  if (_hit.collider.tag == PLAYER_TAG)
           // {
                Debug.Log(_hit.collider.name + " has been shot with " + weapon.name + "from " + transform.name + " with a damage of " + weapon.damage);
                Player _player = PlayerManager.Instance.GetPlayerFromConnectedPlayers(_hit.collider.name);
                _player.RpcTakeDamage(weapon.damage);
          //  }
        }
        catch (Exception e)
        {
            Debug.Log("Did'nt hit anything. Error in PlayerShoots-Shoot(): " + e);
            // didnt hit any player. TODO: rewrite this part
            return;
        }

    }

    [ClientRpc] // this method gets executed on every client if its called
    public void RpcSpawnShootEffect(Vector2 _pos, Quaternion _rot)
    {
        showBulletTrail(_pos, _rot);
        showMuzzle(_pos, _rot);
    }

    private void showMuzzle(Vector2 _pos, Quaternion _rot)
    {
        // TODO: Refactor this ugly piece of shit
        Vector3 muzzlePos = new Vector3(_pos.x, _pos.y, -1f);
        GameObject muzzleFlashObject = Instantiate(muzzleFlashPrefab, muzzlePos, _rot);
        muzzleFlashObject.transform.Rotate(0, 0, muzzleRotationOffset);
        //muzzleFlashObject.transform.parent = GetComponent<PlayerShoot>().weapon.firePoint;
        //float size = Random.Range(0.03f, 0.07f);
        //muzzleFlashObject.transform.localScale = new Vector2(size, size);
        Destroy(muzzleFlashObject, muzzleLifeTime);
    }

    private void showBulletTrail(Vector2 _pos, Quaternion _rot)
    {
        // spawn a bulletTrail and Destroy it after 1 second
        Destroy(Instantiate(bulletTrailPrefab, _pos, _rot), bulletTrailLifeTime);
    }

    [ClientRpc] // this method gets executed on every client if its called
    public void RpcTakeDamage(int _amount)
    {
        if (isDead)
        {
            return;
        }
        currentHealt -= _amount;

        // only on server because this is the host, we deactivate all remotePlayer-Components
        Debug.Log(transform.name + " now has " + currentHealt + " Health.");

        if (currentHealt <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // disable components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        // disable gameObjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }

        // disable collider
        Collider2D _col = GetComponent<Collider2D>();
        if (_col != null)
        {
            _col.enabled = true;
        }

        // spawn deathEffect
        GameObject _gfxInstance = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(_gfxInstance, 3);

        // TODO: Switch cameras
        // GamePlayerManager.instance.SetSceneCameraActiva(true);
        // GetComponent<PlayerSetup>().playerUIInstance.setActive(false);

        Debug.Log(transform.name + " is DEAD!");

        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(PlayerManager.Instance.matchSettings.respawnTime);

        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        // wait for playerPosition informations being spread through the network
        yield return new WaitForSeconds(0.1f);

        SetupPlayer();

        Debug.Log(transform.name + " respawned.");
    }

    public void SetDefaults()
    {
        isDead = false;
        currentHealt = maxHealth;

        // enable components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        // enable gameObjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        // enable the collider
        Collider2D _col = GetComponent<Collider2D>();
        if (_col != null)
        {
            _col.enabled = true;
        }

        // create Spawn Effect
        // spawn deathEffect
        GameObject _gfxInstance = Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_gfxInstance, 1);
    }
    #endregion
}
