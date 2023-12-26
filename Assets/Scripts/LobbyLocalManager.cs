using System.Net.Sockets;
using System.Net;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;
using UnityEngine.UI;

public class LobbyLocalManager : MonoBehaviour
{
    [SerializeField] Button _createButton;
    [SerializeField] Button _joinButton;

    [SerializeField] TextMeshProUGUI _ipAddressText;

    [SerializeField] TMP_InputField _ipInput;

    [SerializeField] string _ipAddress;
    [SerializeField] UnityTransport _transport;

    void Start()
    {

        _createButton.onClick.AddListener(StartHost);
        _joinButton.onClick.AddListener(StartClient);

        GetLocalIPAddress();
        SetIpAddress();
    }

    // To Host a game
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("GameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
        
    }

    // To Join a game
    public void StartClient()
    {
        _ipAddress = _ipInput.text;
        SetIpAddress();
        NetworkManager.Singleton.StartClient();
    }
    
    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                _ipAddressText.text = ip.ToString();
                _ipAddress = ip.ToString();
                Debug.Log(_ipAddress);
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
    public void SetIpAddress()
    {
        _transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        _transport.ConnectionData.Address = _ipAddress;
        Debug.Log(_transport.ConnectionData.Address);
    }
}
