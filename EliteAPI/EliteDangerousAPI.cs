﻿using EliteAPI.Bindings;
using EliteAPI.Discord;
using Newtonsoft.Json;
using Somfic.Logging;
using Somfic.Version;
using Somfic.Version.Github;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using EliteAPI.Events;
using EliteAPI.Status;
using EliteAPI.Status.Cargo;
using EliteAPI.Status.Market;
using EliteAPI.Status.Modules;
using EliteAPI.Status.Outfitting;
using EliteAPI.Status.Ship;
using EliteAPI.Status.Shipyard;
using EventHandler = System.EventHandler;

namespace EliteAPI
{
    //Credits to DarkWanderer for this fix.

    /// <summary>
    /// Main EliteAPI class.
    /// </summary>
    public class EliteDangerousAPI : IEliteDangerousAPI
    {
        /// <summary>
        /// The standard Directory of the Player Journal files (C:\Users\%username%\Saved Games\Frontier Developments\Elite Dangerous).
        /// </summary>
        public static DirectoryInfo StandardDirectory
        {
            get
            {
                // Don't try to find the special folder if not on Windows
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return new DirectoryInfo(Directory.GetCurrentDirectory());
                }

                int result = UnsafeNativeMethods.SHGetKnownFolderPath(new Guid("4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4"), 0, new IntPtr(0), out IntPtr path);
                if (result >= 0)
                {
                    try { return new DirectoryInfo(Marshal.PtrToStringUni(path) + "/Frontier Developments/Elite Dangerous"); }
                    catch { return new DirectoryInfo(Directory.GetCurrentDirectory()); }
                }
                else
                {
                    return new DirectoryInfo(Directory.GetCurrentDirectory());
                }
            }
        }

        /// <summary>
        /// The version of EliteAPI.
        /// </summary>
        public static string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// Whether the API is ready.
        /// </summary>
        public bool IsReady { get; internal set; }

        /// <summary>
        /// Whether the API is currently running.
        /// </summary>
        public bool IsRunning { get; private set; } = false;

        /// <summary>
        /// The Journal directory that is being used by the API.
        /// </summary>
        public DirectoryInfo JournalDirectory { get; private set; }

        public Events.EventHandler Events { get; internal set; }

        public CargoStatus Cargo { get; internal set; }

        public MarketStatus Market { get; internal set; }

        public ModulesStatus Modules { get; internal set; }

        public OutfittingStatus Outfitting { get; internal set; }

        public ShipStatus Status { get; internal set; }

        public ShipyardStatus Shipyard { get; internal set; }

        /// <summary>
        /// Holds information on all key bindings in the game set by the user.
        /// </summary>
        public UserBindings Bindings
        {
            get
            {
                try
                {
                    string wantedFile = FileReader.ReadAllText($@"C:\Users\{Environment.UserName}\AppData\Local\Frontier Developments\Elite Dangerous\Options\Bindings\StartPreset.start") + ".binds";
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(wantedFile);
                    return JsonConvert.DeserializeObject<UserBindings>(JsonConvert.SerializeXmlNode(xml));
                }
                catch { return new UserBindings(); }
            }
        }

        /// <summary>
        /// Rich presence service for Discord.
        /// </summary>
        public RichPresenceClient DiscordRichPresence { get; internal set; }

        /// <summary>
        /// Whether the API should skip the processing of previous events before the API was started.
        /// </summary>
        public bool SkipCatchUp { get; internal set; }

        internal JournalParser JournalParser { get; set; }

        /// <summary>
        /// Creates a new EliteDangerousAPI object using the standard Journal directory.
        /// </summary>
        public EliteDangerousAPI()
        {
            //Set the fields to the parameters.
            JournalDirectory = StandardDirectory;
            SkipCatchUp = false;
        }

