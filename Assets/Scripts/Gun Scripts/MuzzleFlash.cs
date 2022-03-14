using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public GameObject flashHolder;

    public Sprite[] flashSprites;
    public SpriteRenderer[] spriteRenderer;
    public float flashTime = 0.05f;

    private void Start()
    {
        Deactivate();
    }

    public void Activate()
    {
        flashHolder.SetActive(true);

        int flashSpriteIndex = Random.Range(0, flashSprites.Length);

        for (int i = 0; i < spriteRenderer.Length; i++)
        {
            spriteRenderer[i].sprite = flashSprites[flashSpriteIndex];
        }

        Invoke("Deactivate", flashTime);
    }

    void Deactivate()
    {
        flashHolder.SetActive(false);
    }


} // class
