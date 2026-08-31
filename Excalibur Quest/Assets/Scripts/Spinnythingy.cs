using UnityEngine;

public class Spinnythingy : MonoBehaviour
{
    void Update()
    {
        //spin the object on y axis for visual effect

        transform.Rotate(0, 50 * Time.deltaTime, 0);
    }
}
