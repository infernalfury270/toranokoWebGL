using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "LocationData", menuName = "Explore/LocationData")]
public class LocationData : ScriptableObject
{
    [System.Serializable]
    public class LocationInfo
    {
        public Sprite background;
        public List<TravelPt> travelPts;
        public List<Interactable> interactables;
    }
    [System.Serializable]
    public class TravelPt
    {
        public Vector2 pos;
        public LocationData destination;
    }
    [System.Serializable]
    public class Interactable
    {
        public string name;
        public Vector2 pos;
        public UnityEvent OnInteract;
    }
    [System.Serializable]
    public class LocationVariant
    {
#if UNITY_EDITOR
        public string variantName;
#endif
        [Range(-1,6)]
        public int day;
        [Range(0, 2359)]
        public int timeStart;
        [Range(0, 2359)]
        public int timeEnd;
        public LocationInfo variant;
    }
    public string locationName;
    public LocationInfo baseInfo;
    public List<LocationVariant> locationVariants;
    public LocationInfo GetCurrentInfo(DateTime dateTime)
    {
        LocationInfo info = baseInfo;
        for (int i = 0; i < locationVariants.Count; i++)
        {
            var data = locationVariants[i];
            if (data.day == -1 || data.day == (int)dateTime.DayOfWeek)
            {
                var tod = dateTime.TimeOfDay;
                float hrStart = data.timeStart / 100.0f;
                float hrEnd = data.timeEnd / 100.0f;
                float curr = tod.Hours + (tod.Minutes / 100.0f);
                if (hrEnd < hrStart) // if it goes into the next day
                {
                    if (curr <= hrEnd || curr >= hrStart)
                    {
                        info = data.variant;
                    }
                } else
                {
                    if (curr >= hrStart && curr <= hrEnd)
                    {
                        info = data.variant;
                    }
                }
            }
        }
        return info;
    }
}
