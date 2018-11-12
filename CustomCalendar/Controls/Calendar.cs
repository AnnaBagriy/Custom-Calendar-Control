using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms;
using CustomCalendar.ExtensionMethods;
using System.Windows.Input;
using CustomCalendar.Strings;
using CustomCalendar.Helpers;

namespace CustomCalendar.Controls
{
    public class Calendar : Grid
    {
        #region Private fields

        private const int RowsCount = 8;
        private const int ColumnsCount = 7;

        private const int MonthHeaderRowId = 0;
        private const int DayHeaderRowId = 1;
        private const int FirstDayRowId = 2;

        private const int BackwardNavigationLabelColumnId = 0;
        private const int ForwardNavigationLabelColumnId = 5;
        private const int MonthHeaderColumnId = 2;

        private const int BackwardNavigationLabelColumnSpanId = 2;
        private const int ForwardNavigationLabelColumnSpanId = 2;
        private const int MonthHeaderColumnSpanId = 3;

        private Dictionary<DayOfWeek, string> _dayDictionary;
        private List<DayCell> _dayCellCollection = new List<DayCell>();
        private Label _monthLabel = new Label();

        private ICommand NavigationCommand => new Command((obj) => ChangeMonth(obj), NavigationCommandCanExecute);

        #endregion

        #region Public fields

        public static readonly BindableProperty SelectedDatesProperty = BindableProperty.Create(
            nameof(SelectedDates),
            typeof(ObservableCollection<DateTime>),
            typeof(Calendar),
            new ObservableCollection<DateTime>(),
            BindingMode.TwoWay,
            propertyChanged: SelectedDatesPropertyChanged);

        public static readonly BindableProperty SelectedDateProperty = BindableProperty.Create(
            nameof(SelectedDate),
            typeof(DateTime),
            typeof(Calendar),
            DateTime.Today,
            BindingMode.TwoWay,
            propertyChanged: SelectedDatePropertyChanged);

        public static readonly BindableProperty SelectedDateCommandProperty = BindableProperty.Create(
            nameof(SelectedDateCommand),
            typeof(Command<object>),
            typeof(Calendar));

        public ObservableCollection<DateTime> SelectedDates
        {
            get => (ObservableCollection<DateTime>)GetValue(SelectedDatesProperty);
            set => SetValue(SelectedDatesProperty, value);
        }

        public DateTime SelectedDate
        {
            get => (DateTime)GetValue(SelectedDateProperty);
            private set => SetValue(SelectedDateProperty, value);
        }

        public Command<object> SelectedDateCommand
        {
            get => (Command<object>)GetValue(SelectedDateCommandProperty);
            set => SetValue(SelectedDateCommandProperty, value);
        }

        public DayOfWeek FirstDayOfWeek { get; set; }

        public DateTime MinDate { get; set; } = new DateTime(1980, 1, 1);
        public DateTime MaxDate { get; set; } = new DateTime(2050, 1, 1);

        public bool IsSelectionOnTapEnabled { get; set; }
        public bool IsMultiselectionOnTapEnabled { get; set; }

        #endregion

        protected override void OnParentSet()
        {
            base.OnParentSet();

            _dayDictionary = DaysOfWeekHelper.GetWeekNames(FirstDayOfWeek);
            _monthLabel = GetMonthHeaderLabel();

            SelectedDatesPropertyChanged(this, null, SelectedDates);

            ConfigureGestureRecognizers();

            InitializeControlUi();
        }

        #region UI

        #region Init UI elements

        private void ConfigureGrid()
        {
            for (int i = 0; i < 2; i++)
            {
                ColumnDefinitions.Add(new ColumnDefinition());
                RowDefinitions.Add(new RowDefinition() { Height = 40 });
            }

            for (int i = 2; i < ColumnsCount; i++)
            {
                ColumnDefinitions.Add(new ColumnDefinition());
                RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            }

            RowSpacing = 5;
        }

