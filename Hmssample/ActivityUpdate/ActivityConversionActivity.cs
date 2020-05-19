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
    [Activity(Label = "ActivityTransitionConvert")]
    public class ActivityConversionActivity : LocationBaseActivity, View.IOnClickListener
    {
        public static readonly string TAG = "ActivityConversionUpdate";

        private CheckBox IN_VEHICLE_IN, WALKING_IN, WALKING_OUT,
        IN_VEHICLE_OUT, ON_BICYCLE_IN, ON_BICYCLE_OUT, ON_FOOT_IN, ON_FOOT_OUT, STILL_IN, STILL_OUT, RUNNING_IN,
        RUNNING_OUT;

        public ActivityIdentificationService activityIdentificationService;

        public ActivityConversionRequest activityTransitionRequest;

        public List<ActivityConversionInfo> transitions;

        private PendingIntent pendingIntent;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_transition);
            activityIdentificationService = ActivityIdentification.GetService(this);
            RequestPermission.RequestActivityPermission(this);
            IN_VEHICLE_IN = (CheckBox)FindViewById(Resource.Id.IN_VEHICLE_IN);
            IN_VEHICLE_OUT = (CheckBox)FindViewById(Resource.Id.IN_VEHICLE_OUT);
            ON_BICYCLE_IN = (CheckBox)FindViewById(Resource.Id.ON_BICYCLE_IN);
            ON_BICYCLE_OUT = (CheckBox)FindViewById(Resource.Id.ON_BICYCLE_OUT);
            ON_FOOT_IN = (CheckBox)FindViewById(Resource.Id.ON_FOOT_IN);
            ON_FOOT_OUT = (CheckBox)FindViewById(Resource.Id.ON_FOOT_OUT);
            STILL_IN = (CheckBox)FindViewById(Resource.Id.STILL_IN);
            STILL_OUT = (CheckBox)FindViewById(Resource.Id.STILL_OUT);
            WALKING_IN = (CheckBox)FindViewById(Resource.Id.WALKING_IN);
            WALKING_OUT = (CheckBox)FindViewById(Resource.Id.WALKING_OUT);
            RUNNING_IN = (CheckBox)FindViewById(Resource.Id.RUNNING_IN);
            RUNNING_OUT = (CheckBox)FindViewById(Resource.Id.RUNNING_OUT);
            FindViewById(Resource.Id.btnSubmit).SetOnClickListener(this);
            FindViewById(Resource.Id.btnMove).SetOnClickListener(this);
            AddLogFragment();
        }

        public void GetRequest()
        {
            transitions = new List<ActivityConversionInfo>();
            ActivityConversionInfo.Builder activityTransition = new ActivityConversionInfo.Builder();
            RequestValueResult requestValueResult = new RequestValueResult();
            if (IN_VEHICLE_IN.Checked)
                requestValueResult.AddList(100, 0);
            if (IN_VEHICLE_OUT.Checked)
                requestValueResult.AddList(100, 1);
            if (ON_BICYCLE_IN.Checked)
                requestValueResult.AddList(101, 0);
            if (ON_BICYCLE_OUT.Checked)
                requestValueResult.AddList(101, 1);
            if (ON_FOOT_IN.Checked)
                requestValueResult.AddList(102, 0);
            if (ON_FOOT_OUT.Checked)
                requestValueResult.AddList(102, 1);
            if (STILL_IN.Checked)
                requestValueResult.AddList(103, 0);
            if (STILL_OUT.Checked)
                requestValueResult.AddList(103, 1);
            if (WALKING_IN.Checked)
                requestValueResult.AddList(107, 0);
            if (WALKING_OUT.Checked)
                requestValueResult.AddList(107, 1);
            if (RUNNING_IN.Checked)
                requestValueResult.AddList(108, 0);
            if (RUNNING_OUT.Checked)
                requestValueResult.AddList(108, 1);
            List<RequestValue> result = requestValueResult.result;
            for (int i = 0; i < result.Count; i++)
            {
                RequestValue temp = result.ElementAt(i);
                activityTransition.SetActivityType(temp.ActivityType);
                activityTransition.SetConversionType(temp.ActivityTransition);
                transitions.Add(activityTransition.Build());
            }
            Log.Debug(TAG, "transitions size is " + transitions.Count);
        }

        public void RequestActivityTransitionUpdates()
        {
            try
            {
                if (pendingIntent != null)
                {
                    RemoveActivityTransitionUpdates();
                }
                LocationBroadcastReceiver.AddConversionListener();
                pendingIntent = GetPendingIntent();
                activityTransitionRequest = new ActivityConversionRequest(transitions);
                Task task = activityIdentificationService.CreateActivityConversionUpdates(activityTransitionRequest, pendingIntent);
                task
                    .AddOnSuccessListener(new ReqActivityTransUpdatesOnSuccessListenerImpl())
                    .AddOnFailureListener(new ReqActivityTransUpdatesOnFailureListenerImpl());
            }
            catch (Exception e)
            {
                LocationLog.Error(TAG, "createActivityConversionUpdates exception:" + e.Message);
            }
        }

        public void RemoveActivityTransitionUpdates()
        {
            try
            {
                LocationBroadcastReceiver.RemoveConversionListener();
                activityIdentificationService.DeleteActivityConversionUpdates(pendingIntent)
                    .AddOnSuccessListener(new RemoveActivityTransUpdatesOnSuccessListenerImpl())
                    .AddOnFailureListener(new RemoveActivityTransUpdatesOnFailureListenerImpl());
            } 
            catch (Exception e)
            {
                LocationLog.Error(TAG, "removeActivityTransitionUpdates exception:" + e.Message);
            }
        }

        protected override void OnDestroy()
        {
            if (pendingIntent != null)
            {
                RemoveActivityTransitionUpdates();
            }
            base.OnDestroy();
        }

        private PendingIntent GetPendingIntent()
        {
            Intent intent = new Intent(this, typeof(LocationBroadcastReceiver));
            intent.SetAction(LocationBroadcastReceiver.ACTION_PROCESS_LOCATION);
            return PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.UpdateCurrent);
        }

        public void OnClick(View v)
        {
            try
            {
                switch (v.Id)
                {
                    case Resource.Id.btnSubmit:
                        GetRequest();
                        RequestActivityTransitionUpdates();
                        break;
                    case Resource.Id.btnMove:
                        RemoveActivityTransitionUpdates();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                LocationLog.Error(TAG, "RequestLocationUpdatesWithCallbackActivity Exception:" + e.Message);
            }
        }

        class RequestValue
        {
            public int ActivityType;

            public int ActivityTransition;

            public RequestValue(int a, int b)
            {
                ActivityType = a;
                ActivityTransition = b;
            }
        }

        class RequestValueResult
        {
            public List<RequestValue> result = new List<RequestValue>();

            public void AddList(int activityType, int activityTransition)
            {
                RequestValue temp = new RequestValue(activityType, activityTransition);
                result.Add(temp);
            }
        }
    }

    class ReqActivityTransUpdatesOnSuccessListenerImpl : Java.Lang.Object, IOnSuccessListener
    {
        public static readonly string TAG = "RequestActivityConversionUpdate:SuccessListenerImpl";

        public void OnSuccess(Java.Lang.Object activityObj)
        {
            LocationLog.Info(TAG, "createActivityConversionUpdates onSuccess");
        }
    }

    class ReqActivityTransUpdatesOnFailureListenerImpl : Java.Lang.Object, IOnFailureListener
    {
        public static readonly string TAG = "RequestActivityConversionUpdate:FailureListenerImpl";

        public void OnFailure(Java.Lang.Exception e)
        {
            LocationLog.Error(TAG, "createActivityConversionUpdates onFailure:" + e.Message);
        }
    }

    class RemoveActivityTransUpdatesOnSuccessListenerImpl : Java.Lang.Object, IOnSuccessListener
    {
        public static readonly string TAG = "RemoveActivityConversionUpdate:SuccessListenerImpl";

        public void OnSuccess(Java.Lang.Object activityObj)
        {
            LocationLog.Info(TAG, "deleteActivityConversionUpdates onSuccess");
        }
    }

    class RemoveActivityTransUpdatesOnFailureListenerImpl : Java.Lang.Object, IOnFailureListener
    {
        public static readonly string TAG = "RemoveActivityConversionUpdate:FailureListenerImpl";

        public void OnFailure(Java.Lang.Exception e)
        {
            LocationLog.Error(TAG, "deleteActivityConversionUpdates onFailure:" + e.Message);
        }
    }
}