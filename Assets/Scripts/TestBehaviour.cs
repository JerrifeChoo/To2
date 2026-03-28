using TMPro;
using UnityEngine;

public class TestBehaviour : MonoBehaviour
{
    public TextMeshProUGUI text;

    private void Start()
    {
        if (text != null)
        {
            var time = System.DateTime.Now;
            text.text = $"{time.Hour:D2}:{time.Minute:D2}:{time.Second:D2}:{time.Ticks % 1000:D3}";
        }
    }
}
