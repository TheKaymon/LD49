using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingDock : MonoBehaviour
{
    public Transform createAt;
    public List<Piece> pieceList;
    public Piece loadedPiece;
    public LoadingUI ui;
    public int PiecesRemaining => ( pieces.Count );

    private Queue<Piece> pieces;

    // Start is called before the first frame update
    void Start()
    {
        // Convert List to Queue
        pieces = new Queue<Piece>(pieceList);
        pieceList.Clear();

        ui.SetPieces(pieces.ToArray());
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if( loadTimer < loadDuration )
    //    {
    //        loadTimer += Time.deltaTime;

    //        if ( loadTimer > loadDuration )
    //            LoadNextPiece();
    //    }
    //}

    public void LoadNextPiece()
    {
        if ( pieces.Count > 0 )
        {
            loadedPiece = Instantiate(pieces.Dequeue(), createAt.position, Quaternion.identity);
            // Move Queued Sprites

            ui.SetPieces(pieces.ToArray());
        }
    }

}
