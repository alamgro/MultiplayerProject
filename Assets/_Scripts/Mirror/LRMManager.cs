using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightReflectiveMirror;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class LRMManager : MonoBehaviour
{
    LightReflectiveMirrorTransport lrm;
    [SerializeField] Transform panel;
    [SerializeField] GameObject serverPrefab;

    void Start()
    {
        lrm = (LightReflectiveMirrorTransport)Transport.activeTransport;
        lrm.serverListUpdated.AddListener(ServerListUpdate); //Llama esta función cuando haya un cambio en la lista de servidores
    }

    void ServerListUpdate()
    {
        //Limpiamos los que estan ahorita
        foreach (Transform t in panel)
        {
            Destroy(t.gameObject);
        }

        print("Lista size: " + lrm.relayServerList.Count);
        //Creamos 1 por cada servidor
        for (int i = 0; i < lrm.relayServerList.Count; i++)
        {
            if (!lrm.relayServerList[i].serverName.StartsWith(GameConstants.General.alamServer))
            {
                Debug.Log($"Skiping server: {lrm.relayServerList[i].serverName}");
                continue;
            }

            GameObject go = Instantiate(serverPrefab, panel);
            go.GetComponentInChildren<TextMeshProUGUI>().SetText(lrm.relayServerList[i].serverName.Replace(GameConstants.General.alamServer, string.Empty)); //Le actualizo el nombre de la partida
            string serverID = lrm.relayServerList[i].serverId; //Guardar el ID del servidor que ayudara hacer todo proceso de conexion
            go.GetComponentInChildren<Button>().onClick.AddListener(() => Conectarse(serverID)); //Al dar click al boton, llama la función 'Conectarse'
        }
    }

    public void ActualizarLista() //Llamado por el boton de refrescar
    {
        lrm.RequestServerList();
    }

    void Conectarse(string _serverID)
    {
        NetworkManager.singleton.networkAddress = _serverID;
        NetworkManager.singleton.StartClient();
    }
    private void OnDestroy()
    {
        lrm.serverListUpdated.RemoveListener(ServerListUpdate); //Si se destruye (al cambiar de escena), ya no me actualizes si hay nuevos servidores
    }
}
