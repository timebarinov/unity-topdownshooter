using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public Rigidbody myBody;
    public float forceMin = 91f, forceMax = 119f;

    public float lifetime = 3f;
    public float fadeTime = 2f;

    public Material shellMat;

    private void Start()
    {
        shellMat = GetComponent<Renderer>().material;

        float force = Random.Range(forceMin, forceMax);
        myBody.AddForce(transform.right * force);
        myBody.AddTorque(Random.insideUnitSphere * force);

        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifetime);

        float percent = 0f;
        float fadeSpeed = 1 / fadeTime;

        Color initialColor = shellMat.color;

        while (percent < 1)
        {
            percent += Time.deltaTime * fadeSpeed;
            shellMat.color = Color.Lerp(initialColor, Color.clear, percent);
            yield return null;
        }

        Destroy(gameObject);
    }
}
