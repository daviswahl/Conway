using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conway
{
    class Program
    {
        static void Main(string[] args)
        {
            // "Unit Tests"
            TestGetCell();
            TestGetLivingNeighbourCount();
            TestBoardCompare();
            TestGameLogic();

            // "Integration Tests"
            TestGame();

            // These test cases come from the wiki
            TestBlinker();

            System.Console.ReadKey();
        }

        private static void TestGetCell()
        {
            var cells = new List<List<int>>
            {
                new List<int>{1, 1, 1},
                new List<int>{1, 1, 1},
                new List<int>{1, 1, 1},
            };
            var b = new Board(cells);

            var tests = from x in Enumerable.Range(0, 3)
                        from y in Enumerable.Range(0, 3)
                        select Tuple.Create(x, y);

            foreach(var test in tests)
            {
                AssertCellIsAlive(b, test.Item1, test.Item2);
            }

            AssertCellIsDead(b, 3, 3);
            AssertCellIsDead(b, -1, 2);
            AssertCellIsDead(b, 2, -1);
            AssertCellIsDead(b, 3, 2);
            AssertCellIsDead(b, 2, 3);
            Console.WriteLine("TestGetCell successful");
        }
        
        private static void AssertCellIsAlive(Board b, int x, int y)
        {
            if (b.GetCellAt(x, y).State != CellState.Alive)
            {
                throw new Exception($"Expected coordinate ({x}, {y}) to be alive");
            } 
        }

        private static void AssertCellIsDead(Board b, int x, int y)
        {
            if (b.GetCellAt(x, y).State != CellState.Dead)
            {
                throw new Exception($"Expected coordinate ({x}, {y}) to be dead");
            } 
        }


        private static void TestGetLivingNeighbourCount()
        {
            var cells = new List<List<int>>
            {
                new List<int>{0, 1, 0, 0, 1},
                new List<int>{0, 1, 1, 1, 0},
                new List<int>{0, 1, 1, 1, 0},
                new List<int>{0, 1, 1, 1, 0},
                new List<int>{1, 0, 0, 0, 0},
            };

            var b = new Board(cells);
            // y = 0
            AssertLivingNeighbourCountMatchesExpected(b, 0, 0, 2);
            AssertLivingNeighbourCountMatchesExpected(b, 1, 0, 2);
            AssertLivingNeighbourCountMatchesExpected(b, 4, 0, 1);

            // y = 1
            AssertLivingNeighbourCountMatchesExpected(b, 0, 1, 3);
            AssertLivingNeighbourCountMatchesExpected(b, 1, 1, 4);
            AssertLivingNeighbourCountMatchesExpected(b, 2, 1, 6);
            AssertLivingNeighbourCountMatchesExpected(b, 4, 1, 3);

            // y = 2
            AssertLivingNeighbourCountMatchesExpected(b, 2, 2, 8);

            // y = 4
            AssertLivingNeighbourCountMatchesExpected(b, 0, 4, 1);
            AssertLivingNeighbourCountMatchesExpected(b, 4, 4, 1);

            Console.WriteLine("TestGetNeighbourCellCount successful");
        }

        private static void AssertLivingNeighbourCountMatchesExpected(Board b, int x, int y, int expected)
        {
            var count = b.GetLivingNeighbourCountAt(x, y);
            if (count != expected)
            {
                throw new Exception($"Expected {expected} neighbo(u)rs at ({x}, {y}), got: {count}");
            }
        }

        // Comparison is only used in testing but I'm not up to speed on C#'s equality semantics
        // and I want to be comparisons work properly.
        private static void TestBoardCompare()
        {
           var cells = new List<List<int>>
           {
               new List<int> {1,1},
               new List<int> {1,0}
           };

           var cells2 = new List<List<int>>
           {
               new List<int> {1,1},
               new List<int> {1,0}
           };
        
           var cells3 = new List<List<int>>
           {
               new List<int> {1,1},
               new List<int> {0,1},
           };

            var b1 = new Board(cells);
            var b2 = new Board(cells2);
            var b3 = new Board(cells3);

            if (!b1.Compare(b2)) {
                throw new Exception("expected b1 to equal b2");
            }
            if (b1.Compare(b3))
            {
                throw new Exception("expected b1 not to equal b3");
            }
        }

        private static void TestGameLogic()
        {
            // Alive, N in {0,1,4,5,6,7,8} -> Dead
            foreach(int i in Enumerable.Range(0, 2).Union(Enumerable.Range(4, 5)))
            {
                AssertNextCellStateMatchesExpected(CellState.Alive, i, CellState.Dead);
            }

            AssertNextCellStateMatchesExpected(CellState.Alive, 2, CellState.Alive);
            AssertNextCellStateMatchesExpected(CellState.Alive, 3, CellState.Alive);

            // Dead, N = 3 -> Alive
            AssertNextCellStateMatchesExpected(CellState.Dead, 3, CellState.Alive);

            // Dead, N in {0,1,2,4,5,6,7,8} -> Dead
            foreach(int i in Enumerable.Range(0,3).Union(Enumerable.Range(4,5)))
            {
                AssertNextCellStateMatchesExpected(CellState.Dead, i, CellState.Dead);
            }

            Console.WriteLine("TestGameLogic successful");
        }

        private static void AssertNextCellStateMatchesExpected(CellState current, int neighbourCount, CellState expected)
        {
            var actual = GameLogic.NextCellState(current, neighbourCount);
            if (actual != expected)
            {
                throw new Exception($"Expected {expected} for current state: {current} and count: {neighbourCount}, got: {actual}");
            };
        }

        private static void TestGame()
        {
            var initialState = new List<List<int>>
            {
                new List<int>{0, 0, 0},
                new List<int>{0, 1, 1},
                new List<int>{0, 0, 1},
            };

            var nextState = new List<List<int>>
            {
                new List<int>{0, 0, 0},
                new List<int>{0, 1, 1},
                new List<int>{0, 1, 1},
            };

            var finalState = new List<List<int>>
            {
                new List<int>{0, 0, 0},
                new List<int>{0, 1, 1},
                new List<int>{0, 1, 1},
            };

            var initialBoard = new Board(initialState);
            var nextBoard = new Board(nextState);
            var finalBoard = new Board(finalState);

            AssertBoardsEqual(nextBoard, initialBoard.Tick());
            AssertBoardsEqual(finalBoard, initialBoard.Tick().Tick());
            Console.WriteLine("TestGame successful");
        }

        private static void TestBlinker()
        {
            var initialState = new List<List<int>>
            {
                new List<int>{0, 0, 0, 0, 0},
                new List<int>{0, 0, 0, 0, 0},
                new List<int>{0, 1, 1, 1, 0},
                new List<int>{0, 0, 0, 0, 0},
                new List<int>{0, 0, 0, 0, 0},
            };

            var secondState = new List<List<int>>
            {
                new List<int>{0, 0, 0, 0, 0},
                new List<int>{0, 0, 1, 0, 0},
                new List<int>{0, 0, 1, 0, 0},
                new List<int>{0, 0, 1, 0, 0},
                new List<int>{0, 0, 0, 0, 0},
            };

            var initialBoard = new Board(initialState);
            var secondBoard = new Board(secondState);

            AssertBoardsEqual(secondBoard, initialBoard.Tick());
            // It's periodic
            AssertBoardsEqual(initialBoard, initialBoard.Tick().Tick()); 
            Console.WriteLine("TestBlinker successful");
        }

        private static void AssertBoardsEqual(Board b1, Board b2)
        {
            if (!b1.Compare(b2))
            {
                throw new Exception($"Expected the state of {b1} to equal the state of {b2}");
            }
        }

    };

    class Board 
    {
        // Protected so that we can compare cells in the `Compare` method.
        protected List<List<Cell>> cells;

        public Board(List<List<Cell>> cells)
        {
            this.cells = cells;
        }

        // For ease of creating a board in tests, convert a 2-dimensional list of ints into Cells, 
        // where 0 = alive and 1 = dead. 
        // In real code I would probably provide more robust helpers for creating a board.
        // This method is unsafe as it attempts to cast an integer to an enum, but does nothing
        // to validate that the integer is in range, so it's marked internal, but I'm not sure if this is a good 
        // practice.
        internal Board(List<List<int>> cells)
        {
            this.cells = cells.Select(row =>
                row.Select(cell => new Cell((CellState)cell)).ToList()
            ).ToList();
        }

        public Cell GetCellAt(int x, int y)
        {
            // Cells outside boundary are assumed to be dead.
            if (x < 0 || y < 0) { return new Cell(CellState.Dead); }

            if (cells.Count() -1 < y || cells[y].Count() -1 < x)
            {
                    return new Cell(CellState.Dead);
            }
            return cells[y][x];
        }

        public int GetLivingNeighbourCountAt(int x, int y)
        {
            var neighbours =
                from rx in Enumerable.Range(-1, 3)
                from ry in Enumerable.Range(-1, 3)
                // Exclude the cell at (x,y).
                where !(rx == 0 && ry == 0)
                // We cast the CellState to an int (dead = 0, alive = 1), and then sum the list.
                select (int)GetCellAt(rx + x, ry + y).State;
            return neighbours.Sum();
        }
        
        // Returns a new board generated from the current state.
        public Board Tick() 
        {
            var newCells = cells.Select((row, y) =>
                row.Select((cell, x) =>
                    GetNextCellState(cell, x, y)
                ).ToList()
            ).ToList();
            return new Board(newCells);
        }

        private Cell GetNextCellState(Cell cell, int x, int y)
        {
            var n = GetLivingNeighbourCountAt(x, y);
            var nextState = GameLogic.NextCellState(cell.State, n);
            return new Cell(nextState);
        }

        // Very slow, but only used in tests. Once again I'm unsure about the usage of internal.
        internal Boolean Compare(Board other)
        {
            if (this.cells.Count() != other.cells.Count()) { return false;  }

            var i = 0;
            foreach (var row in this.cells)
            {
                var otherRow = other.cells[i];
                if (otherRow.Count() != row.Count()) { return false; }

                var k = 0;
                foreach(var elem in row)
                {
                    if (elem.State != otherRow[k].State) { return false; }
                    k++;
                }
                i++;
            }
            return true;
        }
    }

    static class GameLogic
    {
        // Style is probably bad here; unfortunately switch doesn't look very compelling.
        public static CellState NextCellState(CellState currentState, int neighbourCount)
        {
            if (currentState == CellState.Alive)
            {
                if (neighbourCount < 2) { return CellState.Dead;  }
                else if (neighbourCount == 2 || neighbourCount == 3) { return CellState.Alive;  }
                else { return CellState.Dead; }
            }

            if (currentState == CellState.Dead)
            {
                if (neighbourCount == 3) { return CellState.Alive; }
                else { return CellState.Dead;  }
            }
            // Unreachable.
            throw new Exception($"Bad State: currentState: {currentState}, neighbourCount: {neighbourCount}");
        }
    }

    enum CellState {
        Dead = 0,
        Alive = 1
    };

    struct Cell 
    {
        public CellState State { get; }
        public Cell(CellState state)
        {
            State = state; 
        }
    };
}
