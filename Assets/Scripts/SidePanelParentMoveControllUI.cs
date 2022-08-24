using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SidePanelParentMoveControllUI : MonoBehaviour
{

    public GameObject panelToAnimate;
    public int movementOpened;
    public int movementClosed;


    public void OpenPanel()
    {
        panelToAnimate.transform.DOLocalMove(new Vector3(movementOpened, panelToAnimate.transform.localPosition.y, 0), 1f);
        //panelToAnimate.transform.DOLocalMoveX(movementOpened, 1f);
    }

    public void ClosePanel()
    {
        panelToAnimate.transform.DOLocalMove(new Vector3(movementClosed, panelToAnimate.transform.localPosition.y, 0), 1f);

        //panelToAnimate.transform.DOLocalMoveX(movementOpened, 1f);

    }

}
