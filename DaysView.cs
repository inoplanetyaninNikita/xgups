using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;


namespace XGups
{
    public class DaysView : INotifyPropertyChanged
    {
        public MainMenu page;
        public bool HaveLessons = true;
        public DaysView(DateTime dateTime, bool haveLessons)
        {
            DateTime = dateTime;
            ClickOnFrame = new Command(() =>
            {
                page.LastSelectedView?.returnType();
                page.LastSelectedView = this;
                page.OpenDay(datetime);

                Type = DaysType.Select;
            });
            HaveLessons = haveLessons;

            returnType();
        }
        public void returnType()
        {
            if (datetime.DayOfWeek != DayOfWeek.Sunday && HaveLessons == true)
            {
                Type = DaysType.Default;
            }
            else
            {
                Type = DaysType.Party;
            }

            if (datetime.DayOfYear == DateTime.Now.DayOfYear)
            {
                Type = DaysType.Today;
            }
        }

        DateTime datetime;
        DaysType type;

        public DateTime DateTime
        {
            get
            {
                return datetime;
            }
            set
            {
                SetProperty(ref datetime, value);
            }
        }
        public DaysType Type { 
            get { return type; }
            set
            {
                SetProperty(ref type, value);
            }
        }
        public string DayName
        {
            get 
            {
                switch (DateTime.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        return "вс";
                    case DayOfWeek.Monday:
                        return "пн";
                    case DayOfWeek.Tuesday:
                        return "вт";
                    case DayOfWeek.Wednesday:
                        return "ср";
                    case DayOfWeek.Thursday:
                        return "чт";
                    case DayOfWeek.Friday:
                        return "пт";
                    case DayOfWeek.Saturday:
                        return "сб";
                        default: return "Unknown";
                }
            }
        }
        public string DayNumber
        {
            get
            {
                return DateTime.Date.Day.ToString();
            }
        }
        public Command ClickOnFrame { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
    public enum DaysType
    {
        Default,
        Party,
        Today,
        Select
    }
    public class DaysTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = Color.White;
            DaysType colorStr = (DaysType)value;
            switch (colorStr)
            {
                case DaysType.Default:
                    color = Color.FromHex("#9191FF");
                    break;
                case DaysType.Party:
                    color = Color.FromHex("#B9B9F0");
                    break;
                case DaysType.Today:
                    color = Color.WhiteSmoke;
                    break;
                case DaysType.Select:
                    color = Color.White;
                    break;
            }
            return color;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Color.White;
        }
    }
    public class DaysTypeToColorConverterForFonts : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = Color.White;
            DaysType colorStr = (DaysType)value;
            switch (colorStr)
            {
                case DaysType.Default:
                    color = Color.White;
                    break;
                case DaysType.Party:
                    color = Color.White;
                    break;
                case DaysType.Today:
                    color = Color.Black;
                    break;
                case DaysType.Select:
                    color = Color.FromHex("#9191FF");
                    break;
            }
            return color;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Color.White;
        }
    }
}
