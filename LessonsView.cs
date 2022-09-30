using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace XGups
{
    public class LessonsView : INotifyPropertyChanged
    {
        public LessonsView(Format.Lesson lesson, DateTime dateTime)
        {
            Name = lesson.Title;
            Teacher = lesson.TeacherFIO;
            Auditorium = lesson.Auditorium;
            PerfectTime = lesson.PerfectTime;

            DateTime now = DateTime.Now;
            DateTime date = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
            switch (lesson.Number)
            {
                case 0:
                    startLesson = date.Add(new TimeSpan(8, 30, 0));
                    endLesson = startLesson.AddMinutes(90);
                    break;
                case 1:
                    startLesson = date.Add(new TimeSpan(10, 15, 0));
                    endLesson = startLesson.AddMinutes(90);
                    break;
                case 2:
                    startLesson = date.Add(new TimeSpan(12, 10, 0));
                    endLesson = startLesson.AddMinutes(90);
                    break;
                case 3:
                    startLesson = date.Add(new TimeSpan(13, 55, 0));
                    endLesson = startLesson.AddMinutes(90);
                    break;
                case 4:
                    startLesson = date.Add(new TimeSpan(15, 35, 0));
                    endLesson = startLesson.AddMinutes(90);
                    break;
                case 5:
                    startLesson = date.Add(new TimeSpan(18, 55, 0));
                    endLesson = startLesson.AddMinutes(90);
                    break;
                case 6:
                    startLesson = date;
                    endLesson = startLesson.AddMinutes(90);
                    break;
                default:
                    break;
            }

            Type = LessonsType.Past;

            AI = new Command(() =>
            {
                if (Name == "пр.Системы искусственного интеллекта " || Name == "лаб.Системы искусственного интеллекта ")
                {
                    ((MainMenu)App.Current.MainPage).GiveawayAI();
                }
            });
          

            if (startLesson.DayOfYear == now.DayOfYear)
            {
                if ((startLesson - now).TotalMinutes >= 15)
                {
                    Type = LessonsType.Today;
                }
                if ((startLesson - now).TotalMinutes > 0 && (startLesson - now).TotalMinutes < 15)
                {
                    Type = LessonsType.Prepare;
                }
                if ((startLesson - now).TotalMinutes > -90 && (startLesson - now).TotalMinutes < 0)
                {
                    Type = LessonsType.Running;
                }
                if ((startLesson - now).TotalMinutes <= -90)
                {
                    Type = LessonsType.Past;
                }
            }
            else
            {
                if (startLesson > now) Type = LessonsType.Future;
                else Type = LessonsType.Past;
            }
        }

    
        string name;
        string teacher;
        string auditorium;
        string perfecttime;
        LessonsType type;

        public DateTime startLesson;
        public DateTime endLesson;

        public Command AI { get; set; }
        public void TypeUpdate()
        {
            DateTime now = DateTime.Now;
            Type = LessonsType.Past;

            if (startLesson.DayOfYear == now.DayOfYear)
            {
                if ((startLesson - now).TotalMinutes >= 15)
                {
                    Type = LessonsType.Today;
                }
                if ((startLesson - now).TotalMinutes > 0 && (startLesson - now).TotalMinutes < 15)
                {
                    Type = LessonsType.Prepare;
                }
                if ((startLesson - now).TotalMinutes > -90 && (startLesson - now).TotalMinutes < 0)
                {
                    Type = LessonsType.Running;
                }
                if ((startLesson - now).TotalMinutes <= -90)
                {
                    Type = LessonsType.Past;
                }
            }
            else
            {
                if (startLesson > now) Type = LessonsType.Future;
                else Type = LessonsType.Past;
            }
        }
        public string PerfectTime
        {
            get { return perfecttime; }
            set
            {
                SetProperty(ref perfecttime, value);
            }
        }
        public string Name
        {
            get { return name; }
            set
            {
                SetProperty(ref name, value);
            }
        }
        public string Teacher
        {
            get { return teacher; }
            set
            {
                SetProperty(ref teacher, value);
            }
        }
        public string Auditorium
        {
            get
            {
                return auditorium;
            }
            set
            {
                SetProperty(ref auditorium, value);
            }
        }
        public LessonsType Type
        {
            get { return type; }
            set
            {
                SetProperty(ref type, value);
            }
        }

        #region Binding
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
        #endregion
    }
    public enum LessonsType
    { 
        Future,
        Today,
        Prepare,
        Running,
        Past
    }
    public class LessonsTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = Color.White;
            LessonsType colorStr = (LessonsType)value;
            switch (colorStr)
            {
                case LessonsType.Today:
                    //color = Color.FromHex("#A2A2F5");
                    color = Color.FromHex("#C8C7FF");
                    break;
                case LessonsType.Past:
                    color = Color.FromHex("#BEBEBE");
                    break;
                case LessonsType.Future:
                    //DAB8FC
                    color = Color.FromHex("#C8C7FF");
                    break;
                case LessonsType.Running:
                    //DAB8FC
                    color = Color.FromHex("#A5F8A5");
                    break;
                case LessonsType.Prepare:
                    //DAB8FC
                    color = Color.FromHex("#FAF99A");
                    break;
            }

            return color;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Color.White;
        }
    }
    public class LessonsTypeToColorConverterForFonts : IValueConverter
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
    public class LessonsTypeToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool Visible = false;
            LessonsType colorStr = (LessonsType)value;
            switch (colorStr)
            {
                case LessonsType.Today:
                    //color = Color.FromHex("#A2A2F5");
                    Visible = false;
                    break;
                case LessonsType.Past:
                    Visible = false;
                    break;
                case LessonsType.Future:
                    //DAB8FC
                    Visible = false;
                    break;
                case LessonsType.Running:
                    //DAB8FC
                    Visible = true;
                    break;
                case LessonsType.Prepare:
                    //DAB8FC
                    Visible = true;
                    break;
            }

            return Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}
