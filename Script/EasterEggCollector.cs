using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class EasterEggCollector : MonoBehaviour
{
public GameObject[] easterEggs;
public GameObject finalObject;
public AudioClip collectSound;
public AudioClip rewardSound;
private int collectedEasterEggs = 0;
private bool rewardPlayed = false;
private string webhookUrl = "YOUR_DISCORD_WEBHOOK_URL"; // Add the webhook address you created here. You will not use it, leave it empty.
private void Update()
{
    if (Input.GetKeyDown(KeyCode.E)) 
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 2.0f))
        {
            GameObject hitObject = hit.collider.gameObject;

            for (int i = 0; i < easterEggs.Length; i++)
            {
                if (hitObject == easterEggs[i] && hit.collider is BoxCollider)
                {
                    collectedEasterEggs++;
                    AudioSource.PlayClipAtPoint(collectSound, transform.position);
                    Destroy(hitObject);
                    break;
                }
            }
        }
    }

    if (collectedEasterEggs >= 10 && !rewardPlayed) // You can ask them to find how many easter eggs by changing the number 10 here.
    {
        rewardPlayed = true;
        Instantiate(finalObject, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(rewardSound, transform.position);
        SendMessageToDiscord();
    }
}

private void SendMessageToDiscord()
{
    StartCoroutine(PostRequest(webhookUrl, "Congratulations, one more person finished PLEASE CONTACT US @everyone")); // Enter the message to be sent to discord here.
}

IEnumerator PostRequest(string url, string message)
{
    using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
    {
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes("{\"content\":\"" + message + "\"}");
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("Message sent to Discord!");
        }
    }
}
}