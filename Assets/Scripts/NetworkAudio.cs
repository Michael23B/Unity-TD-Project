using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class NetworkAudio : NetworkBehaviour {

    public AudioSource source;
    public AudioClip[] soundClips;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        source = GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    [Command]
    void CmdSendSoundIndex(int index)
    {
        RpcSendSoundIndex(index);
    }

    [ClientRpc]
    void RpcSendSoundIndex(int index)
    {
        source.PlayOneShot(soundClips[index]);
    }
}
