﻿using System;
using System.ComponentModel;
using Microsoft.TeamFoundation.Controls;
using VisualStudio.GitStashExtension.Models;
using VisualStudio.GitStashExtension.VS.UI;

namespace VisualStudio.GitStashExtension.TeamExplorerExtensions
{
    [TeamExplorerPage(Constants.StashInfoPageId)]
    public class StashInfoTeamExplorerPage: ITeamExplorerPage
    {
        private object _pageContent;
        private Stash _stashInfo;


        public StashInfoTeamExplorerPage()
        {
        }

        public void Dispose()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void Initialize(object sender, PageInitializeEventArgs e)
        {
            _pageContent = new StashInfoPage(e.Context as Stash);
            var changesSection = this.GetSection(new Guid(Constants.StashInfoChangesSectionId));
            _stashInfo = e.Context as Stash;
            changesSection.SaveContext(this, new SectionSaveContextEventArgs
            {
                Context = _stashInfo
            });
        }

        public void Loaded(object sender, PageLoadedEventArgs e)
        {
        }

        public void SaveContext(object sender, PageSaveContextEventArgs e)
        {
            e.Context = _stashInfo;
        }

        public void Refresh()
        {
        }

        public void Cancel()
        {
        }

        public object GetExtensibilityService(Type serviceType)
        {
            return null;
        }

        public string Title => Constants.StashesInfoLabel;
        public object PageContent => _pageContent;
        public bool IsBusy { get; }
    }
}
