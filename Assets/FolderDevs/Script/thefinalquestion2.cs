using UnityEngine;

public class thefinalquestion2 : MonoBehaviour
{
    public CanvasGroup question;
    public FinalQuestion fquestion;
    public bool fadein = false;
    public bool fadeout = false;





    public void Showui()
    {
        
        fadein = true;
    }

    public void hideui()
    {
        fadeout = true;
    }



    private void Update()
    {

        if (fquestion.reachedFull == true)
        {
            Showui();
        }

        if (fadein)
        {
            if (question.alpha < 1)
            {
                question.alpha += Time.deltaTime;

                if (question.alpha >= 1)
                {
                    fadein = false;
                }
            }
        }

        if (fadeout)
        {
            if (question.alpha >= 0)
            {
                question.alpha -= Time.deltaTime;

                if (question.alpha == 0)
                {
                    fadeout = false;
                }
            }
        }


        
    }

}
