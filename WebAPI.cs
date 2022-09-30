using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using System.Threading.Tasks;
using System.Net.Http;
using Xamarin.Essentials;
using static XGups.Human.Detail;

namespace XGups
{
    [Serializable]
    public static class Format
    {
        private static StringBuilder builder = new StringBuilder();
        public static string Group(string Group)
        {
            builder.Clear();
            builder.Append(Group);
            // проверка на первую букву, для уборки мусора озУПб11 -> УПб11 | дСОДП11 -> СОДП11
            for (int i = 0; i < Group.Length; i++)
            {
                if (char.IsLower(Group[i])) builder.Remove(i, 1);
                else break;
            }
            builder.Replace("-", "").Replace(" ", "");
            for (int i = 0; i < builder.Length; i++)
            {
                builder[i] = char.ToUpper(builder[i]);
            }
            return builder.ToString();
        }
        public class Week
        {
            /// <summary>
            /// Четная неделя?
            /// </summary>
            public bool IsEven { get; set; }
            public List<Day> Days = new List<Day>();
            /// <summary>
            /// Понедельник
            /// </summary>
            public Day Monday
            {
                get
                {
                    return Days[0];
                }
                set
                {
                    Days[0] = value;
                }
            }
            /// <summary>
            /// Вторник
            /// </summary>
            public Day Tuesday
            {
                get
                {
                    return Days[1];
                }
                set
                {
                    Days[1] = value;
                }
            }
            /// <summary>
            /// Среда
            /// </summary>
            public Day Wednesday
            {
                get
                {
                    return Days[2];
                }
                set
                {
                    Days[2] = value;
                }
            }
            /// <summary>
            /// Четверг
            /// </summary>
            public Day Thursday
            {
                get
                {
                    return Days[3];
                }
                set
                {
                    Days[3] = value;
                }
            }
            /// <summary>
            /// Пятница
            /// </summary>
            public Day Friday
            {
                get
                {
                    return Days[4];
                }
                set
                {
                    Days[4] = value;
                }
            }
            /// <summary>
            /// Суббота
            /// </summary>
            public Day Saturday
            {
                get
                {
                    return Days[5];
                }
                set
                {
                    Days[5] = value;
                }
            }
            /// <summary>
            /// Воскресенье
            /// </summary>
            public Day Sunday
            {
                get
                {
                    return Days[6];
                }
                set
                {
                    Days[6] = value;
                }
            }
            public Day Today
            {
                get
                {
                    int nowDay = (int)DateTime.Now.DayOfWeek;
                    nowDay = (nowDay == 0) ? 7 : nowDay;
                    return Days[nowDay - 1];
                }
                set
                {
                    int nowDay = (int)DateTime.Now.DayOfWeek;
                    nowDay = (nowDay == 0) ? 7 : nowDay;
                    Days[nowDay - 1] = value;
                }
            }
        }
        public class Day
        {
            public string Name { get; set; }
            public Day NextDay { get; set; }
            public Day LastDay { get; set; }
            public List<Lesson> Lessons = new List<Lesson>();
            public Day(int NumberDay)
            {
                switch (NumberDay)
                {
                    case 0:
                        Name = "Понедельник";
                        break;
                    case 1:
                        Name = "Вторник";
                        break;
                    case 2:
                        Name = "Среда";
                        break;
                    case 3:
                        Name = "Четверг";
                        break;
                    case 4:
                        Name = "Пятница";
                        break;
                    case 5:
                        Name = "Суббота";
                        break;
                    case 6:
                        Name = "Воскресенье";
                        break;
                    default:
                        throw new ArgumentException("Неправильный номер дня, 0-Понедельник 6-Воскресенье");
                        break;
                }
            }
        }
        public class Lesson
        {
            /// <summary>
            /// Полное название пары.
            /// </summary>
            public string FullName { get; set; }
            /// <summary>
            /// Аудитория с ДОТ/ЭИОС.
            /// Например 5555/ДОТ
            /// </summary>
            public string FullAuditorium
            {
                get
                {
                    if (FullName == null || FullName.Length < 5) return null;

                    var words = FullName.Split(' ');
                    return words[words.Length - 2];
                }
            }
            /// <summary>
            /// Аудитория без ДОТ/ЭИОС.
            /// Например 5555
            /// </summary>
            public string Auditorium
            {
                get
                {
                    string Name = FullName;
                    if (FullName == null || FullName.Length < 5) return null;
                    if (NewAuditorium != null) return NewAuditorium.Split('/')[0];

                    var words = Name.Split(' ');
                    words = words[words.Length - 2].Split('/');
                    return words[0];
                }
            }

