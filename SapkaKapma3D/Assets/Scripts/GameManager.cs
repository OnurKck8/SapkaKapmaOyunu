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

    private float hatPickupT�me;//�apka tutma s�relerini tutmak i�in

    [Header("Player")]
    public string playerPrefabLocation;//kulln�c�n�n prefab�n�n adresini tutmak i�in. Olu�acak obje

    public Transform[] spawnPoints;//��kma yerleri i�in
    public PlayerController[] players;//ka� tane oyunucu var tutmak i�in
    public int playerWithHat;//�apkal� oyuncunun idsi
    private int playersInGame;//oyunda ka� ki�i var.

    public static GameManager Instance;

    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];//playerlistin uzunlu�u kadar.
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
        if(!initialGive)//�apka verilmediyse
        {
            GetPlayer(playerWithHat).SetHat(false);
        }
        playerWithHat = playerId;
        GetPlayer(playerId).SetHat(true);
        hatPickupT�me = Time.time;//al�nca sayac ba�las�n
    }

    public bool CanHetHat()
    {
        if(Time.time > hatPickupT�me + invictibleDuration)
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
