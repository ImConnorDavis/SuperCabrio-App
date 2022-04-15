using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using static SuperCabrio.Pages.HomePages;

namespace SuperCabrio.Pages
{
    public partial class PostPage : ContentPage
    {
        public bool VisibleSet { get; set; }
        public int PageControl { get; set; }
        public long PostCheck { get; set; }
        public int PageCheck { get; set; }
        public int PageCounter { get; set; }
        public PostPage()
        {
            InitializeComponent();
            PageControl = 0;
            PageCounter = 1;
            //PageCounter = PageControl.ToString()[0];
            //PageLbl.Text = PageCounter.ToString();
            PageLbl.Text = PageCounter.ToString();
            if (Application.Current.Properties.ContainsKey("FirstUse"))
            {
                //Do things if it's NOT the first run of the app...
            }
            else
            {
                //BindingContext = new PostControl();
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            BindingContext = new PostControl();
            if (PostControl.PostCount <= 0)
            {
                //PostPageLayout.IsVisible = false;
                PostList.IsVisible = false;
                NoPostLbl.IsVisible = true;
            }
            else
            {
                NoPostLbl.IsVisible = false;
                //PostPageLayout.IsVisible = true;
                PostList.IsVisible = true;
            }
        }
        async void PostClicked(object sender, ItemTappedEventArgs e)
        {
            var mySelectedItem = e.Item as Post;
            await Navigation.PushAsync(new ViewPost(mySelectedItem.Data, mySelectedItem.Name, mySelectedItem.Votes, mySelectedItem.Location, mySelectedItem.Address, mySelectedItem.TimePosted, mySelectedItem.ID));
        }
        async void NextBtn(object sender, EventArgs e)
        {
            PageCheck = PostControl.PageControl + PostControl.RunControl;
            PostCheck = PostControl.PostCount;
            if (PageCheck < PostCheck)
            {
                Console.WriteLine("Next");
                Console.WriteLine(PageCheck);
                Console.WriteLine(PostCheck);
                PageControl = PageControl + 10;
                PageCounter = PageCounter + 1;
                PageLbl.Text = PageCounter.ToString();
                PostControl.PageControl = PageControl;
                //new PostControl();
                BindingContext = new PostControl();
                //await Navigation.PopAsync();
                //await Navigation.PushAsync(new PostPage());
            }
        }
        async void PreviousBtn(object sender, EventArgs e)
        {
            if (PageControl >= 10)
            {
                PageControl = PageControl - 10;
                PageCounter = PageCounter - 1;
                PageLbl.Text = PageCounter.ToString();
                PostControl.PageControl = PageControl;
                //new PostControl();
                BindingContext = new PostControl();
                //await Navigation.PopAsync();
                //await Navigation.PushAsync(new PostPage());
            }
        }
    }
}
