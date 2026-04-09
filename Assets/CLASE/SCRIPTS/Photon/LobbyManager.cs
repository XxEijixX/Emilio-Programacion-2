using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [Header("Lobby Config")]
    [SerializeField] private Transform viewportContent;
    [SerializeField] private GameObject lobbyPrefab;
    [SerializeField] private GameObject warningMessage;

    [Header("Panel Create Lobby")]
    [SerializeField] private GameObject panelCreateLobby;
    [SerializeField] private TMP_InputField inputFieldNameLobby;
    [SerializeField] private TMP_InputField inputFieldPlayerCount;
    [SerializeField] private TextMeshProUGUI errorMessage;
                                                           
    [Header("Game Start")]
    [SerializeField] private GameObject targets;          // el GameObject "targets"
    [SerializeField] private GameObject waitingPanel;     // el padre del texto de espera
    [SerializeField] private TextMeshProUGUI waitingText; // el TMP del mensaje

    private void Start()
    {
        PhotonManager._PhotonManager.onSessionListUpdated += DestroyCanvasContent;
        PhotonManager._PhotonManager.onSessionListUpdated += UpdateSesioncanvas;

        PhotonManager._PhotonManager.onLobbyFull += OnLobbyFull;
        PhotonManager._PhotonManager.onPlayerCountChanged += ActualizarConteoJugadores;

        if (panelCreateLobby != null) panelCreateLobby.SetActive(false);
        if (inputFieldPlayerCount != null) inputFieldPlayerCount.text = "4";

        if (targets != null) targets.SetActive(false);
        if (waitingPanel != null) waitingPanel.SetActive(true);

        ActualizarConteoJugadores();
    }

    private void OnLobbyFull()
    {
        if (targets != null) targets.SetActive(true);
        if (waitingPanel != null) waitingPanel.SetActive(false);
    }

    private void ActualizarConteoJugadores()
    {
        Debug.Log($"waitingText es null: {waitingText == null}");
        if (waitingText == null) return;
        if (waitingPanel != null) waitingPanel.SetActive(true);
        int current = PhotonManager._PhotonManager.CurrentPlayers;
        int max = PhotonManager._PhotonManager.MaxPlayers;
        waitingText.text = $"Waiting for players... {current}/{max}";
        Debug.Log($"Texto actualizado: {current}/{max}");
    }

    public void OnSearchLobbiesPressed()
    {
        PhotonManager._PhotonManager.JoinLobby();
    }

    public void OnCreateSessionButtonPressed()
    {
        if (panelCreateLobby != null) panelCreateLobby.SetActive(true);
    }

    public void OnConfirmCreateSession()
    {
        string sessionName = inputFieldNameLobby != null ? inputFieldNameLobby.text : "New Session";

        // Validar nombre de sesión 
        if (string.IsNullOrEmpty(sessionName) || sessionName.Trim().Length < 3 || sessionName.Trim().Length > 6)
        {
            if (errorMessage != null) errorMessage.text = "Session name must be between 3 and 6 characters long";
            return;
        }

        int maxPlayers = 4;

        // Validar cantidad de jugadores
        if (inputFieldPlayerCount != null && int.TryParse(inputFieldPlayerCount.text, out int inputPlayers))
        {
            if (inputPlayers > 4)
            {
                if (errorMessage != null) errorMessage.text = "Maximum 4 players allowed";
                inputFieldPlayerCount.text = "4";
                return;
            }
            else if (inputPlayers < 2)
            {
                if (errorMessage != null) errorMessage.text = "Minimum 2 player required";
                inputFieldPlayerCount.text = "2";
                return;
            }
            else
            {
                maxPlayers = inputPlayers; // Usar el valor ingresado si es válido
            }
        }

        Debug.Log($"Creating session: '{sessionName}' with {maxPlayers} max players");

        PhotonManager._PhotonManager.CreateSession(sessionName, maxPlayers);

        // Limpiar inputs
        if (inputFieldNameLobby != null) inputFieldNameLobby.text = "";
        if (inputFieldPlayerCount != null) inputFieldPlayerCount.text = "4";
        if (panelCreateLobby != null) panelCreateLobby.SetActive(false);
    }

    public void OnCancelCreateSession() // Cierra el panel de creación sin hacer nada
    {
        if (inputFieldNameLobby != null) inputFieldNameLobby.text = "";
        if (inputFieldPlayerCount != null) inputFieldPlayerCount.text = "4";
        if (panelCreateLobby != null) panelCreateLobby.SetActive(false);
    }

    public void UpdateSesioncanvas()
    {
        foreach (SessionInfo session in PhotonManager._PhotonManager.avaibleSessions)
        {
            GameObject sessionInstance = Instantiate(lobbyPrefab, viewportContent);
            sessionInstance.GetComponent<SessionEntry>().SetInfo(session);
        }
    }

    public void DestroyCanvasContent()
    {
        if (viewportContent != null)
        {
            for (int i = 0; i < viewportContent.childCount; i++)
            {
                Destroy(viewportContent.GetChild(i).gameObject);
            }
        }

        if (warningMessage != null)
        {
            bool hasLobbies = PhotonManager._PhotonManager.avaibleSessions != null && PhotonManager._PhotonManager.avaibleSessions.Count > 0;

            if (!hasLobbies)
            {
                if (warningMessage.GetComponentInChildren<TextMeshProUGUI>() != null) warningMessage.GetComponentInChildren<TextMeshProUGUI>().text = "No Lobbies Available";
                if (warningMessage.GetComponent<Image>() != null) warningMessage.GetComponent<Image>().color = Color.red;
            }
            else
            {
                if (warningMessage.GetComponentInChildren<TextMeshProUGUI>() != null) warningMessage.GetComponentInChildren<TextMeshProUGUI>().text = "Lobbies Available";
                if (warningMessage.GetComponent<Image>() != null) warningMessage.GetComponent<Image>().color = Color.green;
            }
        }
    }

    private void OnDestroy()
    {
        if (PhotonManager._PhotonManager == null) return;
        PhotonManager._PhotonManager.onSessionListUpdated -= DestroyCanvasContent;
        PhotonManager._PhotonManager.onSessionListUpdated -= UpdateSesioncanvas;
        PhotonManager._PhotonManager.onLobbyFull -= OnLobbyFull;
        PhotonManager._PhotonManager.onPlayerCountChanged -= ActualizarConteoJugadores;
    }
}