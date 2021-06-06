using System;
using System.Drawing;
using System.Threading.Tasks;

namespace AutoResizer
{
    class OperationWindow
    {

        IntPtr HWND;
        Rectangle _currentWindowSize;

        string filePath = @"./setting.xml";

        int time = 100;
        bool _isEdit = false;
        public OperationWindow( IntPtr hwnd )
        {
            HWND = hwnd;

            LoadPosition();

            Task task1 = Task.Run( WaitKeyInput );
            Task mainLoop = Task.Run( SetWindowSize );

            mainLoop.Wait();

        }

        private async Task WaitKeyInput()
        {
            while(true)
            {
                string str = Console.ReadLine();
                if (str != "")
                {
                    Operation(str)();
                }
                await Task.Delay( 100 );
            }
        }

        private Action Operation(string op) => op switch
        {
            "s" => SavePosition,  //  エイリアス
            "save" => SavePosition,
            "c" => ChangeWindowPosition,  //  エイリアス
            "change" => ChangeWindowPosition,
            _ => () => { }
        };

        private void LoadPosition()
        {
            if(System.IO.File.Exists(filePath))
            {
                System.Xml.Serialization.XmlSerializer s = new(typeof(Rectangle));
                using (var sr = new System.IO.StreamReader(filePath, new System.Text.UTF8Encoding(false)))
                {
                    _currentWindowSize = (Rectangle)s.Deserialize( sr );
                }
            }
            else
            {
                SavePosition();
            }
        }

        private void SavePosition()
        {
            Rectangle rect = _currentWindowSize = GetWindowSize();
            _isEdit = false;


            System.Xml.Serialization.XmlSerializer s = new( typeof( Rectangle ) );
            using (var sw = new System.IO.StreamWriter( filePath, false, new System.Text.UTF8Encoding(false) ))
            {
                s.Serialize( sw, rect );
            }

            Console.WriteLine("現在位置を保存しました");
        }

        private void ChangeWindowPosition() => _isEdit = true;
        

        private Rectangle GetWindowSize()
        {

            WAPI.RECT rect;
            bool flag = WAPI.GetWindowRect(HWND, out rect);

            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            

            return new Rectangle( rect.left, rect.top, width, height );
        }

        private async Task SetWindowSize()
        {
            while(true)
            {
                if (WAPI.IsWindow(HWND) == 0)
                {
                    Environment.Exit(0);
                }
                if (!_isEdit && WAPI.GetForegroundWindow() == HWND && _currentWindowSize != GetWindowSize())
                {
                    WAPI.SetWindowPos(HWND, IntPtr.Zero, _currentWindowSize.X, _currentWindowSize.Y, _currentWindowSize.Width, _currentWindowSize.Height, 0);
                }
                await Task.Delay( time );
            }
        }
    }
}