        private void ConfigureGestureRecognizers()
        {
            GestureRecognizers.Add(new SwipeGestureRecognizer()
            {
                Direction = SwipeDirection.Left,
                Command = NavigationCommand,
                CommandParameter = "forward"
            });

            GestureRecognizers.Add(new SwipeGestureRecognizer()
            {
                Direction = SwipeDirection.Right,
                Command = NavigationCommand,
                CommandParameter = "backward"
            });
        }

        private void InitializeControlUi()
        {
            ConfigureGrid();

            InitNavigationButtonsForCalendar();
            InitMonthHeaderForCalendar();
            InitDaysHeadersForCalendar();

            InitializeDaysUi();
        }

        private void InitSelectedCalendarDate(out int daysInMonth, out DayOfWeek firstDayOfMonth, out DayOfWeek lastDayOfMonth)
        {
            daysInMonth = DateTime.DaysInMonth(SelectedDate.Year, SelectedDate.Month);

            firstDayOfMonth = new DateTime(SelectedDate.Year, SelectedDate.Month, 1).DayOfWeek;
            lastDayOfMonth = new DateTime(SelectedDate.Year, SelectedDate.Month, daysInMonth).DayOfWeek;
        }

        private void InitializeDaysUi()
        {
            var dayCount = 1;

            InitSelectedCalendarDate(out int daysInMonth, out DayOfWeek firstDayOfMonth, out DayOfWeek lastDayOfMonth);

            var dayOfWeekColumnIndex = firstDayOfMonth.ChangeFirstDayOfWeek(FirstDayOfWeek);

            for (int row = FirstDayRowId; row < RowsCount; row++)
            {
                for (int column = dayOfWeekColumnIndex; column < ColumnsCount; column++)
                {
                    var dateTime = new DateTime(SelectedDate.Year, SelectedDate.Month, dayCount);
                    var dayCell = GetDayCell(dateTime);

                    _dayCellCollection.Add(dayCell);
                    Children.Add(dayCell, column, row);

                    dayCount++;

                    if (dayCount > daysInMonth)
                    {
                        return;
                    }
                }

                dayOfWeekColumnIndex = 0;
            }
        }

        private void InitMonthHeaderForCalendar()
        {
            _monthLabel = GetMonthHeaderLabel();

            SetColumnSpan(_monthLabel, MonthHeaderColumnSpanId);
            SetRow(_monthLabel, MonthHeaderRowId);
            SetColumn(_monthLabel, MonthHeaderColumnId);

            _monthLabel.VerticalOptions = LayoutOptions.Center;
            _monthLabel.HorizontalOptions = LayoutOptions.FillAndExpand;

            Children.Add(_monthLabel);
        }

        private void InitNavigationButtonsForCalendar()
        {
            var back = GetBackLabel();
            var forward = GetForwardLabel();

            SetColumnSpan(back, BackwardNavigationLabelColumnSpanId);
            SetColumnSpan(forward, ForwardNavigationLabelColumnSpanId);

            SetRow(back, MonthHeaderRowId);
            SetRow(forward, MonthHeaderRowId);

            SetColumn(back, BackwardNavigationLabelColumnId);
            SetColumn(forward, ForwardNavigationLabelColumnId);

            back.VerticalOptions = LayoutOptions.Center;
            forward.VerticalOptions = LayoutOptions.Center;

            back.HorizontalOptions = LayoutOptions.FillAndExpand;
            forward.HorizontalOptions = LayoutOptions.FillAndExpand;

            Children.Add(back);
            Children.Add(forward);
        }

        private void InitDaysHeadersForCalendar()
        {
            int column = 0;

            foreach (var day in _dayDictionary)
            {
                Children.Add(GetDayHeaderLabel(day.Value), column, DayHeaderRowId);
                column++;
            }
        }

        #endregion

        #region Remove UI elements

        private void ClearDayCells()
        {
            _dayCellCollection.ForEach(dayCell => Children.Remove(dayCell));
            _dayCellCollection.Clear();
        }

        private void ClearMonthHeader()
        {
            Children.Remove(_monthLabel);
            _monthLabel.Text = string.Empty;
        }

        #endregion

        #region Get UI elements

