﻿/*
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
using Android.Util;
using Android.Views;

using Com.Huawei.Hms.Location;
using Com.Huawei.Hmf.Tasks;
using Com.Huawei.Hms.Common;

using XLocationDemoProjectRef.Logger;

/**
 * Example of Using requestLocationUpdates and removeLocationUpdates.
 * Requests a location update and calls back on the specified Looper thread. This method requires that the requester process exist for continuous callback.
 * If you still want to receive the callback after the process is killed, see requestLocationUpdates (LocationRequest request,PendingIntent callbackIntent)
 */
namespace XLocationDemoProjectRef.Hmssample.Location.Fusedlocation
{
    [Activity(Label = "RequestLocationUpdatesWithCallbackActivity")]
    public class RequestLocationUpdatesWithCallbackActivity : LocationBaseActivity, View.IOnClickListener
    {
        public static readonly string TAG = "LocationUpdatesCallback";
        LocationCallback mLocationCallback;
        LocationRequest mLocationRequest;
        private FusedLocationProviderClient mFusedLocationProviderClient;
        private SettingsClient mSettingsClient;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_location_request_location_updates_callback);
            // Button click listeners
            FindViewById(Resource.Id.location_requestLocationUpdatesWithCallback).SetOnClickListener(this);
            FindViewById(Resource.Id.location_removeLocationUpdatesWithCallback).SetOnClickListener(this);
            AddLogFragment();
            mFusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
            mSettingsClient = LocationServices.GetSettingsClient(this);
            mLocationRequest = new LocationRequest();
            // Sets the interval for location update (unit: Millisecond)
            mLocationRequest.SetInterval(5000);
            // Sets the priority
            mLocationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            if (null == mLocationCallback)
            {
                mLocationCallback = new LocationCallbackImpl();
            }
        }


        protected override void OnDestroy()
        {
            // Removed when the location update is no longer required.
            RemoveLocationUpdatesWithCallback();
            base.OnDestroy();
        }

        /**
        * Requests a location update and calls back on the specified Looper thread.
        */
        private void RequestLocationUpdatesWithCallback()
        {
            try
            {
                LocationSettingsRequest.Builder builder = new LocationSettingsRequest.Builder();
                builder.AddLocationRequest(mLocationRequest);
                LocationSettingsRequest locationSettingsRequest = builder.Build();
                // Before requesting location update, invoke checkLocationSettings to check device settings.
                Task locationSettingsResponseTask = mSettingsClient.CheckLocationSettings(locationSettingsRequest);
                locationSettingsResponseTask
                    .AddOnSuccessListener(new LocCallSettOnSuccessListenerImpl(mFusedLocationProviderClient, mLocationRequest, mLocationCallback))
                    .AddOnFailureListener(new LocCallSettOnFailureListenerImpl(this));                    
            }
            catch (Exception e)
            {
                LocationLog.Error(TAG, "requestLocationUpdatesWithCallback exception:" + e.Message);
            }
        }

        private void RemoveLocationUpdatesWithCallback()
        {
            try
            {
                Task voidTask = mFusedLocationProviderClient.RemoveLocationUpdates(mLocationCallback);
                voidTask
                    .AddOnSuccessListener(new RemoveLocCallUpdateOnSuccessListener())
                    .AddOnFailureListener(new RemoveLocCallUpdateOnFailureListener());
            }
            catch (Exception e)
            {
                LocationLog.Error(TAG, "removeLocatonUpdatesWithCallback exception:" + e.Message);
            }
        }

        public void OnClick(View v)
        {
            try
            {
                switch (v.Id)
                {
                    case Resource.Id.location_requestLocationUpdatesWithCallback:
                        RequestLocationUpdatesWithCallback();
                        break;
                    case Resource.Id.location_removeLocationUpdatesWithCallback:
                        RemoveLocationUpdatesWithCallback();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Info(TAG, "RequestLocationUpdatesWithCallbackActivity Exception:" + e);
            }
        }

    }

    class LocationCallbackImpl : LocationCallback
    {
        public static readonly string TAG = "LocationCallbackImpl";
        public override void OnLocationResult(LocationResult locationResult)
        {
            if (locationResult != null)
            {
                IList<Android.Locations.Location> locations = locationResult.Locations;
                if (locations.Count != 0)
                {
                    foreach (Android.Locations.Location location in locations)
                    {
                        LocationLog.Info(TAG,
                                "onLocationResult location[Longitude,Latitude,Accuracy]:" + location.Longitude
                                        + "," + location.Latitude + "," + location.Accuracy);
                    }
                }
            }
        }


        public override void OnLocationAvailability(LocationAvailability locationAvailability)
        {
            if (locationAvailability != null)
            {
                bool flag = locationAvailability.IsLocationAvailable;
                LocationLog.Info(TAG, "onLocationAvailability isLocationAvailable:" + flag);
            }
        }
    }

    class LocCallSettOnSuccessListenerImpl : Java.Lang.Object, IOnSuccessListener
    {
        public static readonly string TAG = "RequestLocationUpdatesWithCallbackActivity:SettingsSuccessListenerImpl";

        private FusedLocationProviderClient mFusedLocationProviderClient;
        private LocationRequest mLocationRequest;
        private LocationCallback mLocationCallback;

        public LocCallSettOnSuccessListenerImpl(FusedLocationProviderClient fusedLocationProviderClient, LocationRequest locationRequest, LocationCallback locationCallback)
        {
            mFusedLocationProviderClient = fusedLocationProviderClient;
            mLocationRequest = locationRequest;
            mLocationCallback = locationCallback;
        }

        public void OnSuccess(Java.Lang.Object locObj)
        {
            Log.Info(TAG, "check location settings success");
            mFusedLocationProviderClient
                .RequestLocationUpdates(mLocationRequest, mLocationCallback, Looper.MainLooper)
                .AddOnSuccessListener(new LocCallUpdateReqOnSuccessListenerImpl())
                .AddOnFailureListener(new LocCallUpdateReqOnFailureListenerImpl());
        }
    }

    class LocCallSettOnFailureListenerImpl : Java.Lang.Object, IOnFailureListener
    {
        public static readonly string TAG = "RequestLocationUpdatesWithCallbackActivity:SettingsFailureListenerImpl";
        private RequestLocationUpdatesWithCallbackActivity callbackActivity;

        public LocCallSettOnFailureListenerImpl(RequestLocationUpdatesWithCallbackActivity activity)
        {
            callbackActivity = activity;
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            LocationLog.Error(TAG, "checkLocationSetting onFailure:" + e.Message);
            int statusCode = ((ApiException)e).StatusCode;
            switch (statusCode)
            {
                case LocationSettingsStatusCodes.ResolutionRequired:
                    try
                    {
                        //When the startResolutionForResult is invoked, a dialog box is displayed, asking you to open the corresponding permission.
                        ResolvableApiException rae = (ResolvableApiException)e;
                        rae.StartResolutionForResult(callbackActivity, 0);
                    }
                    catch (IntentSender.SendIntentException sie)
                    {
                        LocationLog.Error(TAG, "PendingIntent unable to execute request.");
                    }
                    break;
            }
        }
    }

    class LocCallUpdateReqOnSuccessListenerImpl : Java.Lang.Object, IOnSuccessListener
    {
        public static readonly string TAG = "RequestLocationUpdatesWithCallback:SuccessListenerImpl";

        public void OnSuccess(Java.Lang.Object locObj)
        {
            LocationLog.Info(TAG, "requestLocationUpdatesWithCallback onSuccess");
        }
    }

    class LocCallUpdateReqOnFailureListenerImpl : Java.Lang.Object, IOnFailureListener
    {
        public static readonly string TAG = "RequestLocationUpdatesWithCallback:FailureListenerImpl";

        public void OnFailure(Java.Lang.Exception e)
        {
            LocationLog.Error(TAG, "requestLocationUpdatesWithCallback onFailure:" + e.Message);
        }
    }

    class RemoveLocCallUpdateOnSuccessListener : Java.Lang.Object, IOnSuccessListener
    {
        public static readonly string TAG = "RemoveLocationUpdatesWithCallback:SuccessListenerImpl";

        public void OnSuccess(Java.Lang.Object locObj)
        {
            LocationLog.Info(TAG, "removeLocatonUpdatesWithCallback onSuccess");
        }
    }

    class RemoveLocCallUpdateOnFailureListener : Java.Lang.Object, IOnFailureListener
    {
        public static readonly string TAG = "RemoveLocationUpdatesWithCallback:FailureListenerImpl";

        public void OnFailure(Java.Lang.Exception e)
        {
            LocationLog.Error(TAG, "removeLocatonUpdatesWithCallback exception:" + e.Message);
        }
    }
}