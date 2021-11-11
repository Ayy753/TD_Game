using DefaultNamespace;
using DefaultNamespace.EffectSystem;
using DefaultNamespace.IO;
using UnityEngine;
using Zenject;

public class EffectGroupInitializer : IInitializable{
    readonly EffectParserJSON effectParser;

    EffectGroupInitializer(EffectParserJSON effectParser) {
        Debug.Log("effect group init");
        this.effectParser = effectParser;
    }

    public void Initialize() {
        //  Set effect group for this towerdata object

        TowerData[] towerDatas = Resources.LoadAll<TowerData>("ScriptableObjects/TileData/StructureData/TowerData");

        foreach (TowerData towerData in towerDatas) {
            EffectGroup effectGroup = effectParser.GetEffectGroup(towerData.ProjectileName);
            towerData.SetEffectGroup(effectGroup);
        }
    }
}
