using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    public List<Image> pieceImages;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPieces( Piece[] pieces )
    {
        int i = 0;

        for ( i = 0; i < pieceImages.Count; i++ )
        {
            if ( i < pieces.Length )
            {
                pieceImages[i].sprite = pieces[i].GetSprite();
                pieceImages[i].gameObject.SetActive(true);
            }
            else
            {
                pieceImages[i].gameObject.SetActive(false);
            }
        }
    }
}
