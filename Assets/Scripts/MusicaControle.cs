using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicaControle : MonoBehaviour
{
    private static MusicaControle musicaControle = null;

    private void Awake()
    {
        if(musicaControle != null)
        {
            Destroy(gameObject);
        }
        else
        {
            musicaControle = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
