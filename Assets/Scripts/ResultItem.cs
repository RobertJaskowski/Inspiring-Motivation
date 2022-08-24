using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultItem : MonoBehaviour
{

    public TextMeshProUGUI resultText;
    public Image imageBG;

    [Space(20)]
    public Color answerColor;
    public Color questionColor;


    public string id;


    public void Trigger()
    {
        SendToFirebase.instance.OnAnswerChosen(id, resultText.text);
    }

    public void TriggerAnswerButton()
    {
        SendToFirebase.instance.OnAnswerChosen(id,resultText.text);
    }

    public void TriggerQuestionButton()
    {
        SendToFirebase.instance.OnQuestionChosen(id);
    }

    public void SetId(string id)
    {
        this.id = id;
    }

    public void SetTextOfResult(string text)
    {
        resultText.text = text;
    }

    public void SetColor(ResultItemType typeOfResult)
    {
        if(typeOfResult == ResultItemType.Answer)
        {
            imageBG.color = answerColor;
        }
        else
        {
            imageBG.color = questionColor;
        }
    }



}

public enum ResultItemType
{
    Answer,Question
}
