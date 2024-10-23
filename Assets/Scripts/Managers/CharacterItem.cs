using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class CharacterItem : MonoBehaviour
{
    #region Variables
    public TMP_Text characterNameText; // Referencia al TMP_Text para el nombre del Personaje
    public RawImage characterImage; // Referencia al  Rawimage del pesonaje 
    public Button detailsButton; // Referencia al boton DetailsButtons

    private Character characterData; // Almacena los datos del Personaje actual
    private GameManager gameManager; // Referencia al GameManager
    #endregion

#region Metodos

    //configura los datos del personaje y los enlaza al boton
    public void Setup(Character character, GameManager manager) 
    { 
        characterData = character; // Almacena los datos del personaje
        gameManager = manager;    // Asocia el GameManager 


        Debug.Log($"Configurando: {character.name}"); // Verfica si llego el nombre 
        //Configura la UI con el nombre e imagen del personaje 
        characterNameText.text = character.name;

        //Inicia la descarga de la imagen desde la URL del personaje 
        StartCoroutine(LoadCharacterImage(character.image));

        //Vincula el boton al metodo para mostrar los detalles 
        detailsButton.onClick.AddListener(() => gameManager.ShowCharacterDetails(characterData));
    
    }

    //Corrutina para descargar la imagen  desde la URL del personaje
    private IEnumerator LoadCharacterImage(string url) 
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError ) 
        {

            Debug.LogError($"Error al cargar la imagen: {request.error} ");
            characterNameText.text += "Imagen no disponible";
        }
        else 
        {
            //Asigna la textura descargada directamente al RawImage
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            characterImage.texture = texture;
        
        }
    
    }

#endregion

}
