using UnityEngine.SceneManagement;

namespace SonicCTK.Core
{
    public static class SceneUtility
    {
        public static void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().path);
        }
    }
}