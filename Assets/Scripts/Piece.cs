using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Sprite GetSprite() => ( sr.sprite );

    public SpriteRenderer sr;
    public Rigidbody2D body2D;

    private bool rotating = false;

    // Start is called before the first frame update
    //void Awake()
    //{
    //    sr = GetComponent<SpriteRenderer>();
    //}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Rotate()
    {

    }

    public void Destruction( float delay )
    {
        Destroy(gameObject, delay);
    }
}
