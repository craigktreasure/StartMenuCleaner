namespace StartMenuCleaner
{
    using System;
    using System.Collections.Generic;

    public class CleanerOptions
    {
        /// <summary>
        /// Gets the root folders to clean.
        /// </summary>
        public IReadOnlyList<string> RootFoldersToClean { get; }

        /// <summary>
        /// Gets a value indicating whether to simulate.
        /// </summary>
        public bool Simulate { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CleanerOptions"/> class.
        /// </summary>
        /// <param name="rootFoldersToClean">The contents of these foldres will be searched and cleaned.</param>
        public CleanerOptions(IReadOnlyList<string> rootFoldersToClean)
        {
            if (rootFoldersToClean is null)
            {
                throw new ArgumentNullException(nameof(rootFoldersToClean));
            }

            this.RootFoldersToClean = rootFoldersToClean;
        }
    }
}
