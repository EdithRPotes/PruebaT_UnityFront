using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Unity.VisualScripting;



//maneja las peticiones y  deserealiza los datos de los personajes 

[System.Serializable]
public class Character 
{
    public int id;
    public string name;
    public string status;
    public string species;
    public string gender;
    public Origin origin;
    public Location location;
    public string image;
}

[System.Serializable]
public class Origin
{
    public string name;
}


[System.Serializable]
public class Location 
{
    public string name;
}

[System.Serializable]
public class CharacterResponse
{
   
    public List<Character> results;
    public Info info; // info de total de paginas 


}

[System.Serializable]
public class Info 
{
   
    public int pages; // Numero Total de Paginas 
}


public class APIHandler : MonoBehaviour
{
    private const string API_URL = "https://rickandmortyapi.com/api/character/?page=";

    public TMP_Text errorText; // Muestra los Mensajes de Error 
    public IEnumerator GetCharacters(int page, System.Action<List<Character>, int> callback) 
    { 
        UnityWebRequest request =  UnityWebRequest.Get(API_URL + page);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success) 
        {
            HandleError<List<Character>>($"Error al obtener Personajs: {request.error}", callback);
        }
        else
        {
            try
            {
                string jsonResponse = request.downloadHandler.text;
                CharacterResponse response = JsonUtility.FromJson<CharacterResponse>(jsonResponse);
                callback(response.results, response.info.pages);
            }
            catch (System.Exception e)
            {
                HandleError<List<Character>>($"Error al obtener Personajs: {e.Message}", callback);

            }
           
        }
    
    }

    //Metodo para descargar la imagen del personaje 
    public IEnumerator GetCharacterImage(string url, System.Action<Texture2D> callback) 
    { 
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();


        if (request.result != UnityWebRequest.Result.Success) 
        {
            HandleError($"Error al obtener Personajs: {request.error}", callback);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            callback(texture);
        }
    
    }

    private void HandleError<T>(string errorMessage, System.Action<List<Character>, int> callback) 
    { 
        Debug.Log(errorMessage);
        errorText.text = "Ups! Algo salio mal. Intente de nuevo."; // Mensaje al usuario
        callback?.Invoke(null, 0);
    }

    private void HandleError(string errorMessage, System.Action<Texture2D> callback)
    {
        Debug.Log(errorMessage);
        errorText.text = "Ups! Algo salió mal. Intente de nuevo."; // Mensaje al usuario
        callback?.Invoke(null);
    }
}
