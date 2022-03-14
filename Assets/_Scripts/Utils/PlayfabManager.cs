using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private int currentKills;

    private void Awake()
    {
        _instance = this;
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

    public void UpdateKills(int _killsToAdd)
    {
        GetKills(_killsToAdd);
    }

    private void GetKills(int _killsToAdd)
    {
        var request = new GetUserDataRequest()
        {
            
        };

        void GetUserDataSuccess(GetUserDataResult result)
        {
            if (result.Data.ContainsKey(PlayfabConsts.UserData.Kills))
            {
                print($"El jugador tiene: {result.Data[PlayfabConsts.UserData.Kills].Value} kills");
                currentKills = int.Parse( result.Data[PlayfabConsts.UserData.Kills].Value); 
                AddKills(_killsToAdd);
            }
            else
            {
                GiveCoinsStarterPack();
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
                { PlayfabConsts.UserData.Kills , Convert.ToString(currentKills + _killsToAdd) }
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

    [ContextMenu("Update Statistics")]
    private void UpdatePoints()
    {
        List<StatisticUpdate> statistics = new List<StatisticUpdate>();
        statistics.Add(new StatisticUpdate()
        {
            StatisticName = PlayfabConsts.Statistics.Points,
            Value = 10
        });

        var request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = statistics
        };

        void UpdatePlayerStatisticsSuccess(UpdatePlayerStatisticsResult result)
        {
            print("The statistics were successfully updated.");
        }

        void UpdatePlayerStatisticsError(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
        }

        PlayFabClientAPI.UpdatePlayerStatistics(request, UpdatePlayerStatisticsSuccess, UpdatePlayerStatisticsError);
    }

    [ContextMenu("Get Leader Board")]
    private void FetchLeaderBoard()
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
            }
        }

        void GetLeaderBoardError(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
        }

        PlayFabClientAPI.GetLeaderboard(request, GetLeaderBoardSuccess, GetLeaderBoardError);

    }

}
