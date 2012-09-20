
/*
 * This file is part of the tictactoe package.
 *
 * (c) Stefano Oldeman <stefano.oldeman@gmail.com>
 *
 * For the full copyright and license information, please view the README.md
 * file that was distributed with this source code.
 */

using System;
using Gtk;

namespace tictactoe
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow window = new MainWindow ();
			window.Show ();
			Application.Run ();
		}
	}
}
