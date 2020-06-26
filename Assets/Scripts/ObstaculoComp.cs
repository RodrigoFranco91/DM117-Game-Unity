using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ObstaculoComp : MonoBehaviour
{

    [Header("Public Variables")]

    [Tooltip("Quanto tempo antes de reiniciar o jogo")]
    public float tempoEspera = 2.0f;

    [Tooltip("Particle system da explosao")]
    public GameObject explosao;

    [Tooltip("Acesso para o componente MeshRender")]
    MeshRenderer mr = new MeshRenderer();

    [Tooltip("Acesso para o componente BoxCollider")]
    BoxCollider bc = new BoxCollider();

    public AudioSource music;

    private static int contador = 0;


    public static Text texto;

    [Header("Private Variables")]

    [SerializeField]
    [Tooltip("Nome da scene")]
    ///<summary>
    /// Nome da scene
    /// </summary>
    private string SceneName;

    [SerializeField]
    [Tooltip("Variavel referencia para o jogador")]
    /// <summary>
    /// Variavel referencia para o jogador
    /// </summary>
    private GameObject jogador;

    public void OnCollisionEnter(Collision collision)
    {
        // Verifica se o colidiu com o jogador
        if (collision.gameObject.GetComponent<JogadorComportamento>())
        {

           if( collision.gameObject.name.Equals("Jogador"))
            {
                collision.gameObject.GetComponent<AudioSource>().Play();
            }

            jogador = collision.gameObject;

            // Vamos esconder o jogador
            collision.gameObject.SetActive(false);

            // Destroy(collision.gameObject);

            // Chama a função ResetaJogo depois de um tempo
            Invoke("ResetaJogo", tempoEspera);

            

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Pega o nome da scene no inicio do jogo
        SceneName = SceneManager.GetActiveScene().name;
        mr = GetComponent<MeshRenderer>();
        bc = GetComponent<BoxCollider>();
        music = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Reinicia o jogo
    /// </summary>
    void ResetaJogo()
    {
        // Faz o menu GameOver aparecer
        var gameOverMenu = GetGameOverMenu();
        gameOverMenu.SetActive(true);

        // Busca os botoe sdo menu GameOver
        var botoes = gameOverMenu.transform.GetComponentsInChildren<Button>();

        // Define o botão continue
        Button botaoContinue = null;

        // Varre todos os botoes, em busca do botão continue
        foreach(var botao in botoes)
        {
            if (botao.gameObject.name.Equals("BotaoContinuar"))
            {
                // Salva uma referencia para o botão continue;
                botaoContinue = botao;
                break;
            }
        }

        if (botaoContinue)
        {
#if UNITY_ADS
            // Se botão continue for clicado, invoca o metodo ShowRewardAd
            //botaoContinue.onClick.AddListener(UnityAdsController.ShowRewardAd);

            // Seta a referencia do abustaculo com base no this
            // UnityAdsController.obstaculo = this;
            StartCoroutine(ShowContinue(botaoContinue));
#else
            // Se não existe ad, não precisa mostar o botão continue
            botaoContinue.gameObject.SetActive(false);
#endif
        }

        // Reinicia o jogo (Fase atual);
        // Não é necessario recarregar o jogo por aqui.
        // SceneManager.LoadScene(SceneName);
    }

    public IEnumerator ShowContinue(Button botaoContinue)
    {
        var btnText = botaoContinue.GetComponentInChildren<Text>();
        while (true)
        {
            if(UnityAdsController.proxTempoReward.HasValue && (DateTime.Now < UnityAdsController.proxTempoReward.Value))
            {
                botaoContinue.interactable = false;
                TimeSpan restante = UnityAdsController.proxTempoReward.Value - DateTime.Now;
                var ContagemRegressiva = string.Format("{0:D2}:{1:D2}", restante.Minutes, restante.Seconds);

                btnText.text = ContagemRegressiva;

                yield return new WaitForSeconds(1f);
            }
            else
            {
                botaoContinue.interactable = true;
                botaoContinue.onClick.AddListener(UnityAdsController.ShowRewardAd);
                UnityAdsController.obstaculo = this;
                btnText.text = "Continue (Ver Ad)";
                break;
            }
        }
    }

    /// <summary>
    /// Faz o reset do jogo
    /// </summary>
    public void Continue()
    {
        var go = GetGameOverMenu();
        go.SetActive(false);
        jogador.SetActive(true);

        // Explode o obstaculo, caso o jogador resolva apertar continue;
        ObstaculoTocado();
    }

    /// <summary>
    /// Busca pelo menu GameOver
    /// </summary>
    GameObject GetGameOverMenu()
    {
        return GameObject.Find("Canvas").transform.Find("MenuGameOver").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {

            // Destroy inimigos
            tocarObjetos(Input.mousePosition);
        }
    }

    /// <summary>
    /// Metodo para identificar se objetos foram tocados
    /// </summary>
    /// <param name="toque"> Toque ocorido nesse frame </param>
    private static void tocarObjetos(Vector2 toque)
    {

        // Convertendo a posição do toque (Screen Space) para um Ray
        Ray toqueRay = Camera.main.ScreenPointToRay(toque);

        RaycastHit hit;

        if (Physics.Raycast(toqueRay, out hit))
        {
 
            hit.transform.SendMessage("ObstaculoTocado", SendMessageOptions.DontRequireReceiver);


        }
    }

    /// <summary>
    /// Metofo invocado atraves do SendMessage(), para detectar que este objeto foi tocado
    /// </summary>
    public void ObstaculoTocado()
    {

        if (explosao != null)
        {
            var particulas = Instantiate(explosao, transform.position, Quaternion.identity);
            music.Play();

            Destroy(particulas, 1.0f);
            contador++;
            var botaoContador = GameObject.Find("Canvas").transform.Find("Contador").gameObject;
            botaoContador.GetComponentInChildren<Text>().text = contador.ToString();
            texto.text = contador.ToString();
        }
        mr.enabled = false;
        bc.enabled = false;
       // Destroy(this.gameObject);
    }

}
