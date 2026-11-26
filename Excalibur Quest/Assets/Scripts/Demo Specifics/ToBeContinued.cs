using UnityEngine;
using UnityEngine.SceneManagement;

public class ToBeContinued : MonoBehaviour
{
    [SerializeField] private GameObject toBeContinuedUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ShowToBeContinuedUI());
        }
    }

    private System.Collections.IEnumerator ShowToBeContinuedUI()
    {
        PauseManager.Instance.DisablePause();
        toBeContinuedUI.SetActive(true);
        toBeContinuedUI.GetComponent<Animator>().SetTrigger("PlayAnimation");
        yield return new WaitForSeconds(11f);
        SceneManager.LoadScene("StartScreen");
    }
}