            /// <summary>
            /// Фамилия [имя отчество](Сокращено).
            /// Васильев Д.В.
            /// </summary>
            public string TeacherFIO
            {
                get
                {
                    if (FullName == null || FullName.Length < 5) return null;
                    if (NewGroup != null) return NewGroup;

                    var words = FullName.Split(' ');
                    return words[words.Length - 4] + " " + words[words.Length - 3];
                }
            }
            /// <summary>
            /// Название предмета с типом предмета.
            /// Например пр.(тип)Математика 
            /// </summary>
            public string Title
            {
                get
                {
                    if (FullName == null || FullName.Length < 5) return null;
                    if (NewLessonName != null) return NewLessonName;

                    var words = FullName.Split(' ');
                    string word = "";
                    for (int i = 0; i < words.Length - 4; i++)
                    {
                        word += words[i] + " ";
                    }
                    return word;
                }
            }
            /// <summary>
            /// Номер пары.
            /// </summary>
            public int Number { get; set; }
            /// <summary>
            /// Время начала пары.
            /// </summary>
            public string TimeStart
            {
                get
                {
                    switch (Number)
                    {
                        case 0:
                            return "8:30";
                            break;
                        case 1:
                            return "10:15";
                            break;
                        case 2:
                            return "12:10";
                            break;
                        case 3:
                            return "13:55";
                            break;
                        case 4:
                            return "15:35";
                            break;
                        case 5:
                            return "17:15";
                            break;
                        case 6:
                            return "18:55";
                            break;
                        default:
                            return
                                "Unknown";
                            break;
                    }
                }
            }
            /// <summary>
            /// Время окончания пары
            /// </summary>
            public string TimeEnd
            {
                get
                {
                    switch (Number)
                    {
                        case 0:
                            return "10:00";
                            break;
                        case 1:
                            return "11:45";
                            break;
                        case 2:
                            return "13:40";
                            break;
                        case 3:
                            return "15:25";
                            break;
                        case 4:
                            return "17:05";
                            break;
                        case 5:
                            return "18:45";
                            break;
                        case 6:
                            return "20:25";
                            break;
                        default:
                            return
                                "Unknown";
                            break;
                    }
                }
            }
            /// <summary>
            /// Отформатированное время.
            /// </summary>
            public string PerfectTime
            {
                get
                {
                    return TimeStart + " — " + TimeEnd;
                }
            }

            public string NewGroup
            {
                get; set;
            }
            public string NewLessonName
            {
                get; set;
            }
            public string NewAuditorium
            {
                get; set;
            }
            public Lesson(string group, string lesson, string auditorium, int num)
            {
                FullName = $"{lesson} {group} {auditorium}";
                Number = num;

                NewGroup = group;
                NewLessonName = lesson;
                NewAuditorium = auditorium + "/ДОТ";
            }
            public Lesson(string name, int num)
            {
                FullName = name;
                Number = num;
            }
        }
    }
    [Serializable]
    public class Request
    {

        //https://www.samgups.ru/raspisanie/2021-2022/vtoroy-semestr/HTML/raspisan.html
        //
        private static string listScheduleUri = "https://www.samgups.ru/raspisanie/2022-2023/perviy-semestr/HTML/raspisan.html";
        //https://www.samgups.ru/raspisanie/2021-2022/vtoroy-semestr/HTML_PREPS/Praspisan.html
        private static string listScheduleUriTeacher = "https://www.samgups.ru/raspisanie/2022-2023/perviy-semestr/HTML_PREPS/Praspisan.html";

        /// <summary>
        /// Ссылка на список расписаний, используется для метода - <see cref="getScheduleForGroupUri(string)"/>
        /// </summary>
        /// 
        public static bool reverse = false;
        public static Uri ListOfSchedules
        {
            get { return new Uri(listScheduleUri); }
            set { listScheduleUri = value.ToString(); }
        }
        public static Uri ListOfSchedulesTeacher
        {
            get { return new Uri(listScheduleUriTeacher); }
            set { listScheduleUriTeacher = value.ToString(); }
        }

