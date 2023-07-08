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
      new LinesWin ().Show ();
      new Application ().Run ();
   }

   [STAThread]
   static void Main1 () {
      // Create a MandelWin that shows an animated Mandelbrot set,
      // and create an Application object to do message-pumping and keep
      // the window alive
      new MandelWin ().Show ();
      new Application ().Run ();
   }

   public static string DataDir {
      get {
         if (string.IsNullOrEmpty (mDataDir)) {
            Assembly assy = Assembly.GetExecutingAssembly ();
            var bindir = Path.GetDirectoryName (assy.Location);
            mDataDir = Path.GetFullPath (bindir + "/../data").Replace ('\\', '/');
         }
         return mDataDir;
      }
   }
   static string mDataDir = ""; 
}
