using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TileStatus
{
    X = -1,
    None = 0, 
    O = 1
}

public class Board : MonoBehaviour
{
    public const int SIZEX = 3;
    public const int SIZEY = 3;
    public GameObject[] goTiles = null;
    public TileStatus[,] tiles = new TileStatus[SIZEX, SIZEY];
    private static Board instance = null;
    private TileStatus currentPlayer = TileStatus.X;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < goTiles.Length; i++)
        {
            int temp = i;
            goTiles[i].GetComponent<Button>()?.onClick.AddListener(() => PickTile(new Vector2Int(temp % SIZEX, Mathf.FloorToInt(temp / SIZEY))));
        }
    }

    private GameObject GetTileGO(Vector2Int position) => goTiles[position.x + position.y * 3];

    private void Reset()
    {
        currentPlayer = TileStatus.X;
        for (int i = 0; i < goTiles.Length; i++)
        {
            goTiles[i].GetComponentInChildren<Text>().text = string.Empty;
        }

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                tiles[x, y] = TileStatus.None;
            }
        }
    }

    public void PickTile(Vector2Int position)
    {
        if(SetTile(position) == true)
        {
            if(CheckIfWon(tiles) != TileStatus.None)
            {
                Debug.Log(currentPlayer.ToString() + " WON!");
                Reset();
                return;
            }
            else if(GetOpenTiles(tiles) <= 0)
            {
                Debug.Log("DRAW!");
                Reset();
                return;
            }
            currentPlayer = currentPlayer == TileStatus.X ? TileStatus.O : TileStatus.X;

            if(SetTile(AIMove()))
            {
                if(CheckIfWon(tiles) != TileStatus.None)
                {
                    Debug.Log(currentPlayer.ToString() + " WON!");
                    Reset();
                    return;
                }
                currentPlayer = currentPlayer == TileStatus.X ? TileStatus.O : TileStatus.X;
            }
        }
    }

    private bool SetTile(Vector2Int position)
    {
        TileStatus tile = tiles[position.x, position.y];
        if(tile != TileStatus.None)
        {
            Debug.Log("This tile is already taken.");
            return false;
        }

        tile = currentPlayer;
        tiles[position.x, position.y] = tile;
        GetTileGO(position).GetComponentInChildren<Text>().text = tile.ToString();
        return true;
    }

    private Vector2Int AIMove()
    {
        Vector2Int move = new Vector2Int(-1, -1);
        int score = -2;

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if(tiles[x, y] == TileStatus.None)
                {
                    TileStatus[,] newBoard = (TileStatus[,]) tiles.Clone();
                    newBoard[x, y] = currentPlayer;
                    int scoreForTheMove = -MiniMax(newBoard, (TileStatus)((int)currentPlayer * -1));
                    if(scoreForTheMove > score)
                    {
                        score = scoreForTheMove;
                        move = new Vector2Int(x, y);
                    }
                }
            }
        }

        return move;
    }

    private TileStatus CheckIfWon(TileStatus[,] tiles)
    {
        // Check horizontal
        for (int y = 0; y < 3; y++)
        {
            for (int x = 1; x < 3; x++)
            {
                if(tiles[x, y] != tiles[x - 1, y])
                {
                    break;
                }
                if(tiles[x, y] != TileStatus.None && x == 2)
                {
                    return tiles[x, y];
                }
            }
        }

        // Check vertical
        for (int x = 0; x < 3; x++)
        {
            for (int y = 1; y < 3; y++)
            {
                if(tiles[x, y] != tiles[x, y - 1])
                {
                    break;
                }
                if(tiles[x, y] != TileStatus.None && y == 2)
                {
                    return tiles[x, y];
                }   
            }     
        }

        // Check diagonal right
        for (int i = 1; i < 3; i++)
        {
            if(tiles[i, i] != tiles[i - 1, i - 1])
            {
                break;
            }
            if(tiles[i, i] != TileStatus.None && i == 2)
            {
                return tiles[i, i];
            }                
        }

        // Check diagonal left
        for (int i = 1; i < 3; i++)
        {
            if(tiles[2 - i, i] != tiles[3 - i, i - 1])
            {
                break;
            }
            if(tiles[2 - i, i] != TileStatus.None && i == 2)
            {
                return tiles[2 - i, i];
            }
        }

        return TileStatus.None;
    }

    public int MiniMax(TileStatus[,] board, TileStatus currentPlayer)
    {
        if(CheckIfWon(board) != TileStatus.None)
        {
            return (int)currentPlayer * (int)CheckIfWon(board);
        }

        Vector2Int move = new Vector2Int(-1, -1);
        int score = -2;

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if(board[x, y] == TileStatus.None)
                {
                    TileStatus[,] newBoard = (TileStatus[,]) board.Clone();
                    newBoard[x, y] = currentPlayer;
                    int scoreForTheMove = -MiniMax(newBoard, (TileStatus)((int)currentPlayer * -1));
                    if(scoreForTheMove > score)
                    {
                        score = scoreForTheMove;
                        move = new Vector2Int(x, y);
                    }
                }
            }
        }

        if(move == new Vector2Int(-1, -1))
        {
            return 0;
        }

        return score;
    }

    private int GetOpenTiles(TileStatus[,] board)
    {
        int count = 0;
        foreach (TileStatus status in board)
        {
            if(status == TileStatus.None)
            {
                count++;
            }
        }
        return count;
    }
}