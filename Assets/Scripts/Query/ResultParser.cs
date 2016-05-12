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
            return results;
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
            string id = obj.Value<string>("@id");
            string name = obj.Value<string>("name");
            string description = "", longDescr = "", imgUrl = "", wikiUrl = "";
            if (obj["description"] != null)
            {
                description = obj.Value<string>("description");
            }
            
            if (obj["detailedDescription"] != null)
            {
                if (obj["detailedDescription"]["articleBody"] != null)
                {
                    longDescr = obj.Value<JToken>("detailedDescription").Value<string>("articleBody");
                }
                if (obj["detailedDescription"]["url"] != null)
                {
                    wikiUrl = obj.Value<JToken>("detailedDescription").Value<string>("url");
                }
            }
            if (obj["image"] != null)
            {
                if (obj["image"]["contentUrl"] != null)
                {
                    imgUrl = obj["image"].Value<string>("contentUrl");
                }
            }
            return new QueryResult(id, name, description, longDescr, imgUrl, wikiUrl, score);
            
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
