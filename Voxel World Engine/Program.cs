using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.Desktop;


namespace Voxel_World_Engine
{

    internal class Program
    {
        private static TestWindow window = new TestWindow(GameWindowSettings.Default, NativeWindowSettings.Default);

        static void Main(string[] args)
        {
            window.Run();
        }

    }
}
