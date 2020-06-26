using UnityEngine;

public class FimTileComportameto : MonoBehaviour
{
    [Header("Public Variables")]

    [Tooltip("Tempo esperado antes de destruir  o TileBasico")]
    public float tempoDestruir = 2.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        // Vamos ver se foi a bola que passou pelo fim do TileBasico
        if (other.GetComponent<JogadorComportamento>())
        {
            // Como foi a bola que pasou vamos criar um TileBasico no prox ponto
            // Mas esse proximo ponto esta depois do ultimo TileBasico presente na cena.
            GameObject.FindObjectOfType<ControladorJogo>().SpawnProxTile(true);

            // Destroi o TileBasico
            Destroy(transform.parent.gameObject, tempoDestruir);
        }
    }
}
