namespace DefaultNamespace {

    using DefaultNamespace.EffectSystem;
    using DefaultNamespace.IO;
    using DefaultNamespace.TilemapSystem;
    using System.Collections.Generic;
    using UnityEngine;
    using Zenject;

    /// <summary>
    /// Instantiates structure gameobjects based on tilemap placeholders at initalization time
    /// </summary>
    public class PrefabTileInstantiator : IInitializable {
        private IMapManager mapManager;
        private EffectParserJSON effectParser;
        private RadiusRenderer radiusRenderer;

        private Dictionary<TotemData, GameObject> totemToPrefab;
        private readonly Vector3 TILEMAP_OFFSET = new Vector3(0.5f, 0.5f, 0);

        public PrefabTileInstantiator(IMapManager mapManager, EffectParserJSON effectParser, RadiusRenderer radiusRenderer) {
            this.mapManager = mapManager;
            this.effectParser = effectParser;
            this.radiusRenderer = radiusRenderer;
        }

        public void Initialize() {
            totemToPrefab = new Dictionary<TotemData, GameObject>();
            GameObject[] totemPrefabs = Resources.LoadAll<GameObject>("Prefabs/Totems");

            foreach (GameObject prefab in totemPrefabs) {
                if (prefab.name != "BaseTotem") {
                    totemToPrefab.Add(prefab.GetComponent<Totem>().totemData, prefab);
                }
            }

            InitializeTotemData();
            InstantiateTotems();
        }

        /// <summary>
        /// Sets totemdata's effect group from json file
        /// </summary>
        private void InitializeTotemData() {
            foreach (TotemData totemData in totemToPrefab.Keys) {
                EffectGroup effectGroup = effectParser.GetEffectGroup(totemData.EffectName);
                totemData.SetEffectGroup(effectGroup);
            }
        }

        /// <summary>
        /// Creates totem gameobject for each totem placeholder on tilemap
        /// </summary>
        private void InstantiateTotems() {
            Vector3Int[] tilePositions = mapManager.GetTilePositionsOnLayer(MapLayer.StructureLayer);

            foreach (Vector3Int position in tilePositions) {
                TilemapSystem.TileData tileData = mapManager.GetTileData(MapLayer.StructureLayer, position);
                if (tileData is TotemData) {
                    GameObject gameObject = GameObject.Instantiate(totemToPrefab[(TotemData)tileData]);
                    Totem totem = gameObject.GetComponent<Totem>();

                    totem.Initialize(radiusRenderer);
                    gameObject.transform.position = position + TILEMAP_OFFSET;
                }
            }
        }
    }
}
