using TMPro;
using UnityEngine;

public class LeaderboardEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI posicionTxt;
    [SerializeField] private TextMeshProUGUI nombreTxt;
    [SerializeField] private TextMeshProUGUI victoriasTxt;

    public void SetData(int posicion, string nombre, int victorias)
    {
        posicionTxt.text = $"#{posicion}";
        nombreTxt.text = nombre;
        victoriasTxt.text = victorias == 1 ? "1 victoria" : $"{victorias} victorias";
    }
}