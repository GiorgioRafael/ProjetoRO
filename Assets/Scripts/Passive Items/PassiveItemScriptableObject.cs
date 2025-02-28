using UnityEngine;

[CreateAssetMenu(fileName ="PassiveItemScriptableObject", menuName = "ScriptableObjects/Passive Item")]
public class PassiveItemScriptableObject : ScriptableObject
{
    [SerializeField]
    float multipler;
    public float Multipler { get => multipler; private set => multipler = value; }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
