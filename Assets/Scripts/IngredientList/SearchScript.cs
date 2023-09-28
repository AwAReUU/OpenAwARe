using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SearchScript : MonoBehaviour
{
    public GameObject ContentHolder;
    public GameObject[] Elements;
    public GameObject SearchBar;
    public int totalElements;

    // Start is called before the first frame update
    void Start()
    {
        totalElements = ContentHolder.transform.childCount;
        Elements = new GameObject[totalElements];
        for (int i = 0; i < totalElements; i++) 
        {
            Elements[i] = ContentHolder.transform.GetChild(i).gameObject;
        }
    }

    public void Search() 
    {
        string searchText = SearchBar.GetComponent<TMP_InputField>().text;
        int searchTextLength = searchText.Length;

        for(int i = 0; i < Elements.Length; i++)
        {
            GameObject elem = Elements[i];
            if (elem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text.Length >= searchTextLength) 
            {
                if (searchText == elem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text.Substring(0, searchTextLength).ToLower())
                    elem.SetActive(true);
                else
                    elem.SetActive(false);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
