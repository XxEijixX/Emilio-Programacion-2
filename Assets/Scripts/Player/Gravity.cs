using UnityEngine;
using Fusion;
using Fusion.Addons.KCC;

public class Gravity : NetworkBehaviour
{
    [SerializeField] private float gravity = 15f;
    private KCC kcc;

    [Networked] private float verticalVelocity { get; set; }

    void Awake()
    {
        kcc = GetComponent<KCC>();
    }

    public override void FixedUpdateNetwork()
    {
        if (kcc == null) return;

        // Si está en el suelo, resetea la velocidad vertical
        if (kcc.Data.IsGrounded)
        {
            verticalVelocity = 0f;
        }
        else
        {
            // Acumula velocidad de caída
            verticalVelocity += gravity * Runner.DeltaTime;
        }

        // Aplica movimiento vertical usando AddExternalDelta
        kcc.AddExternalDelta(Vector3.down * verticalVelocity * Runner.DeltaTime);
    }
}