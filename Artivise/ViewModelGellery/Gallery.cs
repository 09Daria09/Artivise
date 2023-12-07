using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Artivise.Model;
using System.Windows.Input;
using ResumeDatabase.Commands;
using Artivise.Interfaces_Services;
using Artivise.View;
using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using System.Collections.Generic;
using Artivise.ViewModelGellery;
using static Artivise.Model.ImageData;

namespace Artivise.ViewModelGallery
{
    public class Gallery : INotifyPropertyChanged
    {

        private readonly IMessageService messageService;
        private readonly ICloseable closeable;

        private readonly UserData userData;
        private string _userPhoto;
        private readonly Random _random = new Random();

        private ObservableCollection<ImageData> _images;
        private int _currentImageIndex;

        public ICommand NextImageCommand { get; }
        public ICommand PreviousImageCommand { get; }
        public ICommand FirstImageCommand { get; }
        public ICommand LastImageCommand { get; }

        public ICommand ToggleSidebarCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Gallery(ICloseable closeable, IMessageService messageService, UserData userData)
        {
            this.closeable = closeable;
            this.messageService = messageService;
            this.userData = userData;
            ToggleSidebarCommand = new DelegateCommand(_ => ToggleSidebar(), _ => true);
            UpdateUserPhoto();

            NextImageCommand = new DelegateCommand(_ => MoveToNextImage(), _ => CanMoveToNextImage());
            PreviousImageCommand = new DelegateCommand(_ => MoveToPreviousImage(), _ => CanMoveToPreviousImage());
            FirstImageCommand = new DelegateCommand(_ => MoveToFirstImage(), _ => CanMoveToFirstImage());
            LastImageCommand = new DelegateCommand(_ => MoveToLastImage(), _ => CanMoveToLastImage());

            _images = new ObservableCollection<ImageData>();

            SetThemeBasedOnIndex(0);
            LoadImagesForTheme();
            SelectImage(Images[CurrentImageIndex]);
        }
        #region дефолтный конструктор
        public Gallery()
        {
            _images = new ObservableCollection<ImageData>();
        }
        #endregion

        private int _selectedIndex;
        public string SelectedTheme;
        public int SelectedIndex
        {
            get =>_selectedIndex;
            set
            {
                if (_selectedIndex != value)
                {
                    _selectedIndex = value;
                    OnPropertyChanged(nameof(SelectedIndex));
                    SetThemeBasedOnIndex(value);

                    LoadImagesForTheme();
                    SelectImage(Images[CurrentImageIndex]);
                }
            }
        }

        private void SetThemeBasedOnIndex(int index)
        {
            switch (index)
            {
                case 0:
                    SelectedTheme = "Impressionism";
                    break;
                case 1:
                    SelectedTheme = "UkrainianArtists";
                    break;
                case 2:
                    SelectedTheme = "Strange";
                    break;
                case 3:
                    SelectedTheme = "ModernAbstract";
                    break;
                default:
                    SelectedTheme = "Impressionism";
                    break;
            }
        }

        public double CurrentImageRating
        {
            get
            {
                if (Images.Count > _currentImageIndex && Images[_currentImageIndex].Ratings.Any())
                {
                    return Images[_currentImageIndex].Ratings.Average(r => r.Score);
                }
                return 0;
            }
        }


        private int _currentRating;
        public int CurrentRating
        {
            get => _currentRating;
            set
            {
                if (_currentRating != value)
                {
                    _currentRating = value;
                    AddRatingToCurrentImage(_currentRating);
                    OnPropertyChanged(nameof(CurrentRating));
                }
            }
        }

        private void SelectImage(ImageData selectedImage)
        {
            var currentUser = userData.UserName; 

            var userRating = selectedImage.Ratings
                .FirstOrDefault(r => r.UserName == currentUser);

            CurrentRating = userRating?.Score ?? 0; 
        }

        private void AddRatingToCurrentImage(int newRating)
        {
            var currentUser = userData.UserName;

            var userRating = Images[CurrentImageIndex].Ratings
                .FirstOrDefault(r => r.UserName == currentUser);

            if (userRating != null)
            {
                userRating.Score = newRating;
            }
            else
            {
                Images[CurrentImageIndex].Ratings.Add(new Rating { UserName = currentUser, Score = newRating });
            }

            RatingsManager.SaveRatingsToFile(Images.ToList(), SelectedTheme); 
        }



        public string CurrentTitle
        {
            get => Images.Count > _currentImageIndex ? Images[_currentImageIndex].Title : null;
            set
            {
                if (Images.Count > _currentImageIndex)
                {
                    Images[_currentImageIndex].Title = value;
                    OnPropertyChanged(nameof(CurrentTitle));
                }
            }
        }

        public string CurrentAuthorName
        {
            get => Images.Count > _currentImageIndex ? Images[_currentImageIndex].AuthorName : null;
            set
            {
                if (Images.Count > _currentImageIndex)
                {
                    Images[_currentImageIndex].AuthorName = value;
                    OnPropertyChanged(nameof(CurrentAuthorName));
                }
            }
        }

