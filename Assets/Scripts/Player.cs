using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public LoadingDock loadZone;

    //public float vertForce = 1500f;
    //public float horzForce = 1250f;

    DistanceJoint2D distanceJoint;
    Rigidbody2D body2D;

    public Piece currentPiece;
    public LineRenderer attachLine;
    public bool paused;

    public float speed = 250f;
    private float heldGravity = 2f;
    private float droppedGravity = 0.5f;

    Vector2 input;

    // Start is called before the first frame update
    void Awake()
    {
        distanceJoint = GetComponent<DistanceJoint2D>();
        body2D = GetComponent<Rigidbody2D>();

        if ( currentPiece != null )
            currentPiece.grabbed = true;
    }

    // Update is called once per frame
    void Update()
    {
        input.y = Input.GetAxis("Vertical");

        input.x = Input.GetAxis("Horizontal");

        //float speed = 1000f;
        //body2D.velocity = new Vector2(horz * speed * Time.deltaTime, vert * speed * Time.deltaTime);
        //body2D.AddForce(new Vector2(horz * horzForce * Time.deltaTime, vert * vertForce * Time.deltaTime));
        //float speed = 10f;
        //body2D.MovePosition((Vector2)transform.position + new Vector2(horz * speed * Time.deltaTime, vert * speed * Time.deltaTime));

        if ( Input.GetKeyDown(KeyCode.Space))
        {
            if ( currentPiece != null )
            {
                DropPiece();
            }
            //else if( loadZone != null )
            //{
            //    GrabPiece(loadZone.GetNextPiece());
            //}
        }

        if( currentPiece != null )
        {
            if ( Input.GetKeyDown(KeyCode.Q) )
                RotatePiece(false);
            if ( Input.GetKeyDown(KeyCode.E) )
                RotatePiece(true);
            UpdateLine();
        }
    }

    private void FixedUpdate()
    {
        if ( !paused )
            body2D.velocity = input * speed * Time.deltaTime;
        else
            body2D.velocity = Vector2.zero;
    }

    private void GrabPiece(Piece piece)
    {
        if ( piece != null && !piece.grabbed)
        {
            // Set Physics
            distanceJoint.connectedBody = piece.body2D;
            currentPiece = piece;
            distanceJoint.enabled = true;
            piece.grabbed = true;
            piece.body2D.gravityScale = heldGravity;
            // Set Line
            UpdateLine();
            attachLine.gameObject.SetActive(true);

            loadZone.PieceGrabbed(piece);
        }
    }

    private void RotatePiece( bool clockwise )
    {
        if( currentPiece != null )
        {
            currentPiece.Rotate(clockwise);
        }
    }

    private void DropPiece()
    {
        Piece p = currentPiece;
        currentPiece = null;
        distanceJoint.enabled = false;
        attachLine.gameObject.SetActive(false);
        p.body2D.gravityScale = droppedGravity;
        p.body2D.constraints = RigidbodyConstraints2D.None;
        p.released = true;


        // Notify Load Zone
        loadZone.PieceDropped(p);
    }

    private void UpdateLine()
    {
        attachLine.SetPosition(0, transform.position);
        attachLine.SetPosition(1, currentPiece.transform.position);
    }

    private void OnTriggerEnter2D( Collider2D collision )
    {
        if ( currentPiece == null && collision.gameObject.CompareTag("Piece") )
        {
            GrabPiece(collision.gameObject.GetComponent<Piece>());
        }
    }

    //private void OnTriggerExit2D( Collider2D collision )
    //{
    //    loadZone = null;
    //    //Debug.Log("Exiting Loading Zone");
    //}

    //private void OnCollisionEnter2D( Collision2D collision )
    //{
    //    if( currentPiece == null && collision.gameObject.CompareTag("Piece"))
    //    {
    //        GrabPiece(collision.gameObject.GetComponent<Piece>());
    //    }
    //}
}
