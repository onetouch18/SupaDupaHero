using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DamageEventArgs : EventArgs
{
    public float Damage { get; set; }
    public HitZones Zone { get; set; }

    public DamageEventArgs(float damage, HitZones zone)
    {
        Damage = damage;
        Zone = zone;
    }

    public override string ToString()
    {
        return string.Format("{0} - {1}", Zone, Damage);
    }
}

public class HitController : MonoBehaviour
{
    public event EventHandler<DamageEventArgs> OnDamage;
    public HitZone[] Zones;

    public void Start()
    {
        foreach (var hitZone in Zones)
        {
            hitZone.Controller = this;
        }
    }

    public void Damage(object sender, DamageEventArgs args)
    {	
		if (OnDamage != null)
			OnDamage (sender, args);
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(HitController))]
    class HitControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var obj = target as HitController;

            if (obj.Zones != null)
            {
                for (var i = 0; i < obj.Zones.Length; i++)
                {
                    if (obj.Zones[i] == null)
                    {
                        obj.Zones = obj.GetComponentsInChildren<HitZone>();
                        break;
                    }

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(string.Format("{0,-10}:", obj.Zones[i].Zone.ToString()));
                    GUILayout.FlexibleSpace();
                    obj.Zones[i].Modifier = (DamageModifier)EditorGUILayout.EnumPopup(obj.Zones[i].Modifier);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Value:");
                    obj.Zones[i].ModifierValue = EditorGUILayout.FloatField(obj.Zones[i].ModifierValue);
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(); EditorGUILayout.Space();

            GUI.color = Color.grey;
            if (GUILayout.Button("Get elements"))
            {
                obj.Zones = obj.GetComponentsInChildren<HitZone>();
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
        }
    }
#endif
}
