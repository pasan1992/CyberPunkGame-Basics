using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingDialogTextGroup : MonoBehaviour
{
    public List<FloatingInfoText> AnswerTexts;
    public Transform Target;
    void Start()
    {
        updateTarget();
    }

    public void setTarget(Transform target)
    {
        this.Target = target;
    }

    private void updateTarget()
    { 
        if(Target != null)
        {
            Vector3 staringOffset = new Vector3(1,200,1);
            foreach (FloatingInfoText text in AnswerTexts)
            {
                text.setTarget(Target,staringOffset);
                staringOffset += Vector3.down*17;
                Debug.Log(staringOffset);
            }
        }
    }
}
