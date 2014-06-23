using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using _2048.Matrix;

#if DEBUG
[assembly: InternalsVisibleTo("2048Test")]
#endif

namespace _2048
{
	class _2048Model : AI.MCTS.IMCTSGame
	{
		public IMatrix<int> Matrix
		{
			get
			{
				if (this.__matrix == null)
					this.__matrix = Matrix<int>.FromArray(this._matrix, size);
				return this.__matrix;
			}
		}
		private IMatrix<int> __matrix;


		private const int size = 4;
		private const int size_sq = size * size;
		private const int startTiles = 2;
		private readonly int[,] _matrix = new int[size, size];
		private int? emptyTilesCount;
		private readonly Random random;


		public int Score { get { return this._score; } }
		private int _score = 0;


		public int PossibleMoves { get { return this._possibleMoves; } }
		private int _possibleMoves;


		public bool PlayersATurn { get { return !this.emptyTilesCount.HasValue; } }


		public bool IsAutoMovePossible
		{
			get
			{
				return this.emptyTilesCount.HasValue && 0 < this.emptyTilesCount.Value;
			}
		}


		public _2048Model(int? randomSeed = null)
		{
			if (randomSeed.HasValue)
				random = new Random(randomSeed.Value);
			else
				random = new Random();

			for (var counter = 0; counter < startTiles; counter++)
			{
				this.SetEmptyTiles();
				this.TryAutoAddTile();
			}
		}


		public _2048Model(_2048Model model)
		{
			Array.Copy(model._matrix, this._matrix, size * size);
			var formatter = new BinaryFormatter();
			using (Stream stream = new MemoryStream())
			{
				formatter.Serialize(stream, model.random);
				stream.Position = 0;
				this.random = (Random)formatter.Deserialize(stream);
			}
			this._score = model._score;
			if (model.emptyTilesCount.HasValue)
				this.SetEmptyTiles();
			else
				this.ResetEmptyTiles(force: true);
		}


		public bool MoveLeft()
		{
			return this.TryMove(_2048MoveDirection.left);
		}


		public bool MoveRight()
		{
			return this.TryMove(_2048MoveDirection.right);
		}


		public bool MoveUp()
		{
			return this.TryMove(_2048MoveDirection.up);
		}


		public bool MoveDown()
		{
			return this.TryMove(_2048MoveDirection.down);
		}


		public bool TryMove(_2048MoveDirection move, bool autoAddTile = true)
		{
			if (this.emptyTilesCount.HasValue)
				throw new InvalidOperationException("Tile wasn't added after previous move!");
			bool moved = false;
			var get = this.CreateValueGetter(move);
			var set = this.CreateValueSetter(move);
			for (int row = 0; row < size; ++row)
			{
				int destination = -1;
				int destinationValue;
				int source = 0;
				int sourceValue = -1;
				bool sourceMoved = source < size;
				do
				{
					destinationValue = get(row, ++destination);
					// move source to non-empty tile.
					while ((sourceMoved = ++source < size)
						&& (sourceValue = get(row, source)) == 0
					) ;
					if (sourceMoved)
					{
						bool repeat;
						do
						{
							repeat = false;
							// move tile into empty space
							if (destinationValue == 0)
							{
								destinationValue = sourceValue;
								set(row, destination, destinationValue);
								sourceValue = 0;
								set(row, source, sourceValue);
								moved = true;
								/*
									even moved tile can be subsequently
									merged so we need to move the source
									enumerator to the next tile and 
									repeat with the same destination 
									to try to merge them.
									*/
								while ((sourceMoved = ++source < size)
									&& (sourceValue = get(row, source)) == 0
								) ;
								repeat = true;
							}
							// merge tiles with the same value
							else if (destinationValue == sourceValue)
							{
								this._score += destinationValue *= 2;
								set(row, destination, destinationValue);
								sourceValue = 0;
								set(row, source, sourceValue);
								moved = true;
							}
							/*
								* if there is a gap between source and
								* destination we can try to move destination
								* closer to source and repeat.
								*/
							else if (destination < source - 1)
							{
								destinationValue = get(row, ++destination);
								repeat = true;
							}
						} while (repeat && sourceMoved);
					}
				} while (sourceMoved);
			}
			if (moved)
			{
				this.SetEmptyTiles();
				return !autoAddTile || this.TryAutoAddTile();
			}
			else
				return false;
		}


		public bool TryMove(int move)
		{
			if (move < 0 || this.PossibleMoves <= move)
				throw new ArgumentOutOfRangeException("move");
			if (this.emptyTilesCount.HasValue)
			{
				if (0 < this.emptyTilesCount.Value)
				{
					int expectedIndex = move / 2;
					int index = -1;
					for (int rowIndex = 0; rowIndex < size && index < expectedIndex; ++rowIndex)
					{
						for (int colIndex = 0; colIndex < size; ++colIndex)
						{
							if (this._matrix[rowIndex, colIndex] == 0 &&
								++index == expectedIndex)
							{
								this._matrix[rowIndex, colIndex] = move % 2 == 1 ? 4 : 2;
								this.__matrix = null;
								break;
							}
						}
					}
					if (index != expectedIndex)
						throw new InvalidOperationException(
							"internal 2048Model error: empty tile wasn't found."
						);
					this.ResetEmptyTiles();
					return true;
				}
				else
					return false;
			}
			else
				return this.TryMove((_2048MoveDirection)move, false);
		}


		public AI.MCTS.IMCTSGame Clone()
		{
			return new _2048Model(this);
		}


