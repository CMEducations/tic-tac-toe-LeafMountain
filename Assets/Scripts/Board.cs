using UnityEngine;
using UnityEngine.UI;

public enum TileStatus
{
    None, X, O
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
        TileStatus tile = tiles[position.x, position.y];
        if(tile == TileStatus.None)
        {
            tile = currentPlayer;
            tiles[position.x, position.y] = tile;
            GetTileGO(position).GetComponentInChildren<Text>().text = tile.ToString();

            if(CheckIfWon(position) == true)
            {
                Debug.Log(currentPlayer.ToString() + " WON!");
                Reset();
                return;
            }

            currentPlayer = currentPlayer == TileStatus.X ? TileStatus.O : TileStatus.X;
            Debug.Log("Picked tile " + position);
        }
        else
        {
            Debug.Log("This tile is already taken.");
        }
    }

    private bool CheckIfWon(Vector2Int position)
    {
        // Check vertical
        for (int i = 0; i < 3; i++)
        {
            if(tiles[position.x, i] != currentPlayer)
            {
                break;
            }
            if(i == 2)
            {
                return true;
            }
        }

        // Check horizontal
        for (int i = 0; i < 3; i++)
        {
            if(tiles[i, position.y] != currentPlayer)
            {
                break;
            }
            if(i == 2)
            {
                return true;
            }        
        }

        // Check diagonal right
        if(position == new Vector2Int(1,1) || position == new Vector2Int(0,0) || position == new Vector2Int(2,2))
        {
            for (int i = 0; i < 3; i++)
            {
                if(tiles[i, i] != currentPlayer)
                {
                    break;
                }
                if(i == 2)
                {
                    return true;
                }                
            }
        }

        // Check diagonal left
        if(position == new Vector2Int(1,1) || position == new Vector2Int(0,2) || position == new Vector2Int(2,0))
        {
            for (int i = 0; i < 3; i++)
            {
                if(tiles[2 - i, i] != currentPlayer)
                {
                    break;
                }
                if(i == 2)
                {
                    return true;
                }
            }
        }

        return false;
    }
}