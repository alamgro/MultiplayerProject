using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using FMODUnity;

[RequireComponent(typeof(Rigidbody2D))]
[SelectionBase]
public class Soldier : NetworkBehaviour, IDamageable
{
    [Header("Basic attributes")]
    [SerializeField] private float baseMovementSpeed;
    [SerializeField] private float walkingTime; //Minimum time that the enemy will walk towards the player
    [SerializeField] private int hp;
    [SerializeField] private int pointsValue; //Amount of points given for killing it
    [Header("Attack attributes")]
    [SerializeField] private float attackRangeDistance;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackCastingDelay;
    [SerializeField] private Transform attackOriginPoint;
    [Header("Projectile attributes")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private GameObject pfbProjectile;
    [Header("SFX attributes")]
    [SerializeField] private EventReference death_AudioEvent;

    private Rigidbody2D rigidB;
    private Animator anim;
    private Vector3 vectorToPlayer;
    private float attackCurrentCooldown = 0f;
    private float currentMovementSpeed;
    private bool isReadyToAttack;
    private bool followPlayer = false;

    public float CurrentMovementSpeed { get => currentMovementSpeed; set => currentMovementSpeed = value; }
    public float BaseMovementSpeed { get => baseMovementSpeed; set => baseMovementSpeed = value; }

    private void Awake()
    {
        rigidB = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {

        currentMovementSpeed = baseMovementSpeed;

        foreach (Player player in GameManager.Instance.PlayerInstances)
        {
            Debug.Log($"player found: {player.name}");
        }
    }

    [ServerCallback]
    void Update()
    {
        //vectorToPlayer = playerInstances.GetComponent<Collider2D>().bounds.center - attackOriginPoint.position;
        
        rigidB.velocity = Vector3.up * rigidB.velocity.y;
        vectorToPlayer = GetClosestPlayer();
        //Debug.Log(vectorToPlayer);
        //Attack Timer
        attackCurrentCooldown -= Time.deltaTime;
        isReadyToAttack = attackCurrentCooldown <= 0f ? true : false;

        anim.SetBool(GameConstants.DemonAnimationParameter.walking, followPlayer);

        //if it is too far, move and get closer
        if (vectorToPlayer.magnitude > attackRangeDistance)
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
        if (vectorToPlayer.magnitude < attackRangeDistance)
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
        isReadyToAttack = true;
    }


    private IEnumerator Attack()
    {
        //Debug.Log("Attack");
        attackCurrentCooldown = attackCooldown + attackCastingDelay;
        currentMovementSpeed = 0f;
        followPlayer = false;

        yield return new WaitForSecondsRealtime(attackCastingDelay);
        //print("Attack!!!");
        anim.SetTrigger(GameConstants.DemonAnimationParameter.attack);
    }

    public void SpawnProjectile()
    {
        Vector3 spawnPosition = attackOriginPoint.position + (vectorToPlayer.normalized * 0.5f);
        Projectile projectile = Instantiate(pfbProjectile, transform.localPosition, pfbProjectile.transform.rotation).GetComponent<Projectile>();
        NetworkServer.Spawn(projectile.gameObject);
        projectile.Init(spawnPosition, LayerMask.NameToLayer(GameConstants.Layer.enemyProjectile));
        projectile.Speed = projectileSpeed;
        projectile.MovementDirection = vectorToPlayer.normalized;
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
        //Debug.Log("Died.", gameObject);
        GameManager.Instance.LevelKills += 1;
        GameManager.Instance.LevelPoints += pointsValue;
        RuntimeManager.PlayOneShot(death_AudioEvent, transform.position);

        NetworkManager.Destroy(gameObject);
    }

    private Vector3 GetClosestPlayer()
    {
        float distanceToPlayer = Mathf.Infinity;
        Player closestPlayer = null;

        foreach (Player player in GameManager.Instance.PlayerInstances)
        {
            //Debug.Log($"player found: {player.name}");

            if (distanceToPlayer > Vector3.Distance(player.AttackOriginPoint.position, attackOriginPoint.position))
            {
                distanceToPlayer = Vector3.Distance(player.AttackOriginPoint.position, attackOriginPoint.position);
                closestPlayer = player;
            }
        }

        return closestPlayer.AttackOriginPoint.position - attackOriginPoint.position;
    }

}
