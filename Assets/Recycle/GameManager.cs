using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TMP_Text cortisolText;
    public float cortisolLevel = 0f;
    public float countSpeed = 10f;
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        CortisolLevel();
    }

    public void CortisolLevel()
    {
        if (cortisolLevel < 100f)
        {
            cortisolLevel += countSpeed * Time.deltaTime;
            if (cortisolLevel > 100f)
            cortisolLevel = 100f;
        }

        cortisolText.text = "Cortisol: " + cortisolLevel.ToString("0.00");
    }
}
