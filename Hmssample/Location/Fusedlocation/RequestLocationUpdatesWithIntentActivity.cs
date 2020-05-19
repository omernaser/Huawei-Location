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
using Com.Huawei.Hms.Common;

using XLocationDemoProjectRef.Logger;

/**
 * Example of Using requestLocationUpdates and removeLocationUpdates
 * Use the PendingIntent mode to continuously request the location update.
 * The location update result LocationResult and LocationAvailability are encapsulated in the Intent, and the send() of the transferred PendingIntent is invoked to send the result to the requester.
 * If the requester process is killed, use this method to continue to call back.
 * If the requester does not want to receive the location update result when the process is killed, see requestLocationUpdates (LocationRequest request,LocationCallback callback,Looper looper).
 */
namespace XLocationDemoProjectRef.Hmssample.Location.Fusedlocation
{
    [Activity(Label = "RequestLocationUpdatesWithIntentActivity")]
    public class RequestLocationUpdatesWithIntentActivity : LocationBaseActivity, View.IOnClickListener
    {
        public static readonly string TAG = "LocationUpdatesIntent";
        LocationRequest mLocationRequest;
        private FusedLocationProviderClient mFusedLocationProviderClient;
        private SettingsClient mSettingsClient;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_location_request_location_updates_intent);
            // Button click listeners
            FindViewById(Resource.Id.location_requestLocationUpdatesWithIntent).SetOnClickListener(this);
            FindViewById(Resource.Id.location_removeLocationUpdatesWithIntent).SetOnClickListener(this);
            AddLogFragment();
            mFusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
            mSettingsClient = LocationServices.GetSettingsClient(this);
            mLocationRequest = new LocationRequest();
            //Sets the interval for location update (unit: Millisecond)
            mLocationRequest.SetInterval(10000);
            //Sets the priority
            mLocationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
        }

        private void RequestLocationUpdatesWithIntent()
        {
            try {
                LocationSettingsRequest.Builder builder = new LocationSettingsRequest.Builder();
                builder.AddLocationRequest(mLocationRequest);
                LocationSettingsRequest locationSettingsRequest = builder.Build();
                //Before requesting location update, invoke checkLocationSettings to check device settings.
                Task locationSettingsResponseTask = mSettingsClient.CheckLocationSettings(locationSettingsRequest)
                    .AddOnSuccessListener(new LocIntentSettOnSuccessListenerImpl(mFusedLocationProviderClient, mLocationRequest, GetPendingIntent()))
                    .AddOnFailureListener(new LocIntentSettOnFailureListenerImpl(this));
            }
            catch(System.Exception e) {
                LocationLog.Error(TAG, "requestLocationUpdatesWithIntent exception:" + e.Message);
            }
        }

        /**
        * Remove Location Update
        */
        private void RemoveLocationUpdatesWithIntent()
        {
            try
            {
                Task voidTask = mFusedLocationProviderClient.RemoveLocationUpdates(GetPendingIntent());
                voidTask
                    .AddOnSuccessListener(new RemoveLocIntentUpdateOnSuccessListener())
                    .AddOnFailureListener(new RemoveLocIntentUpdateOnFailureListener());
            }
            catch (Exception e)
            {
                LocationLog.Error(TAG, "removeLocatonUpdatesWithIntent exception:" + e.Message);
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
            // Removed when the location update is no longer required.
            RemoveLocationUpdatesWithIntent();
            base.OnDestroy();
        }

        public void OnClick(View v)
        {
            try
            {
                switch (v.Id)
                {
                    case Resource.Id.location_requestLocationUpdatesWithIntent:
                        RequestLocationUpdatesWithIntent();
                        break;
                    case Resource.Id.location_removeLocationUpdatesWithIntent:
                        RemoveLocationUpdatesWithIntent();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Info(TAG, "RequestLocationUpdatesWithIntentActivity Exception:" + e);
            }
        }
    }

    class LocIntentSettOnSuccessListenerImpl : Java.Lang.Object, IOnSuccessListener
    {
        public static readonly string TAG = "RequestLocationUpdatesWithIntentActivity:SettingsSuccessListenerImpl";
        private FusedLocationProviderClient mFusedLocationProviderClient;
        private LocationRequest mLocationRequest;
        private PendingIntent pendingIntent;

        public LocIntentSettOnSuccessListenerImpl(FusedLocationProviderClient fusedLocationProviderClient, LocationRequest locationRequest, PendingIntent intent)
        {
            mFusedLocationProviderClient = fusedLocationProviderClient;
            mLocationRequest = locationRequest;
            pendingIntent = intent;
        }

        public void OnSuccess(Java.Lang.Object locObj)
        {
            LocationLog.Info(TAG, "check location settings success");
            mFusedLocationProviderClient.RequestLocationUpdates(mLocationRequest, pendingIntent)
                .AddOnSuccessListener(new LocIntentUpdateReqOnSuccessListenerImpl())
                .AddOnFailureListener(new LocIntentUpdateReqOnFailureListenerImpl());
        }
    }

    class LocIntentSettOnFailureListenerImpl : Java.Lang.Object, IOnFailureListener
    {
        public static readonly string TAG = "RequestLocationUpdatesWithIntentActivity:SettingsFailureListenerImpl";
        private RequestLocationUpdatesWithIntentActivity intentActivity;

        public LocIntentSettOnFailureListenerImpl(RequestLocationUpdatesWithIntentActivity activity)
        {
            intentActivity = activity;
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            LocationLog.Error(TAG, "checkLocationSetting onFailure:" + e.Message);
            int statusCode = ((ApiException) e).StatusCode;
            switch (statusCode)
            {
                case LocationSettingsStatusCodes.ResolutionRequired:
                    try {
                        //When the startResolutionForResult is invoked, a dialog box is displayed, asking you to open the corresponding permission.
                        ResolvableApiException rae = (ResolvableApiException) e;
                        rae.StartResolutionForResult(intentActivity, 0);
                    }
                    catch (IntentSender.SendIntentException sie) {
                        LocationLog.Error(TAG, "PendingIntent unable to execute request.");
                    }
                    break;
            }
        }
    }

    class LocIntentUpdateReqOnSuccessListenerImpl : Java.Lang.Object, IOnSuccessListener
    {
        public static readonly string TAG = "RequestLocationUpdateswithIntent:SuccessListenerImpl";

        public void OnSuccess(Java.Lang.Object locObj)
        {
            LocationLog.Info(TAG, "requestLocationUpdatesWithIntent onSuccess");
        }
    }

    class LocIntentUpdateReqOnFailureListenerImpl : Java.Lang.Object, IOnFailureListener
    {
        public static readonly string TAG = "RequestLocationUpdateswithIntent:FailureListenerImpl";

        public void OnFailure(Java.Lang.Exception e)
        {
            LocationLog.Error(TAG, "requestLocationUpdatesWithIntent onFailure:" + e.Message);
        }
    }

    class RemoveLocIntentUpdateOnSuccessListener : Java.Lang.Object, IOnSuccessListener
    {
        public static readonly string TAG = "RemoveLocationUpdatesWithIntent:SuccessListenerImpl";

        public void OnSuccess(Java.Lang.Object locObj)
        {
            LocationLog.Info(TAG, "removeLocatonUpdatesWithIntent onSuccess");
        }
    }

    class RemoveLocIntentUpdateOnFailureListener : Java.Lang.Object, IOnFailureListener
    {
        public static readonly string TAG = "RemoveLocationUpdatesWithIntent:FailureListenerImpl";

        public void OnFailure(Java.Lang.Exception e)
        {
            LocationLog.Error(TAG, "removeLocatonUpdatesWithIntent exception:" + e.Message);
        }
    }
}