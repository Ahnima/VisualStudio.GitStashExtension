﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using VisualStudio.GitStashExtension.Annotations;

namespace VisualStudio.GitStashExtension.Models
{
    /// <summary>
    /// Represents treeview item viewmodel. Contains file text and icon.
    /// </summary>
    public class TreeViewItemWithIcon: INotifyPropertyChanged
    {
        private BitmapSource _source;

        /// <summary>
        /// File or folder name for treeview.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Full path for current file or folder.
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// Icon source for file or folder.
        /// </summary>
        public BitmapSource Source
        {
            get => _source;
            set
            {
                _source = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicates whether folder is expanded or not.
        /// </summary>
        public bool IsExpanded { get; set; }

        /// <summary>
        /// Indicates whether current item is file.
        /// </summary>
        public bool IsFile { get; set; }

        /// <summary>
        /// Child items.
        /// </summary>
        public IList<TreeViewItemWithIcon> Items { get; set; }

        /// <summary>
        /// Flag indicates whether this file is new (untracked) or not.
        /// </summary>
        public bool? IsNew { get; set; }

        /// <summary>
        /// Flag indicates whether this file is new (untracked) and staged or not.
        /// </summary>
        public bool? IsStaged { get; set; }

        /// <summary>
        /// Context menu header text for file comparing/opening.
        /// </summary>
        public string ContextMenuText => IsNew ?? false ? "Open" : "Compare with previous";

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
