using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanUP : MonoBehaviour
{
    private void Awake()
    {
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }  
    }
}
