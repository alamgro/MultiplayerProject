using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using System;

/*
 * Estructura de Playfab: 
 * 1) Request.
 * 2) Función en caso de request exitoso.
 * 3) Función en caso de request fallido.
 */

public class PlayfabManager : MonoBehaviour
{
    #region SINGLETON
    private static PlayfabManager _instance;
    public static PlayfabManager Instance => _instance;
    #endregion

    [SerializeField] private string dummyUser;
    [SerializeField] private string friendID;
    [SerializeField] private int hatID;

    //Datos importantes de un login
    private string id;
    private string entityID;
    private string entityType;

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Login();
    }

    [ContextMenu("Login")]
    private void Login()
    {
        //Todos los request terminan con la palabra "Request"
        var request = new LoginWithCustomIDRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            CustomId = dummyUser, //SystemInfo.deviceUniqueIdentifier
            CreateAccount = true //Si no existe, lo crea
        };

        void LoginWithCustomIDSuccess(LoginResult result) // Cuando es exitoso siempre regresa un XXXXResult
        {
            print($"Inicio de sesión exitoso, ID: {result.PlayFabId}");
            id = result.PlayFabId; //Se automaja, pero así lo tenemos guardado
            entityID = result.EntityToken.Entity.Id;
            entityType = result.EntityToken.Entity.Type;

            SceneManager.LoadScene(GameConstants.Scene.mainMenu);
            //GetCoins();
            //AddFriend();
            //GetFriendList();
        }

        void LoginWithCustomIDError(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
        }

        PlayFabClientAPI.LoginWithCustomID(request, LoginWithCustomIDSuccess, LoginWithCustomIDError);

        /* 
        //Otra forma de hacer las funciones de success y error
        PlayFabClientAPI.LoginWithCustomID(request,
            (result) =>
            {
                print($"Inicio de sesión exitoso, ID: {result.PlayFabId}");
                id = result.PlayFabId; //Se automaja, pero así lo tenemos guardado
                entityID = result.EntityToken.Entity.Id;
                entityType = result.EntityToken.Entity.Type;
            },
            (error) =>
            {
                Debug.LogError(error.GenerateErrorReport());
            }
        );
        */
    }

    private void GiveCoinsStarterPack()
    {
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                { PlayfabConsts.UserData.Monedas , "10" }
            }
        };

        void UpdateUserDataSucess(UpdateUserDataResult result)
        {
            print("Coins successfully added");
        }

        void UpdateUserDataError(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
        }

        PlayFabClientAPI.UpdateUserData(request, UpdateUserDataSucess, UpdateUserDataError);
    }
    
    //Adds 1 death to the "Deaths" Data
    public void UpdateDeaths()
    {
        GetDeathsAndUpdate();
    }

    private void GetDeathsAndUpdate()
    {
        var request = new GetUserDataRequest()
        {

        };

        void GetUserDataSuccess(GetUserDataResult result)
        {
            GameManager.Instance.TotalDeaths = 0;
            if (result.Data.ContainsKey(PlayfabConsts.UserData.Deaths))
            {
                print($"El jugador tiene: {result.Data[PlayfabConsts.UserData.Deaths].Value} deaths");
                GameManager.Instance.TotalDeaths = int.Parse(result.Data[PlayfabConsts.UserData.Deaths].Value);
            }

            AddDeath();
        }

        void GetUserDataError(PlayFabError error)
        {
            print(error.GenerateErrorReport());
        }

        PlayFabClientAPI.GetUserData(request, GetUserDataSuccess, GetUserDataError);
    }

    public void GetDeaths()
    {
        var request = new GetUserDataRequest()
        {

        };

        void GetUserDataSuccess(GetUserDataResult result)
        {
            GameManager.Instance.TotalDeaths = 0;
            if (result.Data.ContainsKey(PlayfabConsts.UserData.Deaths))
            {
                print($"El jugador tiene: {result.Data[PlayfabConsts.UserData.Deaths].Value} deaths");
                GameManager.Instance.TotalDeaths = int.Parse(result.Data[PlayfabConsts.UserData.Deaths].Value);
            }
        }

        void GetUserDataError(PlayFabError error)
        {
            print(error.GenerateErrorReport());
        }

        PlayFabClientAPI.GetUserData(request, GetUserDataSuccess, GetUserDataError);
    }

    private void AddDeath()
    {
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                { PlayfabConsts.UserData.Deaths , Convert.ToString(GameManager.Instance.TotalDeaths + 1) }
            }
        };

        void UpdateUserDataSucess(UpdateUserDataResult result)
        {
            print($"1 death were successfully added.");
        }

        void UpdateUserDataError(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
        }

        PlayFabClientAPI.UpdateUserData(request, UpdateUserDataSucess, UpdateUserDataError);
    }

    public void UpdateKills(int _killsToAdd)
    {
        GetKillsAndUpdate(_killsToAdd);
    }

    private void GetKillsAndUpdate(int _killsToAdd)
    {
        var request = new GetUserDataRequest()
        {
            
        };

        void GetUserDataSuccess(GetUserDataResult result)
        {
            if (result.Data.ContainsKey(PlayfabConsts.UserData.Kills))
            {
                print($"El jugador tiene: {result.Data[PlayfabConsts.UserData.Kills].Value} kills");
                GameManager.Instance.TotalKills = int.Parse( result.Data[PlayfabConsts.UserData.Kills].Value); 
                AddKills(_killsToAdd);
            }
            /*else
            {
                GiveCoinsStarterPack();
            }*/
        }

        void GetUserDataError(PlayFabError error)
        {
            print(error.GenerateErrorReport());
        }

        PlayFabClientAPI.GetUserData(request, GetUserDataSuccess, GetUserDataError);
    }

    public void GetKills()
    {
        var request = new GetUserDataRequest()
        {

        };

        void GetUserDataSuccess(GetUserDataResult result)
        {
            if (result.Data.ContainsKey(PlayfabConsts.UserData.Kills))
            {
                print($"El jugador tiene: {result.Data[PlayfabConsts.UserData.Kills].Value} kills");
                GameManager.Instance.TotalKills = int.Parse(result.Data[PlayfabConsts.UserData.Kills].Value);
            }
        }

        void GetUserDataError(PlayFabError error)
        {
            print(error.GenerateErrorReport());
        }

        PlayFabClientAPI.GetUserData(request, GetUserDataSuccess, GetUserDataError);
    }

    private void AddKills(int _killsToAdd)
    {
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                { PlayfabConsts.UserData.Kills , Convert.ToString(GameManager.Instance.TotalKills + GameManager.Instance.LevelKills) }
            }
        };

        void UpdateUserDataSucess(UpdateUserDataResult result)
        {
            print($"{_killsToAdd} kills were successfully added.");
        }

        void UpdateUserDataError(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
        }

        PlayFabClientAPI.UpdateUserData(request, UpdateUserDataSucess, UpdateUserDataError);
    }


    [ContextMenu("Get friends list")]
    private void GetFriendCoins(string _friendID)
    {
        var request = new GetUserDataRequest()
        {
            PlayFabId = _friendID
        };

        void GetUserDataSuccess(GetUserDataResult result)
        {
            if (result.Data.ContainsKey(PlayfabConsts.UserData.Monedas))
            {
                print($"Your friend has: {result.Data[PlayfabConsts.UserData.Monedas].Value} coins.");
            }
            else
            {
                print("Your friend's poor.");
            }
        }

        void GetUserDataError(PlayFabError error)
        {
            print(error.GenerateErrorReport());
        }

        PlayFabClientAPI.GetUserData(request, GetUserDataSuccess, GetUserDataError);
    }

    private void UpdateHat()
    {
        var request = new UpdateCharacterDataRequest()
        {
            CharacterId = "1",
            Data = new Dictionary<string, string>()
            {
                { PlayfabConsts.CharacterData.Hat, hatID.ToString() }
            }
        };

        void UpdateHatSuccess(UpdateCharacterDataResult result)
        {
            print(result.ToString());
        }

        void UpdateHatError(PlayFabError error)
        {
            print(error.GenerateErrorReport());
        }

        PlayFabClientAPI.UpdateCharacterData(request, UpdateHatSuccess, UpdateHatError);
    }

    private void AddFriend()
    {
        var request = new AddFriendRequest()
        {
            FriendPlayFabId = friendID
        };

        void AddFriendSuccess(AddFriendResult result)
        {
            print("Friend successfully added.");
        }

        void AddFriendError(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
        }

        PlayFabClientAPI.AddFriend(request, AddFriendSuccess, AddFriendError);
    }

    [ContextMenu("Get friends list")]
    private void GetFriendList()
    {
        var request = new GetFriendsListRequest();

        void GetFriendListSuccess(GetFriendsListResult result)
        {
            foreach (var friend in result.Friends)
            {
                print($"Friend ID: {friend.FriendPlayFabId}, username: {friend.Username}");
                GetFriendCoins(friend.FriendPlayFabId);
            }
        }

        void GetFriendListError(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
        }

        PlayFabClientAPI.GetFriendsList(request, GetFriendListSuccess, GetFriendListError);
    }

    private void DeleteFriend()
    {

    }

    private void GetStatisticsPointsAndUpdate()
    {
        List<string> points = new List<string>
        {
            PlayfabConsts.Statistics.Points
        };

        var request = new GetPlayerStatisticsRequest()
        {
            StatisticNames = points
        };

        void GetStatisticsPointsSuccess(GetPlayerStatisticsResult result)
        {
            foreach (var points in result.Statistics)
            {
                GameManager.Instance.TotalPoints = points.Value;
                print($"Current points: {GameManager.Instance.TotalPoints}");
                AddPoints();
            }
        }

        void GetStatisticsPointsError(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
        }

        PlayFabClientAPI.GetPlayerStatistics(request, GetStatisticsPointsSuccess, GetStatisticsPointsError);
    }

    public void GetStatisticsPoints()
    {
        List<string> points = new List<string>
        {
            PlayfabConsts.Statistics.Points
        };

        var request = new GetPlayerStatisticsRequest()
        {
            StatisticNames = points
        };

        void GetStatisticsPointsSuccess(GetPlayerStatisticsResult result)
        {
            foreach (var points in result.Statistics)
            {
                GameManager.Instance.TotalPoints = points.Value;
                print($"Current points: {GameManager.Instance.TotalPoints}");
            }
        }

        void GetStatisticsPointsError(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
        }

        PlayFabClientAPI.GetPlayerStatistics(request, GetStatisticsPointsSuccess, GetStatisticsPointsError);
    }

    [ContextMenu("Update Statistics")]
    private void AddPoints()
    {
        print($"Total points: {GameManager.Instance.LevelPoints}");
        List<StatisticUpdate> statistics = new List<StatisticUpdate>();
        statistics.Add(new StatisticUpdate()
        {
            StatisticName = PlayfabConsts.Statistics.Points,
            Value = (GameManager.Instance.LevelPoints + GameManager.Instance.TotalPoints)
        });

        var request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = statistics
        };

        void UpdatePlayerStatisticsSuccess(UpdatePlayerStatisticsResult result)
        {
            print("The Points statistics were successfully updated.");
        }

        void UpdatePlayerStatisticsError(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
        }

        PlayFabClientAPI.UpdatePlayerStatistics(request, UpdatePlayerStatisticsSuccess, UpdatePlayerStatisticsError);
    }

    public void UpdatePoints()
    {
        GetStatisticsPointsAndUpdate();
    }

    [ContextMenu("Get Leader Board")]
    public void FetchLeaderBoard()
    {
        var request = new GetLeaderboardRequest()
        {
            StartPosition = 0, //Usar variable de preferencia, si quieremos paginar
            StatisticName = PlayfabConsts.Statistics.Points,
            MaxResultsCount = 10, //Opcional, pero recomendado 
        };

        void GetLeaderBoardSuccess(GetLeaderboardResult result)
        {
            foreach (var leader in result.Leaderboard)
            {
                print($"User ID {leader.PlayFabId} has: {leader.StatValue} points.");
                if (GameManager.Instance.LeaderBoard.ContainsKey(leader.PlayFabId))
                {
                    GameManager.Instance.LeaderBoard[leader.PlayFabId] = leader.StatValue;
                }
                else
                {
                    GameManager.Instance.LeaderBoard.Add(leader.PlayFabId, leader.StatValue);
                }
            }
        }

        void GetLeaderBoardError(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
        }

        PlayFabClientAPI.GetLeaderboard(request, GetLeaderBoardSuccess, GetLeaderBoardError);

    }

}
