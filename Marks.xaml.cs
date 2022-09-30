using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XGups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Marks : ContentPage
    {
        Human human;
        public int AllSemestr = 0;
        public int NowSemestr = 0;
        public ObservableCollection<Human.Detail.Marks> Mark { get; set; } = new ObservableCollection<Human.Detail.Marks>();
        public Marks()
        {
            InitializeComponent();
            BindingContext = this; 
        }
        public Marks(Human human)
        {
            this.human = human;
            if(human.DetailInfo.marks == null) Request.GetMarks(human);

            InitializeComponent();
            BindingContext = this;

            foreach (var item in human.DetailInfo.marks)
            {
                AllSemestr = item.Semester;
                if (item.Mark.Length > 3) NowSemestr = item.Semester;
            }
            for (int i = 1; i <= AllSemestr; i++)
            {   
                if(i - 1 == NowSemestr)
                {
                    PickerSemestr.Items.Add(i.ToString() + "     (Текущий семестр)");
                }
                else
                {
                    PickerSemestr.Items.Add(i.ToString());
                }
            }

            foreach (var item in human.DetailInfo.marks)
            {
                Mark.Add(item);
            }
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var sem = PickerSemestr.SelectedIndex + 1;

            Mark.Clear();
            foreach (var item in human.DetailInfo.marks)
            {
                if (sem == item.Semester)
                {
                    if(sem <= NowSemestr && item.Mark.Length < 3)
                    {
                    }
                    else
                    {
                        Mark.Add(item);
                    }
                    
                }
            }
        }
    }
}