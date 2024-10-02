using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public abstract class CardName
    {
        public abstract string Name { get; set; }
        public abstract string Description { get; set; }

        public CardName() { }

        public CardName(string name, string description)
        {
            Name = name;
            Description = description;
        }



    }
}