        #region работа с изображением
        public ObservableCollection<ImageData> Images
        {
            get => _images;
            set
            {
                _images = value;
                OnPropertyChanged(nameof(Images));
                OnPropertyChanged(nameof(ImageCount));
            }
        }
        public int ImageCount
        {
            get => Images.Count - 1;
        }
        public int CurrentImageIndex
        {
            get => _currentImageIndex;
            set
            {
                if (value >= 0 && value < Images.Count)
                {
                    _currentImageIndex = value;
                    OnPropertyChanged(nameof(CurrentImageIndex));
                    OnPropertyChanged(nameof(CurrentImage));

                    OnPropertyChanged(nameof(CurrentTitle));
                    OnPropertyChanged(nameof(CurrentAuthorName));

                    OnPropertyChanged(nameof(CurrentImageRating));
                    SelectImage(Images[CurrentImageIndex]);
                }
            }
        }
        public string CurrentImage
        {
            get => Images.Count > _currentImageIndex ? Images[_currentImageIndex].ImagePath : null;
            set
            {
                if (Images.Count > _currentImageIndex)
                {
                    Images[_currentImageIndex].ImagePath = value;
                    OnPropertyChanged(nameof(CurrentImage));
                }
            }
        }
        private void LoadImagesForTheme()
        {
            Images.Clear();

            var assembly = Assembly.GetExecutingAssembly();
            var resourcePath = $"{assembly.GetName().Name}.Images.{SelectedTheme}.{SelectedTheme}.json";

            var imageInfo = LoadImageInfo(assembly, resourcePath);

            for (int i = 1; i <= imageInfo.Count; i++)
            {
                if (imageInfo.TryGetValue(i, out var info))
                {
                    var imagePath = $"pack://application:,,,/Images/{SelectedTheme}/{i}.jpg";
                    Images.Add(new ImageData
                    {
                        ImagePath = imagePath,
                        AuthorName = info.artist,
                        Title = $"\"{info.title_en}\" \\\"{info.title_ru}\""
                    });
                }
            }
            RatingsManager.LoadRatingsFromFile(Images.ToList(), SelectedTheme);

            if (Images.Count > 0)
            {
                CurrentImageIndex = 0;
            }
            OnPropertyChanged(nameof(ImageCount));
        }

        private Dictionary<int, (string artist, string title_en, string title_ru)> LoadImageInfo(Assembly assembly, string resourcePath)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            using (StreamReader reader = new StreamReader(stream))
            {
                string jsonText = reader.ReadToEnd();
                var items = JsonConvert.DeserializeObject<List<dynamic>>(jsonText);
                var imageInfo = new Dictionary<int, (string artist, string title_en, string title_ru)>();

                foreach (var item in items)
                {
                    imageInfo.Add((int)item.id, ((string)item.artist, (string)item.title_en, (string)item.title_ru));
                }

                return imageInfo;
            }
        }
        #endregion

        #region
        private void MoveToNextImage()
        {
            if (CurrentImageIndex < Images.Count - 1)
                CurrentImageIndex++;

            SelectImage(Images[CurrentImageIndex]);
        }

        private bool CanMoveToNextImage()
        {
            return CurrentImageIndex < Images.Count - 1;
        }

        private void MoveToPreviousImage()
        {
            if (CurrentImageIndex > 0)
                CurrentImageIndex--;

            SelectImage(Images[CurrentImageIndex]);
        }

        private bool CanMoveToPreviousImage()
        {
            return CurrentImageIndex > 0;
        }

        private void MoveToFirstImage()
        {
            CurrentImageIndex = 0;

            SelectImage(Images[CurrentImageIndex]);
        }

        private bool CanMoveToFirstImage()
        {
            return Images.Count > 0 && CurrentImageIndex > 0;
        }

        private void MoveToLastImage()
        {
            CurrentImageIndex = Images.Count - 1;

            SelectImage(Images[CurrentImageIndex]);
        }

        private bool CanMoveToLastImage()
        {
            return Images.Count > 0 && CurrentImageIndex < Images.Count - 1;
        }
        #endregion


        public string UserPhoto
        {
            get => _userPhoto;
            set
            {
                _userPhoto = value;
                OnPropertyChanged(nameof(UserPhoto));
            }
        }

        public string FullName => $"{userData.FirstName} {userData.LastName} :)";

        private void UpdateUserPhoto()
        {
            int imageNumber = _random.Next(1, 46);
            UserPhoto = $"pack://application:,,,/Avatar/{imageNumber}.jpg";
        }

        private void ToggleSidebar()
        {
            IsSidebarVisible = !IsSidebarVisible;
        }

        private bool _isSidebarVisible;
        public bool IsSidebarVisible
        {
            get => _isSidebarVisible;
            set
            {
                _isSidebarVisible = value;
                OnPropertyChanged(nameof(IsSidebarVisible));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
