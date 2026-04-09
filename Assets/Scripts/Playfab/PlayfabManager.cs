//using PlayFab;
//using PlayFab.ClientModels;
//using UnityEngine;
//using TMPro;

//public class PlayfabManager : MonoBehaviour
//{

//    [SerializeField] private TMP_InputField usernameInput;
//    [SerializeField] private TMP_InputField passwordInput;
//    private void Start()
//    {
        
//        if(PlayFabSettings.DeveloperSecretKey == null)
//        {
//            PlayFabSettings.DeveloperSecretKey = "KRADQU1W7XIXDJ17IUIKHX1RF63C6NOREQDZWSBUATUWSFA3TY";
//        }

//        if (PlayFabSettings.TitleId == null)
//        {
//            PlayFabSettings.TitleId = "96019";
//        }

//    }

//    public void CreatePlayfabAccount()
//    {
//        RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest()
//        {
//            Username = usernameInput.text.ToLower(),
//            DisplayName = "",
//            Password = "",
//        };

//        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterPlayerSuccess, OnError);

//    }

//    private void OnRegisterPlayerSuccess(RegisterPlayFabUserResult result)
//    {
        
//    }

//    private void OnError(PlayFabError error)
//    {
       
//    }

//}
