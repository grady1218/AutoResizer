using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AutoResizer
{
    class Program
    {
        static GetWindow getWindow;
        static OperationWindow operationWindow;
        static void Main(string[] args)
        {
            
            getWindow = new GetWindow( "umamusume" );

            operationWindow = new OperationWindow( getWindow.HWND );

        }
    }
}
