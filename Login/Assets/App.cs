using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class memeber_res
{
    public string user_name;
    public string user_id;
    public string user_pw;

    public memeber_res(string user_name, string user_id, string user_pw)
    {
        this.user_name = user_name;
        this.user_id = user_id;
        this.user_pw = user_pw;
    }
}

public class member_req
{
    public string user_id;
    public string user_pw;
    public member_req(string user_id, string user_pw)
    {
        this.user_id = user_id;
        this.user_pw = user_pw;
    }
}

public class App : MonoBehaviour
{
    public InputField ifMemberName;
    public InputField ifPassword;
    public Button btnSubmit;
    public Button btnGet;
    // private string host = "http://127.0.0.1:8000/";
    private string host = "https://obliy.azurewebsites.net/";

    // Start is called before the first frame update
    void Start()
    {
        this.btnSubmit.onClick.AddListener(() =>
        {
            Debug.LogFormat("{0} {1}",this.ifMemberName.text, this.ifPassword.text);
            var info = new member_req(this.ifMemberName.text, this.ifPassword.text);

            // StartCoroutine(this.Post("api/account/login_result", info));
            StartCoroutine(this.Post2("api/account/login_result", info));
            
        });

        this.btnGet.onClick.AddListener(() =>
        {
            StartCoroutine(this.Get(""));
        });
    }

    private IEnumerator Get(string uri)
    {
        var url = string.Format("{0}{1}", this.host, uri);
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log("www error");
        }
        else
        {
            var results = www.downloadHandler.data;
            var json = Encoding.UTF8.GetString(results);
            Debug.Log(json);
            var arrMembers = JsonConvert.DeserializeObject<memeber_res[]>(json);
            foreach (var member in arrMembers)
            {
                Debug.LogFormat("{0} {1}", member.user_id, member.user_pw);
            }
        }
    }

    private IEnumerator Post(string uri, member_req info)
    {
        var form = new WWWForm();
        form.AddField("user_id", info.user_id);
        form.AddField("user_pw", info.user_pw);
        var url = string.Format("{0}{1}", this.host, uri);
        var www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log("www error");
        }
        else
        {
            Debug.LogFormat("www.responseCode : {0}", www.responseCode);
            Debug.LogFormat("data.responseCode : {0}", www.downloadHandler.text);
        }
    }

    private IEnumerator Post2(string uri, member_req info)
    {
        var json = JsonConvert.SerializeObject(info);
        var url = string.Format("{0}{1}", this.host, uri);
        var request = new UnityWebRequest(url, "POST");
        var bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if(request.isNetworkError || request.isHttpError)
        {
            Debug.Log("www error");
        }
        else
        {
            Debug.LogFormat("request.responseCode: {0}", request.responseCode);
            Debug.LogFormat("data.responseCode : {0}", request.downloadHandler.text);
        }
    }
}
