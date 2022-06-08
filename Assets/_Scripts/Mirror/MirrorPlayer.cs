using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MirrorPlayer : NetworkBehaviour
{
    public static MirrorPlayer Instance { get; private set; }
    public GameObject pfbProjectile;

    [SerializeField] private float speed;
    
    private Vector3 moveDir;
    [SyncVar(hook = nameof(OnColorChange))]
    private Color myColor;
    [SyncVar(hook = nameof(OnHPChange))]
    private int hp = 10;
    //SyncList<int> myList = new SyncList<int>();

    private MirrorTransform mirrorTransform;

    private void Awake()
    {
        mirrorTransform = GetComponent<MirrorTransform>();
    }

    private void Start()
    {
        if (isLocalPlayer)
        {
            Instance = this;
            Camera.main.GetComponent<CameraController>().FollowLocalPlayer();
        }

        //Para este punto, las variables SyncVar ya están sincronizadas

        //myList.Callback += OnMyListChange;
    }

    void Update()
    {
        //Código para procesar en todas las instancias 
        
        if (!isLocalPlayer) return;
        //Owner code:

        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.z = Input.GetAxisRaw("Vertical");

        mirrorTransform.direction = moveDir * speed;

        //transform.Translate(moveDir.normalized * speed * Time.deltaTime);
        transform.Translate(mirrorTransform.direction.normalized * speed * Time.deltaTime);


        if (Input.GetKeyDown(KeyCode.C))
            CMD_ChangeColor();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CMD_Shoot();
        }

    }

    private void OnDestroy()
    {
        //myList.Callback -= OnMyListChange;
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Fire"))
        {
            hp--;
        }
    }

    private void OnMyListChange(SyncList<int>.Operation _operation, int _index, int _oldValue, int _newValue)
    {
        switch (_operation)
        {
            case SyncList<int>.Operation.OP_ADD: 
                //_index, _newValue
                break;
            case SyncList<int>.Operation.OP_CLEAR:
                //No utiliza otro parámetro
                break;
            case SyncList<int>.Operation.OP_INSERT:
                //_index, _newValue
                break;
            case SyncList<int>.Operation.OP_REMOVEAT:
                //_index, _oldValue
                break;
            case SyncList<int>.Operation.OP_SET:
                //_index, _newValue, _oldValue
                break;
            default:
                break;
        }
    }

    private void OnHPChange(int _oldHP, int _newHP)
    {

    }

    //Siempre va el valor nuevo primero y después el valor viejo
    private void OnColorChange(Color _oldColor, Color _newColor)
    {
        //Debug.Log($"The color was updated. From  {_oldColor} to {_newColor}. MyColor = {myColor}");
        GetComponent<MeshRenderer>().material.color = _newColor;
    }

    //De cliente a servidor
    [Command]
    private void CMD_ChangeColor()
    {
        myColor = Random.ColorHSV();
        //RPC_ChangeColor(c);
    }

    //Es obligatorio que tenga el prefijo RPC. Son funciones que se mandan por red.
    [ClientRpc]
    void RPC_ChangeColor(Color _color)
    {
        GetComponent<MeshRenderer>().material.color = _color;
    }

    [Command]
    private void CMD_Shoot()
    {
        GameObject go = NetworkManager.Instantiate(pfbProjectile, transform.position, Quaternion.identity);
        go.GetComponent<Rigidbody>().velocity = Vector3.up * 10f;
        NetworkServer.Spawn(go); //ya le avisa a los clientes que se generen
        NetworkManager.Destroy(go, 5f);
    }
}
