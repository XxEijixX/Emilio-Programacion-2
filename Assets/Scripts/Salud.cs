using Fusion;
using System.Linq;
using UnityEngine;

public class Salud : NetworkBehaviour
{
    [SerializeField] int maxHealth = 100;
    [Networked] public int _currentHealth { get; set; }

    private ScoreManager scoreManager;

    public override void Spawned()
    {
        _currentHealth = maxHealth;
        scoreManager = FindFirstObjectByType<ScoreManager>();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_TakeDamage(int damage, PlayerRef victima)
    {
        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Muerte(victima);
        }
    }

    private void Muerte(PlayerRef asesino)
    {
        if (gameObject.CompareTag("Enemigo"))
        {
            if (scoreManager == null)
            {
                scoreManager = FindFirstObjectByType<ScoreManager>();
            }

            else if (asesino != PlayerRef.None)
            {
                PlayerRef[] players = Runner.ActivePlayers.ToArray();  // Obtener lista de jugadores ordenada

                if (players.Length >= 1)
                {
                    if (asesino == players[0])
                    {
                        scoreManager.Rpc_PuntajeJugador1();
                    }
                    else if (players.Length >= 2 && asesino == players[1])
                    {
                        scoreManager.Rpc_PuntajeJugador2();
                    }
                }
            }
        }

        if (Object != null && Object.HasStateAuthority)
        {
            Runner.Despawn(Object);
        }
    }
}
