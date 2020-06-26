using System.Collections.Generic;
using UnityEngine;

public class ControladorJogo : MonoBehaviour
{
    [Header("Public Variables")]

    [Tooltip("Referencia para tile basic")]
    public Transform tile;

    [Tooltip("Referencia para o Obstaculo")]
    public Transform obstaculo;

    [Tooltip("Ponto para colocar o tile basico inicial")]
    public Vector3 pontoInicial = new Vector3(0, 0, -5);

    [Tooltip("Quantidade de Tiles Iniciais")]
    public int numSpawnIni;

    [Tooltip("Numero de tiles sem obstaculos")]
    public int numTilesSemObs = 4;



    [Header("Private Variables")]

    [SerializeField]
    [Tooltip("Local para spaw do proximo Tile")]
    ///<summary>
    /// Local para spaw do proximo Tile
    /// </summary>
    private Vector3 proxTilePos;

    [SerializeField]
    [Tooltip("Rotação do proximo Tile")]
    ///<summary>
    /// Rotação do proximo Tile
    /// </summary>
    private Quaternion proxTileRot;

    // Start is called before the first frame update
    void Start()
    {
        proxTilePos = pontoInicial;
        proxTileRot = Quaternion.identity;

#if UNITY_ADS
        UnityAdsController.initialize();
#endif
        for (int i = 0; i < numSpawnIni; i++)
        {
            SpawnProxTile(i >= numTilesSemObs);
        }
    }

    public void SpawnProxTile(bool spawObstaculos)
    {

        // Instancia o next tile
        var novoTile = Instantiate(tile, proxTilePos, proxTileRot);

        // Busca o ponto de referencia para o spawn do tile
        var proxTile = novoTile.Find("PontoSpawn");
        
        // Seta o posição e rotação do proximo ponto de spawn
        proxTilePos = proxTile.position;
        proxTileRot = proxTile.rotation;

        if (!spawObstaculos)
            return;

        // Pode spawnar obstaculos
        // Pega todos os possiveis pontos de spaw
        var pontosObstaculo = new List<GameObject>();

        // Varrer os GOs filhos buscando os pontos de spawn
        foreach (Transform filho in novoTile)
        {
            if (filho.CompareTag("obsSpawn"))
            {
                pontosObstaculo.Add(filho.gameObject);
            }
        }

        // Verifica se tem os pontos de spawn
        if (pontosObstaculo.Count > 0)
        {
            // Pega um index dos pontos aleatorios
            var spawnIndex = Random.Range(0, pontosObstaculo.Count);

            // Pega o GO com base no index
            var pontoSpawn = pontosObstaculo[spawnIndex];

            // Pega a posição do GO
            var obsSpawnPos = pontoSpawn.transform.position;

            // Instancia o obstaculo na posição do ponto de spawn specifico
            var novoObs = Instantiate(obstaculo, obsSpawnPos, Quaternion.identity);

            // Seta o obstaculo como filho do tile inicial
            novoObs.SetParent(pontoSpawn.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
