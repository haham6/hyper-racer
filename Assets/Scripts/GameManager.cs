using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject carPrefab;
    [SerializeField] private GameObject roadPrefab;
    
    [SerializeField] private MoveButton leftMoveButton;
    [SerializeField] private MoveButton rightMoveButton;
    
    Queue<GameObject> _roadPool = new Queue<GameObject>();
    int _roadPoolSize = 3;

    private List<GameObject> _activeRoads = new List<GameObject>();

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
        
        StartGame();
    }

    private void Update()
    {
        foreach (var activeRoad in _activeRoads)
        {
            activeRoad.transform.Translate(-Vector3.forward * Time.deltaTime);
        }
    }

    void StartGame()
    {
        SpawnRoad(Vector3.zero);
        
        var carController = Instantiate(carPrefab, new Vector3(0,0,-3f), Quaternion.identity)
            .GetComponent<CarController>();

        leftMoveButton.OnMoveButtonDown += () => carController.Move(-1f);
        rightMoveButton.OnMoveButtonDown += () => carController.Move(1f);
    }

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
        if (_roadPool.Count > 0)
        {
            GameObject road = _roadPool.Dequeue();
            road.transform.position = position;
            road.SetActive(true);
            
            _activeRoads.Add(road);
        }
        else
        {
            GameObject road = Instantiate(roadPrefab, position, Quaternion.identity);
            
            _activeRoads.Add(road);
        }
    }
    
    #endregion
    
}