        private DayCell GetDayCell(DateTime dateTime)
        {
            var isSelected = SelectedDates.Any(x => x.Date == dateTime.Date);
            var dayCell = new DayCell(dateTime)
            {
                IsSelected = isSelected,
                IsEnabled = dateTime >= MinDate ? IsEnabled = true : IsEnabled = false
            };

            dayCell.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    if (IsSelectionOnTapEnabled && SelectedDates != null)
                    {
                        var existingDate = SelectedDates.FirstOrDefault(x => x.Date == dateTime.Date);

                        if (existingDate != default(DateTime))
                        {
                            SelectedDates.Remove(existingDate);
                        }
                        // One time selection.
                        else if (!IsMultiselectionOnTapEnabled && (SelectedDates.Count == 1 || SelectedDates.Count == 0))
                        {
                            var selectedDate = SelectedDates.FirstOrDefault();
                            var selectedCell = _dayCellCollection.FirstOrDefault(x => selectedDate.Date == x.Date) as DayCell;

                            if (selectedCell != null)
                            {
                                selectedCell.IsSelected = false;
                                SelectedDates.Remove(selectedDate);
                            }

                            SelectedDates.Add(dateTime);
                        }
                        // Multiselection
                        else if (IsMultiselectionOnTapEnabled)
                        {
                            SelectedDates.Add(dateTime);
                        }
                    }

                    SelectedDateCommand.Execute(dateTime);
                })
            });

            return dayCell;
        }

        private Label GetDayHeaderLabel(string dayName)
        {
            // TODO: Set styles
            return new Label
            {
                Text = dayName,
                TextColor = Color.Black,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
        }

        private Label GetMonthHeaderLabel()
        {
            return new Label
            {
                Text = SelectedDate.ToString("MMMM"),
                TextColor = Color.Black,
                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
        }

        private Label GetBackLabel()
        {
            var label = new Label
            {
                Text = "<",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };

            label.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = NavigationCommand,
                CommandParameter = "backward"
            });

            return label;
        }

        private Label GetForwardLabel()
        {
            var label = new Label
            {
                Text = ">",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };

            label.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = NavigationCommand,
                CommandParameter = "forward"
            });

            return label;
        }

        #endregion

        #endregion

        #region Property changed methods

        private static void SelectedDatesPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (!(bindable is Calendar control))
            {
                return;
            }

            if (newvalue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += control.SelectedDatesValue_CollectionChanged;
            }

            if (oldvalue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= control.SelectedDatesValue_CollectionChanged;
            }
        }

        private static void SelectedDatePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (!(bindable is Calendar control))
            {
                return;
            }

            control.ClearDayCells();
            control.ClearMonthHeader();

            control.InitMonthHeaderForCalendar();
            control.InitializeDaysUi();
        }

        private void SelectedDatesValue_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is DateTime date && date.Year == SelectedDate.Year && date.Month == SelectedDate.Month)
                    {
                        var dayCell = _dayCellCollection.FirstOrDefault(x => x.Date.Date == date.Date);

                        if (dayCell != null)
                        {
                            dayCell.IsSelected = true;
                        }
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is DateTime date && date.Year == SelectedDate.Year && date.Month == SelectedDate.Month)
                    {
                        var dayCell = _dayCellCollection.FirstOrDefault(x => x.Date.Date == date.Date);

                        if (dayCell != null)
                        {
                            dayCell.IsSelected = false;
                        }
                    }
                }
            }
        }

        #endregion

        #region Command methods

        private void ChangeMonth(object param)
        {
            if (param is string move)
            {
                if (move == "forward")
                {
                    SelectedDate = SelectedDate.AddMonths(1);
                }
                else if (move == "backward")
                {
                    SelectedDate = SelectedDate.AddMonths(-1);
                }
            }
        }

        private bool NavigationCommandCanExecute(object param)
        {
            if (param is string move)
            {
                if (move == "forward")
                {
                    return SelectedDate < MaxDate;
                }
                else if (move == "backward")
                {
                    return SelectedDate > MinDate;
                }
            }

            return false;
        }

        #endregion
    }
}