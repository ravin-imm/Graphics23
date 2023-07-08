//// PolyFill.cs - Contains the PolyFill class which is mainly used to 
//// fill a given polygon
//// ---------------------------------------------------------------------------------------
namespace GrayBMP;

#region class PolyFill ---------------------------------------------------------
/// <summary>Class to read a given polygon and to fill it </summary>
class PolyFill {
   public void AddLine (int x0, int y0, int x1, int y1) {
      // We will try to keep Y0 as the smaller one always
      if (y0 > y1) { (y0, y1) = (y1, y0); (x0, x1) = (x1, x0); }
      mLines.Enqueue (new Line (x0, y0, x1, y1));
   }

   public void Fill (GrayBMP bmp, int color) {
      // Current active lines for the given scan and the intersections
      var (aLines, ixs) = (new List<Line> (), new List<int> ());
      for (int y = 0; y < bmp.Height; y++) {
         ixs.Clear ();
         var ys = y + 0.5;
         // Remove lines that are out of the scope of this scan
         for (int i = aLines.Count - 1; i >= 0; i--)
            if (aLines[i].Y1 < ys) aLines.RemoveAt (i);
         // Add lines that come into the scope of this scan
         while (true) {
            if (mLines.IsEmpty) break; // Nothing to add break;
            var ln = mLines.Dequeue ();
            if (ln.Y0 < ys) aLines.Add (ln);
            else { mLines.Enqueue (ln); break; }
         }
         if (!aLines.Any ()) continue; // No active lines, continue
         foreach (var ln in aLines) {
            if (ln.Y0 == ln.Y1) continue;
            ixs.Add ((int)(ln.X1 + ((double)(ln.X1 - ln.X0) / (ln.Y1 - ln.Y0)) * (ys - ln.Y1)));
         }
         var (nC, ya) = (ixs.Count, bmp.Height - y);
         if (nC % 2 != 0) throw new Exception ();
         ixs = ixs.Order ().ToList ();

         for (int i = 0; i < ixs.Count; i += 2)
            bmp.DrawLine (ixs[i], ya, ixs[i + 1], ya, color);
      }
   }

   PriorityQueue<Line> mLines = new ();
}

record Line (int X0, int Y0, int X1, int Y1) : IComparable<Line> {
   /// <summary>Compares two Lines</summary>
   public int CompareTo (Line b) {
      if (Y0 < b.Y0) return -1; if (Y0 > b.Y0) return 1;
      if (X0 < b.X0) return -1; if (X1 > b.X1) return 1;
      return 0;
   }
}
#endregion


