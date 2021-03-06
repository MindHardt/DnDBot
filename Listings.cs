using System;

namespace DnDBot
{
    public abstract class Skill: IRollable
    {
        Character Owner;
        public uint Points { get; private set; }
        private readonly Classes[] ClassialFor = null;
        private readonly int relevantAttribute;
        private bool requiresPoints;
        public int other;
        public int GetModifyer()
        {
            if (requiresPoints && Points == 0) return -2147483648;
            else return (0);
        }
        public class Acrobatics : Skill
        {
            private readonly Classes[] ClassialFor = { Classes.Rogue, Classes.Barbarian };
            private int relevantAttribute
            {
                get
                {
                    return Owner.Dexterity.GetModifyer();
                }
            }
            private bool requiresPoints = false;
        }
        public class Bluff : Skill
        {
            private readonly Classes[] ClassialFor = { Classes.Rogue };
            private int relevantAttribute
            {
                get
                {
                    return Owner.Charisma.GetModifyer();
                }
            }
            private bool requiresPoints = false;
        }
    }
}
