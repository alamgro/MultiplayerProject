using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[SelectionBase]
public class Player : MonoBehaviour, IDamageable
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

    private Vector2 moveDirection;
    private Vector3 shootDirection;
    private Rigidbody2D rigidB;
    private Collider2D playerCollider;
    private float shootTimer;
    private List<Projectile> poolProjectiles;

    void Start()
    {
        rigidB = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        poolProjectiles = new List<Projectile>();
        shootTimer = attackCooldown;

        GameManager.Instance.PlayerInstance = this;
    }

    
    void Update()
    {
        #region MOVEMENT
        moveDirection.x = Input.GetAxisRaw(GameConstants.Key.horizontal);//Get movement X direction 
        moveDirection.x *= movementSpeed; //Apply movement speed
        //moveDirection.y = 0f;
        #endregion

        #region SHOOT
        shootTimer -= Time.deltaTime;
        if(shootTimer <= 0f)
        {
            shootDirection.x = Input.GetAxisRaw(GameConstants.Key.shootHorizontal);
            shootDirection.y = Input.GetAxisRaw(GameConstants.Key.shootVertical);
            if (shootDirection != Vector3.zero)
            {
                shootDirection.Normalize();
                shootTimer = attackCooldown;
                //Shoot(shootDirection);
                ShootProjectile();
            }
        }
        #endregion

        #region JUMP
        Debug.DrawRay(playerCollider.bounds.center, Vector2.down * (playerCollider.bounds.extents.y + 0.05f), Color.green);
        if (Input.GetKeyDown(KeyCode.W))
        {
            if(Physics2D.BoxCast(playerCollider.bounds.center, new Vector2(playerCollider.bounds.size.x - 0.1f, 0.05f), 0f, Vector2.down, playerCollider.bounds.extents.y, maskIgnorePlayer) )
            {
                //Debug.Log("Jumping");
                rigidB.AddForce(Vector2.up * jumpForce * rigidB.mass, ForceMode2D.Impulse);
            }
            
        }
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
        }
        else
        {
            //Look at where the player is walking to
            if(moveDirection.x != 0f)
                transform.localScale = new Vector3(Mathf.Sign(moveDirection.x), transform.localScale.y, transform.localScale.z);
        }
        #endregion

        moveDirection.y = rigidB.velocity.y;
    }

    private void FixedUpdate()
    {
        rigidB.velocity = moveDirection;
    }

    void IDamageable.TakeDamage(int _damage)
    {
        //Die();
        Debug.Log("Die");

        //QUIERO CAMBIAR ESTO AL FINAL DEL NIVEL (UNA SOLA ACTUALIZACIÓN AL FINAL, Y NO UNA POR MUERTE)
        PlayfabManager.Instance.UpdateDeaths(); 
    }

    private Projectile ShootProjectile()
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
                poolProjectiles[i].GetComponent<Projectile>().Init(
                    spawnPosition, shootDirection, projectileSpeed, attackDamage, LayerMask.NameToLayer(GameConstants.Layer.ally));
                return poolProjectiles[i];
            }
        }

        //Instantiate and setup new projectile
        poolProjectiles.Add(Instantiate(pfbProjectile).GetComponent<Projectile>());
        //Setup projectile
        poolProjectiles[poolProjectiles.Count - 1].GetComponent<Projectile>().Init(
            spawnPosition, shootDirection.normalized, projectileSpeed, attackDamage, LayerMask.NameToLayer(GameConstants.Layer.ally));

        return poolProjectiles[poolProjectiles.Count - 1];
    }
}
