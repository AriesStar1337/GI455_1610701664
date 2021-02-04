using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputSearch : MonoBehaviour
{
    public List<string> dataList;
    public Text dataDisplay;
    public InputField searchText;
    public Text searchResult;

    void Start()
	{
		for (int i = 0; i < this.dataList.Count; i++)
		{
			dataDisplay.text += dataList[i] + "\n";
		}
	}

    public void OnDataCheck()
    {
        for(int i = 0 ; i < dataList.Count ; i++)
        {
            if(dataList[i] == searchText.text)
            {
                searchResult.text = "[ <color=green>" + this.searchText.text + "</color> ] is found.";
            }
            else
            {
                searchResult.text = "[ <color=red>" + this.searchText.text + "</color> ] is not found.";
            }
        }
    }
}
