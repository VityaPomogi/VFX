using UnityEngine;
using UnityEngine.UI;

public class PopUpMessageBox : PopUpMessageBoxBasic
{
    [SerializeField] private Text messageLabel;

    public void Show( string resultMessage )
    {
        LeanTween.cancel( this.gameObject );
        messageLabel.text = resultMessage;
        base.Show();
    }
}
