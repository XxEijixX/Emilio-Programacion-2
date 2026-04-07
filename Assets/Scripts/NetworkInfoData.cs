using Fusion;
using UnityEngine;

public struct NetworkInfoData : INetworkInput
{
    public NetworkButtons buttons;
    public Vector2 move;
    public Vector2 look;
    public float yRotation;

    public const int BotonDisparo = 0;
    public const int BotonCorrer = 1;
}