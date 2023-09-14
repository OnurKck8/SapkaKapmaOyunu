using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PlayerController : MonoBehaviourPunCallbacks,IPunObservable
{
    [HideInInspector]
    public int id;//herkese id bildirmek için public yaptý. Ama deðiþtirmemek için gizledi.

    [Header("Info")]
    public float moveSpeed;
    public float jumpForce;
    public GameObject hatObject;

    [HideInInspector]
    public float curHatTime;// belli bir süre elinde kalýrsa þapka o kaybedecek o yüzden.

    [Header("Component")]
    public Rigidbody rb;
    public Player photonPlayer;

    private void Update()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            //þapkalý süre kazanma süresini geçtiyse ve oyun bitmediyse
            if(curHatTime>=GameManager.Instance.timeToWin && !GameManager.Instance.gameEnded)
            {
                GameManager.Instance.gameEnded = true;
                GameManager.Instance.photonView.RPC("WinGame", RpcTarget.All,id);
            }
        }

        if(photonView.IsMine)
        {
            Move();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TryJump();
            }

            if(hatObject.activeInHierarchy)//þapka oluþtuðu sürece
            {
                curHatTime += Time.deltaTime;//durmadan saymak için
            }
        } 
    }

    public void Move()
    {
        float x = Input.GetAxis("Horizontal")*moveSpeed;
        float z = Input.GetAxis("Vertical")*moveSpeed;

        rb.velocity = new Vector3(-x, rb.velocity.y, -z);

    }

    void TryJump()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if(Physics.Raycast(ray,0.7f))//zeminden 0.7 yukarýda ise 
        {
            rb.AddForce(Vector3.up * jumpForce,ForceMode.Impulse);//hafif silkelesin force modu
        }
    }

    public void SetHat(bool hasHat)
    {
        hatObject.SetActive(hasHat);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (photonView.IsMine)//þapka bende deðilse bir þey yapma diðerleri çarpýþýnca
            return;

        if(collision.gameObject.CompareTag("Player"))
        {
            if(GameManager.Instance.GetPlayer(collision.gameObject).id==GameManager.Instance.playerWithHat)//Benim çarptýðým kiþinin id'si withhat'in idsine eþitse
            {
                if(GameManager.Instance.CanHetHat())// alacabilecek durumda ise
                {
                    GameManager.Instance.photonView.RPC("GiveHat", RpcTarget.All,id,false);
                }
            }
        }
    }

    [PunRPC]
    public void Initialized(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;//oyuncunun kendi idsini verir.

        GameManager.Instance.players[id - 1]=this;

        if (id == 1)
            GameManager.Instance.GiveHat(id, true);//en baþta birinde þapka olmasý için

        //ben deðilsem diðerlerini true yapar kendinikini oynatýr tek
        if (!photonView.IsMine)
        {
            rb.isKinematic = true;
        }

    }

    //zaman bilgisinin sekronize çalýþmasý için
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(curHatTime);//bilgiyi sen veriyorsan gönder
        }
        else if(stream.IsReading)
        {
            curHatTime =(float) stream.ReceiveNext();//bilgiyi sen alýyorsan oradan oku
        }
    }
}
