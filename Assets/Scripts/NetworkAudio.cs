using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(AudioSource))]
public class NetworkAudio : NetworkBehaviour {

    public AudioSource source;
    public AudioClip[] soundClips;

    private void Start()
    {
        source = GetComponent<AudioSource>();
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
