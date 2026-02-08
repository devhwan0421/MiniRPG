using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers _instance;
    public static Managers Instance { get { Init(); return _instance; } }

    private NetworkManager _networkManager;
    public static NetworkManager Network => Instance._networkManager;

    //private ObjectManager _objectManager;
    private ObjectManager _objectManager;
    public static ObjectManager Object => Instance._objectManager;

    private MapManager _mapManager;
    public static MapManager Map => Instance._mapManager;

    private CharacterData _characterData;
    public static CharacterData CharacterData => Instance._characterData;

    private MonsterData _monsterData;
    public static MonsterData MonsterData => Instance._monsterData;

    private ResourceManager _resourceManager;
    public static ResourceManager Resource => Instance._resourceManager;

    private PoolManager _poolManager;
    public static PoolManager Pool => Instance._poolManager;

    private UiManager _uiManager;
    public static UiManager Ui => Instance._uiManager;

    private void Awake()
    {
        Init();
    }

    private static void Init()
    {
        if(_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }
            DontDestroyOnLoad(go);
            _instance = go.GetComponent<Managers>();

            if (_instance._networkManager == null)
            {
                _instance._networkManager = NetworkManager.Instance;
            }

            if (_instance._objectManager == null)
            {
                _instance._objectManager = ObjectManager.Instance;
            }

            if (_instance._mapManager == null)
            {
                _instance._mapManager = MapManager.Instance;
            }

            /*if (_instance._mapManager == null)
            {
                //_instance._mapManager = MapManager.Instance;
                _instance._mapManager = go.AddComponent<MapManager>();
            }*/

            if (_instance._characterData == null)
            {
                _instance._characterData = go.AddComponent<CharacterData>();
                _instance._characterData.Init();
            }

            if (_instance._monsterData == null)
            {
                _instance._monsterData = go.AddComponent<MonsterData>();
                _instance._monsterData.Init();
            }

            if (_instance._resourceManager == null)
            {
                _instance._resourceManager = ResourceManager.Instance;
            }

            if(_instance._poolManager == null)
            {
                _instance._poolManager = PoolManager.Instance;
            }

            if (_instance._uiManager == null)
            {
                _instance._uiManager = UiManager.Instance;
            }
        }
    }
}
