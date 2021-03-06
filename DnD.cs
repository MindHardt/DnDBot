using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DnDBot
{
    public class Character
    {
        public string Name { get; private set; }
        public Classes CharClass;
        public int Level;
        public Attribute Strength;
        public Attribute Dexterity;
        public Attribute Constitution;
        public Attribute Intellect;
        public Attribute Wisdom;
        public Attribute Charisma;

        public Initiative Init;

        public Fortitude Fortitude;
        public Reflex Reflex;
        public Will Will;

        public List<Feats> Features = new List<Feats>();

        public async void Initialize(string name)
        {
            Name = name;
            Strength     = new Attribute(Environment.CurrentDirectory + Name + ".txt", "STR");
            Dexterity    = new Attribute(Environment.CurrentDirectory + Name + ".txt", "DEX");
            Constitution = new Attribute(Environment.CurrentDirectory + Name + ".txt", "CON");
            Intellect    = new Attribute(Environment.CurrentDirectory + Name + ".txt", "INT");
            Wisdom       = new Attribute(Environment.CurrentDirectory + Name + ".txt", "WIS");
            Charisma     = new Attribute(Environment.CurrentDirectory + Name + ".txt", "CHA");
        }
    }
    public class Defence
    {
        public Character Owner;
        public int General
        {
            get
            {
                return (Armor.GetBiggestBuff() + Natural.GetBiggestBuff() + Magic.GetBiggestBuff() + Shield.GetBiggestBuff() + Owner.Dexterity.GetModifyer());
            }
        }
        public int Touch
        {
            get
            {
                return (Magic.GetBiggestBuff() + Owner.Dexterity.GetModifyer());
            }
        }
        public int Unaware
        {
            get
            {
                return (Armor.GetBiggestBuff() + Natural.GetBiggestBuff() + Magic.GetBiggestBuff() + Shield.GetBiggestBuff());
            }
        }
        public List<Buff> Armor   = new List<Buff>();
        public List<Buff> Natural = new List<Buff>();
        public List<Buff> Magic   = new List<Buff>();
        public List<Buff> Shield  = new List<Buff>();
        public Defence(Character Owner) { this.Owner = Owner; }
    }
    public class Stat : IRollable
    {
        protected Character Owner;
        public int Constant { get; private set; }
        public virtual int GetModifyer() { return 0; }
        public override string ToString()
        {
            int i = GetModifyer();
            return (i < 0 ? i.ToString() : "+" + i.ToString() );
        }
    }
    public class Fortitude : BattleStat, IRollable
    {
        private readonly Classes[] RelevantClasses = { Classes.Warrior, Classes.Priest, Classes.Barbarian };
        public override int GetModifyer()
        {
            return base.GetModifyer() + Owner.Constitution.GetModifyer();
        }
    }
    public class Reflex : BattleStat, IRollable
    {
        private readonly Classes[] RelevantClasses = { Classes.Rogue };
        public override int GetModifyer()
        {
            return base.GetModifyer() + Owner.Dexterity.GetModifyer();
        }
    }
    public class Will : BattleStat, IRollable
    {
        private readonly Classes[] RelevantClasses = { Classes.Mage, Classes.Priest };
        public override int GetModifyer()
        {
            return base.GetModifyer() + Owner.Wisdom.GetModifyer();
        }
    }
    public abstract class BattleStat : Stat, IRollable
    {
        private Classes[] RelevantClasses;
        private int ClassBonus
        {
            get
            {
                foreach (Classes cs in RelevantClasses)
                {
                    if (cs == Owner.CharClass) return (2 + Owner.Level / 2);
                }
                return (Owner.Level / 3);
            }
        }
        private Feats RelevantFeat;
        private int featBonus
        {
            get
            {
                return Owner.Features.Contains(RelevantFeat) ? 2 : 0;
            }
        }
        public int Temp;
        public virtual int GetModifyer()
        {
            return (ClassBonus + featBonus + Temp);
        }
    }
    public class Initiative : Stat, IRollable
    {
        public override int GetModifyer()
        {
            return (Owner.Dexterity.GetModifyer() + (Owner.Features.Contains(Feats.Advanced_initiative) ? 4 : 0));
        }
    }
    public class Attribute : Stat, IRollable
    {
        public int BaseValue { get; private set; } = 10;
        public int BaseModifyer
        {
            get
            {
                return ((BaseValue / 2) - 5);
            }
        }
        public int Temp;
        public List<Buff> Empower = new List<Buff>();
        public List<Buff> Morale = new List<Buff>();
        public override int GetModifyer()
        {
            return BaseModifyer + Empower.GetBiggestBuff() + Morale.GetBiggestBuff() + Temp;
        }
        public string Notation;
        public Attribute(string ownerName, string notification) { this.BaseValue = int.Parse(FileWorker.FindLine(ownerName, notification).ToString()); }
    }
    public class Buff
    {
        public int Amount { get; private set; }
        public string Source { get; private set; }
        public int Duration { get; private set; }
        public Buff(int amount, string source, int duration) { this.Amount = amount; this.Source = source; this.Duration = duration; }
    }
    public abstract class Item
    {
        public string Name;
        public uint Quantity = 1;
        public double Price;
    }
    public abstract class Weapon : Item
    {
        public string Damage;
        public int AttackBonus;
        public int Range;
        public Handling Handling;
    }
    public class Damage
    {
        private int diceCount;
        private int die;
        private int constant;
        private DamageTypes DamageType;
        public override string ToString()
        {
            string toString = String.Empty;
            if (diceCount > 0 && die > 0) toString += $"{diceCount}d{die}";
            if (constant != 0) toString += constant < 0 ? constant.ToString() : "+" + constant.ToString();
            return toString;
        }
        public string ToStringWithIcon()
        {
            return ToString() + DnD.DamageTypesIcons[DamageType];
        }
        public Damage(int diceCount, int die, int constant, DamageTypes damageType) { this.diceCount = diceCount; this.die = die; this.constant = constant; this.DamageType = damageType; }
    }
    public abstract class Slot
    {
        public string Name;
        public readonly EquipmentSlots Type;
        Item currentEquipment;
    }
    public enum Handling
    {
        Simple,
        Special,
        Improvised
    }
    public enum DamageTypes
    {
        Crushing,
        Chopping,
        Piercing,
        Cutting,
        Bite,
        Fire,
        Acid,
        Frost,
        Electricity,
        Sound,
        Light,
        Dark,
        Magic,
        Natural
    }
    public enum Classes
    { 
        Warrior,
        Mage,
        Priest,
        Rogue,
        Barbarian,
        Peasant
    }

    public enum EquipmentSlots
    {
        Head,
        Armor,
        LeftRing,
        RightRing,
        Hands,
        Forearm,
        Feet,
        Waist,
        Weapon,
        None
    }
    public enum Feats
    {
        Athlete,
        Blinded_combat,
        Massacre,
        Massacre_plus,
        Quick_recharge,
        Quick_draw,
        Swift_feeted,
        Quick_shot,
        Greater_deity_power,
        Impressive_fortitude,
        Impressive_fortitude_plus,
        Magic_strike,
        Distant_shot,
        Lasting_fury,
        Additional_deity_power,
        Lively,
        Defensive_stance,
        Electoral_deity_power,
        Exile_of_the_undead,
        Light_step,
        Light_step_plus,
        Precise_shot,
        Swift_reaction,
        Swift_reaction_plus,
        Special_weapon_handling,
        Simple_weapon_handling,
        Barrier,
        Crushing_strike,
        Will_of_steel,
        Will_of_steel_plus,
        Close_shot,
        Deadly_precision,
        Confident_weapon_handling,
        Confident_shield_handling,
        Evasion,
        Advanced_initiative,
        Empowered_spell,
        Fencing,
        Expert,
        Paired_weapons,
        Distant_healing,
        Battle_reflexes
    }
    public static class DnD
    {
        public static string ToDamageString(this Damage[] dmgArray)
        {
            string result = String.Empty;
            foreach (Damage dmg in dmgArray)
            {
                result += "+" + dmg.ToStringWithIcon();
            }
            result = result.Replace("+-", "-");
            result = result.Replace("++", "+");
            if (result[0] == '+') result = result.Substring(1);
            return result;
        }
        public static Dictionary<DamageTypes, string> DamageTypesIcons = new Dictionary<DamageTypes, string>
        {
            { DamageTypes.Crushing, ":hammer:" },
            { DamageTypes.Chopping, ":axe:" },
            { DamageTypes.Piercing, " :sewing_needle:" },
            { DamageTypes.Cutting, ":knife:"},
            { DamageTypes.Bite, ":tooth:"},
            { DamageTypes.Fire, ":flame:"},
            { DamageTypes.Acid, ":test_tube:"},
            { DamageTypes.Frost, ":snowflake:"},
            { DamageTypes.Electricity, ":zap:"},
            { DamageTypes.Sound, ":loud_sound:"},
            { DamageTypes.Light, ":sunny:"},
            { DamageTypes.Dark, ":new_moon:"},
            { DamageTypes.Magic, ":crystal_ball:"},
            { DamageTypes.Natural, ":natural:"}
        };
        public static string Roll(string s, out int result)
        {
            if (!CheckRollString(s, out int count, out int die, out int constant))
            {
                result = 0;
                return ("**Введенные данные неверны, проверьте правильность написания.**\nПример верного ввода: *=r 2d8+3*, *=r 1d20*.");
            }
            else
            {
                string feedback = constant < 0 ? $"**Бросок {count}d{die}{constant}**\n\n" : $"**Бросок {count}d{die}+{constant}**\n";
                int sum = constant;
                Random rnd = new Random();
                if (count > 10) feedback += "Вы бросаете слишком много кубов! Результаты бросков не будут приведены.";
                for (int i = 1; i <= count; i++)
                {
                    int currentDie = rnd.Next(1, die + 1);
                    if (count <= 10) feedback += $"\nКуб №{i} = **{currentDie}**";
                    sum += currentDie;
                }
                feedback += $"\nКонстанта = **{constant}**\nРЕЗУЛЬТАТ = **{sum}**:white_check_mark:";
                result = sum;
                return (feedback);
            };
        }
        private static bool CheckRollString(string s, out int count, out int die, out int constant)
        {
            Console.WriteLine(s);
            s = s.Replace("=r", "");
            s = s.Replace("d", " ");
            s = s.Replace("+", " ");
            s = s.Replace("-", " -");
            Console.WriteLine(s);
            count = 1;
            die = 1;
            constant = 0;
            string[] parts = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            bool result = false;
                 if (parts.Length == 2) result = int.TryParse(parts[0], out count) && int.TryParse(parts[1], out die) && count > 0 && die > 0;
            else if (parts.Length == 3) result = int.TryParse(parts[0], out count) && int.TryParse(parts[1], out die) && int.TryParse(parts[2], out constant) && count > 0 && die > 0;
            return (result);
        }
    }
    public static class ListExtension
    {
        public static int GetBiggestBuff(this List<Buff> list)
        {
            if (list.Count == 0) return 0;
            int max = -2147483648;
            foreach (Buff buff in list)
            {
                if (buff.Amount > max) max = buff.Amount;
            }
            return max;
        }
    }
    public interface IRollable
    {
        int GetModifyer();
    }


}
