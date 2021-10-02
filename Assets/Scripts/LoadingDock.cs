using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingDock : MonoBehaviour
{
    public Transform createAt;
    public List<Piece> pieceList;
    public DistanceJoint2D distanceJoint;
    public LineRenderer attachLine;
    public Piece loadedPiece;
    public LoadingUI ui;

    private float loadTimer = 0f;
    private float loadDuration = 1f;

    private Queue<Piece> pieces;

    // Start is called before the first frame update
    void Start()
    {
        // Convert List to Queue
        pieces = new Queue<Piece>(pieceList);
        pieceList.Clear();

        if( loadedPiece == null )
            LoadNextPiece();
        ui.SetPieces(pieces.ToArray());
    }

    // Update is called once per frame
    void Update()
    {
        if ( loadedPiece != null )
        {
            UpdateLine();
        }
        else if( loadTimer < loadDuration )
        {
            loadTimer += Time.deltaTime;

            if ( loadTimer > loadDuration )
                LoadNextPiece();
        }
    }

    public Piece GetNextPiece()
    {
        distanceJoint.enabled = false;
        attachLine.gameObject.SetActive(false);
        Piece next = loadedPiece;
        StartLoading();

        return next;
    }

    private void StartLoading()
    {
        loadedPiece = null;

        if ( pieces.Count > 0 )
            loadTimer = 0f;
    }

    private void LoadNextPiece()
    {
        //if( pieces.Count > 0 )
        loadedPiece = Instantiate(pieces.Dequeue(), createAt.position, Quaternion.identity);
            // Move Queued Sprites

        ui.SetPieces(pieces.ToArray());

        distanceJoint.connectedBody = loadedPiece.body2D;
        distanceJoint.enabled = true;
        UpdateLine();
        attachLine.gameObject.SetActive(true);
    }

    private void UpdateLine()
    {
        attachLine.SetPosition(0, createAt.position);
        attachLine.SetPosition(1, loadedPiece.transform.position);
    }

    private void OnTriggerEnter2D( Collider2D collider )
    {
        if( collider.CompareTag("Player") )
            Debug.Log("Player Entering");
    }

    private void OnTriggerExit2D( Collider2D collider )
    {
        if ( collider.CompareTag("Player") )
            Debug.Log("Player Exiting");
    }
}
