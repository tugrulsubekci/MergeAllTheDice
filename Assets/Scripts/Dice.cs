using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Dice : MonoBehaviour
{
    private int value;
    private Material material => GetComponent<MeshRenderer>().material;
    public Vector2 Pos => transform.position;
    public Transform _transform;
    private void Start()
    {
        _transform = transform;
    }
    public void Init(DiceType diceType)
    {
        diceType.value = value;
        transform.rotation = Quaternion.Euler(diceType.rotation);
        material.mainTexture = diceType.texture;
    }
}