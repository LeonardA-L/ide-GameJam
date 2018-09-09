using UnityEngine;

public class CubeController : MonoBehaviour {

    // Will be called when the user clicks on the cube
    void OnMouseDown ()
    {
        Debug.Log("Clicked on cube, playing sound");

        // Call AudioManager
        AudioManager.Instance.PlaySound("soundName");

        AudioManager.Instance.StopAllSounds();
    }

}
