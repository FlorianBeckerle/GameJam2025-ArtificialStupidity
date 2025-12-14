using NUnit.Framework;
using UnityEngine;

public class SpriteSortLayer : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    
    // Increase this if you want more precision between objects
    [SerializeField] private int sortingMultiplier = 1000;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    void LateUpdate()
    {
        _spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * sortingMultiplier);
    }
}
