using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    //void Update()
    //{

    //}

    private void OnTriggerEnter2D( Collider2D collider )
    {
        Debug.Log("Collision with Death Zone!");
        //if ( collider.CompareTag("Piece") )
        //{
        //    collider.GetComponent<Piece>().Destruction(.5f);
        //}
        if ( collider.CompareTag("Player") )
        {
            //GameManager.instance.PlayerDeath();
        }
    }

    private void OnTriggerExit2D( Collider2D collider )
    {
        Debug.Log("Leaving Death Zone!");
        if ( collider.CompareTag("Piece") && collider.transform.position.y < transform.position.y)
        {
            collider.GetComponent<Piece>().Destruction(.5f);
        }
    }
}
