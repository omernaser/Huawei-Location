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
using System.Collections;
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

namespace XLocationDemoProjectRef.Hmssample.GeofenceUpdate
{
    [Activity(Label = "GeofenceActivity")]
    public class GeofenceActivity : LocationBaseActivity, View.IOnClickListener
    {
        public static readonly string TAG = "GeoFenceActivity";

        public EditText setLatitude, setLongitude, setRadius, setUniqueId, setConversions, setValidContinueTime, setDwellDelayTime, setNotificationInterval;
        LocationCallback mLocationCallback;
        LocationRequest mLocationRequest;
        private FusedLocationProviderClient mFusedLocationProviderClient;
        private SettingsClient mSettingsClient;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_geo_fence);
            setLatitude = (EditText) FindViewById(Resource.Id.setlatitude);
            setLongitude = (EditText) FindViewById(Resource.Id.setlongitude);
            setRadius = (EditText) FindViewById(Resource.Id.setradius);
            setUniqueId = (EditText) FindViewById(Resource.Id.setUniqueId);
            setConversions = (EditText) FindViewById(Resource.Id.setConversions);
            setValidContinueTime = (EditText) FindViewById(Resource.Id.setValidContinueTime);
            setDwellDelayTime = (EditText) FindViewById(Resource.Id.setDwellDelayTime);
            setNotificationInterval = (EditText) FindViewById(Resource.Id.setNotificationInterval);
            FindViewById(Resource.Id.getCurrentLocation).SetOnClickListener(this);
            FindViewById(Resource.Id.geofence_btn).SetOnClickListener(this);
            FindViewById(Resource.Id.showGeoList).SetOnClickListener(this);
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
                mLocationCallback = new LocationCallbackImpl(mFusedLocationProviderClient, mLocationCallback, setLatitude, setLongitude);
            }
        }

        public void GetText()
        {
            Data temp = new Data();
            temp.longitude = double.Parse(setLatitude.Text);
            temp.latitude = double.Parse(setLatitude.Text);
            temp.radius = float.Parse(setRadius.Text);
            temp.uniqueId = setUniqueId.Text;
            temp.conversions = int.Parse(setConversions.Text);
            temp.validContinueTime = long.Parse(setValidContinueTime.Text);
            temp.dwellDelayTime = int.Parse(setDwellDelayTime.Text);
            temp.notificationInterval = int.Parse(setNotificationInterval.Text);
            GeoFenceData.AddGeofence(temp);
        }

        public void GetLocation()
        {
            RequestLocationUpdatesWithCallback();
        }

        public void OnClick(View v)
        {
            try
            {
                switch(v.Id)
                {
                    case Resource.Id.geofence_btn:
                        GetText();
                        break;
                    case Resource.Id.showGeoList:
                        GeoFenceData.Show();
                        break;
                    case Resource.Id.getCurrentLocation:
                        GetLocation();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                LocationLog.Error(TAG, "GeoFenceActivity Exception:" + e);
            }
        }

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
                    .AddOnSuccessListener(new GeoCallSettOnSuccessListenerImpl(mFusedLocationProviderClient, mLocationRequest, mLocationCallback))
                    .AddOnFailureListener(new GeoCallSettOnFailureListenerImpl());
            }
            catch (Exception e)
            {
                LocationLog.Error(TAG, "requestLocationUpdatesWithCallback exception:" + e.Message);
            }
        }
    }

    class LocationCallbackImpl : LocationCallback
    {
        public static readonly string TAG = "GeoFenceActivity:LocationCallback";

        public EditText setLatitude, setLongitude, setRadius, setUniqueId, setConversions, setValidContinueTime, setDwellDelayTime, setNotificationInterval;
        private FusedLocationProviderClient mFusedLocationProviderClient;
        private LocationCallback mLocationCallback;

        public LocationCallbackImpl (FusedLocationProviderClient providerClient, LocationCallback locationCallback, EditText latitude, EditText longitude)
        {
            mFusedLocationProviderClient = providerClient;
            mLocationCallback = locationCallback;
            setLatitude = latitude;
            setLongitude = longitude;
        }

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
                        setLatitude.SetText(location.Latitude.ToString(), TextView.BufferType.Normal);
                        setLongitude.SetText(location.Longitude.ToString(), TextView.BufferType.Normal);
                        RemoveLocationUpdatesWithCallback();
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

        private void RemoveLocationUpdatesWithCallback()
        {
            try
            {
                Task voidTask = mFusedLocationProviderClient.RemoveLocationUpdates(mLocationCallback);
                voidTask
                    .AddOnSuccessListener(new RemoveGeoCallUpdateOnSuccessListenerImpl())
                    .AddOnFailureListener(new RemoveGeoCallUpdateOnFailureListenerImpl());
            } 
            catch (Exception e) 
            {
                LocationLog.Error(TAG, "removeLocationUpdatesWithCallback exception:" + e.Message);
            }
        }


    }

    class GeoCallSettOnSuccessListenerImpl : Java.Lang.Object, IOnSuccessListener
    {
        public static readonly string TAG = "GeoFenceActivityLocationSettingsRequest:SuccessListenerImpl";

        private FusedLocationProviderClient mFusedLocationProviderClient;
        private LocationRequest mLocationRequest;
        private LocationCallback mLocationCallback;

        public GeoCallSettOnSuccessListenerImpl(FusedLocationProviderClient fusedLocationProviderClient, LocationRequest locationRequest, LocationCallback locationCallback)
        {
            mFusedLocationProviderClient = fusedLocationProviderClient;
            mLocationRequest = locationRequest;
            mLocationCallback = locationCallback;
        }

        public void OnSuccess(Java.Lang.Object geoObj)
        {
            Log.Info(TAG, "check location settings success");
            mFusedLocationProviderClient
                .RequestLocationUpdates(mLocationRequest, mLocationCallback, Looper.MainLooper)
                .AddOnSuccessListener(new GeoCallUpdateReqOnSuccessListenerImpl())
                .AddOnFailureListener(new GeoCallUpdateReqOnFailureListenerImpl());
        }
    }

    class GeoCallSettOnFailureListenerImpl : Java.Lang.Object, IOnFailureListener
    {
        public static readonly string TAG = "GeoFenceActivityLocationSettingsRequest:FailureListenerImpl";

        public void OnFailure(Java.Lang.Exception e)
        {
            LocationLog.Error(TAG, "checkLocationSetting onFailure:" + e.Message);
        }
    }

    class GeoCallUpdateReqOnSuccessListenerImpl : Java.Lang.Object, IOnSuccessListener
    {
        public static readonly string TAG = "GeoFenceActivityLocationUpdateRequestWithCallback:SuccessListenerImpl";

        public void OnSuccess(Java.Lang.Object geoObj)
        {
            LocationLog.Info(TAG, "requestLocationUpdatesWithCallback onSuccess");
        }
    }

    class GeoCallUpdateReqOnFailureListenerImpl : Java.Lang.Object, IOnFailureListener
    {
        public static readonly string TAG = "GeoFenceActivityLocationUpdateRequestWithCallback:FailureListenerImpl";

        public void OnFailure(Java.Lang.Exception e)
        {
            LocationLog.Error(TAG,"requestLocationUpdatesWithCallback onFailure:" + e.Message);
        }
    }

    class RemoveGeoCallUpdateOnSuccessListenerImpl : Java.Lang.Object, IOnSuccessListener
    {
        public static readonly string TAG = "GeoFenceActivityRemoveLocationUpdateWithCallback:SuccessListenerImpl";

        public void OnSuccess(Java.Lang.Object geoObj)
        {
            LocationLog.Info(TAG, "removeLocationUpdatesWithCallback onSuccess");
        }
    }

    class RemoveGeoCallUpdateOnFailureListenerImpl : Java.Lang.Object, IOnFailureListener
    {
        public static readonly string TAG = "GeoFenceActivityRemoveLocationUpdateWithCallback:FailureListenerImpl";

        public void OnFailure(Java.Lang.Exception e)
        {
            LocationLog.Error(TAG, "removeLocationUpdatesWithCallback onFailure:" + e.Message);
        }
    }

    class Data
    {
        public double latitude;
        public double longitude;
        public float radius;
        public string uniqueId;
        public int conversions;
        public long validContinueTime;
        public int dwellDelayTime;
        public int notificationInterval;
    }

    class GeoFenceData
    {
        private static int requestCode = 0;
        static List<IGeofence> geofences = new List<IGeofence>();
        static GeofenceBuilder geoBuild = new GeofenceBuilder();
        public static void AddGeofence(Data data)
        {
            if (checkStyle(geofences, data.uniqueId) == false)
            {
                LocationLog.Debug("GeoFenceActivity", "not unique ID!");
                LocationLog.Info("GeoFenceActivity", "addGeofence failed!");
                return;
            }
            geoBuild.SetRoundArea(data.latitude, data.longitude, data.radius);
            geoBuild.SetUniqueId(data.uniqueId);
            geoBuild.SetConversions(data.conversions);
            geoBuild.SetValidContinueTime(data.validContinueTime);
            geoBuild.SetDwellDelayTime(data.dwellDelayTime);
            geoBuild.SetNotificationInterval(data.notificationInterval);
            geofences.Add(geoBuild.Build());
            LocationLog.Info("GeoFenceActivity", "addGeofence success!");
        }
        public static void CreateNewList()
        {
            geofences = new List<IGeofence>();
        }
        public static bool checkStyle(List<IGeofence> geofences, String ID)
        {
            for (int i = 0; i < geofences.Count; i++)
            {
                if (geofences.ElementAt(i).UniqueId.Equals(ID))
                    return false;
            }
            return true;
        }
        public static List<IGeofence> ReturnList()
        {
            return geofences;
        }

        public static void Show()
        {
            if (geofences.Count == 0)
            {
                LocationLog.Debug("GeoFenceActivity", "no GeoFence Data!");
            }
            for (int i = 0; i < geofences.Count; i++)
            {
                LocationLog.Debug("GeoFenceActivity", "Unique ID is " + (geofences.ElementAt(i).UniqueId));
            }
        }
        public static void NewRequest()
        {
            requestCode++;
        }

        public static int GetRequestCode()
        {
            return requestCode;
        }
    }
}