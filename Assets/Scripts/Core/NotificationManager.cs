using UnityEngine;
using System.Collections;
using System;
using System.Xml.Linq;
using System.Runtime.ConstrainedExecution;




#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

public class NotificationManager : MonoBehaviour
{
#if UNITY_ANDROID
    [Header("Settings")]
    [SerializeField] private string studentName = "Mariana";
    [SerializeField] private int waitForMinutes = 1;

    private const string groupId = "Main";
    private const string ChannelId = "clicker_channel";
    private string NotiChannelsCreatedKey = "NotisChannels_Created";

    private readonly WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();

    private void Start()
    {
        if (!PlayerPrefs.HasKey(NotiChannelsCreatedKey))
        {
            RegisterNotificationChannels();
            StartCoroutine(RequestPermissionsAndSchedule());
        }
        else
        {
            ScheduleDefaultNotification();
        }
    }

    private void RegisterNotificationChannels()
    {
        var group = new AndroidNotificationChannelGroup()
        {
            Id = "Main",
            Name = "Main Notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannelGroup(group);
        var channel = new AndroidNotificationChannel()
        {
            Id = ChannelId,
            Name = "Clicker Notifications",
            Importance = Importance.Default,
            Description = "Notificaciones del juego CombateRPG"
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        PlayerPrefs.SetString(NotiChannelsCreatedKey, "done");
        PlayerPrefs.Save();
    }

    private IEnumerator RequestPermissionsAndSchedule()
    {
        var request = new PermissionRequest();
        while (request.Status == PermissionStatus.RequestPending)
        {
            yield return _waitForEndOfFrame;
        }

        if (request.Status == PermissionStatus.Allowed)
        {
            ScheduleDefaultNotification();
        }
        else
        {
            Debug.LogWarning("Permiso de notificaciones denegado por el usuario.");
        }
    }

    private void ScheduleDefaultNotification()
    {
        string title = "¡Te extrañamos!";
        string body = $"¡{studentName}! Tus héroes llevan demasiado tiempo durmiendo en la taberna. ¡A pelear!";

        ScheduleNotification(title, body, waitForMinutes);
    }

    public void ScheduleNotification(string title, string body, int delayInMinutes) 
    {
        AndroidNotificationCenter.CancelAllScheduledNotifications();

        var notification = new AndroidNotification()
        {
            Title = title,
            Text = body,
            FireTime = System.DateTime.Now.AddMinutes(delayInMinutes),
            SmallIcon = "default",
            LargeIcon = "default"
        };

        AndroidNotificationCenter.SendNotification(notification, ChannelId);
        Debug.Log($"Notificación programada para {waitForMinutes} minutos");
    }
#endif
}
