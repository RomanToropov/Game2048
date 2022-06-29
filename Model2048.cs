using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Game2048
{
    // Направления сдвига ячеек
    public enum DirectionEnum
    {
        
        None,
        
        Left,
        
        Right,
        
        Up,
        
        Down
    }

    
    public class Model2048 : OnPropertyChangedClass
    {
        //Неизменяемая матрица клеток
        readonly ImmutableArray<ImmutableArray<Cell>> cells;

        // конструктор без параметров
        public Model2048()
        {
            ImmutableArray<Cell>[] _cells = new ImmutableArray<Cell>[4];
            for (int row = 0; row < 4; row++)
            {
                Cell[] rowCells = new Cell[4];
                for (int col = 0; col < 4; col++)
                    rowCells[col] = new Cell(row, col);
                _cells[row] = rowCells.ToImmutableArray();
            }
            cells = _cells.ToImmutableArray();
        }

        // Возвращает матрицу клеток
        // Неизменяемая матрица
        public ImmutableArray<ImmutableArray<Cell>> GetCells() => cells;

        // Статический
        static readonly Random rnd = new Random();
        private bool _isGameOver = false;
        

        //Игра закончена
        public bool IsGameOver { get => _isGameOver; private set { _isGameOver = value; OnPropertyChanged(); } }
       

        //Метод установки в пустой клетки значения 4 или 2
        private void SetRandom_2_4()
        {
            if (rnd.NextDouble() > 0.1)
                SetRandomValue(CellValueEnum.One);
            else
                SetRandomValue(CellValueEnum.Two);
        }

        //Установка заданного значения в случайную пустую клетку
        /// <param name="value">Заданное значение
        private void SetRandomValue(CellValueEnum value)
        {
            if (IsGameOver || value == CellValueEnum.None)
                return;

            int maxValue = 0;
            int sumValue = 0;

            /// Получение списка всех пустых клеток
            List<Cell> emptyCells = new List<Cell>();
            for (int row = 0; row < 4; row++)
                for (int col = 0; col < 4; col++)
                    if (cells[row][col].Value == CellValueEnum.None)
                    {
                        emptyCells.Add(cells[row][col]);

                    }
                    else
                    {
                        int val = (int)cells[row][col].Value;
                        sumValue += val;
                        if (maxValue < val)
                            maxValue = val;
                    }

            // Установка значения количество пустых ячеек
           int countEmptyCell = emptyCells.Count;

            // Если пустых ячеек нет, то конец игры
            if (countEmptyCell == 0)
                IsGameOver = true;
            else
            {
                // Выбор случайной пустой клетки и установка её свойств
                int randIndex = rnd.Next(emptyCells.Count);
                emptyCells[randIndex].Value = value;
                emptyCells[randIndex].IsNewValue = true;

                countEmptyCell--; // Уменьшение количества пустых ячеек

               
            }

            
        }

        /// <summary>Обработка начала шага - сброс всех IsCalculated и IsNewValue</summary>
        private void BeginStep()
        {
            for (int row = 0; row < 4; row++)
                for (int col = 0; col < 4; col++)
                {
                    cells[row][col].IsCalculated = false;
                    cells[row][col].IsNewValue = false;
                }
        }

        /// <summary>Сдвиг значения ячейки</summary>
        /// <param name="row">Строка ячейки</param>
        /// <param name="column">Колонка ячейки</param>
        /// <param name="direction">Направление сдвига</param>
        private void MoveCell(int row, int column, DirectionEnum direction)
        {
            if (row < 0 || row > 3 || column < 0 || column > 3)
                throw new ArgumentOutOfRangeException("Метод \"void Left(int row, int column)\"\r\nОдин из индексов вне диапазона");
            Cell currCell = cells[row][column];
            int dirRow = 0, dirCol = 0;
            switch (direction)
            {
                case DirectionEnum.Left: dirCol = -1; break;
                case DirectionEnum.Right: dirCol = +1; break;
                case DirectionEnum.Up: dirRow = -1; break;
                case DirectionEnum.Down: dirRow = +1; break;
                default: return;
            }
            while
                (
                    currCell != null
                    && currCell.Row + dirRow >= 0 && currCell.Row + dirRow <= 3
                    && currCell.Column + dirCol >= 0 && currCell.Column + dirCol <= 3
                )
                currCell = currCell.MoveTo(cells[currCell.Row + dirRow][currCell.Column + dirCol]);
        }

        /// <summary>Сдвиг всех значений вверх</summary>
        private void Up()
        {
            for (int row = 1; row < 4; row++)
                for (int col = 0; col < 4; col++)
                    MoveCell(row, col, DirectionEnum.Up);
        }

        /// <summary>Сдвиг всех значений вниз</summary>
        private void Down()
        {
            for (int row = 2; row > -1; row--)
                for (int col = 0; col < 4; col++)
                    MoveCell(row, col, DirectionEnum.Down);
        }

        /// <summary>Сдвиг всех значений влево</summary>
        private void Left()
        {
            for (int col = 1; col < 4; col++)
                for (int row = 0; row < 4; row++)
                    MoveCell(row, col, DirectionEnum.Left);
        }

        /// <summary>Сдвиг всех значений вправо</summary>
        private void Right()
        {
            for (int col = 2; col > -1; col--)
                for (int row = 0; row < 4; row++)
                    MoveCell(row, col, DirectionEnum.Right);
        }

        /// <summary>Очередной шаг с заданным направлением</summary>
        /// <param name="direction"></param>
        public void Step(DirectionEnum direction)
        {
            BeginStep();
            switch (direction)
            {
                case DirectionEnum.Down: Down(); break;
                case DirectionEnum.Left: Left(); break;
                case DirectionEnum.Right: Right(); break;
                case DirectionEnum.Up: Up(); break;
            }
            SetRandom_2_4();
        }
    }
}
