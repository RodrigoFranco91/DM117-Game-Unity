using System.Runtime.Serialization;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class JogadorComportamento : MonoBehaviour
{
    public enum TipoMovimentoHorizontal
    {
        Acelerometro,
        Touch
    }

    public static AudioSource music;

    [Tooltip("Tipo de orientação de movimentação")]
    public TipoMovimentoHorizontal movimentoHorizontal = TipoMovimentoHorizontal.Acelerometro;

    [Header("Public Variables")]

    [Tooltip("Velocidade que a bola ira esquivar para os lados")]
    [Range(0, 10)]
    public float velocidadeEsquiva = 5.0f;

    [Tooltip("Velocidade que a bola ira rolar para fente ou para tras")]
    [Range(0, 10)]
    public float velocidadeRolamento = 5.0f;

    [Tooltip("Particle system da explosao")]
    public GameObject explosao;


    [Header("Atribustos responsaveis pelo swipe")]

    [Tooltip("Determina qual a distancia que o dedo do jogador deve deslocar pela tela - considerando swipe")]
    public float minDisSwipe = 2.0f;

    [Tooltip("Distancia que o jgadr (bola) ira percorrer atraves do swipe")]
    public float swipeMove = 2.0f;

    [SerializeField]
    [Tooltip("Ponto inicial onde o swipe ocoreu")]
    /// <summary>
    /// Ponto inicial onde o swipe ocoreu
    /// </summary>
    private Vector2 toqueInicio;


    [Header("Private Variables")]

    [SerializeField]
    [Tooltip("Referencia para o componente RigidBody, pegara automaticamente ao iniciar o jogo")]
    /// <summary>
    /// Referencia para o componente RigidBody, pegara automaticamente ao iniciar o jogo
    /// </summary>
    private Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        // Pega o componente Rigidbody a qual foi atrelado
        rb = GetComponent<Rigidbody>();
        music = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        
        // Se o jogo esta pausado não faça nada
        if (MenuPauseComp.pausado)
        {
            return;
        }

        var velocidadeHorizontal = Input.GetAxis("Horizontal") * velocidadeEsquiva;

#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBPLAYER

        // Detecta click na tela ou touch
        // Click
        //      0 -> Botão Esquerdo
        //      1 -> Botão Direito
        //      2 -> Botão Meio
        // Touch
        //      0 -> Touch
        if (Input.GetMouseButton(0))
        {
            // Set a direcao de movimentacao
            velocidadeHorizontal = CalculaMovimento(Input.mousePosition);
            // tocarObjetos(Input.mousePosition);
        }

#elif UNITY_IOS || UNITY_ANDROID

        if (movimentoHorizontal == TipoMovimentoHorizontal.Acelerometro)
        {
            // Seta a velocidade com base no acelerometro
            velocidadeHorizontal = Input.acceleration.x * velocidadeRolamento;
        }
        else
        {
            // Detectar movimento exclusivamente via touch
            if (Input.touchCount > 0)
            {
                // Pegando o primeiro toque na tela dentro do frame
                Touch toque = Input.touches[0];

                // Set a direcao de movimentacao
                velocidadeHorizontal = CalculaMovimento(toque.position);

                // Chama a funcao de verificacao de movimento
                SwipeTeleport(toque);
                tocarObjetos(toque);
            }
        }

#endif
        var forcaMoviment = new Vector3(velocidadeHorizontal, 0.0F, velocidadeRolamento);

        // Time.deltaTime: nos retorna o tempo gasto no frame anterior (algo em torno de 1/60fps)
        // Usamos esse valor para garantir que o nosso jogador(bola) se desloque com a mesma velocidade sem importar o hardware
        forcaMoviment *= (Time.deltaTime * 60);

        rb.AddForce(forcaMoviment);

        //rb.AddForce(new Vector3(velocidadeHorizontal, 0.0f, velocidadeRolamento));
    }

    /// <summary>
    /// Metodo para calcular para onde o jogador deslocara na horizontal
    /// </summary>
    private float CalculaMovimento(Vector2 screenSpaceCoords)
    {
        var pos = Camera.main.ScreenToViewportPoint(screenSpaceCoords);
        float direcaoX;

        if (pos.x < 0.5)
        {
            direcaoX = -1;
        }
        else
        {
            direcaoX = 1;
        }

        return direcaoX * velocidadeEsquiva;
    }

    private void SwipeTeleport(Touch toque)
    {
        // Verifica se esse é o ponto de inicio do swipe
        if (toque.phase == TouchPhase.Began)
        {
            // Pega a posicao do swipe
            toqueInicio = toque.position;
        }
        // Verifica se é o fim do toque
        else if (toque.phase == TouchPhase.Ended)
        {
            Vector2 toqueFim = toque.position;
            Vector3 direcaoMov;

            // Pega a deiferenca do inicio para o fim do swipe
            float diff = toqueFim.x - toqueInicio.x;

            // Verifica se o swipe percorreu a distancia minima para gerar a movimentacao
            if (Mathf.Abs(diff) >= minDisSwipe)
            {
                if (diff < 0)
                {
                    direcaoMov = Vector3.left;
                }
                else
                {
                    direcaoMov = Vector3.right;
                }
            }
            else
            {
                return;
            }

            // Raycast eh outra forma de detectar colisao com base na projecao
            RaycastHit hit;

            if (!rb.SweepTest(direcaoMov, out hit, swipeMove))
            {
                rb.MovePosition(rb.position + (direcaoMov * swipeMove));
            }
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
            
            // hit.transform.SendMessage("ObjetoTocado", SendMessageOptions.DontRequireReceiver);
            hit.transform.SendMessage("ObstaculoTocado", SendMessageOptions.DontRequireReceiver);
        }

    }

    /// <summary>
    /// Metofo invocado atraves do SendMessage(), para detectar que este objeto foi tocado
    /// </summary>
    public void ObjetoTocado()
    {
        if (explosao != null)
        {
            var particulas = Instantiate(explosao, transform.position, Quaternion.identity);
        }
        Destroy(this.gameObject, 1.0f);
    }

    public void OnCollisionEnter(Collision collision)
    {

        if(collision.gameObject.name.Equals("Parede_direita") || collision.gameObject.name.Equals("Parede_esquerda") || collision.gameObject.name.Equals("Obstaculo"))
        {
            music.Play();
        }
    }

}
