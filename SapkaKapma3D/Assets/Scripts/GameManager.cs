using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Stats")]
    public bool gameEnded = false;
    public float timeToWin;
    public float invictibleDuration;

    private float hatPickupTÝme;//þapka tutma sürelerini tutmak için

    [Header("Player")]
    public string playerPrefabLocation;//kullnýcýnýn prefabýnýn adresini tutmak için. Oluþacak obje

    public Transform[] spawnPoints;//çýkma yerleri için
    public PlayerController[] players;//kaç tane oyunucu var tutmak için
    public int playerWithHat;//þapkalý oyuncunun idsi
    private int playersInGame;//oyunda kaç kiþi var.

    public static GameManager Instance;

    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];//playerlistin uzunluðu kadar.
        photonView.RPC("ImInGame", RpcTarget.All);
    }

    [PunRPC]
    void WinGame(int playerId)
    {
        gameEnded = true;
        PlayerController player = GetPlayer(playerId);

        GameUI.Instance.SetWinText(player.photonPlayer.NickName);

        Invoke("GoBackToMenu",3.0f);
    }

    void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.Instance.ChangeScene("MainMenu");
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;

        if (playersInGame == PhotonNetwork.PlayerList.Length)
            SpawnPlayer();
    }

    [PunRPC]
    public void GiveHat(int playerId,bool initialGive)
    {
        if(!initialGive)//þapka verilmediyse
        {
            GetPlayer(playerWithHat).SetHat(false);
        }
        playerWithHat = playerId;
        GetPlayer(playerId).SetHat(true);
        hatPickupTÝme = Time.time;//alýnca sayac baþlasýn
    }

    public bool CanHetHat()
    {
        if(Time.time > hatPickupTÝme + invictibleDuration)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, 
            spawnPoints.Length)].position, 
            Quaternion.identity);

        PlayerController playerScript = playerObj.GetComponent<PlayerController>();

        playerScript.photonView.RPC("Initialized", RpcTarget.All,PhotonNetwork.LocalPlayer);
    }

    public PlayerController GetPlayer(int playerId)
    {
        return players.First(x => x.id == playerId);
    }

    public PlayerController GetPlayer(GameObject playerObj)
    {
        return players.First(x => x.gameObject == playerObj);
    }

}
