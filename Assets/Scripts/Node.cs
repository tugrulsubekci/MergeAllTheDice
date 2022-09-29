using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Node : MonoBehaviour
{
    public Vector2 Pos => transform.position;
    public bool isIndicated;
    public void ChangeAlpha(float alphaValue,bool indicated)
    {
        Color color = transform.GetComponent<SpriteRenderer>().color;
        color.a = alphaValue;
        transform.GetComponent<SpriteRenderer>().color = color;
        isIndicated = indicated;
    }
}
