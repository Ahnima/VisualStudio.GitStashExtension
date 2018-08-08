﻿namespace VisualStudio.GitStashExtension.GitHelpers
{
    /// <summary>
    /// Represents container for git commmands.
    /// </summary>
    public class GitCommandConstants
    {
        public const string StashList = "stash list";

        public const string StashApplyFormatted = "stash apply stash@{{{0}}}";

        public const string Stash = "stash";

        public const string StashIncludeUntracked = "stash --include-untracked";

        public const string StashSaveFormatted = "stash save {0}";

        public const string StashSaveFormattedIncludeUntracked = "stash save --include-untracked {0}";

        public const string StashDeleteFormatted = "stash drop stash@{{{0}}}";

        public const string StashInfoFormatted = "stash show stash@{{{0}}} --name-only";

        public const string StashUntrackedInfoFormatted = "show stash@{{{0}}}^^3 --name-only --pretty=\"\"";

        public const string CatFileStashCheckUntrackedFilesExist = "cat-file -t stash@{{{0}}}^^3";

        public const string StashFileDiffFormatted = "difftool --trust-exit-code -y -x \"'{0}' //t\" stash@{{{1}}}^^ stash@{{{1}}} -- {2}";

        public const string AfterStashFileVersionSaveTempFormatted = "show stash@{{{0}}}:\"{1}\" > {2}";

        public const string BeforeStashFileVersionSaveTempFormatted = "show stash@{{{0}}}^^:\"{1}\" > {2}";
    }
}
