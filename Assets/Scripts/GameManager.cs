using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 spawnPos;
    private Vector2 mousePos;
    [SerializeField] private Vector2 dragOffset = new Vector2(-0.5f,2);
    private Dice selectedDice;
    private Dice selectableDice;
    private Node closestNode;
    private GameState _state;

    [SerializeField] private Node nodePrefab;
    [SerializeField] private Dice dicePrefab;
    [SerializeField] private GameObject spawnAreaPrefab;
    [SerializeField] private Transform nodeParent;
    [SerializeField] private Transform diceParent;
    [SerializeField] private List<DiceType> diceTypes = new List<DiceType>();

    private List<Node> nodes = new List<Node>();
    private List<Dice> dices = new List<Dice>();
    private List<Node> indicatedNodes = new List<Node>();
    private void Start()
    {
        mainCam = Camera.main;
        ChangeState(GameState.GenerateGame);
    }
    private void Update()
    {
        if (_state != GameState.WaitingInput) return;

        if (Input.GetMouseButtonDown(0))
        {
            // select dice
            mousePos = Input.mousePosition;
            Vector2 pos = mainCam.WorldToScreenPoint(selectableDice.Pos);

            if ((mousePos - pos).sqrMagnitude < 30000f)
            {
                selectedDice = selectableDice;
            }
            else
            {
                selectedDice = null;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            // select dice
            if (selectedDice == null)
            {
                mousePos = Input.mousePosition;
                Vector2 pos = mainCam.WorldToScreenPoint(selectableDice.Pos);

                if ((mousePos - pos).sqrMagnitude < 30000f)
                {
                    selectedDice = selectableDice;
                }
                else
                {
                    selectedDice = null;
                }
            }
            if (selectedDice == null) return;

            // drag dice
            mousePos = Input.mousePosition;
            Vector2 mouseWorldPos = mainCam.ScreenToWorldPoint(mousePos);
            selectedDice.transform.position =  mouseWorldPos + dragOffset;

            // find closest node
            closestNode = nodes.OrderBy(n => (n.Pos - selectedDice.Pos).sqrMagnitude).First();

            // indicate closest node
            if (!closestNode.isIndicated && isCloseToBoard(selectedDice))
            {
                foreach(Node node in indicatedNodes)
                {
                    node.ChangeAlpha(0.1f,false);
                }
                indicatedNodes.Clear();

                closestNode.ChangeAlpha(0.3f,true);
                indicatedNodes.Add(closestNode);
            }
            else if(closestNode.isIndicated && !isCloseToBoard(selectedDice))
            {
                closestNode.ChangeAlpha(0.1f,false);
            }
        }
        else if (Input.GetMouseButtonUp(0) && selectedDice != null)
        {
            if(closestNode != null & closestNode.isIndicated)
            {
                selectedDice._transform.position = closestNode.Pos;
            }
            selectedDice = null;
        }
    }
    private void ChangeState(GameState newState)
    {
        _state = newState;
        switch (newState)
        {
            case GameState.GenerateGame:
                GenerateGrid();
                CreateSpawnArea();
                break;
            case GameState.SpawnDice:
                SpawnRandomDice();
                break;
            case GameState.WaitingInput:
                break;
            default:
                break;
        }
    }
    private void GenerateGrid()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                var node = Instantiate(nodePrefab, new Vector2(i, j), Quaternion.identity, nodeParent);
                nodes.Add(node);
            }
        }
    }
    private void CreateSpawnArea()
    {
        spawnPos = Instantiate(spawnAreaPrefab, new Vector2(2, -2), Quaternion.identity).transform.position;
        ChangeState(GameState.SpawnDice);
    }

    private void SpawnRandomDice()
    {
        var dice = Instantiate(dicePrefab, spawnPos, Quaternion.identity, diceParent);
        dice.Init(GetRandomDiceType());
        dices.Add(dice);
        selectableDice = dice;
        ChangeState(GameState.WaitingInput);
    }

    private DiceType GetRandomDiceType()
    {
        return diceTypes[UnityEngine.Random.Range(0, diceTypes.Count)];
    }

    private bool isCloseToBoard(Dice dice)
    {
        if (dice.Pos.x > -0.5f && dice.Pos.x < 4.5f && dice.Pos.y > -0.5f && dice.Pos.y < 4.5f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
public enum GameState
{
    GenerateGame,
    SpawnDice,
    WaitingInput,
}
[Serializable]
public struct DiceType
{
    public int value;
    public Vector3 rotation;
    public Texture2D texture;
}