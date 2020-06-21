﻿using EliteAPI.Bindings;
using EliteAPI.Status;
using Somfic.Logging;
using System;
using System.IO;
using EliteAPI.Service.Discord;
using EliteAPI.Status.Cargo;
using EliteAPI.Status.Market;
using EliteAPI.Status.Modules;
using EliteAPI.Status.Outfitting;
using EliteAPI.Status.Ship;
using EliteAPI.Status.Shipyard;

namespace EliteAPI
{
    /// <summary>
    /// Interface for the EliteDangerousAPI class.
    /// </summary>
    public interface IEliteDangerousAPI
    {
        /// <summary>
        /// Whether the API is currently running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// The Journal directory that is being used by the API.
        /// </summary>
        DirectoryInfo JournalDirectory { get; }

        /// <summary>
        /// Whether the API should skip the processing of previous events before the API was started.
        /// </summary>
        bool SkipCatchUp { get; }

        /// <summary>
        /// Object that holds all the events.
        /// </summary>
        Events.EventHandler Events { get; }

        /// <summary>
        /// Holds information on all key bindings in the game set by the user.
        /// </summary>
        UserBindings Bindings { get; }

        CargoStatus Cargo { get; }

         MarketStatus Market { get;}

         ModulesStatus Modules { get; }
         OutfittingStatus Outfitting { get; }

         ShipStatus Status { get; }

         ShipyardStatus Shipyard { get; }

        /// <summary>
        /// Gets triggered when EliteAPI could not successfully load up.
        /// </summary>
        event EventHandler<Tuple<string, Exception>> OnError;

        /// <summary>
        /// Gets triggered when EliteAPI is closing.
        /// </summary>
        event EventHandler OnQuit;

        /// <summary>
        /// Gets triggered when EliteAPI has successfully loaded up.
        /// </summary>
        event EventHandler OnReady;

        /// <summary>
        /// Rich presence service for Discord.
        /// </summary>
        DiscordService Discord { get; }

        /// <summary>
        /// Resets the API.
        /// </summary>
        void Reset();

        /// <summary>
        /// Starts the API.
        /// </summary>
        /// <param name="runRichPresence"></param>
        void Start(bool runRichPresence = true);

        /// <summary>
        /// Stops the API.
        /// </summary>
        void Stop();

        /// <summary>
        /// Changes the journal directory.
        /// </summary>
        /// <param name="newJournalDirectory"></param>
        void ChangeJournal(DirectoryInfo newJournalDirectory);
    }
}