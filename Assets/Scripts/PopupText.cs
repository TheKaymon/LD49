using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupText : MonoBehaviour
{
    public TextMeshPro text;
    public AudioClip pointsLost;

    private float speed = 1.25f;
    private float lifeTime = 1.5f;
    private float timer = 0;


    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if ( timer > lifeTime )
            Destroy(gameObject);
        else
            transform.Translate(0, speed * Time.deltaTime, 0);

    }

    public void Initialize( int amount ) //color
    {
        if ( amount < 0 )
        {
            text.color = Color.red;
            text.SetText($"{amount}");
            Audio.instance.PlaySFX(pointsLost, transform.position);
        }
        else
            text.SetText($"+{amount}");
    }
}
