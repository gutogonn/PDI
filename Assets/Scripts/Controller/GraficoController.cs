using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GraficoController : MonoBehaviour
{
    public Image graph1Sprite;
    public Image graph2Sprite;
    public Image graph3Sprite;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void create(int[] histo, Sprite sprite, Image grafSprite)
    {
        Texture2D newImage = new Texture2D(sprite.texture.width, sprite.texture.height);
        Texture2D itemBGTex = sprite.texture;

        for (int i = 0; i < histo.Length; i++)
        {
            for (int j = 0; j < histo[i]; j++)
            {
                Color newColor = new Color(1, 1, 1, 1);
                newImage.SetPixel(i, j, newColor);
            }
        }

        newImage.Apply();
        byte[] itemBGBytes = newImage.EncodeToPNG();
        grafSprite.sprite = Sprite.Create(newImage, new Rect(0.0f, 0.0f, newImage.width, newImage.height), new Vector2(0.5f, 0.5f), 100.0f);
        File.WriteAllBytes("Assets/Resources/Images/" + sprite.name + " - grafico.png", itemBGBytes);
    }
}
