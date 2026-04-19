using Fusion;
using Fusion.Addons.KCC;
using System;
using UnityEngine;
using UnityEngine.Serialization;


public class CameraController : NetworkBehaviour
{

    [Header("Camera Settings")]
    [SerializeField]
    private Transform player;

    [SerializeField] private float mouseSensitivity = 1;

    [FormerlySerializedAs("smooth")]
    [SerializeField]
    private float smoothnes;

    [SerializeField] private float maxAngleY = 80;
    [SerializeField] private float minAngleY = -80;

    private Vector2 camVelociy;
    private Vector2 smoothVelocity;

    [Header("Blob Movement")]
    [SerializeField] private float walkingSpeed = 1f;

    [SerializeField, Range(0, 0.1f)] private float walkingAmplitude = 0.015f; // Que tanto se mueve hacia los lados al caminar
    [SerializeField, Range(0, 0.1f)] private float runningAmplitude = 0.015f; // Que tanto se mueve hacia los lados al correr
    [SerializeField, Range(0, 15)] private float walkingFrequency = 10.0f; // La frecuencia con la que se mueve al caminar
    [SerializeField, Range(10, 20)] private float runningFrequency = 18f; // La frecuencia con la que se mueve al correr
    [SerializeField] private float resetPosSpeed = 3.0f; // Cuando dejas de moverte que regrese al centro
   // [SerializeField] private float toggleSpeed = 3.0f; // 

    private Vector3 startPos; // Posicion inicial de la cabeza , el centro
    [SerializeField] private bool moveHead;
    private Vector2 head;
    private InputManager inputManager;

    [Networked] private float CameraY { get; set; } // Rotación vertical 
    [Networked] private float CameraX { get; set; } // Rotación horizontal
    [Networked] private Vector2 CamVel { get; set; } // Velocidad acumulada de la cámara

    private KCC kcc;

    private void Awake()
    {
        startPos = transform.localPosition;
        kcc = player.GetComponent<KCC>();

        if (player == null)
        {
            player = FindFirstObjectByType<MovementController>().transform;
        }

        Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = false;
    }

    private void RotateCamera(NetworkInfoData input)
    {
        Vector2 velocity = Vector2.Scale(input.look, Vector2.one * mouseSensitivity);
        smoothVelocity = Vector2.Lerp(smoothVelocity, velocity, 1 / smoothnes);

        Vector2 camVelocity = CamVel; // Obtengo la velocidad actual de la camara
        camVelocity += smoothVelocity; // Le sumo la nueva velocidad suavizada
        camVelocity.y = Mathf.Clamp(camVelocity.y, minAngleY, maxAngleY); // Limito la rotacion vertical
        CamVel = camVelocity;

        CameraY = CamVel.y;
        CameraX = CamVel.x;

        transform.localRotation = Quaternion.AngleAxis(-CamVel.y, Vector3.right);
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInfoData input))
        {
            RotateCamera(input);
        }

        Quaternion lookRotation = Quaternion.AngleAxis(CameraX, Vector3.up);
        kcc.SetLookRotation(lookRotation);
        player.rotation = lookRotation;
    }
    public override void Spawned() // Al momento de hacer spawn 
    {
        Transform body = player.Find("Body");

        if (!HasInputAuthority)
        {
            // Client Player
            GetComponent<Camera>().enabled = false;
            GetComponent<AudioListener>().enabled = false;

            if (body != null)
            {
                int remoteLayer = LayerMask.NameToLayer("RemotePlayer");
                SetLayers(body.gameObject, remoteLayer);
            }
        }

        else
        {
            // Host Player
            Camera hostCam = GetComponent<Camera>();

            if (body != null)
            {
                int hostLayer = LayerMask.NameToLayer("LocalPlayer");
                SetLayers(body.gameObject, hostLayer);

                int hideLayer = ~(1 << hostLayer);
                hostCam.cullingMask = hideLayer;
            }
        }
    }

    private void SetLayers(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayers(child.gameObject, layer);
        }
    }

    public override void Render()
    {
        transform.localRotation = Quaternion.AngleAxis(-CameraY, Vector3.right);
    }

    #region Blob Movement
    private void BlobMove()
    {
        if (!inputManager.IsMoveInputPressed()) // Si no presiono ningun input
        {
            return; // termina el metodo
        }

        if (inputManager.IsMoveInputPressed()) // Pregunto si me estoy moviendo
        {
            if (inputManager.IsMovingBackwards() || inputManager.IsMovingOnXAxis()) // Me estoy moviendo hacia atras o hacia los lados?
            {
                transform.localPosition += FootStepMotion();
            }
            else //  Entonces me muevo hacia adelante
            {
                if (inputManager.WasRunInputPressed()) // Estoy corriendo?
                {
                    transform.localPosition += RunningFootStepMotion();
                }
                else // Estoy caminando
                {
                    transform.localPosition += FootStepMotion();
                }
            }
        }

        if (inputManager.IsMoveInputPressed())
        {
            transform.localPosition += inputManager.IsMovingBackwards() || inputManager.IsMovingOnXAxis() ? FootStepMotion() : inputManager.WasRunInputPressed() ? RunningFootStepMotion() : FootStepMotion();
        }



    }

    private void ResetPosition()
    {
        if (transform.localPosition == startPos) return; // Si la camara ya esta en la pos inicial, no hace nada
        transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, resetPosSpeed * Time.deltaTime);
    }

    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y = Mathf.Sin(Time.time * walkingFrequency) * walkingAmplitude * walkingSpeed;
        pos.x = Mathf.Cos(Time.time * walkingFrequency / 2) * walkingAmplitude * 2 * walkingSpeed;
        return pos;
    }


    private Vector3 RunningFootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y = Mathf.Sin(Time.time * runningFrequency) * runningAmplitude * walkingSpeed;
        pos.x = Mathf.Cos(Time.time * runningFrequency / 2) * runningAmplitude * 2 * walkingSpeed;
        return pos;
    }

    #endregion Blob movement

}