        /// <summary>
        /// Creates a new EliteDangerousAPI object.
        /// </summary>
        /// <param name="JournalDirectory">The directory in which the Player Journals are located.</param>
        public EliteDangerousAPI(DirectoryInfo JournalDirectory)
        {
            //Set the fields to the parameters.
            this.JournalDirectory = JournalDirectory;
            SkipCatchUp = false;
        }

        /// <summary>
        /// Creates a new EliteDangerousAPI object using the standard Journal directory.
        /// </summary>
        /// <param name="SkipCatchUp">Whether the API should skip the processing of previous events before the API was started.</param>
        public EliteDangerousAPI(bool SkipCatchUp)
        {
            //Set the fields to the parameters.
            JournalDirectory = StandardDirectory;
            this.SkipCatchUp = SkipCatchUp;
        }

        /// <summary>
        /// Creates a new EliteDangerousAPI object.
        /// </summary>
        /// <param name="journalDirectory">The directory in which the Player Journals are located.</param>
        /// <param name="skipCatchUp">Whether the API should skip the processing of previous events before the API was started.</param>
        public EliteDangerousAPI(DirectoryInfo journalDirectory, bool skipCatchUp)
        {
            //Set the fields to the parameters.
            JournalDirectory = journalDirectory;
            SkipCatchUp = skipCatchUp;
        }

        /// <summary>
        /// Checks for a new update.
        /// </summary>
        /// <returns>Returns true if a newer version is available.</returns>
        private bool CheckForUpdate()
        {
            Logger.Log(Severity.Debug, "Checking for updates from GitHub.");

            try
            {
                VersionController version = new GithubVersionControl("EliteAPI", "EliteAPI");


                Logger.Log(Severity.Debug, $"Latest version: {version.Latest} (curr. {version.This}).");

                if (version.This < version.Latest)
                {
                    Logger.Log($"A new update ({version.Latest}) is available. Visit github.com/EliteAPI/EliteAPI to download the latest version.");

                    return true;
                }

                Logger.Debug("EliteAPI is up-to-date with the latest version.");
            }
            catch (Exception ex)
            {
                Logger.Warning("Could not check for updates.", ex);
            }

            return false;
        }

