using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DeckSpawner
{

    [MenuItem("Example/Setup ScriptableObject Card Example")]
    static void MenuCallBack()
    {
        List<string> elementList = new List<string>() { "Fire", "Wind", "Water", "Earth" };

        for (int i = 0; i < 15; i++)
        {
            int number = Random.Range(1, 13);
            int element = Random.Range(0, 4);
            string cardName = "Attack" + number + elementList[element];
            string cardPath = "Assets/Data/" + cardName + ".asset";

            // Step 1 - Create or reload the assets that store each Deck object.
            Deck card = AssetDatabase.LoadAssetAtPath<Deck>(cardPath);
            if (card == null)
            {
                // Create and save ScriptableObject because it doesn't exist yet
                card = ScriptableObject.CreateInstance<Deck>();
                card.Id = i;
                card.Name = "Attack";
                card.Description = "Deal 1 dmg";

                card.Number = number;
                card.Element = elementList[element];
                if (element == 0 || element == 1)
                {
                    card.Color = "Red";
                }
                else if (element == 2 || element == 3)
                {
                    card.Color = "Black";
                }
                AssetDatabase.CreateAsset(card, cardPath);
            }
        }

        for (int i = 15; i < 25; i++)
        {
            int number = Random.Range(1, 13);
            int element = Random.Range(0, 2);
            string cardName = "Dodge" + number + elementList[element];
            string cardPath = "Assets/Data/" + cardName + ".asset";

            // Step 1 - Create or reload the assets that store each Deck object.
            Deck card = AssetDatabase.LoadAssetAtPath<Deck>(cardPath);
            if (card == null)
            {
                // Create and save ScriptableObject because it doesn't exist yet
                card = ScriptableObject.CreateInstance<Deck>();
                card.Id = i;
                card.Name = "Dodge";
                card.Description = "Evade an Attack";

                card.Number = number;
                card.Element = elementList[element];
                card.Color = "Red";
                AssetDatabase.CreateAsset(card, cardPath);
            }
        }

       // Step 2 - Create some example vehicles in the current scene
       //Deck Attack1Earth = AssetDatabase.LoadAssetAtPath<Deck>("Assets/Data/Attack1Earth.asset");
       // {
       //     var card = GameObject.CreatePrimitive(PrimitiveType.Plane);
       //     card.name = "Attack1Earth";

       //     var cardInfo = card.AddComponent<DeckInstance>();
       //     cardInfo.Initialize(Attack1Earth);
       // }
    }
}
