using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ACCommunication;
using static ACCommunication.Authentification;
using System.Collections.ObjectModel;

namespace Client
{
    public class MainWindowViewModel : ViewModelBase
    {
        private int _selectedYear;
        public int SelectedYear
        {
            get
            {
                return _selectedYear;
            }
            set
            {
                if (_selectedYear != value)
                {
                    _selectedYear = value;
                    this.OnPropertyChanged("SelectedYear");
                }
            }
        }

        private int[] _years;
        public int[] Years
        {
            get
            {
                return _years;
            }
            set
            {
                if (_years != value)
                {
                    _years = value;
                    this.OnPropertyChanged("Years");
                }
            }
        }

        private SeasonsEnum _selectedSeason;
        public SeasonsEnum SelectedSeason
        {
            get
            {
                return _selectedSeason;
            }
            set
            {
                if (_selectedSeason != value)
                {
                    _selectedSeason = value;
                    this.OnPropertyChanged("SelectedSeason");
                }
            }
        }

        private SeasonsEnum[] _seasons;
        public SeasonsEnum[] Seasons
        {
            get
            {
                return _seasons;
            }
            set
            {
                if (_seasons != value)
                {
                    _seasons = value;
                    this.OnPropertyChanged("Seasons");
                }
            }
        }

        private bool _hasAuthenticated;
        public bool HasAuthenticated
        {
            get
            {
                return _hasAuthenticated;
            }
            set
            {
                if (_hasAuthenticated != value)
                {
                    _hasAuthenticated = value;
                    this.OnPropertyChanged("HasAuthenticated");
                    ((RelayCommand)this.BrowseCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private string _tokenErrorMsg;
        public string TokenErrorMsg
        {
            get
            {
                return _tokenErrorMsg;
            }
            set
            {
                if (_tokenErrorMsg != value)
                {
                    _tokenErrorMsg = value;
                    this.OnPropertyChanged("TokenErrorMsg");
                }
            }
        }

        private ObservableCollection<AnimeInfo> _results;
        public ObservableCollection<AnimeInfo> Results
        {
            get { return _results; }
            set
            {
                if (_results != value)
                {
                    _results = value;
                    this.OnPropertyChanged("Results");
                }
            }
        }

        private bool _isBrowseAllowed;
        public bool IsBrowseAllowed
        {
            get{ return _isBrowseAllowed; }
            set
            {
                if (_isBrowseAllowed != value)
                {
                    _isBrowseAllowed = value;
                    this.OnPropertyChanged("IsBrowseAllowed");
                    ((RelayCommand)this.BrowseCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private string _currentItemComment;
        public string CurrentItemComment
        {
            get { return _currentItemComment; }
            set
            {
                if (_currentItemComment != value)
                {
                    _currentItemComment = value;
                    this.OnPropertyChanged("CurrentItemComment");
                }
            }
        }

        private AnimeInfo _currentItem;
        public AnimeInfo CurrentItem
        {
            get { return this._currentItem; }
            set
            {
                if (_currentItem != value)
                {
                    this.StoreCurrentItemComment();
                    _currentItem = value;
                    this.OnPropertyChanged("CurrentItem");
                    this.FetchCurrentItemComment();
                }
            }
        }

        private bool _isSaveAllowed;
        public bool IsSaveAllowed
        {
            get { return _isSaveAllowed; }
            set
            {
                if (_isSaveAllowed != value)
                {
                    _isSaveAllowed = value;
                    this.OnPropertyChanged("IsSaveAllowed");
                    ((RelayCommand)this.SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private Dictionary<int, AnimeComment> _comments;

        private IAcService _service;
        private Authentification.AuthInfo _currentToken;
        public ICommand BrowseCommand { get; }
        public ICommand SaveCommand { get; }

        public MainWindowViewModel()
        {
            this.SelectedYear = DateTime.Now.Year;
            this.Years = this.GenerateLast20Years(this.SelectedYear).ToArray();
            this.Seasons = new SeasonsEnum[] { SeasonsEnum.WINTER, SeasonsEnum.SPRING, SeasonsEnum.SUMMER, SeasonsEnum.FALL };
            this.SelectedSeason = GetCurrentSeason();
            this.BrowseCommand = new RelayCommand(Browse, this.CanExecuteBrowseCommand);
            this.SaveCommand = new RelayCommand(Save, this.CanExecuteSaveCommand);
            this.IsBrowseAllowed = true;
            this.HasAuthenticated = false;
            this.Results = new ObservableCollection<AnimeInfo>();
            this.TokenErrorMsg = string.Empty;
            this._service = new AcService();
            this._comments = new Dictionary<int, AnimeComment>();

            var askTokenTask = this._service.AskToken();

            askTokenTask.ContinueWith(
                t => OnTokenReceived(t.Result),
                new System.Threading.CancellationToken(false),
                TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.FromCurrentSynchronizationContext()
            );

            askTokenTask.ContinueWith(
                t => OnTokenErrorReceived(),
                new System.Threading.CancellationToken(false),
                TaskContinuationOptions.NotOnRanToCompletion,
                TaskScheduler.FromCurrentSynchronizationContext()
            );

            this.ReadExistingComments();
        }

        private void ReadExistingComments()
        {
            var readTask = Task.Run(() => this._service.ReadComments())
                .ContinueWith(t => this.PopulateComments(t.Result));
        }

        private void PopulateComments(IEnumerable<Tuple<int, AnimeComment>> result)
        {
            foreach (var item in result)
            {
                _comments.Add(item.Item1, item.Item2);
            }
        }

        public bool CanExecuteBrowseCommand(object _)
        {
            return this.HasAuthenticated && this.IsBrowseAllowed;
        }

        public bool CanExecuteSaveCommand(object _)
        {
            return true;
        }

        private SeasonsEnum GetCurrentSeason()
        {
            var curMonth = DateTime.Now.Month;
            if (curMonth == 1 || curMonth == 2 || curMonth == 3)
                return SeasonsEnum.WINTER;
            if (curMonth == 4 || curMonth == 5 || curMonth == 6)
                return SeasonsEnum.SPRING;
            if (curMonth == 7 || curMonth == 8 || curMonth == 9)
                return SeasonsEnum.SUMMER;
            return SeasonsEnum.FALL;
        }

        private void OnTokenReceived(AuthInfo result)
        {
            this.HasAuthenticated = true;
            this._currentToken = result;
        }

        private void OnTokenErrorReceived()
        {
            this.HasAuthenticated = false;
            this._currentToken = null;
            this.TokenErrorMsg = "Error: cannot authenticate";
        }

        public void Save(object _)
        {
            this.SaveLastCommentIfForgotten();

            DisableSave();

            var saveTask = Task.Run(() => this._service.WriteComments(this._comments.Select(kvPair => kvPair.Value)));

            saveTask.ContinueWith(
                t => OnSaveResultReceived(t.Result), 
                new System.Threading.CancellationToken(false),
                TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.FromCurrentSynchronizationContext());

            saveTask.ContinueWith(
                t => OnSaveResultErrorReceived(t.Result),
                new System.Threading.CancellationToken(false),
                TaskContinuationOptions.NotOnRanToCompletion,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void SaveLastCommentIfForgotten()
        {
            if (!string.IsNullOrEmpty(this.CurrentItemComment) && !this._comments.ContainsKey(this.CurrentItem.Id))
                this.StoreCurrentItemComment();
        }

        private void DisableSave()
        {
            this.IsSaveAllowed = false;
        }

        private void OnSaveResultReceived(bool _)
        {
            this.IsSaveAllowed = true;
        }

        private void OnSaveResultErrorReceived(bool _)
        {
            this.IsSaveAllowed = true;
            this.TokenErrorMsg = "Save process failed";
        }

        public void Browse(object _)
        {
            DisableBrowse();

            Task.Factory
                .StartNew(() => _service.QueryYearAndSeason(this._currentToken, this.SelectedYear, this.SelectedSeason))
                .ContinueWith(t => OnResultReceived(t.Result), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void DisableBrowse()
        {
            this.IsBrowseAllowed = false;
        }

        public void OnResultReceived(IEnumerable<AnimeInfo> list)
        {
            Results = new ObservableCollection<AnimeInfo>(list.OrderBy(s => s.TitleRomaji));
            this.IsBrowseAllowed = true;
        }

        public IEnumerable<int> GenerateLast20Years(int startYear)
        {
            for (int i = 0; i <= 20; i++)
                yield return (startYear - i);
        }

        private void StoreCurrentItemComment()
        {
            if (this.CurrentItemComment == string.Empty || this.CurrentItem == null)
                return;
            this._comments[this.CurrentItem.Id] = new AnimeComment(this.CurrentItem.Id, this.CurrentItem.TitleRomaji, this.CurrentItemComment);
            this.CurrentItemComment = string.Empty;
        }

        private void FetchCurrentItemComment()
        {
            if (this.CurrentItem == null)
                return;
            if (this._comments.ContainsKey(this.CurrentItem.Id))
                this.CurrentItemComment = this._comments[this.CurrentItem.Id].Comment;
        }
    }
}
