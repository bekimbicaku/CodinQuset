using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using System.Text.RegularExpressions;
using System.Text;

[System.Serializable]
public class SubmissionResponse
{
    public string token;
}

[System.Serializable]
public class SubmissionResult
{
    public string stdout;
    public string stderr;
    public string compile_output;
    public string message;
    public SubmissionStatus status;
}

[System.Serializable]
public class SubmissionStatus
{
    public int id;
    public string description;
}

[System.Serializable]
public class RequestData
{
    public int language_id;
    public string source_code;
    public string stdin;
}

public class CodeCompiler : MonoBehaviour
{
    private string apiUrl = "https://judge0-ce.p.rapidapi.com/submissions";
    private string apiKey = "fb535bf1edmsh784afdb08bd5de2p12dfcajsnba4d7391b18e";
    private string apiHost = "judge0-ce.p.rapidapi.com";
    private string token;

    public int Lvl;
    public TMP_InputField userInputField; // Reference to the input field where the user enters the code

    public TextMeshProUGUI feedbackText;

    public string expectedOutput = "50"; // The correct output to compare with
    public lvl2 lvl2;
    public nrTek lvl3;
    public Lvl5 lvl5;


    public void SubmitCode()
    {
        string userCode = userInputField.text;

        // Basic validation with line ending handling
        if (!IsValidCppCode(userCode))
        {
            feedbackText.text = "Invalid C++ code. Please check syntax and try again.";
            return;
        }

        // Preprocess code to ensure consistent line endings (optional)
        userCode = PreprocessCode(userCode);

        StartCoroutine(SendRequest(userCode));

        
    }

    // Function to perform basic C++ syntax validation with line ending checks
    private bool IsValidCppCode(string code)
    {
        // Basic checks for C++ code validity
        if (!Regex.IsMatch(code, @"\b(int|float|double|char|bool|void|string)\b\s+\w+\s*\(.*\)\s*{"))
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
        var requestData = new RequestData
        {
            language_id = 52,
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
            UnityEngine.Debug.LogError("Error: " + request.error);
            yield break;
        }

        string responseData = request.downloadHandler.text;
        Debug.Log("Response: " + responseData); // Debugging: Log the response data

        // Extract the token from the response using Unity's JSON utility
        var responseJson = JsonUtility.FromJson<SubmissionResponse>(responseData);
        token = responseJson.token;

        // Now that we have the token, we can get the output
        yield return StartCoroutine(GetOutput(token));
    }

    IEnumerator GetOutput(string token)
    {
        string getUrl = apiUrl + "/" + token + "?base64_encoded=false&fields=*";

        UnityWebRequest getRequest = UnityWebRequest.Get(getUrl);
        getRequest.SetRequestHeader("X-RapidAPI-Key", apiKey);
        getRequest.SetRequestHeader("X-RapidAPI-Host", apiHost);

        yield return getRequest.SendWebRequest();

        if (getRequest.result != UnityWebRequest.Result.Success)
        {
            feedbackText.text = "Error: " + getRequest.error;
            UnityEngine.Debug.LogError("Error: " + getRequest.error);
            yield break;
        }

        string outputData = getRequest.downloadHandler.text;

        // Parse the response to check for completion status
        var responseJson = JsonUtility.FromJson<SubmissionResult>(outputData);

        // Keep checking until processing is finished
        while (responseJson.status.id == 2 || responseJson.status.id == 1)
        {
            yield return new WaitForSeconds(1); // Wait for a short while

            // Get the latest output data
            getRequest = UnityWebRequest.Get(getUrl);
            getRequest.SetRequestHeader("X-RapidAPI-Key", apiKey);
            getRequest.SetRequestHeader("X-RapidAPI-Host", apiHost);
            yield return getRequest.SendWebRequest();

            if (getRequest.result != UnityWebRequest.Result.Success)
            {
                feedbackText.text = "Error: " + getRequest.error;
                UnityEngine.Debug.LogError("Error: " + getRequest.error);
                yield break;
            }

            outputData = getRequest.downloadHandler.text;
            responseJson = JsonUtility.FromJson<SubmissionResult>(outputData);
        }

        // Process the output after successful completion
        if (responseJson.status.id != 1) // Not "In Queue"
        {
            if (!string.IsNullOrEmpty(responseJson.stdout))
            {
                string programOutput = responseJson.stdout.Trim();
                Debug.Log("Program Output: " + programOutput); // Debugging: Log the program output

                // Extract the numerical value from the output
                string extractedValue = ExtractValueFromOutput(programOutput);
                Debug.Log("Extracted Value: " + extractedValue); // Debugging: Log the extracted value
                
                if (extractedValue == expectedOutput)
                {
                    feedbackText.text = "Sakte! Outputi eshte : " + expectedOutput + " Monedha eshte liruar.";
                    // Additional logic for handling correct output
                    ExecuteLevelScript(Lvl);
                }
                else
                {
                    feedbackText.text = "Gabim! Outputi : "+ extractedValue +" . Provo perseri!";
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
        }
        else
        {
            feedbackText.text = "Unexpected status: " + responseJson.status.description;
        }
    }

    // Function to extract the numerical value from the program's output
    private string ExtractValueFromOutput(string output)
    {
        // Assuming the output format is "Vlera me e madhe eshte: X" where X is the value
        // We will split the string by space and get the last element
        string[] parts = output.Split(' ');
        if (parts.Length > 0)
        {
            return parts[parts.Length - 1];
        }
        else
        {
            return ""; // Return empty string if unable to extract value
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
            // Shto më shumë raste për nivelet e tjera
            default:
                Debug.LogWarning("No script defined for this level.");
                break;
        }
    }
    
}
