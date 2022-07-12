using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class memeber_res
{
    public string membername;
    public string password;
    public string created_at;

    public memeber_res(string membername, string password, string created_at)
    {
        this.membername = membername;
        this.password = password;
        this.created_at = created_at;
    }
}

public class member_req
{
    public string membername;
    public string password;
    public member_req(string membername, string password)
    {
        this.membername = membername;
        this.password = password;
    }
}

public class App : MonoBehaviour
{
    public InputField ifMemberName;
    public InputField ifPassword;
    public Button btnSubmit;
    public Button btnGet;
    private string host = "http://127.0.0.1:8000/";

    // Start is called before the first frame update
    void Start()
    {
        this.btnSubmit.onClick.AddListener(() =>
        {
            Debug.LogFormat("{0} {1}",this.ifMemberName.text, this.ifPassword.text);
            var info = new member_req(this.ifMemberName.text, this.ifPassword.text);

            StartCoroutine(this.Post2("API/POST", info));
            // this.Post("/member", info);
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
                Debug.LogFormat("{0} {1}", member.membername, member.created_at);
            }
        }
    }

    private IEnumerator Post(string uri, member_req info)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        var form = new WWWForm();
        form.AddField("membername", info.membername);
        form.AddField("password", info.password);
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
        }
    }
}
