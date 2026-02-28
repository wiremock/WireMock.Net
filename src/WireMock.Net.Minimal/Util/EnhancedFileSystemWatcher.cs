// Copyright © WireMock.Net

using System.Collections.Concurrent;
using JetBrains.Annotations;
using Stef.Validation;

namespace WireMock.Util;

/// <summary>
/// An EnhancedFileSystemWatcher, which can be used to suppress duplicate events that fire on a single change to the file.
/// </summary>
/// <seealso cref="FileSystemWatcher" />
public class EnhancedFileSystemWatcher : FileSystemWatcher
{
    #region Private Members
    // Default Watch Interval in Milliseconds
    private const int DefaultWatchInterval = 100;

    // This Dictionary keeps the track of when an event occurred last for a particular file
    private ConcurrentDictionary<string, DateTime> _lastFileEvent = new();

    // Watch Interval in Milliseconds
    private int _interval;

    // Timespan created when interval is set
    private TimeSpan _recentTimeSpan;
    #endregion

    #region Public Properties
    /// <summary>
    /// Interval, in milliseconds, within which events are considered "recent".
    /// </summary>
    [PublicAPI]
    public int Interval
    {
        get => _interval;
        set
        {
            _interval = value;

            // Set timespan based on the value passed
            _recentTimeSpan = new TimeSpan(0, 0, 0, 0, value);
        }
    }

    /// <summary>
    /// Allows user to set whether to filter recent events.
    /// If this is set a false, this class behaves like System.IO.FileSystemWatcher class.
    /// </summary>
    [PublicAPI]
    public bool FilterRecentEvents { get; set; }
    #endregion

