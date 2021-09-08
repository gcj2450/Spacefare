using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationWatcher : MonoBehaviour
{
    public string notification;
    public string pinnedNote;

    // Start is called before the first frame update
    void Awake()
    {
        pinnedNote = "";
        notification = "";
        updateNotification();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void reset()
    {
        pinnedNote = "";
        notification = "";
        updateNotification();
    }

    public void sendNotification(string note)
    {
        notification = note;
        updateNotification();
        Invoke("clearNotification", 5);
    }
    public void pinNote(string note)
    {
        pinnedNote = note;
        updateNotification();
    }
    public void unpinNote()
    {
        pinnedNote = "";
    }

    private void updateNotification()
    {
        GameObject note = GameObject.FindWithTag("Notification");
        note.GetComponent<UnityEngine.UI.Text>().text = pinnedNote + notification;
    }

    private void clearNotification()
    {
        notification = "";
        updateNotification();
    }
}