        /// <summary>
        /// Resets the API.
        /// </summary>
        public void Reset()
        {
            //Reset services.
            try { StatusReader.Hook<CargoStatus>(Cargo, Path.Combine(JournalDirectory.FullName, "Cargo.json")); } catch (Exception ex) { Logger.Warning("Couldn't instantiate service CargoWatcher.", ex);}
            try { StatusReader.Hook<ShipStatus>(Status, Path.Combine(JournalDirectory.FullName, "Status.json")); } catch (Exception ex) { Logger.Warning("Couldn't instantiate service CargoWatcher.", ex);}
            try { Events = new Events.EventHandler(); } catch (Exception ex) { Logger.Warning("Couldn't instantiate service 'Events'.", ex); }
            try { DiscordRichPresence = new RichPresenceClient(this); } catch (Exception ex) { Logger.Warning("Couldn't instantiate service 'DiscordRichPresence'.", ex); }
            try { JournalParser = new JournalParser(this); } catch (Exception ex) { Logger.Warning("Couldn't instantiate service 'JournalParser'.", ex); }
            JournalParser.processedLogs = new List<string>();

            OnReset?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Starts the API.
        /// </summary>
        public void Start(bool runRichPresence = true)
        {
            OnError += (sender, e) =>
            {
                Logger.Log(Severity.Error, e.Item1, e.Item2);
                Logger.Log(Severity.Warning, "EliteAPI stumbled upon a critical error and cannot continue.", new Exception("ELITEAPI TERMINATED"));
            };
            OnReady += (sender, e) => Logger.Log(Severity.Success, "EliteAPI is ready.");

            Stopwatch s = new Stopwatch();
            s.Start();

            Logger.Log("Starting EliteAPI.");
            Logger.Log(Severity.Debug, "EliteAPI by CMDR Somfic (discord.gg/jwpFUPZ) (github.com/EliteAPI/EliteAPI).");
            Logger.Log(Severity.Debug, $"EliteAPI v{Version}.");

            //Check for updates.
            CheckForUpdate();

            Logger.Log(Severity.Debug, "Checking journal directory.");

            if (!Directory.Exists(JournalDirectory.FullName))
            {
                if (JournalDirectory.FullName != StandardDirectory.FullName)
                {
                    Logger.Log(Severity.Warning, $"{JournalDirectory.Name} does not exist.",
                        new DirectoryNotFoundException($"'{JournalDirectory.FullName}' could not be found."));
                    Logger.Log(Severity.Debug, "Trying standard journal directory instead.");
                }

                if (!Directory.Exists(StandardDirectory.FullName))
                {
                    OnError?.Invoke(this,
                        new Tuple<string, Exception>(
                            "The default journal directory does not exist on this machine. This error usually occurs when Elite: Dangerous hasn't been run on this machine yet.",
                            new DirectoryNotFoundException($"'{StandardDirectory.FullName}' could not be found.")));

                    return;
                }

                JournalDirectory = StandardDirectory;
            }

            Logger.Log($"Journal directory set to '{JournalDirectory}'.");

            //Mark the API as running.
            IsRunning = true;

            //We'll process the journal one time first, to catch up.
            //Select the last edited Journal file.
            FileInfo journalFile;

            //Find the last edited Journal file.
            try
            {
                Logger.Log(Severity.Debug, "Searching for 'Journal.*.log' files.");
                journalFile = JournalDirectory.GetFiles("Journal.*").OrderByDescending(x => x.LastWriteTime).First();
                Logger.Log(Severity.Debug, $"Found '{journalFile}'.");
            }
            catch (Exception ex)
            {
                IsRunning = false;
                OnError?.Invoke(this, new Tuple<string, Exception>($"Could not find Journal files in '{JournalDirectory}'.", ex));

                return;
            }

            //Check for the support JSON files.
            bool foundStatus = false;
            Logger.Log(Severity.Debug, "Checking support files.");

            try
            {
                //Status.json.
                if (File.Exists(Path.Combine(JournalDirectory.FullName, "Status.json")))
                {
                    Logger.Log(Severity.Debug, "Found 'Status.json'.");
                    foundStatus = true;
                }
                else
                {
                    Logger.Log(Severity.Warning, "Could not find 'Status.json' file.");
                    foundStatus = false;
                }

                //Cargo.json.
                Logger.Log(Severity.Debug, File.Exists(Path.Combine(JournalDirectory.FullName, "Cargo.json"))
                    ? "Found 'Cargo.json'."
                    : "Could not find 'Cargo.json' file.");

                //Shipyard.json.
                Logger.Log(Severity.Debug, File.Exists(Path.Combine(JournalDirectory.FullName, "Shipyard.json"))
                    ? "Found 'Shipyard.json'."
                    : "Could not find 'Shipyard.json' file.");

                //Outfitting.json.
                Logger.Log(Severity.Debug, File.Exists(Path.Combine(JournalDirectory.FullName, "Outfitting.json"))
                    ? "Found 'Outfitting.json'."
                    : "Could not find 'Outfitting.json' file.");

                //Market.json.
                Logger.Log(Severity.Debug, File.Exists(Path.Combine(JournalDirectory.FullName, "Market.json"))
                    ? "Found 'Market.json'."
                    : "Could not find 'Market.json' file.");

                //ModulesInfo.json.
                Logger.Log(Severity.Debug, File.Exists(Path.Combine(JournalDirectory.FullName, "ModulesInfo.json"))
                    ? "Found 'ModulesInfo.json'."
                    : "Could not find 'ModulesInfo.json' file.");
            }
            catch (Exception ex)
            {
                Logger.Log(Severity.Error, "Could not check for support files.", ex);
            }

            if (foundStatus)
            {
                Logger.Log("Found Journal and Status files.");
            }
            else
            {
                Logger.Log(Severity.Error, "Could not find Status.json.", new FileNotFoundException("This error usually occurs when Elite: Dangerous hasn't been run on this machine yet.", $"{Path.Combine(JournalDirectory.FullName, "Status.json")}"));
                Logger.Log("Live updates, such as the landing gear & hardpoints, are not supported without access to 'Status.json'. The Status file is only created after the first run of Elite: Dangerous. If this is not the first time you're running Elite: Dangerous on this machine, change the journal directory.");
                Logger.Log(Severity.Warning, "A critical part of EliteAPI will be offline.", new Exception("PROCEEDING WITH LIMITED FUNCTIONALITY"));
                Logger.Log("Proceeding in 20 seconds ...");
                Thread.Sleep(20000);
            }

            Reset();

            //Check if Elite: Dangerous is running.
            if (!Status.IsRunning)
            {
                Logger.Log(Severity.Warning, "Elite: Dangerous is not in-game.");
            }

            //Process the journal file.
            if (!SkipCatchUp)
            {
                Logger.Log(Severity.Debug, "Catching up with past events from this session.");
            }

            JournalParser.ProcessJournal(journalFile, SkipCatchUp, false, true);

            if (!SkipCatchUp)
            {
                Logger.Log(Severity.Debug, "Catchup on past events completed.");
            }

            //Go async.
            Task.Run(() =>
            {
                //Run for as long as we're running.
                while (IsRunning)
                {
                    //Select the last edited Journal file.
                    FileInfo newJournalFile = JournalDirectory.GetFiles("Journal.*").OrderByDescending(x => x.LastWriteTime).First();

                    if (journalFile.FullName != newJournalFile.FullName)
                    {
                        Logger.Log(Severity.Info, $"Switched to '{newJournalFile}'.");
                        JournalParser.processedLogs.Clear();
                    }

                    journalFile = newJournalFile;

                    //Process the journal file.
                    JournalParser.ProcessJournal(journalFile, false);

                    //Wait half a second to avoid overusing the CPU.
                    Thread.Sleep(500);
                }
            });

            s.Stop();

            Logger.Log(Severity.Debug, $"Finished in {s.ElapsedMilliseconds}ms.");
            IsReady = true;
            OnReady?.Invoke(this, EventArgs.Empty);

            if (runRichPresence)
            {
                //Start the Rich Presence.
                DiscordRichPresence.TurnOn();
            }
        }

        /// <summary>
        /// Changes the journal directory.
        /// </summary>
        /// <param name="newJournalDirectory">The new journal directory.</param>
        public void ChangeJournal(DirectoryInfo newJournalDirectory)
        {
            if (newJournalDirectory == JournalDirectory)
            {
                return;
            }

            JournalDirectory = newJournalDirectory;
            Logger.Log(Severity.Debug, $"Changed Journal directory to '{newJournalDirectory}'.");
        }

        /// <summary>
        /// Stops the API.
        /// </summary>
        public void Stop()
        {
            //Mark the API as not running.
            IsRunning = false;

            Logger.Log("EliteAPI has been terminated.");

            OnQuit?.Invoke(this, EventArgs.Empty);
        }

        internal void TriggerOnLoad(string message, float percentage)
        {
            OnLoad?.Invoke(this, new Tuple<string, float>(message, percentage));
        }

        /// <summary>
        /// Gets triggered when EliteAPI has successfully loaded up.
        /// </summary>
        public event EventHandler OnReady;

        /// <summary>
        /// Gets triggered when EliteAPI is closing.
        /// </summary>
        public event EventHandler OnQuit;

        /// <summary>
        /// Gets triggered when EliteAPI has been reset.
        /// </summary>
        public event EventHandler OnReset;

        /// <summary>
        /// Gets triggered when EliteAPI could not successfully load up.
        /// </summary>
        public event EventHandler<Tuple<string, Exception>> OnError;

        /// <summary>
        /// Gets triggered when EliteAPI is starting up.
        /// </summary>
        public event EventHandler<Tuple<string, float>> OnLoad;
    }
}