    #region Constructors        
    /// <summary>
    /// Initializes a new instance of the <see cref="EnhancedFileSystemWatcher"/> class.
    /// </summary>
    /// <param name="interval">The interval.</param>
    public EnhancedFileSystemWatcher(int interval = DefaultWatchInterval)
    {
        Guard.Condition(interval, i => i >= 0);

        InitializeMembers(interval);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnhancedFileSystemWatcher"/> class.
    /// </summary>
    /// <param name="path">The directory to monitor, in standard or Universal Naming Convention (UNC) notation.</param>
    /// <param name="interval">The interval.</param>
    public EnhancedFileSystemWatcher(string path, int interval = DefaultWatchInterval) : base(path)
    {
        Guard.NotNullOrEmpty(path);
        Guard.Condition(interval, i => i >= 0);

        InitializeMembers(interval);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnhancedFileSystemWatcher"/> class.
    /// </summary>
    /// <param name="path">The directory to monitor, in standard or Universal Naming Convention (UNC) notation.</param>
    /// <param name="filter">The type of files to watch. For example, "*.txt" watches for changes to all text files.</param>
    /// <param name="interval">The interval.</param>
    public EnhancedFileSystemWatcher(string path, string filter, int interval = DefaultWatchInterval) : base(path, filter)
    {
        Guard.NotNullOrEmpty(path);
        Guard.NotNullOrEmpty(filter);
        Guard.Condition(interval, i => i >= 0);

        InitializeMembers(interval);
    }
    #endregion

    #region Events
    // These events hide the events from the base class. 
    // We want to raise these events appropriately and we do not want the 
    // users of this class subscribing to these events of the base class accidentally

    /// <summary>
    /// Occurs when a file or directory in the specified <see cref="P:System.IO.FileSystemWatcher.Path" /> is changed.
    /// </summary>
    public new event FileSystemEventHandler? Changed;

    /// <summary>
    /// Occurs when a file or directory in the specified <see cref="P:System.IO.FileSystemWatcher.Path" /> is created.
    /// </summary>
    public new event FileSystemEventHandler? Created;

    /// <summary>
    /// Occurs when a file or directory in the specified <see cref="P:System.IO.FileSystemWatcher.Path" /> is deleted.
    /// </summary>
    public new event FileSystemEventHandler? Deleted;

    /// <summary>
    /// Occurs when a file or directory in the specified <see cref="P:System.IO.FileSystemWatcher.Path" /> is renamed.
    /// </summary>
    public new event RenamedEventHandler? Renamed;
    #endregion

    #region Protected Methods to raise the Events for this class        
    /// <summary>
    /// Raises the <see cref="E:System.IO.FileSystemWatcher.Changed" /> event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.IO.FileSystemEventArgs" /> that contains the event data.</param>
    protected new virtual void OnChanged(FileSystemEventArgs e)
    {
        Changed?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="E:System.IO.FileSystemWatcher.Created" /> event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.IO.FileSystemEventArgs" /> that contains the event data.</param>
    protected new virtual void OnCreated(FileSystemEventArgs e)
    {
        Created?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="E:System.IO.FileSystemWatcher.Deleted" /> event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.IO.FileSystemEventArgs" /> that contains the event data.</param>
    protected new virtual void OnDeleted(FileSystemEventArgs e)
    {
        Deleted?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="E:System.IO.FileSystemWatcher.Renamed" /> event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.IO.RenamedEventArgs" /> that contains the event data.</param>
    protected new virtual void OnRenamed(RenamedEventArgs e)
    {
        Renamed?.Invoke(this, e);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// This Method Initializes the private members.
    /// Interval is set to its default value of 100 millisecond.
    /// FilterRecentEvents is set to true, _lastFileEvent dictionary is initialized.
    /// We subscribe to the base class events.
    /// </summary>
    private void InitializeMembers(int interval = 100)
    {
        Interval = interval;
        FilterRecentEvents = true;
        _lastFileEvent = new ConcurrentDictionary<string, DateTime>();

        base.Created += OnCreated;
        base.Changed += OnChanged;
        base.Deleted += OnDeleted;
        base.Renamed += OnRenamed;
    }

    /// <summary>
    /// This method searches the dictionary to find out when the last event occurred 
    /// for a particular file. If that event occurred within the specified timespan
    /// it returns true, else false
    /// </summary>
    /// <param name="fileName">The filename to be checked</param>
    /// <returns>True if an event has occurred within the specified interval, False otherwise</returns>
    private bool HasAnotherFileEventOccurredRecently(string fileName)
    {
        // Check dictionary only if user wants to filter recent events otherwise return value stays false.
        if (!FilterRecentEvents)
        {
            return false;
        }

        bool retVal = false;
        if (_lastFileEvent.ContainsKey(fileName))
        {
            // If dictionary contains the filename, check how much time has elapsed
            // since the last event occurred. If the timespan is less that the 
            // specified interval, set return value to true 
            // and store current datetime in dictionary for this file
            DateTime lastEventTime = _lastFileEvent[fileName];
            DateTime currentTime = DateTime.Now;
            TimeSpan timeSinceLastEvent = currentTime - lastEventTime;
            retVal = timeSinceLastEvent < _recentTimeSpan;
            _lastFileEvent[fileName] = currentTime;
        }
        else
        {
            // If dictionary does not contain the filename, 
            // no event has occurred in past for this file, so set return value to false
            // and append filename along with current datetime to the dictionary
            _lastFileEvent.TryAdd(fileName, DateTime.Now);
        }

        return retVal;
    }

    #region FileSystemWatcher EventHandlers
    // Base class Event Handlers. Check if an event has occurred recently and call method
    // to raise appropriate event only if no recent event is detected
    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (!HasAnotherFileEventOccurredRecently(e.FullPath))
        {
            OnChanged(e);
        }
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        if (!HasAnotherFileEventOccurredRecently(e.FullPath))
        {
            OnCreated(e);
        }
    }

    private void OnDeleted(object sender, FileSystemEventArgs e)
    {
        if (!HasAnotherFileEventOccurredRecently(e.FullPath))
        {
            OnDeleted(e);
        }
    }

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        if (!HasAnotherFileEventOccurredRecently(e.OldFullPath))
        {
            OnRenamed(e);
        }
    }
    #endregion
    #endregion
}