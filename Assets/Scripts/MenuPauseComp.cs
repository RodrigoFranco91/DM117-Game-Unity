using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPauseComp : MonoBehaviour
{

    public static bool pausado;

    public GameObject menuPausePanel;

    /// <summary>
    /// Metodo para reiniciar a scene
    /// </summary>
    public void Restart()
    {
        Pause(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Metodo para pausar o jogo
    /// </summary>
    public void Pause(bool isPausado)
    {
        pausado = isPausado;

        // Se o jogo estiver pausado, timescale recebe 0
        // Se o jogo não estiver pausado, timescale recebe 1
        Time.timeScale = (pausado) ? 0 : 1;

        // Habilita/Desabilita o menu Pause
        menuPausePanel.SetActive(pausado);
    }

    /// <summary>
    /// Metodo para carregar uma scene
    /// </summary>
    public void CarregaScenne(string nomeScene)
    {
        SceneManager.LoadScene(nomeScene);
    }

    // Start is called before the first frame update
    void Start()
    {
        // pausado = false;
#if !UNITY_ADS
        Pause(false);
#endif
    }

    // Update is called once per frame
    void Update()
    {

    }
}
