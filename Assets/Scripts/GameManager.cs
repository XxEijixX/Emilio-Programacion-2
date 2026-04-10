using Fusion;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [Networked] private int ScorePlayerOne { get; set; }
    [Networked] private int ScorePlayerTwo { get; set; }
    [Networked] private int ScorePlayerThree { get; set; }
    [Networked] private int ScorePlayerFour { get; set; }

    [Networked] private PlayerRef PlayerOne { get; set; }
    [Networked] private PlayerRef PlayerTwo { get; set; }
    [Networked] private PlayerRef PlayerThree { get; set; }
    [Networked] private PlayerRef PlayerFour { get; set; }

    [Header("Puntuación máxima")]
    [SerializeField] private int maxScore = 10;
    [SerializeField] private GameObject winObject; 

    [Header("Scores UI")]
    [SerializeField] private TextMeshProUGUI scorePlayerOneText;
    [SerializeField] private TextMeshProUGUI scorePlayerTwoText;
    [SerializeField] private TextMeshProUGUI scorePlayerThreeText;
    [SerializeField] private TextMeshProUGUI scorePlayerFourText;

    [Header("Nombres UI")]
    [SerializeField] private TextMeshProUGUI namePlayerOneText;
    [SerializeField] private TextMeshProUGUI namePlayerTwoText;
    [SerializeField] private TextMeshProUGUI namePlayerThreeText;
    [SerializeField] private TextMeshProUGUI namePlayerFourText;

    [Header("Final Message")]
    [SerializeField] private TextMeshProUGUI finalMessageText;
    [SerializeField] private GameObject finalMessagePanel;

    // Diccionario local para guardar nombres (no necesita ser [Networked], el host lo gestiona)
    private readonly string[] _playerNames = new string[4];

    public override void Spawned()
    {
        UpdateScoreDisplay(0, 0, 0, 0);
        if (winObject != null) winObject.SetActive(false);
    }

    public void RegistrarJugador(PlayerRef player, int index, string username)
    {
        if (!Object.HasStateAuthority) return;

        Debug.Log($"[GameManager] Registrando jugador {player} '{username}' en slot {index}");

        switch (index)
        {
            case 0: PlayerOne = player; break;
            case 1: PlayerTwo = player; break;
            case 2: PlayerThree = player; break;
            case 3: PlayerFour = player; break;
        }

        // Guardar nombre localmente en el host
        _playerNames[index] = username;

        // Sincronizar TODOS los nombres acumulados a todos los clientes
        Rpc_SincronizarNombres(
            _playerNames[0] ?? "",
            _playerNames[1] ?? "",
            _playerNames[2] ?? "",
            _playerNames[3] ?? ""
        );
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_SincronizarNombres(string n0, string n1, string n2, string n3)
    {
        if (!string.IsNullOrEmpty(n0) && namePlayerOneText != null) namePlayerOneText.text = n0;
        if (!string.IsNullOrEmpty(n1) && namePlayerTwoText != null) namePlayerTwoText.text = n1;
        if (!string.IsNullOrEmpty(n2) && namePlayerThreeText != null) namePlayerThreeText.text = n2;
        if (!string.IsNullOrEmpty(n3) && namePlayerFourText != null) namePlayerFourText.text = n3;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_AddScore(PlayerRef scorer)
    {
        Debug.Log($"[GameManager] Rpc_AddScore scorer={scorer}");

        int winnerIndex = -1;

        if (scorer == PlayerOne) { ScorePlayerOne++; if (ScorePlayerOne >= maxScore) winnerIndex = 0; }
        else if (scorer == PlayerTwo) { ScorePlayerTwo++; if (ScorePlayerTwo >= maxScore) winnerIndex = 1; }
        else if (scorer == PlayerThree) { ScorePlayerThree++; if (ScorePlayerThree >= maxScore) winnerIndex = 2; }
        else if (scorer == PlayerFour) { ScorePlayerFour++; if (ScorePlayerFour >= maxScore) winnerIndex = 3; }
        else
        {
            Debug.LogWarning($"[GameManager] scorer {scorer} no coincide con ningún jugador registrado");
            return;
        }

        Rpc_UpdateDisplay(ScorePlayerOne, ScorePlayerTwo, ScorePlayerThree, ScorePlayerFour);

        if (winnerIndex >= 0)
            Rpc_TriggerWin(winnerIndex);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_UpdateDisplay(int p1, int p2, int p3, int p4)
    {
        UpdateScoreDisplay(p1, p2, p3, p4);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_TriggerWin(int winnerIndex)
    {
        // Obtener nombre del ganador desde el texto ya asignado
        string winnerName = winnerIndex switch
        {
            0 => namePlayerOneText != null ? namePlayerOneText.text : "Player 1",
            1 => namePlayerTwoText != null ? namePlayerTwoText.text : "Player 2",
            2 => namePlayerThreeText != null ? namePlayerThreeText.text : "Player 3",
            3 => namePlayerFourText != null ? namePlayerFourText.text : "Player 4",
            _ => "Unknown"
        };

        if (finalMessageText != null)
            finalMessageText.text = $"{winnerName} wins!";

        if (finalMessagePanel != null) finalMessagePanel.SetActive(true);
        if (winObject != null) winObject.SetActive(true);
    }

    private void UpdateScoreDisplay(int p1, int p2, int p3, int p4)
    {
        if (scorePlayerOneText != null) scorePlayerOneText.text = p1.ToString();
        if (scorePlayerTwoText != null) scorePlayerTwoText.text = p2.ToString();
        if (scorePlayerThreeText != null) scorePlayerThreeText.text = p3.ToString();
        if (scorePlayerFourText != null) scorePlayerFourText.text = p4.ToString();
    }
}