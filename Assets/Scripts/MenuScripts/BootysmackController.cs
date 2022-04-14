//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class bootysmackcontroller : monobehaviour
//{
//    // start is called before the first frame update
//    void start()
//    {
//        //start the coroutine we define below named examplecoroutine.
//        startcoroutine(gotostartmenu());
//    }

//    ienumerator gotostartmenu()
//    {
//        soundmanager.instance.playbooty();
//        yield return new waitforseconds(4);
//        scenemanager.loadscene(scenename: "startmenu");
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BootysmackController : MonoBehaviour
{
	public GameObject soundManager;

	#region FIELDS
	public Image fadeOutUIImage;
	public float fadeSpeed = 0.8f;

	public enum FadeDirection
	{
		In, //Alpha = 1
		Out // Alpha = 0
	}
	#endregion

	#region MONOBHEAVIOR
	void OnEnable()
	{
		StartCoroutine(Fade(FadeDirection.Out));
	}
	#endregion

	#region FADE
	private IEnumerator Fade(FadeDirection fadeDirection)
	{
		SoundManager.instance.PlayBooty();
		float alpha = (fadeDirection == FadeDirection.Out) ? 1 : 0;
		float fadeEndValue = (fadeDirection == FadeDirection.Out) ? 0 : 1;
		if (fadeDirection == FadeDirection.Out)
		{
			while (alpha >= fadeEndValue)
			{
				SetColorImage(ref alpha, fadeDirection);
				yield return null;
			}
			fadeOutUIImage.enabled = false;
		}
		else
		{
			fadeOutUIImage.enabled = true;
			while (alpha <= fadeEndValue)
			{
				SetColorImage(ref alpha, fadeDirection);
				yield return null;
			}
		}
		SceneManager.LoadScene(sceneName: "StartMenu");
	}
	#endregion

	#region HELPERS
	public IEnumerator FadeAndLoadScene(FadeDirection fadeDirection, string sceneToLoad)
	{
		yield return Fade(fadeDirection);
		SceneManager.LoadScene(sceneToLoad);
	}

	private void SetColorImage(ref float alpha, FadeDirection fadeDirection)
	{
		fadeOutUIImage.color = new Color(fadeOutUIImage.color.r, fadeOutUIImage.color.g, fadeOutUIImage.color.b, alpha);
		alpha += Time.deltaTime * (1.0f / fadeSpeed) * ((fadeDirection == FadeDirection.Out) ? -1 : 1);
	}
	#endregion
}