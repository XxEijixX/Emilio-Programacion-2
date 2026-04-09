using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sessionName;
    [SerializeField] private TextMeshProUGUI playerCount;
    [SerializeField] private Button joinButton;

    public void SetInfo(SessionInfo sessioninfo)
    {
        sessionName.text = sessioninfo.Name;

        int maxPlayers = 4;

        if (sessioninfo.Properties.TryGetValue("MaxPlayers", out var maxPlayersProperty)) maxPlayers = maxPlayersProperty;

        playerCount.text = $"{sessioninfo.PlayerCount}/{maxPlayers}";

        if (sessioninfo.PlayerCount >= maxPlayers) joinButton.interactable = false;
    }

    public void JoinSession()
    {
        PhotonManager._PhotonManager.JoinSession(sessionName.text);
    }

}
