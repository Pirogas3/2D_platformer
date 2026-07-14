using System;

namespace Assets.Scripts.Utils
{
    public static class DamageCalc
    {
        public static int CalculateDamage(int baseDamage, int attack = 0, int defense = 0)
        {
            double modifier = (20.0 + attack) / (20.0 + defense);
            double rawDamage = baseDamage * modifier;

            // Округляем к ближайшему целому, при .5 — вверх (AwayFromZero)
            return (int)Math.Round(rawDamage, MidpointRounding.AwayFromZero);
        }
    }
}
