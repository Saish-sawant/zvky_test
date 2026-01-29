using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] public SpriteRenderer image;

    private CardConfig config;

    public void ApplyConfig(CardConfig newConfig)
    {
        config = newConfig;
        image.sprite = config.sprite;
        Id = config.id;
        Value = config.value;
    }

    public int Id;// => config.id;
    public int Value;// => config.value;
}
