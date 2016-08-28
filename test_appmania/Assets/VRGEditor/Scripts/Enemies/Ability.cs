using UnityEngine;
using System.Collections;

[System.Serializable]
public class Ability{
	
		public SpecialAbilities Type;
		public AttackType AttackType;
		public int Chance;
		public int Damage;
		public float Range;

		public bool IsUse(){
			int r = Random.Range(0, 100);
			return (!GameManager.IsTutor && r < Chance) || (GameManager.IsTutor && Chance == 100);
		}
}
	

