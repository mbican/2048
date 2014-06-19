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
		public readonly IMatrix<int> Matrix;


		private const int size = 4;
		private const int startTiles = 2;
		private readonly Matrix<int> _matrix;
		private readonly Random random;


		public int Score { get { return this._score; } }
		private int _score = 0;


		public int PossibleMoves { get { return 4; } }


		public _2048Model(int? randomSeed = null)
		{
			this._matrix = new Matrix<int>(size, size, 0);
			this.Matrix = this._matrix.AsReadOnly();

			if (randomSeed.HasValue)
				random = new Random(randomSeed.Value);
			else
				random = new Random();

			for (var counter = 0; counter < startTiles; counter++)
				this.TryAddTile();
		}


		public _2048Model(_2048Model model)
		{
			this._matrix = model._matrix.ToMatrix();
			this.Matrix = this._matrix.AsReadOnly();
			var formatter = new BinaryFormatter();
			using (Stream stream = new MemoryStream())
			{
				formatter.Serialize(stream, model.random);
				stream.Seek(0, SeekOrigin.Begin);
				this.random = (Random) formatter.Deserialize(stream);
			}
			this._score = model._score;
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


		public bool TryMove(_2048MoveDirection move)
		{
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
			return moved && this.TryAddTile();
		}


		public bool TryMove(int move)
		{
			if (move < 0 || this.PossibleMoves <= move)
				throw new ArgumentOutOfRangeException("move");
			return this.TryMove((_2048MoveDirection)move);
		}


		public AI.MCTS.IMCTSGame Clone()
		{
			return new _2048Model(this);
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


		private bool TryAddTile()
		{
			var emptyElems = (from elem in _matrix.TraverseByRows()
							  where elem.Value == 0
							  select elem).ToList();
			if (0 < emptyElems.Count)
			{
				emptyElems[this.random.Next(emptyElems.Count)].Value =
					this.GetNewTileValue();
				return true;
			}
			else return false;
		}


		private int GetNewTileValue()
		{
			return this.random.Next(10) == 0 ? 4 : 2;
		}

	}

}
