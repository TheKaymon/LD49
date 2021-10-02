using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public LoadingDock loadZone;

    public float vertForce = 1500f;
    public float horzForce = 1250f;

    DistanceJoint2D distanceJoint;
    Rigidbody2D body2D;

    public Piece currentPiece;
    public LineRenderer attachLine;


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
        float vert = Input.GetAxis("Vertical");

        float horz = Input.GetAxis("Horizontal");

        body2D.AddForce(new Vector2(horz * horzForce * Time.deltaTime, vert * vertForce * Time.deltaTime));

        if( Input.GetKeyDown(KeyCode.Space))
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

    private void GrabPiece(Piece piece)
    {
        if ( piece != null && !piece.grabbed)
        {
            // Set Physics
            distanceJoint.connectedBody = piece.body2D;
            currentPiece = piece;
            distanceJoint.enabled = true;
            piece.grabbed = true;
            // Set Line
            UpdateLine();
            attachLine.gameObject.SetActive(true);

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
        distanceJoint.enabled = false;
        attachLine.gameObject.SetActive(false);
        currentPiece.body2D.constraints = RigidbodyConstraints2D.None;
        currentPiece.released = true;
        currentPiece = null;

        // Notify Load Zone
        loadZone.LoadNextPiece();
    }

    private void UpdateLine()
    {
        attachLine.SetPosition(0, transform.position);
        attachLine.SetPosition(1, currentPiece.transform.position);
    }

    //private void OnTriggerEnter2D( Collider2D collision )
    //{
    //    loadZone = collision.GetComponent<LoadingDock>();
    //    //Debug.Log("Entering Loading Zone");
    //}

    //private void OnTriggerExit2D( Collider2D collision )
    //{
    //    loadZone = null;
    //    //Debug.Log("Exiting Loading Zone");
    //}

    private void OnCollisionEnter2D( Collision2D collision )
    {
        if( currentPiece == null && collision.gameObject.CompareTag("Piece"))
        {
            GrabPiece(collision.gameObject.GetComponent<Piece>());
        }
    }
}
