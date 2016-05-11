using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class SubmitField : AppMonoBehaviour {

    public InputField labelInput;
    public GameObject validateDialog;
    public Text validateText;
    public GameObject submitInput;

	public void OnClick()
    {
        if (IsInputValid())
        {
            submitInput.SetActive(false);
            manager.SendSubmitQuery(labelInput.text);
            HideValidateDialog();
        }
    }

    private bool IsInputValid()
    {
        string text = labelInput.text;
        if (text.Length == 0)
        {
            ShowInvalidLabelDialog("Please enter a label.");
            return false;
        }
        else if (text.All(char.IsLetterOrDigit))
        {
            return true;
        }
        ShowInvalidLabelDialog("Please use alphanumeric characters.");
        return false;
    }

    private void ShowInvalidLabelDialog(string msg)
    {
        validateDialog.SetActive(true);
        validateText.text = msg;
    }

    private void HideValidateDialog()
    {
        validateDialog.SetActive(false);
    }
}
