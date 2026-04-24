using Fusion;
using UnityEngine;

public class PlayerNameSender : NetworkBehaviour
{
    private GameManager _gameManager;

    public override void Spawned()
    {
        _gameManager = FindFirstObjectByType<GameManager>();

        // Cuando el jugador spawea, si es local envía su nombre inmediatamente
        if (Object.HasInputAuthority)
        {
            Debug.Log($"[PlayerNameSender] UsernameActual al spawnear: '{PlayFabManager.Instance?.UsernameActual}'");
            string miNombre = PlayFabManager.Instance != null && !string.IsNullOrEmpty(PlayFabManager.Instance.UsernameActual)
                ? PlayFabManager.Instance.UsernameActual
                : $"Player {Runner.LocalPlayer.PlayerId}";

            Debug.Log($"[PlayerNameSender] Enviando nombre: {miNombre}");
            Rpc_EnviarNombre(miNombre);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void Rpc_EnviarNombre(string nombre)
    {
        Debug.Log($"[PlayerNameSender] Host recibió nombre: {nombre} de player:{Object.InputAuthority}");

        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
            gameManager.Rpc_EnviarNombrePropio(Object.InputAuthority, nombre);
    }
}