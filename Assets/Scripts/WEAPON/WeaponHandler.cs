using Fusion;
using UnityEngine;

public class WeaponHandler : NetworkBehaviour
{
    [SerializeField] Weapon actualWeapon;

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority)
            return;

        if (GetInput(out NetworkInfoData input))
        {
            if (input.buttons.IsSet((int)NetworkInfoData.BotonDisparo))
            {
                actualWeapon.RigidBodyShoot();
            }
        }
    }
}