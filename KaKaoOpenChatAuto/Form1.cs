using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaKaoOpenChatAuto
{
    public partial class Form1 : Form
    {
        IntPtr KakaoTalkChatHandle = IntPtr.Zero;
        IntPtr KakaoTalkChatParentHandle = IntPtr.Zero;
        public Form1()
        {
            InitializeComponent();
        }



        private void button1_Click(object sender, EventArgs e)
        {

            NativeHelper.EnumWindows((x, y) =>
             {
                 if (NativeHelper.GetClassNameOfWindow(x) == "#32770")
                 {
                     string s = NativeHelper2.GetControlText(x.Children().First());
                     if (s == "채팅방 이름/소개, 태그 검색")
                     {
                         KakaoTalkChatParentHandle = x;
                         KakaoTalkChatHandle = x.Children().First();
                         return false;
                     }
                 }
                 return true;
             }, IntPtr.Zero);


            if (KakaoTalkChatHandle != IntPtr.Zero && KakaoTalkChatParentHandle != IntPtr.Zero)
            {
               textBox2.Text = "카톡상태 : 0x" + String.Format("{0:X08}", KakaoTalkChatHandle.ToInt32());
                NativeHelper.SetWindowPos(KakaoTalkChatParentHandle, IntPtr.Zero, 0, 0, 412, 592, NativeHelper.SetWindowPosFlags.ShowWindow);
                KakaoTalkService.ChatRoomInfo[] rooms = KakaoTalkService.SearchOpenedChatRooms();
               foreach(KakaoTalkService.ChatRoomInfo c in rooms)
                {
                    KakaoTalkService.CloseWindow(c.Handle);
                }
            }
            else
            {
               textBox2.Text = "카톡상태 : 0x0";
                MessageBox.Show("카톡창을찾을수없습니다\n오픈채팅창을열어주세요\n이미열려있다면 껏다가 켜주시기바랍니다");

            }


        }

        
        Thread Main;
        public void ScrollDown(IntPtr Handle)
        {
            NativeHelper.SetFocus(KakaoTalkChatParentHandle);
            NativeHelper.SetForegroundWindow(KakaoTalkChatParentHandle);
            ///(186, 93));
            NativeHelper.SendMessage(Handle, 0x84, (IntPtr)0, NativeHelper.MakeLParam(170, 100));
            NativeHelper.SendMessage(Handle, (uint)NativeHelper.WM_SETCURSOR, Handle, (IntPtr)0x2000001);

            NativeHelper.PostMessage(Handle, NativeHelper.WM_MOUSEWHEEL, (IntPtr)(-120 << 16), NativeHelper.MakeLParam(186, 93));
        }
        void MainWork()
        {
            #region 검색
            NativeHelper.SetFocus(KakaoTalkChatParentHandle); // 포커스
            NativeHelper.LbuttonClick(KakaoTalkChatParentHandle, 380, 53); // 초기화
            Thread.Sleep(1000);
            NativeHelper.SendMessage(KakaoTalkChatHandle, NativeHelper.WM_SETTEXT, IntPtr.Zero, KeywordBox.Text); // 키워드삽입
            Thread.Sleep(55);
            SendKeyInput(KakaoTalkChatHandle, (IntPtr)NativeHelper.VK_ENTER); // 검색
            Thread.Sleep(500);
            #endregion

            #region 그룹채팅방으로변경
            NativeHelper.LbuttonClick(KakaoTalkChatParentHandle.Children().Skip(2).First(), 360, 20); 
            Thread.Sleep(1000);
            NativeHelper.LbuttonClick(KakaoTalkChatParentHandle.Children().Skip(2).First(), 357, 100);
            Thread.Sleep(1000);
            SendKeyInput(KakaoTalkChatHandle, (IntPtr)NativeHelper.VK_DOWN);
            Thread.Sleep(33);
            SendKeyInput(KakaoTalkChatHandle, (IntPtr)NativeHelper.VK_DOWN);
            Thread.Sleep(33);
            SendKeyInput(KakaoTalkChatHandle, (IntPtr)NativeHelper.VK_DOWN);
            Thread.Sleep(33);
            SendKeyInput(KakaoTalkChatParentHandle, (IntPtr)NativeHelper.VK_ENTER);
            NativeHelper.SendMessage(KakaoTalkChatHandle, NativeHelper.WM_SETTEXT, IntPtr.Zero, KeywordBox.Text);
            Thread.Sleep(33);
            #endregion

            NativeHelper.SetFocus(KakaoTalkChatParentHandle);
            NativeHelper.SetForegroundWindow(KakaoTalkChatParentHandle);
     
            List<IntPtr> 그룹참가대기리스트 = new List<IntPtr>();
            List<String> 그룹이름리스트 = new List<string>();
            int  errcount = 0;
            while(Thread.CurrentThread.IsAlive)
            {

                foreach (KakaoTalkService.ChatRoomInfo c in KakaoTalkService.SearchOpenedChatRooms())
                {
                    KakaoTalkService.CloseWindow(c.Handle);
                }
                if (errcount >= 5)
                {
                    break;
                }
                ScrollDown(KakaoTalkChatParentHandle);
                Thread.Sleep(1000);
                Point p = NativeHelper.ColorPicker(NativeHelper.GetWindowScreen(KakaoTalkChatParentHandle.Children().Skip(2).First()), ColorTranslator.FromHtml("#4A4A4A"));
                NativeHelper.LbuttonDoubleClick(KakaoTalkChatParentHandle.Children().Skip(2).First(), p.X, p.Y);
                Thread.Sleep(1000);
                IntPtr po = IntPtr.Zero;
                NativeHelper.EnumWindows((x, y) =>
                {
                    if (NativeHelper.GetClassNameOfWindow(x) == "EVA_Window_Dblclk")
                    {
                        po = x;
                        return false;
                    }
                    return true;
                }, IntPtr.Zero);
                if(그룹참가대기리스트.Contains(po))
                {
                    errcount++;
                    continue;
                }
                errcount = 0;
                그룹참가대기리스트.Add(po);
                NativeHelper.LbuttonClick(po, 170, 570);
                Thread.Sleep(1000);
                IntPtr po2 = IntPtr.Zero;
                NativeHelper.EnumWindows((x, y) =>
                {
                    if (NativeHelper.GetClassNameOfWindow(x) == "#32770")
                    {
                        if (x.Children().Count() == 1)
                        {
                            if (NativeHelper.GetClassNameOfWindow(x.Children().First()) == "Edit")
                            {
                                po2 = x;
                                return false;
                            }
                        }

                    }
                    return true;
                }, IntPtr.Zero);
                if(po2 == IntPtr.Zero && KakaoTalkService.SearchOpenedChatRooms().Count()==0)
                {
                    Point pppp = NativeHelper.ColorPicker(NativeHelper.GetWindowScreen(po), ColorTranslator.FromHtml("#FFEA40"));
                    NativeHelper.LbuttonClick(po, pppp.X, pppp.Y);
                    continue;
                }
                KakaoTalkService.ChatRoomInfo[] rooms = KakaoTalkService.SearchOpenedChatRooms();
                NativeHelper.LbuttonClick(po2, 215, 185); // 카카오프렌즈설정
                Thread.Sleep(1000);
                NativeHelper.SendMessage(po2.Children().First(), NativeHelper.WM_SETTEXT, IntPtr.Zero, RandomString(4)); // 카톡이름설정
                NativeHelper.LbuttonClick(po2, 115, 295); // 방참가
                Thread.Sleep(1000);
                if (rooms.Count()==0)
                {
                    continue;
                }
                if(그룹이름리스트.Contains(rooms[0].Name))
                {
                    KakaoTalkService.CloseWindow(rooms[0].Handle);
                    continue;
                }
                그룹이름리스트.Add(rooms[0].Name);
               
               PerformSendText(rooms[0].Handle, WriteBox.Text);
                Thread.Sleep(1000 * 60 * Convert.ToInt32(Delay.Value));
                
            }
            WriteBox.Enabled = true;
            button1.Enabled = true;
            button2.Text = "시작";
            checkBox1.Enabled = true;
            Delay.Enabled = true;
            KeywordBox.Enabled = true;
            MessageBox.Show("완료");
        }
        void SendKeyInput(IntPtr Handle, IntPtr Key)
        {
            NativeHelper.PostMessage(Handle, NativeHelper.WM_KEYDOWN, Key, IntPtr.Zero);
            NativeHelper.PostMessage(Handle, NativeHelper.WM_CHAR, Key, IntPtr.Zero);
            NativeHelper.SendMessage(Handle, NativeHelper.WM_CHAR, Key, IntPtr.Zero);
            NativeHelper.PostMessage(Handle, NativeHelper.WM_KEYUP, Key, IntPtr.Zero);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "중지")
            {
                button2.Text = "시작";
                KeywordBox.Enabled = true;
                
                WriteBox.Enabled = true;
                checkBox1.Enabled = true;
                Delay.Enabled = true;
                button1.Enabled = true;
                Main.Abort();
            }
            else
            {
                
                if (KeywordBox.Text.Length == 0)
                {
                    MessageBox.Show("검색키워드를 넣어주세요");
                    return;
                }
                if(WriteBox.Text.Length==0)
                {
                    MessageBox.Show("홍보글을넣어주세요");
                    return;
                }
                if (KakaoTalkChatHandle == IntPtr.Zero)
                {
                    MessageBox.Show("오픈채팅방을 여신후 새로고침을눌러주세요");
                    return;
                }
                Main = new Thread(new ThreadStart(MainWork));
                Main.IsBackground = true;
                Main.Start();
                checkBox1.Enabled = false;
                Delay.Enabled = false;
                KeywordBox.Enabled = false;
          
                WriteBox.Enabled = false;
                button1.Enabled = false;
                button2.Text = "중지";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
        }

       
        public void PerformSendText(IntPtr hWnd, string text)
        {
            IntPtr hEdit = NativeHelper.FindWindowEx(hWnd, IntPtr.Zero, "RichEdit20W", null);
            if (hEdit == IntPtr.Zero) return;

            Thread.Sleep(33);
            NativeHelper.SendMessage(hEdit, NativeHelper.WM_SETTEXT, IntPtr.Zero, text);
            Thread.Sleep(34);
            NativeHelper.PostMessage(hWnd, NativeHelper.WM_KEYDOWN, (IntPtr)13, IntPtr.Zero);
            Thread.Sleep(33);
            NativeHelper.PostMessage(hWnd, NativeHelper.WM_KEYUP, (IntPtr)13, IntPtr.Zero);
            Thread.Sleep(500);
            if (checkBox1.Checked)
            {
                OutRoom();
            }
            else
            {
                KakaoTalkService.CloseWindow(hWnd);
            }
            Thread.Sleep(1000);
        }
        Random rrrr = new Random();
        public string RandomString(int Length)
        {
            string chars = "qwertzuiopasdfghjklyxcvbnmQWERTZUIOPASDFGHJKLYXCVBNM";
            string ret = "";
            for (int i = 0; i < Length; i++)
                ret += chars[rrrr.Next(chars.Length)];
            return ret;
        }
        void OutRoom()
        {
            NativeHelper.LbuttonClick(KakaoTalkService.SearchOpenedChatRooms()[0].Handle, 330, 50);
            Thread.Sleep(33);
            SendKeyInput(KakaoTalkService.SearchOpenedChatRooms()[0].Handle, (IntPtr)NativeHelper.VK_DOWN);
            Thread.Sleep(33);
            SendKeyInput(KakaoTalkService.SearchOpenedChatRooms()[0].Handle, (IntPtr)NativeHelper.VK_DOWN);
            Thread.Sleep(33);
            SendKeyInput(KakaoTalkService.SearchOpenedChatRooms()[0].Handle, (IntPtr)NativeHelper.VK_DOWN);
            Thread.Sleep(33);
            SendKeyInput(KakaoTalkService.SearchOpenedChatRooms()[0].Handle, (IntPtr)NativeHelper.VK_DOWN);
            Thread.Sleep(33);
            SendKeyInput(KakaoTalkService.SearchOpenedChatRooms()[0].Handle, (IntPtr)NativeHelper.VK_DOWN);
            Thread.Sleep(33);
            SendKeyInput(KakaoTalkService.SearchOpenedChatRooms()[0].Handle, (IntPtr)NativeHelper.VK_DOWN);
            Thread.Sleep(33);
            SendKeyInput(KakaoTalkService.SearchOpenedChatRooms()[0].Handle, (IntPtr)NativeHelper.VK_DOWN);
            Thread.Sleep(33);
            SendKeyInput(KakaoTalkService.SearchOpenedChatRooms()[0].Handle, (IntPtr)NativeHelper.VK_DOWN);
            Thread.Sleep(33);
            SendKeyInput(KakaoTalkService.SearchOpenedChatRooms()[0].Handle, (IntPtr)NativeHelper.VK_DOWN);
            Thread.Sleep(33);
            SendKeyInput(KakaoTalkService.SearchOpenedChatRooms()[0].Handle, (IntPtr)NativeHelper.VK_ENTER);
            Thread.Sleep(500);
            NativeHelper.EnumWindows((x, y) =>
            {
                if (NativeHelper.GetClassNameOfWindow(x) == "EVA_Window_Dblclk")
                {
                    if (x.Children().Count() == 0)
                    {
                        if (NativeHelper.ColorPickerb(NativeHelper.GetWindowScreen(x), ColorTranslator.FromHtml("#FFEA40")))
                        {
                            Point p = NativeHelper.ColorPicker(NativeHelper.GetWindowScreen(x), ColorTranslator.FromHtml("#FFEA40"));
                            NativeHelper.LbuttonClick(x, p.X, p.Y);
                            return false;
                        }

                    }

                }
                return true;
            }, IntPtr.Zero);
           
        }

    }
}
