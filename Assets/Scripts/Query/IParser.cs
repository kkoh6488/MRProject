using UnityEngine;
using System.Collections;

public interface IParser<T> {

    T[] ParseGroup(string s);

    T ParseSingle(string s);

}
