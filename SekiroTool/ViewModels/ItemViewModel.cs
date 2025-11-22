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
    
    private readonly ObservableCollection<Skill> _skillSearchResults = new ObservableCollection<Skill>();
    private ObservableCollection<Skill> _allSkills;

    public ItemViewModel(IItemService itemService, IStateService stateService)
    {
        _itemService = itemService;

        LoadItems();
        LoadSkills();

        stateService.Subscribe(State.Loaded, OnGameLoaded);
        stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);

        SpawnCommand = new DelegateCommand(SpawnItem);
        LearnSkillCommand = new DelegateCommand(LearnSkill);
        UnlearnSkillCommand = new DelegateCommand(UnlearnSkill);
    }

    
    #region Commands

    public ICommand SpawnCommand { get; set; }
    public ICommand LearnSkillCommand { get; set; }
    public ICommand UnlearnSkillCommand { get; set; }

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
    
    private string _skillSearchText = string.Empty;

    public string SkillSearchText
    {
        get => _skillSearchText;
        set
        {
            if (!SetProperty(ref _skillSearchText, value))
                return;

            if (string.IsNullOrEmpty(value))
            {
                Skills = _allSkills;
            }
            else
            {
                ApplySkillFilter();
            }
        }
    }
    private void ApplySkillFilter()
    {
        _skillSearchResults.Clear();
        var searchTextLower = SkillSearchText.ToLower();

        foreach (var skill in _allSkills)
        {
            if (skill.Name.ToLower().Contains(searchTextLower))
            {
                _skillSearchResults.Add(skill);
            }
        }
    
        Skills = _skillSearchResults;
    }
    
    private Skill _selectedSkill;

    public Skill SelectedSkill
    {
        get => _selectedSkill;
        set => SetProperty(ref _selectedSkill, value);
    }
    
    private ObservableCollection<Skill> _skills = new ObservableCollection<Skill>();
    public ObservableCollection<Skill> Skills
    {
        get => _skills;
        set => SetProperty(ref _skills, value);
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
    
    private void LoadItems()
    {
        Categories.Add("Goods");
        Categories.Add("Prosthetics");

        _itemsByCategory.Add("Goods",
            new ObservableCollection<Item>(DataLoader.GetItemList("Goods", (short)ItemType.Goods)));
        _itemsByCategory.Add("Prosthetics",
            new ObservableCollection<Item>(DataLoader.GetItemList("Prosthetics", (short)ItemType.Weapons)));
        
        _allItems = _itemsByCategory.Values.SelectMany(x => x).ToLookup(i => i.Name);
        
        SelectedCategory = Categories.FirstOrDefault();
    }
    
    private void LoadSkills()
    {
        _allSkills = new ObservableCollection<Skill>(DataLoader.GetSkillList());
        Skills = _allSkills;
    }
    
    private void SpawnItem()
    {
        if (SelectedItem.ItemType == (short) ItemType.Weapons)
        {
            _itemService.GiveSkillOrPros(SelectedItem.ItemId);
        }
        else
        {
            
            _itemService.SpawnItem(SelectedItem, 1);
        }
     
    }
    
    private void LearnSkill()
    {
        _itemService.GiveSkillOrPros(SelectedSkill.Id);
    }

    private void UnlearnSkill()
    {
        _itemService.RemoveItem(SelectedSkill.Id);
    }

    #endregion
}