using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controleCamera : MonoBehaviour
{
    [Header("Public Variables")]
    
    [Tooltip("Alvo a ser acompanhado pela camera")]
    public Transform alvo;

    [Tooltip("Offset da camera em relação ao alvo")]
    public Vector3 offset = new Vector3(0, 3, -6);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(alvo != null)
        {
            // Altera a posição da camera
            transform.position = alvo.position + offset;

            // altera a rotação da camera em relação ao alvo
            transform.LookAt(alvo);
        }
    }
}
