using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using TestApp.Shared;

namespace TestApp.Droid
{
    [Activity(Label = "TestApp.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += ShowUTC;
        }

        async private void ShowUTC(object sender, EventArgs e)
        {
            NodeHelper helper = new NodeHelper();
            TimerAmination timerAnimation = new TimerAmination();

            Button button = FindViewById<Button>(Resource.Id.MyButton);
            TextView responseTime = FindViewById<TextView>(Resource.Id.ResponseTime);
            TextView differenceTime = FindViewById<TextView>(Resource.Id.DifferenceServerTime);
            TextView currentTime = FindViewById<TextView>(Resource.Id.CurrentTime);

            timerAnimation.UpdateNumber += timerAnimation_UpdateNumber;

            var response = await helper.Calculate();
            var story = helper.GetAnimationStory();

            button.Text = response.ServerTime.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
            responseTime.Text = response.ResponseMilliSeconds.ToString();
            differenceTime.Text = response.DifferenceMilliSeconds.ToString();
            currentTime.Text = response.CurrentTime.ToString("MM/dd/yyyy hh:mm:ss.fff tt");

            timerAnimation.Start(story.StartDateTime, response.DifferenceMilliSeconds);
        }

        void timerAnimation_UpdateNumber(object sender, int e)
        {
			try
			{
				TextView currentTime = FindViewById<TextView>(Resource.Id.Seconds);
				RunOnUiThread (() => currentTime.Text = e.ToString());
			}
			catch(Exception ee) {
				var ii = ee.ToString ();
			}
        }
    }
}

