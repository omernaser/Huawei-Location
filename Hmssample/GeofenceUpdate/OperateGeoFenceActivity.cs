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

namespace XLocationDemoProjectRef.Hmssample.GeofenceUpdate
{
    [Activity(Label = "OperateGeofenceActivity")]
    public class OperateGeofenceActivity : LocationBaseActivity, View.IOnClickListener
    {
        public string TAG = "operateGeoFenceActivity";
        private TextView geoFenceData, geoRequestData;
        private EditText removeWithPendingIntentInput, removeWithIdInput, trigger;
        public GeofenceService geofenceService;
        public static List<RequestList> requestList = new List<RequestList>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_operate_geo_fence);
            FindViewById(Resource.Id.getGeoFenceData).SetOnClickListener(this);
            FindViewById(Resource.Id.CreateGeofence).SetOnClickListener(this);
            FindViewById(Resource.Id.sendRequest).SetOnClickListener(this);
            FindViewById(Resource.Id.sendRequestWithNew).SetOnClickListener(this);
            FindViewById(Resource.Id.GetRequestMessage).SetOnClickListener(this);
            FindViewById(Resource.Id.removeGeofence).SetOnClickListener(this);
            FindViewById(Resource.Id.removeWithID).SetOnClickListener(this);
            FindViewById(Resource.Id.removeWithIntent).SetOnClickListener(this);
            removeWithPendingIntentInput = (EditText)FindViewById(Resource.Id.removeWithPendingIntentInput);
            removeWithIdInput = (EditText)FindViewById(Resource.Id.removeWithIDInput);
            trigger = (EditText)FindViewById(Resource.Id.trigger);
            geoFenceData = (TextView)FindViewById(Resource.Id.GeoFenceData);
            geoRequestData = (TextView)FindViewById(Resource.Id.GeoRequestData);
            geofenceService = new GeofenceService(this);
            AddLogFragment();
        }

        public void OnClick(View v)
        {
            try
            {
                switch (v.Id)
                {
                    case Resource.Id.CreateGeofence:
                        Intent geofenceIntent = new Intent();
                        geofenceIntent.SetClass(this, typeof(GeofenceActivity));
                        geofenceIntent.SetFlags(ActivityFlags.ClearTop);
                        StartActivity(geofenceIntent);
                        break;
                    case Resource.Id.removeGeofence:
                        GeoFenceData.CreateNewList();
                        break;
                    case Resource.Id.getGeoFenceData:
                        GetData();
                        break;
                    case Resource.Id.sendRequest:
                        RequestGeofenceWithIntent();
                        break;
                    case Resource.Id.sendRequestWithNew:
                        RequestGeofenceWithNewIntent();
                        break;
                    case Resource.Id.GetRequestMessage:
                        GetRequestMessage();
                        break;
                    case Resource.Id.removeWithIntent:
                        RemoveWithIntent();
                        break;
                    case Resource.Id.removeWithID:
                        RemoveWithId();
                        break;
                    default:
                        break;

                }
            }
            catch (Exception e)
            {
                LocationLog.Error(TAG, "operateGeoFenceActivity Exception:" + e);
            }
        }

        public void RequestGeofenceWithNewIntent()
        {
            if (GeoFenceData.ReturnList().Count == 0)
            {
                geoRequestData.SetText("no new request to add!", TextView.BufferType.Normal);
                return;
            }

            if (CheckUniqueId())
            {
                geoRequestData.SetText("ID already exist,please remove and add it again!", TextView.BufferType.Normal);
                return;
            }
            GeofenceRequest.Builder geofenceRequest = new GeofenceRequest.Builder();
            geofenceRequest.CreateGeofenceList(GeoFenceData.ReturnList());
            if (trigger.Text != null)
            {
                int Trigger = int.Parse(trigger.Text);
                geofenceRequest.SetInitConversions(Trigger);
                LocationLog.Debug(TAG, "trigger is " + Trigger);
            }
            else
            {
                geofenceRequest.SetInitConversions(5);
                LocationLog.Debug(TAG, "default trigger is 5");
            }

            PendingIntent pendingIntent = GetPendingIntent();
            SetList(pendingIntent, GeoFenceData.GetRequestCode(), GeoFenceData.ReturnList());
            try
            {
                geofenceService.CreateGeofenceList(geofenceRequest.Build(), pendingIntent)
                    .AddOnCompleteListener(new OnTaskCompleteListenerImpl("add geofence success！", "add geofence failed : "));
            }
            catch (Exception e)
            {
                LocationLog.Info(TAG, "add geofcence error : " + e.Message);
            }
            GeoFenceData.CreateNewList();
        }

        public PendingIntent FindIntentById(int a)
        {
            PendingIntent intent = null;
            for (int i = requestList.Count - 1; i >= 0; i--)
            {
                if (requestList.ElementAt(i).requestCode == a)
                {
                    intent = requestList.ElementAt(i).intnet;
                    requestList.RemoveAt(i);
                }
            }
            return intent;
        }

        public void RemoveWithIntent()
        {
            int s = int.Parse(removeWithPendingIntentInput.Text);
            PendingIntent intent = FindIntentById(s);
            if(intent == null)
            {
                geoRequestData.SetText("no such intent!", TextView.BufferType.Normal);
                return;
            }
            try
            {
                geofenceService.DeleteGeofenceList(intent)
                    .AddOnCompleteListener(new OnTaskCompleteListenerImpl("delete geofence with intent success！", "delete geofence with intent failed : "));
            }
            catch (Exception e)
            {
                LocationLog.Info(TAG, "delete geofence error：" + e.Message);
            }
        }

        public void RemoveWithId()
        {
            string s = removeWithIdInput.Text;
            string[] str = s.Split(" ");
            List<string> list = new List<string>();
            for (int i = 0; i < str.Length; i++)
                list.Add(str[i]);
            try
            {
                geofenceService.DeleteGeofenceList(list)
                    .AddOnCompleteListener(new OnTaskCompleteListenerImpl("delete geofence with ID success！", "delete geofence with ID failed : "));
            } catch (Exception e) {
                LocationLog.Info(TAG, "delete ID error：" + e.Message);
            }
            ListRemoveId(str);
        }

        public void ListRemoveId(string[] str)
        {
            for (int i = 0; i < requestList.Count; i++)
            {
                requestList.ElementAt(i).RemoveId(str);
            }
        }

        public void RequestGeofenceWithIntent()
        {
            if (GeoFenceData.ReturnList().Count == 0)
            {
                geoRequestData.SetText("no new request to add!", TextView.BufferType.Normal);
                return;
            }

            if (requestList.Count == 0)
            {
                geoRequestData.SetText("no pengdingIntent to send!", TextView.BufferType.Normal);
            }

            if (CheckUniqueId())
            {
                geoRequestData.SetText("ID already exist,please remove and add it again!", TextView.BufferType.Normal);
                return;
            }

            GeofenceRequest.Builder geofenceRequest = new GeofenceRequest.Builder();
            geofenceRequest.CreateGeofenceList(GeoFenceData.ReturnList());

            if(trigger.Text != null)
            {
                int Trigger = int.Parse(trigger.Text);
                geofenceRequest.SetInitConversions(Trigger);
                LocationLog.Debug(TAG, "trigger is" + Trigger);
            }
            else
            {
                geofenceRequest.SetInitConversions(5);
                LocationLog.Debug(TAG, "default trigger is 5");
            }
            RequestList temp = requestList.ElementAt(requestList.Count - 1);
            PendingIntent pendingIntent = temp.intnet;
            SetList(pendingIntent, temp.requestCode, GeoFenceData.ReturnList());
            geofenceService.CreateGeofenceList(geofenceRequest.Build(), pendingIntent);
            GeoFenceData.CreateNewList();
        }

        public bool CheckUniqueId()
        {
            for (int i = 0; i < requestList.Count; i++)
            {
                if (requestList.ElementAt(i).CheckId() == true)
                    return true;
            }
            return false;
        }

        public void GetData()
        {
            List<IGeofence> geofences = GeoFenceData.ReturnList();
            string s = "";
            if (geofences.Count == 0)
            {
                s = "no GeoFence Data!";
            }
            for (int i = 0; i < geofences.Count; i++)
            {
                s += "Unique ID is " + geofences.ElementAt(i).UniqueId + "\n";
            }
            geoFenceData.SetText(s, TextView.BufferType.Normal);
        }

        public void SetList(PendingIntent intent, int code, List<IGeofence> geofences)
        {
            RequestList temp = new RequestList(intent, code, geofences);
            requestList.Add(temp);
        }

        public void GetRequestMessage()
        {
            String s = "";
            for (int i = 0; i < requestList.Count; i++)
            {
                s += requestList.ElementAt(i).Show();
            }
            if (s.Equals(""))
                s = "no request!";
            geoRequestData.SetText(s, TextView.BufferType.Normal);
        }

        private PendingIntent GetPendingIntent()
        {
            Intent intent = new Intent(this, typeof(GeofenceBroadcastReceiver));
            intent.SetAction(GeofenceBroadcastReceiver.ACTION_PROCESS_LOCATION);
            Log.Debug(TAG, "new request");
            GeoFenceData.NewRequest();
            return PendingIntent.GetBroadcast(this, GeoFenceData.GetRequestCode(), intent, PendingIntentFlags.UpdateCurrent);
        }
    }

    public class RequestList
    {
        public PendingIntent intnet;
        public int requestCode;
        public List<IGeofence> geofences;

        public RequestList(PendingIntent intnet, int requestCode, List<IGeofence> geofences)
        {
            this.intnet = intnet;
            this.requestCode = requestCode;
            this.geofences = geofences;
        }

        public string Show()
        {
            string s = "";
            for (int i = 0; i < geofences.Count; i++)
            {
                s += "PendingIntent: " + requestCode.ToString() + " UniqueID: " + geofences.ElementAt(i).UniqueId + "\n";
            }
            return s;
        }

        public bool CheckId()
        {
            List<IGeofence> list = GeoFenceData.ReturnList();
            for (int j = 0; j < list.Count; j++)
            {
                String s = list.ElementAt(j).UniqueId;
                for (int i = 0; i < geofences.Count; i++)
                {
                    if (s.Equals(geofences.ElementAt(i).UniqueId))
                    {
                        return true;
                        //id already exist
                    }
                }
            }
            return false;
        }

        public void RemoveId(string[] str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                string s = str[i];
                for (int j = (geofences.Count - 1); j >= 0; j--)
                {
                    if (s.Equals(geofences.ElementAt(j).UniqueId))
                    {
                        geofences.Remove(geofences.ElementAt(j));
                    }
                }
            }
        }
    }

    class OnTaskCompleteListenerImpl : Java.Lang.Object, IOnCompleteListener
    {

        public static readonly string TAG = "OnTaskCompleteListenerImpl";

        private string infoMesg;
        private string warnMesg;

        public OnTaskCompleteListenerImpl(string info, string warn)
        {
            infoMesg = info;
            warnMesg = warn;
        }

        public void OnComplete(Task task)
        {
            if(task.IsSuccessful)
            {
                LocationLog.Info(TAG, infoMesg);
            }
            else
            {
                LocationLog.Warn(TAG, warnMesg + task.Exception.Message);
            }
        }
    }
}