using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ImageController : MonoBehaviour
{
    public Color data;
    public SpriteRenderer image;
    private Sprite newSprite;
    public Text rText, gText, bText;
    public Button grayscaleButton;

    public Button linearButton;
    public Slider linear;

    public Button noiseButton;
    public Dropdown noiseOption;


    Color[] color;

    public void Start()
    {
        color = image.sprite.texture.GetPixels();
        grayscaleButton.onClick.AddListener(turnGray);
        linearButton.onClick.AddListener(turnLinear);
        noiseButton.onClick.AddListener(removeNoise);
    }

    public void Update()
    {
        PixelToMouse.Instance().GetSpritePixelColorUnderMousePointer(image, out data);
        rText.text = data.r.ToString();
        gText.text = data.g.ToString();
        bText.text = data.b.ToString();
    }

    public void turnGray()
    {
        newSprite = image.sprite;
        Texture2D newImage = new Texture2D(newSprite.texture.width, newSprite.texture.height);
        Texture2D itemBGTex = newSprite.texture;

        for (int y = 0; y < newImage.height; y++)
        {
            for (int x = 0; x < newImage.width; x++)
            {
                Color pixel = newSprite.texture.GetPixel(x, y);
                Color color = new Color((pixel.r + pixel.g + pixel.b) / 3, (pixel.r + pixel.g + pixel.b) / 3, (pixel.r + pixel.g + pixel.b) / 3, 1);
                newImage.SetPixel(x, y, color);
            }
        }
        saveImage(newImage, newSprite.name + " - GrayScale");
    }

    public void turnLinear()
    {
        newSprite = image.sprite;
        Texture2D newImage = new Texture2D(newSprite.texture.width, newSprite.texture.height);

        Texture2D itemBGTex = newSprite.texture;

        for (int y = 0; y < newImage.height; y++)
        {
            for (int x = 0; x < newImage.width; x++)
            {
                Color pixel = newSprite.texture.GetPixel(x, y);
                Color color;

                if(pixel.r >= linear.value)
                {
                    color = new Color(1, 1, 1, pixel.a);
                }
                else
                {
                    color = new Color(0, 0, 0, pixel.a);
                }
                newImage.SetPixel(x, y, color);
            }
        }
        saveImage(newImage, newSprite.name + " - linear");
    }

    public void removeNoise()
    {
        newSprite = image.sprite;
        Texture2D newImage = new Texture2D(newSprite.texture.width, newSprite.texture.height);

        Texture2D itemBGTex = newSprite.texture;

        for (int y = 1; y < newImage.height - 1; y++)
        {
            for (int x = 1; x < newImage.width - 1; x++)
            {
                Color pixel = newSprite.texture.GetPixel(x, y);
                Color color = new Color(pixel.r, pixel.g, pixel.b);
                Color[] vetor = null;

                if (noiseOption.value == 0)
                {
                    Color[] viz3 = new Color[9];
                    viz3[0] = newSprite.texture.GetPixel(x, y - 1);
                    viz3[1] = newSprite.texture.GetPixel(x, y + 1);
                    viz3[2] = newSprite.texture.GetPixel(x - 1, y);
                    viz3[3] = newSprite.texture.GetPixel(x + 1, y);
                    viz3[4] = newSprite.texture.GetPixel(x - 1, y + 1);
                    viz3[5] = newSprite.texture.GetPixel(x + 1, y - 1);
                    viz3[6] = newSprite.texture.GetPixel(x - 1, y - 1);
                    viz3[7] = newSprite.texture.GetPixel(x + 1, y + 1);
                    viz3[8] = pixel;
                    vetor = viz3;
                }
                if (noiseOption.value == 1) { 
                    Color[] vizX = new Color[5];
                    vizX[0] = newSprite.texture.GetPixel(x - 1, y + 1);
                    vizX[1] = newSprite.texture.GetPixel(x + 1, y - 1);
                    vizX[2] = newSprite.texture.GetPixel(x - 1, y - 1);
                    vizX[3] = newSprite.texture.GetPixel(x + 1, y + 1);
                    vizX[4] = pixel;
                    vetor = vizX;
                }
                if (noiseOption.value == 2)
                {
                    Color[] vizC = new Color[5];
                    vizC[0] = newSprite.texture.GetPixel(x, y - 1);
                    vizC[1] = newSprite.texture.GetPixel(x, y + 1);
                    vizC[2] = newSprite.texture.GetPixel(x - 1, y);
                    vizC[3] = newSprite.texture.GetPixel(x + 1, y);
                    vizC[4] = pixel;
                    vetor = vizC;
                }

                float red = mediana(vetor, PixelColor.RED);
                float green = mediana(vetor, PixelColor.GREEN);
                float blue = mediana(vetor, PixelColor.BLUE);
                Color colorN = new Color(red, green, blue, color.a);
                newImage.SetPixel(x, y, colorN);
            }
        }
        saveImage(newImage, newSprite.name + " - noise");
    }

    private float mediana(Color[] vetor, PixelColor sec)
    {
        float[] v = new float[vetor.Length];
        for(int i = 0; i < vetor.Length; i++)
        {
            if(sec.Equals(PixelColor.RED))
            {
                v[i] = vetor[i].r;
            }
            if (sec.Equals(PixelColor.GREEN))
            {
                v[i] = vetor[i].g;
            }
            if (sec.Equals(PixelColor.BLUE))
            {
                v[i] = vetor[i].b;
            }
        }
        Array.Sort(v);
        return v[v.Length / 2];
    }

    private void saveImage(Texture2D newImage, string name)
    {
        newImage.Apply();
        byte[] itemBGBytes = newImage.EncodeToPNG();
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(newImage, new Rect(0.0f, 0.0f, newImage.width, newImage.height), new Vector2(0.5f, 0.5f), 100.0f);
        File.WriteAllBytes("Assets/Images/" + name + ".png", itemBGBytes);
    }
}

public enum PixelColor { 
    RED, GREEN, BLUE
}
