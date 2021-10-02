using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public Player player;
    public LoadingDock loadZone;
    public Transform scoringZoneParent;
    public LayerMask pieceMask;
    private List<BoxCollider2D> scoringZones;

    // Start is called before the first frame update
    void Start()
    {
        scoringZones = new List<BoxCollider2D>();

        for ( int i = 0; i < scoringZoneParent.childCount; i++ )
        {
            scoringZones.Add(scoringZoneParent.GetChild(i).GetComponent<BoxCollider2D>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EndLevel()
    {
        Piece p;
        List<Piece> countedPieces = new List<Piece>();
        int hits = 0;
        // TODO: Set Cap to Match Level Pieces
        Collider2D[] colliders = new Collider2D[32];
        // Setup Filter
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(pieceMask);
        //filter.NoFilter();
        int totalScore = 0;
        int score = 1;

        for ( int i = 0; i < scoringZones.Count; i++ )
        {
            hits = scoringZones[i].OverlapCollider(filter, colliders);
            //Debug.Log($"{hits} Hits!");
            for ( int h = 0; h < hits; h++ )
            {
                p = colliders[h].GetComponent<Piece>();
                if( !countedPieces.Contains(p) )
                {
                    totalScore += score;
                    countedPieces.Add(p);
                    Debug.Log($"{score} scored from {p}");
                }
            }

            score *= 2;
        }

        Debug.Log($"Level Score is: {totalScore}");
    }

    private void OnTriggerEnter2D( Collider2D collider )
    {
        //Debug.Log("Triggered Level End Zone!");
        //if ( collider.CompareTag("Piece") )
        //{
        //    collider.GetComponent<Piece>().Destruction(.5f);
        //}
        if ( collider.CompareTag("Player") )
        {
            if( player.currentPiece == null && loadZone.PiecesRemaining == 0 )
                EndLevel();
        }
    }
}
