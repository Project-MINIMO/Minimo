using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class Prolog_6 : PrologBase
{
    [SerializeField] private Image _background;
    
    protected override IEnumerator ShowProlog()
    {
        FadeIn(_background, 1f);
        yield return new WaitForSeconds(3f);
        
        EndProlog();
    }
}