        /// <summary>
        /// Возрашает URL расписания группы
        /// </summary>
        /// <param name="FindGroup"></param>
        private static Uri getScheduleForGroupUri(Human human)
        {
            var FindGroup = Format.Group(human.DetailInfo.gname);
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(listScheduleUri);
            HtmlNodeCollection table = document.DocumentNode.SelectNodes("/html/body/table");

            int Row = 0;
            while (true)
            {
                if (string.IsNullOrWhiteSpace(table[0].ChildNodes[3 + Row * 2].InnerText) == true) break; // Когда строки пустые, выходим из цикла
                for (int Column = 0; string.IsNullOrWhiteSpace(table[0].ChildNodes[3 + Row * 2].ChildNodes[Column].ChildNodes[1].ChildNodes[0].InnerText) == false; Column++)
                {
                    var groupFix = Format.Group(table[0].ChildNodes[3 + Row * 2].ChildNodes[Column].ChildNodes[1].ChildNodes[0].InnerText);
                    if (groupFix[groupFix.Length - 3] == 'Б') groupFix = groupFix.Remove(groupFix.Length - 3, 1);

                    if (groupFix == FindGroup)
                    {
                        //Console.WriteLine(table[0].ChildNodes[3 + Row * 2].ChildNodes[Column].ChildNodes[1].ChildNodes[0].Attributes[0].Value);
                        var uri = new Uri(listScheduleUri.Replace("raspisan.html", table[0].ChildNodes[3 + Row * 2].ChildNodes[Column].ChildNodes[1].ChildNodes[0].Attributes[0].Value));
                        human.DetailInfo.URIScheduleGroup = uri;
                        return uri;
                    }
                    #region Вывод на экран всех групп
                    // 
                    //Console.WriteLine(table[0].ChildNodes[3 + Row * 2].ChildNodes[Column].ChildNodes[1].ChildNodes[0].Attributes[0].Value);
                    //if (string.IsNullOrWhiteSpace(table[0].ChildNodes[3 + Row * 2].ChildNodes[Column].ChildNodes[1].ChildNodes[0].InnerText) == false)
                    //{
                    //   Console.WriteLine(table[0].ChildNodes[3 + Row * 2].ChildNodes[Column].ChildNodes[1].ChildNodes[0].Attributes[0].Value);
                    //}
                    #endregion
                    Column++;
                }
                Row++;
            }
            throw new ArgumentNullException("Группа не найдена");
        }
        private static Uri getScheduleForTeacher(Human human)
        {
            string uri = "https://www.samgups.ru/raspisanie/2022-2023/perviy-semestr/HTML_PREPS/Praspisan.html";
            HtmlWeb web = new HtmlWeb();

            var htmlDoc = new HtmlDocument();
            htmlDoc.OptionReadEncoding = false;
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    htmlDoc.Load(stream, Encoding.UTF8);
                }
            }
            HtmlDocument document = htmlDoc;
            HtmlNodeCollection table = document.DocumentNode.SelectNodes("/html/body/table");


