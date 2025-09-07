using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataEraser : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        var firebaseManager = App.GetManager<FirebaseManager>();
        await firebaseManager.DeleteUserAsync();
        await firebaseManager.InitializeFirebase();
        
        // find 'AccountInfo' and forget the instance
        AccountInfo.Instance.ForgetInstance();
        
        // find 'App' and destroy it
        App.Instance.ForceDestroy();
        
        // find 'TutorialManager' and destroy it
        TutorialManager.Instance.ForceDestroy();

        SceneManager.LoadScene("00. Developer");
    }
}
