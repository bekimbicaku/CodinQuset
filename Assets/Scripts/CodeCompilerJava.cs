using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using System.Text.RegularExpressions;
using System.Text;

[System.Serializable]
public class SubmissionResponseJava
{
    public string token;
}

[System.Serializable]
public class SubmissionResultJava
{
    public string stdout;
    public string stderr;
    public string compile_output;
    public string message;
    public SubmissionStatusJava status;
}

[System.Serializable]
public class SubmissionStatusJava
{
    public int id;
    public string description;
}

[System.Serializable]
public class RequestDataJava
{
    public int language_id;
    public string source_code;
    public string stdin;
}

public class CodeCompilerJava : MonoBehaviour
{
    private string apiUrl = "https://judge0-ce.p.rapidapi.com/submissions";
    private string apiKey = "fb535bf1edmsh784afdb08bd5de2p12dfcajsnba4d7391b18e";
    private string apiHost = "judge0-ce.p.rapidapi.com";
    private string token;

    public int Lvl;
    public TMP_InputField userInputField; // Reference to the input field where the user enters the code

    public TextMeshProUGUI feedbackText;

    public string expectedOutput = "Numri 100 eshte i plotpjestueshem me 5 dhe 10."; // The correct output to compare with
    public lvl2 lvl2;
    public nrTek lvl3;
    public Lvl5 lvl5;
    public Lvl6 lvl6;

    public void SubmitCode()
    {
        string userCode = userInputField.text;

        // Basic validation with line ending handling
        if (!IsValidJavaCode(userCode))
        {
            feedbackText.text = "Invalid Java code. Please check syntax and try again.";
            return;
        }

        // Preprocess code to ensure consistent line endings (optional)
        userCode = PreprocessCode(userCode);

        StartCoroutine(SendRequest(userCode));
    }

    // Function to perform basic Java syntax validation with line ending checks
    private bool IsValidJavaCode(string code)
    {
        // Basic checks for Java code validity
        if (!Regex.IsMatch(code, @"\b(int|float|double|char|boolean|void|String)\b\s+\w+\s*\(.*\)\s*{"))
        {
            return false;
        }

        // Check for valid line endings (LF or CRLF)
        if (!Regex.IsMatch(code, @"(?:\n|\r\n)"))
        {
            return false; // No line endings found
        }

        return true; // Basic syntax and line endings seem valid
    }

    // Function to preprocess code for consistent line endings (optional)
    private string PreprocessCode(string code)
    {
        // Replace all line endings with a common format (e.g., LF)
        return code.Replace("\r\n", "\n"); // Replace CRLF with LF
    }

    IEnumerator SendRequest(string userCode)
    {
        // Prepare the request data with the user's code
        var requestData = new RequestDataJava
        {
            language_id = 62, // Java language ID
            source_code = userCode,
            stdin = ""
        };

        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log("Request Data: " + jsonData); // Debugging: Log the request data

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-RapidAPI-Key", apiKey);
        request.SetRequestHeader("X-RapidAPI-Host", apiHost);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            feedbackText.text = "Error: " + request.error;
            Debug.LogError("Error: " + request.error);
            yield break;
        }

        string responseData = request.downloadHandler.text;
        Debug.Log("Response: " + responseData); // Debugging: Log the response data

        // Extract the token from the response using Unity's JSON utility
        var responseJson = JsonUtility.FromJson<SubmissionResponseJava>(responseData);
        token = responseJson.token;

        // Now that we have the token, we can get the output
        yield return StartCoroutine(GetOutput(token));
    }

    IEnumerator GetOutput(string token)
    {
        string getUrl = $"{apiUrl}/{token}?base64_encoded=false&fields=*";

        while (true)
        {
            UnityWebRequest getRequest = UnityWebRequest.Get(getUrl);
            getRequest.SetRequestHeader("X-RapidAPI-Key", apiKey);
            getRequest.SetRequestHeader("X-RapidAPI-Host", apiHost);
            yield return getRequest.SendWebRequest();

            if (getRequest.result != UnityWebRequest.Result.Success)
            {
                feedbackText.text = "Error: " + getRequest.error;
                Debug.LogError("Error: " + getRequest.error);
                yield break;
            }

            string outputData = getRequest.downloadHandler.text;

            // Parse the response to check for completion status
            var responseJson = JsonUtility.FromJson<SubmissionResultJava>(outputData);

            // Check if the processing is finished
            if (responseJson.status.id != 2 && responseJson.status.id != 1) // Not "In Queue" and Not "Processing"
            {
                // Process the output after successful completion
                if (!string.IsNullOrEmpty(responseJson.stdout))
                {
                    string programOutput = responseJson.stdout.Trim();
                    Debug.Log("Program Output: " + programOutput); // Debugging: Log the program output

                    if (programOutput == expectedOutput)
                    {
                        feedbackText.text = "Sakte! Outputi eshte: " + expectedOutput + ". Monedha eshte liruar.";
                        // Additional logic for handling correct output
                        ExecuteLevelScript(Lvl);
                    }
                    else
                    {
                        feedbackText.text = "Gabim! Output: " + programOutput + ". Provo perseri!";
                    }
                }
                else if (!string.IsNullOrEmpty(responseJson.stderr))
                {
                    feedbackText.text = "Runtime Error: " + responseJson.stderr;
                }
                else if (!string.IsNullOrEmpty(responseJson.compile_output))
                {
                    feedbackText.text = "Compile Error: " + responseJson.compile_output;
                }
                else
                {
                    feedbackText.text = "Error: " + responseJson.message;
                }

                yield break; // Exit the loop
            }

            // Wait for a short while before checking again
            yield return new WaitForSeconds(1);
        }
    }

    void ExecuteLevelScript(int level)
    {
        switch (level)
        {
            case 1:
                //ExecuteLevel1Script();
                break;
            case 2:
                lvl2.startEvent();
                break;
            case 3:
                lvl3.startEvent();
                break;
            case 5:
                lvl5.startEvent();
                break;
            case 6:
                lvl6.startEvent();
                break;
            // Add more cases for other levels
            default:
                Debug.LogWarning("No script defined for this level.");
                break;
        }
    }
}
