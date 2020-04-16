using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ImageController : MonoBehaviour
{
    public GraficoController graficoController;
    public Color data;
    public SpriteRenderer image;
    private Sprite newSprite;
    
    public Text rText, gText, bText;
    public Button grayscaleButton;

    public Button linearButton;
    public Slider linear;

    public Button noiseButton;
    public Dropdown noiseOption;

    public Button adicionarButton;
    public Button subtrairButton;
    public SpriteRenderer image2;
    private Sprite newSprite2;
    public Slider image1Pr;
    public Slider image2Pr;


    public Button histogramaButton;

    Color[] color;

    public void Start()
    {
        color = image.sprite.texture.GetPixels();

        grayscaleButton.onClick.AddListener(turnGray);
        linearButton.onClick.AddListener(turnLinear);
        noiseButton.onClick.AddListener(removeNoise);
        adicionarButton.onClick.AddListener(adicaoImagem);
        subtrairButton.onClick.AddListener(subtracaoImagem);
        histogramaButton.onClick.AddListener(gerarHistograma);
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

    public void adicaoImagem()
    {
        newSprite = image.sprite;
        newSprite2 = image2.sprite;

        Texture2D newImage = new Texture2D(newSprite.texture.width, newSprite.texture.height);
        Texture2D itemBGTex = newSprite.texture;

        for (int y = 0; y < newImage.height; y++)
        {
            for (int x = 0; x < newImage.width; x++)
            {
                Color pixel1 = newSprite.texture.GetPixel(x, y);
                Color pixel2 = newSprite2.texture.GetPixel(x, y);

                float r = (pixel1.r * image1Pr.value) + (pixel2.r * image2Pr.value);
                float g = (pixel1.g * image1Pr.value) + (pixel2.g * image2Pr.value);
                float b = (pixel1.b * image1Pr.value) + (pixel2.r * image2Pr.value);

                r = r > 1 ? 1 : r;
                g = g > 1 ? 1 : g;
                b = b > 1 ? 1 : b;

                Color newColor = new Color(r, g, b, 1);
                newImage.SetPixel(x, y, newColor);
            }
        }
        saveImage(newImage, newSprite.name + " - adicionado");
    }

    public void subtracaoImagem()
    {
        newSprite = image.sprite;
        newSprite2 = image2.sprite;

        Texture2D newImage = new Texture2D(newSprite.texture.width, newSprite.texture.height);
        Texture2D itemBGTex = newSprite.texture;

        for (int y = 0; y < newImage.height; y++)
        {
            for (int x = 0; x < newImage.width; x++)
            {
                Color pixel1 = newSprite.texture.GetPixel(x, y);
                Color pixel2 = newSprite2.texture.GetPixel(x, y);

                float r = (pixel1.r * image1Pr.value) - (pixel2.r * image2Pr.value);
                float g = (pixel1.g * image1Pr.value) - (pixel2.g * image2Pr.value);
                float b = (pixel1.b * image1Pr.value) - (pixel2.r * image2Pr.value);

                r = r < 0 ? 0 : r;
                g = g < 0 ? 0 : g;
                b = b < 0 ? 0 : b;

                Color newColor = new Color(r, g, b, 1);
                newImage.SetPixel(x, y, newColor);
            }
        }
        saveImage(newImage, newSprite.name + " - subtraido");
    }

    public void gerarHistograma()
    {
        histogramaGrafico(image, graficoController.graph1Sprite);
        histogramaGrafico(image2, graficoController.graph2Sprite);
        histogramaGrafico(GetComponent<SpriteRenderer>(), graficoController.graph3Sprite);
    }

    public void histogramaGrafico(SpriteRenderer image, Image grafSprite)
    {
        int[] qt = new int[256];
        Texture2D newImage = new Texture2D(image.sprite.texture.width, image.sprite.texture.height);
        for (int y = 0; y < newImage.height; y++)
        {
            for (int x = 0; x < newImage.width; x++)
            {
                qt[(int)(image.sprite.texture.GetPixel(x, y).r * 255)]++;
                qt[(int)(image.sprite.texture.GetPixel(x, y).g * 255)]++;
                qt[(int)(image.sprite.texture.GetPixel(x, y).b * 255)]++;
            }
        }
        graficoController.create(qt, image.sprite, grafSprite);
    }

    private void saveImage(Texture2D newImage, string name)
    {
        newImage.Apply();
        byte[] itemBGBytes = newImage.EncodeToPNG();
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(newImage, new Rect(0.0f, 0.0f, newImage.width, newImage.height), new Vector2(0.5f, 0.5f), 100.0f);
        File.WriteAllBytes("Assets/Resources/Images/" + name + ".png", itemBGBytes);
    }
}

public enum PixelColor { 
    RED, GREEN, BLUE
}
