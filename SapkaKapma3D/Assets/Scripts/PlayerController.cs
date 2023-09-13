using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PlayerController : MonoBehaviour
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
        Move();
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TryJump();
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
}
