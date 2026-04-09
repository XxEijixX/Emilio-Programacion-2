
using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(GroundCheck))]
public class MovementController : NetworkBehaviour
{
    private Rigidbody rbPlayer;
    private KCC kcc;

    [SerializeField] private Animator _animator;


    private void Awake()
    {
        rbPlayer = GetComponent<Rigidbody>();
        kcc = GetComponent<KCC>();
    }

    public override void FixedUpdateNetwork()
    {

        if (GetInput(out NetworkInfoData input)) 
        {
            Movement(input);
            UpdateAnimations(input);

        }

    }

    private void UpdateAnimations(NetworkInfoData input)
    {
        _animator.SetBool("IsWalking", input.move.magnitude > 0.01f);
        _animator.SetBool("IsRunning", input.buttons.IsSet(NetworkInfoData.BotonCorrer));
        _animator.SetFloat("WalkingZ", input.move.y);
        _animator.SetFloat("WalkingX", input.move.x);
    }


    #region Movimiento

    [SerializeField] private float walkSpeed = 5.5f;
    [SerializeField] private float runSpeed = 7.7f;
   // [SerializeField] private float crouchSpeed = 3.9f;

    private void Movement(NetworkInfoData input)
    {
        kcc.SetKinematicVelocity(transform.localRotation * new Vector3(input.move.x, 0, input.move.y) * (Runner.DeltaTime * Speed(input)));
    }

    private float Speed(NetworkInfoData input)
    {                                                              // .IsSet es para revisar si un boton esta presionado
        return input.move.y < 0 || input.move.x != 0 ? walkSpeed : input.buttons.IsSet(NetworkInfoData.BotonCorrer) ? runSpeed : walkSpeed;
    }


    #endregion


}