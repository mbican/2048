using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using _2048.Matrix;

#if DEBUG
[assembly: InternalsVisibleTo("2048Test")]
#endif

namespace _2048
{
	class _2048Model
	{
		public readonly IMatrix<int> Matrix;


		private const int size = 4;
		private const int startTiles = 2;
		private readonly Matrix<int> _matrix = new Matrix<int>(size, size, 0);
		private readonly Random random;


		public _2048Model(int? randomSeed = null)
		{
			this.Matrix = this._matrix.AsReadOnly();

			if (randomSeed.HasValue)
				random = new Random(randomSeed.Value);
			else
				random = new Random();

			for (var counter = 0; counter < startTiles; counter++)
				this.TryAddTile();
		}


		public bool MoveLeft()
		{
			return this.Move(_2048MoveDirection.left);
		}


		public bool MoveRight()
		{
			return this.Move(_2048MoveDirection.right);
		}


		public bool MoveUp()
		{
			return this.Move(_2048MoveDirection.up);
		}


		public bool MoveDown()
		{
			return this.Move(_2048MoveDirection.down);
		}


		public bool Move(_2048MoveDirection move)
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
										destination.Current.Value *= 2;
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


		int GetNewTileValue()
		{
			return this.random.Next(10) == 0 ? 4 : 2;
		}

	}

}
