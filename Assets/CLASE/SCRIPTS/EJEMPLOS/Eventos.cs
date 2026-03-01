using System;
using UnityEngine;
using UnityEngine.Events;

public class Eventos : MonoBehaviour
{

    [SerializeField] UnityEvent eventoDeEjemplo;

    public GameObject pared;

    public event Action eventos;
    PhotonManager photonManager;

    private void OnEnable()
    {
        eventos += Evento1;
        eventos += Evento2;
        eventos += Evento3;
    }

    private void OnDisable()
    {
        eventos -= Evento1;
        eventos -= Evento2;
        eventos -= Evento3;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvocarEvento();

    }

    [ContextMenu("Invocar Eventos")]
    public void InvocarEvento()
    {
        eventos.Invoke();
    }

    public void Evento1()
    {
        Debug.Log("Evento1");
    }

    public void Evento2()
    {
        Debug.Log("Evento2");
        eventos -= Evento2;
    }

    public void Evento3()
    {
        Debug.Log("Evento3");
    }



}
