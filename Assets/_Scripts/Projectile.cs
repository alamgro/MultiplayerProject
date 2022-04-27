using System.Collections;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : NetworkBehaviour
{
    [SerializeField] private float distanceToDisable;

    private float timeToDisable; //The time that will pass before disabling the object
    [SyncVar] private float speed;
    [SyncVar] private Vector2 movementDirection;
    private int attackDamage;
    private Rigidbody2D rigidB;


    public Vector2 MovementDirection { get => movementDirection; set => movementDirection = value; }
    public float Speed { get => speed; set => speed = value; }
    public int AttackDamage { get => attackDamage; set => attackDamage = value; }

    
    void Start()
    {
        rigidB = GetComponent<Rigidbody2D>();
    }

    [ServerCallback]
    private void FixedUpdate()
    {
        rigidB.velocity = MovementDirection * Speed;
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

        NetworkManager.Destroy(gameObject, timeToDisable);
    }

    //Spawn setup (with default attack damage 1)
    public void Init(Vector3 _spawnPosition, Vector3 _movementDirection, float _speed, LayerMask _layerOfShooterObject)
    {
        transform.position = _spawnPosition;
        MovementDirection = _movementDirection;
        Speed = _speed;
        AttackDamage = 1;

        //This should be the layer of the object that is calling this function. This way it will ignore its own collider.
        gameObject.layer = _layerOfShooterObject;

        #region DISABLE AFTER DISTANCE
        timeToDisable = distanceToDisable / Speed;
        NetworkManager.Destroy(gameObject, timeToDisable);

        //CMD_DisableProjectile();
        //print(timeToDisable);
        #endregion
    }


    //Spawn setup (with custom attack damage)
    public void Init(Vector3 _spawnPosition, Vector3 _movementDirection, float _speed, int _attackDamage, LayerMask _layerOfShooterObject)
    {
        transform.position = _spawnPosition;
        MovementDirection = _movementDirection;
        AttackDamage = _attackDamage;
        Speed = _speed;

        //print($"Dirección: {MovementDirection}, Vel: {Speed}");

        //This should be the layer of the object that is calling this function. This way it will ignore its own collider.
        gameObject.layer = _layerOfShooterObject;

        #region DISABLE AFTER DISTANCE
        timeToDisable = distanceToDisable / Speed;
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

}
