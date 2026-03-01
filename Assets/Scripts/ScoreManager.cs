using Fusion;
using TMPro;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI puntajeJ1;
    [SerializeField] private TextMeshProUGUI puntajeJ2;

    [Networked] private int PuntajeJugador1 { get; set; } 
    [Networked] private int PuntajeJugador2 { get; set; }

    [Header("Panel de Puntuacion Final")]
    [SerializeField] private GameObject fondoPuntuacionFinal;
    [SerializeField] private TextMeshProUGUI textFinalJ1;
    [SerializeField] private TextMeshProUGUI textFinalJ2;

    [Header("Configuracion de la Partida")]
    [SerializeField] private int marcadorParaGanar;
    [SerializeField] private string mensajeGanadorJ1;
    [SerializeField] private string mensajeGanadorJ2;
    [SerializeField] private string mensajeDerrotaJ1;
    [SerializeField] private string mensajeDerrotaJ2;

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        if (PuntajeJugador1 >= marcadorParaGanar || PuntajeJugador2 >= marcadorParaGanar)
        {
            RPC_MostrarPanel(PuntajeJugador1 >= marcadorParaGanar ? 1 : 2);
        }
    }
    public override void Spawned()
    {
        ActualizarPantalla();
    }

    private void ActualizarPantalla()
    {
        puntajeJ1.text = PuntajeJugador1.ToString();
        puntajeJ2.text = PuntajeJugador2.ToString();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_PuntajeJugador1()
    {
        if (Object.HasStateAuthority)
        {
            PuntajeJugador1++;
        }
        ActualizarPantalla();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_PuntajeJugador2()
    {
        if (Object.HasStateAuthority)
        {
            PuntajeJugador2++;
        }
        ActualizarPantalla();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_MostrarPanel(int ganador)
    {
        fondoPuntuacionFinal.SetActive(true);

        if (ganador == 1)
        {
            textFinalJ1.text = mensajeGanadorJ1;
            textFinalJ2.text = mensajeDerrotaJ2;
        }
        else
        {
            textFinalJ2.text = mensajeGanadorJ2;
            textFinalJ1.text = mensajeDerrotaJ1;
        }
    }


}