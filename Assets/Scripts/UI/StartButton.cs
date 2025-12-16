using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    [SerializeField]
    private string toLoad;

    private void OnMouseDown()
    {
        PerformAction();
    }

    public void PerformAction()
    {
        SceneManager.LoadScene(toLoad);
    }
}
