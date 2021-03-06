using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Sprite GetSprite() => ( sr.sprite );

    public SpriteRenderer sr;
    public Rigidbody2D body2D;
    public Collider2D col2D;
    public SpriteRenderer outline;
    public AudioClip rotateSFX;

    [HideInInspector]
    public bool grabbed;
    [HideInInspector]
    public bool released;
    [HideInInspector]
    public bool scored = false;

    public float currentAngle;
    private float startAngle;
    private float targetAngle;
    private int rotating = 0;

    private const float rotateSpeed = 270;
    private const float quarterTurn = 90f;


    // Start is called before the first frame update
    //void Awake()
    //{
    //    sr = GetComponent<SpriteRenderer>();
    //}

    // Update is called once per frame
    void Update()
    {
        if( rotating != 0 )
        {
            currentAngle += rotating * rotateSpeed * Time.deltaTime;

            if( Mathf.Abs( currentAngle - startAngle) > quarterTurn )
            {
                currentAngle = targetAngle;
                rotating = 0;
            }

            transform.localRotation = Quaternion.Euler(0, 0, currentAngle);
        }
    }

    public void Initialize( float angle )
    {
        currentAngle = angle;
    }

    public void ToggleOutline( bool show )
    {
        if ( show )
            outline.gameObject.SetActive(true);
        else
            outline.gameObject.SetActive(false);
    }

    public void Rotate( bool clockwise )
    {
        if( rotating == 0 )
        {
            rotating = clockwise ? 1 : -1;

            startAngle = currentAngle;
            targetAngle = startAngle + rotating * quarterTurn;

            //Debug.Log($"Rotating from {startAngle} to {targetAngle}");
            Audio.instance.PlaySFX(rotateSFX, transform.position);
        }
    }

    public void Destruction( float delay )
    {
        Level.instance.PieceDestroyed(this);
        Destroy(gameObject, delay);
    }
}
