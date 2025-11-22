using System.Collections.ObjectModel;
using System.Windows.Input;
using SekiroTool.Core;
using SekiroTool.Enums;
using SekiroTool.Interfaces;
using SekiroTool.Models;
using SekiroTool.Utilities;

namespace SekiroTool.ViewModels;

public class TravelViewModel : BaseViewModel
{
    private readonly ITravelService _travelService;
    private readonly HotkeyManager _hotkeyManager;
    private readonly IEventService _eventService;

    public TravelViewModel(ITravelService travelService, IStateService stateService,
        HotkeyManager hotkeyManager, IEventService eventService)
    {
        _travelService = travelService;
        _hotkeyManager = hotkeyManager;
        _eventService = eventService;
        
        RegisterHotkeys();

        stateService.Subscribe(State.Loaded, OnGameLoaded);
        stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);
        
        _mainAreas = new ObservableCollection<string>();
        _warpLocations = new ObservableCollection<Warp>();
        
        WarpCommand = new DelegateCommand(Warp);
        UnlockIdolsCommand = new DelegateCommand(UnlockIdols);
        
        LoadWarps();
        

        idolEventIds = DataLoader.GetIdolEventIds();
    }
    
    #region Private Fields

    private Dictionary<string, List<Warp>> _warpDict;
    private List<Warp> _allWarps;
    private string _preSearchMainArea;
    private readonly ObservableCollection<Warp> _searchResultsCollection = new ObservableCollection<Warp>();
    private List<long> idolEventIds;
    #endregion


    #region Commands

    public ICommand WarpCommand { get; set; }
    public ICommand UnlockIdolsCommand { get; set; }

    #endregion

    #region Properties

    private bool _areOptionsEnabled;
    public bool AreOptionsEnabled
    {
        get => _areOptionsEnabled;
        set => SetProperty(ref _areOptionsEnabled, value);
    }

    private ObservableCollection<string> _mainAreas;
    public ObservableCollection<string> MainAreas
    {
        get => _mainAreas;
        private set => SetProperty(ref _mainAreas, value);
    }

    private string _selectedMainArea;
    public string SelectedMainArea
    {
        get => _selectedMainArea;
        set
        {
            if (!SetProperty(ref _selectedMainArea, value)) return;

            if (_isSearchActive)
            {
                IsSearchActive = false;
                _searchText = string.Empty;
                OnPropertyChanged(nameof(SearchText));
                _preSearchMainArea = null;
            }

            UpdateLocationsList();
        }
    }

    private ObservableCollection<Warp> _warpLocations;
    public ObservableCollection<Warp> WarpLocations
    {
        get => _warpLocations;
        set => SetProperty(ref _warpLocations, value);
    }

    private Warp _selectedWarpLocation;
    public Warp SelectedWarpLocation
    {
        get => _selectedWarpLocation;
        set => SetProperty(ref _selectedWarpLocation, value);
    }

    private bool _isSearchActive;
    public bool IsSearchActive
    {
        get => _isSearchActive;
        private set => SetProperty(ref _isSearchActive, value);
    }

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (!SetProperty(ref _searchText, value)) return;

            if (string.IsNullOrEmpty(value))
            {
                _isSearchActive = false;

                if (_preSearchMainArea != null)
                {
                    _selectedMainArea = _preSearchMainArea;
                    UpdateLocationsList();
                    _preSearchMainArea = null;
                }
            }
            else
            {
                if (!_isSearchActive)
                {
                    _preSearchMainArea = SelectedMainArea;
                    _isSearchActive = true;
                }

                ApplyFilter();
            }
        }
    }

    #endregion

    #region Private Methods

    private void RegisterHotkeys()
    {
        
    }

    private void OnGameLoaded()
    {
        AreOptionsEnabled = true;
    }

    private void OnGameNotLoaded()
    {
        AreOptionsEnabled = false;
    }

    private void LoadWarps()
    {
        _warpDict = DataLoader.GetWarpLocations();
        _allWarps = _warpDict.Values.SelectMany(x => x).ToList();

        foreach (var area in _warpDict.Keys)
        {
            
            _mainAreas.Add(area);
        }
        
        SelectedMainArea = _mainAreas.FirstOrDefault();
    }

    private void UpdateLocationsList()
    {
        if (string.IsNullOrEmpty(SelectedMainArea) || !_warpDict.ContainsKey(SelectedMainArea))
        {
            WarpLocations = new ObservableCollection<Warp>();
            return;
        }

        WarpLocations = new ObservableCollection<Warp>(_warpDict[SelectedMainArea]);
        SelectedWarpLocation = WarpLocations.FirstOrDefault();
    }

    private void ApplyFilter()
    {
        _searchResultsCollection.Clear();
        var searchTextLower = SearchText.ToLower();

        foreach (var location in _allWarps)
        {
            if (location.Name.ToLower().Contains(searchTextLower) ||
                location.MainArea.ToLower().Contains(searchTextLower))
            {
                _searchResultsCollection.Add(location);
            }
        }

        WarpLocations = new ObservableCollection<Warp>(_searchResultsCollection);
        SelectedWarpLocation = WarpLocations.FirstOrDefault();
    }

    private void Warp() =>  _ = Task.Run(() => _travelService.Warp(SelectedWarpLocation));
    private void UnlockIdols() => idolEventIds.ForEach(id => _eventService.SetEvent(id, true));

    #endregion
}