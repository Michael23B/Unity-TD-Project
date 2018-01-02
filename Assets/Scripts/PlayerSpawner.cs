using UnityEngine;
using UnityEngine.Networking;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerPrefab = null;

    public void SpawnPlayer(NetworkConnection conn) // spawn a new player for the desired connection
    {
        GameObject playerObj = GameObject.Instantiate(_playerPrefab); // instantiate on server side
        NetworkServer.AddPlayerForConnection(conn, playerObj, 0); // spawn on the clients and set owner
    }
}