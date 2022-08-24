using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Header : MonoBehaviour
{

    public TMPro.TextMeshProUGUI headerText;


    private void OnEnable()
    {

    }


    public void SetText(string text)
    {
        Debug.Log("setting text to " + text);
        headerText.text = text;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
