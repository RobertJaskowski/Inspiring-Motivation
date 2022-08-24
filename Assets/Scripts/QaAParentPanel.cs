using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class QaAParentPanel : MonoBehaviour
{

    public int QuestionPosition;
    public int AnswerPosition;
    public int OpenPosition;


    public void SetQuestionPosition()
    {
        transform.transform.DOLocalMove(new Vector3(QuestionPosition, transform.localPosition.y, 0), 1f);
    }

    public void SetAnswerPosition()
    {
        transform.transform.DOLocalMove(new Vector3(AnswerPosition, transform.localPosition.y, 0), 1f);
    }

    public void SetOpenPosition()
    {
        transform.transform.DOLocalMove(new Vector3(OpenPosition, transform.localPosition.y, 0), 1f);
    }


}
