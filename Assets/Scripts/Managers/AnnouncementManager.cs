using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ServerApiResponse;

public class AnnouncementManager : Singleton<AnnouncementManager>
{
    private List<string> announcementUrlList = null;
    private List<Texture2D> announcementTextureList = null;

    public void SetUpAnnouncement( GetUserProfileResponse_Data responseData )
    {
        announcementUrlList = new List<string>();

        string[] _announcements = responseData.announcements;
        for (int i = 0; i < _announcements.Length; i++)
        {
            announcementUrlList.Add( _announcements[ i ] );
        }

        StartCoroutine( LoadAnnouncementImages() );
    }

    private IEnumerator LoadAnnouncementImages()
    {
        announcementTextureList = new List<Texture2D>();

        for (int i = 0; i < announcementUrlList.Count; i++)
        {
            string _announcementUrl = announcementUrlList[ i ];
            UnityWebRequest _request = UnityWebRequestTexture.GetTexture( _announcementUrl );
            yield return _request.SendWebRequest();
            if (_request.result == UnityWebRequest.Result.Success)
            {
                announcementTextureList.Add( ( ( DownloadHandlerTexture )_request.downloadHandler ).texture );
            }
            else
            {
                Debug.Log( _request.error );
            }
        }
    }

    public Texture2D GetAnnouncementImage( int announcementIndex )
    {
        if (announcementTextureList.Count > announcementIndex)
        {
            return announcementTextureList[ announcementIndex ];
        }

        return null;
    }

    public int GetAnnouncementCount()
    {
        return announcementUrlList.Count;
    }
}
