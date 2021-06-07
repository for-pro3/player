using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MazeCreator : MonoBehaviour
{
    public GameObject obj_wall;
    class FieldCreater
    {
        int _mazeX;  //迷路を上から見た時の横軸、５以上の奇数
        int _mazeY;  //迷路を上から見た時の縦軸、５以上の奇数
        int[,] _maze;

        public int getMazeX  // getter
        {
            get { return _mazeX; }
        }
        public int getMazeY  // getter
        {
            get { return _mazeY; }
        }

        // 通路・壁
        const int path = 1;
        const int wall = 0;

        // 迷路の中での１マス（_maze内での位置情報）
        private class Square
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        // 方向
        private enum Direction
        {
            Front = 0,
            Right = 1,
            Back = 2,
            Left = 3
        }

        // 穴掘り開始候補座標
        private List<Square> StartSquare;


        //（穴を掘る前の）フィールドを生成
        public void Create(int mazeX, int mazeY)
        {
            _mazeX = mazeX;
            _mazeY = mazeY;
            _maze = new int[mazeY, mazeX]; //最初が縦軸の大きさ、２番目が横軸の大きさ
            StartSquare = new List<Square>();
        }


        public int[,] CreateMaze()
        {
            // 全てを壁で埋める
            // 穴掘り開始候補(x,yともに偶数)座標を保持しておく
            for (int y = 0; y < _mazeY; y++)
            {
                for (int x = 0; x < _mazeX; x++)
                {
                    if (x == 0 || y == 0 || x == _mazeX - 1 || y == _mazeY - 1)
                    {
                        _maze[x, y] = path;  // 外壁は判定の為通路にしておく(最後に戻す)
                    }
                    else
                    {
                        _maze[x, y] = wall;
                    }
                }
            }

            // 穴掘り開始
            Dig(1, 1);

            // 外壁を戻す
            for (int y = 0; y < _mazeY; y++)
            {
                for (int x = 0; x < _mazeX; x++)
                {
                    if (x == 0 || y == 0 || x == _mazeX - 1 || y == _mazeY - 1)
                    {
                        _maze[x, y] = wall;
                    }
                }
            }
            return _maze;
        }


        // 穴掘り開始位置をランダムに取得する
        private Square GetStartSquare()
        {
            // 穴掘り候補が無いとき
            if (StartSquare.Count == 0) return null;

            // ランダムに開始座標を取得する
            var rand = new System.Random();
            var index = rand.Next(StartSquare.Count);
            var square = StartSquare[index];
            StartSquare.RemoveAt(index);

            return square;
        }


        // 座標(x, y)に穴を掘る
        public void Dig(int x, int y)
        {
            // 指定座標から掘れなくなるまで堀り続ける
            var rand = new System.Random();
            while (true)
            {
                // 掘り進めることができる方向のリストを作成
                var directions = new List<Direction>();
                if (_maze[x, y - 1] == wall && _maze[x, y - 2] == wall)
                    directions.Add(Direction.Front);
                if (_maze[x + 1, y] == wall && _maze[x + 2, y] == wall)
                    directions.Add(Direction.Right);
                if (_maze[x, y + 1] == wall && _maze[x, y + 2] == wall)
                    directions.Add(Direction.Back);
                if (_maze[x - 1, y] == wall && _maze[x - 2, y] == wall)
                    directions.Add(Direction.Left);

                // 掘り進められない場合、ループを抜ける
                if (directions.Count == 0) break;

                // 指定座標を通路とし穴掘り候補座標から削除
                SetPath(x, y);

                // 掘り進められる場合はランダムに方向を決めて掘り進める
                var dirIndex = rand.Next(directions.Count);

                // 決まった方向に先2マス分を通路とする
                switch (directions[dirIndex])
                {
                    case Direction.Front:
                        SetPath(x, --y);
                        SetPath(x, --y);
                        break;
                    case Direction.Right:
                        SetPath(++x, y);
                        SetPath(++x, y);
                        break;
                    case Direction.Back:
                        SetPath(x, ++y);
                        SetPath(x, ++y);
                        break;
                    case Direction.Left:
                        SetPath(--x, y);
                        SetPath(--x, y);
                        break;
                }
            }

            // どこにも掘り進められない場合、穴掘り開始候補座標から掘りなおし
            // 候補座標が存在しないとき、穴掘り完了
            var square = GetStartSquare();
            if (square != null)
            {
                Dig(square.X, square.Y);
            }
        }


        // セルの中を道にする(穴掘り開始候補座標のとき保持)
        private void SetPath(int x, int y)
        {
            _maze[x, y] = path;

            // x,yともに奇数マスは穴掘り開始候補になりえる
            if (x % 2 == 1 && y % 2 == 1)
            {
                // 穴掘り候補座標
                StartSquare.Add(new Square() { X = x, Y = y });
            }
        }
    }

    
    void Start()
    {
        FieldCreater fieldCreater = new FieldCreater();
        fieldCreater.Create(21, 21);
        //fieldCreater.Dig(1, 1);
        int[,] maze = fieldCreater.CreateMaze();

        //objectを壁=０に配置
        for (int y = 0; y < fieldCreater.getMazeY; y++) //フィールドの縦幅の分だけループする。
        {
            for (int x = 0; x < fieldCreater.getMazeX; x++) //フィールドの横幅の分だけループする。
            {
                if (maze[x, y] == 1) //通路なら
                {
                    //何も配置しない
                }
                else if (maze[x, y] == 0) //壁なら
                {
                    Instantiate(obj_wall, new Vector3(5.0f * x, 5.0f, 5.0f * y), Quaternion.identity); //壁を配置する。
                }
            }
        }

    }


}



    
