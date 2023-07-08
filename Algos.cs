//// Algos.cs - Contains various algorithms
//// ---------------------------------------------------------------------------------------

namespace GrayBMP;

/// <summary>Implements a generic priority queue using a heap data structure</summary>
public class PriorityQueue<T> where T : IComparable<T> {
   /// <summary>Adds an element to the collection.</summary>
   /// We add the element at the end and check if the heap property is maintained.
   /// If the heap property is violated do a sift-up operation till this property is retained.
   public void Enqueue (T item) {
      mTs.Add (item);
      // Do a sift-up operation till the heap property is retained
      int n = mTs.Count - 1;
      while (n > 1) {
         // Compare with the parent (Parent is at the n / 2 the index)
         if (mTs[n].CompareTo (mTs[n / 2]) <= 0) { (mTs[n], mTs[n / 2]) = (mTs[n / 2], mTs[n]); n /= 2; } else break;
      }
   }

   /// <summary>Gets the first element in the queue without removing it</summary>
   public T Peek () => mTs[1];

   /// <summary>Removes an element to the collection.</summary>
   /// As per the heap property the first element is always the lowest in the collection.
   /// We swap this with the last eleemnt and to maintain the heap property, we do 
   /// a sift-down till the property is retained.
   public T Dequeue () {
      var c = mTs.Count - 1;
      (mTs[1], mTs[^1]) = (mTs[^1], mTs[1]); // Swap the first and the last items
      var rem = mTs[^1]; mTs.RemoveAt (c);
      // Do a sift-down operation from the first element till the heap property is maintained
      int n = 1;
      while (true) { // Children are at 2 * n and 2 * n + 1 indices 
         int a = 2 * n; if (a >= c) break; // No children, break;
         if ((a + 1) < c && mTs[a + 1].CompareTo (mTs[a]) < 0) a++; // Take the index of the min of the children
         if (mTs[n].CompareTo (mTs[a]) > 0) { (mTs[n], mTs[a]) = (mTs[a], mTs[n]); n = a; } // Swap the elements and reset the index
         else break;
      }
      return rem;
   }

   public bool IsEmpty => mTs.Count == 1;

   public IEnumerable<T> Elems => mTs.Skip (1); // Get all the elements in the collection. This is primarily for testing the heap

   readonly List<T> mTs = new () { default }; // Contains the generic collection
}
