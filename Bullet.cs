using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
   private Rigidbody m_Bullet;
   private float speed = 15f;

    void Start(){
       m_Bullet = GetComponent<Rigidbody>();
       Invoke("Destroy", 5f);
   }

    void Update(){
       Vector3 move = transform.forward * speed * Time.deltaTime;
       m_Bullet.MovePosition(m_Bullet.position + move);
   }

   void Destroy(){
       Destroy(gameObject);
   }

   private void OnCollisionEnter(Collision collision){
       CancelInvoke();
       Destroy(gameObject, 1f);
       enabled = false;
   }
}
