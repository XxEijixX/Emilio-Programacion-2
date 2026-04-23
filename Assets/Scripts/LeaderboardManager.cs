using PlayFab.ClientModels;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform viewportContent;  // El Content del ScrollView
    [SerializeField] private GameObject entryPrefab;     // Prefab de cada fila
    [SerializeField] private GameObject leaderboardPanel;

    public void MostrarLeaderboard()
    {
        leaderboardPanel.SetActive(true);

        PlayFabManager.Instance.ObtenerLeaderboard(entries =>
        {
            // Limpiar entradas anteriores
            foreach (Transform child in viewportContent)
                Destroy(child.gameObject);

            // Crear una fila por jugador
            foreach (PlayerLeaderboardEntry entry in entries)
            {
                GameObject fila = Instantiate(entryPrefab, viewportContent);
                LeaderboardEntry leaderboardEntry = fila.GetComponent<LeaderboardEntry>();
                leaderboardEntry.SetData(entry.Position + 1, entry.DisplayName ?? entry.PlayFabId, entry.StatValue);
            }
        });
    }

    public void CerrarLeaderboard()
    {
        leaderboardPanel.SetActive(false);
    }
}