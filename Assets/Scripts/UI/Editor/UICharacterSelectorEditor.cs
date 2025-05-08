using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEditor.Events;
using UnityEngine.UI;

[DisallowMultipleComponent]
[CustomEditor(typeof(UICharacterSelector))]
public class UICharacterSelectorEditor : Editor
{
    UICharacterSelector selector;

    void OnEnable()
    {
        selector = target as UICharacterSelector;        
    }

    public override void OnInspectorGUI()
    {
       //cria um botao no inspector, que cria os templates quando clicado 
        base.OnInspectorGUI();
        if(GUILayout.Button("Generate Selectable Characters"))
        {
            CreateTogglesForCharactersData();
        }
    }
    
    public void CreateTogglesForCharactersData()
    {
        //if the toggle template is not assinged leave a warning and abort
        if(!selector.toggleTemplate)
        {
            Debug.LogWarning("Please assign a toggle template for the UI character selector first.");
            return;
        }

        //loopa por todas as childrens do pai do toggle template
        //e deleta tudo abaixo dele exceto o template
       for(int i = selector.toggleTemplate.transform.parent.childCount - 1; i >= 0; i--)
       {
            Toggle tog = selector.toggleTemplate.transform.parent.GetChild(i).GetComponent<Toggle>();
            if(tog == selector.toggleTemplate) continue;
            Undo.DestroyObjectImmediate(tog.gameObject); //grava a ação (para depois podermos desfazer)
       } 

        //grava as mudanças feitas no componente UiCharacterSelector como desfaziveis e limpa a lista 
        Undo.RecordObject(selector, "Updates to UICharacterSelector.");
        selector.selectableToggles.Clear();
        CharacterData[] characters = UICharacterSelector.GetAllCharacterDataAssets();

        //para cada characterdata no projeto, criamos um toggle para ele no seletor de personagem
        for (int i = 0; i < characters.Length; i++)
        {
            Toggle tog;
            if(i ==0)
            {
                tog = selector.toggleTemplate;
                Undo.RecordObject(tog, "Modifying the template.");
            }
            else
            {
                tog = Instantiate(selector.toggleTemplate, selector.toggleTemplate.transform.parent);
                Undo.RegisterCreatedObjectUndo(tog.gameObject,"Created new toggle.");
            }

            //encontra o nome do personagem, icone, icone da arma para assign
            Transform characterName = tog.transform.Find(selector.characterNamePath);
            if(characterName && characterName.TryGetComponent(out TextMeshProUGUI tmp))
            {
                tmp.text = tog.gameObject.name = characters[i].Name;
            }

            Transform characterIcon = tog.transform.Find(selector.characterIconPath);
            if(characterIcon && characterIcon.TryGetComponent(out Image chrIcon))
                chrIcon.sprite = characters[i].Icon;
            
            Transform weaponIcon = tog.transform.Find(selector.weaponIconPath);
            if(weaponIcon && weaponIcon.TryGetComponent(out Image wpnIcon))
            {
                wpnIcon.sprite = characters[i].StartingWeapon.icon;
            }

            selector.selectableToggles.Add(tog);

            //remove todos os eventos selecionados e adcionado o seu proprio evento que verifica qual foi clicado
            for(int j = 0; j < tog.onValueChanged.GetPersistentEventCount(); j++)
            {
                if(tog.onValueChanged.GetPersistentMethodName(j) == "Select")
                {
                    UnityEventTools.RemovePersistentListener(tog.onValueChanged, j);
                }
            }
            UnityEventTools.AddObjectPersistentListener(tog.onValueChanged, selector.Select, characters[i]);
        }

        //registra as mudanças para ser salvadas quando pronto
        EditorUtility.SetDirty(selector); 
    }
}
