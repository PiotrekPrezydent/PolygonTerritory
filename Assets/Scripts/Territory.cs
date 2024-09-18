using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Territory : MonoBehaviour
{
    public int Points
    {
        get => _points;
        set
        {
            _points = value;
            _pointsText.text = _points.ToString();
        }
    }
    int _points;

    public Player Player
    {
        get => _player;
        set
        {
            switch (value)
            {
                case Player.None:
                    _sprite.color = new Color(0f, 0f, 0f, 0f);
                    break;
                case Player.Red:
                    _sprite.color = Color.red;
                    break;
                case Player.Green:
                    _sprite.color = Color.green;
                    break;
                case Player.Blue:
                    _sprite.color = Color.blue;
                    break;
                case Player.Yellow:
                    _sprite.color = Color.yellow;
                    break;
            }
            _player = value;
        }
    }
    Player _player;

    [SerializeField]
    Button _button;

    [SerializeField]
    TextMeshProUGUI _pointsText;

    [SerializeField]
    SpriteRenderer _sprite;

    public bool IsExploding;

    public static Game Instance;

    public void Initialize(Player player, int points)
    {
        Player = player;
        Points = points;
        _button.onClick.AddListener(OnClicked);
        IsExploding = false;
    }

    public void SetValues(Player player, int points)
    {
        Player = player;
        Points = points;
    }

    public void Explode()
    {
        if (IsExploding)
            return;
        StartCoroutine(ExplodeSlowly());
    }

    IEnumerator ExplodeSlowly()
    {
        IsExploding = true;
        int i = (int)transform.position.y * -1;
        int j = (int)transform.position.x;
        SetValues(Player.None, 0);
        //Up
        if (i - 1 < 0)
            SetValues(Game.CurrentPlayerTurn, Game.Matrix[i, j].Points + 1);
        else
            Game.Matrix[i - 1, j].SetValues(Game.CurrentPlayerTurn, Game.Matrix[i - 1, j].Points + 1);

        //Right
        if (j + 1 >= Game.Matrix.GetLength(1))
            SetValues(Game.CurrentPlayerTurn, Game.Matrix[i, j].Points + 1);
        else
            Game.Matrix[i, j + 1].SetValues(Game.CurrentPlayerTurn, Game.Matrix[i, j + 1].Points + 1);

        //Down
        if (i + 1 >= Game.Matrix.GetLength(0))
            SetValues(Game.CurrentPlayerTurn, Game.Matrix[i, j].Points + 1);
        else
            Game.Matrix[i + 1, j].SetValues(Game.CurrentPlayerTurn, Game.Matrix[i + 1, j].Points + 1);

        //left
        if (j - 1 < 0)
            SetValues(Game.CurrentPlayerTurn, Game.Matrix[i, j].Points + 1);
        else
            Game.Matrix[i, j - 1].SetValues(Game.CurrentPlayerTurn, Game.Matrix[i, j - 1].Points + 1);

        yield return new WaitForSeconds(0.25f);
        IsExploding = false;
    }

    void OnClicked()
    {
        if (Player != Game.CurrentPlayerTurn || Game.TurnLock)
            return;

        Points++;
        Instance.NextTurn();
    }


}
