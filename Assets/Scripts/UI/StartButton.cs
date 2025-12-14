using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour, IButton
{

    public Scene toLoad;

    public void OnMouseDown()
    {
        PerformAction();
    }

    public void PerformAction()
    {
        SceneManager.LoadScene(toLoad.name);
    }
}
