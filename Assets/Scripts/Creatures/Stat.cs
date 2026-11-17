using System;
using UnityEngine;

public abstract class Stat : MonoBehaviour
{
   [SerializeField] protected Trait trait;
   
   
   public Stat(Trait trait)
   {
      this.trait = trait;
   }

   public virtual Trait GetTrait()
   {
      return trait;
   }

   // Default inheritance for any trait: the max value comes from one of the parents (or mutates higher).
   // One parent may be null if only the other parent has this trait.
   public virtual bool Inherit(Stat parentA, Stat parentB, MutationTable table, System.Random rng)
   {
      Trait a = (parentA != null ? parentA : parentB).GetTrait();
      Trait b = (parentB != null ? parentB : parentA).GetTrait();

      // Keep the same fill level the trait started with (e.g. Health starts full, Waste starts empty).
      float fill = trait.MaxValue > 0 ? trait.Value / trait.MaxValue : 0;
      trait.MaxValue = Genetics.InheritStat(a.MaxValue, b.MaxValue, table, rng, out bool mutated);
      trait.Value = trait.MaxValue * fill;
      return mutated;
   }

   public virtual string ToString()
   {
      string formattedString = "";
      formattedString = trait.Name+ ": " + Mathf.Round(trait.Value) + "/" + trait.MaxValue;
      return formattedString;
   }
   
}
