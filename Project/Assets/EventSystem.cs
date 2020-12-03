using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EventSystem : MonoBehaviour
{
    [System.Serializable]
    class Switcher
    {
        public GameObject first;
        public GameObject second;
        private bool trigger = true;
        public void Close()
        {
            first.SetActive(false);
            second.SetActive(false);
        }
        public void Set()
        {
            first.SetActive(trigger);
            second.SetActive(!trigger);
        }
        public void Switch()
        {
            trigger = !trigger;
            Set();
        }
    }

    [SerializeField] Switcher switcher;
    [Header("Auth")]
    [SerializeField] InputField login;
    [SerializeField] InputField password;
    [Header("Util")]
    [SerializeField] GameObject forClose;

    void Start()
    {
        switcher.Set();
    }

    
    
    void Update()
    {
        
    }
    public void Close(GameObject someObj)
    {
        someObj.SetActive(false);
    }
    public void SwitcherSwitch() => switcher.Switch();
    
    public void Authorizate()
    {
        //UnityWebRequest.Post
        StartCoroutine(Authorizate(SendModels.RemoteSettings.authorithateUrl, login.text,password.text));
            
    }
    IEnumerator Authorizate(string url,string _login,string _password)
    {
        var data = new SendModels.Authorithate(_login, _password);

        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(JsonUtility.ToJson(data));
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            var jwt = JsonUtility.FromJson<SendModels.JWT>(uwr.downloadHandler.text);
            GameManager.instance.token = jwt.jwt;
            Debug.Log("Received: " + uwr.downloadHandler.text);
            forClose.SetActive(false);
        }
    }
    public void Authentificate()
    {
        //UnityWebRequest.Post
        StartCoroutine(Authentificate(SendModels.RemoteSettings.authentificateUrl, login.text, password.text));

    }
    IEnumerator Authentificate(string url, string _login, string _password)
    {
        var data = new SendModels.Authentificate(_login, _password);

        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(JsonUtility.ToJson(data));
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            var playerInfo = JsonUtility.FromJson<SendModels.PlayerInfo>(uwr.downloadHandler.text);
            GameManager.instance.id = playerInfo.id;
            GameManager.instance.name = playerInfo.name;
            Debug.Log("Received: " + uwr.downloadHandler.text);
            StartCoroutine(Authorizate(SendModels.RemoteSettings.authorithateUrl, _login, _password));
        }
    }
}
