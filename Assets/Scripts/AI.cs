using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AI {
	// Game state evaluation scores.
	// For X, the higher the better.
	// For O, the lower the better.
	// The maximum boundary score for win/lose
	public const int MAX = 200;
	// X won the match
	public const int X_WIN = MAX;
	// X lost the match
	public const int O_WIN = - MAX;
	// two chesses in a line (one more to win):
	public const int TWO_IN_LINE = MAX / 2;
	// nothing special, continue playing:
	public const int CONTINUE = 1;
	// Draw.
	public const int DRAW = 0;

	// There are 8 win/lose conditions:
	// 3 horizontal lines, 
	// 3 vertical lines, 
	// and 2 diagonal lines.
	private readonly int[][] WINNING_CELLS = new int[][]{
		// horizontal
		new int []{0, 1, 2}, new int []{3, 4, 5}, new int []{6, 7, 8},
		// vertical
		new int []{0, 3, 6}, new int []{1, 4, 7}, new int []{2, 5, 8},
		// diagonal
		new int []{0, 4, 8}, new int []{2, 4, 6},
	};

	// This openning strategy increases the winning chance.
	// Refered WIKIPEDIA page: http://en.wikipedia.org/wiki/Tic-tac-toe Variations section.
	// This works as a openning chess book for our evaluation function.
	private readonly int[] OPENNING_EVA = new int[]{
		3, 2, 3,
		2, 4, 2,
		3, 2, 3
	};

	/// <summary>
	/// The _initial move flag indicates we evaluate with AlphaBeta search or given openning score.
	/// If set to FALSE, won't use the openning score.
	/// </summary>
	private bool _initialMoveFlag = false;

	/// <summary>
	/// Evaluates the current board.
	/// </summary>
	/// <returns>
	/// The board score for X.
	/// The higher, the better.
	/// (so, for O, the lower, the better.)
	/// </returns>
	public int EvaluateBoard(GameController.Cell[] board) {
		int ret = CONTINUE;
		
		// Test winning condition:
		foreach (int[] indices in WINNING_CELLS) {
			GameController.Cell cell = board[indices[0]];
			if (cell == GameController.Cell.EMPTY) {
				continue;
			}
			bool gameOver = true;
			for (int i = 1; i < indices.Length; ++i) {
				if (board[indices[i]] != cell) {
					gameOver = false;
					break;
				}
			}
			if (gameOver) {
				return board[indices[0]] == GameController.Cell.X ? X_WIN : O_WIN;
			}
		}
		
		// No win/lose:
		// check game status:
		bool draw = true;
		for (int i=0; i<board.Length; ++i) {
			if (board[i] == GameController.Cell.EMPTY) {
				draw = false;
			}
		}
		if (draw) {
			return DRAW;
		}
		
		// No win, no lose, check other conditions (deciding how smart the AI is):
		// Try to find lines with 1 step to win (2 chesses in line with an empty cell):
		//for (int i=0; i<board.Length; ++i) {
		//	Debug.Log(board[i] + ", ");
		//}
		foreach (int[] indices in WINNING_CELLS) {
			int empty_count = 0;
			int x_count = 0;
			int o_count = 0;
			for (int i=0; i<indices.Length; ++ i) {
				if (board[indices[i]] == GameController.Cell.EMPTY) {
					empty_count++;
				} else if (board[indices[i]] == GameController.Cell.X) {
					x_count++;
				} else if (board[indices[i]] == GameController.Cell.O) {
					o_count++;
				}
			}
			
			if (empty_count == 1) {
				if (x_count == 2) {
					// If there are 2 cases of 2 in lines, it's a guaranteed win.
					ret = TWO_IN_LINE;
				} else if (o_count == 2) {
					ret = TWO_IN_LINE;
				}
			}
		}
		return ret;
	}

	/// <summary>
	/// Return the best move from algorithm.
	/// </summary>
	/// <returns>The move index.</returns>
	/// <param name="board">Current board.</param>
	/// <param name="chess">AI chess type.</param>
	public int BestMove(GameController.Cell[] board, GameController.Cell chess) {
		int bestScore = chess == GameController.Cell.X ? O_WIN : X_WIN;
		int index = -1;
		List<int> bestMoves = new List<int>();
		for (int i = 0; i < board.Length; ++i) {
			if (board[i] == GameController.Cell.EMPTY) {
				if (index == -1) {
					index = i;
				}
				board[i] = chess;
				int score = 0;
				if (!_initialMoveFlag) {
					score = chess == GameController.Cell.X ? maxSearchO(board, O_WIN, X_WIN, 9) : maxSearchX(board, O_WIN, X_WIN, 9);
				} else {
					score = chess == GameController.Cell.X ? OPENNING_EVA[i] : -OPENNING_EVA[i];
				}
				if ((chess == GameController.Cell.X && score > bestScore) || (chess == GameController.Cell.O && score < bestScore)) {
					index = i;
					bestMoves.Clear();
					bestMoves.Add(i);
					bestScore = score;
				} else if (score == bestScore) {
					bestMoves.Add(i);
				}
				board[i] = GameController.Cell.EMPTY;
			}
		}
		// clear openning flag
		if (_initialMoveFlag) {
			_initialMoveFlag = false;
		}

		if (bestMoves.Count == 0) {
			return index;
		}
		System.Random rand = new System.Random();
		return bestMoves[rand.Next(0, bestMoves.Count)];
	}

	/// <summary>
	/// Solution tree search to maxmize X's next score.
	/// 	- which minimizes O's score.
	/// </summary>
	/// <returns>The next highest score step of X.</returns>
	public int maxSearchX(GameController.Cell[] board,
	                      int alpha, int beta, int depth) {
		int score = EvaluateBoard(board);
		
		// reached search depth.
		if (depth == 0) {
			return score;
		}
		
		// reached end
		if (score == X_WIN || score == O_WIN || score == DRAW) {
			return score;
		}
		
		// search
		int best = O_WIN;
		// The order to place next chess matters a lot.
		// simplify it by just doing 0~9 itertation
		for (int i=0; i<board.Length; ++i) {
			if (board[i] == GameController.Cell.EMPTY) {
				board[i] = GameController.Cell.X;
				// switch turn: X/O
				best = Math.Max(best, maxSearchO(board, alpha, beta, depth-1));
				board[i] = GameController.Cell.EMPTY;
				// opponent won't let any beta to go higher,
				//  this entire node can be prunned.
				if (best >= beta) {
					return beta;
				}
				// update alpha if we get anything better:
				if (alpha < best) {
					alpha = best;
				}
			}
		}
		
		return best;
	}

	/// <summary>
	/// Solution tree search to maxmize O's next score.
	/// 	- which minimizes X's score.
	/// </summary>
	/// <returns>The next highest score step of O.</returns>
	public int maxSearchO(GameController.Cell[] board, 
	                      int alpha, int beta, int depth) {
		int score = EvaluateBoard(board);
		
		// reached search depth.
		if (depth == 0) {
			return score;
		}
		
		// reached end
		if (score == X_WIN || score == O_WIN || score == DRAW) {
			return score;
		}
		
		// search
		int best = X_WIN;
		// The order to place next chess matters a lot.
		// simplify it by just doing 0~9 itertation
		for (int i=0; i<board.Length; ++i) {
			if (board[i] == GameController.Cell.EMPTY) {
				board[i] = GameController.Cell.O;
				// switch turn: X/O
				best = Math.Min(best, maxSearchX(board, alpha, beta, depth-1));
				board[i] = GameController.Cell.EMPTY;
				// opponent won't let any beta to go higher,
				//  this entire node can be prunned.
				if (best <= alpha) {
					return alpha;
				}
				// update beta if we get anything better:
				if (best < beta) {
					beta = best;
				}
			}
		}
		
		return best;
	}
}


