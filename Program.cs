// Program.cs - Entry point into the GrayBMP application   
// ---------------------------------------------------------------------------------------
using System.IO;
using System.Reflection;
using System.Windows;
namespace GrayBMP;

class Program {
   [STAThread]
   static void Main () {
      // Create a LinesWin that demonstrates the Line Drawing
      Init ();
      new LinesWin ().Show ();
      new Application ().Run ();
   }

   [STAThread]
   static void Main1 () {
      // Create a MandelWin that shows an animated Mandelbrot set,
      // and create an Application object to do message-pumping and keep
      // the window alive
      Init ();
      new MandelWin ().Show ();
      new Application ().Run ();
   }

   static void Init () {
      Assembly assy = Assembly.GetExecutingAssembly ();
      var bindir = Path.GetDirectoryName (assy.Location);
      DataDir = Path.GetFullPath (bindir + "/../data").Replace ('\\', '/');
   }

   public static string DataDir = "";
}
