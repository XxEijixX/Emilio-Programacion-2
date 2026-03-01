using Fusion;
using UnityEngine;
// Este struct es la iformacion que se envia a los demas clientes
public struct NetworkInfoData : INetworkInput // Implementa la interfaz INetworkInput para enviar datos de entrada a otros clientes
{
    public NetworkButtons buttons; 

    public Vector3 move; // Movimiento del jugador
    public Vector2 rotation; // Rotación del jugador

    public const byte BotonDisparo = 0; // Botón de disparo
    public const byte BotonCorrer = 1; // Botón de correr
    
}
