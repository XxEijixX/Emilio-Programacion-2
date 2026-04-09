using Fusion;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    // Scores for each player, synchronized across the network
    [Networked] private int ScorePlayerOne { get; set; }
    [Networked] private int ScorePlayerTwo { get; set; }
    [Networked] private int ScorePlayerThree { get; set; }
    [Networked] private int ScorePlayerFour { get; set; }

    // Player references to identify which player is which, synchronized across the network
    [Networked] private PlayerRef PlayerOne { get; set; }
    [Networked] private PlayerRef PlayerTwo { get; set; }
    [Networked] private PlayerRef PlayerThree { get; set; }
    [Networked] private PlayerRef PlayerFour { get; set; }

    [Header("Scores Text")]
    [SerializeField] private TextMeshProUGUI scorePlayerOneText;
    [SerializeField] private TextMeshProUGUI scorePlayerTwoText;
    [SerializeField] private TextMeshProUGUI scorePlayerThreeText;
    [SerializeField] private TextMeshProUGUI scorePlayerFourText;

    [Header("Final Message")]
    [SerializeField] private TextMeshProUGUI finalMessageText;
    [SerializeField] private GameObject finalMessagePanel;

    public override void Spawned()
    {
        UpdateScoreDisplay(0, 0, 0, 0);  // Solo inicializar display, no asignar jugadores todavía
    }

    public void RegistrarJugador(PlayerRef player, int index)  // Llamado desde PhotonManager cada vez que entra un jugador
    {
        if (!Object.HasStateAuthority) return;

        Debug.Log($"[GameManager] Registrando jugador {player} en slot {index}");

        switch (index)
        {
            case 0: PlayerOne = player; break;
            case 1: PlayerTwo = player; break;
            case 2: PlayerThree = player; break;
            case 3: PlayerFour = player; break;
        }

        Debug.Log($"[GameManager] PlayerOne={PlayerOne} PlayerTwo={PlayerTwo} PlayerThree={PlayerThree} PlayerFour={PlayerFour}");
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_AddScore(PlayerRef scorer) // Llamado desde cualquier cliente cuando un jugador anota, el servidor actualiza el score y luego notifica a todos los clientes para actualizar el display
    {
        Debug.Log($"[GameManager] Rpc_AddScore recibido. scorer={scorer}");
        Debug.Log($"[GameManager] PlayerOne={PlayerOne} PlayerTwo={PlayerTwo} PlayerThree={PlayerThree} PlayerFour={PlayerFour}");

        if (scorer == PlayerOne) { ScorePlayerOne++; Debug.Log($"Score P1: {ScorePlayerOne}"); }
        else if (scorer == PlayerTwo) { ScorePlayerTwo++; Debug.Log($"Score P2: {ScorePlayerTwo}"); }
        else if (scorer == PlayerThree) { ScorePlayerThree++; Debug.Log($"Score P3: {ScorePlayerThree}"); }
        else if (scorer == PlayerFour) { ScorePlayerFour++; Debug.Log($"Score P4: {ScorePlayerFour}"); }
        else Debug.LogWarning($"[GameManager] scorer {scorer} no coincide con ningún jugador registrado");

        Rpc_UpdateDisplay(ScorePlayerOne, ScorePlayerTwo, ScorePlayerThree, ScorePlayerFour);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_UpdateDisplay(int p1, int p2, int p3, int p4) // Llamado por el servidor para actualizar el display de scores en todos los clientes
    {
        UpdateScoreDisplay(p1, p2, p3, p4);
    }

    private void UpdateScoreDisplay(int p1, int p2, int p3, int p4)
    {
        if (scorePlayerOneText != null) scorePlayerOneText.text = p1.ToString();
        if (scorePlayerTwoText != null) scorePlayerTwoText.text = p2.ToString();
        if (scorePlayerThreeText != null) scorePlayerThreeText.text = p3.ToString();
        if (scorePlayerFourText != null) scorePlayerFourText.text = p4.ToString();
    }
}