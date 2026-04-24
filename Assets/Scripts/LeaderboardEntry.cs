using TMPro;
using UnityEngine;

public class LeaderboardEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI posicionTxt;
    [SerializeField] private TextMeshProUGUI nombreTxt;
    [SerializeField] private TextMeshProUGUI victoriasTxt;
    [SerializeField] private TextMeshProUGUI derrotasTxt;
    [SerializeField] private TextMeshProUGUI puntuacionTxt;

    public void SetData(int posicion, string nombre, int victorias, int derrotas, int puntuacion)
    {
        posicionTxt.text = $"#{posicion}";
        nombreTxt.text = nombre;
        victoriasTxt.text = victorias == 1 ? "1 victoria" : $"{victorias} victorias";
        derrotasTxt.text = derrotas == 1 ? "1 derrota" : $"{derrotas} derrotas";
        puntuacionTxt.text = $"{puntuacion} pts";
    }
}