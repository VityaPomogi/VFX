using UnityEngine;

public class ExternalLinkManager : Singleton<ExternalLinkManager>
{
    private string websiteUrl = "";
    private string termOfUseUrl = "";
    private string tutorialUrl = "";
    private string marketplaceUrl = "";

    public void SetUp( string websiteUrl, string termOfUseUrl, string tutorialUrl, string marketplaceUrl )
    {
        this.websiteUrl = websiteUrl;
        this.termOfUseUrl = termOfUseUrl;
        this.tutorialUrl = tutorialUrl;
        this.marketplaceUrl = marketplaceUrl;
    }

    public void OpenWebsiteUrl()
    {
        Application.OpenURL( websiteUrl );
    }

    public void OpenTermOfUseUrl()
    {
        Application.OpenURL( termOfUseUrl );
    }

    public void OpenTutorialUrl()
    {
        Application.OpenURL( tutorialUrl );
    }

    public void OpenMarketplaceUrl()
    {
        Application.OpenURL( marketplaceUrl );
    }
}
