using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{

    static UIManager current;

    public TextMeshProUGUI debugText;

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
        Debug.Log("this is set");
        DontDestroyOnLoad(gameObject);
    }

    public static void UpdateDebugText(string text)
    {
        if (current == null)
        {
            return;
        }

        if (current.debugText == null)
        {
            Debug.Log("debug text is null");
            return;
        }
        current.debugText.text = text;
    }
}
