using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;

    private void Awake()
    {
        //oturum varsa devam ettir yoksa yeni aç
        if (Instance != null && Instance != this)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
       
    }
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }


    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}
