using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PlayerController : MonoBehaviourPunCallbacks,IPunObservable
{
    [HideInInspector]
    public int id;//herkese id bildirmek i�in public yapt�. Ama de�i�tirmemek i�in gizledi.

    [Header("Info")]
    public float moveSpeed;
    public float jumpForce;
    public GameObject hatObject;

    [HideInInspector]
    public float curHatTime;// belli bir s�re elinde kal�rsa �apka o kaybedecek o y�zden.

    [Header("Component")]
    public Rigidbody rb;
    public Player photonPlayer;

    private void Update()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            //�apkal� s�re kazanma s�resini ge�tiyse ve oyun bitmediyse
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

            if(hatObject.activeInHierarchy)//�apka olu�tu�u s�rece
            {
                curHatTime += Time.deltaTime;//durmadan saymak i�in
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

        if(Physics.Raycast(ray,0.7f))//zeminden 0.7 yukar�da ise 
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
        if (photonView.IsMine)//�apka bende de�ilse bir �ey yapma di�erleri �arp���nca
            return;

        if(collision.gameObject.CompareTag("Player"))
        {
            if(GameManager.Instance.GetPlayer(collision.gameObject).id==GameManager.Instance.playerWithHat)//Benim �arpt���m ki�inin id'si withhat'in idsine e�itse
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
            GameManager.Instance.GiveHat(id, true);//en ba�ta birinde �apka olmas� i�in

        //ben de�ilsem di�erlerini true yapar kendinikini oynat�r tek
        if (!photonView.IsMine)
        {
            rb.isKinematic = true;
        }

    }

    //zaman bilgisinin sekronize �al��mas� i�in
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(curHatTime);//bilgiyi sen veriyorsan g�nder
        }
        else if(stream.IsReading)
        {
            curHatTime =(float) stream.ReceiveNext();//bilgiyi sen al�yorsan oradan oku
        }
    }
}
