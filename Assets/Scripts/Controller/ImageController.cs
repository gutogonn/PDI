using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ImageController : MonoBehaviour
{
    public Color data;
    public SpriteRenderer image;
    public Sprite newSprite;
    public Text rText, gText, bText;
    public Button grayscaleButton;

    Color[] color;

    public void Start()
    {
        color = image.sprite.texture.GetPixels();
        grayscaleButton.onClick.AddListener(turnGray);
        //GetComponent<SpriteRenderer>().sprite = newSprite;
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
                Color pixel = newSprite.texture.GetPixel(x,y);
                Color color = new Color((pixel.r + pixel.g + pixel.b)/3, (pixel.r + pixel.g + pixel.b) / 3, (pixel.r + pixel.g + pixel.b) / 3, 1);
                newImage.SetPixel(x, y, color);
            }
        }
        newImage.name = image.name + " - GrayScale";
        newImage.Apply();
        byte[] itemBGBytes = newImage.EncodeToPNG();
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(newImage, new Rect(0.0f, 0.0f, newImage.width, newImage.height), new Vector2(0.5f, 0.5f), 100.0f);
        File.WriteAllBytes("Assets/Images/" + newSprite.name + " - GrayScale.png", itemBGBytes);
    }
}
