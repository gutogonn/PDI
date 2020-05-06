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

    public Button inverterButton;
    private Sprite q1, q2, q3, q4;
    public Toggle q1inv, q2inv, q3inv, q4inv;

    public Button verificaRectButton;


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
        inverterButton.onClick.AddListener(inverterImagem);
        verificaRectButton.onClick.AddListener(scanImageRetangulo);
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
        GetComponent<SpriteRenderer>().sprite = saveImage(newImage, newSprite.name + " - GrayScale", true);
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
        GetComponent<SpriteRenderer>().sprite = saveImage(newImage, newSprite.name + " - linear", true);
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
        GetComponent<SpriteRenderer>().sprite = saveImage(newImage, newSprite.name + " - noise", true);
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

    public void inverterImagem()
    {
        //GetComponent<SpriteRenderer>().sprite = invertImage(image.sprite);
        q1 = q1inv.isOn ? invertImage(scanQuadrant(image.sprite, 0, image.sprite.texture.width / 2, 0, image.sprite.texture.height / 2)) : scanQuadrant(image.sprite, 0, image.sprite.texture.width / 2, 0, image.sprite.texture.height / 2);
        Quadrante quad1 = new Quadrante(q1, new Vector2(0, image.sprite.texture.width / 2), new Vector2(0, image.sprite.texture.height / 2));

        q2 = q2inv.isOn ? invertImage(scanQuadrant(image.sprite, image.sprite.texture.width / 2, image.sprite.texture.width, image.sprite.texture.height / 2, image.sprite.texture.height)) : scanQuadrant(image.sprite, image.sprite.texture.width / 2, image.sprite.texture.width, image.sprite.texture.height / 2, image.sprite.texture.height);
        Quadrante quad2 = new Quadrante(q2, new Vector2(image.sprite.texture.width / 2, image.sprite.texture.width), new Vector2(image.sprite.texture.height / 2, image.sprite.texture.height));

        q3 = q3inv.isOn ? invertImage(scanQuadrant(image.sprite, 0, image.sprite.texture.width / 2, image.sprite.texture.height / 2, image.sprite.texture.height)) : scanQuadrant(image.sprite, 0, image.sprite.texture.width / 2, image.sprite.texture.height / 2, image.sprite.texture.height);
        Quadrante quad3 = new Quadrante(q3, new Vector2(0, image.sprite.texture.width / 2), new Vector2(image.sprite.texture.height / 2, image.sprite.texture.height));

        q4 = q4inv.isOn ? invertImage(scanQuadrant(image.sprite, image.sprite.texture.width / 2, image.sprite.texture.width, 0, image.sprite.texture.height / 2)) : scanQuadrant(image.sprite, image.sprite.texture.width / 2, image.sprite.texture.width, 0, image.sprite.texture.height / 2);
        Quadrante quad4 = new Quadrante(q4, new Vector2(image.sprite.texture.width / 2, image.sprite.texture.width), new Vector2(0, image.sprite.texture.height / 2));

        GetComponent<SpriteRenderer>().sprite = buildingSpriteWithQuadrant(quad1, quad2, quad3, quad4);
    }

    public Sprite buildingSpriteWithQuadrant(Quadrante quad1, Quadrante quad2, Quadrante quad3, Quadrante quad4)
    {
        newSprite = image.sprite;

        Texture2D newImage = new Texture2D(newSprite.texture.width, newSprite.texture.height);
        Texture2D itemBGTex = newSprite.texture;

        for (int y = 0; y < newImage.height; y++)
        {
            for (int x = 0; x < newImage.width; x++)
            {
                Sprite quad = null;

                if (y >= quad1.altura.x && y < quad1.altura.y && x >= quad1.largura.x && x < quad1.largura.y)
                {
                    quad = quad1.spirte;
                }
                if (y >= quad2.altura.x && y < quad2.altura.y && x >= quad2.largura.x && x < quad2.largura.y)
                {
                    quad = quad2.spirte;
                }
                if (y >= quad3.altura.x && y < quad3.altura.y && x >= quad3.largura.x && x < quad3.largura.y)
                {
                    quad = quad3.spirte;
                }
                if (y >= quad4.altura.x && y < quad4.altura.y && x >= quad4.largura.x && x < quad4.largura.y)
                {
                    quad = quad4.spirte;
                }


                Color pixel = quad.texture.GetPixel(x, y);
                Color color = new Color(pixel.r, pixel.g, pixel.b, 1);

                newImage.SetPixel(x, y, color);
            }
        }

        return saveImage(newImage, newSprite.name + " - inverted", true);
    }

    public Sprite invertImage(Sprite sprite)
    {
        newSprite = sprite;

        Texture2D newImage = new Texture2D(newSprite.texture.width, newSprite.texture.height);
        Texture2D itemBGTex = newSprite.texture;

        for (int y = 0; y < newImage.height; y++)
        {
            for (int x = 0; x < newImage.width; x++)
            {
                Color pixel = newSprite.texture.GetPixel(x, newImage.height - y);
                Color color = new Color(pixel.r, pixel.g, pixel.b, 1);
                newImage.SetPixel(x, y, color);
            }
        }

        return saveImage(newImage, newSprite.name + " - inverted", true);
    }

    public Sprite scanQuadrant(Sprite sprite, int inix, int fimx, int iniy, int fimy)
    {
        newSprite = sprite;

        Debug.Log(inix + " - " + fimx + " 88 " + iniy + " - " + fimy);

        Texture2D newImage = new Texture2D(newSprite.texture.width, newSprite.texture.height);
        Texture2D newImage2 = new Texture2D(112, 175);

        for (int y = iniy; y < fimy; y++)
        {
            for (int x = inix; x < fimx; x++)
            {

                Color pixel = newSprite.texture.GetPixel(x, y);
                Color color = new Color(pixel.r, pixel.g, pixel.b, 1);
                newImage2.SetPixel(x, y, color);

            }
        }

        return saveImage(newImage2, newSprite.name + " - quad", true);
    }

    public void scanImageRetangulo()
    {

        bool hasBlank = false;

        newSprite = image.sprite;
        Texture2D newImage = new Texture2D(newSprite.texture.width, newSprite.texture.height);
        Texture2D itemBGTex = newSprite.texture;

        for (int y = 0; y < newImage.height; y++)
        {
            for (int x = 0; x < newImage.width; x++)
            {
                Color pixel = newSprite.texture.GetPixel(x, y);
                Color color = new Color(pixel.r, pixel.g , pixel.b, 1);

                if (verificaBranco(color))
                {
                    hasBlank = true;
                }
            }
        }
        Debug.Log(hasBlank);
    }

    public bool verificaBranco(Color color)
    {
        return color.r == 0 && color.g == 0 && color.b == 0;
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
        GetComponent<SpriteRenderer>().sprite = saveImage(newImage, newSprite.name + " - adicionado", true);
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
        GetComponent<SpriteRenderer>().sprite = saveImage(newImage, newSprite.name + " - subtraido", true);
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

    public void equalizacaoHistograma(SpriteRenderer img, bool acc)
    {

        //int[] hR = gerarHistograma;

    }

    //private int[] histograma(SpriteRenderer img, PixelColor sec)
    //{
    //    newSprite = image.sprite;
    //    Texture2D newImage = new Texture2D(newSprite.texture.width, newSprite.texture.height);

    //    Texture2D itemBGTex = newSprite.texture;
    //    float[] v = new float[vetor.Length];
    //    for (int i = 0; i < vetor.Length; i++)
    //    {
    //        if (sec.Equals(PixelColor.RED))
    //        {
    //            v[i] = vetor[i].r;
    //        }
    //        if (sec.Equals(PixelColor.GREEN))
    //        {
    //            v[i] = vetor[i].g;
    //        }
    //        if (sec.Equals(PixelColor.BLUE))
    //        {
    //            v[i] = vetor[i].b;
    //        }
    //    }
    //    Array.Sort(v);
    //    return v;
    //}

    private Sprite saveImage(Texture2D newImage, string name, bool saveFile)
    {
        newImage.Apply();
        byte[] itemBGBytes = newImage.EncodeToPNG();
        if (saveFile)
        {
            File.WriteAllBytes("Assets/Resources/Images/" + name + ".png", itemBGBytes);
        }
        return Sprite.Create(newImage, new Rect(0.0f, 0.0f, newImage.width, newImage.height), new Vector2(0.5f, 0.5f), 100.0f);
    }
}

public enum PixelColor { 
    RED, GREEN, BLUE
}
