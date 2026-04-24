using PlayFab.ClientModels;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform viewportContent;
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private GameObject leaderboardPanel;

    public void MostrarLeaderboard()
    {
        leaderboardPanel.SetActive(true);
        CargarLeaderboard();
    }

    private void CargarLeaderboard()
    {
        // Pedimos las 3 estadísticas en paralelo y esperamos a tener las 3
        List<PlayerLeaderboardEntry> wins = null;
        List<PlayerLeaderboardEntry> losses = null;
        List<PlayerLeaderboardEntry> scores = null;

        PlayFabManager.Instance.ObtenerLeaderboardPor(result =>
        {
            wins = result;
            IntentarMostrar();
        }, "Wins");

        PlayFabManager.Instance.ObtenerLeaderboardPor(result =>
        {
            losses = result;
            IntentarMostrar();
        }, "Losses");

        PlayFabManager.Instance.ObtenerLeaderboardPor(result =>
        {
            scores = result;
            IntentarMostrar();
        }, "TotalScore");

        void IntentarMostrar()
        {
            // Solo construimos la UI cuando las 3 listas han llegado
            if (wins == null || losses == null || scores == null) return;

            foreach (Transform child in viewportContent)
                Destroy(child.gameObject);

            // Usamos Wins como lista base para el orden y la posición
            foreach (PlayerLeaderboardEntry entry in wins)
            {
                string id = entry.PlayFabId;

                int victorias = entry.StatValue;
                int derrotas = losses.FirstOrDefault(e => e.PlayFabId == id)?.StatValue ?? 0;
                int puntuacion = scores.FirstOrDefault(e => e.PlayFabId == id)?.StatValue ?? 0;

                string nombre = entry.DisplayName ?? entry.PlayFabId;

                GameObject fila = Instantiate(entryPrefab, viewportContent);
                LeaderboardEntry le = fila.GetComponent<LeaderboardEntry>();
                le.SetData(entry.Position + 1, nombre, victorias, derrotas, puntuacion);
            }
        }
    }

    public void CerrarLeaderboard()
    {
        leaderboardPanel.SetActive(false);
    }
}