using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingDock : MonoBehaviour
{
    public Transform createAtLeft;
    public Transform createAtRight;
    public List<Piece> pieceList;
    public Piece loadedPiece;
    public LoadingUI ui;
    public int PiecesRemaining => ( pieces.Count );
    public bool autoDrop = false;
    public bool alternateDrops = false;
    public float dropInterval = 5f;
    public GameObject beamPrefab;
    public int beamInterval = 10;

    [HideInInspector]
    public List<Piece> droppedPieces;
    private List<Piece> activePieces;
    private float dropTimer = 0f;
    private bool lastDropLeft = false;
    private Queue<Piece> pieces;
    private int pieceCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Convert List to Queue
        pieces = new Queue<Piece>(pieceList);
        pieceList.Clear();
        activePieces = new List<Piece>();
        droppedPieces = new List<Piece>();

        ui.SetPieces(pieces.ToArray());
        GenerateNextPiece();
    }

    // Update is called once per frame
    void Update()
    {
        if ( autoDrop && PiecesRemaining > 0 )
        {
            dropTimer += Time.deltaTime;

            if ( dropTimer > dropInterval )
            {
                dropTimer = 0f;
                GenerateNextPiece();
            }
        }
    }

    public void PieceGrabbed( Piece p )
    {
        activePieces.Remove(p);
    }

    public void PieceDestroyed( Piece p )
    {
        activePieces.Remove(p);
        droppedPieces.Remove(p);
    }

    public void PieceDropped( Piece p )
    {
        droppedPieces.Add(p);
        if ( !autoDrop )
            GenerateNextPiece();
        if ( PiecesRemaining <= 0 && activePieces.Count <= 0 )
        {
            GameManager.level.LastPieceDropped();
        }
    }

    public void GenerateNextPiece()
    {
        if( pieceCounter >= beamInterval )
        {
            Instantiate(beamPrefab, new Vector2(0,createAtLeft.position.y), Quaternion.identity);
            pieceCounter = 0;
        }
        else if ( pieces.Count > 0 )
        {
            int rotate = Random.Range(0, 4) * 90;
            Vector3 position = ( alternateDrops && lastDropLeft ) ? createAtRight.position : createAtLeft.position;
            if ( alternateDrops )
                lastDropLeft = !lastDropLeft;
            Quaternion rotation = Quaternion.Euler(0, 0, rotate);
            loadedPiece = Instantiate(pieces.Dequeue(), position, rotation);
            loadedPiece.currentAngle = rotate;
            activePieces.Add(loadedPiece);
            pieceCounter++;
            if ( pieceCounter >= beamInterval )
                dropTimer = -5f;
            // Move Queued Sprites

            ui.SetPieces(pieces.ToArray());
            // Create UI Counter
        }
    }
}
