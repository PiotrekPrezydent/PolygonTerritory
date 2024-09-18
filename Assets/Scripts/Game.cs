using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class Game : MonoBehaviour
{
    /*
    * 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 
    * 0 Y 0 0 0 0 0 0 0 0 0 0 0 0 B 0 
    * 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 
    * 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 
    * 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 
    * 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 
    * 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 
    * 0 R 0 0 0 0 0 0 0 0 0 0 0 0 G 0 
    * 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 
    */
    public static Territory[,] Matrix = new Territory[9, 16];

    [SerializeField]
    Transform _territoriesContainer;

    [SerializeField]
    GameObject _territoryPrefab;

    public static Player CurrentPlayerTurn;

    public static bool TurnLock;
    private void Awake()
    {
        CurrentPlayerTurn = Player.None;
        TurnLock = false;
        Initialize();
        NextTurn();
    }

    public void Initialize()
    {
        for(int i = 0; i < Matrix.GetLength(0); i++)
        {
            for(int j = 0; j < Matrix.GetLength(1);j++)
            {
                Matrix[i, j] = Instantiate(_territoryPrefab, new Vector3(j, i * -1, 0), transform.rotation, _territoriesContainer).GetComponent<Territory>();

                if (i == 1 && j == 1)
                    Matrix[i, j].Initialize(Player.Yellow, 1);

                else if (i == 1 && j == 14)
                    Matrix[i, j].Initialize(Player.Blue, 1);

                else if (i == 7 && j == 1)
                    Matrix[i, j].Initialize(Player.Red, 1);

                else if (i == 7 && j == 14)
                    Matrix[i, j].Initialize(Player.Green, 1);

                else
                    Matrix[i, j].Initialize(Player.None, 0);
            }
        }
        Territory.Instance = this;
    }

    public void NextTurn()
    {
        TurnLock = true;
        StartCoroutine(ExplodeShapesAsLongAsNeeded());
    }

    public void OnTurnEnded()
    {
        TurnLock = false;

        if(CurrentPlayerTurn != Player.Red)
        {
            List<int[]> botBlocks = new List<int[]>();
            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    if (Matrix[i, j].Player == CurrentPlayerTurn)
                        botBlocks.Add(new int[] { i,j});
                }
            }

            if(botBlocks.Count == 0)
            {
                NextTurn();
                return;
            }

            int ran = Random.Range(0, botBlocks.Count);

            Matrix[botBlocks[ran][0], botBlocks[ran][1]].Points++;
            NextTurn();
        }
        if(CurrentPlayerTurn == Player.Red)
        {
            int rp = 0;
            int gp = 0;
            int bp = 0;
            int yp = 0;
            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    if (Matrix[i, j].Player == Player.Red)
                        rp+= Matrix[i,j].Points;
                    if (Matrix[i, j].Player == Player.Green)
                        gp += Matrix[i,j].Points;
                    if (Matrix[i, j].Player == Player.Blue)
                        bp += Matrix[i,j].Points;
                    if (Matrix[i, j].Player == Player.Yellow)
                        yp += Matrix[i,j].Points;
                }
            }
            Debug.Log("points: (red,green,blue,yeallow) " + rp + " " + gp + " " + bp + " " + yp);
        }
    }

    IEnumerator ExplodeShapesAsLongAsNeeded()
    {
        bool flag = false;
        do
        {
            flag = false;
            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    while (Matrix[i, j].IsExploding)
                        yield return null;

                    if (Matrix[i, j].Points >= 4)
                    {
                        flag = true;
                        Matrix[i, j].Explode();
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
        } while (flag);

        yield return new WaitForSeconds(0.25f);

        CurrentPlayerTurn++;
        if (CurrentPlayerTurn >= Player.None)
            CurrentPlayerTurn = Player.Red;
        OnTurnEnded();
    }
}
