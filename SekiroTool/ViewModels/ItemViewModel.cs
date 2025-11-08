using System.Collections.ObjectModel;
using System.Windows.Input;
using SekiroTool.Core;
using SekiroTool.Enums;
using SekiroTool.Interfaces;
using SekiroTool.Models;
using SekiroTool.Utilities;

namespace SekiroTool.ViewModels;

public class ItemViewModel : BaseViewModel
{
    private readonly IItemService _itemService;
    private ILookup<string, Item> _allItems;

    private readonly Dictionary<string, ObservableCollection<Item>> _itemsByCategory =
        new Dictionary<string, ObservableCollection<Item>>();

    private readonly ObservableCollection<Item> _searchResultsCollection = new ObservableCollection<Item>();
    private string _preSearchCategory;
    

    public ItemViewModel(IItemService itemService, IGameStateService gameStateService)
    {
        _itemService = itemService;

        LoadItems();

        gameStateService.Subscribe(GameState.Loaded, OnGameLoaded);
        gameStateService.Subscribe(GameState.NotLoaded, OnGameNotLoaded);

        SpawnCommand = new DelegateCommand(SpawnItem);
    }

    private void LoadItems()
    {
        Categories.Add("Goods");
        Categories.Add("Prosthetics");
        Categories.Add("Protector");

        _itemsByCategory.Add("Goods",
            new ObservableCollection<Item>(DataLoader.GetItemList("Goods", (short)ItemType.Goods)));
        _itemsByCategory.Add("Prosthetics",
            new ObservableCollection<Item>(DataLoader.GetItemList("Prosthetics", (short)ItemType.Weapons)));
        // _itemsByCategory.Add("Protector",
        //     new ObservableCollection<Item>(DataLoader.GetItemList("Protector", (short)ItemType.Protector)));
        
        _allItems = _itemsByCategory.Values.SelectMany(x => x).ToLookup(i => i.Name);
        
        SelectedCategory = Categories.FirstOrDefault();
    }


    #region Commands

    public ICommand SpawnCommand { get; set; }

    #endregion

    #region Properties

    private bool _areOptionsEnabled;

    public bool AreOptionsEnabled
    {
        get => _areOptionsEnabled;
        set => SetProperty(ref _areOptionsEnabled, value);
    }

    private ObservableCollection<string> _categories = new ObservableCollection<string>();

    public ObservableCollection<string> Categories
    {
        get => _categories;
        private set => SetProperty(ref _categories, value);
    }

    private ObservableCollection<Item> _items = new ObservableCollection<Item>();

    public ObservableCollection<Item> Items
    {
        get => _items;
        set => SetProperty(ref _items, value);
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
            if (!SetProperty(ref _searchText, value))
            {
                return;
            }

            if (string.IsNullOrEmpty(value))
            {
                _isSearchActive = false;

                if (_preSearchCategory != null)
                {
                    _selectedCategory = _preSearchCategory;
                    Items = _itemsByCategory[_selectedCategory];
                    SelectedItem = Items.FirstOrDefault();
                    _preSearchCategory = null;
                }
            }
            else
            {
                if (!_isSearchActive)
                {
                    _preSearchCategory = SelectedCategory;
                    _isSearchActive = true;
                }

                ApplyFilter();
            }
        }
    }
    
    private void ApplyFilter()
    {
        _searchResultsCollection.Clear();
        var searchTextLower = SearchText.ToLower();

        foreach (var category in _itemsByCategory)
        {
            foreach (var item in category.Value)
            {
                if (item.Name.ToLower().Contains(searchTextLower))
                {
                    item.Category = category.Key;
                    _searchResultsCollection.Add(item);
                }
            }
        }
        Items = _searchResultsCollection;
    }
    
    private int _selectedQuantity = 1;

    public int SelectedQuantity
    {
        get => _selectedQuantity;
        set
        {
            int clampedValue = Math.Max(1, Math.Min(value, MaxQuantity));
            SetProperty(ref _selectedQuantity, clampedValue);
        }
    }
    
    private int _maxQuantity;

    public int MaxQuantity
    {
        get => _maxQuantity;
        private set => SetProperty(ref _maxQuantity, value);
    }
    
    private bool _quantityEnabled;

    public bool QuantityEnabled
    {
        get => _quantityEnabled;
        private set => SetProperty(ref _quantityEnabled, value);
    }
    
    private string _selectedCategory;

    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (!SetProperty(ref _selectedCategory, value) || value == null) return;
            if (_selectedCategory == null) return;

            if (_isSearchActive)
            {
                IsSearchActive = false;
                _searchText = string.Empty;
                OnPropertyChanged(nameof(SearchText));
                _preSearchCategory = null;
            }

            Items = _itemsByCategory[_selectedCategory];
            SelectedItem = Items.FirstOrDefault();
     
        }
        
    }
    
    private Item _selectedItem;

    public Item SelectedItem
    {
        get => _selectedItem;
        set
        {
            SetProperty(ref _selectedItem, value);
            if (_selectedItem == null) return;

            QuantityEnabled = _selectedItem.StackSize > 1;
            MaxQuantity = _selectedItem.StackSize;
            SelectedQuantity = _selectedItem.StackSize;
          
        }
    }

    #endregion

    #region Private Methods

    private void OnGameLoaded()
    {
        AreOptionsEnabled = true;
    }

    private void OnGameNotLoaded()
    {
        AreOptionsEnabled = false;
    }
    
    private void SpawnItem()
    {
        if (SelectedItem.ItemType == (short) ItemType.Weapons)
        {
            _itemService.SpawnProsthetic(SelectedItem.ItemId);
        }
        else
        {
            
            _itemService.SpawnItem(SelectedItem, 1);
        }
     
    }

    #endregion
}