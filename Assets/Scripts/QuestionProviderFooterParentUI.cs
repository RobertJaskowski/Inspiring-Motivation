using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionProviderFooterParentUI : MonoBehaviour
{

    public int movingSpaceToOpen;
    public int movingSpaceToClose;


    public void OpenPanel()
    {
        transform.DOLocalMove(new Vector3(0, movingSpaceToOpen, 0),1f);

    }


    public void ClosePanel()
    {
        transform.DOLocalMove(new Vector3(0, movingSpaceToClose, 0), 1f);
    }
}
