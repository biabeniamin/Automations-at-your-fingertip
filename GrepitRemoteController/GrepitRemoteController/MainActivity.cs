using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Org.Apache.Http.Client;
using System.Net.Http;

namespace GrepitRemoteController
{
    [Activity(Label = "GrepitRemoteController", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);
            /*button.Click += delegate {
                Button_Click(1);
                button.Text = string.Format("{0} clicks!", count++);
            } ;*/
            FindViewById<Button>(Resource.Id.MyButton).Click += delegate { Button_Click(1); } ;
            FindViewById<Button>(Resource.Id.MyButton2).Click += delegate { Button_Click(2); };
            FindViewById<Button>(Resource.Id.MyButton3).Click += delegate { Button_Click(3); };
            FindViewById<Button>(Resource.Id.MyButton4).Click += delegate { Button_Click(4); };
            FindViewById<Button>(Resource.Id.MyButton5).Click += delegate { Button_Click(5); };
        }

        private void Button_Click(int pin)
        {
            HttpClient client = new HttpClient();
            client.GetStringAsync(string.Format("http://192.168.0.108?cmd={0}",pin));
        }
    }
}

