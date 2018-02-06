﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VisualStudio.GitStashExtension.Models;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;
using System.Threading.Tasks;

namespace VisualStudio.GitStashExtension.GitHelpers
{
    /// <summary>
    /// Represents service for executing git commands on current repositoty.
    /// </summary>
    public class GitCommandExecuter
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IGitExt _gitService;

        public GitCommandExecuter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _gitService = (IGitExt)_serviceProvider.GetService(typeof(IGitExt));
        }

        /// <summary>
        /// Gets all stashes for current repositoty.
        /// </summary>
        /// <param name="stashes">List of stashes for current repositoty.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>Bool value that indicates whether command execution was succeeded.</returns>
        public bool TryGetAllStashes(out IList<Stash> stashes, out string errorMessage)
        {
            var commandResult = Execute(GitCommandConstants.StashList);
            if (commandResult.IsError)
            {
                errorMessage = commandResult.ErrorMessage;
                stashes = null;
                return false;
            }

            stashes = GitResultParser.ParseStashListResult(commandResult.OutputMessage);
            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// Applies stash to current repository state by stash id.
        /// </summary>
        /// <param name="stashId">Stash Id.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>Bool value that indicates whether command execution was succeeded.</returns>
        public bool TryApplyStash(int stashId, out string errorMessage)
        {
            var applyCommand = string.Format(GitCommandConstants.StashApplyFormatted, stashId);
            var commandResult = Execute(applyCommand);

            errorMessage = commandResult.ErrorMessage;
            return !commandResult.IsError;
        }

        /// <summary>
        /// Creates stash on current branch.
        /// </summary>
        /// <param name="message">Save message for stash.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>Bool value that indicates whether command execution was succeeded.</returns>
        public bool TryCreateStash(string message, out string errorMessage)
        {
            var createCommand = string.IsNullOrEmpty(message) ? 
                GitCommandConstants.Stash : 
                string.Format(GitCommandConstants.StashSaveFormatted, message);

            var commandResult = Execute(createCommand);

            errorMessage = commandResult.ErrorMessage;
            return !commandResult.IsError;
        }

        /// <summary>
        /// Delete stash by id.
        /// </summary>
        /// <param name="id">Stash id.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>Bool value that indicates whether command execution was succeeded.</returns>
        public bool TryDeleteStash(int id, out string errorMessage)
        {
            var deleteCommand = string.Format(GitCommandConstants.StashDeleteFormatted, id);

            var commandResult = Execute(deleteCommand);

            errorMessage = commandResult.ErrorMessage;
            return !commandResult.IsError;
        }

        /// <summary>
        /// Gets stash info by id.
        /// </summary>
        /// <param name="id">Stash id.</param>
        /// <param name="stash">Stash model.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>Bool value that indicates whether command execution was succeeded.</returns>
        public bool TryGetStashInfo(int id, out Stash stash, out string errorMessage)
        {
            var infoCommand = string.Format(GitCommandConstants.StashInfoFormatted, id);

            var commandResult = Execute(infoCommand);

            if (commandResult.IsError)
            {
                errorMessage = commandResult.ErrorMessage;
                stash = null;
                return false;
            }

            errorMessage = string.Empty;
            stash = GitResultParser.ParseStashInfoResult(commandResult.OutputMessage);
            return true;
        }

        /// <summary>
        /// Runs file diff.
        /// </summary>
        /// <param name="id">Stash id.</param>
        /// <param name="filePath">File project full path.</param>
        /// <returns>Bool value that indicates whether command execution was succeeded.</returns>
        public Task<GitCommandResult> TryRunFileDiff(int id, string filePath)
        {
            var diffCommand = string.Format(GitCommandConstants.StashFileDiffFormatted, id, filePath);

            var commandResult = ExecuteAsync(diffCommand);

            return commandResult;
        }

        private GitCommandResult Execute(string gitCommand)
        {
            try
            {
                var activeRepository = _gitService.ActiveRepositories.FirstOrDefault();
                if (activeRepository == null)
                    return new GitCommandResult {ErrorMessage = "Select repository to find stashes."};

                var gitStartInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    FileName = GitPathHelper.GetGitPath(),
                    Arguments = gitCommand,
                    WorkingDirectory = activeRepository.RepositoryPath
                };

                using (var gitProcess = Process.Start(gitStartInfo))
                {
                    var errorMessage = Task.Run(() => gitProcess.StandardError.ReadToEndAsync()); 
                    var outputMessage = Task.Run(() => gitProcess.StandardOutput.ReadToEndAsync());

                    gitProcess.WaitForExit();

                    return new GitCommandResult
                    {
                        OutputMessage = outputMessage.Result,
                        ErrorMessage = errorMessage.Result
                    };
                }
            }
            catch
            {
                return new GitCommandResult { ErrorMessage = "Unexpected error." };
            }
        }


        private Task<GitCommandResult> ExecuteAsync(string gitCommand)
        {
            try
            {
                var activeRepository = _gitService.ActiveRepositories.FirstOrDefault();
                if (activeRepository == null)
                    return Task.FromResult(new GitCommandResult { ErrorMessage = "Select repository to find stashes." });

                var gitStartInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    FileName = GitPathHelper.GetGitPath(),
                    Arguments = gitCommand,
                    UseShellExecute = false,
                    WorkingDirectory = activeRepository.RepositoryPath
                };

                var tastCompletionSource = new TaskCompletionSource<GitCommandResult>();

                var gitProcess = new Process
                {
                    StartInfo = gitStartInfo,
                    EnableRaisingEvents = true
                };

                gitProcess.Exited += (sender, args) =>
                {
                    if (gitProcess.ExitCode != 0)
                    {
                        var error = gitProcess.StandardError.ReadToEnd();
                        tastCompletionSource.SetResult(new GitCommandResult
                        {
                            ErrorMessage = error
                        });
                    }
                    else
                    {
                        var output = gitProcess.StandardOutput.ReadToEnd();
                        tastCompletionSource.SetResult(new GitCommandResult
                        {
                            OutputMessage = output
                        });
                    }
                };

                gitProcess.Start();

                return tastCompletionSource.Task;

            }
            catch
            {
                return Task.FromResult(new GitCommandResult { ErrorMessage = "Unexpected error." });
            }
        }
    }
}
