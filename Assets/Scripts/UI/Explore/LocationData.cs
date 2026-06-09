using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "LocationData", menuName = "Explore/LocationData")]
public class LocationData : ScriptableObject
{
    [System.Serializable]
    public class TravelPt
    {
        public Vector2 pos;
        public LocationData destination;
    }
    [System.Serializable]
    public class Interactable
    {
        public Vector2 pos;
        public UnityEvent OnInteract;
    }
    [System.Serializable]
    public class LocationVariant
    {
        [Range(0,6)]
        public int day;
        [Range(0, 2359)]
        public int timeStart;
        [Range(0, 2359)]
        public int timeEnd;
        public LocationData variant;
    }
    public string locationName;
    public Sprite background;
    public List<TravelPt> travelPts;
    public List<Interactable> interactables;
    public List<LocationVariant> locationVariants;
}
