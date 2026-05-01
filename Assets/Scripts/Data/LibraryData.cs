using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LibraryData", menuName = "LibraryData")]
public class LibraryData : SingletonScriptableObject<LibraryData>
{
    [System.Serializable]
    public class Book
    {
        public enum THEME
        {
            DEFAULT,
            HARUKO,
            MITSUKO,
            TAIGA,
            HARUKO_MITSUKO,
            KATSUKO,
            TORANOKO
        }
        public string name;
        [TextArea(5,10)]
        public string description;
        public Sprite cover;
        public bool contentWarning;
        public THEME theme;

        [System.Serializable]
        public class Chapter
        {
            public string name;
            public string fileUrl;
            public string dateAdded;
        }
        public List<Chapter> chapters;
    }
    public List<Book> books;
}
