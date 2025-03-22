using UnityEngine;

public class SpinnerSoundTester : MonoBehaviour
{
    void Update()
    {
        // Press T to test the spinner sound
        if (Input.GetKeyDown(KeyCode.T) && SFXManager.Instance != null)
        {
            Debug.Log("Testing Spinner sound");
            //SFXManager.Instance.TestSpinnerSound();
        }
    }
}