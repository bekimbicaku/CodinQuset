using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections;

public class Program : MonoBehaviour
{
    public CodeCompiler comparer; // Reference to the CodeComparer component
    public TMP_InputField userInputField; // Reference to the InputField in the UI

    int languageId = 71; // P.sh., p�r C++ (t� shohim nje qasje m�nyr� q� po ne C++)
    public void CheckUserCode()
    {
        //comparer.CompileUserCode(userInputField.text, languageId);
    }
}
