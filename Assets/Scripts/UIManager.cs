using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{

    static UIManager current;

    public TextMeshProUGUI debugText;

    private TextMeshProUGUI[] debugTexts;

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

        current.debugTexts = current.GetComponentsInChildren<TextMeshProUGUI>();
        foreach(var comp in current.debugTexts)
        {
            comp.text = "";
        }
    }


    public static void UpdateDebugText(string text, int nr)
    {
        if (current == null)
        {
            return;
        }

        if (nr > current.debugTexts.Length)
        {
            Debug.Log("debug text is null");
            return;
        }

        current.debugTexts[nr].text = text;
    }
}
