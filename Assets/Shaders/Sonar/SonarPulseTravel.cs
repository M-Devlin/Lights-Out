using UnityEngine;
using UnityEditor;
using System.Collections;

public class SonarPulseTravel : MonoBehaviour {

    Vector3 minScale;
    public Vector3 maxScale;
    public float speed = 2f;
    public float duration = 5f;

    public float fadeSpeed = 0.5f;

	IEnumerator Start () {
        minScale = transform.localScale;
        yield return Pulse(minScale, maxScale, duration);

        //StartCoroutine(FadeOut(0.0f, 1.0f));
        //Get material from component

    }

    // Update is called once per frame
    public IEnumerator Pulse(Vector3 originalSize, Vector3 maxSize, float time) //people who use one-letter variables should be sterilized.
    {
        float sphereScale = 0.0f;

        float rate = (1.0f / time) * speed;
        while (sphereScale < 1.0f) 
        {
            sphereScale += Time.deltaTime * rate;

            transform.localScale = Vector3.Lerp(originalSize, maxSize, sphereScale);
            yield return null; //Dunno why I'm returning a Null value but if I have to slaughter a goat to get this fucker to work, so be it.

        }

        //fade material alpha
        var material = GetComponent<Renderer>().material;
        var color = material.color;

        material.color = new Color(color.r, color.g, color.b, color.a - (fadeSpeed * Time.deltaTime));

        if (sphereScale >= 1f) //Destroy sphere when it reaches max size
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {

        
    }

    //IEnumerator FadeOut(float value, float time)
    //{
    //    float alpha = GetComponent<Renderer>().material.color.a;
    //    for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time)
    //    {
    //        Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, value, t));
    //        GetComponent<Renderer>().material.color = newColor;
    //        yield return null; //sacrifice another goat
    //    }
    //}

}
