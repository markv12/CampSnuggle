using UnityEngine;

public class RenderTextureResizer : MonoBehaviour {

    [SerializeField] RenderTexture theTexture;

	void Start () {
        float theRatio = ((float)theTexture.height) / ((float)Screen.height);
        theTexture.width = Mathf.RoundToInt(Screen.width * theRatio);
	}
}
