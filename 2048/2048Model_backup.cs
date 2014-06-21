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
	class _2048Model_backup : AI.MCTS.IMCTSGame
	{
		public readonly IMatrix<int> Matrix;


		private const int size = 4;
		private const int startTiles = 2;
		private readonly Matrix<int> _matrix;
		private readonly Random random;
		private List<Element<int>> emptyTiles;


		public int Score { get { return this._score; } }
		private int _score = 0;


		public int PossibleMoves { get { return this._possibleMoves; } }
		private int _possibleMoves;


		public bool PlayersATurn { get { return this.emptyTiles == null; } }


		public bool IsAutoMovePossible 
		{ 
			get 
			{ 
				return this.emptyTiles != null && 0 < this.emptyTiles.Count;
			} 
		}


		public _2048Model_backup(int? randomSeed = null)
		{
			this._matrix = new Matrix<int>(size, size, 0);
			this.Matrix = this._matrix.AsReadOnly();

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


		public _2048Model_backup(_2048Model_backup model)
		{
			this._matrix = model._matrix.ToMatrix();
			this.Matrix = this._matrix.AsReadOnly();
			var formatter = new BinaryFormatter();
			using (Stream stream = new MemoryStream())
			{
				formatter.Serialize(stream, model.random);
				stream.Position = 0;
				this.random = (Random) formatter.Deserialize(stream);
			}
			this._score = model._score;
			if (model.emptyTiles == null)
				this.ResetEmptyTiles();
			else
				this.SetEmptyTiles();
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
			if (this.emptyTiles != null)
				throw new InvalidOperationException("Tile wasn't added after previous move!");
			bool moved = false;
			foreach (var row in this.GetNormalizedMatrix(move).Rows())
			{
				using (var destination = row.GetEnumerator())
				{
					using (var source = row.GetEnumerator())
					{
						bool sourceMoved = source.MoveNext();
						do
						{
							// move source to non-empty tile.
							while ((sourceMoved = source.MoveNext())
								&& source.Current.Value == 0
							) ;
							if (sourceMoved)
							{
								destination.MoveNext();
								bool repeat;
								do
								{
									repeat = false;
									// move tile into empty space
									if (destination.Current.Value == 0)
									{
										destination.Current.Value = 
											source.Current.Value;
										source.Current.Value = 0;
										moved = true;
										/*
											even moved tile can be subsequently
											merged so we need to move the source
											enumerator to the next tile and 
											repeat with the same destination 
											to try to merge them.
										 */
										while ((sourceMoved = source.MoveNext())
											&& source.Current.Value == 0
										) ;
										repeat = true;
									}
									// merge tiles with the same value
									else if (destination.Current.Value == 
										source.Current.Value)
									{
										this._score += destination.Current.Value *= 2;
										source.Current.Value = 0;
										moved = true;
									}
									/*
									 * if there is a gap between source and
									 * destination we can try to move destination
									 * closer to source and repeat.
									 */
									else if (
									   destination.Current.ColumnIndex <
									   source.Current.ColumnIndex - 1
									)
									{
										destination.MoveNext();
										repeat = true;
									}
								} while (repeat && sourceMoved);
							}
						} while (sourceMoved);
					}
				}
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
			if (this.emptyTiles != null)
			{
				if (0 < this.emptyTiles.Count)
				{
					this.emptyTiles[move / 2].Value = move % 2 == 1 ? 4 : 2;
					this.ResetEmptyTiles();
					return true;
				}
				else
					return false;
			}else
				return this.TryMove((_2048MoveDirection)move, false);
		}


		public AI.MCTS.IMCTSGame Clone()
		{
			return new _2048Model_backup(this);
		}


		public int GetAutoMoveIndex()
		{
			if (!this.IsAutoMovePossible)
				throw new InvalidOperationException("auto move is not possible.");
			var move = this.random.Next(this.emptyTiles.Count) * 2;
			if (this.random.Next(10) == 0)
				move += 1;
			return move;
		}


		/// <summary>
		/// Transforms matrix so MoveLeft performed on the resulting matrix
		/// is translated into requested <paramref name="move"/>.
		/// </summary>
		private IMatrix<int> GetNormalizedMatrix(_2048MoveDirection move)
		{
			switch (move)
			{
				case _2048MoveDirection.left:
					return this._matrix;
				case _2048MoveDirection.right:
					return this._matrix.Rotate(Rotation._180);
				case _2048MoveDirection.up:
					return this._matrix.Rotate(Rotation.left);
				case _2048MoveDirection.down:
					return this._matrix.Rotate(Rotation.right);
				default:
					throw new ArgumentException("unknown move", "move");
			}
		}


		private bool TryAutoAddTile()
		{
			if (this.emptyTiles == null)
				throw new InvalidOperationException(
					"Trying to add new tile twice."
				);
			if (0 < this.emptyTiles.Count)
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
			this.emptyTiles = this.GetEmptyTiles();
			this._possibleMoves = this.emptyTiles.Count * 2;
		}


		private void ResetEmptyTiles()
		{
			this.emptyTiles = null;
			this._possibleMoves = 0;
			IMatrix<int>[] matrixes = { this._matrix, this._matrix.Rotate(Rotation.right) };
			foreach (var matrix in matrixes)
			{
				foreach (var row in matrix.Rows())
				{
					int prev = 0;
					foreach (var element in row)
					{
						if (element.Value == 0 || element.Value == prev)
						{
							this._possibleMoves = 4;
							return;
						}
						prev = element.Value;
					}
				}
			}
		}


		private List<Element<int>> GetEmptyTiles()
		{
			if (this.emptyTiles != null)
				return this.emptyTiles;
			else
			{
				return (
					from elem in this._matrix.TraverseByRows()
					where elem.Value == 0
					select elem
				).ToList();
			}
		}

	}

}
