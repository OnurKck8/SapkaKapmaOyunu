using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class GameUI : MonoBehaviour
{
    public PlayerUIContainer[] playerContainers;
    public TextMeshProUGUI winText;

    public static GameUI Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InitializerPlayerUI();    }

    void InitializerPlayerUI()
    {
        for(int i=0;i<playerContainers.Length;++i)
        {
            PlayerUIContainer container = playerContainers[i];
            if(i<PhotonNetwork.PlayerList.Length)
            {
                container.obj.SetActive(true);
                container.nameText.text = PhotonNetwork.PlayerList[i].NickName;
                container.hatTimeSlider.maxValue = GameManager.Instance.timeToWin;
            }
            else
            {
                container.obj.SetActive(false);
            }
        }
    }

    void Update()
    {
        UpdatePlayerUI();
    }

    void UpdatePlayerUI()
    {
        for(int i=0;i<GameManager.Instance.players.Length;++i)
        {
            if(GameManager.Instance.players[i]!=null)
            {
                playerContainers[i].hatTimeSlider.value = GameManager.Instance.players[i].curHatTime; // oranlý bir þekilde göstermeyi saðlayacak.

            }
        }
    }

    public void SetWinText(string winnerName)
    {
        winText.gameObject.SetActive(true);
        winText.text = winnerName + "Wins";
    }
}

[System.Serializable]
public class PlayerUIContainer
{
    public GameObject obj;
    public TextMeshProUGUI nameText;
    public Slider hatTimeSlider;
}
