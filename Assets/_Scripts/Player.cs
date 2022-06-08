using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using FMODUnity;
using FMOD.Studio;
using Cinemachine;

[RequireComponent(typeof(Rigidbody2D))]
[SelectionBase]
public class Player : NetworkBehaviour, IDamageable
{
    [Header("Basic attributes")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpForce;
    [Header("Attack attributes")]
    [SerializeField] private int attackDamage;
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform attackOriginPoint;
    [Header("Projectile attributes")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private LayerMask maskIgnorePlayer;
    [SerializeField] private GameObject pfbProjectile;
    [Header("Animation attributes")]
    [SerializeField] private float walkAnimThreshold;
    [Header("SFX attributes")]
    [SerializeField] private EventReference spell_AudioEvent;
    [SerializeField] private EventReference jump_AudioEvent;

    private Vector2 moveDirection;
    private Vector3 shootDirection;
    private Rigidbody2D rigidB;
    private Collider2D playerCollider;
    private Animator anim;
    private float shootTimer;
    private CinemachineVirtualCamera virtualCam;

    public Transform AttackOriginPoint => attackOriginPoint;

    private void Awake()
    {
        rigidB = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        if (isLocalPlayer)
        {
            virtualCam = GameObject.FindGameObjectWithTag(GameConstants.Tag.virtualCamera).GetComponent<CinemachineVirtualCamera>();
            virtualCam.Follow = transform;
            CMD_AddPlayerToManager(this);
            gameObject.AddComponent(typeof(StudioListener));
        }

        shootTimer = attackCooldown;

    }

    
    void Update()
    {
        if (!isLocalPlayer) return;
        //Owner code:
        #region MOVEMENT
        moveDirection.x = Input.GetAxisRaw(GameConstants.Key.horizontal);//Get movement X direction 
        moveDirection.x *= movementSpeed; //Apply movement speed
        anim.SetBool(GameConstants.PlayerAnimationParameter.walking, Mathf.Abs(moveDirection.x) > walkAnimThreshold);
        //moveDirection.y = 0f;
        #endregion

        #region SHOOT
        shootTimer -= Time.deltaTime;
        CheckShootInput();
        #endregion

        #region JUMP
        Debug.DrawRay(transform.position, Vector2.down * 0.05f, Color.green);
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (IsGrounded())
            {
                rigidB.AddForce(Vector2.up * jumpForce * rigidB.mass, ForceMode2D.Impulse);
                anim.SetTrigger(GameConstants.PlayerAnimationParameter.jump);
                RuntimeManager.PlayOneShot(jump_AudioEvent, transform.position);
            }
        }
        anim.SetBool(GameConstants.PlayerAnimationParameter.grounded, IsGrounded());
        #endregion

        #region CROUCH
        //This is a nice to have
        #endregion

        #region LOOK/ROTATE AT
        //Defines where the player should be looking at
        if (shootDirection != Vector3.zero)
        {
            //Look at where the player is shooting, regardless of where the player is walking to
            transform.localScale = new Vector3(Mathf.Sign(shootDirection.x), transform.localScale.y, transform.localScale.z);

            //Check wheather to walk forward of backwards based on the movement and shooting directing
            anim.SetBool(GameConstants.PlayerAnimationParameter.walkBackwards, Mathf.Sign(shootDirection.x) != Mathf.Sign(moveDirection.x)); //Walk Forward
        }
        else
        {
            //Look at where the player is walking to
            if (moveDirection.x != 0f)
                transform.localScale = new Vector3(Mathf.Sign(moveDirection.x), transform.localScale.y, transform.localScale.z);

            anim.SetBool(GameConstants.PlayerAnimationParameter.walkBackwards, false); //Walk Forward
        }
        #endregion

        moveDirection.y = rigidB.velocity.y;
    }

    private void FixedUpdate()
    {
        rigidB.velocity = moveDirection;
    }

    [ServerCallback]
    void IDamageable.TakeDamage(int _damage)
    {
        //Die();
        //Debug.Log("Die");

        //QUIERO CAMBIAR ESTO AL FINAL DEL NIVEL (UNA SOLA ACTUALIZACIÓN AL FINAL, Y NO UNA POR MUERTE)
        //PlayfabManager.Instance.UpdateDeaths(); 
    }

    private void CheckShootInput()
    {
        if (shootTimer <= 0f)
        {
            shootDirection.x = Input.GetAxisRaw(GameConstants.Key.shootHorizontal);
            shootDirection.y = Input.GetAxisRaw(GameConstants.Key.shootVertical);
            if (shootDirection != Vector3.zero)
            {
                shootDirection.Normalize();
                shootTimer = attackCooldown;
                //playerAudio = RuntimeManager.CreateInstance(playerAudioEvent);
                //playerAudio.start();
                //Shoot(shootDirection);
                CMD_ShootProjectile(shootDirection);
            }
        }
    }

    [Command]
    private void CMD_ShootProjectile(Vector3 _shootDirection)
    {
        Vector3 spawnPosition = attackOriginPoint.position + (_shootDirection * 0.5f);

        RuntimeManager.PlayOneShot(spell_AudioEvent, spawnPosition);

        Projectile projectile = NetworkManager.Instantiate(pfbProjectile, spawnPosition, Quaternion.identity).GetComponent<Projectile>();
        NetworkServer.Spawn(projectile.gameObject); //ya le avisa a los clientes que se generen
        projectile.Init(spawnPosition,  attackDamage, LayerMask.NameToLayer(GameConstants.Layer.ally));
        projectile.Speed = projectileSpeed;
        projectile.MovementDirection = _shootDirection;
        //projectile.rigidB.AddForce(_shootDirection * projectileSpeed, ForceMode2D.Impulse);
        
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(transform.position, new Vector2(playerCollider.bounds.size.x - 0.1f, 0.05f), 0f, Vector2.down, 0.05f, maskIgnorePlayer);
    }

    [Command]
    private void CMD_AddPlayerToManager(Player player)
    {
        GameManager.Instance.PlayerInstances.Add(player);
    }

    /*
    [Command]
    private void CMD_ShootProjectile()
    {
        //Spawn position with an offset on the Z axis
        Vector3 spawnPosition = attackOriginPoint.position + (shootDirection * 0.5f);

        for (int i = 0; i < poolProjectiles.Count; i++)
        {
            //Setup existing projectile
            if (!poolProjectiles[i].gameObject.activeSelf)
            {
                poolProjectiles[i].gameObject.SetActive(true);
                //Setup projectile
                poolProjectiles[i].GetComponent<Projectile>().CMD_Init(
                    spawnPosition, shootDirection, projectileSpeed, attackDamage, LayerMask.NameToLayer(GameConstants.Layer.ally));
                //return poolProjectiles[i];
            }
        }

        //Instantiate and setup new projectile
        poolProjectiles.Add(NetworkManager.Instantiate(pfbProjectile).GetComponent<Projectile>());
        //Setup projectile
        poolProjectiles[poolProjectiles.Count - 1].GetComponent<Projectile>().CMD_Init(
            spawnPosition, shootDirection.normalized, projectileSpeed, attackDamage, LayerMask.NameToLayer(GameConstants.Layer.ally));
        NetworkServer.Spawn(poolProjectiles[poolProjectiles.Count - 1].gameObject);

        //return poolProjectiles[poolProjectiles.Count - 1];
    }*/
}
