using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnityAdsController.initialize();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CarregaScene(string nomeScene)
    {
        SceneManager.LoadScene(nomeScene);

#if UNITY_ADS
        if (UnityAdsController.showAds)
        {
            UnityAdsController.showAd();
        }
#endif
    }
}
