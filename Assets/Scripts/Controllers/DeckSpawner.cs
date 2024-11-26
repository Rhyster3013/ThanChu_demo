using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DeckSpawner
{

    [MenuItem("Example/Setup ScriptableObject Card Example")]
    static void MenuCallBack()
    {
        List<string> elementList = new List<string>() { "Fire", "Wind", "Water", "Earth" };

        int a = 0;
        int b = 15;
        int c = 25;

        while ( a < 15)
        {
            int number = UnityEngine.Random.Range(1, 13);
            int element = UnityEngine.Random.Range(0, 4);
            string cardName = "Attack" + number + elementList[element];
            string cardPath = "Assets/Data/" + cardName + ".asset";

            // Step 1 - Create or reload the assets that store each Deck object.
            Deck card = AssetDatabase.LoadAssetAtPath<Deck>(cardPath);
            if (card == null)
            {
                // Create and save ScriptableObject because it doesn't exist yet
                card = ScriptableObject.CreateInstance<Deck>();
                card.Id = a;
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

                card.Damage = 1;
                card.Targets = 1;

                AssetDatabase.CreateAsset(card, cardPath);

                a++;
            }
        }

        while ( b < 25)
        {
            int number = UnityEngine.Random.Range(1, 13);
            int element = UnityEngine.Random.Range(0, 2);
            string cardName = "Dodge" + number + elementList[element];
            string cardPath = "Assets/Data/" + cardName + ".asset";

            // Step 1 - Create or reload the assets that store each Deck object.
            Deck card = AssetDatabase.LoadAssetAtPath<Deck>(cardPath);
            if (card == null)
            {
                // Create and save ScriptableObject because it doesn't exist yet
                card = ScriptableObject.CreateInstance<Deck>();
                card.Id = b;
                card.Name = "Dodge";
                card.Description = "Evade an Attack";

                card.Number = number;
                card.Element = elementList[element];
                card.Color = "Red";

                card.Damage = 0;
                card.Targets = 0;

                AssetDatabase.CreateAsset(card, cardPath);

                b++;
            }
        }


        while (c < 30)
        {
            int number = UnityEngine.Random.Range(1, 13);
            int element = UnityEngine.Random.Range(0, 2);
            string cardName = "Heal" + number + elementList[element];
            string cardPath = "Assets/Data/" + cardName + ".asset";

            // Step 1 - Create or reload the assets that store each Deck object.
            Deck card = AssetDatabase.LoadAssetAtPath<Deck>(cardPath);
            if (card == null)
            {
                // Create and save ScriptableObject because it doesn't exist yet
                card = ScriptableObject.CreateInstance<Deck>();
                card.Id = c;
                card.Name = "Heal";
                card.Description = "Heal 1 HP or Rescue 1 player";

                card.Number = number;
                card.Element = elementList[element];
                card.Color = "Red";

                card.Damage = 0;
                card.Targets = -1;

                AssetDatabase.CreateAsset(card, cardPath);

                c++;
            }
        }
    }
}
