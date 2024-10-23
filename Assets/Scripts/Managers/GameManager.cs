using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class GameManager : MonoBehaviour
{
    //Controla el flujo del juego y muestra los personajes en la interfaz 

    #region Varibles 
    public APIHandler apiHandler; //Enlaza al APIHandler
    public Transform contentPanel; // Contenedor de la lista de personajes 
    public GameObject characterItemPrefab;
    public GameObject card3D; // Referencia al Panel de la carta 3D 
    public TMP_Text cardFrontText; //Texto dentro de la carta 3D

    //Camara secundaria 
    public Camera secondaryCamera; // Camara secundaria para la carta 
    public Canvas mainCanvas; // Canvas principal para control de visibilidad

    //public CharacterDetails characterDetailsPanel;

    public int currentPage = 1; // pagina inicial 
    public int totalPages = 0; // se actualiza segun la api 

    //Referencias a los botones de navegacion y el indicador de pagina 
    public Button nextButton;
    public Button previousButton;
    public TMP_Text pageIndicador; // Texto que indica la pagina actual
    public TMP_Dropdown pageDropdown; // Dropdown para seleccionar las paginas directamente 
    public TMP_Text errorText; // Texto para mensajes de error
    public CardRotation cardRotation;


    #endregion Metodos

    void Start()
    {
        SetupPageDropdown(); 
        LoadCharacters(currentPage);

        card3D.SetActive(false); // La carta inicia desactivada 
        secondaryCamera.gameObject.SetActive(false); 
    }


    void Update()
    {
        // Verifica si se presiona la tecla Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();  // Llama al método para cerrar el juego
        }
    }


    #region Methods 
    public void QuitGame()
    {
        Debug.Log("El juego se ha cerrado.");
        Application.Quit();  // Cierra la aplicación

       // asegura que funcione en el editor
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void LoadCharacters(int page) 
    {
        errorText.text = ""; // Limpia mensajes de error previos
        StartCoroutine(apiHandler.GetCharacters(page, OnCharactersReceived));
    }


    //Callback
    private void OnCharactersReceived(List<Character> characters, int totalPageCount) 
    {

        if (characters == null || characters.Count == 0) 
        {
            errorText.text = "No se pudieron cargar los personajes. Inténtalo de nuevo.";
            //Debug.LogError("No se recibieron personajes");
            return;
        }

        //Actualiza el total de paginas 
        totalPages = totalPageCount;

        //Debug.Log($"Personajes recibidos: {characters.Count}");

        //Limpia el panel antes de anadir los nuevos personajes
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        //Instancia la lista de personajes recibidos
        foreach (Character character in characters) 
        { 
            GameObject item = Instantiate(characterItemPrefab,contentPanel);
            item.GetComponent<CharacterItem>().Setup(character, this);
        }

        // Actualiza la interfaz de navegacion 
        UpdateNavigationUI();
    }

    #region Metodo Control de carta 3D

    public void ShowCharacterDetails(Character character) 
    {
        // Reinicia la rotación cada vez que se muestra la carta
        cardRotation.ResetRotation();

        //Muestra la carta 3D 
        card3D.SetActive(true);
        secondaryCamera.gameObject.SetActive(true);
        mainCanvas.gameObject.SetActive(false); // Desactiva el canvas principal 

        //Actualiza el texto de la carta con los detalles del personaje 
        cardFrontText.text = $"Nombre: {character.name}\n" +
                             $"Estado: {character.status}\n" +
                             $"Especie:{character.species}\n" +
                             $"Origen: {character.origin.name}\n" +
                             $"Ultima Locacion: {character.location.name}";

        
        // Inicia la rotación controlada
        cardRotation.StartRotation();

        // la camara secundaria apunta a la carta 
        secondaryCamera.transform.LookAt(card3D.transform.position);
    }
    #endregion

    #region Metodo Carta 3D 
    public void HideCard3D() 
    {
        //Desactiva la carta 3D y la camara secundaria y vuelve al canvas principal 
        card3D.SetActive(false);
        secondaryCamera.gameObject.SetActive(false);
        mainCanvas.gameObject.SetActive(true);
    
    }

    #endregion
 
    #region Metodos Botones Navegacion de paginas 
    public void NextPage() 
    {
        if (currentPage < totalPages) // avanza solo si no se esta en la ultima pagina  
        {
            currentPage++;
            LoadCharacters(currentPage);
            UpdateNavigationUI();
        }
    
    }

    public void PreviousPage() 
    {

        if (currentPage > 1) // Retrocede solo si  no se esta en la primera pagina0  
        { 
            currentPage--;
            LoadCharacters(currentPage);
            UpdateNavigationUI();
        }
    
    }

    #region Metodo que actualiza la interfaz de navegacion 
    private void UpdateNavigationUI() 
    { 
        // Desactiva los botones cuando se esta en los limites de las paginas 
        previousButton.interactable = currentPage > 1;  
        nextButton.interactable = currentPage < totalPages;

        //Actualiza el texto del indicador de paginas 
        pageIndicador.text = $"Página {currentPage} / {totalPages}";

        //Actualiza la seleccion en el Dropdown solo si no hay conflicto con la navegacion 
        if (pageDropdown != null && pageDropdown.value != currentPage - 1) 
            pageDropdown.value = currentPage - 1;
    }

    #endregion


    #region Metodo para la seleccion directa de la pagina 
    private void SetupPageDropdown() 
    { 
    
        pageDropdown.ClearOptions();
        List<string> options = new List<string>();

        //se muestran las 42 Paginas 
        for (int i = 1; i <= 42; i++) 
        {
            options.Add($"Página {i}");
        }

        pageDropdown.AddOptions(options);
        pageDropdown.onValueChanged.AddListener(OnPageSelected);

        //Actualiza la seleccion inicial
        pageDropdown.value = currentPage - 1;
    
    }

    public void OnPageSelected(int pageIndex) 
    { 
        currentPage = pageIndex + 1; // se ajusta el indice para que empiece en 1 y no en 0
        LoadCharacters(currentPage); // carga los personajes de la nueva pagina 
        UpdateNavigationUI(); //Actualiza la UI 
    
    }
    #endregion
    #endregion
    #endregion
}
