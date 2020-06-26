using System;
using UnityEngine;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

public class UnityAdsController : MonoBehaviour
{

    public static bool showAds = true;

    public static DateTime? proxTempoReward = null; 

    // Referencia ao obstaculo
    public static ObstaculoComp obstaculo;

#if UNITY_IOS
    private static string gameId = "3665496";
#elif UNITY_ANDROID
    private static string gameId = "3665497";
#endif

    public static void showAd()
    {
#if UNITY_ADS
        // Opções para o Ad
        ShowOptions opcoes = new ShowOptions();
        opcoes.resultCallback = Unpause;

       
        if (Advertisement.IsReady())
        {
            Advertisement.Show(opcoes);
        }

        MenuPauseComp.pausado = true;
        Time.timeScale = 0.0f;

#endif
    }

    public static void Unpause(ShowResult result)
    {
        // Quando o anucio sai do modo pausado.
        MenuPauseComp.pausado = false;
        Time.timeScale = 1.0f;
    }

    public static void initialize()
    {
        Advertisement.Initialize(gameId);
    }

    /// <summary>
    /// Metodo para mostrar ad com recompensa
    /// </summary>
    public static void ShowRewardAd()
    {
#if UNITY_ADS
        proxTempoReward = DateTime.Now.AddSeconds(15);
        if (Advertisement.IsReady())
        {
            // Pausa o jogo
            MenuPauseComp.pausado = true;
            Time.timeScale = 0.0f;

            // Outra forma de criar o ShowOptions e setar o callback
            var opcoes = new ShowOptions
            {
                resultCallback = TratarMostarResultado
            };

            Advertisement.Show(opcoes);
        }
#endif
    }

    /// <summary>
    /// Metodo para tratar o resultado com reward (recompensa)
    /// </summary>
#if UNITY_ADS
    public static void TratarMostarResultado(ShowResult result)
    {

        switch (result)
        {
            case ShowResult.Finished:
                // Anuncio mostrado, Continue o jogo
                obstaculo.Continue();
            break;

            case ShowResult.Skipped:
                Debug.Log("Ad pulado, faz nada");
            break;

            case ShowResult.Failed:
                Debug.LogError("Erro no add, faz nada");
            break;
        }

        // Saia do modo pausado
        MenuPauseComp.pausado = false;
        Time.timeScale = 1.0f;
    }
#endif

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
