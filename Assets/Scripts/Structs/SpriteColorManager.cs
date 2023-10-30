using System.Collections.Generic;
using UnityEngine;

public class SpriteColorManager : MonoBehaviour {
    private Color defaultColor;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private List<Color> colors;

    public void Awake() {
        spriteRenderer ??= GetComponent<SpriteRenderer>();
        colors = new List<Color>();
        defaultColor = spriteRenderer.color;
    }
    public Color AddColor(Color color) {
        colors.Add(color);
        spriteRenderer.color = CurrentColor();
        return spriteRenderer.color;
    }
    public Color RemoveColor(Color color) {
        colors.Remove(color);
        spriteRenderer.color = CurrentColor();
        return spriteRenderer.color;
    }
    public Color CurrentColor() {
        Color final = defaultColor;
        for(int i=0; i<colors.Count; i++)
            final = new Color(final.r * colors[i].r, final.g * colors[i].g, final.b * colors[i].b, final.a * colors[i].a);
        return final;
    }
}