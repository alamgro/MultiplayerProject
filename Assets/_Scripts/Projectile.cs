using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float distanceToDisable;

    private float timeToDisable; //The time that will pass before disabling the object
    private float speed;
    private int attackDamage;
    private Vector2 movementDirection;
    private Rigidbody2D rigidB;


    public Vector2 MovementDirection { get => movementDirection; set => movementDirection = value; }
    public float Speed { get => speed; set => speed = value; }
    public int AttackDamage { get => attackDamage; set => attackDamage = value; }

    void Start()
    {
        rigidB = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rigidB.velocity = MovementDirection * speed;
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        //Check if the collisioned object has the IDamageable component, meaning that it can take damage
        IDamageable damageable = collider2D.GetComponent<IDamageable>();
        //Debug.Log(collision.gameObject, collision.gameObject);
        if (damageable != null)
        {
            damageable.TakeDamage(AttackDamage);
        }

        gameObject.SetActive(false);

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
    }

    //Spawn setup (with custom attack damage)
    public void Init(Vector3 _spawnPosition, Vector3 _movementDirection, float _speed, int _attackDamage, LayerMask _layerOfShooterObject)
    {
        transform.position = _spawnPosition;
        MovementDirection = _movementDirection;
        AttackDamage = _attackDamage;
        Speed = _speed;

        //This should be the layer of the object that is calling this function. This way it will ignore its own collider.
        gameObject.layer = _layerOfShooterObject;

        #region DISABLE AFTER DISTANCE
        timeToDisable = distanceToDisable / Speed;
        StartCoroutine(DisableProjectile());
        //print(timeToDisable);
        #endregion
    }

    private IEnumerator DisableProjectile()
    {
        yield return new WaitForSecondsRealtime(timeToDisable);
        gameObject.SetActive(false);
    }

}
