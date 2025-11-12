using UnityEngine;

[CreateAssetMenu(fileName = "GunProperties", menuName = "Scriptable Objects/GunProperties")]
public class GunProperties : ScriptableObject
{
    public string gunName = "Unnamed";
    public int magazineCapacity = 6;
    public float fireRate = 5f;
    public float range = 100f;
    public int damage = 25;
}
