using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PhotonManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkRunner runner;
    [SerializeField] NetworkSceneManagerDefault sceneManager;
    [SerializeField] private NetworkPrefabRef prefabPlayer;
    [SerializeField] private GameObject canvas;
    [SerializeField] private UnityEvent onGameStarted;

    Dictionary<PlayerRef, NetworkObject> players = new Dictionary<PlayerRef, NetworkObject>();
    public List<SessionInfo> avaibleSessions = new List<SessionInfo>();
    public event Action onSessionListUpdated;
    public static PhotonManager _PhotonManager;

    private string _sessionName; // nombre personalizado para la sesión,
    private int _maxPlayers = 4; // cantidad máxima de jugadores personalizada

    private void Awake()
    {
        if (_PhotonManager == null) _PhotonManager = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        runner.AddCallbacks(this);
    }

    public async void JoinLobby()
    {
        if (runner.IsRunning) return;
        await runner.JoinSessionLobby(SessionLobby.ClientServer);
    }

    #region Metodos de Photon
    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnInput(NetworkRunner runner_, NetworkInput input)
    {
        if (InputManager.Instance == null)
        {
            Debug.LogWarning("InputManager.Instance es null en OnInput");
            return;
        }

        var buttons = new NetworkButtons();
        buttons.Set((int)NetworkInfoData.BotonDisparo, InputManager.Instance.BotonDisparoPresionado());
        buttons.Set((int)NetworkInfoData.BotonCorrer, InputManager.Instance.WasRunInputPressed());

        NetworkInfoData data = new NetworkInfoData()
        {
            move = InputManager.Instance.GetMoveInput(),
            look = InputManager.Instance.GetMouseDelta(),
            yRotation = Camera.main != null ? Camera.main.transform.eulerAngles.y : 0f,
            buttons = buttons,
        };

        input.Set(data);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) return;

        if (SceneManager.GetActiveScene().name == "Victoria")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (!players.ContainsKey(player))
        {
            Debug.Log($"Player Joined: {player.PlayerId}");
            Vector3 spawnPosition = new Vector3(player.RawEncoded % runner.Config.Simulation.PlayerCount * 0f, 12f, -15f);
            NetworkObject networkObject = runner.Spawn(prefabPlayer, spawnPosition, Quaternion.identity, player);
            players.Add(player, networkObject);
        }
        else
        {
            Debug.Log($"Player {player.PlayerId} already exists in playerList.");
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (players.ContainsKey(player))
        {
            if (players[player] != null)
            {
                runner.Despawn(players[player]);
            }
            players.Remove(player);
        }
    }

    #endregion

    public async void JoinSession(string sessionname)
    {
        runner.ProvideInput = true;

        var scene = SceneRef.FromIndex(0);
        var sceneInfo = new NetworkSceneInfo();

        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = sessionname,
            Scene = scene,
            SceneManager = sceneManager,
            IsVisible = true
        });

        if (canvas != null) canvas.SetActive(false);
    }

    public void CreateSession(string sessionName, int maxPlayers)
    {
        // Forzar máximo 4 jugadores
        _maxPlayers = Mathf.Clamp(maxPlayers, 1, 4);

        // Forzar máximo 6 caracteres en el nombre, o nombre random si viene vacío
        _sessionName = string.IsNullOrWhiteSpace(sessionName)
            ? RandomSessionName(6)
            : sessionName.Substring(0, Mathf.Min(sessionName.Length, 6));

        StartGame(GameMode.Host);
    }

    private async void StartGame(GameMode mode)
    {
        runner.AddCallbacks(this);
        runner.ProvideInput = true;

        var scene = SceneRef.FromIndex(0);
        var sceneInfo = new NetworkSceneInfo();

        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        string finalSessionName = string.IsNullOrEmpty(_sessionName) ? RandomSessionName(6) : _sessionName; // Usar el nombre personalizado si existe, sino generar uno aleatorio

        int finalMaxPlayers = Mathf.Clamp(_maxPlayers, 1, 4);

        Debug.Log($"Starting game - Session: {finalSessionName}, MaxPlayers: {finalMaxPlayers}");

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = finalSessionName,
            PlayerCount = finalMaxPlayers,
            Scene = scene,
            SceneManager = sceneManager,
            IsVisible = true,
            SessionProperties = new Dictionary<string, SessionProperty>() //propiedad personalizada de MaxPlayers a la sesión
            {
                { "MaxPlayers", finalMaxPlayers }
            }
        });

        if (canvas != null) canvas.SetActive(false);

        onGameStarted?.Invoke();

        _sessionName = null;
        _maxPlayers = 4;
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("Available sessions count: " + sessionList.Count);
        avaibleSessions = sessionList;
        onSessionListUpdated?.Invoke();
    }

    public void StartGameAsHost()
    {
        _sessionName = null; // Nombre aleatorio
        _maxPlayers = 4; // 4 jugadores por defecto
        StartGame(GameMode.Host);
    }

    public void StartGameAsClient()
    {
        StartGame(GameMode.Client);
    }

    private string RandomSessionName(int sessionNameLength)
    {
        string caracters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        string sessionName = "";
        for (int i = 0; i < sessionNameLength; i++)
        {
            char randomChar = caracters[Random.Range(0, caracters.Length)];
            sessionName += randomChar;
        }
        return sessionName;
    }
}