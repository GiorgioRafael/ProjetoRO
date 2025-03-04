using UnityEngine;

[CreateAssetMenu(fileName ="PassiveItemScriptableObject", menuName = "ScriptableObjects/Passive Item")]
public class PassiveItemScriptableObject : ScriptableObject
{
    [SerializeField]
    float multipler;
    public float Multipler { get => multipler; private set => multipler = value; }
    

    [SerializeField]
    int level; //nao e pra ser alterado no jogo [so no editor]
    public int Level { get => level; private set => level = value; }
    
    [SerializeField]
    GameObject nextLevelPrefab; // prefab do proximo nivel (ou seja: qual objeto vira quando upa de nivel)
    public GameObject NextLevelPrefab { get => nextLevelPrefab; private set => nextLevelPrefab = value; }

        [SerializeField]
    Sprite icon; //nao é pra ser modificado
    public Sprite Icon { get => icon; private set => icon = value; }
}
