using UnityEngine.SceneManagement;

public class SceneControlManager
{
    private static string lastSceneName = "";

    public const string USER_LOGIN_SCENE_NAME = "UserLoginScene";
    public const string GAME_LOADING_SCENE_NAME = "GameLoadingScene";
    public const string MAIN_MENU_SCENE_NAME = "MainMenu";
    public const string PLAYERMON_MANAGEMENT_SCENE_NAME = "PlayermonSelectionPage";
    public const string TEAM_MANAGEMENT_SCENE_NAME = "TestTeamAPI";
    public const string SPACEDEN_GAME_SCENE_NAME = "SpaceDenGameScene";
    public const string MAP_SELECTION_SCENE_NAME = "MapSelectionScene";
    public const string LEVEL_SELECTION_SCENE_NAME = "LevelSelectionScene";
    public const string BATTLE_SCENE_NAME = "BattleScene";

    public static void GoToUserLoginScene()
    {
        ChangeScene( USER_LOGIN_SCENE_NAME );
    }

    public static void GoToGameLoadingScene()
    {
        ChangeScene( GAME_LOADING_SCENE_NAME );
    }

    public static void GoToMainMenuScene()
    {
        ChangeScene( MAIN_MENU_SCENE_NAME );
    }

	public static void GoToSpaceDenGameScene()
    {
        ChangeScene( SPACEDEN_GAME_SCENE_NAME );
	}

    public static void GoToMapSelectionScene()
    {
        ChangeScene( MAP_SELECTION_SCENE_NAME );
    }

    public static void GoToLevelSelectionScene()
    {
        ChangeScene( LEVEL_SELECTION_SCENE_NAME );
    }

    public static void GoToBattleGameScene()
    {
        ChangeScene( BATTLE_SCENE_NAME );
    }

    public static void GoToTeamManagementScene()
    {
        ChangeScene( TEAM_MANAGEMENT_SCENE_NAME );
    }

    public static void GoToPlayermonManagementScene()
    {
        ChangeScene( PLAYERMON_MANAGEMENT_SCENE_NAME );
    }

    public static void GoToGameScene()
    {
        ChangeScene( "GameScene" );
    }

    private static void ChangeScene( string sceneName )
    {
        lastSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene( sceneName );
    }

    public static string GetLastSceneName()
    {
        return lastSceneName;
    }
}
