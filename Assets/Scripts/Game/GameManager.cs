using System;
using System.Collections;
using System.Collections.Generic;
//using OpenCover.Framework.Model;
using TMPro;
//using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject carPrefab;
    [SerializeField] private GameObject roadPrefab;
    
    [SerializeField] private MoveButton leftMoveButton;
    [SerializeField] private MoveButton rightMoveButton;
    
    [SerializeField] private TMP_Text gasText;
    [SerializeField] private GameObject startPanelPrefab;
    [SerializeField] private GameObject endPanelPrefab;
    [SerializeField] private Transform CanvasTransform;

    
    
    private CarController _carController;
    
    Queue<GameObject> _roadPool = new Queue<GameObject>();
    int _roadPoolSize = 3;

    private List<GameObject> _activeRoads = new List<GameObject>();
    
    private int _roadIndex = 0;
    
    public enum State
    {
        Start, Play, End
    }
    public State GameState { get; private set; } = State.Start;


    static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    
    void Start()
    {
        InitializeRoadPool();
        
        GameState = State.Start;
        
        ShowStartPanel();
    }

    private void Update()
    {
        foreach (var activeRoad in _activeRoads)
        {
            activeRoad.transform.Translate(-Vector3.forward * Time.deltaTime);
        }

        if (_carController)
        {
            gasText.text = _carController.Gas.ToString();

        } 
    }

    private void StartGame()
    {
        _roadIndex = 0;
        SpawnRoad(Vector3.zero);
        
        _carController = Instantiate(carPrefab, new Vector3(0,0,-3f), Quaternion.identity)
            .GetComponent<CarController>();

        leftMoveButton.OnMoveButtonDown += () => _carController.Move(-1f);
        rightMoveButton.OnMoveButtonDown += () => _carController.Move(1f);
        
        GameState = State.Play;
        InitializeRoadPool();
    }

    public void EndGame()
    {
        GameState = State.End;
        
        Destroy(_carController.gameObject);

        foreach (var activeRoad in _activeRoads)
        {
            activeRoad.SetActive(false);
        }
        
        ShowEndPanel();
    }

    #region UI

    /// <summary>
    /// 시작 화면을 표시
    /// </summary>
    
    private void ShowStartPanel()
    {
        StartPanelController startPanelController = Instantiate(startPanelPrefab, CanvasTransform)
            .GetComponent<StartPanelController>();
        startPanelController.OnStartButtonClick += () =>
        {
            StartGame();
            Destroy(startPanelController.gameObject);
        };
    }

    private void ShowEndPanel()
    {
        StartPanelController endPanelController = Instantiate(endPanelPrefab, CanvasTransform)
            .GetComponent<StartPanelController>();
        endPanelController.OnStartButtonClick += () =>
        {
            Destroy(endPanelController.gameObject);
            ShowStartPanel();
        };
    }
    

    #endregion
    
    #region 도로 생성 및 관리

    /// <summary>
    /// 도로 오브젝트 풀 초기화
    /// </summary>
    
    void InitializeRoadPool()
    {
        for (int i = 0; i < _roadPoolSize; i++)
        {
            GameObject road = Instantiate(roadPrefab);
            road.SetActive(false);
            _roadPool.Enqueue(road);
        }
    }
    
    /// <summary>
    /// 도로 오브젝트 풀에서 불러와 배치하는 함수
    /// </summary>
    
    public void SpawnRoad(Vector3 position)
    {
        GameObject road;
        
        if (_roadPool.Count > 0)
        {
            road = _roadPool.Dequeue();
            road.transform.position = position;
            road.SetActive(true);
        }
        else
        {
            road = Instantiate(roadPrefab, position, Quaternion.identity);
        }

        if (_roadIndex > 0 && _roadIndex % 2 == 0)
        {
            road.GetComponent<RoadController>().SpawnGas();
        }
        
        _activeRoads.Add(road);
        _roadIndex++;
    }

    public void DestroyRoad(GameObject road)
    {
        road.SetActive(false);
        _activeRoads.Remove(road);
        _roadPool.Enqueue(road);
    }
    
    #endregion
    
}
