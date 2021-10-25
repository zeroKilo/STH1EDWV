using System;
using System.Runtime.InteropServices;

namespace Be.Windows.Forms
{
	internal static class NativeMethods
	{
		// Caret definitions
		[DllImport("user32.dll", SetLastError=true)]
		public static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

		[DllImport("user32.dll", SetLastError=true)]
		public static extern bool ShowCaret(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError=true)]
		public static extern bool DestroyCaret();

		[DllImport("user32.dll", SetLastError=true)]
		public static extern bool SetCaretPos(int x, int y);

		// Key definitions
		public const int WmKeydown = 0x100;
		public const int WmKeyup = 0x101;
		public const int WmChar = 0x102;
	}
}
