using UnityEngine;

public class TimeTestDebugHelper : MonoBehaviour
{
    float newTime = 4.0f;
    public KeyCode key;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            if (Time.timeScale == 1)
                Time.timeScale = newTime;
            else
                Time.timeScale = 1;
        }
    }
}
