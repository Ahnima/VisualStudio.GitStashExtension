﻿namespace VisualStudio.GitStashExtension.Models
{
    public class ChangedFile
    {
        /// <summary>
        /// File path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Flag indicates whether this file is new (untracked) or not.
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// Flag indicates whether file was staged before the stash or not.
        /// </summary>
        public bool IsStaged { get; set; }
    }
}