		public int GetAutoMoveIndex()
		{
			if (!this.IsAutoMovePossible)
				throw new InvalidOperationException("auto move is not possible.");
			var move = this.random.Next(this.emptyTilesCount.Value) * 2;
			if (this.random.Next(10) == 0)
				move += 1;
			return move;
		}


		private Func<int, int, int> CreateValueGetter(_2048MoveDirection move)
		{
			switch (move)
			{
				case _2048MoveDirection.left:
					return (rowIndex, colIndex) => this._matrix[
						rowIndex,
						colIndex
					];
				case _2048MoveDirection.right:
					return (rowIndex, colIndex) => this._matrix[
						size - rowIndex - 1,
						size - colIndex - 1
					];
				case _2048MoveDirection.up:
					return (rowIndex, colIndex) => this._matrix[
						colIndex,
						rowIndex
					];
				case _2048MoveDirection.down:
					return (rowIndex, colIndex) => this._matrix[
						size - colIndex - 1,
						size - rowIndex - 1
					];
				default:
					throw new ArgumentException(
						string.Format(
							"uknown _2048MoveDirection (value: {0})",
							move
						)
					);
			}
		}


		private Action<int, int, int> CreateValueSetter(_2048MoveDirection move)
		{
			switch (move)
			{
				case _2048MoveDirection.left:
					return (rowIndex, colIndex, value) =>
					{
						this.__matrix = null;
						this._matrix[
							rowIndex,
							colIndex
						] = value;
					};
				case _2048MoveDirection.right:
					return (rowIndex, colIndex, value) =>
					{
						this.__matrix = null;
						this._matrix[
							size - rowIndex - 1,
							size - colIndex - 1
						] = value;
					};
				case _2048MoveDirection.up:
					return (rowIndex, colIndex, value) =>
					{
						this.__matrix = null;
						this._matrix[
							colIndex,
							rowIndex
						] = value;
					};
				case _2048MoveDirection.down:
					return (rowIndex, colIndex, value) =>
					{
						this.__matrix = null;
						this._matrix[
							size - colIndex - 1,
							size - rowIndex - 1
						] = value;
					};
				default:
					throw new ArgumentException(
						string.Format(
							"uknown _2048MoveDirection (value: {0})",
							move
						)
					);
			}
		}


		[Obsolete]
		private Func<int, int, int> CreateNormalizedRowIndexGetter(_2048MoveDirection move)
		{
			switch (move)
			{
				case _2048MoveDirection.left:
					return (rowIndex, columnIndex) => rowIndex;
				case _2048MoveDirection.right:
					return (rowIndex, columnIndex) => size - rowIndex - 1;
				case _2048MoveDirection.up:
					return (rowIndex, columnIndex) => columnIndex;
				case _2048MoveDirection.down:
					return (rowIndex, columnIndex) => size - columnIndex - 1;
				default:
					throw new ArgumentException(
						string.Format(
							"uknown _2048MoveDirection (value: {0})",
							move
						)
					);
			}
		}


		[Obsolete]
		private Func<int, int, int> CreateNormalizedColumnIndexGetter(_2048MoveDirection move)
		{
			switch (move)
			{
				case _2048MoveDirection.left:
					return (rowIndex, columnIndex) => columnIndex;
				case _2048MoveDirection.right:
					return (rowIndex, columnIndex) => size - columnIndex - 1;
				case _2048MoveDirection.up:
					return (rowIndex, columnIndex) => rowIndex;
				case _2048MoveDirection.down:
					return (rowIndex, columnIndex) => size - rowIndex - 1;
				default:
					throw new ArgumentException(
						string.Format(
							"uknown _2048MoveDirection (value: {0})",
							move
						)
					);
			}
		}


		private bool TryAutoAddTile()
		{
			if (!this.emptyTilesCount.HasValue)
				throw new InvalidOperationException(
					"Trying to add new tile twice."
				);
			if (0 < this.emptyTilesCount.Value)
			{
				this.TryMove(this.GetAutoMoveIndex());

				return true;
			}
			else return false;
		}


		private int GetNewTileValue()
		{
			return this.random.Next(10) == 0 ? 4 : 2;
		}


		private void SetEmptyTiles()
		{
			this.emptyTilesCount = this.GetEmptyTilesCount();
			this._possibleMoves = this.emptyTilesCount.Value * 2;
		}


		private void ResetEmptyTiles(bool force = false)
		{
			if (force || this.emptyTilesCount.HasValue)
			{
				this.emptyTilesCount = null;
				this._possibleMoves = 0;
				var getLeft = this.CreateValueGetter(_2048MoveDirection.left);
				var getUp = this.CreateValueGetter(_2048MoveDirection.up);
				for (int rowIndex = 0; rowIndex < size; ++rowIndex)
				{
					int prev = 0;
					int prevUp = 0;
					for (int colIndex = 0; colIndex < size; ++colIndex)
					{
						int value = getLeft(rowIndex, colIndex);
						int valueUp = getUp(rowIndex, colIndex);
						if (value == 0 || value == prev || valueUp == 0 || valueUp == prevUp)
						{
							this._possibleMoves = 4;
							return;
						}
						prev = value;
						prevUp = valueUp;
					}
				}
			}
		}


		private int GetEmptyTilesCount()
		{
			if (this.emptyTilesCount.HasValue)
				return this.emptyTilesCount.Value;
			else
			{
				int count = 0;
				for (int rowIndex = 0; rowIndex < size; ++rowIndex)
				{
					for (int colIndex = 0; colIndex < size; ++colIndex)
					{
						if (this._matrix[rowIndex, colIndex] == 0)
							count++;
					}
				}
				this.emptyTilesCount = count;
				return count;
			}
		}

	}

}
