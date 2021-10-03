using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupWarning : MonoBehaviour
{
    public TextMeshPro text;
    public SpriteRenderer right;
    public SpriteRenderer left;

    public Sprite sideArrow;
    public Sprite downArrow;

    private float timer;
    private float flashTimer;
    private float flashInterval = 0.5f;

    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    void Update()
    {
        // Flash Timer
        flashTimer += Time.deltaTime;
        if(flashTimer > flashInterval )
        {
            right.enabled = !right.enabled;
            left.enabled = !left.enabled;
            flashTimer = 0f;
        }

        timer -= Time.deltaTime;

        if ( timer > 0 )
            text.SetText(Mathf.CeilToInt(timer).ToString());
        else
            gameObject.SetActive(false);
    }

    public void Initialize( float num, int direction )
    {
        timer = num;
        text.SetText(Mathf.CeilToInt(timer).ToString());
        flashTimer = 0;

        if ( direction == -1 )
        {
            left.sprite = sideArrow;
            left.enabled = true;
            left.gameObject.SetActive(true);
            right.gameObject.SetActive(false);
        }
        else if( direction == 1 )
        {
            right.sprite = sideArrow;
            right.enabled = true;
            right.gameObject.SetActive(true);
            left.gameObject.SetActive(false);
        }
        else
        {
            left.sprite = downArrow;
            right.sprite = downArrow;
            right.enabled = true;
            left.enabled = true;
            right.gameObject.SetActive(true);
            left.gameObject.SetActive(true);
        }

        gameObject.SetActive(true);
    }

}
