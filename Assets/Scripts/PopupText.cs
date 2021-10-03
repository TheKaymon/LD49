using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupText : MonoBehaviour
{
    public TextMeshPro text;
    private float speed;
    private float lifeTime = 1f;
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
        text.SetText($"+{amount}");
    }
}
