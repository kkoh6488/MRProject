using Newtonsoft.Json.Linq;
using UnityEngine;
using System.Collections;
using System;

public class ResultParser : IParser<QueryResult>
{
    public QueryResult[] ParseGroup(string s)
    {
        //Debug.Log("Parsing. " + s);
        try
        {
            JArray joResponse = JArray.Parse(s);
            QueryResult[] results = new QueryResult[joResponse.Count()];
            for (int i = 0; i < joResponse.Count(); i++)
            {
                JToken result = joResponse[i];
                float score = float.Parse(result["resultScore"].ToString());
                results[i] = jsonToResult(result["result"], score);
            }
            
            foreach (QueryResult q in results)
            {
                Debug.Log(q.ToString());
            }
            //JObject first = (JObject)joResponse[0];
            //QueryResult firstResult = jsonToResult((JObject) first["result"], score);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
        }
        return null;
    }

    private QueryResult jsonToResult(JToken obj, float score)
    {
        try
        {
            //Debug.Log("Parsing JToken:" + obj.ToString());
            string name = obj["name"].ToString();
            string description = "", longDescr = "", imgUrl = "", wikiUrl = "";
            if (obj["description"] != null)
            {
                description = obj["description"].ToString();
            }
            
            if (obj["detailedDescription"] != null)
            {
                if (obj["detailedDescription"]["articleBody"] != null)
                {
                    longDescr = obj["detailedDescription"]["articleBody"].ToString();
                }
                if (obj["detailedDescription"]["url"] != null)
                {
                    wikiUrl = obj["detailedDescription"]["url"].ToString();
                }
            }
            if (obj["image"] != null)
            {
                if (obj["image"]["contentUrl"] != null)
                {
                    imgUrl = obj["image"]["contentUrl"].ToString();
                }
            }
            return new QueryResult(name, description, longDescr, imgUrl, wikiUrl, score);
            
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
            return null;
        }
    }

    public QueryResult ParseSingle(string s)
    {
        throw new NotImplementedException();
    }
}
