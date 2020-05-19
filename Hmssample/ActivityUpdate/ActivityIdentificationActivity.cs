/*
*       Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.

        Licensed under the Apache License, Version 2.0 (the "License");
        you may not use this file except in compliance with the License.
        You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

        Unless required by applicable law or agreed to in writing, software
        distributed under the License is distributed on an "AS IS" BASIS,
        WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
        See the License for the specific language governing permissions and
        limitations under the License.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

using Com.Huawei.Hms.Location;
using Com.Huawei.Hmf.Tasks;

using XLocationDemoProjectRef.Logger;
using XLocationDemoProjectRef.Hmssample.Location.Fusedlocation;

namespace XLocationDemoProjectRef.Hmssample.ActivityUpdate
{
    [Activity(Label = "ActivityIdentificationActivity")]
    public class ActivityIdentificationActivity : LocationBaseActivity, View.IOnClickListener
    {
        public static readonly string TAG = "ActivityTransitionUpdate";

        public ActivityIdentificationService activityIdentificationService;

        public static LinearLayout.LayoutParams Type0, Type1, Type2, Type3, Type4, Type5, Type7, Type8;

        public static LinearLayout activityInVehicle, activityOnBicycle, activityOnFoot, activityStill, 
            activityUnknown, activityTilting, activityWalking, activityRunning;

        private PendingIntent pendingIntent;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_transition_type);
            
            activityIdentificationService = ActivityIdentification.GetService(this);
            RequestPermission.RequestActivityPermission(this);
            // RequestPermission.RequestActivityTransitionPermission(this);
            FindViewById(Resource.Id.requestActivityTransitionUpdate).SetOnClickListener(this);
            FindViewById(Resource.Id.removeActivityTransitionUpdate).SetOnClickListener(this);
            activityInVehicle = (LinearLayout)FindViewById(Resource.Id.activityInVehicle);
            Type0 = (LinearLayout.LayoutParams)activityInVehicle.LayoutParameters;
            activityOnBicycle = (LinearLayout)FindViewById(Resource.Id.activityOnBicycle);
            Type1 = (LinearLayout.LayoutParams)activityOnBicycle.LayoutParameters;
            activityOnFoot = (LinearLayout)FindViewById(Resource.Id.activityOnFoot);
            Type2 = (LinearLayout.LayoutParams)activityOnFoot.LayoutParameters;
            activityStill = (LinearLayout)FindViewById(Resource.Id.activityStill);
            Type3 = (LinearLayout.LayoutParams)activityStill.LayoutParameters;
            activityUnknown = (LinearLayout)FindViewById(Resource.Id.activityUnknown);
            Type4 = (LinearLayout.LayoutParams)activityUnknown.LayoutParameters;
            activityTilting = (LinearLayout)FindViewById(Resource.Id.activityTilting);
            Type5 = (LinearLayout.LayoutParams)activityTilting.LayoutParameters;
            activityWalking = (LinearLayout)FindViewById(Resource.Id.activityWalking);
            Type7 = (LinearLayout.LayoutParams)activityWalking.LayoutParameters;
            activityRunning = (LinearLayout)FindViewById(Resource.Id.activityRunning);
            Type8 = (LinearLayout.LayoutParams)activityRunning.LayoutParameters;
            AddLogFragment();
            Reset();
        }

        public void RequestActivityUpdates(long detectionIntervalMillis)
        {
            try
            {
                if(pendingIntent != null)
                {
                    RemoveActivityUpdates();
                }
                pendingIntent = GetPendingIntent();
                LocationBroadcastReceiver.AddIdentificationListener();
                activityIdentificationService.CreateActivityIdentificationUpdates(detectionIntervalMillis, pendingIntent)
                    .AddOnSuccessListener(new ReqActivityUpdatesOnSuccessListenerImpl())
                    .AddOnFailureListener(new ReqActivityUpdatesOnFailureListenerImpl());
            }
            catch (System.Exception e)
            {
                LocationLog.Error(TAG, "createActivityIdentificationUpdates exception:" + e.Message);
            }
        }

        public void RemoveActivityUpdates()
        {
            Reset();
            try
            {
                LocationBroadcastReceiver.RemoveIdentificationListener();
                Log.Info(TAG, "start to removeActivityUpdates");
                activityIdentificationService.DeleteActivityIdentificationUpdates(pendingIntent)
                    .AddOnSuccessListener(new RemoveActivityUpdatesOnSuccessListenerImpl())
                    .AddOnFailureListener(new RemoveActivityUpdatesOnFailureListenerImpl());
            }
            catch (System.Exception e)
            {
                LocationLog.Error(TAG, "removeActivityUpdates exception:" + e.Message);
            }
        }

        private PendingIntent GetPendingIntent()
        {
            Intent intent = new Intent(this, typeof(LocationBroadcastReceiver));
            intent.SetAction(LocationBroadcastReceiver.ACTION_PROCESS_LOCATION);
            return PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.UpdateCurrent);
        }

        protected override void OnDestroy()
        {
            if (pendingIntent != null)
            {
                RemoveActivityUpdates();
            }
            base.OnDestroy();
        }

        public void OnClick(View v)
        {
            try
            {
                switch (v.Id)
                {
                    case Resource.Id.requestActivityTransitionUpdate:
                        RequestActivityUpdates(5000);
                        break;
                    case Resource.Id.removeActivityTransitionUpdate:
                        RemoveActivityUpdates();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                LocationLog.Error(TAG, "RequestLocationUpdatesWithCallbackActivity Exception:" + e);
            }
        }

        public static void SetData(IList<ActivityIdentificationData> list)
        {
            Reset();
            for (int i = 0; i < list.Count; i++)
            {
                int type = list.ElementAt(i).IdentificationActivity;
                int value = list.ElementAt(i).Possibility;
                try
                {
                    switch (type)
                    {
                        case 100:
                            Type0.Width = Type0.Width + value * 6;
                            activityInVehicle.LayoutParameters = Type0;
                            break;
                        case 101:
                            Type1.Width = Type1.Width + value * 6;
                            activityOnBicycle.LayoutParameters = Type0;
                            break;
                        case 102:
                            Type2.Width = Type2.Width + value * 6;
                            activityOnFoot.LayoutParameters = Type0;
                            break;
                        case 103:
                            Type3.Width = Type3.Width + value * 6;
                            activityStill.LayoutParameters = Type0;
                            break;
                        case 104:
                            Type4.Width = Type4.Width + value * 6;
                            activityUnknown.LayoutParameters = Type0;
                            break;
                        case 105:
                            Type5.Width = Type5.Width + value * 6;
                            activityTilting.LayoutParameters = Type0;
                            break;
                        case 107:
                            Type7.Width = Type7.Width + value * 6;
                            activityWalking.LayoutParameters = Type0;
                            break;
                        case 108:
                            Type8.Width = Type8.Width + value * 6;
                            activityRunning.LayoutParameters = Type0;
                            break;
                    }
                }
                catch (System.Exception e)
                {
                    LocationLog.Error("ActivityTransitionUpdate", "setdata Exception");
                }
            }
        }

        public static void Reset()
        {
            Type0.Width = 100;
            activityInVehicle.LayoutParameters = Type0;
            Type1.Width = 100;
            activityOnBicycle.LayoutParameters = Type1;
            Type2.Width = 100;
            activityOnFoot.LayoutParameters = Type2;
            Type3.Width = 100;
            activityStill.LayoutParameters = Type3;
            Type4.Width = 100;
            activityUnknown.LayoutParameters = Type4;
            Type5.Width = 100;
            activityTilting.LayoutParameters = Type5;
            Type7.Width = 100;
            activityWalking.LayoutParameters = Type7;
            Type8.Width = 100;
            activityRunning.LayoutParameters = Type8;
        }
    }

    class ReqActivityUpdatesOnSuccessListenerImpl : Java.Lang.Object, IOnSuccessListener
    {
        public static readonly string TAG = "RequestActivityTransitionUpdate:SuccessListenerImpl";
        public void OnSuccess(Java.Lang.Object activityObj)
        {
            LocationLog.Info(TAG, "createActivityIdentificationUpdates onSuccess");
        }
    }

    class ReqActivityUpdatesOnFailureListenerImpl : Java.Lang.Object, IOnFailureListener
    {
        public static readonly string TAG = "RequestActivityTransitionUpdate:FailureListenerImpl";
        public void OnFailure(Java.Lang.Exception e)
        {
            LocationLog.Error(TAG, "createActivityIdentificationUpdates onFailure:" + e.Message);
        }
    }

    class RemoveActivityUpdatesOnSuccessListenerImpl : Java.Lang.Object, IOnSuccessListener
    {
        public static readonly string TAG = "RemoveActivityTransitionUpdate:SuccessListenerImpl";
        public void OnSuccess(Java.Lang.Object activityObj)
        {
            LocationLog.Info(TAG, "deleteActivityIdentificationUpdates onSuccess");
        }
    }

    class RemoveActivityUpdatesOnFailureListenerImpl : Java.Lang.Object, IOnFailureListener
    {
        public static readonly string TAG = "RemoveActivityTransitionUpdate:FailureListenerImpl";
        public void OnFailure(Java.Lang.Exception e)
        {
            LocationLog.Error(TAG, "deleteActivityIdentificationUpdates onFailure:" + e.Message);
        }
    }
}