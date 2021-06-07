using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MazeCreator : MonoBehaviour
{
    public GameObject obj_wall;
    class FieldCreater
    {
        int _mazeX;  //���H���ォ�猩�����̉����A�T�ȏ�̊
        int _mazeY;  //���H���ォ�猩�����̏c���A�T�ȏ�̊
        int[,] _maze;

        public int getMazeX  // getter
        {
            get { return _mazeX; }
        }
        public int getMazeY  // getter
        {
            get { return _mazeY; }
        }

        // �ʘH�E��
        const int path = 1;
        const int wall = 0;

        // ���H�̒��ł̂P�}�X�i_maze���ł̈ʒu���j
        private class Square
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        // ����
        private enum Direction
        {
            Front = 0,
            Right = 1,
            Back = 2,
            Left = 3
        }

        // ���@��J�n�����W
        private List<Square> StartSquare;


        //�i�����@��O�́j�t�B�[���h�𐶐�
        public void Create(int mazeX, int mazeY)
        {
            _mazeX = mazeX;
            _mazeY = mazeY;
            _maze = new int[mazeY, mazeX]; //�ŏ����c���̑傫���A�Q�Ԗڂ������̑傫��
            StartSquare = new List<Square>();
        }


        public int[,] CreateMaze()
        {
            // �S�Ă�ǂŖ��߂�
            // ���@��J�n���(x,y�Ƃ��ɋ���)���W��ێ����Ă���
            for (int y = 0; y < _mazeY; y++)
            {
                for (int x = 0; x < _mazeX; x++)
                {
                    if (x == 0 || y == 0 || x == _mazeX - 1 || y == _mazeY - 1)
                    {
                        _maze[x, y] = path;  // �O�ǂ͔���̈גʘH�ɂ��Ă���(�Ō�ɖ߂�)
                    }
                    else
                    {
                        _maze[x, y] = wall;
                    }
                }
            }

            // ���@��J�n
            Dig(1, 1);

            // �O�ǂ�߂�
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


        // ���@��J�n�ʒu�������_���Ɏ擾����
        private Square GetStartSquare()
        {
            // ���@���₪�����Ƃ�
            if (StartSquare.Count == 0) return null;

            // �����_���ɊJ�n���W���擾����
            var rand = new System.Random();
            var index = rand.Next(StartSquare.Count);
            var square = StartSquare[index];
            StartSquare.RemoveAt(index);

            return square;
        }


        // ���W(x, y)�Ɍ����@��
        public void Dig(int x, int y)
        {
            // �w����W����@��Ȃ��Ȃ�܂Ŗx�葱����
            var rand = new System.Random();
            while (true)
            {
                // �@��i�߂邱�Ƃ��ł�������̃��X�g���쐬
                var directions = new List<Direction>();
                if (_maze[x, y - 1] == wall && _maze[x, y - 2] == wall)
                    directions.Add(Direction.Front);
                if (_maze[x + 1, y] == wall && _maze[x + 2, y] == wall)
                    directions.Add(Direction.Right);
                if (_maze[x, y + 1] == wall && _maze[x, y + 2] == wall)
                    directions.Add(Direction.Back);
                if (_maze[x - 1, y] == wall && _maze[x - 2, y] == wall)
                    directions.Add(Direction.Left);

                // �@��i�߂��Ȃ��ꍇ�A���[�v�𔲂���
                if (directions.Count == 0) break;

                // �w����W��ʘH�Ƃ����@������W����폜
                SetPath(x, y);

                // �@��i�߂���ꍇ�̓����_���ɕ��������߂Č@��i�߂�
                var dirIndex = rand.Next(directions.Count);

                // ���܂��������ɐ�2�}�X����ʘH�Ƃ���
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

            // �ǂ��ɂ��@��i�߂��Ȃ��ꍇ�A���@��J�n�����W����@��Ȃ���
            // �����W�����݂��Ȃ��Ƃ��A���@�芮��
            var square = GetStartSquare();
            if (square != null)
            {
                Dig(square.X, square.Y);
            }
        }


        // �Z���̒��𓹂ɂ���(���@��J�n�����W�̂Ƃ��ێ�)
        private void SetPath(int x, int y)
        {
            _maze[x, y] = path;

            // x,y�Ƃ��Ɋ�}�X�͌��@��J�n���ɂȂ肦��
            if (x % 2 == 1 && y % 2 == 1)
            {
                // ���@������W
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

        //object���=�O�ɔz�u
        for (int y = 0; y < fieldCreater.getMazeY; y++) //�t�B�[���h�̏c���̕��������[�v����B
        {
            for (int x = 0; x < fieldCreater.getMazeX; x++) //�t�B�[���h�̉����̕��������[�v����B
            {
                if (maze[x, y] == 1) //�ʘH�Ȃ�
                {
                    //�����z�u���Ȃ�
                }
                else if (maze[x, y] == 0) //�ǂȂ�
                {
                    Instantiate(obj_wall, new Vector3(5.0f * x, 5.0f, 5.0f * y), Quaternion.identity); //�ǂ�z�u����B
                }
            }
        }

    }


}



    
