using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class UIManager : MonoBehaviour
{

    static UIManager current;

    private TextMeshProUGUI[] debugTexts;
    private TextMeshProUGUI healthText;

    public int test = 0;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(gameObject);
            return;
        }

        current = this;
        DontDestroyOnLoad(gameObject);

        current.debugTexts = current.GetComponentsInChildren<TextMeshProUGUI>().Where(x => x.CompareTag("Debug")).ToArray();
        foreach(var comp in current.debugTexts)
        {
            comp.text = "test";
        }

        healthText = current.GetComponentsInChildren<TextMeshProUGUI>().Where(x => x.name == "Health").First();

    }


    public static void UpdateDebugText(string text, int nr)
    {
        if (nr > current.debugTexts.Length)
        {
            Debug.Log("debug text is null");
            return;
        }

        current.debugTexts[nr].text = text;
    }

    public static void UpdateHealthText(int health)
    {
        current.healthText.text = "HP = " + health.ToString();
    }

}
