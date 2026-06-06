using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogDanger : MonoBehaviour
{
    [SerializeField] private float damageAmount = 40f;
    //[SerializeField] private float dangerLevel = 10f;

    private GraphNode node;
    //private float oldDangerLevel;

    public GraphNode Node => node;

    public void Init(GraphNode targetNode)
    {
        node = targetNode;

        //oldDangerLevel = node.DangerLevel;
        //node.DangerLevel = dangerLevel;
    }

    public void BiteCat(CatNeeds catNeeds)
    {
        if (catNeeds == null || catNeeds.IsDead)
            return;

        catNeeds.Damage(damageAmount);

        if (catNeeds.health <= 0f)
        {
            InjuredCat injuredCat = catNeeds.GetComponent<InjuredCat>();

            if (injuredCat != null)
                injuredCat.EscapeAfterDog();
        }
    }

    public void Remove()
    {
        /*if (node != null)
        {
            node.DangerLevel = oldDangerLevel;
        }*/

        Destroy(gameObject);
    }
}
