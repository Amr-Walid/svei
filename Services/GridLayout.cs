namespace SVEI.Web.Services
{
    /// <summary>Layout maths for fixed-count grids such as the stat strips.</summary>
    public static class GridLayout
    {
        /// <summary>
        /// Pick a column count for <paramref name="count"/> items that avoids a
        /// lonely last row.
        ///
        /// The stat strips used to rely on <c>repeat(auto-fit, minmax(230px, 1fr))</c>,
        /// which silently wrapped the 5th stat onto a row of its own. Simply
        /// capping the columns is not enough either: 6 items in 5 columns leaves
        /// one stranded item on the second row, which looks just as broken.
        ///
        /// So prefer the widest layout that fits on a single row, and when the
        /// list is too long for that, choose the divisor that fills every row
        /// evenly (6 -> 3+3 rather than 5+1).
        /// </summary>
        /// <param name="count">Number of items to lay out.</param>
        /// <param name="max">Most columns the design stays readable at.</param>
        public static int Columns(int count, int max = 5)
        {
            if (count <= 0) return 1;
            if (count <= max) return count;              // fits on one row

            // First choice: the widest exact divisor, so every row is completely
            // full (6 -> 3+3). A ragged final row still reads as unfinished even
            // when it holds two items, so an even split wins outright.
            for (var cols = max; cols >= 2; cols--)
            {
                if (count % cols == 0) return cols;
            }

            // No exact divisor (7, 11, …): settle for the widest width whose final
            // row holds at least two items, so nothing is ever stranded alone.
            for (var cols = max; cols >= 2; cols--)
            {
                if (count % cols >= 2) return cols;
            }
            return max;
        }
    }
}