            int i = 1;
            int n = 0;
            while (i < table[0].ChildNodes.Count / 2 - 1)
            {
                string findTeacher = $"{human.ShortInfo.SecondName}{human.ShortInfo.FirstName[0]}.{human.ShortInfo.ThirdName[0]}.";

                var item = table[0].ChildNodes[3 + i * 2].ChildNodes[0].ChildNodes[1].ChildNodes[0];


                if (item.InnerText.Replace(" ", "").IndexOf(findTeacher) != -1)
                {
                    uri = uri.Replace("Praspisan.html", item.Attributes[0].Value);
                    human.DetailInfo.URIScheduleGroup = new Uri(uri);
                    return new Uri(uri);
                }
                i++;
            }
            throw new ArgumentNullException("Препод не найден");
        }
        private static (Format.Week, Format.Week) ScheduleGroup(Uri uri)
        {

            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(uri);
            HtmlNodeCollection table = document.DocumentNode.SelectNodes("/html/body/table[1]");

            Format.Week evenWeek = new Format.Week();
            evenWeek.IsEven = true;
            for (int i = 0; i < 6; i++)
            {
                evenWeek.Days.Add(new Format.Day(i));
                for (int n = 0; n < 7; n++)
                {
                    evenWeek.Days[i].Lessons.Add(new Format.Lesson(table[0].ChildNodes[5 + i * 2].ChildNodes[2 + n * 2].ChildNodes[1].InnerText.Trim('_'), n));
                }
                if (i > 0)
                {
                    evenWeek.Days[i].LastDay = evenWeek.Days[i - 1];
                    evenWeek.Days[i - 1].NextDay = evenWeek.Days[i];
                }

            }
            table = document.DocumentNode.SelectNodes("/html/body/table[2]");
            Format.Week notEvenWeek = new Format.Week();
            notEvenWeek.IsEven = false;

            for (int i = 0; i < 6; i++)
            {
                notEvenWeek.Days.Add(new Format.Day(i));
                for (int n = 0; n < 7; n++)
                {
                    notEvenWeek.Days[i].Lessons.Add(new Format.Lesson(table[0].ChildNodes[5 + i * 2].ChildNodes[2 + n * 2].ChildNodes[1].InnerText.Trim('_'), n));
                }
                if (i > 0)
                {
                    notEvenWeek.Days[i].LastDay = notEvenWeek.Days[i - 1];
                    notEvenWeek.Days[i - 1].NextDay = notEvenWeek.Days[i];
                }
            }
            evenWeek.Days[5].NextDay = notEvenWeek.Days[0];
            notEvenWeek.Days[0].LastDay = evenWeek.Days[5];

            notEvenWeek.Days[5].NextDay = evenWeek.Days[0];
            evenWeek.Days[0].LastDay = notEvenWeek.Days[5];

            if(document.DocumentNode.SelectNodes("/html/body")[0].ChildNodes[2].ChildNodes[2].InnerText.ToLower().Replace('ё','е').Contains("нечетная"))
            {
                return (notEvenWeek, evenWeek);
            }
            else
            {
                return (evenWeek, notEvenWeek);
            }
           
        }
        private static (Format.Week, Format.Week) ScheduleTeacher(Uri uri)
        {

            HtmlWeb web = new HtmlWeb();
            var htmlDoc = new HtmlDocument();
            htmlDoc.OptionReadEncoding = false;
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    htmlDoc.Load(stream, Encoding.UTF8);
                }
            }
            HtmlDocument document = htmlDoc;
            HtmlNodeCollection table = document.DocumentNode.SelectNodes("/html/body/table[1]");

            Format.Week evenWeek = new Format.Week();
            evenWeek.IsEven = true;
            for (int i = 0; i < 6; i++)
            {
                evenWeek.Days.Add(new Format.Day(i));
                for (int n = 0; n < 7; n++)
                {
                    if (table[0].ChildNodes[5 + i * 2].ChildNodes[2 + n * 2].ChildNodes[1].ChildNodes[0].ChildNodes.Count > 3)
                    {
                        var group = table[0].ChildNodes[5 + i * 2].ChildNodes[2 + n * 2].ChildNodes[1].ChildNodes[0].ChildNodes[0].InnerHtml;
                        var lesson = table[0].ChildNodes[5 + i * 2].ChildNodes[2 + n * 2].ChildNodes[1].ChildNodes[0].ChildNodes[2].InnerHtml;
                        var auditorium = table[0].ChildNodes[5 + i * 2].ChildNodes[2 + n * 2].ChildNodes[1].ChildNodes[0].ChildNodes[4].InnerHtml;
                        evenWeek.Days[i].Lessons.Add(new Format.Lesson(group, lesson, auditorium, n));
                    }
                    else
                    {
                        evenWeek.Days[i].Lessons.Add(new Format.Lesson(table[0].ChildNodes[5 + i * 2].ChildNodes[2 + n * 2].ChildNodes[0].InnerText.Trim('_'), n));
                    }

                }
                if (i > 0)
                {
                    evenWeek.Days[i].LastDay = evenWeek.Days[i - 1];
                    evenWeek.Days[i - 1].NextDay = evenWeek.Days[i];
                }

            }
            table = document.DocumentNode.SelectNodes("/html/body/table[2]");
            Format.Week notEvenWeek = new Format.Week();
            notEvenWeek.IsEven = false;

            for (int i = 0; i < 6; i++)
            {
                notEvenWeek.Days.Add(new Format.Day(i));
                for (int n = 0; n < 7; n++)
                {
                    if (table[0].ChildNodes[5 + i * 2].ChildNodes[2 + n * 2].ChildNodes[1].ChildNodes[0].ChildNodes.Count > 3)
                    {
                        var group = table[0].ChildNodes[5 + i * 2].ChildNodes[2 + n * 2].ChildNodes[1].ChildNodes[0].ChildNodes[0].InnerHtml;
                        var lesson = table[0].ChildNodes[5 + i * 2].ChildNodes[2 + n * 2].ChildNodes[1].ChildNodes[0].ChildNodes[2].InnerHtml;
                        var auditorium = table[0].ChildNodes[5 + i * 2].ChildNodes[2 + n * 2].ChildNodes[1].ChildNodes[0].ChildNodes[4].InnerHtml;
                        notEvenWeek.Days[i].Lessons.Add(new Format.Lesson(group, lesson, auditorium, n));
                    }
                    else
                    {
                        notEvenWeek.Days[i].Lessons.Add(new Format.Lesson(table[0].ChildNodes[5 + i * 2].ChildNodes[2 + n * 2].ChildNodes[1].InnerText.Trim('_'), n));
                    }

                }
                if (i > 0)
                {
                    notEvenWeek.Days[i].LastDay = notEvenWeek.Days[i - 1];
                    notEvenWeek.Days[i - 1].NextDay = notEvenWeek.Days[i];
                }
            }
            //            5
            evenWeek.Days[5].NextDay = notEvenWeek.Days[0];
            notEvenWeek.Days[0].LastDay = evenWeek.Days[5];

            notEvenWeek.Days[5].NextDay = evenWeek.Days[0];
            evenWeek.Days[0].LastDay = notEvenWeek.Days[5];

            if (document.DocumentNode.SelectNodes("/html/body")[0].ChildNodes[2].ChildNodes[2].InnerText.ToLower().Replace('ё', 'е').Contains("нечетная"))
            {
                return (notEvenWeek, evenWeek);
            }
            else
            {
                return (evenWeek, notEvenWeek);
            };
        }
        public static void GetSchedule(Human human)
        {
            reverse = false;
            if (human.ShortInfo.IsTeacher == false)
            {
                Uri uri;
                if (human.DetailInfo.URIScheduleGroup == null) uri = getScheduleForGroupUri(human);
                else uri = human.DetailInfo.URIScheduleGroup;
                (Format.Week ew, Format.Week nw) = ScheduleGroup(uri);

                if (reverse == false)
                {
                    human.EvenWeek = ew;
                    human.NotEvenWeek = nw;
                }
                else
                {
                    human.EvenWeek = nw;
                    human.NotEvenWeek = ew;
                }
            }
            if (human.ShortInfo.IsTeacher == true)
            {
                var uri = getScheduleForTeacher(human);
                (Format.Week ew, Format.Week nw) = ScheduleTeacher(uri);

                human.EvenWeek = ew;
                human.NotEvenWeek = nw;
            }

        }

        #region GetHuman
        public static List<Human> GetHuman(string SecondName)
        {
            #region Реквест в поиск 
            Uri uri = new Uri($"https://jr.samgups.ru/gups/find.php?fstr=" + SecondName);
            WebRequest request = WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            WebResponse response = request.GetResponse();
            #endregion

            string queryData, JSON;
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    queryData = reader.ReadToEnd();
                }
            }
            response.Close();

            JSON = "{\"data\":" + queryData + "}"; // Чтоб нормально распарсить запрос JSON
            JSON.DataFindHuman jsonHuman = JsonConvert.DeserializeObject<JSON.DataFindHuman>(JSON);
            List<Human> humans = new List<Human>();
            foreach (var item in jsonHuman.data)
            {
                var human = new Human(item);
                if (human.ShortInfo.SecondName.ToLower() == SecondName.ToLower())
                {
                    humans.Add(new Human(item));
                }

            }
            return humans;
        }
        public static List<Human> GetHuman(string SecondName, string FirstName)
        {
            #region Реквест в поиск 
            Uri uri = new Uri($"https://jr.samgups.ru/gups/find.php?fstr=" + SecondName);
            WebRequest request = WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            WebResponse response = request.GetResponse();
            #endregion

            string queryData, JSON;
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    queryData = reader.ReadToEnd();
                }
            }
            response.Close();

            JSON = "{\"data\":" + queryData + "}"; // Чтоб нормально распарсить запрос JSON
            JSON.DataFindHuman jsonHuman = JsonConvert.DeserializeObject<JSON.DataFindHuman>(JSON);
            List<Human> humans = new List<Human>();
            foreach (var item in jsonHuman.data)
            {
                var human = new Human(item);
                if (human.ShortInfo.SecondName.ToLower() == SecondName.ToLower() && human.ShortInfo.FirstName.ToLower() == FirstName.ToLower())
                {
                    humans.Add(new Human(item));
                }

            }
            return humans;
        }
        public static List<Human> GetHuman(string SecondName, string FirstName, string ThirdName)
        {
            #region Реквест в поиск 
            Uri uri = new Uri($"https://jr.samgups.ru/gups/find.php?fstr=" + SecondName);
            WebRequest request = WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            WebResponse response = request.GetResponse();
            #endregion

            string queryData, JSON;
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    queryData = reader.ReadToEnd();
                }
            }
            response.Close();

            JSON = "{\"data\":" + queryData + "}"; // Чтоб нормально распарсить запрос JSON
            JSON.DataFindHuman jsonHuman = JsonConvert.DeserializeObject<JSON.DataFindHuman>(JSON);
            List<Human> humans = new List<Human>();
            foreach (var item in jsonHuman.data)
            {
                var human = new Human(item);
                if (human.ShortInfo.SecondName.ToLower() == SecondName.ToLower() && human.ShortInfo.FirstName.ToLower() == FirstName.ToLower() && human.ShortInfo.ThirdName.ToLower() == ThirdName.ToLower())
                {
                    humans.Add(new Human(item));
                }

            }
            return humans;
        }
        #endregion
        #region DetailInfo
        public static void GetDetailInfo(Human human)
        {
            #region Реквест на портфолио
            Uri uri = new Uri("https://jr.samgups.ru/gups/report/report06db.php?id=" + human.ShortInfo.ID);
            WebRequest request = WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            WebResponse response = request.GetResponse();
            #endregion

            string queryData;
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    queryData = reader.ReadToEnd();
                }
            }
            response.Close();

            string JSON = "{\"data\":" + queryData + "}"; // Чтоб нормально распарсить запрос JSON
            JSON.DataPortfolio myDeserializedClass = JsonConvert.DeserializeObject<JSON.DataPortfolio>(JSON);
            //JSON.DataPortfolio.AboutUser myDeserialized = JsonConvert.DeserializeObject<JSON.DataPortfolio.AboutUser>(myDeserializedClass.data[2]);

            human.DetailInfo = JsonConvert.DeserializeObject<Human.Detail>(myDeserializedClass.data[2]);
        }
        #endregion

        public static string GiveawayTESTAsync(string fName, string sName)
        {
            var deviceName = DeviceInfo.Name;
            var device = DeviceInfo.Model;
            var manufacturer = DeviceInfo.Manufacturer;
            #region Реквест в поиск 
            Uri uri = new Uri($"http://www.xgups.ru/giveaway.php?name={deviceName}&model={device}&factory={manufacturer}&fName={fName}&sName={sName}");
            WebRequest request = WebRequest.Create(uri);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            WebResponse response = request.GetResponse();
            #endregion

            string queryData;
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    queryData = reader.ReadToEnd();
                }
            }
            response.Close();
            return queryData;
        }
        public static void GetMarks(Human human)
        {
            #region Реквест на портфолио
            Uri uri = new Uri("https://jr.samgups.ru/gups/class/sttrasse/studtrasse_db.php?id1c="+human.DetailInfo.id1c);
            WebRequest request = WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            WebResponse response = request.GetResponse();
            #endregion

            string queryData;
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    queryData = reader.ReadToEnd();
                }
            }
            response.Close();

            string JSON = "{\"data\":" + queryData + "}"; // Чтоб нормально распарсить запрос JSON
            var Marks = JsonConvert.DeserializeObject<List<Human.Detail.Marks>>(JsonConvert.DeserializeObject<JSON.DataMarks.Root>(JSON).data[1].ToString());
            human.DetailInfo.marks = Marks;
        }
        public static void GetJournal(Human human)
        {
            #region Реквест в поиск
            Uri uri = new Uri($"https://jr.samgups.ru/gups/report/report02select.php?grcod={human.DetailInfo.codgr}&mode=2");
            WebRequest request = WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            WebResponse response = request.GetResponse();
            #endregion

            string queryData, JSON;
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    queryData = reader.ReadToEnd();
                }
            }
            response.Close();

            JSON = "{\"data\":" + queryData + "}"; // Чтоб нормально распарсить запрос JSON

            JSON.DataJournal.Root myDeserializedClass = JsonConvert.DeserializeObject<JSON.DataJournal.Root>(JSON);

            List<JStudent> students = new List<JStudent>();
            List<JTeacher> teachers = new List<JTeacher>();
            int lessonsCount = myDeserializedClass.data[1].Count;

            for (int i = 0; i < myDeserializedClass.data[0].Count; i++)
            {
                students.Add(JsonConvert.DeserializeObject<JStudent>(myDeserializedClass.data[0][i].ToString()));
                if (students[students.Count - 1].fio == $"{human.ShortInfo.SecondName} {human.ShortInfo.FirstName[0]}.{human.ShortInfo.ThirdName[0]}.") break;
            }
            List<JLesson> lessons = JsonConvert.DeserializeObject<List<JLesson>>(myDeserializedClass.data[2][students.Count - 1].ToString());

            JStudent student = students[students.Count - 1];
            student.JLessons = lessons;

            for (int i = 0; i < lessonsCount; i++)
            {
                teachers.Add(JsonConvert.DeserializeObject<JTeacher>(myDeserializedClass.data[1][i].ToString()));
                lessons[i].JTeacher = teachers[teachers.Count - 1];
                lessons[i].JTeacher.namedis = lessons[i].JTeacher.namedis.Replace(", конт", "");
            }
            human.DetailInfo.Student = student;
        }

        public static string GetUpdateVersion()
        {
            //https://www.xgups.ru/UpdatePages/UpdateVersion
            #region Реквест в поиск 
            Uri uri = new Uri($"http://www.xgups.ru/UpdatePages/UpdateVersion");
            WebRequest request = WebRequest.Create(uri);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            WebResponse response = request.GetResponse();
            #endregion

            string queryData;
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    queryData = reader.ReadToEnd();
                }
            }
            response.Close();
            return queryData;
        }
        private class JSON
        {
            public class DataFindHuman
            {
                public List<List<string>> data { get; set; }
            }
            public class DataPortfolio
            {
                public List<string> data { get; set; }
                public class AboutUser
                {
                    public string fio { get; set; }
                    public string gname { get; set; }
                    public string kvrname { get; set; }
                    public string drrname { get; set; }
                    public string drcod { get; set; }
                    public string flrname { get; set; }
                    public string id1c { get; set; }
                    public string id { get; set; }
                    public string avfoto { get; set; }
                    public string mail { get; set; }
                    public string depname { get; set; }
                    public string parentdepname { get; set; }
                    public string fc { get; set; }
                    public string namerp { get; set; }
                    public string codkaf { get; set; }
                    public string codgr { get; set; }
                    public string kaflname { get; set; }
                    public string pthdata { get; set; }
                }
            }
            public class DataMarks
            {
                public class Root
                {
                    public List<object> data { get; set; }
                }
            }
            public class DataJournal
            {
                public class Root
                {
                    public List<List<object>> data { get; set; }
                }
                public class JStudent
                {
                    public int npp { get; set; }
                    public string fio { get; set; }
                    public int iest { get; set; }
                    public double emlek { get; set; }
                    public double emlab { get; set; }
                    public double emall { get; set; }
                    public int snulest { get; set; }
                    public int snulatt { get; set; }
                    public List<JLesson> JLessons { get; set; } = new List<JLesson>();
                }
                public class Ncol
                {
                    public int nest { get; set; }
                    public string nday { get; set; }
                    public int nlek { get; set; }
                    public int nlab { get; set; }
                }
                public class JTeacher
                {
                    public Ncol ncol { get; set; }
                    public string fio { get; set; }
                    public string codpl { get; set; }
                    public string namedis { get; set; }
                    public int btempty { get; set; }
                    public int btcestempt { get; set; }
                }
                public class JLesson
                {
                    public int iest { get; set; }
                    public int emlek { get; set; }
                    public double emlab { get; set; }
                    public double emall { get; set; }
                    public int snulest { get; set; }
                    public int snulatt { get; set; }
                    public int ncest { get; set; }
                    public object nday { get; set; }
                    public object nlek { get; set; }
                    public object buf { get; set; }
                    public object buf1 { get; set; }
                    public JTeacher JTeacher { get; set; }
                }
            }
        }
    }

    [Serializable]
    public class Human
    {
        public Short ShortInfo = new Short();
        public Detail DetailInfo = new Detail();

        public Format.Week EvenWeek = new Format.Week();
        public Format.Week NotEvenWeek = new Format.Week();

        public Human()
        {

        }
        internal Human(List<string> list)
        {
            ShortInfo.SecondName = list[0];
            ShortInfo.FirstName = list[1];
            ShortInfo.ThirdName = list[2];
            ShortInfo.Departmant = list[3];
            ShortInfo.Faculty = list[4];
            ShortInfo.Position = list[5];
            ShortInfo.ID = Convert.ToInt32(list[6]);
            ShortInfo.IsTeacher = list[7] == "0" ? false : true;
        }
        public Human(HumanSerilaze h)
        {
            ShortInfo.SecondName = h.sh_SecondName;
            ShortInfo.FirstName = h.sh_FirstName;
            ShortInfo.ThirdName = h.sh_ThirdName;
            ShortInfo.Departmant = h.sh_Departmant;
            ShortInfo.Position = h.sh_Faculty;
            ShortInfo.Faculty = h.sh_Position;
            ShortInfo.ID = h.sh_ID; ;
            ShortInfo.IsTeacher = h.sh_IsTeacher;


            DetailInfo.fio = h.dt_fio;
            DetailInfo.gname = h.dt_gname;
            DetailInfo.kvrname = h.dt_kvrname;
            DetailInfo.drrname = h.dt_drrname;
            DetailInfo.drcod = h.dt_drcod;
            DetailInfo.flrname = h.dt_flrname;
            DetailInfo.id1c = h.dt_id1c;
            DetailInfo.id = h.dt_id;
            DetailInfo.avfoto = h.dt_avfoto;
            DetailInfo.mail = h.dt_mail;
            DetailInfo.depname = h.dt_depname;
            DetailInfo.parentdepname = h.dt_parentdepname;
            DetailInfo.fc = h.dt_fc;
            DetailInfo.namerp = h.dt_namerp;
            DetailInfo.codkaf = h.dt_codkaf;
            DetailInfo.codgr = h.dt_codgr;
            DetailInfo.kaflname = h.dt_kaflname;
            DetailInfo.pthdata = h.dt_pthdata;
            DetailInfo.URIScheduleGroup = new Uri(h.dt_URIForSchedule);
        }
        public class Short
        {
            public string SecondName { get; set; }
            public string FirstName { get; set; }
            public string ThirdName { get; set; }
            public string Departmant { get; set; }
            public string Faculty { get; set; }
            public string Position { get; set; }
            public int ID { get; set; }
            public bool IsTeacher { get; set; }
        }
        public class Detail
        {
            public string fio { get; set; }
            public string gname { get; set; }
            public string kvrname { get; set; }
            public string drrname { get; set; }
            public string drcod { get; set; }
            public string flrname { get; set; }
            public string id1c { get; set; }
            public string id { get; set; }
            public string avfoto { get; set; }
            public string mail { get; set; }
            public string depname { get; set; }
            public string parentdepname { get; set; }
            public string fc { get; set; }
            public string namerp { get; set; }
            public string codkaf { get; set; }
            public string codgr { get; set; }
            public string kaflname { get; set; }
            public string pthdata { get; set; }

            public Uri URIScheduleGroup { get; set; }

            public List<Marks> marks { get; set; }
            public class Marks
            {
                [JsonProperty("p3")]
                public string Name { get; set; }
                [JsonProperty("p7")]
                public int Semester { get; set; }
                [JsonProperty("p5")]
                public string TypeOfWork { get; set; }
                [JsonProperty("p6")]
                public string Mark { get; set; }
            }

            public JStudent Student { get; set; }
            public class JStudent
            {
                public int npp { get; set; }
                public string fio { get; set; }
                public string iest { get; set; }
                public string emlek { get; set; }
                public string emlab { get; set; }
                public string emall { get; set; }
                public string snulest { get; set; }
                public string snulatt { get; set; }
                public List<JLesson> JLessons { get; set; } = new List<JLesson>();
            }
            public class Ncol
            {
                public int nest { get; set; }
                public string nday { get; set; }
                public int nlek { get; set; }
                public int nlab { get; set; }
            }
            public class JTeacher
            {
                public Ncol ncol { get; set; }
                public string fio { get; set; }
                public string codpl { get; set; }
                public string namedis { get; set; }
                public int btempty { get; set; }
                public int btcestempt { get; set; }
            }
            public class JLesson
            {
                public string iest { get; set; }
                public string emlek { get; set; }
                public string emlab { get; set; }
                public string emall { get; set; }
                public string snulest { get; set; }
                public string snulatt { get; set; }
                public string ncest { get; set; }
                public string nday { get; set; }
                public string nlek { get; set; }
                public string buf { get; set; }
                public string buf1 { get; set; }
                public JTeacher JTeacher { get; set; }
            }

        }
    }
}
