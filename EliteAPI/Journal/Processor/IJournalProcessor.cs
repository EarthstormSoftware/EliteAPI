﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EliteAPI.Journal.Processor
{
    /// <summary>
    /// Reads the journal and status files
    /// </summary>
    public interface IJournalProcessor
    {
        /// <summary>
        /// Triggered when a new json entry is made in the active journal file
        /// </summary>
        event EventHandler<string> NewJournalEntry;

        /// <summary>
        /// Triggered when a support file is updated
        /// </summary>
        event EventHandler<(FileInfo, string)> NewSupportEntry;

        /// <summary>
        /// Hooks the specified journal file to <see cref="NewJournalEntry"/> and invokes <see cref="NewJournalEntry"/> when needed
        /// </summary>
        Task ProcessJournalFile(FileInfo journalFile);

        /// <summary>
        /// Hooks the specified support files to <see cref="NewSupportEntry"/> and invokes <see cref="NewSupportEntry"/> when needed
        /// </summary>
        Task ProcessSupportFiles(IEnumerable<FileInfo> supportFiles);
    }
}
