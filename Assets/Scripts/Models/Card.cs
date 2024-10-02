using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class Card : CardName
    {
        int Id { get; set; }
        int Number { get; set; }
        string Color { get; set; }
        string Element { get; set; }

        public override string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Description { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Card() { }

        public Card(string name, string description, int id, int number, string color, string element)
        {
            Name = name;
            Description = description;
            Id = id;
            Number = number;
            Color = color;
            Element = element;
        }
    }
}
