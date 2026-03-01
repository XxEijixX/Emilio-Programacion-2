using Fusion;
using System.Threading.Tasks;
using UnityEngine;


public class Bullet : NetworkBehaviour
{

    [SerializeField] private float speed = 100f;
    [SerializeField] private float lifetime = 1f;
    [SerializeField] public int dañoDeBala;
    private Rigidbody rb;

    [Networked] public PlayerRef MiBala { get; set; }

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody>();
        if (Object.HasStateAuthority) rb.linearVelocity = speed * transform.forward;
        DespawnAfterTime();
    }

    private async void DespawnAfterTime()
    {
        await Task.Delay((int)(lifetime * 1000));
        if (Object != null && Object.HasStateAuthority) Runner.Despawn(Object);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!ColisionValida()) return;

        if (collision.gameObject.CompareTag("Enemigo"))
        {
            if (collision.gameObject.TryGetComponent<Salud>(out Salud salud))
            {
                RpcDañoEnemigo(MiBala, salud.Object, dañoDeBala);
                Runner.Despawn(Object);
            }

            else if (collision.gameObject.CompareTag("Pared"))
            {
                Runner.Despawn(Object);
            }
        }
    }

    private bool ColisionValida()
    {
        return Object != null && Object.HasStateAuthority;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    private void RpcDañoEnemigo(PlayerRef jugador, NetworkObject enemigo, int daño)
    {
        if (enemigo != null && enemigo.TryGetComponent<Salud>(out Salud salud))
        {
            salud.Rpc_TakeDamage(daño, jugador);
        }
    }

}