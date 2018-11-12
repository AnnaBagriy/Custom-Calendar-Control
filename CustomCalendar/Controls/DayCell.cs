using System;
using Xamarin.Forms;

namespace CustomCalendar.Controls
{
    public class DayCell : ContentView
    {
        # region Public fields

        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(
            nameof(IsSelected),
            typeof(bool),
            typeof(DayCell),
            false,
            BindingMode.OneWayToSource,
            propertyChanged: IsSelectedPropertyChanged);

        public static readonly new BindableProperty IsEnabledProperty = BindableProperty.Create(
            nameof(IsEnabled),
            typeof(bool),
            typeof(DayCell),
            false,
            BindingMode.OneWayToSource,
            propertyChanged: IsEnabledPropertyChanged);

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public new bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        public DateTime Date { get; private set; }

        #endregion
          
        public DayCell(DateTime dateTime)
        {
            Date = dateTime;

            FillDayCellContent();
        }

        #region Methods for getting different labels

        private View GetSelectedView()
        {
            return new Frame
            {
                // TODO: Dynamic circle frame
                Padding = new Thickness(0, 0),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HasShadow = false,
                HeightRequest = 30,
                WidthRequest = 30,
                CornerRadius = 15,
                BorderColor = Color.LightSkyBlue,
                BackgroundColor = Color.LightSkyBlue,
                InputTransparent = true,
                Content = new Label()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Text = Date.Day.ToString(),
                    TextColor = Color.White,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    HeightRequest = 30,
                    WidthRequest = 30
                }
            };
        }

        private View GetOrdinaryView()
        {
            return new Label()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                HeightRequest = 30,
                WidthRequest = 30,
                Text = Date.Day.ToString(),
                TextColor = Color.Black
            };
        }

        private View GetDisabledView()
        {
            return new Label()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                HeightRequest = 30,
                WidthRequest = 30,
                Text = Date.Day.ToString(),
                TextColor = Color.DarkGray
            };
        }

        #endregion

        #region Private methods

        private static void IsSelectedPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is DayCell control)
            {
                control.FillDayCellContent();
            }
        }

        private static void IsEnabledPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is DayCell control)
            {
                control.FillDayCellContent();
            }
        }

        private void FillDayCellContent()
        {
            if (!IsEnabled)
            {
                Content = GetDisabledView();
            }
            else if (IsSelected)
            {
                Content = GetSelectedView();
            }
            else
            {
                Content = GetOrdinaryView();
            }
        }

        #endregion
    }
}