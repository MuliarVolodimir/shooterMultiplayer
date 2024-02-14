using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public enum Scene
    { 
        MainMenuScene,
        LobbyScene,
        GameScene,
        LobbyOnlineCreateScene
    }

    public static void Load(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
    }
    
    public static void LoadNetwork(Scene scene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
    }
}
