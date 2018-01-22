using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TestRandom2 : MonoBehaviour {

    public int nbrVille;
    public int widthMax = 4;
    public int hightMax = 5;

    public int hight = 1;
    public int width = 1;

    public GameObject cityPrefab;
    public GameObject citiesParent;
    public GameObject linePrefab;
    public GameObject linesParent;
    public List<CityV2> cities;
    List<CityV2> citiesEdge;

    float separation = 3;

    private void Start()
    {
        generationPlateau1();
    }

    public void generationPlateau1()
    {
        cities = new List<CityV2>();
        CityV2 city = Instantiate(cityPrefab).GetComponent<CityV2>();
        city.gameObject.transform.SetParent(citiesParent.transform);
        CityV2 linkCity;
        LineController linkController;
        cities.Add(city);
        citiesEdge = new List<CityV2>() { cities[0] };
        int countVille = 1;
        int link;
        int lineNewCity;
        int colNewCity;

        while (countVille < nbrVille)
        {
            int indexCity = Random.Range(0, cities.Count - 1);
            city = cities[indexCity];
            link = city.neighboors.buildALink();
            lineNewCity = city.line;
            colNewCity = city.column;
            getCoordonateFromLink(ref lineNewCity, ref colNewCity, link);
            linkCity = CityIsExiting(lineNewCity, colNewCity);
            


            if (!linkCity)
            {

                linkCity = Instantiate(cityPrefab).GetComponent<CityV2>();
                linkCity.gameObject.transform.SetParent(citiesParent.transform);
                linkCity.gameObject.transform.position = new Vector3(colNewCity * separation + separation, 0, lineNewCity * separation + separation);

                linkCity.setCoordonate(lineNewCity, colNewCity);

                

                citiesEdge.Add(linkCity);
                cities.Add(linkCity);
                countVille++;
            }
            linkController = createLine(city.gameObject, linkCity.gameObject);

            city.neighboors.addCityLink(linkController, link);
            city.neighboors.addCityLinkInverse(linkController, link);

            if (city.neighboors.nbrLink == 8)
            {
                citiesEdge.Remove(city);
            }
            if (linkCity.neighboors.nbrLink == 8)
            {
                citiesEdge.Remove(linkCity);
            }
            //return;

        }
        
    }

    public void getCoordonateFromLink(ref int line, ref int column, int link)
    {
        switch (link)
        {
            case 0:
                line++;
                break;
            case 1:
                line++;
                column++;
                break;
            case 2:
                column++;
                break;
            case 3:
                line--;
                column++;
                break;
            case 4:
                line--;
                break;
            case 5:
                line--;
                column--;
                break;
            case 6:
                column--;
                break;
            case 7:
                line++;
                column--;
                break;
            default:
                break;
        }
    }

    public CityV2 CityIsExiting(int line, int column)
    {
        int i = 0;
        while (i < citiesEdge.Count && !citiesEdge[i].isGoodCity(line, column))
        {
            i++;
        }
        return (i != citiesEdge.Count) ? citiesEdge[i] : null;
    }

    public LineController createLine(GameObject city, GameObject linkCity)
    {
        GameObject line = Instantiate(linePrefab);
        line.transform.SetParent(linesParent.transform);
        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
        Vector3 posCity = city.transform.position;
        lineRenderer.SetPosition(0, posCity);

        lineRenderer.SetPosition(1, linkCity.transform.position);

        line.GetComponent<LineController>().addCity(city.GetComponent<CityV2>(), linkCity.GetComponent<CityV2>());

        return line.GetComponent<LineController>();
    }
}
