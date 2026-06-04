using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatUIList : MonoBehaviour
{
    [SerializeField] private Transform cardsParent;
    [SerializeField] private CatCard cardPrefab;

    [SerializeField] private GameOverManager gameOverManager;

    private Dictionary<CatNeeds, CatCard> cards = new Dictionary<CatNeeds, CatCard>();

    public int CatsCount => cards.Count;

    public void AddCat(CatNeeds catNeeds)
    {
        if (catNeeds == null || cards.ContainsKey(catNeeds))
            return;

        CatCard card = Instantiate(cardPrefab, cardsParent);
        card.Init(catNeeds);

        cards.Add(catNeeds, card);

        catNeeds.OnCatDied += RemoveCat;
    }

    private void RemoveCat(CatNeeds catNeeds)
    {
        if (catNeeds == null)
            return;

        catNeeds.OnCatDied -= RemoveCat;

        if (cards.TryGetValue(catNeeds, out CatCard card))
        {
            Destroy(card.gameObject);
            cards.Remove(catNeeds);
        }

        if (gameOverManager != null)
            gameOverManager.CheckGameOver();
    }

    public void ClearAll()
    {
        foreach (var pair in cards)
        {
            if (pair.Value != null)
                Destroy(pair.Value.gameObject);

            if (pair.Key != null)
                pair.Key.OnCatDied -= RemoveCat;
        }

        cards.Clear();
    }
}
