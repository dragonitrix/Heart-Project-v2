using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class ExtensionMethods
{
    public static Texture2D GetRTPixels(this RenderTexture rt)
    {
        // Remember currently active render texture
        RenderTexture currentActiveRT = RenderTexture.active;

        // Set the supplied RenderTexture as the active one
        RenderTexture.active = rt;

        // Create a new Texture2D and read the RenderTexture image into it
        Texture2D tex = new Texture2D(rt.width, rt.height);
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        //tex.Apply();

        // Restore previously active render texture
        RenderTexture.active = currentActiveRT;
        return tex;
    }

    public static Texture2D CombineTexture2D(this Texture2D background, Texture2D watermark)
    {
        int startX = 0;
        int startY = 0;
        //Texture2D newTex = new Texture2D(background.width, background.height, background.format, false);
        for (int x = 0; x < background.width; x++)
        {
            for (int y = 0; y < background.height; y++)
            {
                if (x >= startX && y >= startY && x < watermark.width && y < watermark.height)
                {
                    Color bgColor = background.GetPixel(x, y);
                    Color wmColor = watermark.GetPixel(x - startX, y - startY);

                    Color final_color = Color.Lerp(bgColor, wmColor, wmColor.a / 1.0f);

                    background.SetPixel(x, y, final_color);
                }
                //else
                //newTex.SetPixel(x, y, background.GetPixel(x, y));
            }
        }

        //newTex.Apply();
        return background;
    }
    public static Texture2D GetReadableTexture2d(this Texture texture)
    {
        var tmp = RenderTexture.GetTemporary(
            texture.width,
            texture.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear
        );
        Graphics.Blit(texture, tmp);

        var previousRenderTexture = RenderTexture.active;
        RenderTexture.active = tmp;

        var texture2d = new Texture2D(texture.width, texture.height);
        texture2d.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        texture2d.Apply();

        RenderTexture.active = previousRenderTexture;
        RenderTexture.ReleaseTemporary(tmp);
        return texture2d;
    }

    //public static Texture2D CombineTexture2D(Texture2D background, Texture2D watermark)
    //{
    //    int startX = 0;
    //    int startY = 0;
    //    Texture2D newTex = new Texture2D(background.width, background.height, background.format, false);
    //    for (int x = 0; x < background.width; x++)
    //    {
    //        for (int y = 0; y < background.height; y++)
    //        {
    //            if (x >= startX && y >= startY && x < watermark.width && y < watermark.height)
    //            {
    //                Color bgColor = background.GetPixel(x, y);
    //                Color wmColor = watermark.GetPixel(x - startX, y - startY);
    //
    //                Color final_color = Color.Lerp(bgColor, wmColor, wmColor.a / 1.0f);
    //
    //                newTex.SetPixel(x, y, final_color);
    //            }
    //            else
    //                newTex.SetPixel(x, y, background.GetPixel(x, y));
    //        }
    //    }
    //
    //    newTex.Apply();
    //    return newTex;
    //}

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    public static void ShowAll(this CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    public static void HideAll(this CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    public static void Toggle(this CanvasGroup canvasGroup)
    {
        var value = !(canvasGroup.alpha == 1 ? true : false);
        canvasGroup.alpha = value ? 1 : 0;
        canvasGroup.interactable = value;
        canvasGroup.blocksRaycasts = value;
    }
}
