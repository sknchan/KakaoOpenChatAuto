using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;


   public  class KakaoTalkService
    {
        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpClassName, string lpWindowName);
        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int SendMessage(IntPtr hWnd, uint uMsg, IntPtr wParam, string lParam);
        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int PostMessage(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
       public static extern int DestroyWindow(IntPtr hWnd);
        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern void Sleep(uint dwMilliseconds);
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.StdCall)]

        public static extern int GetLastError();
        public static string GetWindowTitle(IntPtr hWnd)
        {
            var sb = new StringBuilder(GetWindowTextLength(hWnd) + 1);
            int length = GetWindowText(hWnd, sb, sb.Capacity);
            if (length > 0)
            { return Convert.ToString(sb); }
            return string.Empty;
        }

        public const int GWL_STYLE = -16;
        public const int WM_SETTEXT = 0x000C;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int WS_VISIBLE = 0x10000000;
        public const int WM_COMMAND = 0x0111;

        public const int ES_PASSWORD = 0x20;
        public const int ES_AUTOHSCROLL = 0x80;

        public struct ChatRoomInfo
        {
            public string Name;
            public IntPtr Handle;
        }
        public static ChatRoomInfo[] SearchOpenedChatRooms()
        {
            List<ChatRoomInfo> openedChatRooms = new List<ChatRoomInfo>(32);
            IntPtr hDialog = IntPtr.Zero;
            while (true)
            {
                hDialog = FindWindowEx(IntPtr.Zero, hDialog, DialogClass, null);

                if (hDialog == IntPtr.Zero) break;
                if (IsValidChatRoom(hDialog))
                {
                    ChatRoomInfo cri = new ChatRoomInfo()
                    {
                        Name = "<제목 없음>",
                        Handle = hDialog
                    };

                    int len = GetWindowTextLength(hDialog);
                    if (len > 0)
                    {
                        len++;
                        StringBuilder sbText = new StringBuilder(len);
                        if (GetWindowText(hDialog, sbText, len) > 0)
                            cri.Name = sbText.ToString();
                    }
                    openedChatRooms.Add(cri);
                }
            }
            return openedChatRooms.ToArray();
        }
        public static uint WM_SYSCOMMAND = 0x0112;
        public static int SC_CLOSE = 0xF060;
        static uint WM_CLOSE = 0x10;
        public static uint WM_DESTROY = 0x0002;
        private static uint WM_NCDESTROY = 0x0082;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRectkonachd);
        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);
       static string hexget(int a)
        {
            return string.Format("{0:X8}", a);
        }
        public static void CloseWindow(IntPtr hWnd)
        {
            PostMessage(hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            PostMessage(hWnd, WM_DESTROY, IntPtr.Zero, IntPtr.Zero);
            PostMessage(hWnd, WM_NCDESTROY, IntPtr.Zero, IntPtr.Zero);
        }


        static bool IsValidChatRoom(IntPtr hWnd)
        {

                IntPtr
                    Ctrl1 = FindWindowEx(hWnd, IntPtr.Zero, ctrl1, null),
                    Ctrl2 = FindWindowEx(hWnd, IntPtr.Zero, ctrl2, null),
                    Ctrl3 = FindWindowEx(Ctrl2, IntPtr.Zero, ctrl3, null);

                return (Ctrl1 != IntPtr.Zero) && (Ctrl2 != IntPtr.Zero) && (Ctrl3 != IntPtr.Zero);
        }

        public static string GetWindowText(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);
            if (length > 0)
            {
                StringBuilder sb = new StringBuilder(length + 1);
                if (GetWindowText(hWnd, sb, length + 1) > 0) return sb.ToString();
                else return null;
            }
            else return null;
        }
        public const string AppTitle = "카카오톡";

        public const string DialogClass = "#32770";
        public const string EditClass = "Edit";
        public const string RichEditClass = "RichEdit20W";
        public const string AdClass = "EVA_Window";

        public const string ctrl1 = "RichEdit20W";             // 채팅방 텍스트 입력 칸 
        public const string ctrl2 = "EVA_VH_ListControl";      // 채팅방 채팅 기록
        public const string ctrl3 = "_EVA_CustomScrollCtrl";
    }

