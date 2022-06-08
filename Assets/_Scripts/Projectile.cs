using System.Collections;
using UnityEngine;
using Mirror;
using FMODUnity;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : NetworkBehaviour
{
    //[SerializeField] private float distanceToDisable;

    [SerializeField] private float timeToDisable; //The time that will pass before disabling the object
    [SerializeField] private GameObject collisionParticle;
    [SerializeField] private EventReference collision_AudioEvent;
    [SyncVar] private float speed;
    [SyncVar] private Vector2 movementDirection;
    private int attackDamage;
    [System.NonSerialized]
    public Rigidbody2D rigidB;


    public Vector2 MovementDirection { get => movementDirection; set => movementDirection = value; }
    public float Speed { get => speed; set => speed = value; }
    public int AttackDamage { get => attackDamage; set => attackDamage = value; }

    
    void Start()
    {
        rigidB = GetComponent<Rigidbody2D>();
        rigidB.AddForce(MovementDirection * speed, ForceMode2D.Impulse);
    }

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        //Check if the collisioned object has the IDamageable component, meaning that it can take damage
        IDamageable damageable = collider2D.GetComponent<IDamageable>();
        //Debug.Log(collision.gameObject, collision.gameObject);
        if (damageable != null)
        {
            damageable.TakeDamage(AttackDamage);
        }

        if(collisionParticle)
            NetworkServer.Spawn(Instantiate(collisionParticle, transform.position, collisionParticle.transform.rotation));
        if(!collision_AudioEvent.IsNull)
            RuntimeManager.PlayOneShot(collision_AudioEvent, transform.position);
        NetworkManager.Destroy(gameObject);
    }

    //Spawn setup (with default attack damage 1)
    public void Init(Vector3 _spawnPosition, LayerMask _layerOfShooterObject)
    {
        transform.position = _spawnPosition;
        AttackDamage = 1;
        //Debug.Log($"Speed: {_speed}");
        //Debug.Log($"MoveDir: {_movementDirection}");

        //This should be the layer of the object that is calling this function. This way it will ignore its own collider.
        gameObject.layer = _layerOfShooterObject;

        //rigidB.AddForce(movementDirection * Speed, ForceMode2D.Impulse);

        #region DISABLE AFTER DISTANCE
        //timeToDisable = distanceToDisable / Speed;
        NetworkManager.Destroy(gameObject, timeToDisable);

        //CMD_DisableProjectile();
        //print(timeToDisable);
        #endregion
    }


    //Spawn setup (with custom attack damage)
    public void Init(Vector3 _spawnPosition, int _attackDamage, LayerMask _layerOfShooterObject)
    {
        transform.position = _spawnPosition;
        AttackDamage = _attackDamage;

        //print($"Dirección: {MovementDirection}, Vel: {Speed}");

        //This should be the layer of the object that is calling this function. This way it will ignore its own collider.
        gameObject.layer = _layerOfShooterObject;

        #region DISABLE AFTER DISTANCE
        NetworkManager.Destroy(gameObject, timeToDisable);
        //CMD_DisableProjectile();
        //print(timeToDisable);
        #endregion
    }

    /*
    private IEnumerator DisableProjectile()
    {
        yield return new WaitForSecondsRealtime(timeToDisable);
        gameObject.SetActive(false);
    }

    [ServerCallback]
    private void CMD_DisableProjectile()
    {
        StartCoroutine(DisableProjectile());
    }
    */

    [ServerCallback]
    private void ApplyImpulse()
    {
        rigidB.AddForce(movementDirection * Speed, ForceMode2D.Impulse);
    }

}
