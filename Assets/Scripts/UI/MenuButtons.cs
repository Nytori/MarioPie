using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MenuButtons : MonoBehaviour
    {

        public GameObject controls;
        
        public void OnClickStart()
        {
            SceneManager.LoadScene("Arena");
        }

        public void OnClickControls()
        {
            controls.SetActive(!controls.activeSelf);
        }

        public void OnClickMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
        
        public void OnClickQuit()
        {
            Application.Quit();
        }
    }
}