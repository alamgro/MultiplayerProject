using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Soldier : MonoBehaviour, IDamageable
{
    [Header("Basic attributes")]
    [SerializeField] private float baseMovementSpeed;
    [SerializeField] private float walkingTime; //Minimum time that the enemy will walk towards the player
    [SerializeField] private int hp;
    [SerializeField] private int pointsValue;
    [Header("Attack attributes")]
    [SerializeField] private float attackRangeDistance;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackCastingDelay;
    [SerializeField] private Transform attackOriginPoint;
    [Header("Projectile attributes")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private GameObject pfbProjectile;

    private Rigidbody2D rigidB;
    private Player playerInstance;
    private Vector3 vectorToPlayer;
    private float attackCurrentCooldown = 0f;
    private float currentMovementSpeed;
    private bool isReadyToAttack;
    private bool followPlayer = false; 

    void Start()
    {
        rigidB = GetComponent<Rigidbody2D>();
        playerInstance = GameManager.Instance.PlayerInstance;

        currentMovementSpeed = baseMovementSpeed;
    }

    
    void Update()
    {
        vectorToPlayer = playerInstance.transform.position - transform.position;
        
        rigidB.velocity = Vector3.up * rigidB.velocity.y;

        //Attack Timer
        attackCurrentCooldown -= Time.deltaTime;
        isReadyToAttack = attackCurrentCooldown <= 0f ? true : false;

        //if it is too far, move and get closer
        if (vectorToPlayer.sqrMagnitude > (attackRangeDistance * attackRangeDistance))
        {
            if(!followPlayer)
                StartCoroutine(MoveToTarget());
        }
        else //Attack when the target is close enough
        {
            if (isReadyToAttack )
                StartCoroutine(Attack());
        }

        #region LOOK/ROTATE AT
        //Defines where the enemy should be looking at
        transform.localScale = new Vector3(Mathf.Sign(vectorToPlayer.x), transform.localScale.y, transform.localScale.z);
        #endregion
    }

    private void FixedUpdate()
    {
        if (followPlayer)
            rigidB.velocity = vectorToPlayer.normalized * Vector2.right * currentMovementSpeed;
    }

    private void OnDrawGizmos()
    {
        if (vectorToPlayer.sqrMagnitude < (attackRangeDistance * attackRangeDistance))
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        if (attackRangeDistance > 0f)
        {
            Gizmos.DrawWireSphere(transform.position, attackRangeDistance);
        }
    }
    
    //Move to target a fixed amount of time
    private IEnumerator MoveToTarget()
    {
        currentMovementSpeed = baseMovementSpeed;
        followPlayer = true;
        isReadyToAttack = false;
        yield return new WaitForSecondsRealtime(walkingTime);
        followPlayer = false;
        isReadyToAttack = true;
    }

    private IEnumerator Attack()
    {
        attackCurrentCooldown = attackCooldown + attackCastingDelay;
        currentMovementSpeed = 0f;
        Vector3 spawnPosition = attackOriginPoint.position + (vectorToPlayer * 0.5f);

        yield return new WaitForSecondsRealtime(attackCastingDelay);
        //print("Attack!!!");
        Projectile projectile = Instantiate(pfbProjectile, transform.localPosition, pfbProjectile.transform.rotation).GetComponent<Projectile>();
        //projectile.Init(vectorToPlayer.normalized, projectileSpeed, gameObject.layer);
        projectile.Init(spawnPosition, vectorToPlayer.normalized, projectileSpeed, LayerMask.NameToLayer(GameConstants.Layer.enemyProjectile));
    }

    void IDamageable.TakeDamage(int _damage)
    {
        hp -= _damage;
        CheckHP();
    }

    private void CheckHP()
    {
        if (hp <= 0)
            Die();
    }

    private void Die()
    {
        GameManager.Instance.EnemiesKilled += 1;
        Destroy(gameObject);
    }
}
