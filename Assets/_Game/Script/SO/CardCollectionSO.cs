using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(
    fileName = "CardDatabase",
    menuName = "Slot/Card Database",
    order = 1)]
public class CardCollectionSO : ScriptableObject
{
    [SerializeField] private List<CardConfigSO> cards;

    private Dictionary<int, CardConfigSO> lookup;

    private void OnEnable()
    {
        BuildLookup();
    }

    private void BuildLookup()
    {
        lookup = new Dictionary<int, CardConfigSO>();

        foreach (var card in cards)
        {
            if (!lookup.ContainsKey(card.config.id))
                lookup.Add(card.config.id, card);
        }
    }

    // 🔁 RANDOM CARD
    public CardConfigSO GetRandom()
    {
        return cards[Random.Range(0, cards.Count)];
    }

    // 🎯 PREDETERMINED CARD
    public CardConfigSO GetById(int id)
    {
        if (lookup.TryGetValue(id, out var card))
            return card;

        Debug.LogError($"Card with ID {id} not found!");
        return null;
    }
}
